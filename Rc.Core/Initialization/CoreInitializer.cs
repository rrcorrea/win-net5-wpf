using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Rc.Core.Logging;

namespace Rc.Core.Initialization
{
    public static class CoreInitializer
    {
        public static void RegisterCoreTypes(this IContainerRegistry containerRegistry)
        {
            containerRegistry.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

        }
    }
}
