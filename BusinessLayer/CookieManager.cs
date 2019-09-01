using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace WindesheimRooster.BusinessLayer
{
	public class CookieManager
	{
		private static HttpCookie[] s_cookies = null;
		public static async void SetCookies(HttpCookie[] cookies)
		{
			s_cookies = cookies;
			await HistoryManager.SaveCookies(cookies);
		}

		public static async Task Initialize()
		{
			var cookies = await HistoryManager.GetCookies();
			if (cookies == null)
			{
				return;
			}
			s_cookies = cookies;
		}

		internal static HttpCookie[] GetCookies()
		{
			return s_cookies;
		}

		public static bool AreCookiesRelevant()
		{
			return s_cookies != null && s_cookies.All(x => x.Expires == null || x.Expires <= DateTimeOffset.Now);
		}
	}
}
