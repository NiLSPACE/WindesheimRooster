using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http.Headers;

namespace WindesheimRooster.BusinessLayer
{
	internal static class Extensions
	{
		public static string Format(this string str, params object[] parameters)
		{
			return String.Format(str, parameters);
		}
	}
}
