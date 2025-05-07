using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CanvasCertificateGenerator;

public partial class PasswordPrompt : Window
{
    public PasswordPrompt()
    {
        InitializeComponent();
    }

    private void OnSubmitClick(object? sender, RoutedEventArgs e)
    {
        MainWindow.EmailPassword = passwordBox.Text;
        Close();
    }
}