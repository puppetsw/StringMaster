using System.Windows;
using StringMaster.UI.Extensions;
using StringMaster.UI.Services.Interfaces;
using StringMaster.UI.ViewModels;

namespace StringMaster.UI.Dialogs;

/// <summary>
/// Interaction logic for LayerSelectionView.xaml
/// </summary>
public partial class LayerSelectDialog : Window, IDialog<LayerSelectDialogViewModel>
{
    public LayerSelectDialogViewModel ViewModel
    {
        get => (LayerSelectDialogViewModel)DataContext;
        set => DataContext = value;
    }

    public LayerSelectDialog()
    {
        InitializeComponent();

        SourceInitialized += (_, _) => this.HideMinimizeAndMaximizeButtons();

        DataGrid.Focus();
    }

    private void ButtonCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ButtonOK_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}
