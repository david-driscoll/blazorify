using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace RazorClassLibrary1
{
    public static class Helpers
    {
        public static void OpenElementNode(this RenderTreeBuilder builder, ref int sequence, string tag, Action<IStyledComponent> nodeAction)
        {
            builder.OpenElement(sequence++, tag);
            var node = new StyledNode();
            nodeAction(node);
            node.Apply(ref sequence, builder);
        }

        public static void OpenComponentNode<TComponent>(this RenderTreeBuilder builder, ref int sequence, Action<IStyledComponent> nodeAction)
            where TComponent : IComponent
        {
            builder.OpenComponent<TComponent>(sequence++);
            var node = new StyledNode();
            nodeAction(node);
            node.Apply(ref sequence, builder);
        }

        public static void OpenComponentNode(this RenderTreeBuilder builder, ref int sequence, Type type, Action<IStyledComponent> nodeAction)
        {
            builder.OpenComponent(sequence++, type);
            var node = new StyledNode();
            nodeAction(node);
            node.Apply(ref sequence, builder);
        }


        public static IStyledComponent ClearClasses(this IStyledComponent @this)
        {
            @this.Class.Clear();
            return @this;
        }

        public static IStyledComponent AddClasses(this IStyledComponent @this, IEnumerable<KeyValuePair<string, bool>> values)
        {
            @this.Class.Add(values);
            return @this;
        }

        public static IStyledComponent AddClasses(this IStyledComponent @this, IEnumerable<string> keys)
        {
            @this.Class.Add(keys);
            return @this;
        }

        public static IStyledComponent AddClasses(this IStyledComponent @this, params string[] keys)
        {
            @this.Class.Add(keys);
            return @this;
        }

        public static IStyledComponent AddClasses(this IStyledComponent @this, CssClassList cssClassList)
        {
            @this.Class.Add(cssClassList);
            return @this;
        }

        public static IStyledComponent AddClasses(this IStyledComponent @this, IStyledComponent component)
        {
            @this.Class.Add(component.Class);
            @this.Style.Add(component.Style);
            return @this;
        }

        public static IStyledComponent AddClass(this IStyledComponent @this, string key, bool value)
        {
            @this.Class.Add(key, value);
            return @this;
        }

        public static IStyledComponent AddClass(this IStyledComponent @this, string key)
        {
            @this.Class.Add(key);
            return @this;
        }

        public static IStyledComponent RemoveClasses(this IStyledComponent @this, IEnumerable<string> keys)
        {
            @this.Class.Remove(keys);
            return @this;
        }

        public static IStyledComponent RemoveClasses(this IStyledComponent @this, params string[] keys)
        {
            @this.Class.Remove(keys);
            return @this;
        }

        public static IStyledComponent RemoveClass(this IStyledComponent @this, string key)
        {
            @this.Class.Remove(key);
            return @this;
        }

        public static IStyledComponent SetBackgroundColor(this IStyledComponent @this, string? color)
        {
            if (String.IsNullOrWhiteSpace(color)) return @this;
            // TODO: Try and parse css color
            @this.Class.Add(color, true);
            return @this;
        }

        public static IStyledComponent SetBackgroundColor(this IStyledComponent @this, CssColor? color)
        {
            if (!color.HasValue) return @this;
            @this.Style.Add("background-color", color).Add("border-color", color);
            return @this;
        }


        public static IStyledComponent ClearStyles(this IStyledComponent @this)
        {
            @this.Style.Clear();
            return @this;
        }

        public static IStyledComponent AddStyles(this IStyledComponent @this, IEnumerable<KeyValuePair<string, string>> values)
        {
            @this.Style.Add(values);
            return @this;
        }

        public static IStyledComponent AddStyle(this IStyledComponent @this, string key, string value)
        {
            @this.Style.Add(key, value);
            return @this;
        }

        public static IStyledComponent RemoveStyles(this IStyledComponent @this, IEnumerable<string> keys)
        {
            @this.Style.Remove(keys);
            return @this;
        }

        public static IStyledComponent RemoveStyle(this IStyledComponent @this, string key)
        {
            @this.Style.Remove(key);
            return @this;
        }

        public static IStyledComponent RemoveStyles(this IStyledComponent @this, params string[] keys)
        {
            @this.Style.Remove(keys);
            return @this;
        }

        public static IStyledComponent SetTextColor(this IStyledComponent @this, string? color)
        {
            if (String.IsNullOrWhiteSpace(color)) return @this;
            // TODO: Try and parse css color
            var parts = color.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var colorName = parts[0];
            @this.Class.Add($"{colorName}--text", true);
            if (parts[^1] != parts[0])
            {
                var colorModifier = parts[^1];
                @this.Class.Add($"text--{colorModifier}", true);
            }
            return @this;
        }

        public static IStyledComponent SetTextColor(this IStyledComponent @this, CssColor? color)
        {
            if (!color.HasValue) return @this;
            @this.Style.Add("color", color).Add("caret-color", color);
            return @this;
        }

        public static IStyledComponent AddAttributes(this IStyledComponent @this, IEnumerable<KeyValuePair<string, object>> values)
        {
            @this.Attributes.Add(values);
            return @this;
        }

        public static IStyledComponent AddAttribute(this IStyledComponent @this, string name, bool value)
        {
            @this.Attributes.Add(name, value);
            return @this;
        }

        public static IStyledComponent AddAttribute(this IStyledComponent @this, string name, object? value)
        {
            @this.Attributes.Add(name, value);
            return @this;
        }

        public static IStyledComponent AddAttribute(this IStyledComponent @this, string name, string? value)
        {
            @this.Attributes.Add(name, value);
            return @this;
        }

        public static IStyledComponent AddAttribute(this IStyledComponent @this, string name, MulticastDelegate value)
        {
            @this.Attributes.Add(name, value);
            return @this;
        }

        public static IStyledComponent AddAttribute(this IStyledComponent @this, string name, EventCallback value)
        {
            @this.Attributes.Add(name, value);
            return @this;
        }

        public static IStyledComponent AddAttribute<TArgument>(this IStyledComponent @this, string name, EventCallback<TArgument> value)
        {
            @this.Attributes.Add(name, value);
            return @this;
        }

        public static IStyledComponent AddHandler(this IStyledComponent @this, string name, EventCallback value)
        {
            @this.Attributes.AddHandler(name, value);
            return @this;
        }

        public static IStyledComponent AddHandler<TArgument>(this IStyledComponent @this, string name, EventCallback<TArgument> value)
        {
            @this.Attributes.AddHandler(name, value);
            return @this;
        }

        public static IStyledComponent RemoveAttributes(this IStyledComponent @this, IEnumerable<string> keys)
        {
            @this.Attributes.Remove(keys);
            return @this;
        }

        public static IStyledComponent RemoveAttribute(this IStyledComponent @this, string key)
        {
            @this.Attributes.Remove(key);
            return @this;
        }

        public static IStyledComponent RemoveAttributes(this IStyledComponent @this, params string[] keys)
        {
            @this.Attributes.Remove(keys);
            return @this;
        }

        public static IStyledComponent Add(this IStyledComponent @this, IStyledComponent component)
        {
            @this.Class.Add(component.Class);
            @this.Attributes.Add(component.Attributes);
            @this.Style.Add(component.Style);
            return @this;
        }
    }
}