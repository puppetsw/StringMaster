using System.Windows;
using StringMaster.Extensions;
using StringMaster.Services.Interfaces;
using StringMaster.ViewModels;

namespace StringMaster.Dialogs;

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
