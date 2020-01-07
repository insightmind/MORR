using System.Composition;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using MORR.Shared.Utility;

namespace MORR.Core
{
    /// <summary>
    ///     Bootstraps the application.
    /// </summary>
    public class Bootstrapper : IBootstrapper
    {
        private CompositionHost container;

        private const string moduleSubdirectoryRelativePath = "\\Modules";
        private const string moduleNamePattern = "*.MORR-Module.dll";

        public Bootstrapper()
        {
            LoadFromPath(new FilePath(Directory.GetCurrentDirectory() + moduleSubdirectoryRelativePath));
        }

        public void ComposeImports(object @object)
        {
            container.SatisfyImports(@object);
        }

        private void LoadFromPath(FilePath path)
        {
            var assemblies = Directory.GetFiles(path.ToString(), moduleNamePattern)
                                      .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath);

            var containerConfiguration = new ContainerConfiguration();
            containerConfiguration.WithAssemblies(assemblies)
                                  .WithAssembly(Assembly.GetExecutingAssembly());

            container = containerConfiguration.CreateContainer();
        }
    }
}