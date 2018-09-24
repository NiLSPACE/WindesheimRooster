using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesheimRooster {
	internal static class Extensions {
		public static string Reverse(this string str) {
			var arr = str.ToCharArray();
			return new String(arr.Reverse().ToArray());
		}
	}
}
