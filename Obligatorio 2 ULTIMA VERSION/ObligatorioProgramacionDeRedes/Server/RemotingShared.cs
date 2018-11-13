﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Server
{
    [Serializable]
    public abstract class RemotingShared : MarshalByRefObject
    {
        public abstract bool AddUser(string name);

        public abstract bool DeleteUser(string name);
        public abstract List<User> GetUsers();
        public abstract bool ModifyUser(string name, string newName);
        public abstract List<Ranking> GetRankings();
    }
}
