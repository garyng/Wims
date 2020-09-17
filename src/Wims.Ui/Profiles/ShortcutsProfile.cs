using AutoMapper;
using Wims.Core.Models.Dto;
using Wims.Core.Models.Raw;

namespace Wims.Ui.Profiles
{
	public class ShortcutsProfile : Profile
	{
		public ShortcutsProfile()
		{
			CreateMap<BindingRo, BindingDto>();
			CreateMap<MatchRo, MatchDto>();
			CreateMap<ContextRo, ContextDto>();
		}
	}
}