using System.Collections.Generic;
using System.Windows.Input;
using FluentAssertions;
using NUnit.Framework;
using Wims.Ui.Controls.KeysRecorder;

namespace Wims.Tests
{
	public class KeyCombinationTests
	{
		private KeyCombination _kc;
		private Key _removal;

		[SetUp]
		public void SetUp()
		{
			_removal = Key.Back;
			_kc = new KeyCombination();
		}

		[TestCaseSource(nameof(OneChord))]
		public void Should_Handle_OneChord(KeyEventDto[] events, string expected)
		{
			// Act
			_kc.Handle(_removal, events);

			// Assert
			_kc.ToString().Should().Be(expected);
		}

		[TestCaseSource(nameof(SequenceOfChords))]
		public void Should_Handle_Sequence(KeyEventDto[] events, string expected)
		{
			// Act
			_kc.Handle(_removal, events);

			// Assert
			_kc.ToString().Should().Be(expected);
		}

		[TestCaseSource(nameof(WithRemoval))]
		public void Should_Handle_RemovalOfKey(KeyEventDto[] events, string expected)
		{
			// Act
			_kc.Handle(_removal, events);


			// Assert
			_kc.ToString().Should().Be(expected);
		}

		[TestCaseSource(nameof(EdgeCases))]
		public void Should_Handle_EdgeCases(KeyEventDto[] events, string expected)
		{
			// Act
			_kc.Handle(_removal, events);


			// Assert
			_kc.ToString().Should().Be(expected);
		}

		[TestCaseSource(nameof(ListOfListOfKeys))]
		public void Should_Convert_ToListOfListOfKeys(KeyEventDto[] events, List<List<string>> expected)
		{
			// Arrange


			// Act
			_kc.Handle(_removal, events);


			// Assert
			_kc.ToStrings().Should().BeEquivalentTo(expected);
		}


		static IEnumerable<object> OneChord()
		{
			yield return new object[]
			{
				new[]
				{
					Key.A.Down(),
					Key.A.Up()
				},
				"A, "
			};
			yield return new object[]
			{
				new[]
				{
					Key.LeftCtrl.Down(),
					Key.LeftShift.Down(),
					Key.A.Down(),
					Key.LeftCtrl.Up(),
					Key.LeftShift.Up(),
					Key.A.Up()
				},
				"Ctrl + Shift + A, "
			};

			// repeating
			yield return new object[]
			{
				new[]
				{
					Key.LeftCtrl.Down(),
					Key.LeftCtrl.Down(),
					Key.LeftShift.Down(),
					Key.A.Down(),
					Key.A.Down(),
					Key.A.Down(),
					Key.A.Down(),
					Key.LeftCtrl.Up(),
					Key.LeftShift.Up(),
					Key.A.Up()
				},
				"Ctrl + Shift + A, "
			};

			// key up non existing
			yield return new object[]
			{
				new[]
				{
					Key.LeftCtrl.Down(),
					Key.LeftShift.Down(),
					Key.A.Down(),
					Key.LeftCtrl.Up(),
					Key.LeftShift.Up(),
					Key.D.Up(),
					Key.A.Up()
				},
				"Ctrl + Shift + A, "
			};
		}


		static IEnumerable<object> SequenceOfChords()
		{
			yield return new object[]
			{
				new[]
				{
					Key.A.Down(),
					Key.A.Up(),
					Key.B.Down(),
					Key.B.Up(),
				},
				"A, B, "
			};
			// repeating
			yield return new object[]
			{
				new[]
				{
					Key.A.Down(),
					Key.A.Up(),
					Key.A.Down(),
					Key.A.Up(),
				},
				"A, A, "
			};
		}

		static IEnumerable<object> WithRemoval()
		{
			yield return new object[]
			{
				new[]
				{
					Key.Back.Down(),
					Key.Back.Up()
				},
				""
			};
			yield return new object[]
			{
				new[]
				{
					Key.Back.Down(),
					Key.Back.Up(),
					Key.Back.Down(),
					Key.Back.Up()
				},
				""
			};

			yield return new object[]
			{
				new[]
				{
					Key.A.Down(),
					Key.Back.Down(),
					Key.Back.Up(),
					Key.A.Up()
				},
				""
			};

			yield return new object[]
			{
				new[]
				{
					Key.A.Down(),
					Key.A.Up(),
					Key.B.Down(),
					Key.B.Up(),
					Key.Back.Down(),
					Key.Back.Up()
				},
				"A, "
			};
		}

		static IEnumerable<object> EdgeCases()
		{
			// press A, then B, then release A, then release B
			yield return new object[]
			{
				new[]
				{
					Key.A.Down(),
					Key.B.Down(),
					Key.B.Up(),
					Key.A.Up(),
				},
				"A + B, "
			};

			// should not show comma at the end
			yield return new object[]
			{
				new[]
				{
					Key.A.Down(),
					Key.B.Down()
				},
				"A + B"
			};
		}

		static IEnumerable<object> ListOfListOfKeys()
		{
			yield return new object[]
			{
				new KeyEventDto[]
				{

				},
				new List<List<string>>()
			};
			yield return new object[]
			{
				new[]
				{
					Key.A.Down(),
					Key.A.Up()
				},
				new List<List<string>>
				{
					new List<string> {"A"}
				}
			};
			yield return new object[]
			{
				new[]
				{
					Key.LeftCtrl.Down(),
					Key.LeftShift.Down(),
					Key.A.Down(),
					Key.LeftCtrl.Up(),
					Key.LeftShift.Up(),
					Key.A.Up()
				},
				new List<List<string>>
				{
					new List<string> {"Ctrl", "Shift", "A"}
				}
			};
			yield return new object[]
			{
				new[]
				{
					Key.A.Down(),
					Key.A.Up(),
					Key.B.Down(),
					Key.B.Up(),
				},
				new List<List<string>>
				{
					new List<string> {"A"},
					new List<string> {"B"}
				}
			};
		}
	}
}