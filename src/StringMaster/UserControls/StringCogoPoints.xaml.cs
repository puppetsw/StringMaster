using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using StringMaster.Helpers;
using StringMaster.ViewModels;

namespace StringMaster.UserControls;

/// <summary>
/// Interaction logic for StringMasterUC.xaml
/// </summary>
public partial class StringCogoPoints : UserControl
{
    private string _fileName;

    public StringCogoPoints() => InitializeComponent();

    private void Load_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog();

        if (dialog.ShowDialog() == true)
        {
            _fileName = dialog.FileName;
            var isLoaded = ((StringCogoPointsViewModel)DataContext).LoadSettings(_fileName);
            if (!isLoaded)
                MessageBox.Show(ResourceHelpers.GetLocalizedString("DescriptionKeySaveError"),
                    ResourceHelpers.GetLocalizedString("Error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog();

        if (dialog.ShowDialog() == true)
        {
            _fileName = dialog.FileName;
            var isSaved = ((StringCogoPointsViewModel)DataContext).SaveSettings(_fileName);
            if (!isSaved)
                MessageBox.Show(ResourceHelpers.GetLocalizedString("DescriptionKeyLoadError"),
                    ResourceHelpers.GetLocalizedString("Error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
