using DataManager;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientController
    {
        ClientProtocol clientProtocol;
        public bool Connect()
        {
            clientProtocol = new ClientProtocol();
            return clientProtocol.Connect();
        }

        public void CreatePlayer(string nickname)
        {
            string message = MessageBuilder.EncodeMessage(MessageBuilder.RequestMessage, ActionOptions.NewPlayer, nickname);
            
            clientProtocol.SendRequestMessage(message);
        }
    }
}

