using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ReactiveUI;
using SharpVectors.Converters;

namespace Wims.Ui.Converters
{
	public class RxStringToImageSourceConverter : IBindingTypeConverter
	{
		public int GetAffinityForObjects(Type fromType, Type toType)
		{
			if (fromType == typeof(string) && toType == typeof(ImageSource)) return 100;
			return 0;
		}

		public bool TryConvert(object? @from, Type toType, object? conversionHint, out object? result)
		{
			result = @from;

			if (@from is string path)
			{
				var ext = Path.GetExtension(path);
				if (ext?.Equals(".svg") == true)
				{
					result = new SvgImageExtension(path).ProvideValue(null) as ImageSource;
				}
				else
				{
					result = new BitmapImage(new Uri(path));
				}

				return true;
			}

			return false;
		}
	}
}