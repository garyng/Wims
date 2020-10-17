using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace Wims.Ui.Converters
{
	/// <summary>
	/// Converts a <see cref="PackIcon" /> to an DrawingImage.
	/// Use the ConverterParameter to pass a Brush.
	/// </summary>
	public class PackIconToImageConverter : MarkupExtension, IValueConverter
	{
		private static Lazy<PackIconToImageConverter> _instance =
			new Lazy<PackIconToImageConverter>(() => new PackIconToImageConverter());

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return _instance.Value;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is PackIcon icon)) return value;

			GeometryDrawing geoDrawing = new GeometryDrawing();

			geoDrawing.Brush = parameter as Brush ?? Brushes.Black;
			geoDrawing.Pen = new Pen(geoDrawing.Brush, 0.25);
			geoDrawing.Geometry = Geometry.Parse(icon.Data);

			var drawingGroup = new DrawingGroup {Children = {geoDrawing}, Transform = new ScaleTransform(1, -1)};

			return new DrawingImage {Drawing = drawingGroup};
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}