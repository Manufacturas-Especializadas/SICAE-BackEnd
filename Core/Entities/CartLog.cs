using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{

    public enum CartSize { Large, Small }

    public enum CartStatus { InPlant, Completed }

    public class CartLog
    {
        public int Id { get; set; }

        public string Folio { get; set; } = string.Empty;

        public CartSize Type { get; set; }

        public DateTime EntryDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExitDate { get; set; }

        public CartStatus Status { get; set; } = CartStatus.InPlant;
    }
}