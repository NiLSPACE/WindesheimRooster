using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WindesheimRooster.BusinessLayer;
using WindesheimRooster.BusinessLayer.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
	public sealed partial class HistoryPage : Page {
		struct FileInfo {
			public StorageFile File { get; private set; }
			public FileInfo(StorageFile file) {
				File = file;
			}

			public override string ToString() {
				var text = File.Name + "\n" + File.GetBasicPropertiesAsync().GetAwaiter().GetResult().DateModified.ToString("MMMM dd");
				return text;
			}
		}


		public HistoryPage() {
			this.InitializeComponent();
			LoadHistory();
		}

		private async void LoadHistory() {
			var history = await HistoryManager.GetHistory();
			foreach (var item in history) {
				lvHistoryList.Items.Add(new FileInfo(item));
			}
		}

		private void lvHistoryList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var className = ((FileInfo)e.AddedItems.First()).File.Name;
            Frame.Navigate(typeof(ClassSchedule), new string[] { className });
        }
	}
}
