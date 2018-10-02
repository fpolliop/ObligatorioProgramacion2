using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UsersRepository
    {
        private static List<User> users;

        public UsersRepository()
        {
            users = new List<User>();
        }

        public void AddUser(User user)
        {
            users.Add(user);
        }

        public List<User> GetUsers()
        {
            return users;
        }

        public void RemoveUser(User user)
        {
            users.Remove(user);
        }

        public int GetCountUsers()
        {
            return users.Count;
        }

        public User GetUserByName(string nickname)
        {
            User userAux = new User(nickname, null);
            foreach (var user in users)
            {
                if (user.Equals(userAux))
                {
                    return user;
                }
            }
            return null;
        }
    }
}
