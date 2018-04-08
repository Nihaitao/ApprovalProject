using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalProject.Model.ViewModel
{
    public class SelectApprovalModel :PageModel
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public int ApprovalStatus { get; set; }
        public int operationStatus { get; set; }

        public int DeptermentID { get;set; }

        public string Name { get; set; }
        public int AddPerson { get; set; }

        public int ApprovalType_ID { get; set; }
    }
}
