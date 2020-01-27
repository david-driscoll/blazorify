using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Binding;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using ReactiveUI;

namespace RazorClassLibrary1
{
    public class VInput<TValue> : ValidatableComponent<TValue>
    {
        private Guid _uid = Guid.NewGuid();
        private RenderFragment? _prependOuterContent;
        private string? _prependOuter;
        //private RenderFragment? _prependContent;
        //private string? _prepend;
        private RenderFragment? _appendOuterContent;
        private string? _appendOuter;
        //private RenderFragment? _appendContent;
        //private string? _append;
        private RenderFragment? _labelContent;
        private string? _label;
        private RenderFragment? _childContent;
        private bool _dense;
        private bool _loading;
        private string? _id;
        private string? _hint;
        private CssValue _height;
        private bool _persistentHint;
        private HideDetails _hideDetails;
        private ObservableAsPropertyHelper<CssClassList> _classes;
        private ObservableAsPropertyHelper<bool> _isLabelActive;
        private ObservableAsPropertyHelper<bool> _isDisabled;
        private ObservableAsPropertyHelper<bool> _isDirty;
        private ObservableAsPropertyHelper<bool> _hasHint;
        private ObservableAsPropertyHelper<bool> _hasLabel;
        private ObservableAsPropertyHelper<string?> _computedId;
        private ObservableAsPropertyHelper<bool> _showDetails;
        private ObservableAsPropertyHelper<IEnumerable<string>> _messagesToDisplay;


        [Parameter]
        public RenderFragment? PrependOuterContent
        {
            get => _prependOuterContent;
            set => this.RaiseAndSetIfChanged(ref _prependOuterContent, value);
        }

        [Parameter]
        public string? PrependOuter
        {
            get => _prependOuter;
            set => this.RaiseAndSetIfChanged(ref _prependOuter, value);
        }

        //[Parameter]
        //public RenderFragment? PrependContent
        //{
        //    get => _prependContent;
        //    set => this.RaiseAndSetIfChanged(ref _prependContent, value);
        //}

        //[Parameter]
        //public string? Prepend
        //{
        //    get => _prepend;
        //    set => this.RaiseAndSetIfChanged(ref _prepend, value);
        //}

        [Parameter]
        public RenderFragment? AppendOuterContent
        {
            get => _appendOuterContent;
            set => this.RaiseAndSetIfChanged(ref _appendOuterContent, value);
        }

        [Parameter]
        public string? AppendOuter
        {
            get => _appendOuter;
            set => this.RaiseAndSetIfChanged(ref _appendOuter, value);
        }

        //[Parameter]
        //public RenderFragment? AppendContent
        //{
        //    get => _appendContent;
        //    set => this.RaiseAndSetIfChanged(ref _appendContent, value);
        //}

        //[Parameter]
        //public string? Append
        //{
        //    get => _append;
        //    set => this.RaiseAndSetIfChanged(ref _append, value);
        //}

        [Parameter]
        public RenderFragment? ChildContent
        {
            get => _childContent;
            set => this.RaiseAndSetIfChanged(ref _childContent, value);
        }

        [Parameter]
        public RenderFragment? InputContent
        {
            get => _childContent;
            set => this.RaiseAndSetIfChanged(ref _childContent, value);
        }

        [Parameter]
        public RenderFragment? LabelContent
        {
            get => _labelContent;
            set => this.RaiseAndSetIfChanged(ref _labelContent, value);
        }

        [Parameter]
        public string? Label
        {
            get => _label;
            set => this.RaiseAndSetIfChanged(ref _label, value);
        }

        [Parameter]
        public bool Dense
        {
            get => _dense;
            set => this.RaiseAndSetIfChanged(ref _dense, value);
        }

        [Parameter]
        public CssValue Height
        {
            get => _height;
            set => this.RaiseAndSetIfChanged(ref _height, value);
        }

        [Parameter]
        public bool Loading
        {
            get => _loading;
            set => this.RaiseAndSetIfChanged(ref _loading, value);
        }

        [Parameter]
        public string Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

        [Parameter]
        public string? Hint
        {
            get => _hint;
            set => this.RaiseAndSetIfChanged(ref _hint, value);
        }

        [Parameter]
        public bool PersistentHint
        {
            get => _persistentHint;
            set => this.RaiseAndSetIfChanged(ref _persistentHint, value);
        }

        [Parameter]
        public HideDetails HideDetails
        {
            get => _hideDetails;
            set => this.RaiseAndSetIfChanged(ref _hideDetails, value);
        }

        [Parameter]
        public EventCallback<MouseEventArgs>? OnClick { get; set; }
        [Parameter]
        public EventCallback<MouseEventArgs>? OnMouseDown { get; set; }
        [Parameter]
        public EventCallback<MouseEventArgs>? OnMouseUp { get; set; }

        protected virtual CssClassList Classes => _classes.Value;
        protected virtual bool IsLabelActive => _isLabelActive.Value;
        protected virtual bool IsDisabled => _isDisabled.Value;
        protected virtual bool IsDirty => _isDirty.Value;
        protected virtual bool HasHint => _hasHint.Value;
        protected virtual bool HasLabel => _hasLabel.Value;
        protected virtual string? ComputedId => _computedId.Value;
        protected virtual bool ShowDetails => _showDetails.Value;
        protected virtual IEnumerable<string> MessagesToDisplay => _messagesToDisplay.Value;

        protected override void OnInitialized()
        {
            base.OnInitialized();


            this.WhenAnyValue(z => z.Id)
                .StartWith((string?)null)
                .Select(z => z ?? $"input-{_uid:N}")
                .ToProperty(this, nameof(ComputedId), out _computedId)
                .DisposeWith(_disposables);

            this.WhenAnyValue(z => z.Label, z => z.LabelContent, (str, fragment) => str != null || fragment != null)
                .ToProperty(this, nameof(HasLabel), out _hasLabel)
                .DisposeWith(_disposables);

            this.WhenAnyValue(x => x.Value)
                .Skip(1)
                .Select(z => z != null)
                .ToProperty(this, nameof(IsDirty), out _isDirty)
                .DisposeWith(_disposables);

            this.WhenAnyValue(z => z.Disabled, z => z.ReadOnly, (disabled, readOnly) => disabled || readOnly)
                .ToProperty(this, nameof(IsDisabled), out _isDisabled)
                .DisposeWith(_disposables);

            this.WhenAnyValue(z => z.IsDirty)
                .ToProperty(this, nameof(IsLabelActive), out _isLabelActive)
                .DisposeWith(_disposables);

            this.WhenAnyValue(x => x.Hint, x => x.IsFocused, x => x.PersistentHint,
                    (hint, isFocused, persistentHint) => hint != null && (persistentHint || isFocused))
                .ToProperty(this, nameof(HasHint), out _hasHint)
                .DisposeWith(_disposables);

            this.WhenAnyValue(z => z.HasHint, z => z.Hint, z => z.HasMessages, z => z.Messages, z => z.ErrorMessages, z => z.SuccessMessages, z => z.ValidationMessages, (hasHint, hint, hasMessages, messages, errorMessages, successMessages, validationMessages) =>
                {
                    if (hasHint) return new[] { hint };
                    if (!hasMessages) return Enumerable.Empty<string>();
                    if (errorMessages?.Any() == true) return errorMessages;
                    if (validationMessages?.Any() == true) return validationMessages;
                    if (successMessages?.Any() == true) return successMessages;
                    if (messages?.Any() == true) return messages;
                    return Enumerable.Empty<string>();
                })
                .ToProperty(this, nameof(MessagesToDisplay), out _messagesToDisplay)
                .DisposeWith(_disposables);

            this.WhenAnyValue(z => z.HideDetails, z => z.MessagesToDisplay,
                    (hideDetails, messagesToDisplay) =>
                        hideDetails == HideDetails.No || (hideDetails == HideDetails.Auto && messagesToDisplay.Any()))
                .ToProperty(this, nameof(ShowDetails), out _showDetails)
                .DisposeWith(_disposables);

            this.WhenAnyPropertyChanged()
                .Scan(new CssClassList(), (acc, x) => CalculateInputClasses(acc))
                .ToProperty(this, nameof(Classes), out _classes)
                .DisposeWith(_disposables);
        }

        protected virtual CssClassList CalculateInputClasses(CssClassList classList)
        {
            return classList
                .Add("v-input--has-state", HasState)
                .Add("v-input--hide-details", !ShowDetails)
                .Add("v-input--is-label-active", IsLabelActive)
                .Add("v-input--is-dirty", IsDirty)
                .Add("v-input--is-disabled", Disabled)
                .Add("v-input--is-focused", IsFocused)
                .Add("v-input--is-loading", Loading)
                .Add("v-input--is-readonly", ReadOnly)
                .Add("v-input--dense", Dense)
                .Add(ThemeClasses);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            // Sequences should never change
            BuildCascadingValues(0, builder, RenderInputElement);
        }

        protected virtual void RenderInputElement(int sequence, RenderTreeBuilder builder)
        {
            builder.OpenElementNode(
                ref sequence, "div",
                node => node
                    .AddClass("v-input")
                    .AddClasses(Classes)
                    .SetTextColor(ValidationState)
                );


            GenerateContent(ref sequence, builder);

            builder.CloseElement();
        }

        protected virtual void GenerateContent(ref int sequence, RenderTreeBuilder builder)
        {
            GenerateSlotFragment(ref sequence, builder, nameof(PrependOuter), PrependOuterContent, PrependOuter);

            GenerateControl(ref sequence, builder);

            GenerateSlotFragment(ref sequence, builder, nameof(AppendOuter), AppendOuterContent, AppendOuter);
        }

        protected virtual void GenerateControl(ref int sequence, RenderTreeBuilder builder)
        {
            builder.OpenElementNode(ref sequence, "div", node => node.AddClass("v-input__control"));

            //GenerateSlotFragment(ref sequence, builder, nameof(Prepend), PrependContent, Prepend);

            GenerateInputFragment(ref sequence, builder);
            GenerateMessages(ref sequence, builder);

            //GenerateSlotFragment(ref sequence, builder, nameof(Append), AppendContent, Append);

            builder.CloseElement();
        }

        protected virtual void GenerateInputFragment(ref int sequence, RenderTreeBuilder builder)
        {
            builder.OpenElementNode(ref sequence, "div", node => node.AddClass("v-input__slot").AddStyle("height", Height));

            sequence++;
            if (OnClick != null)
            {
                builder.AddAttribute(sequence, "onclick", OnClick);
            }

            sequence++;
            if (OnMouseDown != null)
            {
                builder.AddAttribute(sequence, "onmousedown", OnMouseDown);
            }

            sequence++;
            if (OnMouseUp != null)
            {
                builder.AddAttribute(sequence, "onmouseup", OnMouseUp);
            }

            GenerateDefaultFragment(ref sequence, builder);

            builder.CloseElement();
        }

        protected virtual void GenerateMessages(ref int sequence, RenderTreeBuilder builder)
        {
            if (!this.ShowDetails)
            {
                sequence += 5;
                return;
            }

            builder.OpenComponent<VMessages>(sequence++);
            builder.AddAttribute(sequence++, "Color", HasHint ? "" : ValidationState);
            builder.AddAttribute(sequence++, "Dark", Dark);
            builder.AddAttribute(sequence++, "Light", Light);
            builder.AddAttribute(sequence++, "Messages", MessagesToDisplay);
            //builder.AddAttribute(sequence++, "role", HasMessages ? "alert" : "");
            builder.CloseComponent();
        }

        protected virtual void GenerateDefaultFragment(ref int sequence, RenderTreeBuilder builder)
        {
            GenerateLabel(ref sequence, builder);
            builder.AddContent(sequence++, ChildContent);
        }

        protected virtual void GenerateLabel(ref int sequence, RenderTreeBuilder builder)
        {
            if (!HasLabel)
            {
                sequence += 9;
                return;
            }

            builder.OpenComponent<VLabel>(sequence++);
            builder.AddAttribute(sequence++, "Color", ValidationState);
            builder.AddAttribute(sequence++, "Dark", Dark);
            builder.AddAttribute(sequence++, "Light", Light);
            builder.AddAttribute(sequence++, "For", ComputedId);
            builder.AddAttribute(sequence++, "Focused", HasState);
            builder.AddAttribute(sequence++, "Disabled", Disabled);
            if (LabelContent != null)
            {
                builder.AddAttribute(sequence++, "ChildContent", LabelContent);
            }
            else
            {
                var seq = sequence;
                builder.AddAttribute(sequence++, "ChildContent", (RenderFragment)(_ => _.AddMarkupContent(seq, Label)));
            }

            OnGenerateLabel(sequence++, builder);
            builder.CloseComponent();
        }

        protected virtual void OnGenerateLabel(int sequence, RenderTreeBuilder builder)
        {

        }

        protected virtual void GenerateSlotFragment(ref int sequence, RenderTreeBuilder builder, string name, RenderFragment? renderFragment, string? content = null)
        {
            if (renderFragment == null && content == null)
            {
                sequence += StyledNode.SequenceCount + 2;
                return;
            }

            var type = name.StartsWith("prepend", StringComparison.OrdinalIgnoreCase) ? "prepend" : "append";
            var location = name.EndsWith("outer", StringComparison.OrdinalIgnoreCase) ? "outer" : "inner";
            builder.OpenElementNode(ref sequence, "div", node => node.AddClass($"v-input__{type}-{location}"));
            if (renderFragment != null)
            {
                builder.AddContent(sequence++, renderFragment);
            }
            else
            {
                builder.AddContent(sequence++, content);
            }

            builder.CloseElement();
        }
    }
}