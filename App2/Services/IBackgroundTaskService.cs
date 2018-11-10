using System.Threading.Tasks;

using Windows.ApplicationModel.Background;

namespace App2.Services
{
    internal interface IBackgroundTaskService
    {
        Task RegisterBackgroundTasksAsync();

        void Start(IBackgroundTaskInstance taskInstance);
    }
}
