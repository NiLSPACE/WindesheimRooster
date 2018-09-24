using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesheimRooster.BusinessLayer.Models.Schedule {

	public class Schedule {
		public Result result { get; set; }
		public object error { get; set; }
	}

	public class Result {
		public long lastImportTimestamp { get; set; }
		public Data data { get; set; }
	}

	public class Data {
		public int[] elementIds { get; set; }
		public Dictionary<string, object> elementRoomLocks { get; set; }
		public Dictionary<string, ElementPeriod[]> elementPeriods { get; set; }
		public ElementInfo[] elements { get; set; }
	}

	public class ElementInfo {
		public string longName { get; set; }
		public string alternatename { get; set; }
		public string name { get; set; }
		public int id { get; set; }
		public int type { get; set; }
		public string displayname { get; set; }
	}

	public class ElementPeriod {
		public int code { get; set; }
		public int lessonNumber { get; set; }
		public int lessonId { get; set; }
		public string lessonCode { get; set; }
		public string lessonText { get; set; }
		public string periodText { get; set; }
		public Is _is { get; set; }
		public string cellState { get; set; }
		public bool hasPeriodText { get; set; }
		public bool hasInfo { get; set; }
		public int id { get; set; }
		public int priority { get; set; }
		public int date { get; set; }
		public int endTime { get; set; }
		public int startTime { get; set; }
		public ElementPeriodInfo[] elements { get; set; }
	}

	public class Is {
		public bool standard { get; set; }
	}

	public class ElementPeriodInfo {
		public bool missing { get; set; }
		public int orgId { get; set; }
		public int id { get; set; }
		public string state { get; set; }
		public int type { get; set; }
	}

}
