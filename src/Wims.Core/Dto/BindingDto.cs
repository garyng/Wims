﻿using System;

namespace Wims.Core.Dto
{
	public class BindingDto
	{
		public string[] Keys { get; set; }

		public override string ToString()
		{
			return string.Join(" + ", Keys);
		}

		
		// todo: make this implicit conversion
		public static BindingDto FromString(string keys)
		{
			return new BindingDto
			{
				Keys = keys.Split(" + ", StringSplitOptions.RemoveEmptyEntries)
			};
		}
	}
}