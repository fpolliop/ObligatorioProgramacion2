using DataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol
{
    public class Frame
    {
        private const char separator = '|';
      
        public ActionType Action { get; set; }
        public string Data { get; set; }
        public int DataLength { get; set; }

        public Frame()
        {

        }

        public Frame(ActionType action, string data)
        {
            Action = action;
            Data = data;
            DataLength = data.Length;
        }

        public Frame(byte[] dataReceived)
        {
            string dataString = Encoding.ASCII.GetString(dataReceived);
            string[] dataStringArray = dataString.Split(separator);
            Enum.TryParse(dataStringArray[0], out ActionType receivedAction);
            this.Action = receivedAction;
            this.DataLength = int.Parse(dataStringArray[1]);
            this.Data = dataStringArray[2];

        }

        public override string ToString()
        {
            //REQ|01|1015|PEPITO
            //REQ O RES | ACCION | SUMA | NICKNAME O ALGO

            //version neuva
            //ACCION | LARGO | DATA(BYTES)
            //01|4|ABCD
            StringBuilder builder = new StringBuilder();
            builder.Append(this.Action);
            builder.Append(separator);
            builder.Append(this.DataLength);
            builder.Append(separator);
            builder.Append(this.Data);

            return builder.ToString();
        }
    }
}
