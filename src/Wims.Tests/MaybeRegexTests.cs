using FluentAssertions;
using NUnit.Framework;
using Wims.Core.Dto;

namespace Wims.Tests
{
	public class MaybeRegexTests
	{
		[TestCase("/[a-z]/")]
		[TestCase("/http://www.google.com/")]
		public void Should_BeRegex_IfIsRegex(string str)
		{
			// Arrange
			var pattern = new MaybeRegex(str);

			// Act

			// Assert
			pattern.IsRegex.Should().BeTrue();
		}

		[TestCase("a-z")]
		[TestCase("http://www.google.com/")]
		public void Should_BeString_IfNotRegex(string str)
		{
			// Arrange
			var pattern = new MaybeRegex(str);

			// Act


			// Assert
			pattern.IsRegex.Should().BeFalse();
		}

		[TestCase("/[a-z]{2}/", "az", true)]
		[TestCase("/[a-z]{2}/", "123", false)]
		public void Should_MatchWithRegex_IfIsRegex(string pattern, string input, bool expected)
		{
			// Arrange
			var p = new MaybeRegex(pattern);

			// Act
			var match = p.IsMatch(input);

			// Assert
			match.Should().Be(expected);
		}

		[TestCase("abc", "abc", true)]
		[TestCase("abc", "Abc", false)]
		public void Should_MatchIgnoreCase_IfNotRegex(string pattern, string input, bool expected)
		{
			var p = new MaybeRegex(pattern);

			// Act
			var match = p.IsMatch(input);

			// Assert
			match.Should().BeTrue();
		}
	}
}