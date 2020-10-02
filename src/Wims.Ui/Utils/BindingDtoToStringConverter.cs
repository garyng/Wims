using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Wims.Core.Dto;

namespace Wims.Ui.Utils
{
	public class BindingDtoToStringConverter : MarkupExtension, IValueConverter
	{
		private static Lazy<BindingDtoToStringConverter> _instance =
			new Lazy<BindingDtoToStringConverter>(() => new BindingDtoToStringConverter());

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return _instance.Value;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is IEnumerable<BindingDto> binding)
			{
				return binding.AsString();
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string binding)
			{
				return binding.ToBindingDto();
			}

			return value;
		}
	}
}