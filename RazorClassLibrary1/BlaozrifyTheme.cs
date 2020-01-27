using ReactiveUI;

namespace RazorClassLibrary1
{
    public class BlazorifyTheme : ReactiveObject
    {
        private bool _dark;
        public bool Dark
        {
            get => _dark;
            set => this.RaiseAndSetIfChanged(ref _dark, value);
        }
    }
}