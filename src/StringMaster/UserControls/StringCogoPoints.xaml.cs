using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using StringMaster.ViewModels;

namespace StringMaster.UserControls;

/// <summary>
/// Interaction logic for StringMasterUC.xaml
/// </summary>
public partial class StringCogoPoints : UserControl
{
    private string _fileName;

    public StringCogoPoints(StringCogoPointsViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;

        Unloaded += SaveSettings;
    }

    private void SaveSettings(object sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.DescriptionKeyFileName = _fileName;
        Properties.Settings.Default.Save();
        Unloaded -= SaveSettings;
    }

    private void Load_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog();

        if (dialog.ShowDialog() == true)
        {
            _fileName = dialog.FileName;
            var isLoaded = ((StringCogoPointsViewModel)DataContext).LoadSettings(_fileName);
            if (!isLoaded)
            {
                MessageBox.Show("Unable to load Description Key file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            {
                MessageBox.Show("Unable to save Description Key file. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}