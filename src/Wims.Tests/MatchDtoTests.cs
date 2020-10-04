using FluentAssertions;
using NUnit.Framework;
using Wims.Core.Dto;

namespace Wims.Tests
{
	public class MatchDtoTests
	{
		[Test]
		public void Should_AbleTo_HandleNullString()
		{
			// Arrange
			var match = new MatchDto(null, null);

			// Act


			// Assert
			match.Class.Should().BeNull();
			match.Exe.Should().BeNull();
		}

		[Test]
		public void Should_IgnoreNullConditions_WhileMatching()
		{
			// Arrange
			var match = new MatchDto(null, null);

			// Act
			var result = match.IsMatch("class", "exe");

			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public void Should_IgnoreAnyOfNullConditions_WhileMatching()
		{
			// Arrange
			var match = new MatchDto(null, "exe");

			// Act
			var result = match.IsMatch("class", "exe");

			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public void Should_FollowAllNonNullCondition_WhileMatching()
		{
			// Arrange
			var match = new MatchDto("class", "exe");

			// Act
			var result = match.IsMatch("class", "wrong");

			// Assert
			result.Should().BeFalse();
		}

	}
}