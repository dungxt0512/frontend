using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MaxVerse.API.Data;
using MaxVerse.API.DTOs;
using MaxVerse.API.Models;

namespace MaxVerse.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly MaxVerseDbContext _context;

    public ProductsController(MaxVerseDbContext context)
    {
        _context = context;
    }

    // GET /api/products?keyword=&brandId=&categoryId=&minPrice=&maxPrice=&sortBy=&page=&pageSize=
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] ProductFilterDto filter)
    {
        var query = _context.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Variants)
            .Where(p => p.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
            query = query.Where(p => p.ProductName.Contains(filter.Keyword));

        if (filter.BrandId.HasValue)
            query = query.Where(p => p.BrandId == filter.BrandId);

        if (filter.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == filter.CategoryId);

        if (filter.MinPrice.HasValue)
            query = query.Where(p => (p.DiscountPrice ?? p.Price) >= filter.MinPrice);

        if (filter.MaxPrice.HasValue)
            query = query.Where(p => (p.DiscountPrice ?? p.Price) <= filter.MaxPrice);

        query = filter.SortBy switch
        {
            "price_asc" => query.OrderBy(p => p.DiscountPrice ?? p.Price),
            "price_desc" => query.OrderByDescending(p => p.DiscountPrice ?? p.Price),
            "newest" => query.OrderByDescending(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(p => new ProductListItemDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Price = p.Price,
                DiscountPrice = p.DiscountPrice,
                BrandName = p.Brand!.BrandName,
                CategoryName = p.Category!.CategoryName,
                ImageUrl = p.ImageUrl,
                InStock = p.Variants.Sum(v => v.Quantity) > 0
            })
            .ToListAsync();

        return Ok(new PagedResultDto<ProductListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        });
    }

    // GET /api/products/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _context.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.ProductId == id && p.IsActive);

        if (product == null) return NotFound(new { message = "Không tìm thấy sản phẩm." });

        return Ok(new ProductDetailDto
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            Description = product.Description,
            Price = product.Price,
            DiscountPrice = product.DiscountPrice,
            BrandName = product.Brand!.BrandName,
            CategoryName = product.Category!.CategoryName,
            ImageUrl = product.ImageUrl,
            Images = product.Images.Select(i => i.ImageUrl).ToList(),
            Variants = product.Variants.Select(v => new VariantDto
            {
                VariantId = v.VariantId,
                Size = v.Size,
                Color = v.Color,
                Quantity = v.Quantity
            }).ToList()
        });
    }

    // GET /api/products/brands
    [HttpGet("brands")]
    public async Task<IActionResult> GetBrands()
    {
        var brands = await _context.Brands
            .Select(b => new { b.BrandId, b.BrandName, b.LogoUrl })
            .ToListAsync();
        return Ok(brands);
    }

    // GET /api/products/categories
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.Categories
            .Select(c => new { c.CategoryId, c.CategoryName })
            .ToListAsync();
        return Ok(categories);
    }

    // ===== ADMIN ENDPOINTS =====

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateProduct(CreateProductDto dto)
    {
        var product = new Product
        {
            ProductName = dto.ProductName,
            Description = dto.Description,
            Price = dto.Price,
            DiscountPrice = dto.DiscountPrice,
            BrandId = dto.BrandId,
            CategoryId = dto.CategoryId,
            ImageUrl = dto.ImageUrl,
            Variants = dto.Variants.Select(v => new ProductVariant
            {
                Size = v.Size,
                Color = v.Color,
                Quantity = v.Quantity
            }).ToList(),
            StockQuantity = dto.Variants.Sum(v => v.Quantity)
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, new { product.ProductId });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProduct(int id, CreateProductDto dto)
    {
        var product = await _context.Products.Include(p => p.Variants).FirstOrDefaultAsync(p => p.ProductId == id);
        if (product == null) return NotFound();

        product.ProductName = dto.ProductName;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.DiscountPrice = dto.DiscountPrice;
        product.BrandId = dto.BrandId;
        product.CategoryId = dto.CategoryId;
        product.ImageUrl = dto.ImageUrl;

        // Cập nhật variants: xóa hết và tạo lại cho đơn giản (phù hợp đồ án)
        _context.ProductVariants.RemoveRange(product.Variants);
        product.Variants = dto.Variants.Select(v => new ProductVariant
        {
            Size = v.Size,
            Color = v.Color,
            Quantity = v.Quantity
        }).ToList();
        product.StockQuantity = dto.Variants.Sum(v => v.Quantity);

        await _context.SaveChangesAsync();
        return Ok(new { message = "Cập nhật sản phẩm thành công." });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        // Soft delete để giữ lại lịch sử đơn hàng đã tham chiếu sản phẩm này
        product.IsActive = false;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Đã xóa sản phẩm." });
    }
}
