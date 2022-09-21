#nullable enable

using System.Collections.ObjectModel;
using System.Windows.Input;
using Autodesk.AutoCAD.ApplicationServices;
using StringMaster.Models;
using StringMaster.Services.Interfaces;

namespace StringMaster.ViewModels;

public class LayerSelectDialogViewModel : ObservableObject
{
    private readonly IAcadLayerService _acadLayerService = Ioc.Default.GetInstance<IAcadLayerService>();
    private readonly IDialogService _dialogService = Ioc.Default.GetInstance<IDialogService>();
    private ObservableCollection<AcadLayer> _layers = new();
    private AcadLayer? _selectedLayer;
    private ObservableCollection<string> _documents = new();
    private string? _selectedDocument;

    public ObservableCollection<AcadLayer> Layers
    {
        get => _layers;
        set => SetProperty(ref _layers, value);
    }

    public AcadLayer? SelectedLayer
    {
        get => _selectedLayer;
        set => SetProperty(ref _selectedLayer, value);
    }

    public ObservableCollection<string> Documents
    {
        get => _documents;
        set => SetProperty(ref _documents, value);
    }

    public string? SelectedDocument
    {
        get => _selectedDocument;
        set
        {
            SetProperty(ref _selectedDocument, value);
            Layers = new(_acadLayerService.GetLayersFromDocument(_selectedDocument));
        }
    }

    public ICommand NewLayerCommand => new RelayCommand(ShowNewLayerDialog);

    private void ShowNewLayerDialog()
    {
        var vm = new LayerCreateDialogViewModel();
        var dialog = _dialogService.ShowDialog(vm);

        // Add new layer
        _acadLayerService.CreateLayer(vm.NewLayer);
    }

    public LayerSelectDialogViewModel()
    {
        foreach (Document document in CivilApplication.DocumentManager)
            Documents.Add(document.Name);

        SelectedDocument = CivilApplication.ActiveDocument.Name;

        SelectedLayer = Layers[0];
    }
}
