using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Knowzy
{
	public partial class App : Application
	{
        public App( Dictionary <string, string> parameters = null  )
        {
            InitializeComponent();

            string task = string.Empty;
            string noseId = string.Empty;
            if (parameters != null)
            {
                parameters.TryGetValue(Constants.TaskParam, out task);
                parameters.TryGetValue(Constants.NoseParam, out noseId);
            } 
                         
            if (task == Constants.CaptureImageTask  )
            {
                MainPage = new NavigationPage(new CameraPage(noseId)); 
            } 
            else
                MainPage = new NavigationPage(new MainPage());

        }


		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
