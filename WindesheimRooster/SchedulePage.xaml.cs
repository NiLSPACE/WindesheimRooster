using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using WindesheimRooster.BusinessLayer;
using WindesheimRooster.BusinessLayer.Models.Classes;
using WindesheimRooster.BusinessLayer.Models.Schedule;
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

namespace WindesheimRooster {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SchedulePage : Page {
		private ClassInfo[] _currentClasses;
		private int _weekOffset = 0;
		private const string NO_SCHEDULE_FOUND_MESSAGE = "No schedule found";

		public SchedulePage() {
			this.InitializeComponent();
			CheckWeekOffsetButtons();
			NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
		}

		/// <summary>
		/// Called when a change happens in the connected network.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void NetworkChange_NetworkAddressChanged(object sender, EventArgs e) {
			// If the network status changes we don't want the week offset buttons to be active as we aren't able to retrieve the schedule.
			await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, CheckWeekOffsetButtons);
		}

		/// <summary>
		/// Enabled or disables the Week Offset buttons depending on the internet;
		/// </summary>
		private void CheckWeekOffsetButtons() {
			var hasInternet = NetworkInterface.GetIsNetworkAvailable();
			btnNextWeek.IsEnabled = hasInternet;
			btnPreviousWeek.IsEnabled = hasInternet;
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e) {
			// Reset the offset back to 0 in case the SchedulePage was used before.
			_weekOffset = 0;

			IEnumerable<Schedule> schedules;

			// If the provided argument is a ClassInfo object we need to retrieve the schedule from internet.
			// If it's a Schedule object we can use it directly.
			if (e.Parameter is IEnumerable<ClassInfo> currentClasses) {
				_currentClasses = currentClasses.ToArray();

				EnablePinButton(currentClasses);

				var scheduleTasks = currentClasses.Select(x => WindesheimManager.GetScheduleForClass(x.id.ToString(), x.displayname));
				schedules = await Task.WhenAll(scheduleTasks);
			}
			else if (e.Parameter is IEnumerable<Schedule> schedule) {
				schedules = schedule;
			}
			else {
				throw new Exception("Unexpected parameter provided. Type is: " + e.Parameter.GetType().Name);
			}

			UpdateSchedule(schedules);
		}

		/// <summary>
		/// Updates the screen with the provided schedule
		/// </summary>
		/// <param name="schedules"></param>
		private void UpdateSchedule(IEnumerable<Schedule> schedules) {
			// Remove the current items
			lvSchedule.Items.Clear();

			if (!schedules.Any(x => x?.result?.data?.elementPeriods != null)) {
				lvSchedule.Items.Add(NO_SCHEDULE_FOUND_MESSAGE);
				return;
			}

			string currentDate = DateTime.Now.ToString("dddd dd MMMM", CultureInfo.CurrentCulture);
			bool currentDateInSelectedWeek = false;
			var locations = schedules.SelectMany(schedule => schedule.result.data.elements.Where(x => x.type == 4));
			var teachers = schedules.SelectMany(schedule => schedule.result.data.elements.Where(x => x.type == 2));
			var lessons = schedules
				.SelectMany(x => x.result.data.elementPeriods)
				.SelectMany(x => x.Value)
				.OrderBy(x => x.date)
				.ThenBy(x => x.startTime)
				.GroupBy(x => new { x.date, x.lessonText });

			int? lastTime = null;
			foreach (var lesson in lessons) {
				if (lastTime == null || lastTime != lesson.First().date) {
					var date = DateTime.ParseExact(lesson.First().date.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
					var formattedDate = date.ToString("dddd dd MMMM", CultureInfo.CurrentCulture);
					lastTime = lesson.First().date;

					// lastTime is an integer with the format yyyyMMdd
					lvSchedule.Items.Add(formattedDate);

					// If the date is the current date automatically scroll to it.
					if (currentDate == formattedDate) {
						// Don't scroll into view just yet.
						// Not all items have been added which could cause issues.
						currentDateInSelectedWeek = true;
					}
				}

				StringBuilder builder = new StringBuilder();
				builder.AppendLine(lesson.First().lessonText);
				builder.AppendLine("Start: " + lesson.Min(x => x.startTime).ToString().Reverse().Insert(2, ":").Reverse());           // Add a : as the third character
				builder.AppendLine("End: " + lesson.Max(x => x.endTime).ToString().Reverse().Insert(2, ":").Reverse());               // Add a : as the third character
				builder.AppendLine("Location: " + locations.FirstOrDefault(x => x.id == lesson.First().elements.FirstOrDefault(y => y.type == 4)?.id)?.displayname ?? "<unknown>");
				builder.AppendLine("Teacher: " + teachers.FirstOrDefault(x => x.id == lesson.First().elements.FirstOrDefault(y => y.type == 2)?.id)?.displayname ?? "<unknown>");

				string text = builder.ToString();
				lvSchedule.Items.Add(text);

				// Select the lesson if the time and date matches the current time.
				if ((lesson.First().date.ToString() == DateTime.Now.ToString("yyyyMMdd")) && TimeIsCurrentSchedule(lesson)) {
					lvSchedule.SelectedItem = text;
				}
			}

			// If the current day is in the requested week scroll to the current day.
			if (currentDateInSelectedWeek) {
				lvSchedule.UpdateLayout();
				lvSchedule.ScrollIntoView(currentDate, ScrollIntoViewAlignment.Leading);
			}
		}

		/// <summary>
		/// Returns true if the lesson's time matches the current time.
		/// </summary>
		/// <param name="lessons"></param>
		/// <returns></returns>
		private static bool TimeIsCurrentSchedule(IGrouping<object, ElementPeriod> lessons) {
			// Horrible hack, please fix later
			var currentHour = Convert.ToInt32(DateTime.Now.ToString("HHmm"));
			if ((currentHour > lessons.Min(x => x.startTime)) &&
				(currentHour < lessons.Max(x => x.endTime))
			) {
				return true;
			}
			return false;
		}

		/// <summary>
		/// Enabled the pin button if the user doesn't have a tile yet.
		/// </summary>
		/// <param name="info"></param>
		private async void EnablePinButton(IEnumerable<ClassInfo> info) {
			var allTiles = await SecondaryTile.FindAllAsync();
			var exists = SecondaryTile.Exists("ScheduleTile" + String.Join("_", info.Select(x => x.displayname)));
			btnPin.IsEnabled = !exists;
		}

		private async void RequestCreateTile(ClassInfo[] info) {
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
			SecondaryTile secondaryTile = new SecondaryTile("ScheduleTile" + String.Join("_", info.Select(x => x.displayname)),
															 String.Join(", ", info.Select(x => x.displayname)),
															 String.Join(", ", info.Select(x => x.displayname)),
															square150x150Logo,
															TileSize.Square150x150);

			if (!(Windows.Foundation.Metadata.ApiInformation.IsTypePresent(("Windows.Phone.UI.Input.HardwareButtons")))) {
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

			if (await secondaryTile.RequestCreateAsync()) {
				btnPin.IsEnabled = false;
			}
		}

		private void AppBarButton_Click(object sender, RoutedEventArgs e) {
			RequestCreateTile(_currentClasses);
		}

		private void lvSchedule_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args) {
			// This is a hack to see if the item is a date header
			if (args.Item.Equals(NO_SCHEDULE_FOUND_MESSAGE)) {
				args.ItemContainer.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 200, 255, 200));
			}
			else if ((args.Item as string).Split('\n').Length == 1) {
				args.ItemContainer.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 240, 240, 240));
			}
			else {
				// For some reason the background color wasn't always the default so we specifically make it white here.
				args.ItemContainer.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
			}
		}

		private async void BtnNextWeek_Click(object sender, RoutedEventArgs e) {
			var week = _weekOffset++;
			var scheduleTasks = _currentClasses.Select(x => WindesheimManager.GetScheduleForClass(x.id.ToString(), x.displayname, week));
			var schedules = await Task.WhenAll(scheduleTasks);
			UpdateSchedule(schedules);
		}

		private async void BtnPreviousWeek_Click(object sender, RoutedEventArgs e) {
			var week = _weekOffset--;
			var scheduleTasks = _currentClasses.Select(x => WindesheimManager.GetScheduleForClass(x.id.ToString(), x.displayname, week));
			var schedules = await Task.WhenAll(scheduleTasks);
			UpdateSchedule(schedules);
		}
	}
}
