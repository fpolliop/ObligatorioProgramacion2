﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public abstract class RemotingShared : MarshalByRefObject
    {
        public abstract bool AddUser(string name);
    }
}
