using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infraestructura.Dtos;
using Infraestructura.Models;
using Infraestructura.Services;

namespace Infraestructura.Controllers
{
    [Authorize]
    [Route("api/todolists")]
    [ApiController]
    public class TodoListController : ControllerBase
    {
        private readonly Context _context;
        private readonly IListService _todoListService;

        public TodoListController(Context context, IListService todoListService)
        {
            _context = context;
            _todoListService = todoListService;

        }

        // GET: api/TodoList
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemList>>> GetItemLists()
        {
            return await _context.ItemLists.Include(x => x.Items).ToListAsync();
        }

        // GET: api/TodoList/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemList>> GetTodoItemList(long id)
        {
            var todoItemList = await _context.ItemLists.FindAsync(id);

            if (todoItemList == null)
            {
                return NotFound();
            }

            await _context.Entry(todoItemList).Collection(x => x.Items).LoadAsync();

            return todoItemList;
        }

        // PUT: api/TodoList/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItemList(long id, NewItemListDTO todoItemListDTO)
        {

            var todoItemList = await _context.ItemLists.FindAsync(id);
            if (todoItemList == null)
            {
                return NotFound();
            }

            todoItemList.Name = todoItemListDTO.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemListExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/TodoList
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ItemList>> PostTodoItemList(NewItemListDTO todoItemListDTO)
        {
            var todoItemList = await _todoListService.CreateItemListAsync(todoItemListDTO);

            return CreatedAtAction("GetTodoItemList", new { id = todoItemList.Id }, todoItemList);
        }

        // DELETE: api/TodoList/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ItemList>> DeleteTodoItemList(long id)
        {
            var todoItemList = await _context.ItemLists.FindAsync(id);
            if (todoItemList == null)
            {
                return NotFound();
            }

            await _context.Entry(todoItemList).Collection(x => x.Items).LoadAsync();
            if(todoItemList.Items.Count !=0)
            {
                return BadRequest("Cannot delete a non empty list");
            }

            _context.ItemLists.Remove(todoItemList);
            await _context.SaveChangesAsync();

            return todoItemList;
        }

        [HttpPost("{id}/todos")]
        public async Task<ActionResult<ItemList>> PostTodoItemIntoList(long id, NewItemDTO todoItemDTO)
        {
            var todoItemList = await _context.ItemLists.FindAsync(id);
            if (todoItemList == null)
            {
                return NotFound();
            }
            await _context.Entry(todoItemList).Collection(x => x.Items).LoadAsync();

            var todoItem = new Item
            {
                IsComplete = todoItemDTO.IsComplete,
                Name = todoItemDTO.Name
            };

            todoItemList.Items.Add(todoItem);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemListExists(id))
            {
                return NotFound();
            }

            return CreatedAtAction("PostTodoItemIntoList", new { id = todoItem.Id }, TodoController.ItemToDTO(todoItem));
        }

        [HttpPost("{id}/todos/{todoId}")]
        public async Task<IActionResult> AddTodoItemIntoList(long id, long todoId)
        {

            var todoItemList = await _context.ItemLists.FindAsync(id);
            if (todoItemList == null)
            {
                return NotFound("list not found");
            }

            var todoItem = await _context.Items.FindAsync(todoId);
            if (todoItem == null)
            {
                return NotFound("todo item not found");
            }

            await _context.Entry(todoItemList).Collection(x => x.Items).LoadAsync();
            todoItemList.Items.Add(todoItem);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemListExists(id))
                {
                    return NotFound("list not found");
                }

                if (!TodoItemExists(todoId))
                {
                    return NotFound("todo item not found");
                }

            }

            return NoContent();
        }

        [HttpDelete("{id}/todos/{todoId}")]
        public async Task<IActionResult> RemoveTodoItemFromList(long id, long todoId)
        {

            var todoItemList = await _context.ItemLists.FindAsync(id);
            if (todoItemList == null)
            {
                return NotFound("list not found");
            }

            var todoItem = await _context.Items.FindAsync(todoId);
            if (todoItem == null)
            {
                return NotFound("todo item not found");
            }

            await _context.Entry(todoItemList).Collection(x => x.Items).LoadAsync();
            todoItemList.Items.Remove(todoItem);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemListExists(id))
                {
                    return NotFound("list not found");
                }

                if (!TodoItemExists(todoId))
                {
                    return NotFound("todo item not found");
                }

            }

            return NoContent();
        }

        //TODO: Hacer una implementacion generica!
        private bool TodoItemListExists(long id)
        {
            return _context.ItemLists.Any(e => e.Id == id);
        }

        private bool TodoItemExists(long id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
    }
}