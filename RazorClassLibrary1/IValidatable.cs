using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Binding;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ReactiveUI;

namespace RazorClassLibrary1
{
    public interface IValidatable : IColorable, IThemeable, IInputComponent
    {
        public static IDisposable Configure(
            IValidatable @this,
            out ObservableAsPropertyHelper<string?> computedColor,
            out ObservableAsPropertyHelper<bool> hasError,
            out ObservableAsPropertyHelper<bool> hasSuccess,
            out ObservableAsPropertyHelper<bool> hasState,
            out ObservableAsPropertyHelper<bool> shouldValidate,
            out ObservableAsPropertyHelper<string?> validationState,
            out ObservableAsPropertyHelper<IEnumerable<string>> validationMessages,
            out ObservableAsPropertyHelper<bool> hasMessages)
        {
            var disposable = new CompositeDisposable();
            @this.WhenAnyValue(z => z.Disabled, z => z.Color, z => z.IsDark, z => z.AppIsDark,
                    (disabled, color, isDark, appIsDark) =>
                    {
                        if (disabled) return default;
                        if (string.IsNullOrWhiteSpace(color)) return color;
                        // It's assumed that if the input is on a
                        // dark background, the user will want to
                        // have a white color. If the entire app
                        // is setup to be dark, then they will
                        // like want to use their primary color
                        if (isDark && !appIsDark) return "white";
                        else return "primary";
                    })
                .ToProperty(@this, nameof(IValidatable.ComputedColor), out computedColor)
                .DisposeWith(disposable);


            Observable.FromEventPattern<ValidationStateChangedEventArgs>(
                    handler =>
                    {
                        if (@this.EditContext == null) return;
                        @this.EditContext.OnValidationStateChanged += handler;
                    },
                    handler =>
                    {
                        if (@this.EditContext == null) return;
                        @this.EditContext.OnValidationStateChanged -= handler;
                    })
                .Select(z => @this.EditContext?.GetValidationMessages(@this.FieldIdentifier) ?? Enumerable.Empty<string>())
                .StartWith(Enumerable.Empty<string>())
                .ToProperty(@this, nameof(IValidatable.ValidationMessages), out validationMessages)
                .DisposeWith(disposable);

            @this.WhenAnyValue(z => z.ErrorMessages, z => z.Error, z => z.ValidationMessages, (messages, error, validationMessages) => error || messages?.Any() == true || validationMessages?.Any() == true)
                .ToProperty(@this, nameof(IValidatable.HasError), out hasError)
                .DisposeWith(disposable);

            @this.WhenAnyValue(z => z.SuccessMessages, z => z.Success, (messages, success) => success || messages?.Any() == true)
                .ToProperty(@this, nameof(IValidatable.HasSuccess), out hasSuccess)
                .DisposeWith(disposable);

            @this.WhenAnyValue(z => z.Error, z => z.IsResetting, z => z.ValidateOnBlur, z => z.HasFocused,
                    z => z.IsFocused, z => z.HasInput,
                    (error, isResetting, validateOnBlur, hasFocused, isFocused, hasInput) =>
                    {
                        if (error) return true;
                        if (isResetting) return false;
                        if (validateOnBlur) return hasFocused && !isFocused;
                        return hasInput || hasFocused;
                    })
                .ToProperty(@this, nameof(IValidatable.ShouldValidate), out shouldValidate)
                .DisposeWith(disposable);

            @this.WhenAnyValue(x => x.Disabled, z => z.HasSuccess, z => z.ShouldValidate, x => x.Error,
                    (disabled, hasSuccess, shouldValidate, error) =>
                        !disabled && (hasSuccess || (shouldValidate && error)))
                .ToProperty(@this, nameof(IValidatable.HasState), out hasState)
                .DisposeWith(disposable);

            @this.WhenAnyPropertyChanged()
                .Select(_ => @this.CalculateValidationState())
                .ToProperty(@this, nameof(IValidatable.ValidationState), out validationState)
                .DisposeWith(disposable);

            @this.WhenAnyValue(z => z.Messages, z => z.ErrorMessages, z => z.SuccessMessages, z => z.ValidationMessages,
                    (messages, errorMessages, successMessages, validationMessages) => messages?.Any() == true || errorMessages?.Any() == true ||
                                                                                      successMessages?.Any() == true ||
                                                                                      validationMessages?.Any() == true)
                .ToProperty(@this, nameof(IValidatable.HasMessages), out hasMessages)
                .DisposeWith(disposable);

            return disposable;
        }

        string? CalculateValidationState();

        [Parameter]
        bool Disabled { get; set; }

        [Parameter]
        bool Error { get; set; }

        [Parameter]
        IEnumerable<string> Messages { get; set; }

        [Parameter]
        IEnumerable<string> ErrorMessages { get; set; }

        [Parameter]
        bool ReadOnly { get; set; }

        [Parameter]
        bool Success { get; set; }

        [Parameter]
        IEnumerable<string> SuccessMessages { get; set; }

        [Parameter]
        bool ValidateOnBlur { get; set; }

        bool HasColor { get; set; }
        bool HasFocused { get; set; }
        bool HasInput { get; set; }
        bool IsFocused { get; set; }
        bool IsResetting { get; set; }
        bool Valid { get; set; }

        string? ComputedColor { get; }
        bool HasError { get; }
        bool HasSuccess { get; }
        bool HasMessages { get; }
        bool HasState { get; }
        bool ShouldValidate { get; }
        string? ValidationState { get; }
        IEnumerable<string> ValidationMessages { get; }
    }
}