using System.Windows;
using System.Windows.Controls;
using StringMaster.Models;

namespace StringMaster.Resources;

public class PropertyEditTemplateSelector : DataTemplateSelector
{
    public DataTemplate TextTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        return item switch
        {
            StringProperty => TextTemplate,
            _ => base.SelectTemplate(item, container)
        };
    }
}
