using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Business.ViewModels;

public class TaskEditViewModel
{
    public int Id { get; set; }

    [MaxLength(200)]
    [Display(Name = "عنوان")]
    [Required(ErrorMessage = "وارد کردن عنوان الزامی است.")]
    public string Title { get; set; } = string.Empty;
    [Display(Name = "توضیحات")]
    public string? Description { get; set; }

    [Display(Name = "اختصاص به")]
    public string? AssignedToUserId { get; set; }
}