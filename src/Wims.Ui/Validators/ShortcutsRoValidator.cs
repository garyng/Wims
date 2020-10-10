using FluentValidation;
using Wims.Core.Models;

namespace Wims.Ui.Validators
{
	public class ShortcutsRoValidator : AbstractValidator<ShortcutsRo>
	{
		public ShortcutsRoValidator()
		{
			RuleFor(x => x.Contexts)
				.SetValidator(new ContextRoDictionaryValidator());
			RuleFor(x => x.Shortcuts)
				.SetValidator(new ShortcutRoDictionaryValidator());
		}
	}
}