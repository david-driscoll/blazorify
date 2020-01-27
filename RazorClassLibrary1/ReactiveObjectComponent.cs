using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Binding;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using ReactiveUI;

namespace RazorClassLibrary1
{

    public delegate void RenderFragmentWithSequence(int sequence, RenderTreeBuilder builder);
    public abstract class ReactiveObjectComponent : ComponentBase, IReactiveObject, ICascadingProvider, IDisposable
    {
        protected readonly CompositeDisposable _disposables = new CompositeDisposable();
        public event PropertyChangingEventHandler PropertyChanging;
        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) => PropertyChanging?.Invoke(this, args);
        public event PropertyChangedEventHandler PropertyChanged;
        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);

        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.WhenAnyPropertyChanged().Select(x => Observable.FromAsync(() => InvokeAsync(StateHasChanged))).Switch().Subscribe();
        }

        #region ICascadingProvider
        protected void BuildCascadingValues(int sequence, RenderTreeBuilder builder, RenderFragmentWithSequence renderFragment)
        {
            BuildCascadingValues(sequence, _cascadingValues, builder, renderFragment);
        }
        private void BuildCascadingValues(int sequence, ImmutableArray<CascadingItem> cascadingItems, RenderTreeBuilder builder, RenderFragmentWithSequence renderFragment)
        {
            if (cascadingItems.Length > 0)
            {
                var value = cascadingItems[0];
                builder.OpenComponent(sequence++, value.ComponentType);
                builder.AddAttribute(sequence++, "Value", value.Getter());
                builder.AddAttribute(sequence++, "ChildContent", (RenderFragment)((innerBuilder) => BuildCascadingValues(sequence, cascadingItems.RemoveAt(0), innerBuilder, renderFragment)));
                builder.CloseComponent();
                return;
            }

            renderFragment(sequence, builder);
        }

        private ImmutableArray<CascadingItem> _cascadingValues = ImmutableArray<CascadingItem>.Empty;
        ImmutableArray<CascadingItem> ICascadingProvider.CascadingValues
        {
            get => _cascadingValues;
            set => _cascadingValues = value;
        }
        #endregion

        protected virtual void Dispose(bool disposing)
        {
        }

        void IDisposable.Dispose()
        {
            _disposables.Dispose();
            Dispose(disposing: true);
        }
    }
}