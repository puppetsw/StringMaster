using System;
using System.Windows;
using Autodesk.AutoCAD.Windows;
using StringMaster.Common.Helpers;
using StringMaster.Common.Services.Implementation;
using StringMaster.UI.Palettes;
using StringMaster.UI.Services.Interfaces;
using StringMaster.UI.ViewModels;
using Size = System.Drawing.Size;

namespace StringMaster.Common;

public class StringMasterPalette : PaletteSet
{
    private readonly IStringCivilPointsService _stringCivilPointsService;
    private readonly bool _isCivil;
    private Palette _currentPalette;

    private StringCogoPointsView _stringCogoPointsView;
    private StringCogoPointsViewModel _viewModel;

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

        _viewModel = new StringCogoPointsViewModel(
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
            new AcadLineweightDialogService(),
            new CivilPointGroupService());

        _stringCogoPointsView = new StringCogoPointsView(_viewModel);
        _stringCogoPointsView.Background = ColorHelpers.GetBackgroundColor();
        _stringCogoPointsView.DismissPaletteEvent += DismissPalette;
        _stringCogoPointsView.IsVisibleChanged += VisibleChanged;

        AddVisual("StringMaster", _stringCogoPointsView);
        PaletteActivated += MyPaletteSet_PaletteActivated;

        Activate(0);
        _currentPalette = this[0];
    }

    public void ShowPalette()
    {
        _currentPalette.PaletteSet.Visible = true;
        _stringCogoPointsView.Visibility = Visibility.Visible;
    }

    private void DismissPalette(object sender, EventArgs e)
    {
        _currentPalette.PaletteSet.Visible = false;
        _stringCogoPointsView.Visibility = Visibility.Hidden;
    }

    private void MyPaletteSet_PaletteActivated(object sender, PaletteActivatedEventArgs e)
    {
        _currentPalette = e.Activated;
    }

    private void VisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (_stringCogoPointsView == null)
        {
            return;
        }

        if (!_stringCogoPointsView.IsVisible)
        {
            return;
        }

        if (Count <= 0)
        {
            return;
        }

        // Update the point groups before showing view.
        _viewModel.GetPointGroups();

        Activate(0);
        _currentPalette = this[0];
    }

    protected override void Dispose(bool dispose)
    {
        if (_stringCogoPointsView != null)
        {
            _stringCogoPointsView.DismissPaletteEvent -= DismissPalette;
            _stringCogoPointsView.IsVisibleChanged -= VisibleChanged;
        }

        PaletteActivated -= MyPaletteSet_PaletteActivated;
        base.Dispose(dispose);
    }
}
