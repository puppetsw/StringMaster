using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Shapes;
using StringMaster.UI.Helpers;
using StringMaster.UI.Models;
using StringMaster.UI.Services.Interfaces;

// ReSharper disable UnusedMember.Global

namespace StringMaster.UI.ViewModels;

public class StringCogoPointsViewModel : ObservableObject
{
    private bool _isUnsavedChanges;
    private readonly IOpenDialogService _openDialogService;
    private readonly ISaveDialogService _saveDialogService;
    private readonly IMessageBoxService _messageBoxService;
    private readonly IDialogService _dialogService;
    private readonly IImportService _importService;
    private ObservableCollection<DescriptionKey> _descriptionKeys;
    private ObservableCollection<DescriptionKey> _unchangedDescriptionKeys;
    private string _currentFileName;
    private DescriptionKey _selectedKey;

    public string CurrentFileName
    {
        get => _currentFileName;
        set => SetProperty(ref _currentFileName, value);
    }

    public bool IsUnsavedChanges
    {
        get => _isUnsavedChanges;
        set => SetProperty(ref _isUnsavedChanges, value);
    }

    public ObservableCollection<DescriptionKey> DescriptionKeys
    {
        get => _descriptionKeys;
        set => SetProperty(ref _descriptionKeys, value);
    }

    public DescriptionKey SelectedKey
    {
        get => _selectedKey;
        set
        {
            SetProperty(ref _selectedKey, value);
        }
    }

    public ICommand NewDescriptionKeyFileCommand { get; }
    public ICommand OpenDescriptionKeyFileCommand { get; }
    public ICommand SaveDescriptionKeyFileCommand { get; }
    public ICommand SaveAsDescriptionKeyFileCommand { get; }
    public ICommand ImportCommand { get; }
    public ICommand AddRowCommand { get; }
    public ICommand RemoveRowCommand { get; }
    public ICommand StringCommand { get; }
    public ICommand LayerSelectCommand => new RelayCommand(ShowLayerSelectionDialog);

    public StringCogoPointsViewModel(IOpenDialogService openDialogService,
                                     ISaveDialogService saveDialogService,
                                     IMessageBoxService messageBoxService,
                                     IDialogService dialogService,
                                     IImportService importService)
    {
        _openDialogService = openDialogService;
        _saveDialogService = saveDialogService;
        _messageBoxService = messageBoxService;
        _dialogService = dialogService;
        _importService = importService;

        _openDialogService.DefaultExt = ".xml";
        _openDialogService.Filter = "XML Files (*.xml)|*.xml";

        _saveDialogService.DefaultExt = ".xml";
        _saveDialogService.Filter = "XML Files (*.xml)|*.xml";

        DescriptionKeys = new ObservableCollection<DescriptionKey>();

        // Initialize commands
        NewDescriptionKeyFileCommand = new RelayCommand(NewDescriptionKeyFile);
        OpenDescriptionKeyFileCommand = new RelayCommand(OpenDescriptionKeyFile);
        SaveDescriptionKeyFileCommand = new RelayCommand(SaveDescriptionKeyFile, () => IsUnsavedChanges);
        SaveAsDescriptionKeyFileCommand = new RelayCommand(SaveAsDescriptionKeyFile);
        AddRowCommand = new RelayCommand(AddRow);
        RemoveRowCommand = new RelayCommand(RemoveRow);
        StringCommand = new RelayCommand(StringCogoPoints, () => DescriptionKeys is not null &&
                                                                 DescriptionKeys.Count > 0 &&
                                                                 DescriptionKeys.All(x => x.IsValid));
        ImportCommand = new RelayCommand(ImportPointsFromFile);

        LoadSettingsFromFile(Properties.Settings.Default.DescriptionKeyFileName);
    }

    /// <summary>
    /// Hook-up PropertyChanged events for sub-models.
    /// </summary>
    private void DescriptionKeysOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
            foreach (DescriptionKey descriptionKey in e.NewItems)
            {
                descriptionKey.PropertyChanged += DescriptionKeyPropertyChanged;
                descriptionKey.AcadColor.PropertyChanged += DescriptionKeyPropertyChanged;
                descriptionKey.AcadLayer.PropertyChanged += DescriptionKeyPropertyChanged;
            }

        if (e.OldItems != null)
            foreach (DescriptionKey descriptionKey in e.OldItems)
            {
                descriptionKey.PropertyChanged -= DescriptionKeyPropertyChanged;
                descriptionKey.AcadColor.PropertyChanged -= DescriptionKeyPropertyChanged;
                descriptionKey.AcadLayer.PropertyChanged -= DescriptionKeyPropertyChanged;
            }
    }

    private void DescriptionKeyPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        // If DescriptionKeys matches the cloned keys then no changes.
        IsUnsavedChanges = !DescriptionKeys.SequenceEqual(_unchangedDescriptionKeys);
    }

    private void ImportPointsFromFile() => _openDialogService.ShowDialog();

    private void AddRow()
    {
        DescriptionKeys ??= new();
        DescriptionKeys.Add(new DescriptionKey());
        IsUnsavedChanges = true;
        DescriptionKeyPropertyChanged(null, null);
    }

    private void RemoveRow()
    {
        if (DescriptionKeys is null)
            return;

        if (SelectedKey != null)
        {
            if (SelectedKey.IsValid)
            {
                var dialog = _messageBoxService.ShowYesNo("Delete", "Remove this description key? This cannot be undone.");
                if (dialog == true)
                    DescriptionKeys?.Remove(SelectedKey);
            }
            else
            {
                DescriptionKeys?.Remove(SelectedKey);
            }
        }

        IsUnsavedChanges = true;
        DescriptionKeyPropertyChanged(null, null);
    }

    // TODO: This method requires a massive cleanup.
    // The below method current works, but it's very messy and not structured well.
    // The idea was to get something that was working with the 'special codes'.
    // The feature line/poly/3dpoly line system needs a bit of work also.
    // TODO: Remove references to Civil (abstract)
    private void StringCogoPoints()
    {
        
    }

    private void ShowLayerSelectionDialog()
    {
        var vm = new LayerSelectDialogViewModel();
        var dialog = _dialogService.ShowDialog(vm);

        if (dialog == true)
            DescriptionKeys[DescriptionKeys.IndexOf(SelectedKey)].AcadLayer = vm.SelectedLayer;
    }

    private void RemoveInvalidDescriptionKeys()
    {
        // Remove invalid keys
        foreach (var itemToRemove in DescriptionKeys.Where(x => !x.IsValid).ToList())
            DescriptionKeys.Remove(itemToRemove);
    }

    private void UnhookPropertyChangeEvents()
    {
        foreach (DescriptionKey descriptionKey in DescriptionKeys)
        {
            descriptionKey.PropertyChanged -= DescriptionKeyPropertyChanged;
            descriptionKey.AcadColor.PropertyChanged -= DescriptionKeyPropertyChanged;
            descriptionKey.AcadLayer.PropertyChanged -= DescriptionKeyPropertyChanged;
        }
    }

    /// <summary>
    /// Get the last xml file loaded from settings
    /// </summary>
    /// <param name="fileName"></param>
    private void LoadSettingsFromFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            return;

        DescriptionKeys.CollectionChanged -= DescriptionKeysOnCollectionChanged;
        UnhookPropertyChangeEvents();

        CurrentFileName = fileName;

        DescriptionKeys = new ObservableCollection<DescriptionKey>();
        DescriptionKeys.CollectionChanged += DescriptionKeysOnCollectionChanged;

        try
        {
            var keysFromFile = XmlHelper.ReadFromXmlFile<ObservableCollection<DescriptionKey>>(fileName);

            if (keysFromFile is not null)
            {
                SetUnchangedDescriptionKeys(keysFromFile);

                foreach (DescriptionKey key in keysFromFile)
                    DescriptionKeys.Add(key);
            }
        }
        catch (Exception e)
        {
            //CivilApplication.Editor.WriteMessage("\nUnable to load description key file. ");
            //CivilApplication.Editor.WriteMessage($"\n{e.Message}");
            Console.WriteLine(e);
            // Clone didn't work so we set it to empty
            _unchangedDescriptionKeys = new();
        }

        IsUnsavedChanges = false;
        Properties.Settings.Default.DescriptionKeyFileName = fileName;
        Properties.Settings.Default.Save();
    }

    /// <summary>
    /// Clones the a <see cref="ObservableCollection{T}"/> of type <see cref="DescriptionKey"/> to another
    /// ObservableCollection for comparison.
    /// </summary>
    /// <param name="keysFromFile">ObservableCollection to clone.</param>
    private void SetUnchangedDescriptionKeys(ObservableCollection<DescriptionKey> keysFromFile)
    {
        _unchangedDescriptionKeys = new ObservableCollection<DescriptionKey>();

        foreach (var item in keysFromFile)
        {
            var clonedDesKey = item.Clone();
            _unchangedDescriptionKeys.Add(clonedDesKey);
        }
    }

    /// <summary>
    /// Save XML file
    /// </summary>
    /// <param name="fileName"></param>
    private void SaveToFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName) || DescriptionKeys == null)
            throw new ArgumentNullException(nameof(fileName));

        RemoveInvalidDescriptionKeys();

        if (DescriptionKeys.Count == 0)
        {
            _messageBoxService.ShowWarning("StringMaster", "Unable to save file. No valid description keys found.");
            return;
        }

        XmlHelper.WriteToXmlFile(fileName, DescriptionKeys);
        SetUnchangedDescriptionKeys(DescriptionKeys);
        Properties.Settings.Default.DescriptionKeyFileName = fileName;
        Properties.Settings.Default.Save();
        IsUnsavedChanges = false;
    }

    private void Save()
    {
        if (string.IsNullOrEmpty(CurrentFileName))
            SaveAs();
        else
            SaveToFile(CurrentFileName);
    }

    private void SaveAs()
    {
        var result = _saveDialogService.ShowDialog();
        if (result != true)
            return;

        CurrentFileName = _saveDialogService.FileName;
        SaveToFile(_saveDialogService.FileName);
    }

    private bool? CheckForUnsavedChangesAndContinue()
    {
        if (IsUnsavedChanges)
            return _messageBoxService.ShowYesNoCancel(StringHelpers.GetLocalizedString("UnsavedChangesTitle"),
                StringHelpers.GetLocalizedString("UnsavedChangesText"));
        return true;
    }

    // View Commands
    private void SaveDescriptionKeyFile() => Save();

    private void SaveAsDescriptionKeyFile() => SaveAs();

    private void NewDescriptionKeyFile()
    {
        var continueWithChanges = CheckForUnsavedChangesAndContinue();
        switch (continueWithChanges)
        {
            case true: // Do discard, or we can continue
                break;
            case false: // Don't discard changes
                Save();
                break;
            case null: // Cancelled
                return;
        }

        CurrentFileName = string.Empty;
        IsUnsavedChanges = true;
        UnhookPropertyChangeEvents();
        DescriptionKeys = new ObservableCollection<DescriptionKey>();
        DescriptionKeys.CollectionChanged += DescriptionKeysOnCollectionChanged;
        _unchangedDescriptionKeys = new ObservableCollection<DescriptionKey>();
    }

    private void OpenDescriptionKeyFile()
    {
        var continueWithChanges = CheckForUnsavedChangesAndContinue();
        switch (continueWithChanges)
        {
            case true: // Do discard, or we can continue
                break;
            case false: // Don't discard changes
                Save();
                break;
            case null:  // Cancelled
                return;
        }

        var dialog = _openDialogService.ShowDialog();
        if (dialog != true)
            return;

        CurrentFileName = _openDialogService.FileName;
        LoadSettingsFromFile(CurrentFileName);
    }
}
