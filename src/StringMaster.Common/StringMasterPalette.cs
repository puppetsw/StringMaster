﻿using System;
using System.Drawing;
using Autodesk.AutoCAD.Windows;
using StringMaster.Common.Helpers;
using StringMaster.Common.Services.Implementation;
using StringMaster.UI.Palettes;
using StringMaster.UI.Services.Interfaces;
using StringMaster.UI.ViewModels;

namespace StringMaster.Common;

public class StringMasterPalette : PaletteSet
{
    private readonly IStringCivilPointsService _stringCivilPointsService;
    private readonly bool _isCivil;
    private Palette _currentPalette;

    private StringCogoPointsView _stringCogoPointsView;

    public StringMasterPalette(IStringCivilPointsService stringCivilPointsService, bool isCivil = false)
        : base("StringMasterPalette", new Guid("B56213B2-F7C0-499F-A3F3-A5A5EC631DA2"))
    {
        _stringCivilPointsService = stringCivilPointsService;
        _isCivil = isCivil;
        Style = PaletteSetStyles.ShowAutoHideButton | PaletteSetStyles.ShowCloseButton | PaletteSetStyles.Snappable |
                PaletteSetStyles.UsePaletteNameAsTitleForSingle | PaletteSetStyles.ShowPropertiesMenu;
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
        // BUG: Probably some memory leak bug here if we had more than one palette?
        // TODO: Inject services to main view model. Use DI?

        var viewModel = new StringCogoPointsViewModel(
            new OpenDialogService(),
            new SaveDialogService(),
            new MessageBoxService(),
            new DialogService(),
            new ImportService(),
            _stringCivilPointsService,
            new AcadApplicationService(),
            new AcadColorDialogService(),
            new AcadLayerService(),
            new AcadLinetypeDialogService(),
            new AcadLineweightDialogService())
        {
            IsCivil = _isCivil
        };

        _stringCogoPointsView = new StringCogoPointsView(viewModel);
        _stringCogoPointsView.Background = ColorHelpers.GetBackgroundColor();
        _stringCogoPointsView.DismissPaletteEvent += DismissPalette;

        AddVisual("StringMaster", _stringCogoPointsView);
        PaletteActivated += MyPaletteSet_PaletteActivated;
        Activate(0);
        _currentPalette = this[0];
    }

    private void DismissPalette(object sender, EventArgs e)
    {
        _currentPalette.PaletteSet.Visible = false;
    }

    private void MyPaletteSet_PaletteActivated(object sender, PaletteActivatedEventArgs e)
    {
        _currentPalette = e.Activated;
    }

    protected override void Dispose(bool dispose)
    {
        _stringCogoPointsView.DismissPaletteEvent -= DismissPalette;
        PaletteActivated -= MyPaletteSet_PaletteActivated;
        base.Dispose(dispose);
    }
}
