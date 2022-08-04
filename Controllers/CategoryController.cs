using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

[Route("categories")]
public class CategoryController : ControllerBase
{
    [Route("")]
    [HttpGet]
    public async Task<ActionResult<List<Category>>> Get
    (
        [FromServices] DataContext context
    )
    {
        try
        {
            return Ok(await context.Categories.AsNoTracking().ToListAsync());
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Não foi possível listar as categorias." });
        }
    }

    [Route("{id:int}")]
    [HttpGet]
    public async Task<ActionResult<Category>> Get(
        int id
        ,[FromServices] DataContext context
    )
    {
        try
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (category?.Id != id)
            {
                return NotFound(new { message = "Categoria não encontrada" });
            }

            return Ok(category);
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Categoria foi possível listar a categoria" });
        }
    }

    [Route("")]
    [HttpPost]
    public async Task<ActionResult<Category>> Post(
        [FromBody] Category model,
        [FromServices] DataContext context
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            context.Categories.Add(model);
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch
        {
            return BadRequest(new { message = "Não foi possível criar categoria" });
        }
    }

    [Route("{id:int}")]
    [HttpPut]
    public async Task<ActionResult<Category>> Put(
        int id,
        [FromBody] Category model,
        [FromServices] DataContext context
    )
    {
        try
        {
            if (model.Id != id)
                return NotFound(new { message = "Categoria não encontrada" });

            context.Entry<Category>(model).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest(new { message = "Este registro já foi atualizado" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Não foi possível atualizar a categoria" });
        }
    }

    [Route("{id:int}")]
    [HttpDelete]
    public async Task<ActionResult<Category>> Delete(
        int id,
        [FromServices] DataContext context
    )
    {
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound(new { message = "Categoria não encontrada" });
            }

            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return Ok(new { message = "Categoria removida com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Não foi possível remover a categoria" });
        }
    }
}