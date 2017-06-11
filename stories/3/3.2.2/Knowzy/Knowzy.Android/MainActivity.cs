using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Java.IO;
using Android.Content;
using Android.Provider;

namespace Knowzy.Droid
{
	[Activity (Label = "Knowzy", Icon = "@drawable/icon", Theme="@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{

        static readonly File file = new File(Android.OS.Environment.GetExternalStoragePublicDirectory(
    Android.OS.Environment.DirectoryPictures), "tmp.jpg");


        protected override void OnCreate (Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar; 

			base.OnCreate (bundle);
             
			global::Xamarin.Forms.Forms.Init (this, bundle);
            LoadApplication(new Knowzy.App());
		}


        public void StartMediaCaptureActivity()
        {
            var intent = new Intent(MediaStore.ActionImageCapture);
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(file));
            StartActivityForResult(intent, 0);
        }

        public event EventHandler<File> ImageCaptured;

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == 0 && resultCode == Result.Ok)
            {
                ImageCaptured?.Invoke(this, file);
            }
        }
    }
}

