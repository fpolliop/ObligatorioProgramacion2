using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        private const int clientPort = 1100;
        private const int serverPort = 1099;

        public Form1()
        {
            InitializeComponent();

            Connect();
        }

        private void Connect()
        {

            var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), clientPort));
            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverPort));

            // while (true)

            {
                var msg = "MENSAJE DEL CLIENTE";
                var msgInBytes = Encoding.ASCII.GetBytes(msg);
                var dataLength = msgInBytes.Length;
                var dataLengthInBytes = BitConverter.GetBytes(dataLength);
                var sent = 0;
                while (sent < 4)

                {
                    var current = client.Send(dataLengthInBytes, sent, 4 - sent, SocketFlags.None);

                    if (current == 0) throw new SocketException();

                    sent += current;
                }

                sent = 0;
                while (sent < dataLength)

                {
                    var pos = client.Send(msgInBytes, sent, dataLength - sent, SocketFlags.None);

                    if (pos == 0) throw new SocketException();

                    sent += pos;

                }

            }
        }
    }
}

