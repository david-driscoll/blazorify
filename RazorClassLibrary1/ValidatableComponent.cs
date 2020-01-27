using System.Collections.Generic;
using System.Reactive.Disposables;
using Microsoft.AspNetCore.Components;
using ReactiveUI;

namespace RazorClassLibrary1
{
    public abstract class ValidatableComponent<T> : ThemeableInputComponent<T>, IValidatable
    {

        protected override void OnInitialized()
        {
            base.OnInitialized();
            IValidatable.Configure(this, out _computedColor, out _hasError, out _hasSuccess, out _hasState,
                out _shouldValidate, out _validationState, out _validationMessages, out _hasMessages).DisposeWith(_disposables);
        }
        #region IValidatable
        private bool _disabled;
        private bool _error;
        private IEnumerable<string> _errorMessages;
        private bool _readOnly;
        private IEnumerable<string> _messages;
        private bool _success;
        private IEnumerable<string> _successMessages;
        private bool _validateOnBlur;
        private bool _hasColor;
        private bool _hasFocused;
        private bool _hasInput;
        private bool _isFocused;
        private bool _isResetting;
        private bool _valid;
        private ObservableAsPropertyHelper<string?> _computedColor;
        private ObservableAsPropertyHelper<bool> _hasError;
        private ObservableAsPropertyHelper<bool> _hasSuccess;
        private ObservableAsPropertyHelper<bool> _hasState;
        private ObservableAsPropertyHelper<bool> _hasMessages;

        private ObservableAsPropertyHelper<bool> _shouldValidate;
        private ObservableAsPropertyHelper<string?> _validationState;
        private ObservableAsPropertyHelper<IEnumerable<string>> _validationMessages;

        [Parameter]
        public bool Disabled
        {
            get => _disabled;
            set => this.RaiseAndSetIfChanged(ref _disabled, value);
        }

        [Parameter]
        public bool Error
        {
            get => _error;
            set => this.RaiseAndSetIfChanged(ref _error, value);
        }

        [Parameter]
        public IEnumerable<string> ErrorMessages
        {
            get => _errorMessages;
            set => this.RaiseAndSetIfChanged(ref _errorMessages, value);
        }

        [Parameter]
        public bool ReadOnly
        {
            get => _readOnly;
            set => this.RaiseAndSetIfChanged(ref _readOnly, value);
        }

        [Parameter]
        public IEnumerable<string> Messages
        {
            get => _messages;
            set => this.RaiseAndSetIfChanged(ref _messages, value);
        }

        [Parameter]
        public bool Success
        {
            get => _success;
            set => this.RaiseAndSetIfChanged(ref _success, value);
        }

        [Parameter]
        public IEnumerable<string> SuccessMessages
        {
            get => _successMessages;
            set => this.RaiseAndSetIfChanged(ref _successMessages, value);
        }

        [Parameter]
        public bool ValidateOnBlur
        {
            get => _validateOnBlur;
            set => this.RaiseAndSetIfChanged(ref _validateOnBlur, value);
        }

        public bool HasColor
        {
            get => _hasColor;
            set => this.RaiseAndSetIfChanged(ref _hasColor, value);
        }

        public bool HasFocused
        {
            get => _hasFocused;
            set => this.RaiseAndSetIfChanged(ref _hasFocused, value);
        }

        public bool HasInput
        {
            get => _hasInput;
            set => this.RaiseAndSetIfChanged(ref _hasInput, value);
        }

        public bool IsFocused
        {
            get => _isFocused;
            set => this.RaiseAndSetIfChanged(ref _isFocused, value);
        }

        public bool IsResetting
        {
            get => _isResetting;
            set => this.RaiseAndSetIfChanged(ref _isResetting, value);
        }

        public bool Valid
        {
            get => _valid;
            set => this.RaiseAndSetIfChanged(ref _valid, value);
        }

        public string? ComputedColor => _computedColor.Value;
        public bool HasError => _hasError.Value;
        public bool HasSuccess => _hasSuccess.Value;
        public bool HasState => _hasState.Value;
        public bool HasMessages => _hasMessages.Value;
        public bool ShouldValidate => _shouldValidate.Value;
        public string? ValidationState => _validationState.Value;
        public IEnumerable<string> ValidationMessages => _validationMessages.Value;

        protected virtual string? CalculateValidationState()
        {
            if (Disabled) return null;
            if (HasError) return "error";
            if (HasSuccess) return "success";
            if (HasColor) return ComputedColor;
            return null;
        }

        string IValidatable.CalculateValidationState() => CalculateValidationState();

        #endregion
    }
}