using System.Collections.Generic;
using FluentValidation;
using Wims.Core.Models;

namespace Wims.Ui.Validators
{
	public class ContextRoDictionaryValidator : AbstractValidator<IDictionary<string, ContextRo>>
	{
		public ContextRoDictionaryValidator()
		{
			RuleForEach(x => x)
				.OverrideIndexerWithDictionaryKey()
				.Transform(x => x.Value)
				.UseFullPropertyName()
				.NotNull()
				.SetValidator(new ContextRoValidator())
				.OverridePropertyName("");
		}
	}
}