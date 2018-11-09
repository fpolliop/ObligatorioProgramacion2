using System;
using System.Drawing;
namespace Common
{
    public interface IAdminAccess
    {
        void AddPlayer(string nickName, Image avatar);
    }
}
