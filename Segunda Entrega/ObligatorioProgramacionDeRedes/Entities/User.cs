using System;
using System.Drawing;
using System.Net.Sockets;

namespace Entities
{
    public class User
    {
        public string Nickname { get; set; }
        public bool IsConnected { get; set; }
        public Socket SocketNotify { get; set; }
        public Image Avatar { get; set; }

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
