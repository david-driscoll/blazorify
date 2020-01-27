using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace RazorClassLibrary1
{
    public interface ICascadingProvider
    {
        protected static void AddCascadingValue<TValue>(ICascadingProvider cascadingProvider, Func<TValue> valueFunc)
        {
            cascadingProvider.CascadingValues =
                cascadingProvider.CascadingValues.Add(CascadingItem.Create(valueFunc));
        }

        public ImmutableArray<CascadingItem> CascadingValues { get; protected set; }
    }
}