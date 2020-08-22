using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using FluentAssertions;
using KleSharp.Tests.Resources;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using MoreLinq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace KleSharp.Tests
{
	public class DeserializerTests
	{
		private KleDeserializer _deserializer;

		[SetUp]
		public void Setup()
		{
			_deserializer = new KleDeserializer();
		}

		[Test]
		public void Should_Throw_OnNonArray()
		{
			// Arrange

			// Act
			var action = new Action(() => _deserializer.Deserialize("testing"));

			// Assert
			action.Should().Throw<Exception>();
		}

		[Test]
		[Category("Metadata")]
		public void Should_BeNull_IfNo_Metadata()
		{
			// Arrange

			// Act
			var result = _deserializer.Deserialize(@"[]");

			// Assert
			result.Meta.Should().BeNull();
		}

		[Test]
		[Category("Metadata")]
		public void Should_Throw_IfMetadata_Is_NotTheFirstObject()
		{
			// Arrange

			// Act
			var action = new Action(() => _deserializer.Deserialize(@"[[], { name: ""Test"" }]"));

			// Assert
			action.Should().Throw<Exception>();
		}

		[Test]
		[Category("Metadata")]
		public void Should_Throw_IfThereAreMultipleMetadata()
		{
			// Arrange

			// Act
			var action = new Action(() =>
				_deserializer.Deserialize(@"[{ name: ""first"" }, [], { name: ""second"" }]"));

			// Assert
			action.Should().Throw<Exception>();
		}

		[Test]
		[Category("Metadata")]
		public void Should_Deserialize_MetadataCorrectly()
		{
			// Arrange
			var expected = new KeyboardMetadata
			{
				Author = "author",
				Background = new KeyboardBackground
				{
					Name = "background",
					Style = "style"
				}
			};


			// Act
			var result = _deserializer.Deserialize(@"
[
  {
    ""Author"": ""author"",
    ""Background"": {
      ""Name"": ""background"",
      ""Style"": ""style""
    }
  }
]
");

			// Assert
			result.Meta.Should().BeEquivalentTo(expected);
		}

		[Test]
		[Category("Key positions")]
		public void Key_Should_DefaultToZeroZero()
		{
			// Arrange

			// Act
			var result = _deserializer.Deserialize(@"[[ ""1"" ]]");

			// Assert
			result.Keys.Should().HaveCount(1);
			result.Keys[0].X.Should().Be(0);
			result.Keys[0].Y.Should().Be(0);
		}

		[Test]
		[Category("Key positions")]
		public void KeyX_Should_IncrementByTheWidthOfPreviousKey()
		{
			// Arrange
			var json = "[[{ x: 1 }, \"1\", \"2\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(2);
			result[0].X.Should().Be(1);
			result[1].X.Should().Be(result[0].X + result[0].Width);
			result[1].Y.Should().Be(result[0].Y);
		}

		[Test]
		[Category("Key positions")]
		public void KeyY_Should_Increment_WhenANewRowStarts_And_X_Should_ResetToZero()
		{
			// Arrange
			var json = "[[{ y: 1 }, \"1\"], [\"2\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(2);
			result[0].Y.Should().Be(1);
			result[1].X.Should().Be(0);
			result[1].Y.Should().Be(result[0].Y + 1);
		}

		[Test]
		[Category("Key positions")]
		public void Should_Add_X_ToCurrentPosition()
		{
			// Arrange
			var json = "[[\"1\", { x: 1 }, \"2\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(2);
			result[0].X.Should().Be(0);
			result[1].X.Should().Be(2);
		}

		[Test]
		[Category("Key positions")]
		public void Should_Add_Y_ToCurrentPosition()
		{
			// Arrange
			var json = "[[\"1\"], [{ y: 1 }, \"2\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(2);
			result[0].Y.Should().Be(0);
			result[1].Y.Should().Be(2);
		}

		[Test]
		[Category("Key positions")]
		public void Should_Populate_X2Y2_Correctly()
		{
			// Arrange
			var json = "[[{ x: 1, y: 1, x2: 2, y2: 2 }, \"1\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(1);
			result[0].X.Should().NotBe(0);
			result[0].Y.Should().NotBe(0);
			result[0].X2.Should().NotBe(0);
			result[0].Y2.Should().NotBe(0);
		}

		[Test]
		[Category("Key positions")]
		public void Should_Leave_X2Y2_AsZeroIfNotSpecified()
		{
			// Arrange
			var json = "[[{ x: 1, y: 1 }, \"1\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(1);
			result[0].X.Should().NotBe(0);
			result[0].Y.Should().NotBe(0);
			result[0].X2.Should().Be(0);
			result[0].Y2.Should().Be(0);
		}

		[Test]
		[Category("Key sizes")]
		public void Should_ResetKeyWidthToOne()
		{
			// Arrange
			var json = "[[{ w: 5 }, \"1\", \"2\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(2);
			result[0].Width.Should().Be(5);
			result[1].Width.Should().Be(1);
		}

		[Test]
		[Category("Key sizes")]
		public void Should_ResetKeyHeightToOne()
		{
			// Arrange
			var json = "[[{ h: 5 }, \"1\", \"2\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(2);
			result[0].Height.Should().Be(5);
			result[1].Height.Should().Be(1);
		}

		[Test]
		[Category("Key sizes")]
		public void Width2Height2_Should_BeDefaultIfNotSet()
		{
			// Arrange
			var json = "[[{ w: 2, h: 2 }, \"1\", { w: 2, h: 2, w2: 4, h2: 4 }, \"2\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(2);
			result[0].Width2.Should().Be(result[0].Width);
			result[0].Height2.Should().Be(result[0].Height);
			result[1].Width2.Should().NotBe(result[1].Width);
			result[1].Height2.Should().NotBe(result[1].Height);
		}

		[Test]
		[Category("Other properties")]
		public void Should_Reset_Stepped_Homing_Decal_Flags_ToFalse()
		{
			// Arrange
			var json = "[[{ l: true, n: true, d: true }, \"1\", \"2\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(2);
			result[0].Stepped.Should().BeTrue();
			result[0].Nub.Should().BeTrue();
			result[0].Decal.Should().BeTrue();
			result[1].Stepped.Should().BeFalse();
			result[1].Nub.Should().BeFalse();
			result[1].Decal.Should().BeFalse();
		}

		[Test]
		[Category("Other properties")]
		public void Should_Propagate_GhostFlag()
		{
			// Arrange
			var json = "[[\"0\", { g: true }, \"1\", \"2\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(3);
			result[0].Ghost.Should().BeFalse();
			result[1].Ghost.Should().BeTrue();
			result[2].Ghost.Should().BeTrue();
		}

		[Test]
		[Category("Other properties")]
		public void Should_Propagate_ProfileFlag()
		{
			// Arrange
			var json = "[[\"0\", { p: \"DSA\" }, \"1\", \"2\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(3);
			result[0].Profile.Should().BeEmpty();
			result[1].Profile.Should().Be("DSA");
			result[2].Profile.Should().Be("DSA");
		}

		[Test]
		[Category("Other properties")]
		public void Should_Propagate_SwitchMount()
		{
			// Arrange
			var json = "[[\"1\", { sm: \"cherry\" }, \"2\", \"3\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(3);
			result[0].SwitchMount.Should().BeEmpty();
			result[1].SwitchMount.Should().Be("cherry");
			result[2].SwitchMount.Should().Be("cherry");
		}

		[Test]
		[Category("Other properties")]
		public void Should_Propagate_SwitchBrand()
		{
			// Arrange
			var json = "[[\"1\", { sb: \"cherry\" }, \"2\", \"3\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(3);
			result[0].SwitchBrand.Should().BeEmpty();
			result[1].SwitchBrand.Should().Be("cherry");
			result[2].SwitchBrand.Should().Be("cherry");
		}

		[Test]
		[Category("Other properties")]
		public void Should_Propagate_SwitchType()
		{
			// Arrange
			var json = "[[\"1\", { st: \"MX1A-11Nx\" }, \"2\", \"3\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(3);
			result[0].SwitchType.Should().BeEmpty();
			result[1].SwitchType.Should().Be("MX1A-11Nx");
			result[2].SwitchType.Should().Be("MX1A-11Nx");
		}

		[Test]
		[Category("Text color")]
		public void Should_ApplyColorsToAllSubsequentKeys()
		{
			// Arrange
			var json = "[[{ c: \"#ff0000\", t: \"#00ff00\" }, \"1\", \"2\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(2);
			result[0].Color.Should().Be("#ff0000");
			result[1].Color.Should().Be("#ff0000");
			result[0].Default.TextColor.Should().Be("#00ff00");
			result[1].Default.TextColor.Should().Be("#00ff00");
		}

		[Test]
		[Category("Text color")]
		public void Should_Apply_t_ToAllLegends()
		{
			// Arrange
			var json = "[[{ a: 0, t: \"#444444\" }, \"0\\n1\\n2\\n3\\n4\\n5\\n6\\n7\\n8\\n9\\n10\\n11\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(1);
			result[0].Default.TextColor.Should().Be("#444444");
			result[0].TextColor.All(string.IsNullOrEmpty).Should().BeTrue();
		}

		[Test]
		[Category("Text color")]
		public void Should_BeAbleTo_HandleGenericCase()
		{
			// Arrange
			var labels =
				"#111111\n#222222\n#333333\n#444444\n" +
				"#555555\n#666666\n#777777\n#888888\n" +
				"#999999\n#aaaaaa\n#bbbbbb\n#cccccc";

			var json = $"[[{{ a: 0, t: /*colors*/ \"{labels}\" }}, /*labels*/ \"{labels}\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(1);
			result[0].Default.TextColor.Should().Be("#111111");
			for (int i = 0; i < 12; i++)
			{
				// todo: this should be moved into Dto?
				var color = string.IsNullOrEmpty(result[0].TextColor[i])
					? result[0].Default.TextColor
					: result[0].TextColor[i];
				color.Should().Be(result[0].Labels[i]);
			}
		}

		[Test]
		[Category("Text color")]
		public void Should_BeAbleTo_HandleBlanks()
		{
			// Arrange
			var labels =
				"#111111\n__\n#333333\n#444444\n" +
				"__\n#666666\n__\n#888888\n" +
				"#999999\n#aaaaaa\n#bbbbbb\n#cccccc";
			var colors = labels.Replace("__", "");

			var json = $"[[{{ a: 0, t: /*colors*/ \"{colors}\" }}, /*labels*/ \"{labels}\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(1);
			var key = result[0];
			key.Default.TextColor.Should().Be("#111111");
			for (int i = 0; i < 12; i++)
			{
				var color = string.IsNullOrEmpty(result[0].TextColor[i])
					? result[0].Default.TextColor
					: result[0].TextColor[i];
				if (key.Labels[i] == "__")
				{
					color.Should().Be("#111111");
				}
				else
				{
					color.Should().Be(key.Labels[i]);
				}
			}
		}

		[Test]
		[Category("Text color")]
		public void Should_NotResetDefaultColor_IfBlank()
		{
			// Arrange
			var json = "[[{ t: \"#ff0000\" }, \"1\", { t: \"\\n#00ff00\" }, \"2\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(2);
			result[0].Default.TextColor.Should().Be("#ff0000");
			result[1].Default.TextColor.Should().Be("#ff0000");
		}

		[Test]
		[Category("Text color")]
		public void Should_DeleteValuesEqualToDefault()
		{
			// Arrange
			var json = "[[{ t: \"#ff0000\" },\"1\",{ t: \"\\n#ff0000\" },\"\\n2\",{ t: \"\\n#00ff00\" },\"\\n3\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(3);
			result[1].Labels[6].Should().Be("2");
			result[1].TextColor[6].Should().BeNull();
			result[2].Labels[6].Should().Be("3");
			result[2].TextColor[6].Should().Be("#00ff00");
		}

		[TestCase("[[{ r: 45 }, \"1\", \"2\"]]")]
		[TestCase("[[{ rx: 45 }, \"1\", \"2\"]]")]
		[TestCase("[[{ ry: 45 }, \"1\", \"2\"]]")]
		[Category("Rotation")]
		public void Should_NotThrow_IfRotationIsOnFirstKeyInARow(string json)
		{
			// Arrange

			// Act
			var act = new Action(() => _deserializer.Deserialize(json));

			// Assert
			act.Should().NotThrow();
		}

		[TestCase("[[\"1\", { r: 45 }, \"2\"]]")]
		[TestCase("[[\"1\", { rx: 45 }, \"2\"]]")]
		[TestCase("[[\"1\", { ry: 45 }, \"2\"]]")]
		[Category("Rotation")]
		public void Should_Throw_IfRotationIsNotOnFirstKeyInARow(string json)
		{
			// Arrange

			// Act
			var act = new Action(() => _deserializer.Deserialize(json));

			// Assert
			act.Should().Throw<Exception>();
		}

		[Test]
		[Category("Legends")]
		public void Should_BeAbleTo_AlignLegendPositions()
		{
			// Some history, to make sense of this:
			// 1. Originally, you could only have top & botton legends, and they were
			//    left-aligned. (top:0 & bottom:1)
			// 2. Next, we added right-aligned labels (top:2 & bottom:3).
			// 3. Next, we added front text (left:4, right:5).
			// 4. Next, we added the alignment flags that allowed you to move the
			//    labels (0-5) to the centered positions (via checkboxes).
			// 5. Nobody understood the checkboxes.  They were removed in favor of
			//    twelve separate label editors, allowing text to be placed anywhere.
			//    This introduced labels 6 through 11.
			// 6. The internal rendering is now Top->Bottom, Left->Right, but to keep
			//    the file-format unchanged, the serialization code now translates
			//    the array from the old layout to the new internal one.

			// Arrange
			string x = null;
			var expected = new[]
			{
				// @formatter:off
				//       top row   /**/ middle row /**/ bottom row  /**/   front
				new[] {"0","8","2",/**/"6","9","7",/**/"1","10","3",/**/"4","11","5"}, // a=0
				new[] { x ,"0", x ,/**/ x ,"6", x ,/**/ x , "1", x ,/**/"4","11","5"}, // a=1 (center horz)
				new[] { x , x , x ,/**/"0","8","2",/**/ x , x  , x ,/**/"4","11","5"}, // a=2 (center vert)
				new[] { x , x , x ,/**/ x ,"0", x ,/**/ x , x  , x ,/**/"4","11","5"}, // a=3 (center both)

				new[] {"0","8","2",/**/"6","9","7",/**/"1","10","3",/**/ x , "4", x }, // a=4 (center front)
				new[] { x ,"0", x ,/**/ x ,"6", x ,/**/ x , "1", x ,/**/ x , "4", x }, // a=5 (center front+horz)
				new[] { x , x , x ,/**/"0","8","2",/**/ x , x  , x ,/**/ x , "4", x }, // a=6 (center front+vert)
				new[] { x , x , x ,/**/ x ,"0", x ,/**/ x , x  , x ,/**/ x , "4", x }, // a=7 (center front+both)
				// @formatter:on
			};

			for (int a = 0; a < 8; a++)
			{
				Console.WriteLine(a);
				var json = $"[[{{ a: {a} }}, \"0\\n1\\n2\\n3\\n4\\n5\\n6\\n7\\n8\\n9\\n10\\n11\"]]";

				// Act
				var result = _deserializer.Deserialize(json).Keys;

				// Assert
				result.Should().HaveCount(1);
				result[0].Labels.Should().HaveCount(expected[a].Length);
				result[0].Labels.Should().BeEquivalentTo(expected[a], opt => opt.WithStrictOrdering());
			}
		}

		[Test]
		[Category("Font size")]
		public void Should_Handle_f_AtAllAlignments()
		{
			for (int a = 0; a < 8; a++)
			{
				// Arrange
				Console.WriteLine(a);
				var json = $"[[{{ f: 1, a: {a} }}, \"0\\n1\\n2\\n3\\n4\\n5\\n6\\n7\\n8\\n9\\n10\\n11\"]]";

				// Act
				var result = _deserializer.Deserialize(json).Keys;

				// Assert
				result.Should().HaveCount(1);
				result[0].Default.TextSize.Should().Be(1);
				result[0].TextSize.All(s => s == null).Should().BeTrue();
			}
		}

		[Test]
		[Category("Font size")]
		public void Should_Handle_f2_AtAllAlignments()
		{
			for (int a = 0; a < 8; a++)
			{
				// Arrange
				Console.WriteLine(a);
				var json = $"[[{{ f: 1, f2: 2, a: {a} }}, \"0\\n1\\n2\\n3\\n4\\n5\\n6\\n7\\n8\\n9\\n10\\n11\"]]";

				// Act
				var result = _deserializer.Deserialize(json).Keys;
				var key = result[0];

				// Assert
				result.Should().HaveCount(1);
				for (int i = 0; i < 12; i++)
				{
					if (string.IsNullOrEmpty(key.Labels[i]))
					{
						// no text at [i]; TextSize should be null
						key.TextSize[i].Should().BeNull();
					}
					else
					{
						// all labels should be 2, except the first label
						if (key.Labels[i] == "0")
						{
							key.TextSize[i].Should().BeNull();
						}
						else
						{
							key.TextSize[i].Should().Be(2);
						}
					}
				}
			}
		}

		[Test]
		[Category("Font size")]
		public void Should_Handle_fa_AtAllAlignments()
		{
			for (int a = 0; a < 8; a++)
			{
				// Arrange
				var json =
					$"[[{{ f: 1, fa: [2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13], a: {a} }},\"2\\n3\\n4\\n5\\n6\\n7\\n8\\n9\\n10\\n11\\n12\\n13\"]]";

				// Act
				var result = _deserializer.Deserialize(json).Keys;
				var key = result[0];

				// Assert
				result.Should().HaveCount(1);
				for (int i = 0; i < 12; i++)
				{
					if (string.IsNullOrEmpty(key.Labels[i])) continue;
					key.TextSize[i].Should().Be(int.Parse(key.Labels[i]));
				}
			}
		}

		[Test]
		[Category("Font size")]
		public void Should_Handle_BlanksAt_fa()
		{
			for (int a = 0; a < 8; a++)
			{
				// Arrange
				var json =
					$"[[{{ f: 1, fa: [null, 2, null, 4, null, 6, null, 8, 9, 10, null, 12], a: {a} }},\"x\\n2\\nx\\n4\\nx\\n6\\nx\\n8\\n9\\n10\\nx\\n12\"]]";

				// Act
				var result = _deserializer.Deserialize(json).Keys;
				var key = result[0];

				// Assert
				result.Should().HaveCount(1);
				for (int i = 0; i < 12; i++)
				{
					if (key.Labels[i] != "x") continue;
					key.TextSize[i].Should().BeNull();
				}
			}
		}

		[Test]
		[Category("Font size")]
		public void Should_NotResetToDefaultSizeIfBlank()
		{
			// Arrange
			var json = "[[{ f: 1 }, \"1\", { fa: [null, 2] }, \"2\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(2);
			result[0].Default.TextSize.Should().Be(1);
			result[1].Default.TextSize.Should().Be(1);
		}

		[Test]
		[Category("Font size")]
		public void Should_DeleteValuesEqualToDefault_FontSizes()
		{
			// Arrange
			var json = "[[{ f: 1 }, \"1\", { fa: [null, 1] }, \"\\n2\", { fa: [null, 2] }, \"\\n3\"]]";

			// Act
			var result = _deserializer.Deserialize(json).Keys;

			// Assert
			result.Should().HaveCount(3);
			result[1].Labels[6].Should().Be("2");
			result[1].TextSize[6].Should().BeNull();
			result[2].Labels[6].Should().Be("3");
			result[2].TextSize[6].Should().Be(2);
		}

		[Test]
		[Category("String")]
		public void Should_BeLenientAboutQuotes()
		{
			// Arrange
			var json1 = "[{ name: \"Sample\", author: \"Your Name\" },[\"Q\", \"W\", \"E\", \"R\", \"T\", \"Y\"]]";
			var json2 =
				"[{ \"name\": \"Sample\", \"author\": \"Your Name\" },[\"Q\", \"W\", \"E\", \"R\", \"T\", \"Y\"]]";
			var json3 = "[{ name: \"Sample\", author: \"Your Name\" },[\"Q\", \"W\", \"E\", \"R\", \"T\", \"Y\"]]";

			// Act
			var fac = new Func<string, Func<Keyboard>>(json => () => _deserializer.Deserialize(json));
			var act1 = fac(json1);
			var act2 = fac(json2);
			var act3 = fac(json3);


			// Assert
			act1.Should().NotThrow();
			act2.Should().NotThrow();
			act1().Should().BeEquivalentTo(act2());
			act1().Should().BeEquivalentTo(act3());
		}

		[TestCase("ansi-104.json")]
		[TestCase("apple-wireless.json")]
		[TestCase("iso-105.json")]
		[TestCase("programmers-keyboard.json")]
		[Category("Samples")]
		public void Should_DeserializeSamplesCorrectly(string filename)
		{
			// Arrange
			var ns = typeof(ITestResourcesMarker).Namespace;
			var input = ReadResource($"{ns}.Original.{filename}");
			var expected = JsonConvert.DeserializeObject<Keyboard>(ReadResource($"{ns}.Expected.{filename}"));

			foreach (var key in expected.Keys)
			{
				key.TextColor = PadNull(key.TextColor);
				key.Labels = PadNull(key.Labels);
				key.TextSize = PadNull(key.TextSize);
			}

			// Act
			var result = _deserializer.Deserialize(input);


			// Assert
			result.Should().BeEquivalentTo(expected);
		}

		private string ReadResource(string resourceName)
		{
			var assembly = typeof(ITestResourcesMarker).Assembly;
			using var reader = new StreamReader(assembly.GetManifestResourceStream(resourceName));
			return reader.ReadToEnd();
		}

		private T[] PadNull<T>(IList<T> source, int upTo = 12)
		{
			var items = new List<T>(source);
			var diff = Enumerable.Repeat<T>(default, upTo - items.Count);
			items.AddRange(diff);
			return items.ToArray();
		}
	}
}