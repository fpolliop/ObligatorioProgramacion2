using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Statistic
    {
        public List<GameStatistic> GameStatistics { get; set; }
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
