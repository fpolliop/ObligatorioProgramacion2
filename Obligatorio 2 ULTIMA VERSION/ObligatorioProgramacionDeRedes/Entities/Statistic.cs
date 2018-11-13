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
    public class Statistic
    {
        [DataMember]
        public List<GameStatistic> GameStatistics { get; set; }
        [DataMember]
        public string Date { get; set;}

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("- Partida del: " + Date);
            foreach (GameStatistic gameStatistic in GameStatistics)
            {
                builder.Append("\n Jugador: " + gameStatistic.Nickname + "\n  -Rol: " + gameStatistic.Role + "\n  -Ha Ganado: " + gameStatistic.Result + ". \n");
            }
            return builder.ToString();
        }
    }
}
