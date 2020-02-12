// SwitchCaseConverter adapted from https://referencesource.microsoft.com/#System.Activities.Presentation/System.Activities.Presentation/System/Activities/Presentation/Base/Core/Internal/PropertyEditing/FromExpression/Framework/Data/SwitchConverter.cs
// License below
// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace MORR.Core.UI.Utility
{
    // <summary>
    // Transformer which maps from input values to output values, based on a list of SwitchCase children.
    // This isn't strictly a C-style 'switch' statement, since cases aren't guaranteed to be unique.
    // </summary>
    //
    [ContentProperty("Cases")]
    internal class SwitchConverter : DependencyObject, IValueConverter
    {
        private static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.Register(nameof(DefaultValue), typeof(object), typeof(SwitchConverter));

        public SwitchConverter()
        {
            Cases = new List<SwitchCase>();
        }

        public List<SwitchCase> Cases { get; }

        public object DefaultValue
        {
            get => GetValue(DefaultValueProperty);
            set => SetValue(DefaultValueProperty, value);
        }

        // IValueConverter implementation
        public object Convert(object o, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var switchCase in Cases.Where(switchCase => Equals(switchCase.In, o)))
            {
                return switchCase.Out;
            }

            return DefaultValue;
        }

        public object ConvertBack(object o, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back.");
        }
    }

    // <summary>
    // Represents a mapping from an input value to an output value.
    // </summary>
    internal class SwitchCase : DependencyObject
    {
        private static readonly DependencyProperty InProperty =
            DependencyProperty.Register(nameof(In), typeof(object), typeof(SwitchCase));

        private static readonly DependencyProperty OutProperty =
            DependencyProperty.Register(nameof(Out), typeof(object), typeof(SwitchCase));

        public object In
        {
            get => GetValue(InProperty);
            set => SetValue(InProperty, value);
        }

        public object Out
        {
            get => GetValue(OutProperty);
            set => SetValue(OutProperty, value);
        }
    }
}