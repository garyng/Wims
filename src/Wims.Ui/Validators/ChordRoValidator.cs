using FluentValidation;
using Wims.Core.Models;

namespace Wims.Ui.Validators
{
	public class ChordRoValidator : AbstractValidator<ChordRo>
	{
		public ChordRoValidator()
		{
			RuleFor(x => x.Keys)
				.UseFullPropertyName()
				.NotEmpty()
				.ForEach(k => k
					.UseFullPropertyName()
					.NotNull());
		}
	}
}