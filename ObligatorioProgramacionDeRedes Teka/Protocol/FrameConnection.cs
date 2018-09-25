using DataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    public class FrameConnection
    {
        /*
        static int BUFFER_LENGTH = 100;

        public static void SendData(Socket socket, byte[] data)
        {
            int dataLength = data.Length;
            int sent = 0;

            while (sent < dataLength)
            {
                sent += socket.Send(data, sent, (dataLength - sent), SocketFlags.None);
            }
        }

        public static void Send(Socket socket, Frame frame)
        {
            int dataLength = frame.DataLength;
            int lengthCMDandLengthFile = 6;
            byte[] informationData = new byte[lengthCMDandLengthFile];
            byte[] cmdBytes = BitConverter.GetBytes((int)frame.Action);
            byte[] lengthBytes = BitConverter.GetBytes(frame.DataLength);
            int cmdStart = 0;
            int cmdEnd = 2;
            int lengthFileStart = 2;
            int lengthFileEnd = 4;
            Array.Copy(cmdBytes, 0, informationData, cmdStart, cmdEnd);
            Array.Copy(lengthBytes, 0, informationData, lengthFileStart, lengthFileEnd);
            SendData(socket, informationData);
            byte[] data = frame.Data;
            SendSegmented(socket, data);
        }

        public static void SendSegmented(Socket socket, byte[] data)
        {
            byte[] buffer = new Byte[BUFFER_LENGTH];
            int countsFullBuffers = (data.Length) / BUFFER_LENGTH;
            int rest = (data.Length + 1) % BUFFER_LENGTH;

            for (int i = 0; i < countsFullBuffers; i++)
            {
                int start = i * BUFFER_LENGTH;
                Array.ConstrainedCopy(data, start, buffer, 0, buffer.Length - 1);
                SendData(socket, buffer);
            }
            if (rest > 0)
            {
                int start = BUFFER_LENGTH * countsFullBuffers;
                byte[] dataRest = new byte[rest];
                Array.ConstrainedCopy(data, start, dataRest, 0, rest - 1);
                SendData(socket, dataRest);
            }
        }

        public static void ReceiveData(Socket socket, byte[] data)
        {
            int dataLength = data.Length;
            int received = 0;
            while (received < dataLength)
            {
                received += socket.Receive(data, received, (dataLength - received), SocketFlags.None);
            }
        }

        public static Frame Receive(Socket socket)
        {
            Frame frame = new Frame();
            try
            {
                byte[] informationData = new byte[6];
                ReceiveData(socket, informationData);

                int lengthToCopy = 2;
                int startIndex = 0;
                byte[] commandByte = SubArray(informationData, startIndex, lengthToCopy);
                lengthToCopy = 4;
                startIndex = 2;
                byte[] lengthDataByte = SubArray(informationData, startIndex, lengthToCopy);
                int command = BitConverter.ToInt16(commandByte, 0);
                int lengthData = BitConverter.ToInt32(lengthDataByte, 0);
                frame.Action = (ActionType)command;
                byte[] data = new Byte[lengthData];

                data = ReceivedSegmented(socket, data);
                frame.Data = data;
                frame.DataLength = lengthData;
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Error en la conexión");
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            return frame;
        }

        public static byte[] ReceivedSegmented(Socket socket, byte[] data)
        {
            byte[] buffer = new Byte[BUFFER_LENGTH];
            int countsFullBuffers = (data.Length) / BUFFER_LENGTH;
            int rest = (data.Length + 1) % BUFFER_LENGTH;

            for (int i = 0; i < countsFullBuffers; i++)
            {
                int start = i * BUFFER_LENGTH;
                ReceiveData(socket, buffer);
                Array.ConstrainedCopy(buffer, 0, data, start, buffer.Length - 1);
            }
            if (rest > 0)
            {
                int start = BUFFER_LENGTH * countsFullBuffers;
                byte[] dataRest = new byte[rest];
                ReceiveData(socket, dataRest);
                Array.ConstrainedCopy(dataRest, 0, data, start, dataRest.Length - 1);
            }

            return data;
        }

        public static Byte[] SubArray(Byte[] data, int index, int length)
        {
            Byte[] result = new Byte[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    */
    }
}
