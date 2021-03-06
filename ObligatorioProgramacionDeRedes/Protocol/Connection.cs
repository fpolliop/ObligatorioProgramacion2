﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    class Connection
    {
        private const int DATA_LENGTH = 4;
        private Socket socket;

        public Connection(Socket client)
        {
            this.socket = client;
        }


        public void SendMessage(string message)
        {
            SendMessageLength(message);
            var msgInBytes = Encoding.ASCII.GetBytes(message);
            var dataLength = msgInBytes.Length;
            var sent = 0;
            while (sent < dataLength)

            {
                var pos = socket.Send(msgInBytes, sent, dataLength - sent, SocketFlags.None);

                if (pos == 0) throw new SocketException();

                sent += pos;

            }
        }

        public void SendMessageLength(string message)
        {
            var msgInBytes = Encoding.ASCII.GetBytes(message);
            var dataLength = msgInBytes.Length;
            var dataLengthInBytes = BitConverter.GetBytes(dataLength);
            var sent = 0;
            while (sent < 4)
            //while(sent < dataLength)
            {
                var current = socket.Send(dataLengthInBytes, sent, 4 - sent, SocketFlags.None);

                if (current == 0) throw new SocketException();

                sent += current;
            }
        }

        public string ReceiveMessage()
        {
            var dataLength = ReceiveMessageLength();
            var dataReceived = new byte[dataLength];
            var received = 0;
            while (received < dataLength)
            {
                var pos = socket.Receive(dataReceived, received, dataLength - received, SocketFlags.None);
                if (pos == 0) throw new SocketException();

                received += pos;
            }
            var message = Encoding.ASCII.GetString(dataReceived);
            return message;
        }

        public void Close()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        public int ReceiveMessageLength()
        {
            var dataInBytes = new byte[DATA_LENGTH];
            int messageLength = 0;
            while (messageLength < DATA_LENGTH)
            {
                var pos = socket.Receive(dataInBytes, messageLength, 4 - messageLength, SocketFlags.None);
                if (pos == 0) throw new SocketException();

                messageLength += pos;
            }
            var dataLength = BitConverter.ToInt32(dataInBytes, 0);
            return dataLength;
        }


    }
}
