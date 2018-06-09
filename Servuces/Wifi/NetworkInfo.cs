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


namespace WifiSwitcherAp.Servuces.Wifi
{
    public class NetworkInfo
    {
        ScanResult scanResult;

        public NetworkInfo(ScanResult result)
        {
            scanResult = result;
        }

        public string Ssid
        {
            get { return scanResult.Ssid; }
        }

        public int Level
        {
            get { return scanResult.Level; }
        }

        public int Frequency
        {
            get { return scanResult.Frequency; }
        }

        public override string ToString()
        {
            return Ssid;
        }

        public string Bssid
        {
            get { return scanResult.Bssid; }
        }
    }
}