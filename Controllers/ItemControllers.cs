using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infraestructura.Models;
using Infraestructura.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Infraestructura.Services;

namespace Infraestructura.Controllers
{
    [Authorize]
    [Route("api/items")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        private readonly IItemService _itemService;

        public ItemController(UserManager<User> userManager,
                                IItemService todoItemService)
        {
            _userManager =userManager;
            _itemService =todoItemService;
        }

        // GET: api/Todo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDTO>>> GetItems()
        {
            var todoItems = await _itemService.GetAllAsync();
            return todoItems.Select(item => ItemToDTO(item)).ToList();
        }

        // GET: api/item/5
         [HttpGet("{id}")]
        public async Task<ActionResult<ItemDTO>> GetItem(long id)
        {
            Item item = await _itemService.Find(id);

            if (item == null)
            {
                return NotFound();
            }


            return ItemToDTO(item);
        } 

        // PUT: api/Todo/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(long id, ItemDTO itemDTO)
        {
            if (id != itemDTO.Id)
            {
                return BadRequest();
            }

            try
            {
               await _itemService.UpdateAsync(id, itemDTO);            
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return NotFound(e.Message);
            }

            return NoContent();
        } 

        // POST: api/item
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ItemDTO>> PostTodoItem(NewItemDTO itemDTO)
        {
            User appUser =null;
            
            if(itemDTO.UserId !=null)
            {
                appUser = await _userManager.FindByIdAsync(itemDTO.UserId);

                if(appUser is null)
                {
                    return BadRequest("Invalid userId");
                }

            }

            var Item = await _itemService.CreateAsync(itemDTO, appUser);

            return CreatedAtAction("GetTodoItem", new { id = Item.Id }, ItemToDTO(Item));
        }

         //DELETE: api/item/5
         [HttpDelete("{id}")]
        public async Task<ActionResult<ItemDTO>> DeleteTodoItem(long id)
        {
            try
            {
                await _itemService.DeleteAsync(id);            
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }

            return NoContent();
        } 

        public static ItemDTO ItemToDTO(Item item) =>
                new ItemDTO
                {
                    Id = item.Id,
                    Name = item.Name,
                    IsComplete = item.IsComplete,
                    Responsible = item.Responsible is null ? "" : item.Responsible.Email
                };
    }
}