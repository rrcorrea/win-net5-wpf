using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Prism.Unity;
using Rc.Core.Initialization;
using Rc.Shell.ViewModels;
using Rc.Shell.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Prism.Modularity;
using Rc.Core.Interfaces;

namespace Rc.Shell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private ILogger<App> _logger;

        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            base.OnStartup(e);
        }
        
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IConfiguration>(() => new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .Build());
            containerRegistry.RegisterCoreTypes();
            containerRegistry.RegisterSingleton<ShellViewModel>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            //Discover classes that implement IRcModule
            Type moduleInterfaceType = typeof(IRcModule);
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            const string filterName = "Rc.";
            IEnumerable<Type> moduleTypes = Directory.GetFiles(baseDirectory, "*.dll")
                .Where(fileName => fileName.Replace(baseDirectory, string.Empty).Contains(filterName))
                .Select(fileName => Assembly.Load(AssemblyName.GetAssemblyName(fileName)))
                .SelectMany(assembly => assembly.GetTypes()
                    .Where(type =>
                    {
                        try { return moduleInterfaceType.IsAssignableFrom(type) && type.IsClass; }
                        catch { return false; }
                    }))
                .Select(type => (type, module: Container.Resolve(type) as IRcModule))
                .Where(p => p.module is {IsEnabled: true})
                .Select(p=>p.type);
            foreach (var type in moduleTypes)
            {
                moduleCatalog.AddModule(new ModuleInfo
                {
                    ModuleName = type.Name,
                    ModuleType = type.AssemblyQualifiedName
                });
            }
        }

        protected override Window CreateShell()
        {
            _logger = Container.Resolve<ILogger<App>>();
            ShellView view = Container.Resolve<ShellView>();
            view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            return view;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) 
            => LogFinalError(e.Exception);

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) 
            => LogFinalError((Exception)e.ExceptionObject);

        private void LogFinalError(Exception e) => _logger?.LogCritical(e, "Unhandled exception");

        public static App GetCurrentInstance() => App.Current as App;
    }
}
