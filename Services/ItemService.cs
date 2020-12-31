using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infraestructura.Dtos;
using Infraestructura.Models;
using Microsoft.EntityFrameworkCore;
using Infraestructura.Repositories;



namespace Infraestructura.Services
{
    public class ItemService : IItemService
    {
        private   Item _item;
        ItemRepository ItemRepository = new ItemRepository();

        public ItemService(Item item)
        {
          _item = item;
        }
        
        public async Task<List<Item>> GetAllAsync()
        {
            return await ItemRepository.GetAllAsync();
        }
        public async Task< List<Item>> GetAsync(string id)
        {
            return await ItemRepository.Include(id);
        }

        public async Task UpdateAsync(long id, ItemDTO dto)
        {
            Item item = await ItemRepository.Find(id);
            item.Name = dto.Name;
            item.IsComplete = dto.IsComplete;
            await ItemRepository.Save();
        } 



        public async Task<Item> Find(long id)
        {
            return await ItemRepository.Find(id);
        }

        public async Task<Item> CreateAsync(NewItemDTO dto, User appUser)
        {
            var Item = new Item
            {
                IsComplete = false,
                Name = dto.Name,
                Responsible = appUser
            };

            
            return await ItemRepository.Create(Item);
        }
        public async Task DeleteAsync(long id)
        {
            await ItemRepository.Remove(id);
        }
    }
}
            

            

       

        
        
