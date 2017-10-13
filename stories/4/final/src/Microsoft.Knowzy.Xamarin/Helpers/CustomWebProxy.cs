using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Knowzy.Xamarin.Helpers
{
    public class CustomWebProxy : System.Net.IWebProxy
    {
        public System.Net.ICredentials Credentials
        {
            get;
            set;
        }

        private readonly Uri _proxyUri;

        public CustomWebProxy(Uri proxyUri)
        {
            _proxyUri = proxyUri;
        }

        public Uri GetProxy(Uri destination)
        {
            return _proxyUri;
        }

        public bool IsBypassed(Uri host)
        {
            return false;
        }
    }
}
