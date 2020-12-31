using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infraestructura.Models;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Repositories
{
    public class ItemRepository : IItemRepository
    {
        
         private readonly Context _context;

         public ItemRepository(Context context)
        {
            _context = context;
            
        }

        public ItemRepository()
        {
        }

        public async  Task<List<Item>> GetAllAsync()
        {
            return await _context.Item.ToListAsync();
        }

        public  async Task<Item>  Create(Item item)
        {
             _context.Item.Add(item);
              await Save();
            return  item;
        }

        public async Task<List<Item>> Include(string user)
        {
            return  await _context.Item.Where(i => i.Responsible.Id ==user).ToListAsync();
                           
        }

        public async Task<Item> Find(long id)
        {
            return await _context.Item.FindAsync(id);
        } 

        public Item Item { get; internal set; }

        public  async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task Remove(long id)
        {     Item item = await Find(id);
              _context.Item.Remove(item);
              await Save();
        }
    }
}