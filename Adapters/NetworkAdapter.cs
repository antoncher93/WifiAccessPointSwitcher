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
using WifiSwitcherAp.Servuces.Wifi;

namespace WifiSwitcherAp.Adapters
{
    public class NetworkAdapter : BaseAdapter<NetworkInfo>
    {
        protected IList<NetworkInfo> _networks;

        public NetworkAdapter(IList<NetworkInfo> networks)
        {
            _networks = networks;
        }

        public override int Count => _networks.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override NetworkInfo this[int position] => _networks[position];

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = LayoutInflater.From(parent.Context)
                .Inflate(Resource.Layout.Networks, parent, false);

            var name = view.FindViewById<TextView>(Resource.Id.textView_adress);
            var friq = view.FindViewById<TextView>(Resource.Id.textView_rssi);

            name.Text = _networks[position].Ssid;
            friq.Text = _networks[position].Level.ToString();

            return view;
        }
    }
}