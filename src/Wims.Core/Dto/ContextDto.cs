using System;

namespace Wims.Core.Dto
{
	public class ContextDto : IEquatable<ContextDto>
	{
		public string Name { get; set; }
		public string Icon { get; set; }
		public MatchDto Match { get; set; }

		public bool Equals(ContextDto other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Name == other.Name;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ContextDto)obj);
		}

		public override int GetHashCode()
		{
			return (Name != null ? Name.GetHashCode() : 0);
		}
	}
}