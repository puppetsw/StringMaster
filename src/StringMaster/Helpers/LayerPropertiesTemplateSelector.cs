using System;
using System.Windows;
using System.Windows.Controls;
using StringMaster.Models;

namespace StringMaster.Helpers;

public class LayerPropertiesTemplateSelector : DataTemplateSelector
{
    public DataTemplate LayerNameTemplate { get; set; }
    public DataTemplate LayerColorTemplate { get; set; }
    public DataTemplate LayerLinetypeTemplate { get; set; }
    public DataTemplate LayerLineweightTemplate { get; set; }
    public DataTemplate LayerLockedTemplate { get; set; }
    public DataTemplate LayerOnTemplate { get; set; }
    public DataTemplate LayerFreezeTemplate { get; set; }
    public DataTemplate LayerPlotStyleTemplate { get; set; }
    public DataTemplate LayerPlotTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        return item switch
        {
            LayerNameProperty => LayerNameTemplate,
            LayerColorProperty => LayerColorTemplate,
            LayerLinetypeProperty => LayerLinetypeTemplate,
            LayerLineweightProperty => LayerLineweightTemplate,
            LayerLockedProperty => LayerLockedTemplate,
            LayerOnProperty => LayerOnTemplate,
            LayerFrozenProperty => LayerFreezeTemplate,
            LayerPlotStyleProperty => LayerPlotStyleTemplate,
            LayerPlotProperty => LayerPlotTemplate,
            _ => base.SelectTemplate(item, container) //throw new InvalidOperationException("was not a valid layer property")
        };
    }
}
