using System.Threading.Tasks;
using Infraestructura.Dtos;
using Infraestructura.Models;

namespace Infraestructura.Services
{
    
        
    public class ListService : IListService
    {
        private readonly Context _context;

        public ListService(Context context)
        {
            _context = context;
        }

        public async Task<ItemList> CreateItemListAsync(NewItemListDTO itemListDTO)
        {
            var todoItemList = new ItemList {
                Name = itemListDTO.Name
            };
            _context.ItemLists.Add(todoItemList);
            await _context.SaveChangesAsync();

            return todoItemList;
        }

        public Task<ItemList> CreateTodoItemListAsync(NewItemListDTO itemListDTO)
        {
            throw new System.NotImplementedException();
        }
    }
}
