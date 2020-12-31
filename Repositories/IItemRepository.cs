using System.Collections.Generic;
using System.Threading.Tasks;
using Infraestructura.Dtos;
using Infraestructura.Models;

namespace Infraestructura.Repositories
{
    public interface IItemRepository
    {
         Task<List<Item>> GetAllAsync();
         

         Task<Item>  Create(Item item);
         


         Task<List<Item>> Include(string user);

         Task<Item> Find(long id);

         Task Save();

           Task Remove(long id);

    }
}