
namespace Wims.Core.Dto
{
	public class MatchDto
	{
		public MaybeRegex Class { get; set; }
		public MaybeRegex Exe { get; set; }

		public MatchDto(string @class, string exe)
		{
			if (!string.IsNullOrEmpty(@class)) Class = new MaybeRegex(@class);
			if (!string.IsNullOrEmpty(exe)) Exe = new MaybeRegex(exe);
		}

		public bool IsMatch(string @class, string exe)
		{
			var result = true;
			if (Class != null) result = result && Class.IsMatch(@class);
			if (Exe != null) result = result && Exe.IsMatch(exe);

			return result;
		}
	}
}