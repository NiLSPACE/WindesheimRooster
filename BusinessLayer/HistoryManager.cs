using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindesheimRooster.BusinessLayer.Models.Classes;
using WindesheimRooster.BusinessLayer.Models.Schedule;
using Windows.Storage;

namespace WindesheimRooster.BusinessLayer {
	public class HistoryManager {
		const string CLASS_FILE = "CLASS_LIST.json";
		static StorageFolder s_historyFolder;
		static StorageFolder s_additionalData;
		static HistoryManager() {
			s_historyFolder = ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("history", CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();
			s_additionalData = ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("additionalData", CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();
		}

		/// <summary>
		/// Returns a list of all the viewed schedules as a StorageFile.
		/// </summary>
		/// <returns></returns>
		public static async Task<StorageFile[]> GetHistory() {
			var files = await s_historyFolder.GetFilesAsync();
			return files.OrderBy(x => x.DateCreated).ToArray();
		}

		/// <summary>
		/// Writes the json content of a schedule to a file named as the classname.
		/// </summary>
		/// <param name="className"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		internal static async Task AddToHistory(string className, string content) {
			var file = await s_historyFolder.CreateFileAsync(className, CreationCollisionOption.ReplaceExisting);
			await FileIO.WriteTextAsync(file, content);
		}

		public static async Task<Schedule> GetScheduleFor(string className) {
			var item = await s_historyFolder.TryGetItemAsync(className);
			if (item == null || !(item is StorageFile)) {
				return null;
			}
			var json = await FileIO.ReadTextAsync(item as StorageFile);
			return JsonConvert.DeserializeObject<Schedule>(json);
		}

		public static async Task SaveClassList(string content) {
			var file = await s_additionalData.CreateFileAsync(CLASS_FILE, CreationCollisionOption.ReplaceExisting);
			await FileIO.WriteTextAsync(file, content);
		}

		public static async Task<bool> HasReleventClassList() {
			var item = await s_additionalData.TryGetItemAsync(CLASS_FILE);
			if (item == null || !(item is StorageFile)) {
				return false;
			}
			if ((item.DateCreated - DateTime.Now) > TimeSpan.FromDays(7)) {
				return false;
			}
			return true;
		}

		internal static async Task<ClassInfo[]> GetClassList() {
			var item = await s_additionalData.TryGetItemAsync(CLASS_FILE);
			if (item == null || !(item is StorageFile)) {
				return null;
			}
			var content = await FileIO.ReadTextAsync(item as StorageFile);
			return JsonConvert.DeserializeObject<ClassJsonResponse>(content).elements;
		}
	}
}
