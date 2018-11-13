using System.Collections.Generic;
using System.Linq;

namespace Entities
{
    public class Repository
    {
        private List<User> users;
        private List<Ranking> rankings;
        private List<Statistic> statistics;

        public Repository()
        {
            users = new List<User>();
            rankings = new List<Ranking>();
            statistics = new List<Statistic>();
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

        public List<Ranking> GetRankings()
        {
            return rankings.OrderByDescending(r => r.Points).Take(10).ToList();
        }

        public void AddRanking(Ranking ranking)
        {
            rankings.Add(ranking);
        }

        public List<Statistic> GetStatistics()
        {
            return statistics.Take(10).ToList();
        }

        public void AddStatistic(Statistic statistic)
        {
            statistics.Add(statistic);
        }
    }
}
