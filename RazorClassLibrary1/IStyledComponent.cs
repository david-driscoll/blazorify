namespace RazorClassLibrary1
{
    public interface IStyledComponent
    {
        AttributeList Attributes { get; }
        StyleList Style { get; }
        CssClassList Class { get; }
    }
}