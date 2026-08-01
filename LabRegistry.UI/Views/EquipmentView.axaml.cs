using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using LabRegistry.Data;
using LabRegistry.Models;
using LabRegistry.UI.Controllers;
using Microsoft.EntityFrameworkCore;

namespace LabRegistry.UI.Views;

public partial class EquipmentView : Window
{
    private readonly EquipmentController _controller;
    private Equipment? _selectedEquipment;
    public EquipmentView()
    {
        InitializeComponent();

        string connectionString = "Host=localhost;Port=5432;Database=lab_registry;Username=postgres;Password=admin";
        var optionsBuilder = new DbContextOptionsBuilder<LabRegistryDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        var context = new LabRegistryDbContext(optionsBuilder.Options);

        _controller = new EquipmentController(context);
        var categoryComboBox = this.FindControl<ComboBox>("StatusComboBox");
        categoryComboBox!.ItemsSource = Enum.GetValues(typeof(StatusEquipment));

        LoadEquipments();
    }

    private void LoadEquipments()
    {
        List<Equipment> equipments = _controller.GetAll();
        var dataGrid = this.FindControl<DataGrid>("EquipmentsDataGrid");
        dataGrid!.ItemsSource = equipments;
    }

    private void EquipmentsDataGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var dataGrid = sender as DataGrid;
        _selectedEquipment = dataGrid?.SelectedItem as Equipment;
        if (_selectedEquipment is not null)
        {
            this.FindControl<TextBox>("NameTextBox")!.Text = _selectedEquipment.Name;
            this.FindControl<TextBox>("CategoryTextBox")!.Text = _selectedEquipment.Category;
            var statusComboBox = this.FindControl<ComboBox>("StatusComboBox");
            statusComboBox?.SelectedIndex = (int)_selectedEquipment.StatusEquipment;
        }
    }
    
    private void NewButton_Click(object? sender, RoutedEventArgs e)
    {
        _selectedEquipment = null;
        this.FindControl<TextBox>("NameTextBox")!.Text = string.Empty;
        this.FindControl<TextBox>("CategoryTextBox")!.Text = string.Empty;
        this.FindControl<ComboBox>("StatusComboBox")!.SelectedIndex = 0;
        var dataGrid = this.FindControl<DataGrid>("EquipmentsDataGrid");
        dataGrid?.SelectedItem = null;
    }

    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        try
            {
                string name = this.FindControl<TextBox>("NameTextBox")!.Text ?? string.Empty;
                string category = this.FindControl<TextBox>("CategoryTextBox")!.Text ?? string.Empty;
                var statusSelected = this.FindControl<ComboBox>("StatusComboBox")!.SelectedItem;

                if (_selectedEquipment is null)
                {
                    // ----- CREATE -----
                    var newEquipment = new Equipment
                    {
                        Name = name,
                        Category = category,
                    };
                    _controller.Add(newEquipment);
                }
                else
                {
                    if (statusSelected is null)
                    {
                        Console.WriteLine("Debe seleccionar un estado");
                        return;
                    }
                    // ----- UPDATE -----
                    _selectedEquipment.Name = name;
                    _selectedEquipment.Category = category;
                    _selectedEquipment.StatusEquipment = (StatusEquipment)statusSelected;
                    _controller.Update(_selectedEquipment);
                }

                LoadEquipments();
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
            if(_selectedEquipment is not null)
            {
                _controller.Delete(_selectedEquipment.Id);
                LoadEquipments();
                NewButton_Click(sender, e);
            }
        }
}