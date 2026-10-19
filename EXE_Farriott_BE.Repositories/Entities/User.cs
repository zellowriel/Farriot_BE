using System;
using System.Collections.Generic;

namespace EXE_Farriott_BE.Repositories.Entities;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Role { get; set; }

    public DateTime? CreatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
