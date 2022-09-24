#nullable enable

using System.Collections.ObjectModel;
using System.Windows.Input;
using StringMaster.Models;
using StringMaster.Services.Interfaces;

// ReSharper disable UnusedMember.Global

namespace StringMaster.ViewModels;

public class LayerCreateDialogViewModel : ObservableObject
{
    private ObservableCollection<PropertyBase> _properties = new();
    private AcadColor? _acadColor;
    private string _layerName;
    private string? _linetype;
    private string? _lineweight;
    private bool _isLocked;
    private bool _isOn;
    private bool _isFrozen;
    private bool _isPlottable;

    public ICommand ShowLineweightDialogCommand { get; }
    public ICommand ShowLinetypeDialogCommand { get; }

    // TODO: ViewModel Services
    public IAcadColorDialogService ColorDialogService { get; } = Ioc.Default.GetInstance<IAcadColorDialogService>();
    public IAcadLinetypeDialogService LinetypeDialogService { get; } = Ioc.Default.GetInstance<IAcadLinetypeDialogService>();
    public IAcadLineweightDialogService LineweightDialogService { get; } = Ioc.Default.GetInstance<IAcadLineweightDialogService>();

    public ObservableCollection<string> YesNoSelect { get; } = new() { "Yes", "No" };

    public ObservableCollection<PropertyBase> Properties
    {
        get => _properties;
        set => SetProperty(ref _properties, value);
    }

    public string LayerName
    {
        get => _layerName;
        set => SetProperty(ref _layerName, value);
    }

    public AcadColor? AcadColor // BUG: Dialog opens twice.
    {
        get => _acadColor;
        set
        {
            if (value is null)
                return;

            if (value.Name.Contains("Select"))
                SetProperty(ref _acadColor, ColorDialogService.ShowDialog());
            else
                SetProperty(ref _acadColor, value);
        }
    }

    public string? Linetype
    {
        get => _linetype;
        set => SetProperty(ref _linetype, value);
    }

    public string? Lineweight
    {
        get => _lineweight;
        set => SetProperty(ref _lineweight, value);
    }

    public bool IsLocked
    {
        get => _isLocked;
        set => SetProperty(ref _isLocked, value);
    }

    public bool IsOn
    {
        get => _isOn;
        set => SetProperty(ref _isOn, value);
    }

    public bool IsFrozen
    {
        get => _isFrozen;
        set => SetProperty(ref _isFrozen, value);
    }

    public bool IsPlottable
    {
        get => _isPlottable;
        set => SetProperty(ref _isPlottable, value);
    }

    public LayerCreateDialogViewModel()
    {
        AcadColor = new AcadColor(255, 0, 0, 1);
        _layerName = "";
        _lineweight = "Default"; // Default Lineweight
        _linetype = "Continuous";
        _isOn = true;
        _isLocked = false;
        _isFrozen = false;
        _isPlottable = true;

        ShowLineweightDialogCommand = new RelayCommand(ShowLineweightDialog);
        ShowLinetypeDialogCommand = new RelayCommand(ShowLinetypeDialog);

        AddLayerProperties();
    }

    private void ShowLinetypeDialog() => Linetype = LinetypeDialogService.ShowDialog(Linetype);

    private void ShowLineweightDialog() => Lineweight = LineweightDialogService.ShowDialog(Lineweight);

    private void AddLayerProperties()
    {
        Properties.Add(new LayerNameProperty());
        Properties.Add(new LayerColorProperty());
        Properties.Add(new LayerLinetypeProperty());
        Properties.Add(new LayerLineweightProperty());
        Properties.Add(new LayerLockedProperty());
        Properties.Add(new LayerOnProperty());
        Properties.Add(new LayerFrozenProperty());
        Properties.Add(new LayerPlotStyleProperty());
        Properties.Add(new LayerPlotProperty());
    }
}
