
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Knowzy
{
	 
	public partial class RemoteSystemSelectorView : ContentView
	{
        IRemoteLaunchService service;
        
		public RemoteSystemSelectorView ( )
		{
			InitializeComponent ();

            service = DependencyService.Get<IRemoteLaunchService>(DependencyFetchTarget.GlobalInstance);

#if __ANDROID__ 
            this.remoteSystemsList.ItemsSource = service.RemoteSystems;
#endif 

        }

        async private void RemoteSystemSelected(object sender, ItemTappedEventArgs e)
        {

#if __ANDROID__

            Page page = Navigation.NavigationStack.LastOrDefault(); 

            var result = await service.LaunchAsync((e.Item as Microsoft.ConnectedDevices.RemoteSystem).Id, SelectedNose.Id);
            await page.DisplayAlert("Launch Result", result.ToString(), "OK"); 
#endif 
                
         }

        public Nose SelectedNose { get; set;  }
    }
}