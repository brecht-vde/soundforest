using Spotiwood.UI.App.Components.Icons;

namespace Spotiwood.UI.App.Components.Branding;

public record ButtonMenuItem(string Text, Icon Icon, Func<EventArgs, Task> OnClick);