using System;
using System.Linq;
using Wims.Core.Models;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Wims.Ui.Utils
{
	public class BindingRoConverter : IYamlTypeConverter
	{
		public bool Accepts(Type type)
		{
			return type == typeof(BindingRo);
		}

		public object ReadYaml(IParser parser, Type type)
		{
			var value = parser.Consume<Scalar>().Value;
			var keys = value.Split('+', StringSplitOptions.RemoveEmptyEntries)
				.Select(key => key.Trim());
			return new BindingRo
			{
				Keys = keys.ToArray()
			};
		}

		public void WriteYaml(IEmitter emitter, object value, Type type)
		{
			var binding = (BindingRo) value;
			var keys = string.Join(" + ", binding.Keys);
			emitter.Emit(new Scalar(null, null, keys, ScalarStyle.Any, true, false));
		}
	}
}