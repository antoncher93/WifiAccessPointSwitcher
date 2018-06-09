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
using Android.Net.Wifi;
using System.Threading;

namespace WifiSwitcherAp.Servuces.Wifi
{
    public static class WifiModule
    {
        static WifiManager _wifiManager = (WifiManager)Application.Context.GetSystemService(Context.WifiService);
        static WifiConfiguration wifiConfig;

        static public WifiManager wifiManager => _wifiManager;

        public static void ConnectToNetwork(string ssid, string password)
        {
            var formatSSid = $"\"{ssid}\"";
            var formatPassword = $"\"{password}\"";

            wifiConfig = new WifiConfiguration
            {
                Ssid = formatSSid,
                PreSharedKey = formatPassword
            };

            System.Diagnostics.Debug.WriteLine("Current state: " + _wifiManager.ConnectionInfo.SSID);

            var network = _wifiManager.ConfiguredNetworks.FirstOrDefault(n => n.Ssid == formatSSid);

            if (network == null)
            {
                var networkDesc = _wifiManager.AddNetwork(wifiConfig);
                network = _wifiManager.ConfiguredNetworks.FirstOrDefault(n => n.Ssid == formatSSid);
            }

            if (network == null)
            {
                throw new Exception("Could not to connect:-(");
            }

            else
            {
                if(_wifiManager.Disconnect())
                {

                }

                if (!_wifiManager.EnableNetwork(network.NetworkId, true))
                {
                    throw new Exception("cannot to enable network.");
                }
                else
                {
                    while (_wifiManager.ConnectionInfo.SSID != formatSSid)
                    {
                        System.Diagnostics.Debug.WriteLine(_wifiManager.ConnectionInfo.SSID + " Waiting for Enabling");
                        Thread.Sleep(100);
                    }
                }
            }
           
        }

        private static IList<ScanResult> ScanNetworks() => _wifiManager.ScanResults;

        public static IList< NetworkInfo> GetNetworks()
        {
            _wifiManager.StartScan();

            List<NetworkInfo> NetworkList = _wifiManager.ScanResults.
                Select(s =>
                {
                    return new NetworkInfo(s);
                }).ToList();

            return NetworkList;
        }

    }

   
}