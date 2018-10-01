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
        private const char frameSeparator = '|';
        private const char dataSeparator = '#';
      
        public ActionType Action { get; set; }
        public string Data { get; set; }
        public int DataLength { get; set; }
        private const int ACTION_POSITION = 0;
        private const int DATA_LENGTH_POSITION = 1;
        private const int DATA_POSITION = 2;

        private const int NICKNAME_POSITION = 0;
        private const int ROLE_POSITION = 1;


        public Frame()
        {

        }

        public Frame(ActionType action, string data)
        {
            Action = action;
            Data = data;
            DataLength = data.Length;
        }

        public Frame(ActionType action, string nickName, int role)
        {
            Action = action;
            StringBuilder builder = new StringBuilder();
            builder.Append(nickName);
            builder.Append(dataSeparator);
            builder.Append(role);
            Data = builder.ToString();
            DataLength = Data.Length;
        }

        public Frame(byte[] dataReceived)
        {
            string dataString = Encoding.ASCII.GetString(dataReceived);
            string[] dataStringArray = dataString.Split(frameSeparator);
            Enum.TryParse(dataStringArray[ACTION_POSITION], out ActionType receivedAction);
            this.Action = receivedAction;
            this.DataLength = int.Parse(dataStringArray[DATA_LENGTH_POSITION]);
            this.Data = dataStringArray[DATA_POSITION];

        }

        public string GetUserNickname()
        {
            string[] dataArray = this.Data.Split(dataSeparator);
            return dataArray[NICKNAME_POSITION];
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
            builder.Append(frameSeparator);
            builder.Append(this.DataLength);
            builder.Append(frameSeparator);
            builder.Append(this.Data);

            return builder.ToString();
        }

        public Role GetUserRole()
        {
            Enum.TryParse(this.Data.Split(dataSeparator)[ROLE_POSITION], out Role playerRole);
            return playerRole;

        }

        public PlayerGameAction GetPlayerGameAction()
        {
            Enum.TryParse(this.Data.Split(dataSeparator)[ROLE_POSITION], out PlayerGameAction playerGameAction);
            return playerGameAction;
        }  
    }
}
