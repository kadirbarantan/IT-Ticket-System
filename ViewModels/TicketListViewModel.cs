using ITTicketSystem.Data;
using ITTicketSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ITTicketSystem.ViewModels
{
    internal class TicketListViewModel : BaseViewModel
    {
        public ObservableCollection<Ticket> Tickets { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; } = new();
        public ObservableCollection<Employee> Employees { get; set; } = new();
        public ObservableCollection<Status> Statuses { get; set; } = new();

        // --- Form alanları ---
        private string _newTitle = string.Empty;
        public string NewTitle
        {
            get => _newTitle;
            set => SetProperty(ref _newTitle, value);
        }

        private string _newDescription = string.Empty;
        public string NewDescription
        {
            get => _newDescription;
            set => SetProperty(ref _newDescription, value);
        }

        private Category? _selectedCategory;
        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        private Employee? _selectedEmployee;
        public Employee? SelectedEmployee
        {
            get => _selectedEmployee;
            set => SetProperty(ref _selectedEmployee, value);
        }

        private Status? _selectedStatus;
        public Status? SelectedStatus
        {
            get => _selectedStatus;
            set => SetProperty(ref _selectedStatus, value);
        }

        private string _newPriority = "Normal";
        public string NewPriority
        {
            get => _newPriority;
            set => SetProperty(ref _newPriority, value);
        }

        // Şu an düzenlenen talep (null ise "yeni talep" modundayız)
        private Ticket? _editingTicket;
        public Ticket? EditingTicket
        {
            get => _editingTicket;
            set => SetProperty(ref _editingTicket, value);
        }

        // Ekran başlığı ve buton yazısı moda göre değişsin
        public string FormHeader => EditingTicket == null ? "Yeni Talep" : "Talebi Düzenle";
        public string SaveButtonText => EditingTicket == null ? "Kaydet" : "Güncelle";

        public ICommand SaveCommand { get; }
        public ICommand NewTicketCommand { get; }
        public ICommand EditTicketCommand { get; }
        public ICommand DeleteCommand { get; }

        public TicketListViewModel()
        {
            SaveCommand = new RelayCommand(_ => SaveTicket(), _ => !string.IsNullOrWhiteSpace(NewTitle) && SelectedCategory != null);
            NewTicketCommand = new RelayCommand(_ => ResetForm());
            EditTicketCommand = new RelayCommand(param =>
            {
                if (param is Ticket selectedTicket)
                {
                    LoadTicketForEdit(selectedTicket);
                }
            });
            DeleteCommand = new RelayCommand(_ => DeleteTicket(), _ => EditingTicket != null);
            LoadTickets();
            LoadCategories();
            LoadEmployees();
            LoadStatuses();
        }

        public void LoadTickets()
        {
            using var context = new AppDbContext();

            var ticketsFromDb = context.Tickets
                .Include(t => t.Category)
                .Include(t => t.Status)
                .Include(t => t.Employee)
                .ToList();

            Tickets.Clear();
            foreach (var ticket in ticketsFromDb)
            {
                Tickets.Add(ticket);
            }
        }

        private void LoadCategories()
        {
            using var context = new AppDbContext();
            foreach (var category in context.Categories.ToList())
            {
                Categories.Add(category);
            }
        }

        private void LoadEmployees()
        {
            using var context = new AppDbContext();
            foreach (var employee in context.Employees.ToList())
            {
                Employees.Add(employee);
            }
        }

        private void LoadStatuses()
        {
            using var context = new AppDbContext();
            foreach (var status in context.Statuses.ToList())
            {
                Statuses.Add(status);
            }
        }

        public void LoadTicketForEdit(Ticket ticket)
        {
            EditingTicket = ticket;
            NewTitle = ticket.Title;
            NewDescription = ticket.Description;
            NewPriority = ticket.Priority;
            SelectedCategory = Categories.FirstOrDefault(c => c.Id == ticket.CategoryId);
            SelectedEmployee = Employees.FirstOrDefault(e => e.Id == ticket.EmployeeId);
            SelectedStatus = Statuses.FirstOrDefault(s => s.Id == ticket.StatusId);

            OnPropertyChanged(nameof(FormHeader));
            OnPropertyChanged(nameof(SaveButtonText));
        }

        private void ResetForm()
        {
            EditingTicket = null;
            NewTitle = string.Empty;
            NewDescription = string.Empty;
            SelectedCategory = null;
            SelectedEmployee = null;
            SelectedStatus = null;
            NewPriority = "Normal";

            OnPropertyChanged(nameof(FormHeader));
            OnPropertyChanged(nameof(SaveButtonText));
        }

        private void SaveTicket()
        {
            using var context = new AppDbContext();

            if (EditingTicket == null)
            {
                // YENİ TALEP
                var openStatus = context.Statuses.First(s => s.Name == "Açık");

                var newTicket = new Ticket
                {
                    Title = NewTitle,
                    Description = NewDescription,
                    Priority = NewPriority,
                    CategoryId = SelectedCategory!.Id,
                    EmployeeId = SelectedEmployee?.Id,
                    StatusId = openStatus.Id,
                    CreatedDate = DateTime.Now
                };

                context.Tickets.Add(newTicket);
            }
            else
            {
                // GÜNCELLEME
                var ticketInDb = context.Tickets.First(t => t.Id == EditingTicket.Id);

                ticketInDb.Title = NewTitle;
                ticketInDb.Description = NewDescription;
                ticketInDb.Priority = NewPriority;
                ticketInDb.CategoryId = SelectedCategory!.Id;
                ticketInDb.EmployeeId = SelectedEmployee?.Id;
                ticketInDb.StatusId = SelectedStatus?.Id ?? ticketInDb.StatusId;

                context.Tickets.Update(ticketInDb);
            }

            context.SaveChanges();
            ResetForm();
            LoadTickets();
        }
        private void DeleteTicket()
        {
            if (EditingTicket == null) return;

            var result = System.Windows.MessageBox.Show(
                $"\"{EditingTicket.Title}\" talebini silmek istediğine emin misin?",
                "Silme Onayı",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result != System.Windows.MessageBoxResult.Yes) return;

            using var context = new AppDbContext();
            var ticketInDb = context.Tickets.First(t => t.Id == EditingTicket.Id);
            context.Tickets.Remove(ticketInDb);
            context.SaveChanges();

            ResetForm();
            LoadTickets();
        }
    }
}