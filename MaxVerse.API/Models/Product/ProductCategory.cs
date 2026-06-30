using MaxVerse.API.Models.Base;
using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace MaxVerse.API.Models.Product;

public class ProductCategory : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    public int? ParentCategoryId { get; set; }

    public ProductCategory? ParentCategory { get; set; }

    public ICollection<ProductCategory> Children { get; set; }
        = new List<ProductCategory>();

    public ICollection<Product> Products { get; set; }
        = new List<Product>();

    public ICollection<Variation> Variations { get; set; }
        = new List<Variation>();
}