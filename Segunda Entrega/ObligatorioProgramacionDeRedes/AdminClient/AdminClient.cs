using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace AdminClient
{
    class AdminClient
    {
        static void Main(string[] args)
        {
            CreateRemotingInfrastructure();

        }

        private static void CreateRemotingInfrastructure()
        {
            IDictionary props = new Hashtable();
            props["port"] = 0;
            BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
            new TcpChannel(props, null, serverProvider);
            serverProvider.TypeFilterLevel = TypeFilterLevel.Full;
            TcpChannel chan = new TcpChannel(props, null, serverProvider);
            ChannelServices.RegisterChannel(chan, false);
        }
    }
}
