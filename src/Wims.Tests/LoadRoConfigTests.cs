using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.FakeItEasy;
using FluentAssertions;
using FluentValidation;
using NUnit.Framework;
using Wims.Core.Models;
using Wims.Ui.Requests;
using Wims.Ui.Validators;

namespace Wims.Tests
{
	public class LoadRoConfigTests
	{
		private AutoFake _auto;

		[SetUp]
		public void Setup()
		{
			_auto = new AutoFake();
			
			var validator = new ShortcutsRoValidator();
			_auto.Provide<IValidator<ShortcutsRo>>(validator);
		}

		[TearDown]
		public void TearDown()
		{
			_auto.Dispose();
		}

		[Test]
		public async Task Should_FindAllYamlFiles()
		{
			// Arrange

			var fs = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{@"c:\root\shortcuts\vscode\shortcuts.yml", new MockFileData(TestData.OneContext1Shortcut())},
				{@"c:\root\shortcuts\vs\shortcuts.yml", new MockFileData(TestData.TwoContext2Shortcuts())},
				{@"c:\root\shortcuts\notyml\shortcuts.json", new MockFileData("")},
				{@"c:\not-root\shortcuts\xy\shortcuts.yml", new MockFileData("")},
			});

			_auto.Provide<IFileSystem>(fs);

			var handler = _auto.Resolve<LoadRawShortcutsFromFilesRequestHandler>();
			var request = new LoadRawShortcutsFromFiles()
			{
				SourceDirectory = @"C:\root\shortcuts\"
			};

			// Act
			var result = await handler.Handle(request, CancellationToken.None);


			// Assert
			result.Should().HaveCount(2);
		}

		[Test]
		public async Task Should_DeserializeRawConfigs()
		{
			// Arrange
			var fs = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{@"c:\root\shortcuts\vscode\shortcuts.yml", new MockFileData(TestData.OneContext1Shortcut())},
			});
			_auto.Provide<IFileSystem>(fs);

			var handler = _auto.Resolve<LoadRawShortcutsFromFilesRequestHandler>();
			var request = new LoadRawShortcutsFromFiles()
			{
				SourceDirectory = @"C:\root\shortcuts\"
			};

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			result.Should().HaveCount(1);
			result[0].Contexts.Should().ContainKey("vscode");
			result[0].Shortcuts.Should().ContainKey("Show Command Palette");
		}

		[Test]
		public async Task Should_AssignPathOfRawConfigs()
		{
			// Arrange

			var fs = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{@"c:\root\shortcuts\vscode\shortcuts.yml", new MockFileData(TestData.OneContext1Shortcut())},
			});

			_auto.Provide<IFileSystem>(fs);

			var handler = _auto.Resolve<LoadRawShortcutsFromFilesRequestHandler>();
			var request = new LoadRawShortcutsFromFiles()
			{
				SourceDirectory = @"C:\root\shortcuts\"
			};

			// Act
			var result = await handler.Handle(request, CancellationToken.None);


			// Assert
			result.Should().HaveCount(1);
			result[0].Path.Should().Be(@"c:\root\shortcuts\vscode\shortcuts.yml");
		}

		[Test]
		public async Task Should_HandleNullContext()
		{
			var fs = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{@"c:\root\shortcuts\vscode\shortcuts.yml", new MockFileData(TestData.NoContext())},
			});

			_auto.Provide<IFileSystem>(fs);

			var handler = _auto.Resolve<LoadRawShortcutsFromFilesRequestHandler>();
			var request = new LoadRawShortcutsFromFiles()
			{
				SourceDirectory = @"C:\root\shortcuts\"
			};

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			result.Should().HaveCount(1);
			result[0].Shortcuts.Values.Select(v => v.Context).All(c => c == null).Should().BeTrue();
		}

		[Test]
		public async Task Should_RemoveFileWith_0Context_0Shortcut()
		{
			// Arrange
			var fs = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{@"c:\root\shortcuts\vscode\shortcuts.yml", new MockFileData(TestData.OneContext1Shortcut())},
				{@"c:\root\shortcuts\vscode\0c0s.yml", new MockFileData(TestData.ZeroContext0Shortcut())},
			});
			_auto.Provide<IFileSystem>(fs);

			var handler = _auto.Resolve<LoadRawShortcutsFromFilesRequestHandler>();
			var request = new LoadRawShortcutsFromFiles()
			{
				SourceDirectory = @"C:\root\shortcuts\"
			};

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			result.Should().HaveCount(1);
		}


		[Test]
		public async Task Should_RemoveEmptyFile()
		{
			var fs = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{@"c:\root\shortcuts\vscode\shortcuts.yml", new MockFileData("")},
			});

			_auto.Provide<IFileSystem>(fs);

			var handler = _auto.Resolve<LoadRawShortcutsFromFilesRequestHandler>();
			var request = new LoadRawShortcutsFromFiles()
			{
				SourceDirectory = @"C:\root\shortcuts\"
			};

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			result.Should().HaveCount(0);
		}

		[Test]
		public async Task Should_ThrowOnInvalidConfig()
		{
			var fs = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{@"c:\root\shortcuts\vscode\shortcuts.yml", new MockFileData(TestData.InvalidConfig())},
			});

			_auto.Provide<IFileSystem>(fs);

			var handler = _auto.Resolve<LoadRawShortcutsFromFilesRequestHandler>();
			var request = new LoadRawShortcutsFromFiles()
			{
				SourceDirectory = @"C:\root\shortcuts\"
			};

			// Act
			var action = new Func<Task>(() => handler.Handle(request, CancellationToken.None));

			// Assert
			action.Should().Throw<ValidationException>();
		}

	}
}