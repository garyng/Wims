using System;
using FluentAssertions;
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
			result.Metadata.Should().BeNull();
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
			var action = new Action(() => _deserializer.Deserialize(@"[{ name: ""first"" }, [], { name: ""second"" }]"));

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
			result.Metadata.Should().BeEquivalentTo(expected);

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
	}
}