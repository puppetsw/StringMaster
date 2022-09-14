using System;
using System.Windows;
using System.Windows.Controls;
using StringMaster.Services.Interfaces;
using StringMaster.ViewModels;

namespace StringMaster.UserControls;

/// <summary>
/// Interaction logic for StringMasterUC.xaml
/// </summary>
public partial class StringCogoPoints : UserControl, IPaletteControl
{
    public event EventHandler DismissPaletteEvent;

    public StringCogoPoints(StringCogoPointsViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }

    private void DismissPalette_Click(object sender, RoutedEventArgs e)
    {
        DismissPaletteEvent?.Invoke(sender, e);
    }
}
