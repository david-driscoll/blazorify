using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using ReactiveUI;

namespace RazorClassLibrary1
{
    public class VLabel : ThemeableComponent
    {
        private bool _absolute;
        private bool _disabled;
        private bool _value;
        private CssValue _right;
        private CssValue _left;
        private string _for;
        private bool _focused;
        private RenderFragment _childContent;

        public VLabel()
        {
            Color = "primary";
            Left = 0;
            Right = "auto";
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (string.IsNullOrEmpty(Color)) Color = "primary";
        }

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> Attributes { get; set; }

        [Parameter]
        public bool Absolute
        {
            get => _absolute;
            set => this.RaiseAndSetIfChanged(ref _absolute, value);
        }

        [Parameter]
        public bool Disabled
        {
            get => _disabled;
            set => this.RaiseAndSetIfChanged(ref _disabled, value);
        }

        [Parameter]
        public bool Focused
        {
            get => _focused;
            set => this.RaiseAndSetIfChanged(ref _focused, value);
        }

        [Parameter]
        public string For
        {
            get => _for;
            set => this.RaiseAndSetIfChanged(ref _for, value);
        }

        [Parameter]
        public CssValue Left
        {
            get => _left;
            set => this.RaiseAndSetIfChanged(ref _left, value);
        }

        [Parameter]
        public CssValue Right
        {
            get => _right;
            set => this.RaiseAndSetIfChanged(ref _right, value);
        }

        [Parameter]
        public bool Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        [Parameter]
        public RenderFragment? ChildContent
        {
            get => _childContent;
            set => this.RaiseAndSetIfChanged(ref _childContent, value);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            var sequence = 0;
            builder.OpenElementNode(ref sequence, "label", node =>
            {
                node
                    .AddClass("v-label")
                    .AddClass("v-label--active", Value)
                    .AddClass("v-label--is-disabled", Disabled)
                    .AddClasses(ThemeClasses)
                    .AddStyle("left", Left)
                    .AddStyle("right", Right)
                    .AddStyle("position", Absolute ? "absolute" : "relative")
                    .Add(this);

                if (Focused && !string.IsNullOrWhiteSpace(Color))
                    node.SetTextColor(Color);
            });

            builder.AddMultipleAttributes(sequence++, Attributes);
            builder.AddAttribute(sequence++, "for", For);
            builder.AddAttribute(sequence++, "aria-hidden", string.IsNullOrWhiteSpace(For));

            builder.AddContent(sequence++, ChildContent);

            builder.CloseElement();
        }
    }

    public interface ISizeable : IReactiveObject
    {
        public static IDisposable Configure(ISizeable @this, out ObservableAsPropertyHelper<bool> normal, out ObservableAsPropertyHelper<CssClassList> sizeableClasses)
        {
            var disposables = new CompositeDisposable();
            
            @this.WhenAnyValue(z => z.Large, z => z.ExtraLarge, z => z.ExtraSmall, z => z.Small,
                    (lg, xl, xs, sm) => !lg && !xl && !xs && !sm)
                .ToProperty(@this, nameof(Normal), out normal)
                .DisposeWith(disposables);

            @this.WhenAnyValue(z => z.Large, z => z.Normal, z => z.Small, z => z.ExtraLarge, z => z.ExtraSmall,
                    (lg, nm, sm, xl, xs) => (lg, nm, sm, xl, xs))
                .Scan(new CssClassList(), (acc, value) => acc.Add("v-size--x-small", value.xs)
                    .Add("v-size--small", value.sm)
                    .Add("v-size--default", value.nm)
                    .Add("v-size--large", value.lg)
                    .Add("v-size--x-large", value.xl))
                .ToProperty(@this, nameof(SizeableClasses), out sizeableClasses);

            return disposables;
        }
        
        bool Large { get; }
        bool Small { get; }
        bool ExtraSmall { get; }
        bool ExtraLarge { get; }
        bool Normal { get; }
        CssClassList SizeableClasses { get; }
    }

    public class VIcon : ThemeableComponent, ISizeable
    {
        private bool _disabled;
        private bool _dense;
        private bool _value;
        private bool _right;
        private bool _left;
        private string _tag = "i";
        private RenderFragment _childContent;
        private string _icon;
        private bool _large;
        private ObservableAsPropertyHelper<bool> _normal;
        private ObservableAsPropertyHelper<CssClassList> _sizeableClasses;
        private bool _small;
        private bool _extraLarge;
        private bool _extraSmall;

        public VIcon()
        {
            Color = "primary";
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ISizeable.Configure(this, out _normal, out _sizeableClasses).DisposeWith(_disposables);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            var sequence = 0;
            builder.OpenElementNode(ref sequence, Tag, node => node
                .AddClasses("v-icon", "notranslate")
                .AddClass("v-icon--disabled", Disabled)
                .AddClass("v-icon--left", Left)
                // .AddClass("v-icon--link", hasClickListener)
                .AddClass("v-icon--right", Right)
                .AddClass("v-icon--dense", Dense)
                .AddClass(Icon ?? string.Empty)
                // .AddAttribute("aria-hidden",  !hasClickListener)
                // .AddAttribute("role",  hasClickListener ? 'button' : null)
                // .AddAttribute("tabindex",  hasClickListener ? 0 : undefined)
                .Add(this)
            );
            sequence++;
            if (ChildContent != null)
            {
                builder.AddContent(sequence, ChildContent);
            }
            builder.CloseElement();
        }

        [Parameter]
        public bool Large
        {
            get => _large;
            set => this.RaiseAndSetIfChanged(ref _large, value);
        }

        [Parameter]
        public bool Small
        {
            get => _small;
            set => this.RaiseAndSetIfChanged(ref _small, value);
        }

        public bool Normal => _normal.Value;

        [Parameter]
        public bool ExtraLarge
        {
            get => _extraLarge;
            set => this.RaiseAndSetIfChanged(ref _extraLarge, value);
        }

        [Parameter]
        public bool ExtraSmall
        {
            get => _extraSmall;
            set => this.RaiseAndSetIfChanged(ref _extraSmall, value);
        }

        protected CssClassList SizeableClasses => _sizeableClasses.Value;
        CssClassList ISizeable.SizeableClasses => SizeableClasses;

        [Parameter]
        public string Tag
        {
            get => _tag;
            set => this.RaiseAndSetIfChanged(ref _tag, value);
        }

        [Parameter]
        public bool Dense
        {
            get => _dense;
            set => this.RaiseAndSetIfChanged(ref _dense, value);
        }

        [Parameter]
        public bool Disabled
        {
            get => _disabled;
            set => this.RaiseAndSetIfChanged(ref _disabled, value);
        }

        [Parameter]
        public bool Left
        {
            get => _left;
            set => this.RaiseAndSetIfChanged(ref _left, value);
        }

        [Parameter]
        public bool Right
        {
            get => _right;
            set => this.RaiseAndSetIfChanged(ref _right, value);
        }

        [Parameter]
        public bool Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        [Parameter]
        public string Icon
        {
            get => _icon;
            set => this.RaiseAndSetIfChanged(ref _icon, value);
        }

        [Parameter]
        public RenderFragment ChildContent
        {
            get => _childContent;
            set => this.RaiseAndSetIfChanged(ref _childContent, value);
        }
    }
}