using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesheimRooster.BusinessLayer.Models.Classes
{

	public struct ClassJsonResponse
	{
		//public string elementTypeLabel { get; set; }
		//public bool kioskMode { get; set; }
		public ClassInfo[] elements { get; set; }
		/*public bool showQuickSelect { get; set; }
		public Labelpattern labelPattern { get; set; }
		public object filterLabelPattern { get; set; }
		public object filterElements { get; set; }
		public bool hasFilter { get; set; }
		public object filterTypeLabel { get; set; }
		public object filterProperty { get; set; }
		public Format[] formats { get; set; }
		public bool hasDepartmentSelect { get; set; }
		public object departments { get; set; }
		public int selectedDepartmentId { get; set; }
		public int selectedKlasseId { get; set; }
		public int selectedResourceTypeId { get; set; }
		public long lastImportTimestamp { get; set; }
		public bool showControls { get; set; }
		public int defaultElementId { get; set; }
		public int filterType { get; set; }
		public int selectedFormatId { get; set; }
		public bool test { get; set; }
		public Appcolor[] appColors { get; set; }
		public int pollingInterval { get; set; }*/
	}

	public struct Labelpattern
	{
		public string attr { get; set; }
		public string pattern { get; set; }
	}

	public struct ClassInfo
	{
		public int id { get; set; }
		public string name { get; set; }
		public object forename { get; set; }
		public string longName { get; set; }
		public string displayname { get; set; }
		public string externKey { get; set; }
		public int?[] dids { get; set; }
		public object klasseId { get; set; }
		public object title { get; set; }
		public string alternatename { get; set; }
		public object classteacher { get; set; }
		public object classteacher2 { get; set; }
		public object buildingId { get; set; }
		public object restypeId { get; set; }

		public override string ToString()
		{
			return this.displayname;
		}
	}

	public struct Format
	{
		public int days { get; set; }
		public string longName { get; set; }
		public int rowHeads { get; set; }
		public bool timegridDays { get; set; }
		public bool connectConsecutive { get; set; }
		public int colHeads { get; set; }
		public bool showLegend { get; set; }
		public bool hideDetails { get; set; }
		public bool showBreakSupervisions { get; set; }
		public bool renderOnServer { get; set; }
		public bool renderRealTime { get; set; }
		public string periodConnectType { get; set; }
		public int maxSlices { get; set; }
		public bool showHorizontalGridLines { get; set; }
		public bool hideEmptyColumns { get; set; }
		public object elementInfo { get; set; }
		public Periodformat periodFormat { get; set; }
		public int colHeads0 { get; set; }
		public string name { get; set; }
		public int id { get; set; }
		public int type { get; set; }
		public int endTime { get; set; }
		public int startTime { get; set; }
	}

	public struct Periodformat
	{
		public int rows { get; set; }
		public int cols { get; set; }
		public Timeposition timePosition { get; set; }
		public Textposition textPosition { get; set; }
		public Infoposition infoPosition { get; set; }
		public Userposition userPosition { get; set; }
		public Rescheduleinfoposition rescheduleInfoPosition { get; set; }
		public bool showSubstElements { get; set; }
		public bool substituteTextForEmptySubject { get; set; }
		public bool ignoreIndividualColors { get; set; }
		public int minHeight { get; set; }
		public int minWidth { get; set; }
		public Element1[] elements { get; set; }
	}

	public struct Timeposition
	{
		public bool top { get; set; }
		public bool bottom { get; set; }
	}

	public struct Textposition
	{
		public bool top { get; set; }
		public bool bottom { get; set; }
	}

	public struct Infoposition
	{
		public bool top { get; set; }
		public bool bottom { get; set; }
	}

	public struct Userposition
	{
		public bool top { get; set; }
		public bool bottom { get; set; }
	}

	public struct Rescheduleinfoposition
	{
		public bool top { get; set; }
		public bool bottom { get; set; }
	}

	public struct Element1
	{
		public string backColor { get; set; }
		public string foreColor { get; set; }
		public int row { get; set; }
		public int col { get; set; }
		public int fontSize { get; set; }
		public string labelPattern { get; set; }
		public int type { get; set; }
		public string separator { get; set; }
		public int max { get; set; }
	}

	public struct Appcolor
	{
		public string backColor { get; set; }
		public string labelKey { get; set; }
		public int id { get; set; }
		public string foreColor { get; set; }
	}

}
