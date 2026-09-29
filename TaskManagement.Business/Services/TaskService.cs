using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Business.Helpers;
using TaskManagement.Business.Interfaces;
using TaskManagement.Business.ViewModels;
using TaskManagement.Data.Repositories;

namespace TaskManagement.Business.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly UserManager<IdentityUser> _userManager;

    public TaskService(
        ITaskRepository taskRepository,
        UserManager<IdentityUser> userManager)
    {
        _taskRepository = taskRepository;
        _userManager=userManager;
    }

   public async Task<List<TaskIndexViewModel>> GetUserTasksAsync(string userId,bool? assigned = false)
    {
        //base query
        var query = _taskRepository.GetAll();
        //eager load
        query = query.Include(t => t.AssignedToUser);
        //where just assigned is selected or not
        if (assigned == true)
            query = query.Where(t => t.AssignedToUserId == userId );
        else
            query = query.Where(t => t.CreatedByUserId == userId || t.AssignedToUserId == userId );
        //order
        query = query.OrderByDescending(t => t.CreatedAt);
        //projection
        return await query.Select(t => new TaskIndexViewModel
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                CreatedByUserName = t.CreatedByUser.UserName,
                AssignedToUserName = t.AssignedToUser != null ? t.AssignedToUser.UserName : null,  
                //conver to persian date            
                CreatedAtPersian = PersianDateHelper.ToPersianDateTime(t.CreatedAt),
                UpdatedAtPersian = t.UpdatedAt != null 
                      ? PersianDateHelper.ToPersianDateTime((DateTime)t.UpdatedAt) : null,
                DeletedAtPersian = t.DeletedAt != null 
                       ? PersianDateHelper.ToPersianDateTime((DateTime)t.DeletedAt) : null,
                //
                IsDeleted = t.IsDeleted,
                //to show or hide buttons
                Updatable = t.CreatedByUserId == userId,
                Deletable = t.CreatedByUserId == userId
            })
            .ToListAsync();
    } 

    public async Task CreateAsync(TaskCreateViewModel model)
    {
        var task = new Data.Models.Task
        {
            Title = model.Title,
            Description = model.Description,
            AssignedToUserId = model.AssignedToUserId,
            CreatedByUserId = model.CreatedByUserId ?? ""
        };
        await _taskRepository.AddAsync(task);
        await _taskRepository.SaveChangesAsync();
    }

    public async Task<TaskEditViewModel?> GetForEditAsync(int id,string userId)
    {
        //Just Created By User Task That Is Not Deleted
        var task = await GetUserTaskQuery(userId)
        .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        //
        if (task == null)
            return null;

        return new TaskEditViewModel
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            AssignedToUserId = task.AssignedToUserId,
        };
    }

    public async Task<bool> UpdateAsync(TaskEditViewModel model,string userId)
    {
        //Just Created By User Task That Is Not Deleted
        var task = await GetUserTaskQuery(userId)
        .FirstOrDefaultAsync(t => t.Id == model.Id && !t.IsDeleted);

        if (task == null)
            return false;

        task.Title = model.Title;
        task.Description = model.Description;
        task.AssignedToUserId = model.AssignedToUserId;
        task.UpdatedAt = DateTime.UtcNow;

        await _taskRepository.SaveChangesAsync();

        return true;
    }

   public async Task<bool> DeleteAsync(int id,string userId)
    {
        //Just Created By User Task That Is Not Deleted
        var task = await GetUserTaskQuery(userId)
        .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (task == null)
            return false;

        task.IsDeleted = true;
        task.DeletedAt = DateTime.UtcNow;

        await _taskRepository.SaveChangesAsync();

        return true;
    }
    public async Task<List<IdentityUser>> GetAllUsers()
    {
        return await _userManager.Users.ToListAsync();
    }

    //Return A Query For Tasks created by this user
    private IQueryable<Data.Models.Task> GetUserTaskQuery(string userId)
    {  
        return _taskRepository.GetAll().Where(t => t.CreatedByUserId == userId);

    }
}