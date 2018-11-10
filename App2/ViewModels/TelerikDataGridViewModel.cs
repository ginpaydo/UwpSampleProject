using System.Collections.ObjectModel;

using App2.Models;
using App2.Services;

using Prism.Windows.Mvvm;

namespace App2.ViewModels
{
    public class TelerikDataGridViewModel : ViewModelBase
    {
        private readonly ISampleDataService _sampleDataService;

        public TelerikDataGridViewModel(ISampleDataService sampleDataServiceInstance)
        {
            _sampleDataService = sampleDataServiceInstance;
        }

        public ObservableCollection<SampleOrder> Source => _sampleDataService.GetGridSampleData();
    }
}
