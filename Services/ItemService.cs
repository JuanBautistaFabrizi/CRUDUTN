using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infraestructura.Dtos;
using Infraestructura.Models;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Services
{
    public class ItemService : IItemService
    {
        private readonly Context _context;

        public ItemService(Context context)
        {
            _context = context;
        }

         public async Task<List<Item>> GetAllAsync()
        {
            return await _context.Items.ToListAsync();
        }

        public async Task<Item> GetAsync(long id)
        {
            return await _context.Items.Include(i => i.Responsible).
                            FirstOrDefaultAsync( i => i.Id ==id);
        }

        public async Task UpdateAsync(long id, ItemDTO dto)
        {
            var item = await _context.Items.FindAsync(id);

            if(item is null)
            {
                throw new ArgumentException("item not found");
            }
            
            item.Name = dto.Name;
            item.IsComplete = dto.IsComplete;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                throw new InvalidOperationException($"item {id} has been deleted");
            }
        }

        public async Task<Item> CreateAsync(NewItemDTO dto, User appUser)
        {
            var todoItem = new Item
            {
                IsComplete = false,
                Name = dto.Name,
                Responsible = appUser
            };

            _context.Items.Add(todoItem);
            await _context.SaveChangesAsync();

            return todoItem;
        }
        public async Task DeleteAsync(long id)
        {
            var todoItem = await _context.Items.FindAsync(id);
            if (todoItem == null)
            {
                throw new ArgumentException("item not found");
            }

            _context.Items.Remove(todoItem);
            await _context.SaveChangesAsync();
        }

        private bool TodoItemExists(long id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
    }
}
