using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
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
        private const string moduleSubdirectory = "Modules";
        private const string moduleNamePattern = "*.MORR-Module.dll";
        private CompositionContainer container;

        public Bootstrapper()
        {
            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (currentDirectory == null)
            {
                throw new Exception("Failed to get directory to current assembly.");
            }

            LoadFromPath(new DirectoryPath(Path.Combine(currentDirectory, moduleSubdirectory)));
        }

        public void ComposeImports(object @object)
        {
            container.SatisfyImportsOnce(@object);
        }

        private void LoadFromPath(DirectoryPath path)
        {
            var registrationBuilder = BootstrapperConventions.GetRegistrationBuilder();

            var moduleFiles = Directory.GetFiles(path.ToString(), moduleNamePattern);
            var moduleCatalogs = moduleFiles.Select(x =>
            {
                var loadContext = new ModuleLoadContext(x);
                var assembly = loadContext.LoadFromAssemblyPath(x);
                return new AssemblyCatalog(assembly, registrationBuilder) as ComposablePartCatalog;
            }).ToArray();

            var applicationCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly(), registrationBuilder);
            var aggregateCatalog = new AggregateCatalog(moduleCatalogs.Append(applicationCatalog));

            container = new CompositionContainer(aggregateCatalog);
        }

        private class ModuleLoadContext : AssemblyLoadContext
        {
            private readonly AssemblyDependencyResolver resolver;

            public ModuleLoadContext(string pluginPath)
            {
                resolver = new AssemblyDependencyResolver(pluginPath);
            }

            protected override Assembly? Load(AssemblyName assemblyName)
            {
                var assemblyPath = resolver.ResolveAssemblyToPath(assemblyName);
                return assemblyPath != null
                    ? LoadFromAssemblyPath(assemblyPath)
                    : Default.LoadFromAssemblyName(assemblyName);
            }

            protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
            {
                var libraryPath = resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
                return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : IntPtr.Zero;
            }
        }
    }
}