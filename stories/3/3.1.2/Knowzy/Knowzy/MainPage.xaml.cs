using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Knowzy
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            ProductListView.ItemsSource = await DataProvider.GetProducts();
        }

        private void ProductListViewItemTapped(object sender, ItemTappedEventArgs e)
        {
            Navigation.PushAsync(new CameraPage(e.Item as Nose));
        }
    }
}
