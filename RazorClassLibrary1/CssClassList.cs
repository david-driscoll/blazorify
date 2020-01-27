using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace RazorClassLibrary1
{
    public class CssClassList : IReadOnlyDictionary<string, bool>
    {
        public static CssClassList Create(IEnumerable<KeyValuePair<string, bool>> values) => new CssClassList().Add(values);

        private IImmutableDictionary<string, bool> _data = ImmutableDictionary<string, bool>.Empty;
        public IEnumerator<KeyValuePair<string, bool>> GetEnumerator()
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

        public bool TryGetValue(string key, out bool value)
        {
            return _data.TryGetValue(key, out value);
        }

        public bool this[string key] => _data[key];

        public IEnumerable<string> Keys => _data.Keys;
        IEnumerable<bool> IReadOnlyDictionary<string, bool>.Values => _data.Values;

        public CssClassList Clear()
        {
            _data = ImmutableDictionary<string, bool>.Empty;
            return this;
        }

        public CssClassList Add(IEnumerable<KeyValuePair<string, bool>> values)
        {
            _data = _data.SetItems(values);
            return this;
        }

        public CssClassList Add(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                _data = _data.SetItem(key, true);
            }
            return this;
        }

        public CssClassList Add(params string[] keys)
        {
            foreach (var key in keys)
            {
                _data = _data.SetItem(key, true);
            }
            return this;
        }

        public CssClassList Add(CssClassList cssClassList)
        {
            _data = _data.SetItems(cssClassList);
            return this;
        }

        public CssClassList Add(string key, bool value)
        {
            _data = _data.SetItem(key, value);
            return this;
        }

        public CssClassList Remove(IEnumerable<string> keys)
        {
            _data = _data.RemoveRange(keys);
            return this;
        }

        public CssClassList Remove(params string[] keys)
        {
            _data = _data.RemoveRange(keys);
            return this;
        }

        public static explicit operator string(CssClassList classList) =>
            string.Join(" ", classList._data.Where(z => z.Value).Select(z => z.Key));
    }
}