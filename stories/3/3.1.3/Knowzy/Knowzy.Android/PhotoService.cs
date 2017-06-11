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
using System.IO;

[assembly: Dependency(typeof(Knowzy.Droid.PhotoService))]
namespace Knowzy.Droid
{

    public class PhotoService : IPhotoService
    {
        public Task<byte[]> TakePhotoAsync()
        {
            var mainActivity = Forms.Context as MainActivity;
            var tcs = new TaskCompletionSource<byte[]>();
            EventHandler<Java.IO.File> handler = null;
            handler = (s, e) =>
            {
                using (var streamReader = new StreamReader(e.Path))
                {
                    using (var memstream = new MemoryStream())
                    {
                        streamReader.BaseStream.CopyTo(memstream);
                        tcs.SetResult(memstream.ToArray());
                    }
                }
                mainActivity.ImageCaptured -= handler;
            };

            mainActivity.ImageCaptured += handler;
            mainActivity.StartMediaCaptureActivity();
            return tcs.Task;
        }
    }


}