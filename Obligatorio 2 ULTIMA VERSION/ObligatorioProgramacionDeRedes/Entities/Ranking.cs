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
    public class Ranking
    {
        [DataMember]
        public string Nickname { get; set; }
        [DataMember]
        public int Points { get; set; }
        [DataMember]
        public string Date { get; set; }
        [DataMember]
        public string Role { get; set; }

        public override string ToString()
        {
            return $"Jugador: {Nickname} como {Role.ToString()} - fecha: {Date}.  - Puntaje: {Points}";
        }
        public override bool Equals(object obj)
        {
            var ob = obj as Ranking;

            if (ob == null)
            {
                return false;
            }

            return this.Nickname.Equals(ob.Nickname) && this.Points.Equals(ob.Points) && this.Date.Equals(ob.Date) && this.Role.Equals(ob.Role);
        }
    }
}
