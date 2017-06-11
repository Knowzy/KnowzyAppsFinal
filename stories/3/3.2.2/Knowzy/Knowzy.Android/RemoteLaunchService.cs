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
using Microsoft.ConnectedDevices;
using Android.Webkit;

[assembly: Dependency(typeof(Knowzy.Droid.RemoteLaunchService))]
namespace Knowzy.Droid
{

    public class RemoteLaunchService : IRemoteLaunchService
    {
        AuthenticationStatus authStatus = AuthenticationStatus.NotStarted;
        private Context context;
        private Android.Webkit.WebView  webView;
        private Dialog authDialog;
        const string RomeClientId = "99cbef66-1bf1-42bb-8632-3012aa74b54d";
        private RemoteSystemWatcher remoteSystemWatcher;
        private List<RemoteSystem> remoteSystems = new List<RemoteSystem>();
        static RemoteLaunchService instance = null;


        public RemoteLaunchService()
        {
            if (instance != null)
            {
                System.Diagnostics.Debug.Assert(false, "Use DependencyService.Get(DependencyFetchTarget.GlobalInstance) to instantiate");
                throw new InvalidOperationException("Use DependencyService.Get(DependencyFetchTarget.GlobalInstance) to instantiate");
            }
            instance = this;
        }


        public AuthenticationStatus AuthStatus {
            get
            {
                return authStatus;     
            }
            private set
            {
                if (authStatus != value)
                {  
                    authStatus = value;
                    AuthStatusChanged?.Invoke(this, null); 
                }    
            }
        }  

        public event EventHandler<RemoteSystemUpdatedEventArgs> RemoteSystemUpdated;
        public event EventHandler<RemoteSystemAddedEventArgs> RemoteSystemAdded;
        public event EventHandler<RemoteSystemRemovedEventArgs> RemoteSystemRemoved;
        public event EventHandler AuthStatusChanged ;
        public event EventHandler DiscoveryCompleted;

        public List<Microsoft.ConnectedDevices.RemoteSystem> RemoteSystems
        {
            get
            {
                return remoteSystems; 
            }
        } 

        async public Task<bool> InitializeAsync ( object ctx )
        {
            this.context = ctx as Context;

            if (this.context == null)
            {
                throw new InvalidOperationException("Context cannot be null and must be Android.Content.Context instance ");
            }              

            Platform.FetchAuthCode += Platform_FetchAuthCode;
            authStatus = AuthenticationStatus.InProgress; 
            try
            {
                var result = await Platform.InitializeAsync( context , RomeClientId);
                if ( authStatus == AuthenticationStatus.InProgress && result )
                {
                    AuthStatus = AuthenticationStatus.Authenticated; 
                }
                return result;

            }
            catch (Exception ex)
            {
                //TODO: Error handling 
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return false;
        }

        private RemoteSystem FindById ( string id )
        {
            foreach ( var system in RemoteSystems )
            {
                if( system.Id == id )
                {
                    return system; 
                }
            }
            return null; 
        }
        async public Task<RemoteLaunchUriStatus> LaunchAsync ( string id , string noseId )
        {

            var system = FindById(id);  
            if (system != null)
            {
                var request = new Microsoft.ConnectedDevices.RemoteSystemConnectionRequest(system);
                Uri uri = new Uri( $"knowzy://?{Constants.TaskParam}={Constants.CaptureImageTask}&{Constants.NoseParam}={noseId}");
                var launchUriStatus = await Microsoft.ConnectedDevices.RemoteLauncher.LaunchUriAsync(request, uri);
                return launchUriStatus; 
            }

            return RemoteLaunchUriStatus.Unknown; 
        }




        private void Platform_FetchAuthCode(string oauthUrl)
        {
            authDialog = new Dialog(context);             
            var linearLayout = new LinearLayout(authDialog.Context);
            webView = new Android.Webkit.WebView(authDialog.Context);
            linearLayout.AddView(webView);
            authDialog.SetContentView(linearLayout);             
            webView.SetWebChromeClient(new WebChromeClient());
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.DomStorageEnabled = true;
            webView.LoadUrl(oauthUrl);             
            webView.SetWebViewClient(new MsaWebViewClient(this));
            authDialog.Show();
            authDialog.SetCancelable(true);
        }
         
        public bool  HasRemoteSystemsAvailable 
        {
            get
            {
                return remoteSystems.Count > 0;
            }
        }

       

        public void StartDiscovery()
        {
            if ( authStatus != AuthenticationStatus.Authenticated )
            {
                throw new InvalidOperationException("Can't discover until you are authenticated"); 
            }

            if (remoteSystemWatcher == null)
            {
                lock (instance)
                {
                    remoteSystemWatcher = RemoteSystem.CreateWatcher();

                    //hook up event handlers
                    remoteSystemWatcher.RemoteSystemAdded += RemoteSystemWatcher_RemoteSystemAdded; ;
                    remoteSystemWatcher.RemoteSystemRemoved += RemoteSystemWatcher_RemoteSystemRemoved; ;
                    remoteSystemWatcher.RemoteSystemUpdated += RemoteSystemWatcher_RemoteSystemUpdated; ;
                    remoteSystemWatcher.Complete += RemoteSystemWatcher_Complete;
                    //start watcher
                    try
                    {
                        remoteSystemWatcher.Start();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                } 
            }
        }

        private void RemoteSystemWatcher_Complete(RemoteSystemWatcher watcher)
        {
            DiscoveryCompleted?.Invoke(this, null); 
        }

        public void StopDiscovery()
        {
            lock (instance)
            {
                if (remoteSystemWatcher != null)
                {
                    remoteSystemWatcher.Stop();
                    remoteSystemWatcher = null;
                }
            }
        } 

        private void RemoteSystemWatcher_RemoteSystemUpdated(RemoteSystemWatcher watcher, RemoteSystemUpdatedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("KNOWZY: RemoteSystemUpdated " + args.P0.Id);
            remoteSystems.RemoveAll(system => system.Id == args.P0.Id);
            remoteSystems.Add(args.P0);

            RemoteSystemUpdated?.Invoke(this, args);
        }


        private void RemoteSystemWatcher_RemoteSystemRemoved(RemoteSystemWatcher watcher, RemoteSystemRemovedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("KNOWZY: RemoteSystemRemoved " + args.P0);
            remoteSystems.RemoveAll(system => system.Id == args.P0);
            RemoteSystemRemoved?.Invoke(this, args);
        }

        private void RemoteSystemWatcher_RemoteSystemAdded(RemoteSystemWatcher watcher, RemoteSystemAddedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine($"KNOWZY: RemoteSystemAdded: {args.P0.DisplayName}  + ({args.P0.Id})");
            remoteSystems.Add(args.P0);
            RemoteSystemAdded?.Invoke(this, args);
        }




        internal class MsaWebViewClient : Android.Webkit.WebViewClient
        {
            bool authComplete = false;            
            private readonly RemoteLaunchService callerService;

            public MsaWebViewClient(RemoteLaunchService service)
            {
                this.callerService = service;
            }
            
            public override void OnPageFinished(Android.Webkit.WebView view, string url)
            {
                base.OnPageFinished(view, url);
                System.Diagnostics.Debug.WriteLine("MSA signing returned: " + url);
                if (url.Contains("?code=") && !(callerService.AuthStatus == AuthenticationStatus.Authenticated )) 
                {                    
                    var uri = Android.Net.Uri.Parse(url);
                    string token = uri.GetQueryParameter("code");
                    callerService.authDialog.Dismiss();
                    Platform.SetAuthCode(token);
                    callerService.AuthStatus = AuthenticationStatus.Authenticated;
                }
                else if (url.Contains("error=access_denied"))
                {
                    callerService.authDialog.Dismiss();
                    callerService.AuthStatus = AuthenticationStatus.AuthFailed;
                    //authComplete = false;                    
                    //Intent resultIntent = new Intent();
                    //_parentActivity.SetResult(0, resultIntent);
                    //_parentActivity._authDialog.Dismiss();
                }
            }

        }


    }


}