using System;
using System.Dynamic;
using Microsoft.AspNetCore.Components;

namespace RazorClassLibrary1
{
    public class CascadingItem
    {
        public static CascadingItem Create<T>(Func<T> getter) => new CascadingItem(typeof(T), () => getter());
        public CascadingItem(Type type, Func<object> getter)
        {
            Type = type;
            ComponentType = typeof(CascadingValue<>).MakeGenericType(type);
            Getter = getter;
        }

        public Type ComponentType { get; }
        public Type Type { get; }
        public Func<object> Getter { get; }

        public void Deconstruct(out Type type, out Func<object> getter)
        {
            type = Type;
            getter = Getter;
        }
    }
}