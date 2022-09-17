using System;
using System.Windows.Markup;

namespace StringMaster.Helpers;

public sealed class ResourceHelper : MarkupExtension
{
    public string Name
    {
        get; set;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return StringHelpers.GetLocalizedString(Name);
    }
}