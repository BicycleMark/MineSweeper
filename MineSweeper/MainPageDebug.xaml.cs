using Microsoft.Maui.Dispatching;
using MineSweeper.ViewModels;
using MineSweeper.Views.Controls;
using MineSweeper.Models;

namespace MineSweeper;

public partial class MainPageDebug : ContentPage
{
   
    private readonly ILogger _logger = new CustomDebugLogger();
    
   
    
    // Constructor without ViewModel for initial testing
    public MainPageDebug()
    {
        try
        {
           
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MainPageDebug: Exception in basic constructor: {ex}");
            throw; // Rethrow to see the error
        }
    }
    
   
    
    // Navigation to Main Page is now handled through the flyout menu
}
