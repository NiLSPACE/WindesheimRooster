using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindesheimRooster.BusinessLayer.Models.Classes;
using WindesheimRooster.BusinessLayer.Models.Schedule;

namespace WindesheimRooster.BusinessLayer {
	public class WindesheimManager {
		public async static Task<Schedule> GetScheduleForClass(string classId, string className, int weekOffset = 0) {
			var rawSchedule = await WindesheimAPI.GetScheduleForClass(classId, weekOffset);
			if (weekOffset == 0) {
				await HistoryManager.AddToHistory(className, rawSchedule);
			}
			return JsonConvert.DeserializeObject<Schedule>(rawSchedule);
		}

		public async static Task<ClassInfo[]> GetAllClasses() {
			if (await HistoryManager.HasReleventClassList()) {
				return await HistoryManager.GetClassList();
			}

			var rawClasses = await WindesheimAPI.GetAllClasses();
			await HistoryManager.SaveClassList(rawClasses);
			return JsonConvert.DeserializeObject<ClassJsonResponse>(rawClasses).elements;
		}
	}
}
