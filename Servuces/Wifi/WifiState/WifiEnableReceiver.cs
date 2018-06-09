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
using Android.Net;

namespace WifiSwitcherAp.Servuces.Wifi.WifiState
{
    //[BroadcastReceiver]
    //[IntentFilter(new[] { ConnectivityManager.ConnectivityAction,
    //    Android.Net.Wifi.WifiManager.ActionPickWifiNetwork })]
    class WifiEnableReceiver : BroadcastReceiver
    {
        NotificationManager notifManager;
        public override void OnReceive(Context context, Intent intent)
        {
            notifManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            var networkInfo = (Android.Net.NetworkInfo)intent.GetParcelableExtra(ConnectivityManager.ExtraExtraInfo);

            if (networkInfo.IsConnected)
            {
                if(networkInfo.Type == ConnectivityType.Wifi)
                {
                    Notification notify = new Notification(Resource.Drawable.notif, "Wifi Access Point changed", 0);
                    notify = new Notification.Builder(context)
                        .SetSmallIcon(Resource.Drawable.notif)
                        .SetContentTitle("SC")
                        .SetContentText("Access Point has been changed.")
                        .Build();

                    notifManager.Notify(1, notify);
                }
            }
        }
    }
}