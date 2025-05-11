using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebsiteBackend.Data;
using MyWebsiteBackend.Models;

namespace MyWebsiteBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/products
        // Отримати всі продукти
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // Отримати продукти, згруповані по категоріях
        [HttpGet("grouped")]
        public async Task<IActionResult> GetProductsGroupedByCategory()
        {
            var grouped = await _context.Categories
                .Include(c => c.Products)
                .ToListAsync();

            return Ok(grouped.Select(c => new {
                category = c.Name,
                products = c.Products.Select(p => new {
                    p.Name,
                    p.Desc,
                    p.Img,
                    p.Price
                })
            }));
        }


        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProductsGroupedByCategory), new { id = product.Id }, product);
        }

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
