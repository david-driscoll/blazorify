using System;
using System.Reactive.Disposables;
using Microsoft.AspNetCore.Components;
using ReactiveUI;

namespace RazorClassLibrary1
{
    public interface IColorable : IReactiveObject
    {
        [Parameter]
        string Color { get; set; }

        [Parameter]
        string BackgroundColor { get; set; }

        public static IDisposable Configure(IColorable @this)
        {
            return Disposable.Empty;
        }
    }
}