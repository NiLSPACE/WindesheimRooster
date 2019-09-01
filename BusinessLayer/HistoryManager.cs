using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WindesheimRooster.BusinessLayer.Models;
using Windows.Storage;
using Windows.Web.Http;

namespace WindesheimRooster.BusinessLayer
{
	public class HistoryManager
	{
		const string CLASS_FILE = "CLASS_LIST.json";
		static StorageFolder s_historyFolder;
		static StorageFolder s_additionalData;
		static HistoryManager()
		{
			s_historyFolder = ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("history", CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();
			s_additionalData = ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("additionalData", CreationCollisionOption.OpenIfExists).GetAwaiter().GetResult();
		}

		/// <summary>
		/// Returns a list of all the viewed schedules as a StorageFile.
		/// </summary>
		/// <returns></returns>
		public static async Task<StorageFile[]> GetHistory()
		{
			var files = await s_historyFolder.GetFilesAsync();
			return files.OrderBy(x => x.DateCreated).ToArray();
		}

		class CookieWrapper
		{
			public CookieWrapper() { }
			public CookieWrapper(HttpCookie cookie)
			{
				foreach (var pi in typeof(CookieWrapper).GetProperties())
				{
					pi.SetValue(this, cookie.GetType().GetProperty(pi.Name).GetValue(cookie));
				}
			}
			public string Value { get; set; }
			public bool Secure { get; set; }
			public bool HttpOnly { get; set; }
			public DateTimeOffset? Expires { get; set; }
			public string Domain { get; set; }
			public string Name { get; set; }
			public string Path { get; set; }

			public HttpCookie ToHttpCookie()
			{
				var cookie = new HttpCookie(Name, Domain, Path);
				foreach (var pi in typeof(HttpCookie).GetProperties().Where(x => x.CanWrite))
				{
					pi.SetValue(cookie, this.GetType().GetProperty(pi.Name).GetValue(this));
				}
				return cookie;
			}
		}



		public static async Task SaveCookies(HttpCookie[] cookies)
		{
			var file = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("cookies.json", CreationCollisionOption.ReplaceExisting);
			string json = JsonConvert.SerializeObject(cookies.Select(x => new CookieWrapper(x)));
			await FileIO.WriteTextAsync(file, json);
		}

		public static async Task<HttpCookie[]> GetCookies()
		{
			var file = await ApplicationData.Current.LocalCacheFolder.TryGetItemAsync("cookies.json");
			if (file == null || !(file is StorageFile))
			{
				return null;
			}
			var content = await FileIO.ReadTextAsync(file as StorageFile);
			return JsonConvert.DeserializeObject<CookieWrapper[]>(content).Select(x => x.ToHttpCookie()).ToArray();
		}

		public static async Task<List<Klas>> GetKlassen()
		{
			var item = await s_additionalData.TryGetItemAsync(CLASS_FILE);
			if (item == null || !(item is StorageFile))
			{
				return null;
			}
			var content = await FileIO.ReadTextAsync(item as StorageFile);
			return JsonConvert.DeserializeObject<List<Klas>>(content);
		}

		internal static async Task SaveSchedule(string className, string schedule)
		{
			var file = await s_historyFolder.CreateFileAsync(className, CreationCollisionOption.ReplaceExisting);
			await FileIO.WriteTextAsync(file, schedule);
		}

		internal static async Task<string> GetSchedule(string className)
		{
			var file = await s_historyFolder.TryGetItemAsync(className);

			if (file == null || !(file is StorageFile))
			{
				return null;
			}
			return await FileIO.ReadTextAsync(file as StorageFile);
		}

		internal static async Task SaveClassList(string content)
		{
			var file = await s_additionalData.CreateFileAsync(CLASS_FILE, CreationCollisionOption.ReplaceExisting);
			await FileIO.WriteTextAsync(file, content);
		}

		public static async Task<bool> HasReleventClassList()
		{
			var item = await s_additionalData.TryGetItemAsync(CLASS_FILE);
			if (item == null || !(item is StorageFile))
			{
				return false;
			}
			if ((DateTime.Now - item.DateCreated) > TimeSpan.FromDays(7))
			{
				await item.DeleteAsync();
				return false;
			}
			return true;
		}
	}
}
