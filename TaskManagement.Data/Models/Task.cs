using Microsoft.AspNetCore.Identity;

namespace TaskManagement.Data.Models;

public partial class Task
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string CreatedByUserId { get; set; } = null!;

    public string? AssignedToUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    // Navigation Properties
    public virtual IdentityUser CreatedByUser { get; set; } = null!;

    public virtual IdentityUser? AssignedToUser { get; set; }
}
