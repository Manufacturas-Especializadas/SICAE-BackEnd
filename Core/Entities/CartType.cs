using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CartType
    {
        public int Id { get; set; }

        public string TypeName { get; set; } = string.Empty;

        public virtual ICollection<CartLog> CartLogs { get; set; } = new List<CartLog>();
    }
}
