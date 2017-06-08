using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Knowzy
{
	// [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CameraPage : ContentPage
	{
        Nose _nose;

        public CameraPage(Nose nose)
        {
            _nose = nose;
            InitializeComponent();
        }

        private async void captureButton_Clicked(object sender, EventArgs e)
        {
            var photoService = DependencyService.Get<IPhotoService>();
            if (photoService != null)
            {
                var source = await photoService.TakePhotoAsync();
                image.Source = source;
            }
        }
    }
}