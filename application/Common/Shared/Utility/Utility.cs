using System;

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
    }
}