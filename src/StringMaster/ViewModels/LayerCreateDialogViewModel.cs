#nullable enable

using System.Collections.ObjectModel;
using StringMaster.Models;
using StringMaster.Services.Interfaces;

// ReSharper disable UnusedMember.Global

namespace StringMaster.ViewModels;

public class LayerCreateDialogViewModel : ObservableObject
{
    private ObservableCollection<PropertyBase> _properties = new();
    private AcadColor? _acadColor;
    private string _layerName;
    private string _linetype;
    private AcadLineweight _lineweight;
    private bool _isLocked;
    private bool _isOn;
    private bool _isFrozen;
    private bool _isPlottable;
    private ObservableCollection<AcadLineweight> _lineweights;

    // TODO: ViewModel Services
    public IAcadColorDialogService ColorDialogService { get; } = Ioc.Default.GetInstance<IAcadColorDialogService>();
    // public IAcadLineTypeService LineTypeService { get; } = Ioc.Default.GetInstance<IAcadLineTypeService>();
    public IAcadLineweightService LineWeightService { get; } = Ioc.Default.GetInstance<IAcadLineweightService>();

    public ObservableCollection<AcadLineweight> Lineweights
    {
        get => _lineweights;
        set => SetProperty(ref _lineweights, value);
    }

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

    public string Linetype
    {
        get => _linetype;
        set => SetProperty(ref _linetype, value);
    }

    public AcadLineweight Lineweight
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
        _lineweights = new ObservableCollection<AcadLineweight>(LineWeightService.GetLineweightsFromActiveDocument());

        AcadColor = new AcadColor(255, 0, 0, 1);
        _layerName = "";
        _lineweight = Lineweights[2]; // Default Lineweight
        _linetype = "Continuous";
        _isOn = true;
        _isLocked = false;
        _isFrozen = false;
        _isPlottable = true;

        AddLayerProperties();
    }

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
