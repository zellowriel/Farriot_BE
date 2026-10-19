using System;
using System.Collections.Generic;

namespace EXE_Farriott_BE.Repositories.Entities;

public partial class ProductVariant
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string Color { get; set; } = null!;

    public string Size { get; set; } = null!;

    public int? StockQuantity { get; set; }

    public string? Sku { get; set; }

    public DateTime? CreatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public string? ImageUrl { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Product Product { get; set; } = null!;
}
