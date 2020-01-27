using Microsoft.AspNetCore.Components.Forms;

namespace RazorClassLibrary1
{
    public interface IInputComponent
    {

        EditContext? EditContext { get; }
        FieldIdentifier FieldIdentifier { get; }
    }
}