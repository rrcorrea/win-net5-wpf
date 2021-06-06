using System.Reflection;
using Microsoft.Extensions.Logging;
using Prism.Mvvm;
using Rc.Core.Common;

namespace Rc.Shell.ViewModels
{
    class ShellViewModel : BindableBase
    {
        private readonly ILogger<ShellViewModel> _logger;
        private string _footNote;

        public string FootNote { get => _footNote; set => SetProperty(ref _footNote, value); }

        public ShellViewModel(ILogger<ShellViewModel> logger)
        {
            _logger = logger;
            FootNote = $"{Constants.AppName} v{Assembly.GetEntryAssembly().GetName().Version}";
        }
    }
}
