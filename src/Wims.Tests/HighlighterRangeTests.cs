using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Wims.Ui;
using Wims.Ui.Controls;

namespace Wims.Tests
{
	public class HighlighterRangeTests
	{
		[TestCase(0, 1)]
		[TestCase(2, 2)]
		[TestCase(3, 2)]
		public void Should_StartWithSmaller_EndsWithLarger(int start, int end)
		{
			// Arrange
			var range = new HighlighterRange(start, end);

			// Act


			// Assert
			range.Start.Should().BeLessOrEqualTo(range.End);
		}


		[TestCaseSource(nameof(Overlapping))]
		public void Should_CheckOverlaps(HighlighterRange a, HighlighterRange b, bool expected)
		{
			// Arrange

			// Act
			var result = a.OverlapsWith(b);

			// Assert
			result.Should().Be(expected);
		}

		static IEnumerable<object> Overlapping()
		{
			yield return new object[]
			{
				new HighlighterRange(0, 1),
				new HighlighterRange(0, 2),
				true
			};
			yield return new object[]
			{
				new HighlighterRange(0, 2),
				new HighlighterRange(0, 1),
				true
			};
			yield return new object[]
			{
				new HighlighterRange(0, 4),
				new HighlighterRange(2, 5),
				true
			};
			yield return new object[]
			{
				new HighlighterRange(2, 5),
				new HighlighterRange(0, 4),
				true
			};
			yield return new object[]
			{
				new HighlighterRange(0, 5),
				new HighlighterRange(1, 2),
				true
			};
			yield return new object[]
			{
				new HighlighterRange(1, 2),
				new HighlighterRange(0, 5),
				true
			};
			yield return new object[]
			{
				new HighlighterRange(0, 3),
				new HighlighterRange(5, 7),
				false
			};
		}

		[TestCaseSource(nameof(MergeOverlapping))]
		public void Should_MergeOverlapping(IEnumerable<HighlighterRange> ranges,
			IEnumerable<HighlighterRange> expected)
		{
			// Arrange


			// Act
			var result = ranges.Merge();

			// Assert
			result.Should().BeEquivalentTo(expected);
		}

		static IEnumerable<object> MergeOverlapping()
		{
			yield return new object[]
			{
				new[]
				{
					new HighlighterRange(0, 5),
					new HighlighterRange(3, 6),
					new HighlighterRange(9, 10),
				},
				new[]
				{
					new HighlighterRange(0, 6),
					new HighlighterRange(9, 10),
				}
			};
			yield return new object[]
			{
				new[]
				{
					new HighlighterRange(3, 6),
					new HighlighterRange(0, 5),
					new HighlighterRange(9, 10),
				},
				new[]
				{
					new HighlighterRange(0, 6),
					new HighlighterRange(9, 10),
				}
			};
		}

		[TestCaseSource(nameof(FillGaps))]
		public void Should_FillGaps(IEnumerable<HighlighterRange> ranges, IEnumerable<HighlighterRange> expected)
		{
			// Arrange


			// Act
			var result = ranges.FillGaps();

			// Assert
			result.Should().BeEquivalentTo(expected);
		}

		static IEnumerable<object> FillGaps()
		{
			yield return new object[]
			{
				new[]
				{
					new HighlighterRange(0, 5),
					new HighlighterRange(7, 9),
				},
				new[]
				{
					new HighlighterRange(0, 5),
					new HighlighterRange(5, 7),
					new HighlighterRange(7, 9),
				}
			};
		}


		[TestCaseSource(nameof(FillGapsWithMax))]
		public void Should_FillGaps_WithMax(IEnumerable<HighlighterRange> ranges, int max,
			IEnumerable<HighlighterRange> expected)
		{
			// Arrange


			// Act
			var result = ranges.FillGaps(max);

			// Assert
			result.Should().BeEquivalentTo(expected);
		}

		static IEnumerable<object> FillGapsWithMax()
		{
			yield return new object[]
			{
				new[]
				{
					new HighlighterRange(0, 5),
					new HighlighterRange(7, 9),
				},
				9,
				new[]
				{
					new HighlighterRange(0, 5),
					new HighlighterRange(5, 7),
					new HighlighterRange(7, 9),
				}
			};
			yield return new object[]
			{
				new[]
				{
					new HighlighterRange(0, 5),
					new HighlighterRange(7, 9),
				},
				11,
				new[]
				{
					new HighlighterRange(0, 5),
					new HighlighterRange(5, 7),
					new HighlighterRange(7, 9),
					new HighlighterRange(9, 11),
				}
			};
		}
	}
}