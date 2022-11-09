using System.Windows;

namespace StringMaster.UI.Models;

public class ColumnContext : Freezable
{
    public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
        "Data", typeof (object), typeof (ColumnContext), new UIPropertyMetadata(null));

    public object Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    protected override Freezable CreateInstanceCore()
    {
        return new ColumnContext();
    }
}
