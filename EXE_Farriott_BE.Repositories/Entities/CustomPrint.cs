using System;
using System.Collections.Generic;

namespace EXE_Farriott_BE.Repositories.Entities;

public partial class CustomPrint
{
    public int Id { get; set; }

    public int OrderItemId { get; set; }

    public string UploadedImageUrl { get; set; } = null!;

    public string? PrintMetadata { get; set; }

    public string? CustomerNotes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual OrderItem OrderItem { get; set; } = null!;
}
