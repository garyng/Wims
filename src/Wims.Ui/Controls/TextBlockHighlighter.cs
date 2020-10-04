using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Wims.Ui.Controls
{
	public class HighlighterRange
	{
		public int Start { get; }
		public int End { get; }
		public bool Highlight { get; set; }

		/// <param name="start">Inclusive start index</param>
		/// <param name="end">Exclusive end index</param>
		/// <param name="highlight">Whether to highlight the text</param>
		public HighlighterRange(int start, int end, bool highlight = true)
		{
			Start = Math.Min(start, end);
			End = Math.Max(start, end);
			Highlight = highlight;
		}
	}

	public static class HighlighterRangeExtensions
	{
		public static bool OverlapsWith(this HighlighterRange @this, HighlighterRange other)
		{
			return @this.Start <= other.Start
				? @this.End >= other.Start
				: other.End >= @this.Start;
		}

		public static HighlighterRange Merge(this HighlighterRange @this, HighlighterRange other)
		{
			return new HighlighterRange(
				Math.Min(@this.Start, other.Start),
				Math.Max(@this.End, other.End));
		}

		public static IEnumerable<HighlighterRange> Merge(this IEnumerable<HighlighterRange> @this)
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

		public static IEnumerable<HighlighterRange> FillGaps(this IEnumerable<HighlighterRange> @this, int? max = null,
			Func<HighlighterRange, HighlighterRange> transform = null)
		{
			var result = @this.Zip(@this.Skip(1))
				.SelectMany(pair =>
				{
					var gap = new HighlighterRange(pair.First.End, pair.Second.Start);
					return new[]
					{
						pair.First,
						transform?.Invoke(gap) ?? gap,
						pair.Second
					};
				}).ToList();

			if (max.HasValue)
			{
				var end = result.Last().End;
				if (end < max.Value)
				{
					var last = new HighlighterRange(end, max.Value);
					result.Add(transform?.Invoke(last) ?? last);
				}
			}

			return result;
		}
	}

	public class TextBlockHighlighter : DependencyObject
	{
		public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
			"Text", typeof(string), typeof(TextBlockHighlighter),
			new PropertyMetadata(default(string), UpdateHighlight));

		public static void SetText(DependencyObject element, string value)
		{
			element.SetValue(TextProperty, value);
		}

		public static string GetText(DependencyObject element)
		{
			return (string) element.GetValue(TextProperty);
		}

		public static readonly DependencyProperty RangesProperty = DependencyProperty.RegisterAttached(
			"Ranges", typeof(List<HighlighterRange>), typeof(TextBlockHighlighter),
			new PropertyMetadata(new List<HighlighterRange>(), UpdateHighlight));

		public static void SetRanges(DependencyObject element, List<Range> value)
		{
			element.SetValue(RangesProperty, value);
		}

		public static List<HighlighterRange> GetRanges(DependencyObject element)
		{
			return new List<HighlighterRange>
			{
				new HighlighterRange(0, 2),
				new HighlighterRange(5, 8),
			};
			// todo: convert matching block to ranges
			// return (List<HighlighterRange>) element.GetValue(RangesProperty);
		}

		private static void UpdateHighlight(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!(d is TextBlock tb)) return;

			var text = GetText(tb);
			var ranges = GetRanges(d).FillGaps(text.Length, h =>
			{
				h.Highlight = false;
				return h;
			});
			tb.Inlines.Clear();

			var runs = ranges.Select(r => new Run(text[r.Start..r.End])
			{
				FontWeight = r.Highlight ? FontWeights.Heavy : tb.FontWeight
				// Background = r.Highlight ? Brushes.Yellow : tb.Background
			});
			tb.Inlines.AddRange(runs);
		}
	}
}