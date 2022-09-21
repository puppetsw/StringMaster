using System.Windows;
using System.Windows.Controls;
using StringMaster.Models;

namespace StringMaster.Resources;

public class PropertyTemplateSelector : DataTemplateSelector
{
    public DataTemplate TextTemplate { get; set; }

    public DataTemplate ColorTemplate { get; set; }

    public DataTemplate SelectYesNoTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        return item switch
        {
            StringProperty => TextTemplate,
            ColorProperty => ColorTemplate,
            SelectYesNoProperty => SelectYesNoTemplate,
            _ => base.SelectTemplate(item, container)
        };
    }
}
