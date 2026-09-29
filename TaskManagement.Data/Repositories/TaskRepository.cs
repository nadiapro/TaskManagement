using TaskManagement.Data.Context;

namespace TaskManagement.Data.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskManagementDbContext _context;

    public TaskRepository(TaskManagementDbContext context)
    {
        _context = context;
    }

    public IQueryable<Models.Task> GetAll()
    {
        return _context.Tasks;
    }

    public async Task AddAsync(Models.Task task)
    {
        await _context.Tasks.AddAsync(task);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}