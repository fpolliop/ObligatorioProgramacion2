using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Protocol
{
    public class FrameConnection
    {
        public static int BUFFER_LENGTH = 100;
        public static int DATA_LENGTH = 4;

        public static void Send(Socket socket, Frame frame)
        {
            string message = frame.ToString();
            SendMessageLength(socket, message);
            byte[] msgInBytes = Encoding.ASCII.GetBytes(message);
            int dataLength = msgInBytes.Length;
            int sent = 0;
            while (sent < dataLength)
            {
                int pos = socket.Send(msgInBytes, sent, dataLength - sent, SocketFlags.None);

                if (pos == 0)  throw new SocketException();

                sent += pos;
            }
        }

        public static void SendMessageLength(Socket socket, string message)
        {
            byte[] msgInBytes = Encoding.ASCII.GetBytes(message);
            int dataLength = msgInBytes.Length;
            byte[] dataLengthInBytes = BitConverter.GetBytes(dataLength);
            int sent = 0;
            while (sent < DATA_LENGTH)
            {
                int current = socket.Send(dataLengthInBytes, sent, DATA_LENGTH - sent, SocketFlags.None);

                if (current == 0) throw new SocketException();

                sent += current;
            }
        }

        public static void SendAvatar(Socket socket, Image userAvatar)
        {
            MemoryStream memoryStream = new MemoryStream();
            userAvatar.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

            byte[] imageBytes = memoryStream.ToArray();
            userAvatar.Dispose();
            memoryStream.Close();

            SendBytes(socket, imageBytes);
        }

        private static void SendBytes(Socket socket, byte[] data)
        {
            int total = 0;
            int size = data.Length;
            int dataleft = size;
            int sent;

            byte[] datasize = new byte[4];
            datasize = BitConverter.GetBytes(size);
            sent = socket.Send(datasize);

            while (total < size)
            {
                sent = socket.Send(data, total, dataleft, SocketFlags.None);
                total += sent;
                dataleft -= sent;
            }
        }

        public static Image ReceiveAvatar(Socket s)
        {
            int total = 0;
            int receive;
            byte[] datasize = new byte[4];

            receive = s.Receive(datasize, 0, 4, 0);
            int size = BitConverter.ToInt32(datasize, 0);
            int dataleft = size;
            byte[] data = new byte[size];


            while (total < size)
            {
                receive = s.Receive(data, total, dataleft, 0);
                if (receive == 0)
                {
                    break;
                }
                total += receive;
                dataleft -= receive;
            }
            return BytesToImage(data);
        }

        private static Image BytesToImage(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            Image userAvatar = null;
            try
            {
                userAvatar = Image.FromStream(ms);

            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

            return userAvatar;
        }

        public static Frame Receive(Socket socket)
        {
            int dataLength = ReceiveMessageLength(socket);
            byte[] dataReceived = new byte[dataLength];
            int received = 0;
            while (received < dataLength)
            {
                received += socket.Receive(dataReceived, received, (dataLength - received), SocketFlags.None);
            }

            Frame frame = new Frame(dataReceived);
            return frame;
        }

        public static int ReceiveMessageLength(Socket socket)
        {
            byte[] dataInBytes = new byte[DATA_LENGTH];
            int messageLength = 0;
            while (messageLength < DATA_LENGTH)
            {
                messageLength += socket.Receive(dataInBytes, messageLength, DATA_LENGTH - messageLength, SocketFlags.None);
            }
            int dataLength = BitConverter.ToInt32(dataInBytes, 0);
            return dataLength;
        }
    }
}
