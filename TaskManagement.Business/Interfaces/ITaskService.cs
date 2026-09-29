using Microsoft.AspNetCore.Identity;
using TaskManagement.Business.ViewModels;

namespace TaskManagement.Business.Interfaces;

public interface ITaskService
{
    //Get Created or Assigned By This User
    Task<List<TaskIndexViewModel>> GetUserTasksAsync(string userId,bool? assigned = false);

    //
    Task CreateAsync(TaskCreateViewModel model);

    //get the task if created by this user
    Task<TaskEditViewModel?> GetForEditAsync(int id,string userId);

    //update the task if created by this user 
    Task<bool> UpdateAsync(TaskEditViewModel model,string userId);

    //soft delete the task if created by this user 
    Task<bool> DeleteAsync(int id,string userId);

    //For Fill ViewData Users
    Task<List<IdentityUser>> GetAllUsers();
}