
using Catalog.DTO;
using Catalog.Models;
using Catalog.Services;
using Microsoft.AspNetCore.Mvc;


namespace Catalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();

            var result = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Stock = p.Stock,
                CreatedAt = p.CreatedAt,
                UserId = p.UserId,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
                ImageData = p.ImageData != null ? System.Convert.ToBase64String(p.ImageData) : null,
                ImageContentType = p.ImageContentType
            });

            return Ok(result);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetById(Guid id)
        {
            var p = await _productService.GetProductByIdAsync(id);
            if (p == null) return NotFound();

            var product = new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Stock = p.Stock,
                CreatedAt = p.CreatedAt,
                UserId = p.UserId,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
                ImageData = p.ImageData != null ? System.Convert.ToBase64String(p.ImageData) : null,
                ImageContentType = p.ImageContentType
            };

            return Ok(product);
        }

        // GET: api/products/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetByUserId(Guid userId)
        {
            var products = await _productService.GetProductsByUserIdAsync(userId);

            var result = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Stock = p.Stock,
                CreatedAt = p.CreatedAt,
                UserId = p.UserId,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
                ImageData = p.ImageData != null ? System.Convert.ToBase64String(p.ImageData) : null,
                ImageContentType = p.ImageContentType
            });

            return Ok(result);
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create([FromForm] ProductCreateDto dto)
        {
            byte[]? imageData = null;
            string? contentType = null;

            if (dto.Image != null && dto.Image.Length > 0)
            {
                using var ms = new MemoryStream();
                await dto.Image.CopyToAsync(ms);
                imageData = ms.ToArray();
                contentType = dto.Image.ContentType;
            }

            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description,
                Stock = dto.Stock,
                UserId = dto.UserId,
                CategoryId = dto.CategoryId,
                ImageData = imageData,
                ImageContentType = contentType
            };

            var created = await _productService.CreateProductAsync(product);

            var result = new ProductDto
            {
                Id = created.Id,
                Name = created.Name,
                Price = created.Price,
                Description = created.Description,
                Stock = created.Stock,
                CreatedAt = created.CreatedAt,
                UserId = created.UserId,
                CategoryId = created.CategoryId,
                CategoryName = created.Category != null ? created.Category.Name : null,
                ImageData = created.ImageData != null ? System.Convert.ToBase64String(created.ImageData) : null,
                ImageContentType = created.ImageContentType
            };

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetByCategory(Guid categoryId)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);

            var result = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Stock = p.Stock,
                CreatedAt = p.CreatedAt,
                UserId = p.UserId,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
                ImageData = p.ImageData != null ? Convert.ToBase64String(p.ImageData) : null,
                ImageContentType = p.ImageContentType
            });

            return Ok(result);
        }


        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] ProductUpdatedDto dto)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.Description = dto.Description;
            product.Stock = dto.Stock;
            product.CategoryId = dto.CategoryId;

            var updated = await _productService.UpdateProductAsync(product);

            var result = new ProductDto
            {
                Id = updated.Id,
                Name = updated.Name,
                Price = updated.Price,
                Description = updated.Description,
                Stock = updated.Stock,
                CreatedAt = updated.CreatedAt,
                UserId = updated.UserId,
                CategoryId = updated.CategoryId,
                CategoryName = updated.Category != null ? updated.Category.Name : null,
                ImageData = updated.ImageData != null ? System.Convert.ToBase64String(updated.ImageData) : null,
                ImageContentType = updated.ImageContentType
            };

            return Ok(result);
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        // GET: api/products/{id}/image
        [HttpGet("{id}/image")]
        public async Task<IActionResult> GetImage(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null || product.ImageData == null) return NotFound();
            return File(product.ImageData, product.ImageContentType);
        }

        // GET: api/products/my
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetMyProducts()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null) return Unauthorized();

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
                return BadRequest("El identificador de usuario no es válido.");

            var products = await _productService.GetProductsByUserIdAsync(userId);

            var result = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Stock = p.Stock,
                CreatedAt = p.CreatedAt,
                UserId = p.UserId,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
                ImageData = p.ImageData != null ? Convert.ToBase64String(p.ImageData) : null,
                ImageContentType = p.ImageContentType
            });

            return Ok(result);
        }

    }
}
