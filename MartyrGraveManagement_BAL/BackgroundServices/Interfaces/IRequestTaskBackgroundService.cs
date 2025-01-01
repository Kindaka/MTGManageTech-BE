namespace MartyrGraveManagement_BAL.BackgroundServices.Interfaces
{
    public interface IRequestTaskBackgroundService
    {
        Task CheckExpiredRequestTask();
    }
}
