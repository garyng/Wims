using FluentValidation;
using Wims.Core.Models;

namespace Wims.Ui.Validators
{
	public class ContextRoValidator : AbstractValidator<ContextRo>
	{
		public ContextRoValidator()
		{
			RuleFor(x => x.Icon)
				.UseFullPropertyName()
				.NotNull();
			RuleFor(x => x.Match)
				.UseFullPropertyName()
				.SetValidator(new MatchRoValidator());
		}
	}
}