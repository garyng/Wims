using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Wims.Ui.Controls;

namespace Wims.Tests
{
	public class OrderedRangeTests
	{
		[TestCase(0, 1)]
		[TestCase(2, 2)]
		[TestCase(3, 2)]
		public void Should_StartWithSmaller_EndsWithLarger(int start, int end)
		{
			// Arrange
			var range = new OrderedRange(start, end);

			// Act


			// Assert
			range.Start.Should().BeLessOrEqualTo(range.End);
		}


		[TestCaseSource(nameof(Overlapping))]
		public void Should_CheckOverlaps(OrderedRange a, OrderedRange b, bool expected)
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
				new OrderedRange(0, 1),
				new OrderedRange(0, 2),
				true
			};
			yield return new object[]
			{
				new OrderedRange(0, 2),
				new OrderedRange(0, 1),
				true
			};
			yield return new object[]
			{
				new OrderedRange(0, 4),
				new OrderedRange(2, 5),
				true
			};
			yield return new object[]
			{
				new OrderedRange(2, 5),
				new OrderedRange(0, 4),
				true
			};
			yield return new object[]
			{
				new OrderedRange(0, 5),
				new OrderedRange(1, 2),
				true
			};
			yield return new object[]
			{
				new OrderedRange(1, 2),
				new OrderedRange(0, 5),
				true
			};
			yield return new object[]
			{
				new OrderedRange(0, 3),
				new OrderedRange(5, 7),
				false
			};
		}

		[TestCaseSource(nameof(MergeOverlapping))]
		public void Should_MergeOverlapping(IEnumerable<OrderedRange> ranges,
			IEnumerable<OrderedRange> expected)
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
					new OrderedRange(0, 5),
					new OrderedRange(3, 6),
					new OrderedRange(9, 10),
				},
				new[]
				{
					new OrderedRange(0, 6),
					new OrderedRange(9, 10),
				}
			};
			yield return new object[]
			{
				new[]
				{
					new OrderedRange(3, 6),
					new OrderedRange(0, 5),
					new OrderedRange(9, 10),
				},
				new[]
				{
					new OrderedRange(0, 6),
					new OrderedRange(9, 10),
				}
			};
		}

		[TestCaseSource(nameof(FillGaps))]
		public void Should_FillGaps(IEnumerable<OrderedRange> ranges, IEnumerable<OrderedRange> expected)
		{
			// Arrange


			// Act
			var gaps = ranges.GetGaps();

			// Assert
			gaps.Should().BeEquivalentTo(expected);
		}

		static IEnumerable<object> FillGaps()
		{
			yield return new object[]
			{
				new[]
				{
					new OrderedRange(0, 5),
					new OrderedRange(7, 9),
				},
				new[]
				{
					new OrderedRange(5, 7),
				}
			};
		}


		[TestCaseSource(nameof(FillGapsWithMax))]
		public void Should_FillGaps_WithMax(IEnumerable<OrderedRange> ranges, int max,
			IEnumerable<OrderedRange> expected)
		{
			// Arrange


			// Act
			var gaps = ranges.GetGaps(max: max);

			// Assert
			gaps.Should().BeEquivalentTo(expected);
		}

		static IEnumerable<object> FillGapsWithMax()
		{
			yield return new object[]
			{
				new[]
				{
					new OrderedRange(0, 5),
					new OrderedRange(7, 9),
				},
				9,
				new[]
				{
					new OrderedRange(5, 7),
				}
			};
			yield return new object[]
			{
				new[]
				{
					new OrderedRange(0, 5),
					new OrderedRange(7, 9),
				},
				11,
				new[]
				{
					new OrderedRange(5, 7),
					new OrderedRange(9, 11),
				}
			};
		}

		[TestCaseSource(nameof(FillGapsWithMin))]
		public void Should_FillGaps_WithMin(IEnumerable<OrderedRange> ranges, int min,
			IEnumerable<OrderedRange> expected)
		{
			// Arrange


			// Act
			var gaps = ranges.GetGaps(min: min);

			// Assert
			gaps.Should().BeEquivalentTo(expected);
		}

		static IEnumerable<object> FillGapsWithMin()
		{
			yield return new object[]
			{
				new[]
				{
					new OrderedRange(0, 5),
					new OrderedRange(7, 9),
				},
				0,
				new[]
				{
					new OrderedRange(5, 7),
				}
			};
			yield return new object[]
			{
				new[]
				{
					new OrderedRange(7, 9),
				},
				0,
				new[]
				{
					new OrderedRange(0, 7),
				}
			};
		}
	}
}