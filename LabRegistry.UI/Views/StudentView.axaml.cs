using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LabRegistry.Data;
using LabRegistry.Models;
using LabRegistry.UI.Controllers;
using Microsoft.EntityFrameworkCore;

namespace LabRegistry.UI.Views;

public partial class StudentView : Window
{
    private readonly StudentController _controller;
    private Student? _selectedStudent;
    public StudentView()
    {
        InitializeComponent();
        string connectionString = "Host=localhost;Port=5432;Database=lab_registry;Username=postgres;Password=admin";
        var optionsBuilder = new DbContextOptionsBuilder<LabRegistryDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        var context = new LabRegistryDbContext(optionsBuilder.Options);

        _controller = new StudentController(context);

        LoadStudents();
    }

    private void LoadStudents()
    {
        List<Student> students = _controller.GetAll();
        var dataGrid = this.FindControl<DataGrid>("StudentsDataGrid");
        dataGrid!.ItemsSource = students;
    }

    private void StudentsDataGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var dataGrid = sender as DataGrid;
        _selectedStudent = dataGrid?.SelectedItem as Student;
        if (_selectedStudent is not null)
        {
            this.FindControl<TextBox>("NameTextBox")!.Text = _selectedStudent.Name;
            this.FindControl<TextBox>("LastNameTextBox")!.Text = _selectedStudent.LastName;
            this.FindControl<TextBox>("EmailTextBox")!.Text = _selectedStudent.Email;
        }
    }

    private void NewButton_Click(object? sender, RoutedEventArgs e)
    {
        _selectedStudent = null;
        this.FindControl<TextBox>("EmailTextBox")!.Text = string.Empty;
        this.FindControl<TextBox>("NameTextBox")!.Text = string.Empty;
        this.FindControl<TextBox>("LastNameTextBox")!.Text= string.Empty;
    }

    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        try
            {
                string email = this.FindControl<TextBox>("EmailTextBox")!.Text ?? string.Empty;
                string name = this.FindControl<TextBox>("NameTextBox")!.Text ?? string.Empty;
                string lastName = this.FindControl<TextBox>("LastNameTextBox")!.Text ?? string.Empty;

                if (_selectedStudent is null)
                {
                    // ----- CREATE -----
                    var newStudent = new Student
                    {
                        Name = name,
                        LastName= lastName,
                        Email = email
                    };
                    _controller.Add(newStudent);
                }
                else
                {
                    // ----- UPDATE -----
                    _selectedStudent.Name = name;
                    _selectedStudent.LastName = lastName;
                    _selectedStudent.Email = email;
                    _controller.Update(_selectedStudent);
                }

                LoadStudents();
                NewButton_Click(sender, e);
            }
            catch (Exception ex)
            {
                string completeMessage = ex.Message;
                if (ex.InnerException is not null)
                {
                    completeMessage += " | INNER: " + ex.InnerException.Message;
                }
                Console.WriteLine($"Error al guardar: {completeMessage}");
            }
    }

    private void DeleteButton_Click(object? sender, RoutedEventArgs e)
        {
            if(_selectedStudent is not null)
            {
                _controller.Delete(_selectedStudent.Id);
                LoadStudents();
                NewButton_Click(sender, e);
            }
        }
}