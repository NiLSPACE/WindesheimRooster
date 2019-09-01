using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindesheimRooster.BusinessLayer.Models;
using Windows.Web.Http;

namespace WindesheimRooster.BusinessLayer
{
	public class WindesheimAPI
	{
		private static HttpRequestMessage CreateRequest(HttpMethod method, Uri uri)
		{
			var request = new HttpRequestMessage(method, uri);
			request.Headers.Accept.Add(new Windows.Web.Http.Headers.HttpMediaTypeWithQualityHeaderValue("application/json"));
			request.Headers.Host = new Windows.Networking.HostName("windesheimapi.azurewebsites.net");
			var cookies = CookieManager.GetCookies();
			foreach (var cookie in cookies)
			{
				request.Headers.Cookie.Add(new Windows.Web.Http.Headers.HttpCookiePairHeaderValue(cookie.Name, cookie.Value));
			}
			return request;
		}

		internal static async Task<string> GetScheduleForClass(string className)
		{
			var uri = new Uri($"https://windesheimapi.azurewebsites.net/api/v1/Klas/{className}/Les");
			var request = CreateRequest(HttpMethod.Get, uri);
			var client = new HttpClient();
			var result = await client.SendRequestAsync(request);
			var content = await result.Content.ReadAsStringAsync();
			return content;
		}

		public static async Task<string> GetClasses()
		{
			Uri uri = new Uri("https://windesheimapi.azurewebsites.net/api/v1/Klas/");
			var request = CreateRequest(HttpMethod.Get, uri);
			var client = new HttpClient();
			var result = await client.SendRequestAsync(request);
			var content = await result.Content.ReadAsStringAsync();
			return content;
		}
	}
}
