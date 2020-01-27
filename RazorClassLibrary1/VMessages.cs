using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using ReactiveUI;

namespace RazorClassLibrary1
{
    public class VMessages : ThemeableComponent
    {
        private IEnumerable<string> _messages;

        [Parameter]
        public IEnumerable<string> Messages
        {
            get => _messages;
            set => this.RaiseAndSetIfChanged(ref _messages, value);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            var sequence = 0;
            builder.OpenElementNode(ref sequence, "div", node => node.SetTextColor(Color).AddClass("v-messages").AddClasses(ThemeClasses));

            generateChildren(ref sequence, builder);

            builder.CloseElement();
        }

        private void generateChildren(ref int sequence, RenderTreeBuilder builder)
        {
            // TODO: Transitions
            builder.OpenElementNode(ref sequence, "div", node => node.AddClass("v-messages__wrapper"));

            foreach (var item in Messages)
            {
                generateMessage(ref sequence, builder, item);
            }

            builder.CloseElement();
        }

        private void generateMessage(ref int sequence, RenderTreeBuilder builder, string value)
        {
            builder.OpenElementNode(ref sequence, "div", node => node.AddClass("v-messages__message"));
            builder.SetKey(value);
            builder.AddContent(sequence++, value);
            builder.CloseElement();
        }
    }
}