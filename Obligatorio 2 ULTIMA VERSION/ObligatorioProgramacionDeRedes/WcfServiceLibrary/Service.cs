using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Entities;
using Server;

namespace WcfServiceLibrary
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código y en el archivo de configuración a la vez.
    public class Service : IService
    {
        private RemotingShared controller;
        private void EstablishConnection()
        {
            string remotingPath = "tcp://localhost:3333/MyRemotingName";
            TcpChannel lTcpChannel = new TcpChannel();

            if (!ChannelServices.RegisteredChannels.Any(lChannel => lChannel.ChannelName == lTcpChannel.ChannelName))
            {
                ChannelServices.RegisterChannel(lTcpChannel, true);
            }

            controller = (RemotingShared)Activator.GetObject(typeof(RemotingShared), remotingPath);
        }

        public bool AddUser(string name)
        {
            EstablishConnection();
            bool response = controller.AddUser(name);
            return response;
        }

        public bool DeleteUser(string name)
        {
            EstablishConnection();
            bool response = controller.DeleteUser(name);
            return response;
        }

        public List<Ranking> GetRanking()
        {
            throw new NotImplementedException();
        }

        public List<Statistic> GetStatistics()
        {
            throw new NotImplementedException();
        }

        public List<User> GetUsers()
        {
            EstablishConnection();
            return controller.GetUsers();
        }

        public bool ModifyUser(string name, string newName)
        {
            EstablishConnection();
            return controller.ModifyUser(name, newName);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }
    }
}
