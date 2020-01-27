using System;
using System.Collections.Immutable;
using System.Reactive.Disposables;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using ReactiveUI;

namespace RazorClassLibrary1
{
    public abstract class ThemeableInputComponent<T> : ReactiveObjectInput<T>, IDisposable, IThemeable, IColorable , IStyledComponent
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();
            IThemeable
                .Configure(this, out _appIsDark, out _themeIsDark, out _isDark, out _rootIsDark, out _themeClasses, out _rootThemeClasses, out _cascadingTheme)
                .DisposeWith(_disposables);
            IColorable
                .Configure(this)
                .DisposeWith(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        #region IStyledComponent
        protected AttributeList Attributes { get; } = new AttributeList();
        AttributeList IStyledComponent.Attributes => Attributes;
        protected CssClassList Class { get; } = new CssClassList();
        CssClassList IStyledComponent.Class => Class;
        protected StyleList Style { get; } = new StyleList();
        StyleList IStyledComponent.Style => Style;
        #endregion

        #region IColorable
        private string _color;
        private string _backgroundColor;

        [Parameter]
        public string Color
        {
            get => _color;
            set => this.RaiseAndSetIfChanged(ref _color, value);
        }

        [Parameter]
        public string BackgroundColor
        {
            get => _backgroundColor;
            set => this.RaiseAndSetIfChanged(ref _backgroundColor, value);
        }
        #endregion

        #region IThemeable
        private ObservableAsPropertyHelper<bool> _appIsDark;
        private ObservableAsPropertyHelper<bool> _themeIsDark;
        private ObservableAsPropertyHelper<bool> _isDark;
        private ObservableAsPropertyHelper<bool> _rootIsDark;
        private ObservableAsPropertyHelper<CssClassList> _themeClasses;
        private ObservableAsPropertyHelper<CssClassList> _rootThemeClasses;
        private ObservableAsPropertyHelper<CascadingTheme> _cascadingTheme;
        private CascadingTheme _theme;
        private bool _light;
        private bool _dark;
        private Blazorify _blazorify;

        [Inject]
        public Blazorify Blazorify
        {
            get => _blazorify;
            set => this.RaiseAndSetIfChanged(ref _blazorify, value);
        }

        [Parameter]
        public bool Dark
        {
            get => _dark;
            set => this.RaiseAndSetIfChanged(ref _dark, value);
        }

        [Parameter]
        public bool Light
        {
            get => _light;
            set => this.RaiseAndSetIfChanged(ref _light, value);
        }

        [CascadingParameter]
        public CascadingTheme Theme
        {
            get => _theme;
            set => this.RaiseAndSetIfChanged(ref _theme, value);
        }

        public bool AppIsDark => _appIsDark.Value;
        public bool ThemeIsDark => _themeIsDark.Value;
        public bool IsDark => _isDark.Value;
        public bool RootIsDark => _rootIsDark.Value;
        public CssClassList ThemeClasses => _themeClasses.Value;
        public CssClassList RootThemeClasses => _rootThemeClasses.Value;
        CascadingTheme IThemeable.CascadingTheme => _cascadingTheme.Value;
        #endregion
    }
}