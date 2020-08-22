using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using Force.DeepCloner;
using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KleSharp
{
	public class KeyDefault
	{
		public string TextColor { get; set; } = "#000000";
		public double TextSize { get; set; } = 3;
	}

	public class Key
	{
		/// <summary>
		/// The keycap color
		/// </summary>
		public string Color { get; set; } = "#cccccc";

		/// <summary>
		/// An array of up to 12 text labels, left-to-right, top-to-bottom
		/// 0 1 2
		/// 3 4 5
		/// 6 7 8
		/// 9 10 11
		/// </summary>
		public string[] Labels { get; set; } = new string[12];

		/// <summary>
		/// an array of up to 12 colors (e.g., "#ff0000"), to be used for the text labels;
		/// if any entries are null or undefined, you should use the default.textColor.
		/// </summary>
		public string[] TextColor { get; set; } = new string[12];

		/// <summary>
		/// an array of up to 12 sizes (integers 1-9), to be used for the text labels;
		/// if any entries are null or undefined, you should use the default.textSize.
		/// </summary>
		/// <remarks>Note that the sizes are relative and do not correspond to any fixed font size.</remarks>
		public double?[] TextSize { get; set; } = new double?[12];

		/// <summary>
		/// Default text color and (relative) size.
		/// </summary>
		public KeyDefault Default { get; set; } = new KeyDefault();

		/// <summary>
		/// The absolute position of the key in keyboard units 
		/// (1U = size of a standard 1x1 keycap)
		/// </summary>
		public double X { get; set; }

		public double Y { get; set; }

		/// <summary>
		/// The size of the key in keyboard units
		/// </summary>
		public double Width { get; set; } = 1;

		public double Height { get; set; } = 1;

		/// <summary>
		/// The size and position of the second rectange that 
		/// is used to define oddly-shaped keys. 
		/// Two rectanges can be thought of as overlapping, combining to create the desired key shape.
		/// </summary>
		public double X2 { get; set; }

		public double Y2 { get; set; }
		public double Width2 { get; set; } = 1;
		public double Height2 { get; set; } = 1;

		/// <summary>
		/// Defines the center of location for the key
		/// </summary>
		public double RotationX { get; set; }

		public double RotationY { get; set; }

		/// <summary>
		/// Specify the angle the key is rotated about the center of rotation
		/// </summary>
		public double RotationAngle { get; set; }

		/// <summary>
		/// Only text labels should be rendered, not the keycap borders
		/// </summary>
		public bool Decal { get; set; }

		/// <summary>
		/// Render unobtrusively, typically semi-transparent and without any labels
		/// </summary>
		public bool Ghost { get; set; }

		/// <summary>
		/// Specifies that the key is stepped
		/// </summary>
		public bool Stepped { get; set; }

		/// <summary>
		/// Specifies that the key has a homing nub/bump/dish
		/// </summary>
		public bool Nub { get; set; }

		/// <summary>
		/// Specifies the key's profile
		/// </summary>
		public string Profile { get; set; } = "";

		public string SwitchMount { get; set; } = "";
		public string SwitchBrand { get; set; } = "";
		public string SwitchType { get; set; } = "";
	}

	public class RawKey
	{
		public double? r { get; set; }
		public double? rx { get; set; }
		public double? ry { get; set; }
		public int? a { get; set; }
		public double? f { get; set; }
		public double? f2 { get; set; }
		public double?[] fa { get; set; }
		public string p { get; set; }
		public string c { get; set; }
		public string t { get; set; }
		public double? x { get; set; }
		public double? y { get; set; }
		public double? w { get; set; }
		public double? h { get; set; }
		public double? x2 { get; set; }
		public double? y2 { get; set; }
		public double? w2 { get; set; }
		public double? h2 { get; set; }
		public bool n { get; set; }
		public bool l { get; set; }
		public bool d { get; set; }
		public bool g { get; set; }
		public string sm { get; set; }
		public string sb { get; set; }
		public string st { get; set; }
	}

	public class KeyboardMetadata
	{
		public string Author { get; set; }
		public string BackColor { get; set; }
		public KeyboardBackground Background { get; set; }
		public string Name { get; set; }
		public string Radii { get; set; }

		public string SwitchMount { get; set; }
		public string SwitchBrand { get; set; }
		public string SwitchType { get; set; }
	}

	public class KeyboardBackground
	{
		public string Name { get; set; }
		public string Style { get; set; }
	}

	public class Keyboard
	{
		public KeyboardMetadata Metadata { get; set; }
		public List<Key> Keys { get; set; } = new List<Key>();
	}

	public class KleDeserializer
	{
		public Keyboard Deserialize(string json)
		{
			var (metadatas, rows) = Load(json);


			if (metadatas.Any(meta => meta.Item2 != 0))
			{
				throw new Exception("Metadata must be the first element.");
			}

			if (metadatas.Count > 1)
			{
				throw new Exception("There should only be one metadata element.");
			}

			var metadata = metadatas
				.Select(meta => meta.Item1)
				.SingleOrDefault()
				?.ToObject<KeyboardMetadata>();


			var currentKey = new Key();
			var keyboard = new Keyboard
			{
				Metadata = metadata
			};
			var align = 4;
			foreach (var row in rows)
			{
				for (int r = 0; r < row.Count(); r++)
				{
					var item = row[r];
					if (item.Type == JTokenType.String)
					{
						var newKey = currentKey.DeepClone();
						var value = item.ToString();

						// calculate generated values 
						newKey.Width2 =
							newKey.Width2 == 0 ? currentKey.Width : currentKey.Width2;
						newKey.Height2 =
							newKey.Height2 == 0 ? currentKey.Height : currentKey.Height2;
						newKey.Labels = ParseLabels(Split(value, '\n'), align);
						newKey.TextSize = ParseLabels(newKey.TextSize, align);

						// cleanup the data
						for (int i = 0; i < 12; i++)
						{
							if (string.IsNullOrEmpty(newKey.Labels[i]))
							{
								newKey.TextSize[i] = null;
								newKey.TextColor[i] = null;
							}

							if (newKey.TextSize[i] == newKey.Default.TextSize)
							{
								newKey.TextSize[i] = null;
							}

							if (newKey.TextColor[i] == newKey.Default.TextColor)
							{
								newKey.TextColor[i] = null;
							}
						}

						// add the key
						keyboard.Keys.Add(newKey);

						// setup for the next key
						currentKey.X += currentKey.Width;
						currentKey.Width = 1;
						currentKey.Height = 1;
						currentKey.X2 = 0;
						currentKey.Y2 = 0;
						currentKey.Width2 = 0;
						currentKey.Height2 = 0;
						currentKey.Nub = false;
						currentKey.Stepped = false;
						currentKey.Decal = false;
					}
					else if (item.Type == JTokenType.Object)
					{
						var raw = item.ToObject<RawKey>();
						if (r != 0 && (raw.r != null || raw.rx != null || raw.ry != null))
						{
							throw new Exception("Rotation can only be specified on the first key in a row");
						}

						if (raw.r != null) currentKey.RotationAngle = raw.r.Value;
						if (raw.rx != null) currentKey.RotationX = raw.rx.Value;
						if (raw.ry != null) currentKey.RotationY = raw.ry.Value;
						if (raw.a != null) align = raw.a.Value;
						if (raw.f != null)
						{
							currentKey.Default.TextSize = raw.f.Value;
							currentKey.TextSize = new double?[12];
						}

						if (raw.f2 != null)
						{
							for (int i = 1; i < 12; i++)
							{
								currentKey.TextSize[i] = raw.f2;
							}
						}

						if (raw.fa != null) currentKey.TextSize = raw.fa;
						if (!string.IsNullOrEmpty(raw.p)) currentKey.Profile = raw.p;
						if (!string.IsNullOrEmpty(raw.c)) currentKey.Color = raw.c;
						if (!string.IsNullOrEmpty(raw.t))
						{
							var colors = Split(raw.t, '\n');
							if (!string.IsNullOrEmpty(colors[0])) currentKey.Default.TextColor = colors[0];
							currentKey.TextColor = ParseLabels(colors, align);
						}

						if (raw.x != null) currentKey.X += raw.x.Value;
						if (raw.y != null) currentKey.Y += raw.y.Value;
						if (raw.w != null)
						{
							currentKey.Width = raw.w.Value;
							currentKey.Width2 = raw.w.Value;
						}

						if (raw.h != null)
						{
							currentKey.Height = raw.h.Value;
							currentKey.Height2 = raw.h.Value;
						}

						if (raw.x2 != null) currentKey.X2 = raw.x2.Value;
						if (raw.y2 != null) currentKey.Y2 = raw.y2.Value;
						if (raw.w2 != null) currentKey.Width2 = raw.w2.Value;
						if (raw.h2 != null) currentKey.Height2 = raw.h2.Value;
						currentKey.Nub = raw.n;
						currentKey.Stepped = raw.l;
						currentKey.Decal = raw.d;
						currentKey.Ghost = raw.g;
						if (raw.sm != null) currentKey.SwitchMount = raw.sm;
						if (raw.sb != null) currentKey.SwitchBrand = raw.sb;
						if (raw.st != null) currentKey.SwitchType = raw.st;
					}
				}

				// end of a row
				currentKey.Y++;
				currentKey.X = currentKey.RotationX;
			}


			return keyboard;
		}

		private T[] ParseLabels<T>(IList<T> labels, int align)
		{
			var map = new[]
			{
				// @formatter:off
				//     0   1   2   3   4   5    6   7   8   9  10  11   // align flags
				new[] {0,  6,  2,  8,  9,  11,  3,  5,  1,  4,  7, 10}, // 0 = no centering
				new[] {1,  7, -1, -1,  9,  11,  4, -1, -1, -1, -1, 10}, // 1 = center x
				new[] {3, -1,  5, -1,  9,  11, -1, -1,  4, -1, -1, 10}, // 2 = center y
				new[] {4, -1, -1, -1,  9,  11, -1, -1, -1, -1, -1, 10}, // 3 = center x & y
				new[] {0,  6,  2,  8,  10, -1,  3,  5,  1,  4,  7, -1}, // 4 = center front (default)
				new[] {1,  7, -1, -1,  10, -1,  4, -1, -1, -1, -1, -1}, // 5 = center front & x
				new[] {3, -1,  5, -1,  10, -1, -1, -1,  4, -1, -1, -1}, // 6 = center front & y
				new[] {4, -1, -1, -1,  10, -1, -1, -1, -1, -1, -1, -1}, // 7 = center front & x & y
				// @formatter:on
			};
			// var count = labels.Count;
			var ret = new T[12];
			for (int i = 0; i < labels.Count(); i++)
			{
				var pos = map[align][i];
				if (labels[i] == null) continue;
				if (pos == -1) continue;
				ret[pos] = labels[i];
			}

			return ret;
		}


		private List<string> Split(string input, params char[] separator)
		{
			return input.Split(separator)
				.Select(c => string.IsNullOrEmpty(c) ? null : c)
				.ToList();
		}

		private (IList<(JToken, int)> metadata, IList<JToken> rows) Load(string json)
		{
			var (metadata, rows) =
				JArray.Parse(json)
					.Select((token, i) => (token, i))
					.Partition(item => item.token is JObject);

			return (metadata.ToList(), rows.Select(row => row.token).ToList());
		}
	}
}