using FluentValidation;
using Wims.Core.Models;

namespace Wims.Ui.Validators
{
	public class MatchRoValidator : AbstractValidator<MatchRo>
	{
		public MatchRoValidator()
		{
			RuleFor(x => x.Class)
				.UseFullPropertyName()
				.NotNull()
				.When(x => string.IsNullOrEmpty(x.Exe));
			RuleFor(x => x.Exe)
				.UseFullPropertyName()
				.NotNull()
				.When(x => string.IsNullOrEmpty(x.Class));
		}
	}
}