using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Shop.Services;

namespace Shop.Controllers
{
    [Route("users")]
    public class UserController : Controller
    {
        [Route("")]
        [HttpGet]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get
        (
            [FromServices] DataContext context
        )
        {
            try
            {
                var users = await context
                    .Users
                    .AsNoTracking()
                    .ToListAsync();
                return users;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível listar os usuários" });
            }
        }

        [Route("")]
        [HttpPost]
        [AllowAnonymous]
        //[Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Post(
            [FromBody] User model,
            [FromServices] DataContext context
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Users.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível criar usuário" });
            }
        }

        [Route("{id:int}")]
        [HttpPut]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put
        (
            int id,
            [FromBody] User model,
            [FromServices] DataContext context
        )
        {
            try
            {
                if (model.Id != id)
                    return NotFound(new { message = "Usuário não encontrado" });

                context.Entry<User>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Este usuário já foi atualizado" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível atualizar o usuário" });
            }
        }

        [Route("login")]
        [HttpPost]
        public async Task<ActionResult<dynamic>> Authenticate(
            [FromBody] User model,
            [FromServices] DataContext context
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await context
                    .Users
                    .AsNoTracking()
                    .Where(x => x.Username == model.Username && x.Password == model.Password)
                    .FirstOrDefaultAsync();

                if (user == null)
                    return NotFound(new { message = "Usuário ou senha inválido" });

                var token = TokenService.GenerateToken(user);
                return new
                {
                    user,
                    token,
                };
            }
            catch
            {
                return BadRequest(new { message = "Não foi acessar usuário" });
            }
        }
    }
}
