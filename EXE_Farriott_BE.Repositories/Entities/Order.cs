using System;
using System.Collections.Generic;

namespace EXE_Farriott_BE.Repositories.Entities;

public partial class Order
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Status { get; set; }

    public string ShippingAddress { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
    public string? OrderCode { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User? User { get; set; }
}
