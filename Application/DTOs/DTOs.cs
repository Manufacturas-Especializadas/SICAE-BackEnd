using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class DTOs
    {

        public record CartEntryRequest(string Folio, string Type);

        public record CartResponse(
            int id, 
            string Folio, 
            string Type, 
            DateTime EntryDate,
            DateTime? ExitDate,
            string Status
        );

        public record CartEntryDto(
            string Folio, int CartTypeId    
        );
    }
}