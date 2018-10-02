using System;
using System.Collections.Generic;
using System.Drawing;
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
        public Image Avatar { get; private set; }

        public User(string nickname, Image userAvatar, Socket socketToNotify)
        {
            this.Nickname = nickname;
            this.IsConnected = true;
            this.SocketNotify = socketToNotify;
            this.Avatar = userAvatar;
        }
        public override bool Equals(Object obj)
        {
            return ((User)obj).Nickname.ToLower().Equals(this.Nickname.ToLower());
        }
    }
}
