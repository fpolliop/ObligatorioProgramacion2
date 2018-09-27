using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Player
    {
        private readonly int MonsterPowerAttack = 10;
        private readonly int SurvivorPowerAttack = 5;
        private readonly int MonsterHealth = 100;
        private readonly int SurvivorHealth = 20;

        public string Nickname { get; set; }
        public string Avatar { get; set; }
        public Role Role { get; set; }
        public int Health { get; set; }
        public int Movements { get; set; }

        public Player(string nickname, Role role)
        {
            this.Nickname = nickname;
            this.Role = role;
            if (role.Equals(Role.Monster))
            {
                this.Health = MonsterHealth;
            }
            else
            {
                this.Health = SurvivorHealth;
            }
        }

        public int GetPowerAttack()
        {
            if (Role.Equals(Role.Monster))
            {
                return MonsterPowerAttack;
            }
            else
            {
                return 5;
            }
        }
    }
}

public enum Role
{
    Monster = 0,
    Survivor = 1
}
