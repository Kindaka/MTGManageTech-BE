namespace MartyrGraveManagement.BackgroundServices.Interfaces
{
    public interface ITaskBackgroundService
    {
        Task CheckExpiredTasks();
    }
}
