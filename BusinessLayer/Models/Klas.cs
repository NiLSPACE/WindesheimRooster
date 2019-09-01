using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesheimRooster.BusinessLayer.Models
{

	public class Klas
	{
		public string id { get; set; }
		public string code { get; set; }
		public string klasnaam { get; set; }

		public override string ToString()
		{
			return klasnaam;
		}
	}
}
