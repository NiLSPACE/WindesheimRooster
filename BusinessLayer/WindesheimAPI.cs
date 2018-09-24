using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindesheimRooster.BusinessLayer.Models.Classes;
using WindesheimRooster.BusinessLayer.Models.Schedule;
using Windows.Storage;
using Windows.Web.Http;

namespace WindesheimRooster.BusinessLayer {
	internal class WindesheimAPI {
		private static HttpRequestMessage CreateRequest(HttpMethod method, Uri uri) {
			var request = new HttpRequestMessage(method, uri);
			request.Headers.Accept.Add(new Windows.Web.Http.Headers.HttpMediaTypeWithQualityHeaderValue("application/json"));
			request.Headers.Host = new Windows.Networking.HostName("roosters.windesheim.nl");
			request.Headers.Referer = new Uri("https://roosters.windesheim.nl/WebUntis/?school=Windesheim");
			request.Headers.Cookie.ParseAdd("schoolname=\"_V2luZGVzaGVpbQ==\"");
			return request;
		}

		private static DateTime GetDate(int weekOffset = 0) {
			var date = DateTime.Now;
			if (weekOffset != 0) {
				date = date.AddDays(weekOffset * 7);
			}

			if ((date.DayOfWeek == DayOfWeek.Saturday) || (date.DayOfWeek == DayOfWeek.Sunday)) {
				// Skip to the next Monday if it's the weekend.
				int daysUntilMonday = ((int)DayOfWeek.Monday - (int)date.DayOfWeek + 7) % 7;
				date = date.AddDays(daysUntilMonday);
			}
			return date;
		}


		public static async Task<string> GetScheduleForClass(string classId, int weekOffset) {
			var client = new HttpClient();
			var uri = new Uri($"https://roosters.windesheim.nl/WebUntis/Timetable.do?ajaxCommand=getWeeklyTimetable&elementType=1&elementId={classId}&date={GetDate(weekOffset).ToString("yyyyMMdd")}");

			var request = CreateRequest(HttpMethod.Post, uri);
			var result = await client.SendRequestAsync(request);
			var content = await result.Content.ReadAsStringAsync();
			return content;
		}

		public static async Task<string> GetAllClasses() {
			var client = new HttpClient();
			var uri = new Uri("https://roosters.windesheim.nl/WebUntis/Timetable.do");

			string date = GetDate().ToString("yyyyMMdd");
			var request = CreateRequest(HttpMethod.Post, uri);
			request.Content = new HttpStringContent($"ajaxCommand=getPageConfig&type=1&date={date}", Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded");
			var result = await client.SendRequestAsync(request);

			var json = await result.Content.ReadAsStringAsync();

			// Todo: Cache these results somewhere.
			//var objects = JsonConvert.DeserializeObject<ClassJsonResponse>(json);
			return json;
		}


	}
}
