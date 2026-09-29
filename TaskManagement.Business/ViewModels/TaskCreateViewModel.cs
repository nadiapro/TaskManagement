using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Business.ViewModels;

public class TaskCreateViewModel
{
    [MaxLength(200)]
    [Display(Name = "عنوان")]
    [Required(ErrorMessage = "وارد کردن عنوان الزامی است.")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "توضیحات")]
    public string? Description { get; set; }
    public string? CreatedByUserId { get; set; } = null!;

    [Display(Name = "اختصاص به")]
    public string? AssignedToUserId { get; set; }
}