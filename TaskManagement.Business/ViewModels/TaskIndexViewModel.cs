using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Business.ViewModels;

public class TaskIndexViewModel
{
    public int Id { get; set; }

    [Display(Name = "عنوان")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "توضیحات")]
    public string? Description { get; set; }

    [Display(Name = "ایجاد کننده")]
    public string? CreatedByUserName{ get; set; }  = string.Empty;

    [Display(Name = "اختصاص به")]
    public string? AssignedToUserName{ get; set; }

    [Display(Name = "تاریخ ایجاد")]
    public string? CreatedAtPersian {get ;set;}

    [Display(Name = "تاریخ ویرایش")]
    public string? UpdatedAtPersian {get ;set;}

    [Display(Name = "تاریخ حذف")]
    public string? DeletedAtPersian {get ;set;}

    public bool IsDeleted { get; set; }
    public bool? Deletable {get;set;}
    public bool? Updatable {get;set;}

}