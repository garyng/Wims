using System.Windows.Input;

namespace Wims.Ui.Controls.KeysRecorder
{
	public class KeyEventDto
	{
		private readonly KeyEventTypes _type;
		public Key Key { get; }

		private KeyEventDto(KeyEventTypes type, KeyEventArgs args, bool handled = true)
		{
			args.Handled = handled;
			_type = type;
			Key = GetRealKey(args);
		}

		private KeyEventDto(KeyEventTypes type, Key key)
		{
			_type = type;
			Key = key;
		}

		public static KeyEventDto Down(KeyEventArgs args) => new KeyEventDto(KeyEventTypes.Down, args, true);
		public static KeyEventDto Up(KeyEventArgs args) => new KeyEventDto(KeyEventTypes.Up, args, true);

		public static KeyEventDto Down(Key key) => new KeyEventDto(KeyEventTypes.Down, key);
		public static KeyEventDto Up(Key key) => new KeyEventDto(KeyEventTypes.Up, key);

		public bool IsUp(out Key up)
		{
			up = Key;
			return _type == KeyEventTypes.Up;
		}

		public bool IsDown(out Key down)
		{
			down = Key;
			return _type == KeyEventTypes.Down;
		}

		private Key GetRealKey(KeyEventArgs args)
		{
			// https://stackoverflow.com/a/40219379/
			return args.Key switch
			{
				Key.System => args.SystemKey,
				Key.ImeProcessed => args.ImeProcessedKey,
				Key.DeadCharProcessed => args.DeadCharProcessedKey,
				_ => args.Key
			};
		}
	}
}