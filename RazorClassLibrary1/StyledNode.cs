using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DynamicData.Cache.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace RazorClassLibrary1
{
    public class AttributeList : IReadOnlyDictionary<string, object?>
    {
        public static AttributeList Create(IEnumerable<KeyValuePair<string, object?>> values) => new AttributeList().Add(values);
        private IImmutableDictionary<string, object?> _data = ImmutableDictionary<string, object?>.Empty;
        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
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

        public bool TryGetValue(string key, out object value)
        {
            return _data.TryGetValue(key, out value);
        }

        public object? this[string key] => _data[key];

        public IEnumerable<string> Keys => _data.Keys;
        IEnumerable<object?> IReadOnlyDictionary<string, object?>.Values => _data.Values;

        private void AssertKey(string key)
        {
            if (key?.Equals("class", StringComparison.OrdinalIgnoreCase) == true) throw new NotSupportedException("Use CssClassList to add classes");
            if (key?.Equals("style", StringComparison.OrdinalIgnoreCase) == true) throw new NotSupportedException("Use StyleList item to add styles");
        }

        private void AssertKey(IEnumerable<KeyValuePair<string, object>> keys)
        {
            foreach (var key in keys) AssertKey(key.Key);
        }

        public AttributeList Add(IEnumerable<KeyValuePair<string, object>> values)
        {
            AssertKey(values);
            _data = _data.SetItems(values);
            return this;
        }

        public AttributeList Add(string name, bool value)
        {
            AssertKey(name);
            _data = _data.SetItem(name, value);
            return this;
        }

        public AttributeList Add(string name, object? value)
        {
            AssertKey(name);
            _data = _data.SetItem(name, value);
            return this;
        }

        public AttributeList Add(string name, string? value)
        {
            AssertKey(name);
            _data = _data.SetItem(name, value);
            return this;
        }

        public AttributeList Add(string name, MulticastDelegate value)
        {
            AssertKey(name);
            _data = _data.SetItem(name, value);
            return this;
        }

        public AttributeList AddHandler(string name, EventCallback value)
        {
            name = name.StartsWith("on", StringComparison.OrdinalIgnoreCase) ? name : $"on{name}";
            AssertKey(name);
            _data = _data.SetItem(name, value);
            return this;
        }

        public AttributeList AddHandler<TArgument>(string name, EventCallback<TArgument> value)
        {
            name = name.StartsWith("on", StringComparison.OrdinalIgnoreCase) ? name : $"on{name}";
            AssertKey(name);
            _data = _data.SetItem(name, value);
            return this;
        }

        public AttributeList Add(string name, EventCallback value)
        {
            AssertKey(name);
            _data = _data.SetItem(name, value);
            return this;
        }

        public AttributeList Add<TArgument>(string name, EventCallback<TArgument> value)
        {
            AssertKey(name);
            _data = _data.SetItem(name, value);
            return this;
        }

        public AttributeList Remove(IEnumerable<string> keys)
        {
            _data = _data.RemoveRange(keys);
            return this;
        }

        public AttributeList Remove(params string[] keys)
        {
            _data = _data.RemoveRange(keys);
            return this;
        }


    }
    public class StyledNode : IStyledComponent
    {
        public AttributeList Attributes { get; } = new AttributeList();
        public StyleList Style { get; } = new StyleList();
        public CssClassList Class { get; } = new CssClassList();
        public const int SequenceCount = 4;

        public void Apply(ref int sequence, RenderTreeBuilder treeBuilder)
        {
            if (Attributes.Any())
            {
                treeBuilder.AddMultipleAttributes(sequence+3, Attributes);
            }

            if (Class.Any())
            {
                treeBuilder.AddAttribute(sequence + 1, "class", (string)Class);
            }

            if (Style.Any())
            {
                treeBuilder.AddAttribute(sequence + 2, "style", (string)Style);
            }

            sequence += SequenceCount;
        }
    }
}