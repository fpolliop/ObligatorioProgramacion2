using Entities;
using System;
using System.Text;

namespace Protocol
{
    public class Frame
    {
        private const char FRAME_SEPARATOR = '|';
        private const char DATA_SEPARATOR = '#';

        public Command Cmd { get; set; }
        public string Data { get; set; }
        public int DataLength { get; set; }

        private const int CMD_POSITION = 0;
        private const int DATA_LENGTH_POSITION = 1;
        private const int DATA_POSITION = 2;

        private const int NICKNAME_POSITION = 0;
        private const int ROLE_POSITION = 1;

        public enum Command
        {
            ConnectToServer = 0,
            ListConnectedUsers = 1,
            ListRegisteredUsers = 2,
            JoinMatch = 3,
            ViewLog = 4,
            Exit = 5,
            MovePlayer = 6,
            AttackPlayer = 7,
        }

        public Frame()
        {

        }

        public Frame(Command cmd, string data)
        {
            Cmd = cmd;
            Data = data;
            DataLength = data.Length;
        }

        public Frame(Command cmd, string nickName, int role)
        {
            Cmd = cmd;
            StringBuilder builder = new StringBuilder();
            builder.Append(nickName);
            builder.Append(DATA_SEPARATOR);
            builder.Append(role);
            Data = builder.ToString();
            DataLength = Data.Length;
        }

        public Frame(byte[] dataReceived)
        {
            string dataString = Encoding.ASCII.GetString(dataReceived);
            string[] dataStringArray = dataString.Split(FRAME_SEPARATOR);
            Enum.TryParse(dataStringArray[CMD_POSITION], out Command receivedCmd);
            this.Cmd = receivedCmd;
            this.DataLength = int.Parse(dataStringArray[DATA_LENGTH_POSITION]);
            this.Data = dataStringArray[DATA_POSITION];

        }

        public string GetUserNickname()
        {
            string[] dataArray = this.Data.Split(DATA_SEPARATOR);
            return dataArray[NICKNAME_POSITION];
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(this.Cmd);
            builder.Append(FRAME_SEPARATOR);
            builder.Append(this.DataLength);
            builder.Append(FRAME_SEPARATOR);
            builder.Append(this.Data);

            return builder.ToString();
        }

        public Role GetUserRole()
        {
            Enum.TryParse(this.Data.Split(DATA_SEPARATOR)[ROLE_POSITION], out Role playerRole);
            return playerRole;

        }

        public PlayerGameAction GetPlayerGameAction()
        {
            Enum.TryParse(this.Data.Split(DATA_SEPARATOR)[ROLE_POSITION], out PlayerGameAction playerGameAction);
            return playerGameAction;
        }
    }
}
