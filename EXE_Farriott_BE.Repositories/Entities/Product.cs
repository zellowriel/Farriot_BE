using System;
using System.Collections.Generic;

namespace EXE_Farriott_BE.Repositories.Entities;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal BasePrice { get; set; }

    public bool? IsCustomizable { get; set; }

    public DateTime? CreatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public string? ImageUrl { get; set; }

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}
