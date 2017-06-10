using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Knowzy.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Dictionary <string, string> queryParams = null;
            var paramQueryString = e.Parameter as string; 
            if ( ! string.IsNullOrEmpty ( paramQueryString )) 
            {
                queryParams = new Dictionary<string, string>(); 
                WwwFormUrlDecoder decoder = new WwwFormUrlDecoder(paramQueryString);
                var list = decoder.ToList(); 
                foreach ( var entry in list )
                {
                    queryParams.Add(entry.Name, entry.Value); 
                }
            }

            LoadApplication(new Knowzy.App(queryParams)); 
        }
    }
}
