using System;
using System.Windows;
using System.Windows.Controls;
using StringMaster.UI.Services.Interfaces;
using StringMaster.UI.ViewModels;

namespace StringMaster.UI.Palettes;

/// <summary>
/// Interaction logic for StringMasterUC.xaml
/// </summary>
public partial class StringCogoPointsView : UserControl, IPaletteControl
{
    public event EventHandler DismissPaletteEvent;

    public IAcadColorDialogService ColorPicker => ViewModel.ACADColorDialogService;

    public StringCogoPointsViewModel ViewModel
    {
        get => (StringCogoPointsViewModel)DataContext;
        set => DataContext = value;
    }

    public StringCogoPointsView(StringCogoPointsViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }

    private void DismissPalette_Click(object sender, RoutedEventArgs e)
    {
        DismissPaletteEvent?.Invoke(sender, e);
    }
}
