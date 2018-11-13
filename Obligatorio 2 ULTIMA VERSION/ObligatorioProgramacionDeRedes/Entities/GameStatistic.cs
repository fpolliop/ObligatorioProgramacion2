using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [Serializable]
    [DataContract]
    public class GameStatistic
    {
        [DataMember]
        public string Nickname { get; set; }
        [DataMember]
        public string Role { get; set; }
        [DataMember]
        public string Result { get; set; }
    }
}
