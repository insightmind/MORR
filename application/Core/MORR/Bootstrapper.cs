using System;
using System.Collections.Generic;
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
        private const string moduleSubdirectory = "Modules";
        private const string moduleNamePattern = "*.MORR-Module.dll";
        private CompositionHost container;

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
            container.SatisfyImports(@object);
        }

        private void LoadFromPath(DirectoryPath path)
        {
            var moduleAssemblies = Directory.GetFiles(path.ToString(), moduleNamePattern)
                                            .Select(ModuleLoadContext.Current.LoadModule);

            var containerConfiguration = new ContainerConfiguration();
            containerConfiguration.WithAssemblies(moduleAssemblies)
                                  .WithAssembly(Assembly.GetExecutingAssembly())
                                  .WithAssembly(Assembly.GetEntryAssembly());

            container = containerConfiguration.CreateContainer();
        }

        private class ModuleLoadContext : AssemblyLoadContext
        {
            private readonly List<AssemblyDependencyResolver> resolvers = new List<AssemblyDependencyResolver>();

            public static ModuleLoadContext Current { get; } = new ModuleLoadContext();

            public Assembly LoadModule(string modulePath)
            {
                var resolver = new AssemblyDependencyResolver(modulePath);
                resolvers.Add(resolver);

                Default.LoadFromAssemblyPath(
                    modulePath); // TODO This is only to make Type.GetType work elsewhere, remove if possible

                return LoadFromAssemblyPath(modulePath);
            }

            protected override Assembly Load(AssemblyName assemblyName)
            {
                var assemblyPath = resolvers.Select(x => x.ResolveAssemblyToPath(assemblyName))
                                            .FirstOrDefault(x => x != null);

                // If the path cannot be resolved, do not return anything
                // TODO This appears to work fine, resolve only fails for library dependencies, but this is not a good solution
                return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
            }
        }
    }
}