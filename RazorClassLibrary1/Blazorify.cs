using Microsoft.AspNetCore.Components;
using ReactiveUI;

namespace RazorClassLibrary1
{
    public class Blazorify
    {
        public BlazorifyTheme Theme { get; } = new BlazorifyTheme();
        public BlazorifyIcons Icons { get; } = new BlazorifyIcons();
    }

    public class BlazorifyIcons : ReactiveObject
    {
        public MarkupString IndeterminateIcon { get; set; } = new MarkupString("mdi mdi-minus-box");
        public MarkupString OffIcon { get; set; } = new MarkupString("mdi mdi-checkbox-blank-outline");
        public MarkupString OnIcon { get; set; } = new MarkupString("mdi mdi-checkbox-marked");
    }
}