using Configurator;
using Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Webserver.Threads {
	class Listener {
		public static void Run(Logger Log, BlockingCollection<HttpListenerContext> Queue) {
			Log.Info("Starting ListenerThread");

			//Get addresses the server should listen to.
			List<string> Addresses = Config.GetValue("ConnectionSettings.ServerAddresses").ToObject<List<string>>();
			if ((bool)Config.GetValue("ConnectionSettings.AutoDetectAddress")) {
				string address;
				using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0)) {
					socket.Connect("8.8.8.8", 65530);
					IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
					address = endPoint.Address.ToString();
					Addresses.Add(address + ":80");
					Program.CORSAddresses.Add("http://"+address+":80");
				}
				Log.Info("Detected IPv4 address to be " + address);
			}

			//Create HTTPListener
			using HttpListener Listener = new HttpListener();
			Utils.ParseAddresses(Addresses).ForEach(Listener.Prefixes.Add);

			Listener.Start();
			Log.Info("Now listening!");
			while (true) {
				Queue.Add(Listener.GetContext());
			}
		}
	}
}
