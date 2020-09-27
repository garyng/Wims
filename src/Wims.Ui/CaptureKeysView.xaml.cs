using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;

namespace Wims.Ui
{
	/// <summary>
	/// Interaction logic for KeysCaptureControl.xaml
	/// </summary>
	public partial class CaptureKeysView : ReactiveUserControl<CaptureKeysViewModel>
	{
		public CaptureKeysView()
		{
			InitializeComponent();
			this.WhenActivated(d =>
			{
				this.OneWayBind(ViewModel, vm => vm.Keys, v => v.Keys.Text)
					.DisposeWith(d);

				Observable.Merge(
						this.Events().PreviewKeyDown.DistinctUntilChanged().Select(key => (key, keydown: true, keyup: false)),
						this.Events().PreviewKeyUp.DistinctUntilChanged().Select(key => (key, keydown: false, keyup: true)))
					.Where(e => !e.key.IsRepeat)
					.Subscribe(e =>
					{
						e.key.Handled = true;
					})
					.DisposeWith(d);

				//public static Key RealKey(this KeyEventArgs e)
				//{
				//	switch (e.Key)
				//	{
				//		case Key.System:
				//			return e.SystemKey;

				//		case Key.ImeProcessed:
				//			return e.ImeProcessedKey;

				//		case Key.DeadCharProcessed:
				//			return e.DeadCharProcessedKey;

				//		default:
				//			return e.Key;
				//	}
				//}
			});
		}
	}
}
