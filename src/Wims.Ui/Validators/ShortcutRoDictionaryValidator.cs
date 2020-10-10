using System.Collections.Generic;
using FluentValidation;
using Wims.Core.Models;

namespace Wims.Ui.Validators
{
	public class ShortcutRoDictionaryValidator : AbstractValidator<IDictionary<string, ShortcutRo>>
	{
		public ShortcutRoDictionaryValidator()
		{
			RuleForEach(x => x)
				.OverrideIndexerWithDictionaryKey()
				.Transform(x => x.Value)
				.UseFullPropertyName()
				.NotNull()
				.SetValidator(new ShortcutRoValidator())
				.OverridePropertyName("");
		}
	}
}