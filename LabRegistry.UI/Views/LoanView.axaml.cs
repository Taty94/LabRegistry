using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LabRegistry.Data;
using LabRegistry.Models;
using LabRegistry.UI.Controllers;
using Microsoft.EntityFrameworkCore;

namespace LabRegistry.UI.Views;

public partial class LoanView : Window
{
    private readonly LoanController _controller;
    private readonly EquipmentController _equipmentController; // Para llenar el ComboBox de eventos
    private readonly StudentController _studenttController; // Para llenar el ComboBox de clientes
    private Loan? _selectedLoan;
    public LoanView()
    {
        InitializeComponent();
        string connectionString = "Host=localhost;Port=5432;Database=lab_registry;Username=postgres;Password=admin";
        var optionsBuilder = new DbContextOptionsBuilder<LabRegistryDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        var context = new LabRegistryDbContext(optionsBuilder.Options);

        _controller= new LoanController(context);
        _equipmentController= new EquipmentController(context);
        _studenttController= new StudentController(context);

        LoadCombos();
        LoadLoans();
        
    }

    private void LoadCombos()
    {
        var equipmentComboBox = this.FindControl<ComboBox>("EquipmentComboBox");
        equipmentComboBox!.ItemsSource = _equipmentController.GetAll();
        equipmentComboBox.DisplayMemberBinding = new Avalonia.Data.Binding("Name"); // Muestra el nombre, no el objeto completo

        var studentComboBox = this.FindControl<ComboBox>("StudentComboBox");
        studentComboBox!.ItemsSource = _studenttController.GetAll();
        studentComboBox.DisplayMemberBinding = new Avalonia.Data.Binding("Name");

        var statusComboBox = this.FindControl<ComboBox>("StatusComboBox");
        statusComboBox!.ItemsSource = Enum.GetValues(typeof(LoanStatus));
    }

    private void LoadLoans()
    {
        List<Loan> loans = _controller.GetAll();
        var dataGrid = this.FindControl<DataGrid>("LoansDataGrid");
        dataGrid!.ItemsSource = loans;
    }

    private void LoansDataGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var dataGrid = sender as DataGrid;
        _selectedLoan = dataGrid?.SelectedItem as Loan;

        if (_selectedLoan is not null)
        {
            var equipmentComboBox = this.FindControl<ComboBox>("EquipmentComboBox");
            equipmentComboBox!.SelectedItem = (equipmentComboBox.ItemsSource as List<Equipment>)?
                    .FirstOrDefault(e => e.Id == _selectedLoan.EquipmentId);

            var studentComboBox = this.FindControl<ComboBox>("StudentComboBox");
            studentComboBox!.SelectedItem = (studentComboBox.ItemsSource as List<Student>)?
                .FirstOrDefault(s => s.Id == _selectedLoan.StudentId);

            this.FindControl<DatePicker>("LoanDatePicker")!.SelectedDate = new DateTimeOffset(_selectedLoan.LoanDate);
            this.FindControl<ComboBox>("StatusComboBox")!.SelectedItem = _selectedLoan.Status;
        }
    }

    private void NewButton_Click(object? sender, RoutedEventArgs e)
    {
        _selectedLoan = null;

        this.FindControl<ComboBox>("StudentComboBox")!.SelectedIndex = -1;
        this.FindControl<ComboBox>("EquipmentComboBox")!.SelectedIndex = -1;
        this.FindControl<ComboBox>("StatusComboBox")!.SelectedIndex = 0;
        
        var datePicker = this.FindControl<DatePicker>("LoanDatePicker");
        datePicker!.SelectedDate = DateTimeOffset.Now;

        var dataGrid = this.FindControl<DataGrid>("LoansDataGrid");
        dataGrid!.SelectedItem = null;
    }

    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var selectedEquipment = this.FindControl<ComboBox>("EquipmentComboBox")!.SelectedItem as Equipment;
            var selectedStudent = this.FindControl<ComboBox>("StudentComboBox")!.SelectedItem as Student;
            var datePicker = this.FindControl<DatePicker>("LoanDatePicker");
            var statusComboBox = this.FindControl<ComboBox>("StatusComboBox");

            // Validaciones básicas
            if (selectedEquipment is null)
            {
                Console.WriteLine("Debe seleccionar un equipo.");
                return;
            }

            if (selectedEquipment is null)
            {
                Console.WriteLine("Debe seleccionar un estudiante.");
                return;
            }


            DateTime loanDate = datePicker?.SelectedDate?.DateTime ?? DateTime.UtcNow;
            LoanStatus status = statusComboBox != null && statusComboBox.SelectedIndex >= 0
                ? (LoanStatus)statusComboBox.SelectedIndex
                : LoanStatus.Active;

            if (_selectedLoan is null)
            {
                // ----- CREAR PRESTAMO -----
                var newLoan = new Loan
                {
                    EquipmentId = selectedEquipment.Id,
                    StudentId = selectedStudent.Id,
                    LoanDate = loanDate,
                    ExpectedReturnDate = loanDate.AddDays(2),
                };

                _controller.Add(newLoan);
            }
            else
            {
                // ----- ACTUALIZAR PRESTAMO -----
                _selectedLoan.EquipmentId= selectedEquipment.Id;
                _selectedLoan.StudentId= selectedStudent.Id;
                _selectedLoan.LoanDate = loanDate;
                _selectedLoan.ActualReturnDate = DateTime.UtcNow;
                _selectedLoan.Status = status;

                _controller.Update(_selectedLoan);
            }

            LoadLoans();
            NewButton_Click(sender, e);
        }
        catch (Exception ex)
        {
            string completeMessage = ex.Message;
            if (ex.InnerException is not null)
            {
                completeMessage += " | INNER: " + ex.InnerException.Message;
            }
            Console.WriteLine($"Error al guardar el préstamo: {completeMessage}");
        }
    }

    private void DeleteButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_selectedLoan is not null)
        {
            _controller.Delete(_selectedLoan.Id);
            LoadLoans();
            NewButton_Click(sender, e);
        }
    }
}