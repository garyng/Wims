using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Wims.Ui.Controls
{
	/// <summary>
	/// Like <see cref="Range"/> but <see cref="Start"/> is always less than or equal to <see cref="End"/>
	/// </summary>
	public class OrderedRange
	{
		public int Start { get; }
		public int End { get; }

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

		public static IEnumerable<OrderedRange> GetGaps(this IEnumerable<OrderedRange> @this, int? max = null)
		{
			var ranges = @this.ToList();
			var gaps = ranges.Zip(ranges.Skip(1))
				.Select(pair => new OrderedRange(pair.First.End, pair.Second.Start))
				.ToList();

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

	public class TextBlockHighlighter : DependencyObject
	{
		public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.RegisterAttached(
			"HighlightBrush", typeof(Brush), typeof(TextBlockHighlighter),
			new PropertyMetadata(default(Brush), UpdateHighlight));

		public static void SetHighlightBrush(DependencyObject element, Brush value)
		{
			element.SetValue(HighlightBrushProperty, value);
		}

		public static Brush GetHighlightBrush(DependencyObject element)
		{
			return (Brush) element.GetValue(HighlightBrushProperty);
		}

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
			"Ranges", typeof(List<OrderedRange>), typeof(TextBlockHighlighter),
			new PropertyMetadata(new List<OrderedRange>(), UpdateHighlight));

		public static void SetRanges(DependencyObject element, List<Range> value)
		{
			element.SetValue(RangesProperty, value);
		}

		public static List<OrderedRange> GetRanges(DependencyObject element)
		{
			return (List<OrderedRange>)element.GetValue(RangesProperty);
		}

		private static void UpdateHighlight(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!(d is TextBlock tb)) return;

			var text = GetText(tb);
			if (string.IsNullOrEmpty(text)) return;

			var brush = GetHighlightBrush(tb);
			if (brush == null) return;

			var ranges = GetRanges(tb);
			if (ranges?.Count == 0)
			{
				tb.Inlines.Add(new Run(text));
				return;
			}

			var highlights = GetRanges(d).ToList();
			var gaps = highlights.GetGaps(text.Length).ToList();
			var runs = highlights.Select(r => new {range = r, highlight = true})
				.Concat(gaps.Select(r => new {range = r, highlight = false}))
				// todo: will this be too slow?
				.OrderBy(item => item.range.Start)
				.ThenBy(item => item.range.End)
				.Select(item => new Run(text[item.range.Start..item.range.End])
				{
					// FontWeight = item.highlight ? FontWeights.Heavy : tb.FontWeight,
					Foreground = item.highlight ? brush : tb.Foreground
					// Background = r.Highlight ? Brushes.Yellow : tb.Background
				});

			tb.Inlines.AddRange(runs);
		}
	}
}