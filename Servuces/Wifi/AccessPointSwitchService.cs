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
using System.Threading.Tasks;
using System.Threading;
using Java.Lang;
using Java.Util;

namespace WifiSwitcherAp.Servuces.Wifi
{
    class StateChangedEventArgs : EventArgs
    {
        public bool IsWorking { get; set; }
    }

    public class ScanResultEventArgs : EventArgs
    {
        public IList<NetworkInfo> AccessPoints { get; set; }
    }

    [Service (Label = "SC AP Switcher")]
    class AccessPointSwitchService : Service
    {
        public static event Action<object, ScanResultEventArgs> ScanResultStaticEvent;
        public event EventHandler ScanResultEvent;
        private List<string> SsidList = new List<string>();
        public static bool IsWorking { get; private set;}
        bool connected = true;
        NotificationManager manager;
        Notification notif;
        bool forgetPrevNetwork = false;

        public override IBinder OnBind(Intent intent)
        {
            return new Binder();
        }

        public override void OnCreate()
        {
            base.OnCreate();

            SsidList.Add("Technobee_M1");
            SsidList.Add("ShoperCoin_Tracker_v0.1");
            SsidList.Add("Guest");

            PrintOnDebug("Access Point Switcher was created");

            manager = (NotificationManager)GetSystemService(Context.NotificationService);
        }

        public override void OnRebind(Intent intent)
        {
            base.OnRebind(intent);

            PrintOnDebug("Access Point Switcher on Rebind");
        }

        public void SendNotification()
        {
            notif = new Notification(Resource.Drawable.notif, "Wifi changer has been started.", 0);
            var intent = new Intent(this, typeof(MainActivity));
            notif = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.notif)
                .SetContentTitle("SC Service")
                .SetContentText("Wifi Acess Point Switcher has been started.")
                .SetContentIntent(PendingIntent.GetActivities(this, 1, new[] { intent }, PendingIntentFlags.UpdateCurrent))
                .Build();

            manager.Notify(1, notif);
        }

        public override void OnStart(Intent intent, int startId)
        {
            base.OnStart(intent, startId);

            IsWorking = true;

            PrintOnDebug("Access Point Switcher trying to start");

            //intent.PutExtra(MainActivity.ParamStatus, MainActivity.StatusEnabled);

            System.Timers.Timer timer = new System.Timers.Timer
            {
                Enabled = true,
                Interval = 1000
            };

            timer.Elapsed += async delegate
            {
                await Task.Factory.StartNew(() =>
                {
                    ConnectToMaximalLevelNetwork(CompareSignalStrenght(ScanAccessPoints()));
                });
            };
        }

        

        public override void OnDestroy()
        {
            base.OnDestroy();

            PrintOnDebug("Access Point Switcher was destroyed");

            IsWorking = false;
        }

        void PrintOnDebug(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }

        //Получить список доступных Wifi точек из фильтра
        private List<NetworkInfo> ScanAccessPoints()
        {
            var Networks = new List<NetworkInfo>();

            foreach (var ssid in SsidList)
            {
                Networks.Add(
                    WifiModule.GetNetworks()
                    .FirstOrDefault(n => n.Ssid == ssid));
            }

            // Удаляем нулевые позиции в списке
            for (int i = Networks.Count - 1; i >= 0; i--)
            {
                if (Networks[i] == null) Networks.RemoveAt(i);
            }


            if (ScanResultStaticEvent != null)
                ScanResultStaticEvent(this, new ScanResultEventArgs { AccessPoints = Networks });

            return Networks;
        }

        private NetworkInfo CompareSignalStrenght(List<NetworkInfo> List)
        {
            NetworkInfo maxLevelNetwork = null;

            if (List.Count < 2)
            {
                maxLevelNetwork = List[0];
            }
            else
            {
                int maxLevel = -999;

                foreach (var n in List)
                {
                    if (n == null) List.Remove(n);
                }

                for (int i = 0; i < List.Count; i++)
                {
                    if (List[i].Level > maxLevel)
                    {
                        maxLevelNetwork = List[i];
                        maxLevel = List[i].Level;
                    }
                }
            }
            return maxLevelNetwork;

           


        }

        private void ConnectToMaximalLevelNetwork(NetworkInfo network)
        {
            //Если отобранная сеть не совпадает с текущей
            string bssid = "";

            if (connected)
            {
                if (WifiModule.wifiManager.ConnectionInfo.BSSID == network.Bssid)
                {
                    return;
                }
                else bssid = WifiModule.wifiManager.ConnectionInfo.BSSID;
            }

            string password = GetPassword(network.Ssid);

            connected = false;
            try
            {
                System.Diagnostics.Debug.WriteLine("Trying to connect to " + network.Ssid);
                WifiModule.ConnectToNetwork(network.Ssid, password);
                connected = true;
                System.Diagnostics.Debug.WriteLine("Connected to " + network.Ssid);

                // Забываем предыдущую сеть
                if(forgetPrevNetwork)
                {
                    WifiModule.wifiManager.ConfiguredNetworks
                   .Remove(WifiModule.wifiManager.ConfiguredNetworks
                   .FirstOrDefault(n => n.Bssid == bssid));

                }
                forgetPrevNetwork = true;

                SendNotification();
            }
            catch(System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine("EXEPTION: " + e.Message);
            }
           
          
        }

        private string GetPassword(string ssid)
        {
            switch (ssid)
            {
                case "Technobee_M1": return "Storm501";
                case "ShoperCoin_Tracker_v0.1": return "password";
                case "Guest": return "109Elementver";
                default: return "";
            }
        }

        private event EventHandler MaximalSignalNetworkChanged;

      
    }
}