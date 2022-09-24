using System.Windows;
using StringMaster.Extensions;
using StringMaster.Services.Interfaces;
using StringMaster.ViewModels;

namespace StringMaster.Dialogs;

/// <summary>
/// Interaction logic for LayerCreateDialog.xaml
/// </summary>
public partial class LayerCreateDialog : Window, IDialog<LayerCreateDialogViewModel>
{
    public LayerCreateDialogViewModel ViewModel
    {
        get => (LayerCreateDialogViewModel)DataContext;
        set => DataContext = value;
    }

    public LayerCreateDialog()
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
