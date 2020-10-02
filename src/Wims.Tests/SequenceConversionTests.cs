using System.Collections.Generic;
using System.Windows.Data;
using FluentAssertions;
using NUnit.Framework;
using Wims.Core.Dto;

namespace Wims.Tests
{
	public class ChordDtoAndStringConversionTests
	{
		[TestCaseSource(nameof(OneChord))]
		public void Should_Convert_StringToChord(string chord, ChordDto expected)
		{
			// Arrange


			// Act
			var b = ChordDto.FromString(chord);


			// Assert
			b.Should().BeEquivalentTo(expected);
		}

		[TestCaseSource(nameof(OneChord))]
		public void Should_Convert_ChordToString(string expected, ChordDto chord)
		{
			// Arrange


			// Act
			var b = chord.ToString();

			// Assert
			b.Should().Be(expected);
		}

		[TestCaseSource(nameof(SequenceOfChords))]
		public void Should_Convert_StringsToSequence(string sequence, SequenceDto expected)
		{
			// Arrange


			// Act
			var b = sequence.ToSequenceDto();

			// Assert
			b.Should().BeEquivalentTo(expected);
		}

		[TestCaseSource(nameof(SequenceOfChords))]
		public void Should_Convert_SequenceToString(string expected, SequenceDto sequence)
		{
			// Arrange


			// Act
			var b = sequence.ToString();

			// Assert
			b.Should().Be(expected);
		}

		[TestCaseSource(nameof(ListOfKeys))]
		public void Should_Convert_ListOfListOfKeys(List<List<string>> keys, SequenceDto expected)
		{
			// Arrange


			// Act
			var b = keys.ToSequenceDto();

			// Assert
			b.Should().BeEquivalentTo(expected);
		}

		static IEnumerable<object> OneChord()
		{
			yield return new object[]
			{
				"",
				new ChordDto
				{
					Keys = new string[0]
				}
			};
			yield return new object[]
			{
				"A",
				new ChordDto
				{
					Keys = new[]
					{
						"A"
					}
				}
			};
			yield return new object[]
			{
				"Ctrl + Shift + A",
				new ChordDto
				{
					Keys = new[]
					{
						"Ctrl",
						"Shift",
						"A"
					}
				}
			};
		}

		static IEnumerable<object> SequenceOfChords()
		{
			//yield return new object[]
			//{
			//	", , , ",
			//	new SequenceDto()
			//};

			yield return new object[]
			{
				"A, B",
				new SequenceDto
				{
					new ChordDto
					{
						Keys = new[]
						{
							"A"
						}
					},
					new ChordDto
					{
						Keys = new[]
						{
							"B"
						}
					}
				}
			};

			yield return new object[]
			{
				"A, B, C",
				new SequenceDto
				{
					new ChordDto
					{
						Keys = new[]
						{
							"A"
						}
					},
					new ChordDto
					{
						Keys = new[]
						{
							"B"
						}
					},
					new ChordDto
					{
						Keys = new[]
						{
							"C"
						}
					}
				}
			};

			yield return new object[]
			{
				"Ctrl + A, Ctrl + B",
				new SequenceDto
				{
					new ChordDto
					{
						Keys = new[]
						{
							"Ctrl",
							"A"
						}
					},
					new ChordDto
					{
						Keys = new[]
						{
							"Ctrl",
							"B"
						}
					}
				}
			};
		}

		static IEnumerable<object> ListOfKeys()
		{
			yield return new object[]
			{
				new List<List<string>>
				{
				},
				new SequenceDto
				{
				}
			};
			yield return new object[]
			{
				new List<List<string>>
				{
					new List<string> {"Ctrl", "A"},
					new List<string> {"Ctrl", "B"},
				},
				new SequenceDto
				{
					new ChordDto
					{
						Keys = new[]
						{
							"Ctrl",
							"A"
						}
					},
					new ChordDto
					{
						Keys = new[]
						{
							"Ctrl",
							"B"
						}
					}
				}
			};
			yield return new object[]
			{
				new List<List<string>>
				{
					new List<string> {"Ctrl", "A"},
				},
				new SequenceDto
				{
					new ChordDto
					{
						Keys = new[]
						{
							"Ctrl",
							"A"
						}
					},
				}
			};
		}
	}
}