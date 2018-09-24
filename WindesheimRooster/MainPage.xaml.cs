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
		List<ClassInfo> _selectedClasses = new List<ClassInfo>();
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

			var requestedClasses = (e.Parameter as string).Split(',').Select(x => x.Trim());
			if (NetworkInterface.GetIsNetworkAvailable()) {
				// Make sure the classes are loaded.
				await _classLoadingTask;

				var items = requestedClasses.Select(x => _classes.First(@class => @class.displayname == x)).ToArray();
				Frame.Navigate(typeof(SchedulePage), items);
			}
			else {
				var cachedSchedules = await Task.WhenAll(requestedClasses.Select(x => HistoryManager.GetScheduleFor(x)));
				Frame.Navigate(typeof(SchedulePage), cachedSchedules);
			}
		}

		private void FillClassList() {
			var currentSelected = lvClassList.SelectedItems.ToArray();
			lvClassList.ItemsSource = _classes.Where(x => x.displayname.ToLower().Contains(tbFilter.Text.ToLower()));
			foreach (var selected in currentSelected) {
				lvClassList.SelectedItems.Add(selected);
			}
		}

		private void tbFilter_TextChanged(object sender, TextChangedEventArgs e) {
			FillClassList();
		}

		private void History_Click(object sender, RoutedEventArgs e) {
			Frame.Navigate(typeof(HistoryPage));
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			var classInfo = (sender as Button)?.Tag;
			if (classInfo == null) {
				return;
			}
			Frame.Navigate(typeof(SchedulePage), new ClassInfo[] { classInfo as ClassInfo });
		}

		private void ViewMergedList_Click(object sender, RoutedEventArgs e) {
			Frame.Navigate(typeof(SchedulePage), _selectedClasses);
		}

		private void CheckBox_Tapped(object sender, TappedRoutedEventArgs e) {
			var checkbox = sender as CheckBox;
			var @class = checkbox.Tag as ClassInfo;
			if (checkbox.IsChecked == true) {
				_selectedClasses.Add(@class);
			}
			else {
				_selectedClasses.Remove(@class);
			}
		}

		private void CheckBox_Loaded(object sender, RoutedEventArgs e) {
			var checkbox = sender as CheckBox;
			checkbox.IsChecked = _selectedClasses.Contains(checkbox.Tag as ClassInfo);
		}
	}
}
