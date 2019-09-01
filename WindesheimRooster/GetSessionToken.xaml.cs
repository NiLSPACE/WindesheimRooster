using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WindesheimRooster.BusinessLayer;
using WindesheimRooster.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http.Filters;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WindesheimRooster
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class GetSessionToken : Page
	{
		NavigationParameter _redirectTo = null;
		public GetSessionToken()
		{
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			var targetUri = new Uri("https://windesheimapi.azurewebsites.net");
			wvSession.Navigate(targetUri);
			wvSession.LoadCompleted += WvSession_LoadCompleted;
			var httpBaseProtocolFilter = new HttpBaseProtocolFilter();
			var cookieManager = httpBaseProtocolFilter.CookieManager;
			var cookieCollection = cookieManager.GetCookies(targetUri);
			_redirectTo = e.Parameter as NavigationParameter;
		}

		private void WvSession_LoadCompleted(object sender, NavigationEventArgs e)
		{
			if (wvSession.Source.Host != "windesheimapi.azurewebsites.net")
			{
				return;
			}
			var httpBaseProtocolFilter = new HttpBaseProtocolFilter();
			var cookieManager = httpBaseProtocolFilter.CookieManager;
			var cookieCollection = cookieManager.GetCookies(wvSession.Source);
			var cookies = cookieCollection.ToArray();
			CookieManager.SetCookies(cookies);
			if (_redirectTo != null)
			{
				Frame.Navigate(_redirectTo.RedirectTo, _redirectTo.Parameter);
			}
			else
			{
				Frame.Navigate(typeof(MainPage));
			}
			//var r = cookieCollection.ToDictionary(x => x.Name, x => x.Value).Select(x => new { x.Name, x.Value }).ToArray();
			//throw new NotImplementedException();
		}
	}
}
