﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class User
    {
        public string Nickname { get; set; }
        public bool IsConnected { get; set; }

        public User(string nickname)
        {
            this.Nickname = nickname;
            this.IsConnected = true;
        }
        public override bool Equals(Object obj)
        {
            return ((User)obj).Nickname.ToLower().Equals(this.Nickname.ToLower());
        }
    }
}
