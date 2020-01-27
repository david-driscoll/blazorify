namespace RazorClassLibrary1
{
    public struct CssValue
    {
        public CssValue(string value)
        {
            Unit = CssUnit.Undefined;
            StringValue = value;
            NumberValue = default;
        }
        public CssValue(int value, CssUnit unit = CssUnit.px)
        {
            Unit = unit;
            StringValue = default;
            NumberValue = value;
        }
        public readonly CssUnit Unit { get; }
        public readonly string StringValue { get; }
        public readonly int NumberValue { get; }

        public static implicit operator CssValue(int value) => new CssValue(value);
        public static implicit operator CssValue(string value) => new CssValue(value);

        public static implicit operator string(CssValue value) => value.StringValue ?? (value.Unit == CssUnit.Undefined ? null : $"{value.NumberValue}{value.Unit:G}");
    }
}