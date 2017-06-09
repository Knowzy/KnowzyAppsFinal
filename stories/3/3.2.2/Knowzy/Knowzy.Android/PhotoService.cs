using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using System.Threading.Tasks;

[assembly: Dependency(typeof(Knowzy.Droid.PhotoService))]
namespace Knowzy.Droid
{

    public class PhotoService : IPhotoService
    {
        public Task<ImageSource> TakePhotoAsync()
        {
            var mainActivity = Forms.Context as MainActivity;
            var tcs = new TaskCompletionSource<ImageSource>();
            EventHandler<Java.IO.File> handler = null;
            handler = (s, e) =>
            {
                tcs.SetResult(e.Path);
                mainActivity.ImageCaptured -= handler;
            };

            mainActivity.ImageCaptured += handler;
            mainActivity.StartMediaCaptureActivity();
            return tcs.Task;

        }
    }


}