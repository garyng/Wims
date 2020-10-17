using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharpVectors.Converters;

namespace Wims.Ui.Converters
{
	public class StringToImageSourceConverter : MarkupExtension, IValueConverter
	{
		private static Lazy<StringToImageSourceConverter> _instance =
			new Lazy<StringToImageSourceConverter>(() => new StringToImageSourceConverter());

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return _instance.Value;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string path)
			{
				var ext = Path.GetExtension(path);
				if (ext?.Equals(".svg") == true)
				{
					return new SvgImageExtension(path).ProvideValue(null) as ImageSource;
				}

				return new BitmapImage(new Uri(path));
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}