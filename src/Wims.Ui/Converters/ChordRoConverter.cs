using System;
using System.Linq;
using Wims.Core.Models;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Wims.Ui.Converters
{
	public class ChordRoConverter : IYamlTypeConverter
	{
		public bool Accepts(Type type)
		{
			return type == typeof(ChordRo);
		}

		public object ReadYaml(IParser parser, Type type)
		{
			var value = parser.Consume<Scalar>().Value;
			var keys = value.Split('+', StringSplitOptions.RemoveEmptyEntries)
				.Select(key => key.Trim());
			return new ChordRo
			{
				Keys = keys.ToArray()
			};
		}

		public void WriteYaml(IEmitter emitter, object value, Type type)
		{
			var chord = (ChordRo) value;
			var keys = string.Join(" + ", chord.Keys);
			emitter.Emit(new Scalar(null, null, keys, ScalarStyle.Any, true, false));
		}
	}
}