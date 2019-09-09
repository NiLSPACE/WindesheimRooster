using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using WindesheimRooster.BusinessLayer.Models;
using WindesheimRooster.BusinessLayer.Response;

namespace WindesheimRooster.BusinessLayer
{
	public class WindesheimManager
	{
		public async static Task<IResponse> GetScheduleForClass(string className)
		{
			string schedule;
			if (NetworkInterface.GetIsNetworkAvailable())
			{
				schedule = await WindesheimAPI.GetScheduleForClass(className);
				await HistoryManager.SaveSchedule(className, schedule);
			}
			else
			{
				schedule = await HistoryManager.GetSchedule(className);
				if (schedule == null)
				{
					return new NoInternet();
				}
			}

			try
			{
				var res = JsonConvert.DeserializeObject<List<Les>>(schedule);
				return new Success<List<Les>>(res);
			}
			catch (Exception)
			{
				return new InvalidResponse();
			}
		}

		public async static Task<List<Klas>> GetAllClasses()
		{
			if (await HistoryManager.HasReleventClassList() || !NetworkInterface.GetIsNetworkAvailable())
			{
				return await HistoryManager.GetKlassen();
			}

			var rawClasses = await WindesheimAPI.GetClasses();
			await HistoryManager.SaveClassList(rawClasses);
			return JsonConvert.DeserializeObject<List<Klas>>(rawClasses);
		}
	}
}
