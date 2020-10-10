using System.Collections.Generic;
using FluentValidation;
using FluentValidation.TestHelper;
using NUnit.Framework;
using Wims.Core.Models;
using Wims.Ui.Validators;

namespace Wims.Tests
{
	public class ValidationTestsBase<T, U> where T : AbstractValidator<U>, new()
	{
		protected T _validator;

		[SetUp]
		public void Setup()
		{
			_validator = new T();
		}
	}

	public class MatchRoValidationTests : ValidationTestsBase<MatchRoValidator, MatchRo>
	{
		[Test]
		public void Should_BothBeNull()
		{
			// Arrange
			var match = new MatchRo();

			// Act
			var result = _validator.TestValidate(match);

			// Assert
			result.ShouldHaveValidationErrorFor(x => x.Class);
			result.ShouldHaveValidationErrorFor(x => x.Exe);
		}

		[TestCase(null, "exe")]
		[TestCase("class", null)]
		public void Can_HaveEitherOneBeNull(string @class, string exe)
		{
			// Arrange
			var match = new MatchRo
			{
				Class = @class,
				Exe = exe
			};

			// Act
			var result = _validator.TestValidate(match);

			// Assert
			result.ShouldNotHaveValidationErrorFor(x => x.Class);
			result.ShouldNotHaveValidationErrorFor(x => x.Exe);
		}
	}

	public class ChordRoValidationTests : ValidationTestsBase<ChordRoValidator, ChordRo>
	{
		[Test]
		public void Keys_ShouldNotBe_Empty()
		{
			// Arrange
			var chord = new ChordRo();

			// Act
			var result = _validator.TestValidate(chord);


			// Assert
			result.ShouldHaveAnyValidationError();
		}

		[Test]
		public void Keys_ShouldNotContain_Nulls()
		{
			// Arrange
			var chord = new ChordRo
			{
				Keys = new[] {"a", null, "b"}
			};

			// Act
			var result = _validator.TestValidate(chord);

			// Assert
			result.ShouldHaveAnyValidationError();
		}
	}

	public class ShortcutRoValidationTests : ValidationTestsBase<ShortcutRoValidator, ShortcutRo>
	{
		[Test]
		public void Sequence_ShouldNotBe_Empty()
		{
			// Arrange
			var shortcut = new ShortcutRo();

			// Act
			var result = _validator.TestValidate(shortcut);

			// Assert
			result.ShouldHaveAnyValidationError();
		}

		[Test]
		public void Sequence_ShouldNotHave_Nulls()
		{
			// Arrange
			var shortcut = new ShortcutRo
			{
				Sequence = new SequenceRo()
				{
					new ChordRo
					{
						Keys = new[] {"a", "b"}
					},
					null
				}
			};

			// Act
			var result = _validator.TestValidate(shortcut);

			// Assert
			result.ShouldHaveAnyValidationError();
		}
	}

	public class
		ShortcutRoDictionaryValidationTests : ValidationTestsBase<ShortcutRoDictionaryValidator,
			IDictionary<string, ShortcutRo>>
	{
		[Test]
		public void ShouldNotBeNull_IfKeyIsDefined()
		{
			// Arrange
			var shortcut = new Dictionary<string, ShortcutRo> {{"shortcut 1", null}};

			// Act
			var result = _validator.TestValidate(shortcut);

			// Assert
			result.ShouldHaveAnyValidationError();
		}
	}

	public class
		ContextRoDictionaryValidationTests : ValidationTestsBase<ContextRoDictionaryValidator,
			IDictionary<string, ContextRo>>
	{
		[Test]
		public void ShouldNotBeNull_IfKeyIsDefined()
		{
			// Arrange
			var context = new Dictionary<string, ContextRo> {{"vs", null}};

			// Act
			var result = _validator.TestValidate(context);

			// Assert
			result.ShouldHaveAnyValidationError();
		}
	}

}