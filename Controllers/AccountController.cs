using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Infraestructura.Dtos;
using Infraestructura.Models;

namespace Infraestructura.Controllers
{
    [Route("api/users")]
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly Context _context;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            Context context
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context =context;
        }
       
        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody] RegisterDTO dto)
        {
            var user = new User
            {
                UserName = dto.Nickname, 
                Nickname = dto.Nickname
            };
            var result = await _userManager.CreateAsync(user, dto.Password);

            try
            {
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return GenerateJwtToken(dto.Nickname, user);
                }
                else
                {   
                    return "no se pudo generar el token";
                }
              
            }
            catch (System.Exception)
            {

                 throw new ApplicationException("UNKNOWN_ERROR");
            }

        }


        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginDTO dto)
        {
            var result = await _signInManager.PasswordSignInAsync(dto.Nickname, dto.Password, false, false);
            
            if (result.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.Nickname == dto.Nickname);
                return GenerateJwtToken(dto.Nickname, appUser);
            }else
            {
                throw new ApplicationException("Invalid Login"); //TODO resolver con un retorno de error correcto
            }
        }

       

        [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}/todos")]
        public async Task<ActionResult<IEnumerable<ItemDTO>>> GetItemsByUser(string id)
        {
            return await _context.Items.Where(i => i.Responsible.Id == id)
                                            .Select(item => ItemToDTO(item)).ToListAsync();
        }

        [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("{id}/todos/{todoId}")]
        public async Task<IActionResult> AssignResponsibleToItem(string id, long todoId)
        {

            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser == null)
            {
                return NotFound("user not found");
            }

            var todoItem = await _context.Items.FindAsync(todoId);
            if (todoItem == null)
            {
                return NotFound("todo item not found");
            }

            todoItem.Responsible =appUser;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(todoId))
                {
                    return NotFound("todo item not found");
                }

            }

            return NoContent();
        }

        [Authorize(AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}/todos/{todoId}")]
        public async Task<IActionResult> UnnassignResponsibleFromItem(string id, long todoId)
        {

            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser == null)
            {
                return NotFound("user not found");
            }

            var todoItem = await _context.Items.Include(i => i.Responsible).FirstOrDefaultAsync(t => t.Id == todoId);
            if (todoItem == null)
            {
                return NotFound("todo item not found");
            }

            if(!appUser.Equals(todoItem.Responsible))
            {
                return NotFound("item does not belong to user");
            }

            todoItem.Responsible =null;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(todoId))
                {
                    return NotFound("todo item not found");
                }

            }

            return NoContent();
        }


        public static ItemDTO ItemToDTO(Item todoItem) =>
                new ItemDTO
                {
                    Id = todoItem.Id,
                    Name = todoItem.Name,
                    IsComplete = todoItem.IsComplete
                };
        



        private bool ItemExists(long id)
        {
            return _context.Items.Any(e => e.Id == id);
        }


        private string GenerateJwtToken(string email, IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}