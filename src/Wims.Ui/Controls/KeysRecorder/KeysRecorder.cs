using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Wims.Ui.Controls.KeysRecorder
{
	public class KeysRecorder : TextBox
	{
		public static readonly DependencyProperty BackspaceKeyProperty = DependencyProperty.Register(
			"BackspaceKey", typeof(Key), typeof(KeysRecorder), new PropertyMetadata(Key.Back));

		public Key BackspaceKey
		{
			get { return (Key) GetValue(BackspaceKeyProperty); }
			set { SetValue(BackspaceKeyProperty, value); }
		}

		public static readonly DependencyProperty KeysProperty = DependencyProperty.Register(
			"Keys", typeof(List<List<string>>), typeof(KeysRecorder), new PropertyMetadata(new List<List<string>>()));

		public List<List<string>> Keys
		{
			get { return (List<List<string>>) GetValue(KeysProperty); }
			set { SetValue(KeysProperty, value); }
		}

		private CompositeDisposable _disposables;

		public KeysRecorder()
		{
			IsReadOnly = true;
			IsReadOnlyCaretVisible = true;
			_disposables = new CompositeDisposable();

			this.Unloaded += OnUnloaded;

			var down = this.Events()
				.PreviewKeyDown
				.Where(e => !e.IsRepeat || e.Key == BackspaceKey)
				.Select(KeyEventDto.Down);

			var up = this.Events()
				.PreviewKeyUp
				.Where(e => !e.IsRepeat)
				.Select(KeyEventDto.Up);

			var keys = Observable.Merge(down, up);

			var combination = new KeyCombination();

			this.Events().LostKeyboardFocus
				.ObserveOnDispatcher()
				.Subscribe(_ => { combination.End(); })
				.DisposeWith(_disposables);

			keys.Scan(combination, (c, e) => c.Handle(e, BackspaceKey))
				.ObserveOnDispatcher()
				.Subscribe(c =>
				{
					Text = c.ToString();
					Keys = c.ToStrings();
					CaretIndex = Text.Length;
				})
				.DisposeWith(_disposables);
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			_disposables.Dispose();
		}
	}
}