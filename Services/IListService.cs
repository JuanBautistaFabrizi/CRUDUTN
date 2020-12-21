using System.Threading.Tasks;
using Infraestructura.Dtos;
using Infraestructura.Models;

namespace Infraestructura.Services
{
    public interface IListService
    {
         Task<ItemList> CreateItemListAsync(NewItemListDTO itemListDTO);
    }
}