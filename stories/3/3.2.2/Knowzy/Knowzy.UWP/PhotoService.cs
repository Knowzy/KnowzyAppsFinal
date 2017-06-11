using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Windows.Media.Capture;
using Windows.Storage; 


[assembly: Dependency(typeof(Knowzy.UWP.PhotoService))]
namespace Knowzy.UWP
{
    public class PhotoService : IPhotoService 
    {

        public async Task<ImageSource> TakePhotoAsync()
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;

            StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (photo == null) return null;

            return ImageSource.FromFile(photo.Path);


        }

    }
}
