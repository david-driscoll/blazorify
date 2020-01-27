using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ReactiveUI;

namespace RazorClassLibrary1
{
    public abstract class ReactiveObjectInput<TValue> : ReactiveObjectComponent, IInputComponent
    {
        private readonly EventHandler<ValidationStateChangedEventArgs> _validationStateChangedHandler;
        private Type _nullableUnderlyingType;
        private TValue _value;

        [CascadingParameter]
        private EditContext CascadedEditContext { get; set; }

        /// <summary>
        /// Gets or sets a collection of additional attributes that will be applied to the created element.
        /// </summary>
        [Parameter(CaptureUnmatchedValues = true)] 
        public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; }

        /// <summary>
        /// Gets or sets the value of the input. This should be used with two-way binding.
        /// </summary>
        /// <example>
        /// @bind-Value="model.PropertyName"
        /// </example>
        [Parameter]
        public TValue Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        /// <summary>
        /// Gets or sets a callback that updates the bound value.
        /// </summary>
        [Parameter] public EventCallback<TValue> ValueChanged { get; set; }

        /// <summary>
        /// Gets or sets an expression that identifies the bound value.
        /// </summary>
        [Parameter] public Expression<Func<TValue>> ValueExpression { get; set; }

        /// <summary>
        /// Gets the associated <see cref="Microsoft.AspNetCore.Components.Forms.EditContext"/>.
        /// </summary>
        protected EditContext EditContext { get; set; }

        /// <summary>
        /// Gets the <see cref="FieldIdentifier"/> for the bound value.
        /// </summary>
        protected FieldIdentifier FieldIdentifier { get; set; }

        #region IInputComponent
        EditContext? IInputComponent.EditContext => EditContext;
        FieldIdentifier IInputComponent.FieldIdentifier => FieldIdentifier;
        #endregion

        /// <summary>
        /// Constructs an instance of <see cref="InputBase{TValue}"/>.
        /// </summary>
        protected ReactiveObjectInput()
        {
            _validationStateChangedHandler = (sender, eventArgs) => StateHasChanged();
        }

        /// <summary>
        /// Gets a string that indicates the status of the field being edited. This will include
        /// some combination of "modified", "valid", or "invalid", depending on the status of the field.
        /// </summary>
        private string FieldClass
            => EditContext?.FieldCssClass(FieldIdentifier);

        /// <summary>
        /// Gets a CSS class string that combines the <c>class</c> attribute and <see cref="FieldClass"/>
        /// properties. Derived components should typically use this value for the primary HTML element's
        /// 'class' attribute.
        /// </summary>
        protected string CssClass
        {
            get
            {
                if (AdditionalAttributes != null &&
                    AdditionalAttributes.TryGetValue("class", out var @class) &&
                    !string.IsNullOrEmpty(Convert.ToString(@class)))
                {
                    return $"{@class} {FieldClass}";
                }

                return FieldClass; // Never null or empty
            }
        }


        /// <inheritdoc />
        public override Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            if (EditContext == null && CascadedEditContext != null)
            {
                // This is the first run
                // Could put this logic in OnInit, but its nice to avoid forcing people who override OnInit to call base.OnInit()

                //if (CascadedEditContext == null)
                //{
                //    throw new InvalidOperationException($"{GetType()} requires a cascading parameter " +
                //        $"of type {nameof(EditContext)}. For example, you can use {GetType().FullName} inside " +
                //        $"an {nameof(EditForm)}.");
                //}

                if (ValueExpression == null)
                {
                    throw new InvalidOperationException($"{GetType()} requires a value for the 'ValueExpression' " +
                                                        $"parameter. Normally this is provided automatically when using 'bind-Value'.");
                }

                EditContext = CascadedEditContext;
                FieldIdentifier = FieldIdentifier.Create(ValueExpression);
                _nullableUnderlyingType = Nullable.GetUnderlyingType(typeof(TValue));

                EditContext.OnValidationStateChanged += _validationStateChangedHandler;
            }
            else if (CascadedEditContext != null && CascadedEditContext != EditContext)
            {
                // Not the first run

                // We don't support changing EditContext because it's messy to be clearing up state and event
                // handlers for the previous one, and there's no strong use case. If a strong use case
                // emerges, we can consider changing this.
                throw new InvalidOperationException($"{GetType()} does not support changing the " +
                                                    $"{nameof(EditContext)} dynamically.");
            }

            // For derived components, retain the usual lifecycle with OnInit/OnParametersSet/etc.
            return base.SetParametersAsync(ParameterView.Empty);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            this.WhenAnyValue(x => x.Value)
                .Subscribe(value =>
                {
                    _ = ValueChanged.InvokeAsync(value);
                    EditContext?.NotifyFieldChanged(FieldIdentifier);
                })
                .DisposeWith(_disposables);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (EditContext != null)
            {
                EditContext.OnValidationStateChanged -= _validationStateChangedHandler;
            }
        }
    }
}