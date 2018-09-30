using System;
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
        ListRegisteredUsers = 2,
        JoinMatch = 3,
        Exit = 4,
        SelectRole = 5,
        MovePlayer = 6,
        AttackPlayer = 7,
    }

    
}
