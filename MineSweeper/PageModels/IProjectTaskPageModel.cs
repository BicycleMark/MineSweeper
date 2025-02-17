using CommunityToolkit.Mvvm.Input;
using MineSweeper.Models;

namespace MineSweeper.PageModels;

public interface IProjectTaskPageModel
{
    IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
    bool IsBusy { get; }
}