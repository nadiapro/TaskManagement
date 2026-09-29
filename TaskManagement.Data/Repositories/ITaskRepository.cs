namespace TaskManagement.Data.Repositories;

public interface ITaskRepository
{
    IQueryable<Models.Task> GetAll();

    Task AddAsync(Models.Task task);

    Task SaveChangesAsync();
}