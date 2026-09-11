using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITTicketSystem.Models
{
    internal class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }= DateTime.Now;
        public string Priority { get; set; } = "Normal"; //Öncelik seviyesi

        //Category İlişkisi
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        //Status İlişkisi
        public int StatusId { get; set; }
        public Status Status { get; set; }

        //Employee İlişkisi
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
    }
}
