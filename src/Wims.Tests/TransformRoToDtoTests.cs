using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.FakeItEasy;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using Wims.Core.Exceptions;
using Wims.Core.Models.Dto;
using Wims.Core.Models.Raw;
using Wims.Ui;
using Wims.Ui.Profiles;
using Wims.Ui.Requests;

namespace Wims.Tests
{
	public class TransformRoToDtoTests
	{
		private AutoFake _auto;

		[SetUp]
		public void Setup()
		{
			_auto = new AutoFake();
		}

		[TearDown]
		public void TearDown()
		{
			_auto.Dispose();
		}


		[Test]
		public async Task Should_MergeRoIntoDto()
		{
			// Arrange
			var shortcuts = new[]
			{
				MakeVs(),
				MakeVsCode()
			};

			var handler = _auto.Resolve<TransformRawShortcutsToDtoRequestHandler>();
			var request = new TransformRawShortcutsToDto
			{
				Shortcuts = shortcuts
			};

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			result.Contexts.Should().HaveCount(2);
			result.Shortcuts.Should().HaveCount(2);
		}

		[Test]
		public async Task Should_ThrowOnDuplicatedContext()
		{
			// Arrange
			var shortcuts = new[]
			{
				MakeVs(),
				MakeVs()
			};

			var handler = _auto.Resolve<TransformRawShortcutsToDtoRequestHandler>();
			var request = new TransformRawShortcutsToDto
			{
				Shortcuts = shortcuts
			};

			// Act
			var result = new Func<Task<ShortcutsDto>>(() => handler.Handle(request, CancellationToken.None));

			// Assert
			result.Should()
				.ThrowExactly<DuplicatedContextException>()
				.WithMessage("Duplicated context(s): 'vs'");
		}

		[Test]
		public async Task Should_ThrowOnNull()
		{
			// Arrange
			var shortcuts = new[]
			{
				new ShortcutsRo(),
				new ShortcutsRo(),
			};
			var handler = _auto.Resolve<TransformRawShortcutsToDtoRequestHandler>();
			var request = new TransformRawShortcutsToDto
			{
				Shortcuts = shortcuts
			};

			// Act
			var result = new Func<Task<ShortcutsDto>>(() => handler.Handle(request, CancellationToken.None));

			// Assert
			result.Should()
				.ThrowExactly<ArgumentNullException>();
		}

		[Test]
		public async Task Should_ThrowOnMissingContext()
		{
			// Arrange
			var shortcuts = new[]
			{
				new ShortcutsRo
				{
					Contexts = new Dictionary<string, ContextRo>(),
					Shortcuts = new Dictionary<string, ShortcutRo>
					{
						{
							"Reformat code", new ShortcutRo
							{
								Context = "vs",
							}
						}
					}
				}
			};

			var handler = _auto.Resolve<TransformRawShortcutsToDtoRequestHandler>();
			var request = new TransformRawShortcutsToDto
			{
				Shortcuts = shortcuts
			};

			// Act
			var result = new Func<Task<ShortcutsDto>>(() => handler.Handle(request, CancellationToken.None));

			// Assert
			result.Should()
				.ThrowExactly<MissingContextException>()
				.WithMessage("Missing context(s): 'vs'");
		}

		private ShortcutsRo MakeVs()
		{
			return new ShortcutsRo
			{
				Contexts = new Dictionary<string, ContextRo>
				{
					{
						"vs", new ContextRo
						{
							Icon = "vs.png",
							Match = new MatchRo
							{
								Exe = "devenv.exe"
							}
						}
					}
				},
				Path = @"c:\root\shortcuts\vs.yml",
				Shortcuts = new Dictionary<string, ShortcutRo>
				{
					{
						"Reformat code", new ShortcutRo
						{
							Context = "vs",
							Bindings = new List<BindingRo>
							{
								new BindingRo
								{
									Keys = new[] {"Ctrl", "Shift", "K"},
								},
								new BindingRo
								{
									Keys = new[] {"Ctrl", "Shift", "F"},
								}
							}
						}
					}
				}
			};
		}

		private ShortcutsRo MakeVsCode()
		{
			return new ShortcutsRo
			{
				Contexts = new Dictionary<string, ContextRo>
				{
					{
						"vscode", new ContextRo
						{
							Icon = "vscode.png",
							Match = new MatchRo
							{
								Exe = "vscode.exe"
							}
						}
					}
				},
				Path = @"c:\root\shortcuts\vscode.yml",
				Shortcuts = new Dictionary<string, ShortcutRo>
				{
					{
						"Reformat code", new ShortcutRo
						{
							Context = "vscode",
							Bindings = new List<BindingRo>
							{
								new BindingRo
								{
									Keys = new[] {"Ctrl", "Alt", "F"},
								}
							}
						}
					}
				}
			};
		}

		[Test]
		public async Task Should_AssignNameOfContext()
		{
			// Arrange
			var shortcuts = new[]
			{
				MakeVs()
			};

			var handler = _auto.Resolve<TransformRawShortcutsToDtoRequestHandler>();
			var request = new TransformRawShortcutsToDto
			{
				Shortcuts = shortcuts
			};

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			result.Contexts.Should().HaveCount(1);
			result.Contexts.First().Value.Name.Should().Be("vs");
		}

		[Test]
		public async Task Should_AssignDescOfShortcut()
		{
			// Arrange
			var shortcuts = new[]
			{
				MakeVs()
			};

			var handler = _auto.Resolve<TransformRawShortcutsToDtoRequestHandler>();
			var request = new TransformRawShortcutsToDto
			{
				Shortcuts = shortcuts
			};

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			result.Shortcuts.Should().HaveCount(1);
			result.Shortcuts.First().Description.Should().Be("Reformat code");
		}

		[Test]
		public async Task Should_MakeIconPathAbsolute()
		{
			// Arrange

			_auto.Provide<IFileSystem>(new FileSystem());
			var mapper = new MapperConfiguration(config => { config.AddProfile<ShortcutsProfile>(); })
				.CreateMapper();
			_auto.Provide(mapper);

			var shortcuts = new[]
			{
				new ShortcutsRo
				{
					Contexts = new Dictionary<string, ContextRo>()
					{
						{"vs", new ContextRo {Icon = "vs.png"}},
						{"vscode", new ContextRo {Icon = @"c:\another dir\vscode.png"}}
					},
					Path = @"c:\root\folder\vs.yml",
					Shortcuts = new Dictionary<string, ShortcutRo>()
				}
			};

			var handler = _auto.Resolve<TransformRawShortcutsToDtoRequestHandler>();
			var request = new TransformRawShortcutsToDto
			{
				Shortcuts = shortcuts
			};

			// Act
			var result = await handler.Handle(request, CancellationToken.None);

			// Assert
			result.Contexts.Should().HaveCount(2);
			result.Contexts.Values
				.Select(c => c.Icon)
				.Should().ContainInOrder(@"c:\root\folder\vs.png", @"c:\another dir\vscode.png");
		}
	}
}