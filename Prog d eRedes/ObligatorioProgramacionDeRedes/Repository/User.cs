using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class User
    {
        public string Nickname { get; set; }
        public bool IsConnected { get; set; }
        public Socket SocketNotify { get; set; }

        public User(string nickname, Socket socketToNotify)
        {
            this.Nickname = nickname;
            this.IsConnected = true;
            this.SocketNotify = socketToNotify;
        }
        public override bool Equals(Object obj)
        {
            return ((User)obj).Nickname.ToLower().Equals(this.Nickname.ToLower());
        }
    }
}
