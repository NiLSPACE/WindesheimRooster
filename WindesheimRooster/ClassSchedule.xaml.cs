using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WindesheimRooster.BusinessLayer;
using WindesheimRooster.BusinessLayer.Models;
using WindesheimRooster.BusinessLayer.Response;
using WindesheimRooster.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WindesheimRooster
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ClassSchedule : Page
	{
		static readonly string NO_INTERNET_AVAILBLE = "There is no internet connection.";
		IEnumerable<string> _klassen;

		public ClassSchedule()
		{
			this.InitializeComponent();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			if (e.Parameter == null)
			{
				return;
			}

			_klassen = e.Parameter as IEnumerable<string>;
			if (_klassen == null)
			{
				return;
			}

			if (!_klassen.Any())
			{
				return;
			}

			EnablePinButton(_klassen);

			if (!CookieManager.AreCookiesRelevant())
			{
				Frame.Navigate(typeof(GetSessionToken), new NavigationParameter() { Parameter = e.Parameter, RedirectTo = typeof(ClassSchedule) });
				return;
			}

			var requests = await Task.WhenAll(_klassen
				.AsParallel()
				.Select(async x => await WindesheimManager.GetScheduleForClass(x)));

			if (requests.Any(x => x is NoInternet))
			{
				lvSchedule.Items.Add(NO_INTERNET_AVAILBLE);
				return;
			}

			if (requests.Any(x => x is InvalidCookie))
			{
				Frame.Navigate(typeof(GetSessionToken), new NavigationParameter() { Parameter = e.Parameter, RedirectTo = typeof(ClassSchedule) });
				return;
			}


			var completeSchedule = requests.Cast<Success<List<Les>>>().SelectMany(x => x.Value)
				.OrderBy(x => x.roosterdatum)
				.ThenBy(x => x.starttijd)
				.GroupBy(x => x.roosterdatum)
				.ToArray();

			foreach (var day in completeSchedule)
			{
				var date = day.Key.ToString("dddd dd MMMM", CultureInfo.CurrentCulture);
				lvSchedule.Items.Add(date);

				foreach (var lesson in day)
				{
					lvSchedule.Items.Add(lesson);
				}
			}
		}

		/// <summary>
		/// Enabled the pin button if the user doesn't have a tile yet.
		/// </summary>
		/// <param name="info"></param>
		private async void EnablePinButton(IEnumerable<string> info)
		{
			var allTiles = await SecondaryTile.FindAllAsync();
			var exists = SecondaryTile.Exists("ScheduleTile" + String.Join("_", info));
			btnPin.IsEnabled = !exists;
		}

		private void lvSchedule_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
		{
			if (args.Item is string str)
			{
				if (str == NO_INTERNET_AVAILBLE)
				{
					args.ItemContainer.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 0, 0));
					args.ItemContainer.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
				}
				else
				{
					args.ItemContainer.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 75, 75, 75));
				}
			}
			else
			{
				// For some reason the background color wasn't always the default so we specifically make it white here.
				args.ItemContainer.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
			}
		}

		private async void RequestCreateTile(IEnumerable<string> info)
		{
			// Prepare package images for all four tile sizes in our tile to be pinned as well as for the square30x30 logo used in the Apps view.  
			Uri square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png");
			Uri wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.scale-200.png");
			Uri square310x310Logo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png");
			Uri square30x30Logo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png");

			// Create a Secondary tile with all the required arguments.
			// Note the last argument specifies what size the Secondary tile should show up as by default in the Pin to start fly out.
			// It can be set to TileSize.Square150x150, TileSize.Wide310x150, or TileSize.Default.  
			// If set to TileSize.Wide310x150, then the asset for the wide size must be supplied as well.
			// TileSize.Default will default to the wide size if a wide size is provided, and to the medium size otherwise. 
			SecondaryTile secondaryTile = new SecondaryTile("ScheduleTile" + String.Join("_", info.Select(x => x.Replace(' ', '$'))),
															 String.Join(", ", info),
															 String.Join(", ", info),
															square150x150Logo,
															TileSize.Square150x150);

			if (!(Windows.Foundation.Metadata.ApiInformation.IsTypePresent(("Windows.Phone.UI.Input.HardwareButtons"))))
			{
				secondaryTile.VisualElements.Wide310x150Logo = wide310x150Logo;
				secondaryTile.VisualElements.Square310x310Logo = square310x310Logo;
			}

			secondaryTile.VisualElements.ForegroundText = ForegroundText.Light;

			// The display of the secondary tile name can be controlled for each tile size.
			// The default is false.
			secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
			secondaryTile.VisualElements.ShowNameOnWide310x150Logo = true;
			secondaryTile.VisualElements.ShowNameOnSquare310x310Logo = true;

			// Specify a foreground text value.
			// The tile background color is inherited from the parent unless a separate value is specified.
			secondaryTile.VisualElements.ForegroundText = ForegroundText.Light;

			// Set this to false if roaming doesn't make sense for the secondary tile.
			// The default is true;
			secondaryTile.RoamingEnabled = false;

			if (await secondaryTile.RequestCreateAsync())
			{
				btnPin.IsEnabled = false;
			}
		}

		private void AppBarPin_Click(object sender, RoutedEventArgs e)
		{
			RequestCreateTile(_klassen);
		}
	}
}
