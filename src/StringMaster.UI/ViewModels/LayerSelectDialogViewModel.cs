﻿#nullable enable

using System.Collections.ObjectModel;
using System.Windows.Input;
using StringMaster.UI.Models;
using StringMaster.UI.Services.Interfaces;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace StringMaster.UI.ViewModels;

public class LayerSelectDialogViewModel : ObservableObject
{
    private readonly IAcadApplicationService _acadApplicationService;
    private readonly IAcadLayerService _acadLayerService;// = StaticServices.LayerService;
    private readonly IDialogService _dialogService;// = StaticServices.DialogService;

    private ObservableCollection<AcadLayer> _layers = new();
    private ObservableCollection<string> _documents = new();

    private AcadLayer? _selectedLayer;
    private string? _selectedDocument;

    public ObservableCollection<AcadLayer> Layers
    {
        get => _layers;
        set => SetProperty(ref _layers, value);
    }

    public ObservableCollection<string> Documents
    {
        get => _documents;
        set => SetProperty(ref _documents, value);
    }

    public AcadLayer? SelectedLayer
    {
        get => _selectedLayer;
        set => SetProperty(ref _selectedLayer, value);
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

    public ICommand ShowNewLayerDialogCommand => new RelayCommand(ShowNewLayerDialog);

    private void ShowNewLayerDialog() // BUG: something was null. to investigate.
    {
        var vm = new LayerCreateDialogViewModel();
        var dialog = _dialogService.ShowDialog(vm);

        if (dialog == false)
            return;

        // Add new layer
        var layer = new AcadLayer(vm.LayerName, vm.IsOn, vm.IsFrozen, vm.IsLocked, vm.AcadColor)
        {
            Lineweight = vm.Lineweight,
            IsPlottable = vm.IsPlottable,
            Linetype = vm.Linetype
        };

        if (!layer.IsValid)
            return;

        _acadLayerService.CreateLayer(layer, _selectedDocument);
        Layers = new(_acadLayerService.GetLayersFromDocument(_selectedDocument));
    }

    // TODO: Dependency Injection
    public LayerSelectDialogViewModel(IAcadLayerService acadLayerService,
                                      IDialogService dialogService,
                                      IAcadApplicationService acadApplicationService)
    {
        _acadLayerService = acadLayerService;
        _dialogService = dialogService;
        _acadApplicationService = acadApplicationService;

        foreach (AcadDocument document in _acadApplicationService.Documents)
            Documents.Add(document.Name);

        SelectedDocument = _acadApplicationService.ActiveDocument.Name;

        SelectedLayer = Layers[0];
    }
}
