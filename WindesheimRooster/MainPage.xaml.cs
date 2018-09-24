using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WindesheimRooster.BusinessLayer;
using WindesheimRooster.BusinessLayer.Models.Classes;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WindesheimRooster {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page {
		const string NO_INTERNET_CONNECTION_MESSAGE = "No internet connection";
		ClassInfo[] _classes;
		Task _classLoadingTask;
		public MainPage() {
			this.InitializeComponent();
			_classLoadingTask = LoadClasses();
		}

		private async Task LoadClasses() {
			if (NetworkInterface.GetIsNetworkAvailable()) {
				try {
					_classes = await WindesheimManager.GetAllClasses();
					FillClassList();
					tbFilter.IsEnabled = true;
				}
				catch (Exception e) {
					lvClassList.Items.Add(NO_INTERNET_CONNECTION_MESSAGE);
				}
			}
			else {
				lvClassList.Items.Add(NO_INTERNET_CONNECTION_MESSAGE);
			}
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e) {
			if (String.IsNullOrEmpty(e.Parameter as string)) {
				return;
			}

			if (NetworkInterface.GetIsNetworkAvailable()) {
				// Make sure the classes are loaded.
				await _classLoadingTask;

				var item = _classes.First(x => x.displayname == e.Parameter as string);
				Frame.Navigate(typeof(SchedulePage), item);
			}
			else {
				var cachedSchedule = await HistoryManager.GetScheduleFor(e.Parameter as string);
				Frame.Navigate(typeof(SchedulePage), cachedSchedule);
			}
		}

		private void FillClassList() {
			lvClassList.Items.Clear();
			foreach (var item in _classes.Where(x => x.displayname.ToLower().Contains(tbFilter.Text.ToLower()))) {
				lvClassList.Items.Add(item);
			}
		}

		private void lvClassList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			var item = e.AddedItems.First();
			if (!(item is ClassInfo)) {
				return;
			}
			Frame.Navigate(typeof(SchedulePage), (ClassInfo)item);
		}

		private void tbFilter_TextChanged(object sender, TextChangedEventArgs e) {
			FillClassList();
		}

		private void History_Click(object sender, RoutedEventArgs e) {
			Frame.Navigate(typeof(HistoryPage));
		}
	}
}
