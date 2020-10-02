using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GaryNg.Utils.Enumerable;
using GaryNg.Utils.List;

namespace Wims.Ui.Controls.KeysRecorder
{
	public class KeyCombination
	{
		/// <summary>
		/// Hold all the keys inside current combination
		/// </summary>
		private readonly HashSet<Key> _stack;

		/// <summary>
		/// Hold a list of key combinations
		/// </summary>
		private readonly List<HashSet<Key>> _combinations;

		public KeyCombination()
		{
			_stack = new HashSet<Key>();
			_combinations = new List<HashSet<Key>>();
		}


		/// <summary>
		/// Handle <see cref="KeyEventDto"/>s to <see cref="KeyCombination"/>
		/// </summary>
		public KeyCombination Handle(KeyEventDto e, Key removalKey = Key.Back)
		{
			if (e.IsDown(out var down))
			{
				if (e.Key == removalKey)
				{
					Pop();
				}
				else
				{
					Add(down);
				}

				return this;
			}

			if (e.IsUp(out var up))
			{
				TryEnd(up);
				return this;
			}

			return this;
		}

		public KeyCombination Handle(Key removalKey = Key.Back, params KeyEventDto[] events)
		{
			foreach (var e in events)
			{
				Handle(e, removalKey);
			}

			return this;
		}

		/// <summary>
		/// Add a key to current combination.
		/// This also pushes the key to current combination's key stack.
		/// </summary>
		/// <example>
		/// Current combination: Ctrl + Shift
		/// Add 'A': Ctrl + Shift + A
		/// Stack: Ctrl, Shift, A
		/// </example>
		/// <param name="key"></param>
		public void Add(Key key)
		{
			_stack.Add(key);
			if (_combinations.Any())
			{
				_combinations.Last().Add(key);
			}
			else
			{
				_combinations.Add(new HashSet<Key> {key});
			}
		}

		/// <summary>
		/// Try to end the current combination by matching the provided key.
		/// Note that a combination is ended only when its key stack is empty.
		/// </summary>
		/// <example>
		/// Current combination: Ctrl + Shift + A
		/// Current stack: A
		/// End with: A
		/// Result: Ctrl + Shift + A, 
		/// </example>
		/// <param name="key"></param>
		public void TryEnd(Key key)
		{
			if (!_stack.Contains(key)) return;
			_stack.Remove(key);

			// start new combination
			if (_stack.Empty())
			{
				_combinations.Add(new HashSet<Key>());
			}
		}

		/// <summary>
		/// End the current combination and discard its key stack.
		/// </summary>
		/// <example>
		/// Current stack: Ctrl, Shift
		/// Current combination: Ctrl + Shift
		/// After end:
		/// Current Stack: empty
		/// Combination: Ctrl + Shift, 
		/// </example>
		/// <param name="key"></param>
		public void End()
		{
			_stack.Clear();
		}

		/// <summary>
		/// Remove the last combination
		/// </summary>
		/// <example>
		/// Current combination: Ctrl + Shift + A, Shift + B
		/// Remove last: Ctrl + Shift + A
		/// </example>
		public void Pop()
		{
			switch (_combinations.Count, _combinations.LastOrDefault()?.Empty() == true)
			{
				case (0, _):
					break;
				case var (c, _) when c == 1:
					_combinations.RemoveLast();
					break;
				case var (c, lastIsEmpty) when c > 1:
					_combinations.RemoveLast();
					if (lastIsEmpty)
					{
						_combinations.RemoveLast();
					}

					_combinations.Add(new HashSet<Key>());
					break;
			}
		}

		public override string ToString()
		{
			return string.Join(", ",
				_combinations.Select(c => string.Join(" + ",
					c.Select(k => k.GetName())))
			);
		}

		public List<List<string>> ToStrings()
		{
			return _combinations
				.Where(c => c.Any())
				.Select(c =>
					c.Select(k => k.GetName()).ToList())
				.ToList();
		}
	}
}