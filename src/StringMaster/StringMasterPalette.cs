using System;
using System.Drawing;
using Autodesk.AutoCAD.Windows;
using StringMaster.UserControls;
using StringMaster.ViewModels;

namespace StringMaster;

public class StringMasterPalette : PaletteSet
{
    private Palette _currentPalette;

    public StringMasterPalette() : base("StringMaster", new Guid("B56213B2-F7C0-499F-A3F3-A5A5EC631DA2"))
    {
        Style = PaletteSetStyles.ShowAutoHideButton | PaletteSetStyles.ShowCloseButton | PaletteSetStyles.Snappable |
                PaletteSetStyles.UsePaletteNameAsTitleForSingle;
        Opacity = 100;
        Dock = DockSides.None;
        DockEnabled = DockSides.None;
        Size = new Size(500, 400);
        MinimumSize = new Size(250, 200);
        Initialize();
    }

    public string CurrentPaletteName => _currentPalette == null ? string.Empty : _currentPalette.Name;

    private void Initialize()
    {
        AddVisual("StringMasterControl", new StringCogoPoints());
        PaletteActivated += MyPaletteSet_PaletteActivated;
        Activate(0);
        _currentPalette = this[0];
    }

    private void MyPaletteSet_PaletteActivated(object sender, PaletteActivatedEventArgs e)
    {
        _currentPalette = e.Activated;
    }

    protected override void Dispose(bool A_0)
    {
        PaletteActivated -= MyPaletteSet_PaletteActivated;
        base.Dispose(A_0);
    }
}
