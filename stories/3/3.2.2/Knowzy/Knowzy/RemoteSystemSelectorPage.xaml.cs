using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Knowzy
{
	//  [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RemoteSystemSelectorPage : ContentPage
	{
         
        public RemoteSystemSelectorPage ( Nose selectedNose )
		{
			InitializeComponent ();

            remoteSystemView.SelectedNose  = selectedNose;
        }
	}
}