using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Client.Services
{
    public class SocketListener
    {
        private readonly TcpClient _client;
        private Thread _listenThread;

        public event Action<string> MessageReceived;

        public SocketListener(TcpClient client)
        {
            _client = client;
        }

        public void Start()
        {
            _listenThread = new Thread(Listen);
            _listenThread.IsBackground = true;
            _listenThread.Start();
        }

        private void Listen()
        {
            try
            {
                StreamReader reader =
                    new StreamReader(_client.GetStream());

                while (true)
                {
                    string msg = reader.ReadLine();

                    if (!string.IsNullOrEmpty(msg))
                    {
                        MessageReceived?.Invoke(msg);
                    }
                }
            }
            catch
            {
            }
        }
    }
}