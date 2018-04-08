using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalProject.Model
{
    /// <summary>
    /// 会计审批支付选择
    /// </summary>
    public class Sp_Approvalpay
    {
        /// <summary>
        /// 审批数据ID
        /// </summary>
        public int ApprovalList_ID { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Money { get; set; }
    }
}
