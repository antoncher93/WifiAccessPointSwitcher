using Android.App;
using Android.Widget;
using Android.OS;
using WifiSwitcherAp.Servuces.Wifi;
using Android.Content;
using System;
using WifiSwitcherAp.Adapters;

namespace WifiSwitcherAp
{
    [Activity(Label = "SC Wifi Switcher", MainLauncher = true)]
    public class MainActivity : Activity
    {
        public const string BroadcastWifiAction = "WifiSwitcherAp.Action.Wifi";
        public const string MyBroadcastAction = "WifiSwitcherAp.Action.Wifi";
        public const string ParamStatus = "status";
        public const string StatusEnabled = "ENABLE";
        public const string StatusDisabled = "DISABLE";

        BroadcastReceiver br;
        TextView textView_status;
        Button button_start;
        ListView lvNetworks;

        Service service;
       

        Intent intent;
        IServiceConnection serviceConnection;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            textView_status = FindViewById<TextView>(Resource.Id.textView1);

            button_start = FindViewById<Button>(Resource.Id.button1);

            lvNetworks = FindViewById<ListView>(Resource.Id.listView_networks);

            service = new AccessPointSwitchService();

            intent = new Intent(this, service.Class);

            AccessPointSwitchService.ScanResultStaticEvent += ScanResultHandler;

            serviceConnection = new AccessPointServiceConnection();

            BindService(intent, serviceConnection, Bind.AdjustWithActivity);
        }

        protected override void OnResume()
        {
            base.OnResume();
            button_start.Text = "OnResume";

            if (AccessPointServiceConnection.IsBound)
            {
                button_start.Click -= Click_Start;
                button_start.Click += Click_Stop;
                button_start.Text = "Stop";
            }
            else
            {
                button_start.Click += Click_Start;
                button_start.Click -= Click_Stop;
                button_start.Text = "Start";
            }
        }

        protected override void OnRestart()
        {
            base.OnRestart();

            button_start.Text = "OnRestart";

        }

        private void Click_Start(object sender, EventArgs e)
        {
            StartService(intent);


        }

        private void ScanResultHandler(object sender, ScanResultEventArgs e)
        {
            RunOnUiThread(() =>
            {
                var adapter = new NetworkAdapter(e.AccessPoints);

                lvNetworks.Adapter = adapter;
            });
        }

        private void SwitchButtonDestination(object sender, StateChangedEventArgs e)
        {
            if(e.IsWorking)
            {
                button_start.Text = "STOP";
                button_start.Click += Click_Start;
            }

            else
            {
                button_start.Text = "START";
            }
        }

        private void Click_Stop(object sender, EventArgs e)
        {
            StopService(new Intent(this, typeof(AccessPointSwitchService)));
        }

        protected override void OnDestroy()
        {
            UnbindService(serviceConnection);

            base.OnDestroy();
        }
        
    }
}

