﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesheimRooster.BusinessLayer.Response
{
	public class Success<T> : IResponse
	{
		public Success(T val)
		{
			Value = val;
		}
		public T Value { get; set; }
	}
}
