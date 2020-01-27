namespace RazorClassLibrary1
{
    public interface ICascadingValue<T> : ICascadingProvider
    {
        T CascadingValue { get; }
    }
}