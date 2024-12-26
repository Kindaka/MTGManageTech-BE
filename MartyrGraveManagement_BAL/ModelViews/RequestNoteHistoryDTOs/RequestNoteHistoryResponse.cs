using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.RequestNoteHistoryDTOs
{
    public class RequestNoteHistoryResponse
    {
        public int NoteId { get; set; }
        public int RequestId { get; set; }
        public int AccountId { get; set; }
        public string Note { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public bool Status { get; set; }
    }
}
