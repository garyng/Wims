using System;
using System.Collections.Generic;
using System.Linq;

namespace Wims.Ui.Controls.Highlighter
{
	/// <summary>
	/// Like <see cref="Range"/> but <see cref="Start"/> is always less than or equal to <see cref="End"/>
	/// </summary>
	public class OrderedRange
	{
		public int Start { get; }
		public int End { get; }
		public int Length => End - Start;

		/// <param name="start">Inclusive start index</param>
		/// <param name="end">Exclusive end index</param>
		public OrderedRange(int start, int end)
		{
			Start = Math.Min(start, end);
			End = Math.Max(start, end);
		}
	}

	public static class OrderedRangeExtensions
	{
		public static bool OverlapsWith(this OrderedRange @this, OrderedRange other)
		{
			return @this.Start <= other.Start
				? @this.End >= other.Start
				: other.End >= @this.Start;
		}

		public static OrderedRange Merge(this OrderedRange @this, OrderedRange other)
		{
			return new OrderedRange(
				Math.Min(@this.Start, other.Start),
				Math.Max(@this.End, other.End));
		}

		public static IEnumerable<OrderedRange> Merge(this IEnumerable<OrderedRange> @this)
		{
			var sorted = @this.OrderBy(r => r.Start)
				.ThenBy(r => r.End)
				.ToList();
			var results = sorted.Take(1).ToList();
			@sorted
				.Skip(1)
				.Aggregate(results, (acc, current) =>
				{
					if (acc.Last().OverlapsWith(current))
					{
						acc[^1] = acc.Last().Merge(current);
					}
					else
					{
						acc.Add(current);
					}

					return acc;
				});
			return results;
		}

		public static IEnumerable<OrderedRange> GetGaps(this IEnumerable<OrderedRange> @this, int? min = 0, int? max = null)
		{
			var ranges = @this.ToList();
			var gaps = ranges.Zip(ranges.Skip(1))
				.Select(pair => new OrderedRange(pair.First.End, pair.Second.Start))
				.ToList();

			if (min.HasValue)
			{
				var start = ranges.FirstOrDefault()?.Start;
				if (min.Value < start)
				{
					var firstGap = new OrderedRange(min.Value, start.Value);
					gaps.Insert(0, firstGap);
				}
			}

			if (max.HasValue)
			{
				var end = ranges.Last().End;
				if (end < max.Value)
				{
					var lastGap = new OrderedRange(end, max.Value);
					gaps.Add(lastGap);
				}
			}

			return gaps;
		}
	}

}