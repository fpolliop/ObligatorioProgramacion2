
using DataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controllers
{
    public class ServerController
    {
        const int MessageType = 0; //Mayuscula??
        const int ActionType = 1;
        public void HandleClientRequest(string[] clientRequest)
        {
            int action = int.Parse(clientRequest[ActionType]);

            switch (action)
            {
                case ((int)ActionOptions.NewPlayer):
                    //NewPlayer
                    break;
                case ((int)ActionOptions.JoinGame):
                    //JoinGame
                    break;
                case ((int)ActionOptions.JoinMatch):
                    //JoinMatch
                    break;
                case ((int)ActionOptions.SelectRole):
                    //SelectRole
                    break;
                case ((int)ActionOptions.MovePlayer):
                    //MovePlayer
                    break;
                case ((int)ActionOptions.AttackPlayer):
                    //AttackPlayer
                    break;

            }
        }
    }
}
