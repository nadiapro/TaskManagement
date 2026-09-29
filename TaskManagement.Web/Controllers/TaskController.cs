using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManagement.Business.Interfaces;
using TaskManagement.Business.ViewModels;
using TaskManagement.Web.Helpers;

namespace TaskManagement.Web.Controllers;

[Authorize]
public class TaskController : Controller
{
    private readonly ITaskService _taskService;
    private readonly ICurrentUserService _currentUser;
    public TaskController(ITaskService taskService , ICurrentUserService currentUser)
    {
        _taskService = taskService;
        _currentUser = currentUser;
    }

   public async Task<IActionResult> Index(bool? assigned = false)
    {
        var tasks = await _taskService.GetUserTasksAsync(_currentUser.UserId ,assigned);

        return View(tasks);
    }

  [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PrepareViewData();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PrepareViewData();
            return View(model);
        }
        model.CreatedByUserId = _currentUser.UserId ;             
        await _taskService.CreateAsync(model);

        TempData["Message"] = "رکورد با موفقیت ایجاد شد";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _taskService.GetForEditAsync(id,_currentUser.UserId);
        if (model == null)
            return NotFound();
      
        await PrepareViewData();
        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TaskEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PrepareViewData();
            return View(model);
        }

        var result = await _taskService.UpdateAsync(model,_currentUser.UserId );

        if (!result)
            return NotFound();

        TempData["Message"] = "رکورد با موفقیت ویرایش شد";
        return RedirectToAction(nameof(Index));
      
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _taskService.DeleteAsync(id , _currentUser.UserId );

        if (!result)
            return NotFound();

        TempData["Message"] = "رکورد با موفقیت حذف شد";
        return RedirectToAction(nameof(Index));
    }

    //To Fill ViewData Such Users
    public async Task PrepareViewData()
    {
        var Users = new List<SelectListItem>();
        foreach(var user in await _taskService.GetAllUsers())
        {
            Users.Add(new SelectListItem{Value= user.Id , Text=user.UserName});
        }
        ViewData["Users"] = Users;  
    }

}