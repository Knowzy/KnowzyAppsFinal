 
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

 

namespace Knowzy
{
  
    public enum AuthenticationStatus
    {
        NotStarted , 
        NotAvailable, 
        InProgress, 
        AuthFailed, 
        Authenticated 
    }; 

    public interface IRemoteLaunchService
    {
        Task<bool> InitializeAsync(object context);
        AuthenticationStatus AuthStatus { get; }
        event EventHandler AuthStatusChanged ;
        event EventHandler DiscoveryCompleted; 
        void StartDiscovery();
        void StopDiscovery();
        bool HasRemoteSystemsAvailable { get;  }


#if __ANDROID__

        List<Microsoft.ConnectedDevices.RemoteSystem> RemoteSystems { get; }
        Task<Microsoft.ConnectedDevices.RemoteLaunchUriStatus> LaunchAsync(string id,  string noseId );

        event EventHandler<Microsoft.ConnectedDevices.RemoteSystemUpdatedEventArgs> RemoteSystemUpdated;
        event EventHandler<Microsoft.ConnectedDevices.RemoteSystemAddedEventArgs> RemoteSystemAdded;
        event EventHandler<Microsoft.ConnectedDevices.RemoteSystemRemovedEventArgs> RemoteSystemRemoved;        
#endif 
    }
}
