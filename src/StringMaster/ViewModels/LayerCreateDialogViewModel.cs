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
    private string _lineType;
    private string _lineWeight;
    private bool _isLocked;
    private bool _isOn;
    private bool _isFrozen;
    private bool _isPlottable;

    // TODO: ViewModel Services
    public IAcadColorDialogService ColorDialogService { get; } = Ioc.Default.GetInstance<IAcadColorDialogService>();
    // public IAcadLineTypeService LineTypeService { get; } = Ioc.Default.GetInstance<IAcadLineTypeService>();

    // TODO: LineWeights
    // public ObservableCollection<string> LineWeights { get; set; }

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

    public AcadColor? AcadColor
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

    public string LineType
    {
        get => _lineType;
        set => SetProperty(ref _lineType, value);
    }

    public string LineWeight
    {
        get => _lineWeight;
        set => SetProperty(ref _lineWeight, value);
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
        _lineWeight = "";
        _lineType = "Continuous";
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
        Properties.Add(new LayerLineTypeProperty());
        Properties.Add(new LayerLineWeightProperty());
        Properties.Add(new LayerLockedProperty());
        Properties.Add(new LayerOnProperty());
        Properties.Add(new LayerFrozenProperty());
        Properties.Add(new LayerPlotStyleProperty());
        Properties.Add(new LayerPlotProperty());
    }
}
