﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalProject.Model.ViewModel
{
    public class ApprovalTemplet
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string StyleClass { get; set; }
        public int EditApprover { get; set; }             
        public List<TempletField> Field { get; set; }

    }
    
    public class TempletField
    {
        public int fieldId { get; set; }
        public string fieldName { get; set; }
        public int fieldValue { get; set; }
        public int Required { get; set; }
        public TempletJson[] Option { get; set; }
        public List<TempletField> DetailField { get; set; }
        public List<TempletField> Reimbursement { get; set; }

    }

    public class TempletJson
    {
        //public string name { get; set; }
        public string value { get; set; }
    }

}
