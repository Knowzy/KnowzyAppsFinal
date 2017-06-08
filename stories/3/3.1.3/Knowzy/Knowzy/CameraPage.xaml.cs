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

#if WINDOWS_UWP
    var inkingWrapper = (Xamarin.Forms.Platform.UWP.NativeViewWrapper)InkingContent.Content;
    var inkCanvas = (Windows.UI.Xaml.Controls.InkCanvas)inkingWrapper.NativeElement;
    inkCanvas.InkPresenter.InputDeviceTypes =
        Windows.UI.Core.CoreInputDeviceTypes.Touch |
        Windows.UI.Core.CoreInputDeviceTypes.Mouse |
        Windows.UI.Core.CoreInputDeviceTypes.Pen;

    var inkToolbarWrapper = (Xamarin.Forms.Platform.UWP.NativeViewWrapper)InkingToolbar.Content;
    var inkToolbar = (Windows.UI.Xaml.Controls.InkToolbar)inkToolbarWrapper.NativeElement;
    inkToolbar.TargetInkCanvas = inkCanvas;
#endif
        }

        private async void captureButton_Clicked(object sender, EventArgs e)
        {
            var photoService = DependencyService.Get<IPhotoService>();
            if (photoService != null)
            {
                var source = await photoService.TakePhotoAsync();
                noseImage.Source = ImageSource.FromUri(new Uri(_nose.Image)); // set source of nose image
                image.Source = source;
                imageGrid.IsVisible = true; // set visibility to true
            }

        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    var bounds = AbsoluteLayout.GetLayoutBounds(noseImage);
                    bounds.X += noseImage.TranslationX;
                    bounds.Y += noseImage.TranslationY;
                    AbsoluteLayout.SetLayoutBounds(noseImage, bounds);
                    noseImage.TranslationX = 0;
                    noseImage.TranslationY = 0;
                    break;

                case GestureStatus.Running:
                    noseImage.TranslationX = e.TotalX;
                    noseImage.TranslationY = e.TotalY;
                    break;
            }
        }

        private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            switch (e.Status)
            {
                case GestureStatus.Running:
                    noseImage.Scale *= e.Scale;
                    break;
            }
        }
    }
}