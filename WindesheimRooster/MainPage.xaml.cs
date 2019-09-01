using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WindesheimRooster.BusinessLayer;
using WindesheimRooster.BusinessLayer.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
	public sealed partial class MainPage : Page
	{
		private List<Klas> _classes { get; set; }
		List<Klas> _selectedClasses = new List<Klas>();

		public MainPage()
		{
			this.InitializeComponent();
		}
		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			_classes = await WindesheimManager.GetAllClasses();
			FillListBox();
		}

		private void FillListBox()
		{
			lvClassList.ItemsSource = _classes.Where(x => x.klasnaam.ToLower().Contains(tbClassFilter.Text.ToLower()));
			//lbClasses.Items.Add()
		}

		private void TbClassFilter_TextChanged(object sender, TextChangedEventArgs e)
		{
			FillListBox();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var klas = (sender as Button)?.Tag as Klas;
			if (klas == null)
			{
				return;
			}
			Frame.Navigate(typeof(ClassSchedule), new string[] { klas.code });
		}

		private void CheckBox_Tapped(object sender, TappedRoutedEventArgs e)
		{
			var checkbox = sender as CheckBox;
			var @class = checkbox.Tag as Klas;
			if (checkbox.IsChecked == true)
			{
				_selectedClasses.Add(@class);
			}
			else
			{
				_selectedClasses.Remove(@class);
			}
		}

		private void CheckBox_Loaded(object sender, RoutedEventArgs e)
		{
			var checkbox = sender as CheckBox;
			checkbox.IsChecked = _selectedClasses.Contains(checkbox.Tag as Klas);
		}

		private void History_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(HistoryPage));
		}

		private void ViewMergedList_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(ClassSchedule), _selectedClasses.Select(x => x.code));
		}
	}
}
