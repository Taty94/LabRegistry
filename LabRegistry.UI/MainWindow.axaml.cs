using Avalonia.Controls;
using Avalonia.Interactivity;
using LabRegistry.UI.Views;

namespace LabRegistry.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void EquipmentButton_Click(object? sender, RoutedEventArgs e)
    {
        // Instanciar y mostrar la ventana de Equipos
        var equipmentView = new EquipmentView();
        equipmentView.Show();
    }

    private void StudentsButton_Click(object? sender, RoutedEventArgs e)
    {
        // Instanciar y mostrar la ventana de Equipos
        var studentView = new StudentView();
        studentView.Show();
    }

    private void LoansButton_Click(object? sender, RoutedEventArgs e)
    {
        // Instanciar y mostrar la ventana de Equipos
        var loanView = new LoanView();
        loanView.Show();
    }
}