using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Wims.Core.Dto;

namespace Wims.Ui.Utils
{
	public class SequenceDtoToStringConverter : MarkupExtension, IValueConverter
	{
		private static Lazy<SequenceDtoToStringConverter> _instance =
			new Lazy<SequenceDtoToStringConverter>(() => new SequenceDtoToStringConverter());

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return _instance.Value;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is IEnumerable<ChordDto> sequence)
			{
				return sequence.ToString();
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string sequence)
			{
				return sequence.ToSequenceDto();
			}

			return value;
		}
	}
}