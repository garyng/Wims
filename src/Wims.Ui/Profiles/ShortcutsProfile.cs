﻿using AutoMapper;
using Wims.Core.Dto;
using Wims.Core.Models;

namespace Wims.Ui.Profiles
{
	public class ShortcutsProfile : Profile
	{
		public ShortcutsProfile()
		{
			CreateMap<ChordRo, BindingDto>();
			CreateMap<MatchRo, MatchDto>();
			CreateMap<ContextRo, ContextDto>();
		}
	}
}