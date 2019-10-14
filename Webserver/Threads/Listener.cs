using Configurator;
using Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Webserver.Threads {
	class Listener {
		public static void Run(Logger Log, BlockingCollection<HttpListenerContext> Queue) {
			Log.Info("Starting ListenerThread");

			//Get addresses the server should listen to.
			List<String> Addresses = Config.GetValue("ConnectionSettings.ServerAddresses").ToObject<List<string>>();
			if ((bool)Config.GetValue("ConnectionSettings.AutoDetectAddress")) {
				string address;
				using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0)) {
					socket.Connect("8.8.8.8", 65530);
					IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
					address = endPoint.Address.ToString();
					Addresses.Add(address);
				}
				Log.Info("Detected IPv4 address to be " + address);
			}

			//Create HTTPListener
			using HttpListener Listener = new HttpListener();
			foreach (string Address in Addresses) {
				//There has to be a better way!
				string addr;
				//if (!Address.Contains("https://")) {
				//	addr = "https://" + Address;
				//} else 
				if (!Address.Contains("http://")) {
					addr = "http://" + Address;
				} else {
					addr = Address;
				}

				if(addr[^1] != '/') {
					addr += '/';
				}
				Listener.Prefixes.Add(addr);
			}

			Listener.Start();
			Log.Info("Now listening!");
			while (true) {
				//TODO Fix ObjectDisposedException: 'The collection has been disposed. Object name: 'BlockingCollection'.'
				Queue.Add(Listener.GetContext());
			}
		}
	}
}
