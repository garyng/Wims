using FluentValidation;
using Wims.Core.Models;

namespace Wims.Ui.Validators
{
	public class ShortcutRoValidator : AbstractValidator<ShortcutRo>
	{
		public ShortcutRoValidator()
		{
			RuleFor(x => x.Sequence)
				.UseFullPropertyName()
				.NotEmpty()
				.ForEach(c => c
					.UseFullPropertyName()
					.NotNull()
					.SetValidator(new ChordRoValidator()));
		}
	}
}