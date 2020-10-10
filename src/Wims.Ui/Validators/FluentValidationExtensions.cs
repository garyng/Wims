using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Internal;

namespace Wims.Ui.Validators
{
	public static class FluentValidationExtensions
	{
		public static IRuleBuilderInitialCollection<IDictionary<TKey, TElement>, KeyValuePair<TKey, TElement>>
			OverrideIndexerWithDictionaryKey<TKey, TElement>(
				this IRuleBuilderInitialCollection<IDictionary<TKey, TElement>, KeyValuePair<TKey, TElement>> @this)
		{
			return @this.OverrideIndexer((x, collection, element, index) => $"[{element.Key}]");
		}

		public static TNext UseFullPropertyName<TConfiguration, TNext>(
			this IConfigurable<TConfiguration, TNext> @this) where TConfiguration : PropertyRule
		{
			return @this.Configure(cfg =>
			{
				cfg.MessageBuilder = context =>
				{
					context.MessageFormatter.AppendPropertyName(context.PropertyName);
					return context.GetDefaultMessage();
				};
			});
		}
	}
}