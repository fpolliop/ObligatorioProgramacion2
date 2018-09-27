using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager
{
    public static class MessageDecoder
    {
        public static  string RequestMessage = "REQ";
        public static  string ResponseMessage = "RES";
        const char separator = '|';

        public static string EncodeMessage(string messageType, ActionType actionOption, string specification)
        {
            //REQ|01|1015|PEPITO
            //REQ O RES | ACCION | SUMA | NICKNAME O ALGO
            int sum = messageType.Length + (int)actionOption; //????? preguntar


            StringBuilder builder = new StringBuilder();
            builder.Append(messageType);
            builder.Append(separator);
            builder.Append(actionOption);
            builder.Append(separator);
            builder.Append(sum);
            builder.Append(separator);
            builder.Append(specification);

            return builder.ToString();
        }
        public static string[] DecodeMessage(string message)
        {
            //REQ|01|1015|PEPITO
            //REQ O RES | ACCION | SUMA | NICKNAME O ALGO

            return message.Split(separator);
           

        }
    }
}
