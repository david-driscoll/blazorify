using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace RazorClassLibrary1
{
    public class StyleList : IReadOnlyDictionary<string, string>
    {
        public static StyleList Create(IEnumerable<KeyValuePair<string, string>> values) => new StyleList().Add(values);
        private IImmutableDictionary<string, string> _data = ImmutableDictionary<string, string>.Empty;
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count { get; }
        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return _data.TryGetValue(key, out value);
        }

        public string this[string key] => _data[key];

        public IEnumerable<string> Keys => _data.Keys;
        IEnumerable<string> IReadOnlyDictionary<string, string>.Values => _data.Values;

        public StyleList Clear()
        {
            _data = ImmutableDictionary<string, string>.Empty;
            return this;
        }

        public StyleList Add(IEnumerable<KeyValuePair<string, string>> values)
        {
            _data = _data.SetItems(values);
            return this;
        }

        public StyleList Add(string key, string value)
        {
            _data = _data.SetItem(key, value);
            return this;
        }

        public StyleList Remove(IEnumerable<string> keys)
        {
            _data = _data.RemoveRange(keys);
            return this;
        }

        public StyleList Remove(params string[] keys)
        {
            _data = _data.RemoveRange(keys);
            return this;
        }

        public static explicit operator string(StyleList styleList)
        {
            var sb = new StringBuilder();
            foreach (var item in styleList._data.Where(z => z.Value != null))
            {
                sb.Append(item.Key);
                sb.Append(":");
                sb.Append(item.Value);
                sb.Append("; ");
            }

            return sb.ToString();
        }
    }
}