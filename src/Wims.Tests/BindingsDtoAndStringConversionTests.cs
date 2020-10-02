using System.Collections.Generic;
using System.Windows.Data;
using FluentAssertions;
using NUnit.Framework;
using Wims.Core.Dto;

namespace Wims.Tests
{
	public class BindingsDtoAndStringConversionTests
	{
		[TestCaseSource(nameof(OneBinding))]
		public void Should_Convert_StringToBinding(string binding, BindingDto expected)
		{
			// Arrange


			// Act
			var b = BindingDto.FromString(binding);


			// Assert
			b.Should().BeEquivalentTo(expected);
		}

		[TestCaseSource(nameof(OneBinding))]
		public void Should_Convert_BindingToString(string expected, BindingDto binding)
		{
			// Arrange


			// Act
			var b = binding.ToString();

			// Assert
			b.Should().Be(expected);
		}

		// todo: rename to Sequence
		[TestCaseSource(nameof(MultipleBindings))]
		public void Should_Convert_StringsToMultipleBindings(string bindings, List<BindingDto> expected)
		{
			// Arrange


			// Act
			var b = bindings.ToBindingDto();

			// Assert
			b.Should().BeEquivalentTo(expected);
		}

		[TestCaseSource(nameof(MultipleBindings))]
		public void Should_Convert_MultipleBindingsToString(string expected, List<BindingDto> bindings)
		{
			// Arrange


			// Act
			var b = bindings.AsString();

			// Assert
			b.Should().Be(expected);
		}

		[TestCaseSource(nameof(ListOfKeys))]
		public void Should_Convert_ListOfListOfKeys(List<List<string>> keys, List<BindingDto> expected)
		{
			// Arrange


			// Act
			var b = keys.ToBindingDto();

			// Assert
			b.Should().BeEquivalentTo(expected);
		}

		static IEnumerable<object> OneBinding()
		{
			yield return new object[]
			{
				"",
				new BindingDto
				{
					Keys = new string[0]
				}
			};
			yield return new object[]
			{
				"A",
				new BindingDto
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
				new BindingDto
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

		static IEnumerable<object> MultipleBindings()
		{
			//yield return new object[]
			//{
			//	", , , ",
			//	new List<BindingDto>()
			//};

			yield return new object[]
			{
				"A, B",
				new List<BindingDto>
				{
					new BindingDto
					{
						Keys = new[]
						{
							"A"
						}
					},
					new BindingDto
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
				new List<BindingDto>
				{
					new BindingDto
					{
						Keys = new[]
						{
							"A"
						}
					},
					new BindingDto
					{
						Keys = new[]
						{
							"B"
						}
					},
					new BindingDto
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
				new List<BindingDto>
				{
					new BindingDto
					{
						Keys = new[]
						{
							"Ctrl",
							"A"
						}
					},
					new BindingDto
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
				new List<BindingDto>
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
				new List<BindingDto>
				{
					new BindingDto
					{
						Keys = new[]
						{
							"Ctrl",
							"A"
						}
					},
					new BindingDto
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
				new List<BindingDto>
				{
					new BindingDto
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