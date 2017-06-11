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
        IRemoteLaunchService launchService;
        public MainPage()
        {
            InitializeComponent();
            InitializeLaunchService();
        }
        
        async void InitializeLaunchService() 
        {            
            launchService = DependencyService.Get<IRemoteLaunchService>(DependencyFetchTarget.GlobalInstance); 
            if (launchService != null )
            {
                if (launchService.AuthStatus == AuthenticationStatus.NotStarted)
                {
                    launchService.AuthStatusChanged += LaunchService_AuthStatusChanged;
                    launchService.DiscoveryCompleted += LaunchService_DiscoveryCompleted;
#if __ANDROID__
                    bool initialized = await launchService.InitializeAsync(Forms.Context);
                     
#endif
                } 
                else
                {
                    OnRemoteLaunchStatusChange( ); 
                }
            }
        }

        private void LaunchService_DiscoveryCompleted(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("KNOWZY: Discovery completed"); 
            Device.BeginInvokeOnMainThread(() =>
            {
                OnRemoteLaunchDiscoveryComplete(false );
               
            });
        }

        private void OnRemoteLaunchDiscoveryComplete( bool force)
        {
            if ( launchService.HasRemoteSystemsAvailable || force )
            {
                useRemoteLaunch.IsVisible = true;
                useRemoteDiscovery.IsVisible = false;
                useRemoteDiscovery.IsEnabled = false; 
                useRemoteDiscovery.Progress = 1.0f; 
            }
        }

        void UpdateProgress ( float percent )
        {
            useRemoteDiscovery.Progress = 1.0f; 

        }


        float timerProgress = 0f; 
        private void OnRemoteLaunchStatusChange( )
        {
            remoteLaunchContainer.IsVisible = (launchService.AuthStatus == AuthenticationStatus.Authenticated);
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
           {
               useRemoteDiscovery.Progress += .03f;

               if (useRemoteDiscovery.Progress < 1.0f)
               {
                   OnRemoteLaunchDiscoveryComplete(true); 
                   return false; 
               }
               else
                   return true;
           }); 
        }

        private void LaunchService_AuthStatusChanged(object sender, EventArgs e)
        {
            if (launchService.AuthStatus == AuthenticationStatus.Authenticated)
            {
                System.Diagnostics.Debug.WriteLine("KNOWZY: Authenticated");
                launchService.StartDiscovery();
                
            } 
            Device.BeginInvokeOnMainThread(() =>
            {
                OnRemoteLaunchStatusChange(); 
            });                         
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            ProductListView.ItemsSource = await DataProvider.GetProducts();
           
        }

        private void ProductListViewItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!useRemoteLaunch.IsToggled)
            {
                Navigation.PushAsync(new CameraPage(e.Item as Nose));
            } 
            else
            {
                Navigation.PushAsync(new RemoteSystemSelectorPage(e.Item as Nose)); 
            }
        }
    }
}
