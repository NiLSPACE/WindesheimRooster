using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesheimRooster.BusinessLayer.Models
{
	public class Les
	{
		public string id { get; set; }
		public string lokaal { get; set; }
		public long starttijd { get; set; }
		public long eindtijd { get; set; }
		public bool changed { get; set; }
		public object docentcode { get; set; }
		public DateTime roosterdatum { get; set; }
		public string commentaar { get; set; }
		public bool status { get; set; }
		public string roostercode { get; set; }
		public string groepcode { get; set; }
		public string vaknaam { get; set; }
		public string vakcode { get; set; }
		public string[] docentnamen { get; set; }

		public override string ToString()
		{
			var start = UnixTimeStampToDateTime((int)(starttijd / 1000)).ToString("HH:mm");
			var eind = UnixTimeStampToDateTime((int)(eindtijd / 1000)).ToString("HH:mm");
			return $@"{commentaar}
Start: {start}
Eind: {eind}
Lokaal: {lokaal}
Docenten: {String.Join(", ", docentnamen)}
";
		}

		public static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
		{
			// Unix timestamp is seconds past epoch
			System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
			return dtDateTime;
		}
	}
}
