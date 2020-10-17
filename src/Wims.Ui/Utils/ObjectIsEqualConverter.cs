using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Wims.Ui.Utils
{
	public class ObjectIsEqualConverter : MarkupExtension, IValueConverter
	{
		private static Lazy<ObjectIsEqualConverter> _instance =
			new Lazy<ObjectIsEqualConverter>(() => new ObjectIsEqualConverter());

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return _instance.Value;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value?.Equals(parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return new NotSupportedException();
		}
	}
}