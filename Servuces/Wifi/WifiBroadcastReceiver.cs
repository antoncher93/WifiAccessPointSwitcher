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
    [BroadcastReceiver]
    public class WifiBroadcastReceiver : BroadcastReceiver
    {
        static public event Action<object, WifiBREventArgs> StatusEnabledEvent;


        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "New Message: " 
                + intent.GetStringExtra(MainActivity.ParamStatus),
                ToastLength.Long)
                .Show();
        }

    }

    public class WifiBREventArgs : EventArgs
    {
        public string Message { get; set; }
    }
}