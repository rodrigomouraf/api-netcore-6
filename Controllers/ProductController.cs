using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1/products")]
    public class ProductController : ControllerBase
    {
        [Route("")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get
        (
            [FromServices] DataContext context
        )
        {
            try
            {
                var products = await context
                    .Products
                    .Include(c => c.Category)
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível listar os produtos." });
            }
        }

        [Route("{id:int}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> Get(
            int id
            ,[FromServices] DataContext context
        )
        {
            try
            {
                var product = await context
                    .Products
                    .Include(c => c.Category)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (product?.Id != id)
                {
                    return NotFound(new { message = "Produto não encontrado" });
                }

                return Ok(product);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível listar os produtos" });
            }
        }

        [Route("categories/{id:int}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategory(
            int id,
            [FromServices] DataContext context
        )
        {
            try
            {
                var product = await context
                    .Products
                    .Include(c => c.Category)
                    .AsNoTracking()
                    .Where(c => c.CategoryId == id)
                    .ToListAsync();

                if (product.Count == 0)
                {
                    return NotFound(new { message = "Não existem produtos para essa categoria." });
                }

                return Ok(product);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível listar os produtos para essa categoria" });
            }
        }

        [Route("")]
        [HttpPost]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Product>> Post
        (
            [FromBody] Product model,
            [FromServices] DataContext context
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Products.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível criar categoria" });
            }
        }
    }
}
