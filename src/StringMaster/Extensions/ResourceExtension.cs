using System;
using System.Windows.Markup;
using StringMaster.Helpers;

namespace StringMaster.Extensions;

public sealed class ResourceExtension : MarkupExtension
{
    public string Name
    {
        get; set;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return ResourceHelpers.GetLocalizedString(Name);
    }
}