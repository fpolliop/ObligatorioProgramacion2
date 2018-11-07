using System.Collections.Generic;

namespace Entities
{
    public class UsersRepository
    {
        private List<User> users;

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

        public User GetUserByNickname(string nickname)
        {
            foreach (User user in users)
            {
                if (user.Nickname.Equals(nickname))
                {
                    return user;
                }
            }
            return null;
        }
    }
}
