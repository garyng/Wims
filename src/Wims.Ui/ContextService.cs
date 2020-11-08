using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using JetBrains.Annotations;
using Vanara.PInvoke;
using Wims.Core.Dto;

namespace Wims.Ui
{
	public class ActiveContext
	{
		private Process _process;

		public HWND Hwnd { get; set; }
		public int Pid { get; set; }
		public string Title { get; set; }
		public string Exe { get; set; }
		public string Class { get; set; }

		public ActiveContext(HWND hwnd, int pid, string title, string exe, string @class)
		{
			Hwnd = hwnd;
			Pid = pid;
			Title = title;
			Exe = exe;
			Class = @class;
		}

		public ActiveContext()
		{
			Hwnd = User32.GetForegroundWindow();
			Pid = GetPid(Hwnd);
			_process = Process.GetProcessById(Pid);
			Title = GetTitle(_process);
			Exe = GetExe(_process);
			Class = GetClass(Hwnd);
		}

		private int GetPid(HWND hwnd)
		{
			User32.GetWindowThreadProcessId(hwnd, out var pid);
			return (int) pid;
		}

		private string GetTitle(Process process)
		{
			return process.MainWindowTitle;
		}

		private string GetExe(Process process)
		{
			return Path.GetFileName(process.MainModule.FileName);
		}

		private string GetClass(HWND hwnd)
		{
			var @class = new StringBuilder(512);
			User32.GetClassName(hwnd, @class, @class.Capacity);
			return @class.ToString();
		}

		public override string ToString()
		{
			return new {hwnd = Hwnd.DangerousGetHandle(), Pid, Title, Exe, Class}.ToString();
		}
	}


	public class ContextService
	{
		private BehaviorSubject<ActiveContext> _context;

		public IObservable<ActiveContext> ActiveContext { get; set; }

		public ContextService()
		{
			_context = new BehaviorSubject<ActiveContext>(null);
			ActiveContext = _context.AsObservable();
		}

		/// <summary>
		/// Update the current context
		/// </summary>
		public void Refresh()
		{
			_context.OnNext(new ActiveContext());
		}

		/// <summary>
		/// Clear current context
		/// </summary>
		public void Clear()
		{
			_context.OnNext(null);
		}

		public bool Match([CanBeNull] MatchDto match)
		{
			var active = _context.Value;
			if (active == null) return false;
			return match?.IsMatch(active.Class, active.Exe) == true;
		}
	}
}