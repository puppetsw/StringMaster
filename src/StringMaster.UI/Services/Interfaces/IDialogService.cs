﻿using System.ComponentModel;

namespace StringMaster.UI.Services.Interfaces;

public interface IDialogService
{
    /// <summary>
    /// Gets appropriate dialog with associated <paramref name="viewModel"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The type of view model.</typeparam>
    /// <param name="viewModel">The view model of the dialog.</param>
    /// <returns>A new instance of <see cref="IDialog{TViewModel}"/> with associated <paramref name="viewModel"/>.</returns>
    IDialog<TViewModel> GetDialog<TViewModel>(TViewModel viewModel) where TViewModel : class, INotifyPropertyChanged;

    bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : class, INotifyPropertyChanged;
}