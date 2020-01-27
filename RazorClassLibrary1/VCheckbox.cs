using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Net.Http.Headers;
using RazorClassLibrary1.Annotations;
using ReactiveUI;
using ReactiveUI.Blazor;
using IComponent = Microsoft.AspNetCore.Components.IComponent;

namespace RazorClassLibrary1
{
    //public interface ISelectable
    //{
    //    bool IsActive { get; }
    //    bool IsDirty { get; }
    //    bool HasColor { get; }
    //}

    //public abstract class VListSelectable<TCollection, TValue> : VInput<TCollection>, IRippleable, ISelectable<TValue>
    //    where TCollection : ICollection<TValue>, new()
    //    where TValue : IEquatable<TValue>
    //{
    //    private TValue _inputValue;
    //    private ObservableAsPropertyHelper<bool> _isActive;
    //    private ObservableAsPropertyHelper<bool> _isDirty;
    //    private ObservableAsPropertyHelper<bool> _hasColor;

    //    protected override void OnInitialized()
    //    {
    //        base.OnInitialized();

    //        this.WhenAnyValue(z => z.Value, z => z.Model, (value, model) =>
    //            {
    //                if (value == null || model == null) return false;
    //                return value.Contains(model);
    //            })
    //            .ToProperty(this, nameof(IsActive), out _isActive)
    //            .DisposeWith(_disposables);

    //        this.WhenAnyValue(z => z.IsActive, z => !z)
    //            .ToProperty(this, nameof(IsDirty), out _isDirty)
    //            .DisposeWith(_disposables);

    //        this.WhenAnyValue(z => z.Value)
    //            .Select(z => z != null)
    //            .ToProperty(this, nameof(HasColor), out _hasColor)
    //            .DisposeWith(_disposables);
    //    }

    //    protected override void OnGenerateLabel(int sequence, RenderTreeBuilder builder)
    //    {
    //        builder.AddEventPreventDefaultAttribute(sequence, "click", true);
    //        builder.AddAttribute(sequence, "onclick", EventCallback.Factory.Create(this, OnChange));
    //    }

    //    [Parameter]
    //    public TValue Model
    //    {
    //        get => _inputValue;
    //        set => this.RaiseAndSetIfChanged(ref _inputValue, value);
    //    }

    //    protected bool IsActive => _isActive.Value;
    //    protected override bool IsDirty => _isDirty.Value;
    //    protected bool HasColor => _hasColor.Value;
    //    TValue ISelectable<TValue>.Model
    //    {
    //        get => Model;
    //        set => Model = value;
    //    }

    //    bool ISelectable<TValue>.IsActive => IsActive;
    //    bool ISelectable<TValue>.IsDirty => IsDirty;
    //    bool ISelectable<TValue>.HasColor => HasColor;

    //    protected void GenerateInput(ref int sequence, RenderTreeBuilder builder, string type, AttributeList attributeList)
    //    {
    //        builder.OpenElementNode(ref sequence, type, node => node
    //            .AddAttributes(attributeList)
    //            .AddAttribute("aria-checked", IsActive ? "true" : "false")
    //            .AddAttribute("disabled", IsDisabled)
    //            .AddAttribute("id", ComputedId)
    //            .AddAttribute("role", type)
    //            .AddAttribute("type", type)
    //            //.AddAttribute("value", Value)
    //            .AddAttribute("checked", IsActive)
    //            .AddHandler("blur", EventCallback.Factory.Create(this, OnBlur))
    //            .AddHandler("change", EventCallback.Factory.Create(this, OnChange))
    //            .AddHandler("focus", EventCallback.Factory.Create(this, OnFocus))
    //            .AddHandler("keydown", EventCallback.Factory.Create(this, OnKeydown))
    //        );
    //        builder.CloseElement();
    //    }

    //    protected void OnChange()
    //    {
    //        if (IsDisabled) return;

    //        var data = Value.ToImmutableArray();
    //        var length = data.Length;
    //        data = data.RemoveAll(value => !value.Equals(Model));

    //            if (data.Length == length)
    //            {
    //                data = data.Add(Model);
    //            }

    //            var @new = new TCollection();
    //            foreach (var item in data)
    //            @new.Add(item);
    //            Value = @new;
    //    }

    //    protected void OnBlur()
    //    {
    //        IsFocused = false;
    //    }

    //    protected void OnFocus()
    //    {
    //        IsFocused = true;
    //    }

    //    protected void OnKeydown() { }
    //}
    public abstract class VSelectable<TValue> : VInput<TValue>, IRippleable
    {
        protected override void OnGenerateLabel(int sequence, RenderTreeBuilder builder)
        {
            builder.AddEventPreventDefaultAttribute(sequence, "onclick", true);
            builder.AddAttribute(sequence, "onclick", EventCallback.Factory.Create(this, OnChange));
        }

        protected void GenerateInput(ref int sequence, RenderTreeBuilder builder, string type,
            AttributeList attributeList, RenderFragmentWithSequence fragment)
        {
            builder.OpenElementNode(ref sequence, "input", node => node
                .AddAttributes(attributeList)
                .AddAttribute("disabled", IsDisabled)
                .AddAttribute("id", ComputedId)
                .AddAttribute("role", type)
                .AddAttribute("type", type)
                //.AddAttribute("value", Value)
                .AddHandler("blur", EventCallback.Factory.Create(this, OnBlur))
                .AddHandler("change", EventCallback.Factory.Create(this, OnChange))
                .AddHandler("focus", EventCallback.Factory.Create(this, OnFocus))
                .AddHandler("keydown", EventCallback.Factory.Create(this, OnKeydown))
            );
            fragment(sequence, builder);
            builder.CloseElement();
        }

        protected abstract void OnChange();

        protected void OnBlur()
        {
            IsFocused = false;
        }

        protected void OnFocus()
        {
            IsFocused = true;
        }

        protected void OnKeydown()
        {
        }
    }

    public class VCheckbox<TValue> : VSelectable<TValue>
    {
        private TValue _inputValue;
        private ObservableAsPropertyHelper<bool> _isActive;
        private ObservableAsPropertyHelper<bool> _isDirty;
        private ObservableAsPropertyHelper<bool> _hasColor;
        private ObservableAsPropertyHelper<MarkupString> _computedIcon;
        private MarkupString? _onIcon;
        private MarkupString? _offIcon;
        private MarkupString? _indeterminateIcon;
        private bool _indeterminate;
        private bool _inputIndeterminate;
        private bool _isBoolean;

        public VCheckbox()
        {
            _isBoolean = (Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue)) == typeof(bool);
        }

        protected override void OnInitialized()
        {
            this.WhenAnyValue(z => z.Value)
                .Select(z => z != null)
                .ToProperty(this, nameof(HasColor), out _hasColor)
                .DisposeWith(_disposables);

            this.WhenAnyValue(z => z.Value, z => z.Checked, (value, model) =>
                {
                    if (value == null || model == null) return false;
                    return value.Equals(model);
                })
                .ToProperty(this, nameof(IsActive), out _isActive)
                .DisposeWith(_disposables);

            this.WhenAnyValue(z => z.IsActive)
                .Select(z => !z)
                .ToProperty(this, nameof(IsDirty), out _isDirty)
                .DisposeWith(_disposables);

            this.WhenAnyValue(x => x.InputIndeterminate, z => z.IsActive, z => z.OnIcon, z => z.OffIcon,
                    z => z.IndeterminateIcon, (inputIndeterminate, isActive, onIcon, offIcon, indeterminateIcon) =>
                    {
                        if (inputIndeterminate)
                        {
                            return indeterminateIcon ?? Blazorify.Icons.IndeterminateIcon;
                        }

                        if (isActive)
                        {
                            return onIcon ?? Blazorify.Icons.OnIcon;
                        }

                        return offIcon ?? Blazorify.Icons.OffIcon;
                    })
                .ToProperty(this, nameof(ComputedIcon), out _computedIcon)
                .DisposeWith(_disposables);

            base.OnInitialized();
            /*
    computedIcon (): string {
      if (this.inputIndeterminate) {
        return this.indeterminateIcon
      } else if (this.isActive) {
        return this.onIcon
      } else {
        return this.offIcon
      }
    },
    // Do not return undefined if disabled,
    // according to spec, should still show
    // a color when disabled and active
    validationState (): string | undefined {
      if (this.disabled && !this.inputIndeterminate) return undefined
      if (this.hasError && this.shouldValidate) return 'error'
      if (this.hasSuccess) return 'success'
      if (this.hasColor !== null) return this.computedColor
      return undefined
    },
             */
        }

        protected override CssClassList CalculateInputClasses(CssClassList classList)
        {
            return base.CalculateInputClasses(classList)
                .Add("v-input--selection-controls", true)
                .Add("v-input--checkbox", true)
                .Add("v-input--indeterminate", InputIndeterminate);
        }

        protected override string? CalculateValidationState()
        {
            if (Disabled && !InputIndeterminate) return default;
            if (HasError && ShouldValidate) return "error";
            if (HasSuccess) return "success";
            if (HasColor) return ComputedColor;
            return default;
        }

        private bool InputIndeterminate
        {
            get => _inputIndeterminate;
            set => this.RaiseAndSetIfChanged(ref _inputIndeterminate, value);
        }

        private MarkupString ComputedIcon => _computedIcon.Value;
        protected bool IsActive => _isActive.Value;
        protected override bool IsDirty => _isDirty.Value;
        protected bool HasColor => _hasColor.Value;

        [Parameter]
        public TValue Checked
        {
            get => _inputValue;
            set => this.RaiseAndSetIfChanged(ref _inputValue, value);
        }

        [Parameter]
        public bool Indeterminate
        {
            get => _indeterminate;
            set => this.RaiseAndSetIfChanged(ref _indeterminate, value);
        }

        [Parameter]
        public MarkupString? IndeterminateIcon
        {
            get => _indeterminateIcon;
            set => this.RaiseAndSetIfChanged(ref _indeterminateIcon, value);
        }

        [Parameter]
        public MarkupString? OffIcon
        {
            get => _offIcon;
            set => this.RaiseAndSetIfChanged(ref _offIcon, value);
        }

        [Parameter]
        public MarkupString? OnIcon
        {
            get => _onIcon;
            set => this.RaiseAndSetIfChanged(ref _onIcon, value);
        }

        protected override void OnChange()
        {
            if (_isBoolean)
            {
                Value = Value?.Equals(true) == true ? default : (TValue) (object) true;
            }
            else
            {
                Value = Value?.Equals(Checked) == true ? default : Checked!;
            }
        }

        protected override void GenerateDefaultFragment(ref int sequence, RenderTreeBuilder builder)
        {
            builder.OpenElementNode(ref sequence, "div", node => node
                .AddClass("v-input--selection-controls__input")
            );

            GenerateInput(ref sequence, builder, "checkbox",
                new AttributeList()
                    .Add("aria-checked", InputIndeterminate ? "mixed" : IsActive ? "true" : "false")
                    .Add("checked", IsActive),
                (seq, builder) => { }
            );
            // ripple
            builder.OpenComponentNode<VIcon>(ref sequence, node => node
                .AddAttribute(nameof(VIcon.Dense), Dense)
                .AddAttribute(nameof(VIcon.Dark), Dark)
                .AddAttribute(nameof(VIcon.Light), Light)
                .AddAttribute(nameof(VIcon.Icon), ComputedIcon.ToString())
            );
            builder.CloseComponent();
            /*
                */

            builder.CloseElement();
            GenerateLabel(ref sequence, builder);
        }
    }

    public interface IRippleable
    {
        // TODO: Requires js interop to configure and setup the components
    }
}