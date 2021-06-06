using Prism.Modularity;

namespace Rc.Core.Interfaces
{
    public interface IRcModule : IModule
    {
        bool IsEnabled { get; }
    }
}