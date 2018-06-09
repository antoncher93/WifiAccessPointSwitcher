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

namespace WifiSwitcherAp.Servuces.Wifi
{
    
    class AccessPointServiceConnection : Java.Lang.Object, IServiceConnection
    {
       static public bool IsBound { get; private set; }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            IsBound = true;
            PrintOnDebug("=== Conected to Service ===");
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            IsBound = false;
            PrintOnDebug("=== Disconected Service ===");
        }

        public void PrintOnDebug(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }
    }
}