using System.Collections.Generic;
using System.Threading.Tasks;
using Infraestructura.Models;

namespace Infraestructura.Properties
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAll();
        User GetById(int UserID);
        void Insert(User user);
        void Update(User user);
        void Delete(int UserID);
        
    }
}