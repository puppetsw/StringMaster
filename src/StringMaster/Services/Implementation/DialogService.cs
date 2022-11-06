using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using StringMaster.UI.Dialogs;
using StringMaster.UI.Services.Interfaces;
using StringMaster.UI.ViewModels;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace StringMaster.Services.Implementation;

public class DialogService : IDialogService
{
    private readonly IReadOnlyDictionary<Type, Func<Window>> _dialogs;

    public DialogService()
    {
        _dialogs = new Dictionary<Type, Func<Window>>
        {
            { typeof(LayerSelectDialogViewModel), () => new LayerSelectDialog() },
            { typeof(LayerCreateDialogViewModel), () => new LayerCreateDialog() }
        };
    }

    public IDialog<TViewModel> GetDialog<TViewModel>(TViewModel viewModel) where TViewModel : class, INotifyPropertyChanged
    {
        if (!_dialogs.TryGetValue(typeof(TViewModel), out var initializer))
            throw new ArgumentException($"{typeof(TViewModel)} does not have an appropriate dialog associated with it.");

        var contentDialog = initializer();
        if (contentDialog is not IDialog<TViewModel> dialog)
            throw new NotSupportedException($"The dialog does not implement {typeof(IDialog<TViewModel>)}.");

        dialog.ViewModel = viewModel;

        return dialog;
    }

    public bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : class, INotifyPropertyChanged
    {
        try
        {
            var dialog = GetDialog(viewModel);
            return Application.ShowModalWindow(dialog as Window);
        }
        catch (Exception e)
        {
            AcadApplicationService.Editor.WriteMessage($"StringMaster exception: {e.Message}");
            throw;
        }
    }
}