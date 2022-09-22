#nullable enable

using System.Collections.ObjectModel;
using System.Windows.Input;
using StringMaster.Models;

namespace StringMaster.ViewModels;

public class LayerCreateDialogViewModel : ObservableObject
{
    private ObservableCollection<LayerPropertyBase> _properties;

    public ObservableCollection<string> YesNoSelect { get; } = new() { "Yes", "No" };

    public ObservableCollection<LayerPropertyBase> Properties
    {
        get => _properties;
        set => SetProperty(ref _properties, value);
    }

    public AcadLayer? NewLayer { get; private set; }

    public ICommand NewLayerCommand => new RelayCommand(GenerateNewLayer);

    // TODO: this works but its bloaty.

    private void GenerateNewLayer()
    {
        var name = Properties[0].Value.ToString();
        var isOn = Properties[5].Value.ToString() == "Yes";
        var isFrozen = Properties[6].Value.ToString() == "Yes";
        var isLocked = Properties[4].Value.ToString() == "Yes";
        var color = (AcadColor)Properties[1].Value;

        var layer = new AcadLayer(name, isOn, isFrozen, isLocked, color);

        if (layer.IsValid)
            NewLayer = layer;
    }

    public LayerCreateDialogViewModel()
    {
        _properties = new()
        {
            new StringProperty { Name = "Layer name", Value = "" },
            new ColorProperty { Name = "Color", Value = new AcadColor(255, 0, 0, 1) }, // TODO: DIALOG needed
            new StringProperty { Name = "Linetype", Value = "Continuous" }, // TODO: Add dialog to select
            new StringProperty { Name = "Lineweight", Value = "Default" }, // TODO: Add dialog to select
            new SelectYesNoProperty { Name = "Locked", Value = "No" },
            new SelectYesNoProperty { Name = "On", Value = "Yes" },
            new SelectYesNoProperty { Name = "Freeze", Value = "No" },
            new StringProperty { Name = "Plot Style", Value = "", IsReadOnly = true },
            new SelectYesNoProperty { Name = "Plot", Value = "Yes" }
        };
    }
}