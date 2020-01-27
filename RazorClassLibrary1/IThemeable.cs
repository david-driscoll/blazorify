using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using ReactiveUI;

namespace RazorClassLibrary1
{
    public interface IThemeable : IReactiveObject, ICascadingProvider
    {
        public static IDisposable Configure(
            IThemeable @this,
            out ObservableAsPropertyHelper<bool> appIsDark,
            out ObservableAsPropertyHelper<bool> themeIsDark,
            out ObservableAsPropertyHelper<bool> isDark,
            out ObservableAsPropertyHelper<bool> rootIsDark,
            out ObservableAsPropertyHelper<CssClassList> themeClasses,
            out ObservableAsPropertyHelper<CssClassList> rootThemeClasses,
            out ObservableAsPropertyHelper<CascadingTheme> cascadingTheme)
        {
            var disposable = new CompositeDisposable();
            AddCascadingValue(@this, () => @this.CascadingTheme);

            @this.WhenAnyValue(x => x.Blazorify.Theme.Dark)
                .ToProperty(@this, nameof(IThemeable.AppIsDark), out appIsDark)
                .DisposeWith(disposable);

            @this.WhenAnyValue(x => x.Theme.IsDark)
                .StartWith(false)
                .ToProperty(@this, nameof(IThemeable.ThemeIsDark), out themeIsDark)
                .DisposeWith(disposable);

            @this.WhenAnyValue(x => x.ThemeIsDark, z => z.Dark, z => z.Light, x => x.AppIsDark, (themeIsDark, dark, light, appIsDark) =>
                {
                    if (dark) return true;
                    if (light) return false;
                    if (themeIsDark) return true;
                    return appIsDark;
                })
                .ToProperty(@this, nameof(IThemeable.IsDark), out isDark)
                .DisposeWith(disposable);

            @this.WhenAnyValue(x => x.AppIsDark, z => z.Dark, z => z.Light, (darkApp, dark, light) =>
                {
                    if (dark) return true;
                    return !light && darkApp;
                })
                .ToProperty(@this, nameof(IThemeable.RootIsDark), out rootIsDark)
                .DisposeWith(disposable);

            @this.WhenAnyValue(x => x.IsDark)
                .Select(dark => CssClassList.Create(new Dictionary<string, bool>()
                {
                    ["theme--dark"] = dark,
                    ["theme--light"] = !dark,
                }))
                .ToProperty(@this, nameof(IThemeable.ThemeClasses), out themeClasses)
                .DisposeWith(disposable);

            @this.WhenAnyValue(z => z.IsDark)
                .Scan(new CascadingTheme(), (acc, value) =>
                {
                    acc.IsDark = value;
                    return acc;
                })
                .ToProperty(@this, nameof(IThemeable.CascadingTheme), out cascadingTheme)
                .DisposeWith(disposable);

            @this.WhenAnyValue(x => x.RootIsDark)
                .Select(dark => CssClassList.Create(new Dictionary<string, bool>()
                {
                    ["theme--dark"] = dark,
                    ["theme--light"] = !dark,
                }))
                .ToProperty(@this, nameof(IThemeable.RootThemeClasses), out rootThemeClasses)
                .DisposeWith(disposable);

            return disposable;
        }

        [Inject]
        Blazorify Blazorify { get; set; }

        [Parameter]
        bool Dark { get; set; }

        [Parameter]
        bool Light { get; set; }

        [CascadingParameter]
        CascadingTheme Theme { get; set; }
        CascadingTheme CascadingTheme { get; }

        bool AppIsDark { get; }
        bool ThemeIsDark { get; }
        bool IsDark { get; }
        bool RootIsDark { get; }
        CssClassList ThemeClasses { get; }
        CssClassList RootThemeClasses { get; }
    }
}