﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager
{
    public enum ActionType
    {
        ConnectToServer = 0,
        ListConnectedUsers = 1,
        JoinGame = 2,
        JoinMatch = 3,
        SelectRole = 4,
        MovePlayer = 5,
        AttackPlayer = 6,
        Exit = 7
    }

    
}
