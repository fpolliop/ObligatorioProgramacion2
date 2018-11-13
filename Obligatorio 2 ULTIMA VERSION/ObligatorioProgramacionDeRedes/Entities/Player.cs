
namespace Entities
{
    public class Player
    {
        public const int MONSTER_POWER_ATTACK = 10;
        public const int SURVIVOR_POWER_ATTACK = 5;
        public const int MONSTER_HEALTH = 100;
        public const int SURVIVOR_HEALTH = 20;

        public string Nickname { get; set; }
        public Role Role { get; set; }
        public int Health { get; set; }
        public int Movements { get; set; }
        public bool IsDead { get; set; }
        public int Points { get; set; }
        public bool IsWinner { get; set; }


        public Player(string nickname, Role role)
        {
            Nickname = nickname;
            Role = role;
            if (role.Equals(Role.Monster))
            {
                Health = MONSTER_HEALTH;
            }
            else
            {
                Health = SURVIVOR_HEALTH;
            }
            IsDead = false;
            IsWinner = false;
            Movements = 0;
            Points = 0;
        }

        public int GetPowerAttack()
        {
            if (Role.Equals(Role.Monster))
            {
                return MONSTER_POWER_ATTACK;
            }
            else
            {
                return SURVIVOR_POWER_ATTACK;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            string objNickname = ((Player)obj).Nickname;
            if (objNickname != null && Nickname != null)
            {
                return ((Player)obj).Nickname.ToUpper().Equals(Nickname.ToUpper());
            }
            return false;
        }
    }
}