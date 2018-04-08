using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalProject.Model.ViewModel
{
    
    public class PaymentCode
    {
        /// <summary>
        /// 科目代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 科目名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 标识符（是不是最后一级）
        /// </summary>
        public int IsEnd { get; set; }
    }

    public class ApprovallistVModel : Sp_Approvalprocess
    {
        public List<PaymentType> Payment { get; set; }
        public List<PaymentType> LeftCode { get; set; }
        public int ReceiptNum { get; set; }
    }

    
    public class PaymentType
    {
        public string Code { get; set; }
        public decimal Money { get; set; }
        public string Time { get; set; }
        public string Mark { get; set; }
    }


    public class VoucherDetails
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public decimal LeftMoney { get; set; }
        public decimal RightMoney { get; set; }
        public int Person_ID { get; set; }
 
    }
}
