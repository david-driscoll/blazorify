namespace RazorClassLibrary1
{
    public struct BooleanOrString
    {
        public BooleanOrString(string value)
        {
            String = value;
            Bool = default;
        }
        public BooleanOrString(bool value)
        {
            String = default;
            Bool = value;
        }

        public bool? Bool { get; set; }
        public string String { get; set; }

        public bool IsBool => Bool.HasValue;
        public bool IsString => !Bool.HasValue;

        public static implicit operator BooleanOrString(string value) => new BooleanOrString(value);
        public static implicit operator BooleanOrString(bool value) => new BooleanOrString(value);
    }
}