using System;
using System.Collections.Generic;
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
        private const string moduleNamePattern = "*.dll";
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
            var alreadyLoadedAssemblies = AssemblyLoadContext.Default.Assemblies.Select(x => x.FullName).ToList();
            var moduleFiles = Directory.GetFiles(path.ToString(), moduleNamePattern);
            var moduleAssemblies = moduleFiles
                                   .Select(x => LoadWithReferencesIfNotLoaded(x, alreadyLoadedAssemblies))
                                   .Where(x => x != null)
                                   .Select(x => new AssemblyCatalog(x)).ToArray();

            var applicationCatalog = new ApplicationCatalog();
            var catalogs = moduleAssemblies.Cast<ComposablePartCatalog>().Append(applicationCatalog);

            var aggregateCatalog = new AggregateCatalog(catalogs);

            container = new CompositionContainer(aggregateCatalog);
        }

        private static Assembly? LoadWithReferencesIfNotLoaded(string path, IEnumerable<string?> alreadyLoadedAssemblies)
        {
            var resolver = new AssemblyDependencyResolver(path);
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);

            if (alreadyLoadedAssemblies.Contains(assembly.FullName))
            {
                return null;
            }

            foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
            {
                var referencedAssemblyPath = resolver.ResolveAssemblyToPath(referencedAssembly);

                if (referencedAssemblyPath != null)
                {
                    AssemblyLoadContext.Default.LoadFromAssemblyPath(referencedAssemblyPath);
                }
            }

            return assembly;
        }
    }
}