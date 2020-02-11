using System;
using System.Linq;
using System.Runtime.Loader;

namespace MORR.Shared.Utility
{
    public static class Utility
    {
        /// <summary>
        ///     Sets a boolean property and dispatches based on its value.
        /// </summary>
        /// <param name="variable">The property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        /// <param name="onTrue">
        ///     The action to execute if <paramref name="variable" /> was not set to <see langword="true" />
        ///     before calling this method, but <paramref name="value" /> is <see langword="true" />.
        /// </param>
        /// <param name="onFalse">
        ///     The action to execute if <paramref name="variable" /> was not set to <see langword="false" />
        ///     before calling this method, but <paramref name="value" /> is <see langword="false" />.
        /// </param>
        public static void SetAndDispatch(ref bool variable, bool value, Action onTrue, Action onFalse)
        {
            if (variable == value)
            {
                return;
            }

            variable = value;

            if (variable)
            {
                onTrue();
            }
            else
            {
                onFalse();
            }
        }

        /// <summary>
        ///     Attempts to load the type with the specified name from any currently loaded assembly.
        /// </summary>
        /// <param name="type">The name of the type to load.</param>
        /// <returns>The <see cref="Type" /> with the corresponding name or <see cref="null" /> on failure.</returns>
        public static Type? GetTypeFromAnyAssembly(string type)
        {
            return Type.GetType(type) ?? AssemblyLoadContext.All
                                                            .SelectMany(x => x.Assemblies)
                                                            .Select(x => x.GetType(type))
                                                            .FirstOrDefault(loadedType => loadedType != null);
        }
    }
}