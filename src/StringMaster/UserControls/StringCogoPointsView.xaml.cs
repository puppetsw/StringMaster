using System;
using System.Windows;
using System.Windows.Controls;
using StringMaster.Models;
using StringMaster.Services.Interfaces;
using StringMaster.ViewModels;

namespace StringMaster.UserControls;

/// <summary>
/// Interaction logic for StringMasterUC.xaml
/// </summary>
public partial class StringCogoPointsView : UserControl, IPaletteControl
{
    public event EventHandler DismissPaletteEvent;

    public IAcadColorPicker ColorPicker { get; } = Ioc.Resolve<IAcadColorPicker>();

    public IAcadLayerService LayerService { get; } = Ioc.Resolve<IAcadLayerService>();

    public StringCogoPointsView(StringCogoPointsViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }

    private void DismissPalette_Click(object sender, RoutedEventArgs e)
    {
        DismissPaletteEvent?.Invoke(sender, e);
    }

    private void ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var cmb = (ComboBox)sender;
        var color = (AcadColor)cmb.SelectedItem;

        if (color.Name.Contains("Select"))
            cmb.SelectedItem = ColorPicker.GetAcadColor();
    }
}
