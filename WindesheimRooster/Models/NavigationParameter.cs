using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesheimRooster.Models
{
	internal class NavigationParameter
	{
		public Type RedirectTo { get; set; }
		public object Parameter { get; set; }
	}
}
