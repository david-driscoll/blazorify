using ReactiveUI;

namespace RazorClassLibrary1
{
    public class CascadingTheme : ReactiveObject
    {
        private bool _isDark;

        public bool IsDark
        {
            get => _isDark;
            set => this.RaiseAndSetIfChanged(ref _isDark, value);
        }
    }
}