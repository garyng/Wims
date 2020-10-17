using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Wims.Ui.Controls.Highlighter
{
	public class TextBlockHighlighter : DependencyObject
	{
		public static readonly DependencyProperty EnableProperty = DependencyProperty.RegisterAttached(
			"Enable", typeof(bool), typeof(TextBlockHighlighter), new PropertyMetadata(true, UpdateHighlight));

		public static void SetEnable(DependencyObject element, bool value)
		{
			element.SetValue(EnableProperty, value);
		}

		public static bool GetEnable(DependencyObject element)
		{
			return (bool) element.GetValue(EnableProperty);
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
			tb.Inlines?.Clear();

			var text = GetText(tb);
			if (string.IsNullOrEmpty(text)) return;

			var enabled = GetEnable(tb);
			if (!enabled)
			{
				tb.Inlines.Add(new Run(text));
				return;
			}

			var ranges = GetRanges(tb).ToList();
			if (ranges?.Count == 0)
			{
				tb.Inlines.Add(new Run(text));
				return;
			}

			var gaps = ranges.GetGaps(max: text.Length).ToList();
			var runs = ranges.Select(r => new {range = r, highlight = true})
				.Concat(gaps.Select(r => new {range = r, highlight = false}))
				// todo: will this be too slow?
				.OrderBy(item => item.range.Start)
				.ThenBy(item => item.range.End)
				.Select(item => new Run(text[item.range.Start..item.range.End])
				{
					TextDecorations = item.highlight ? TextDecorations.Underline : null
				});

			tb.Inlines.AddRange(runs);
		}
	}
}