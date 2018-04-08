using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalProject.Model
{
    /// <summary>
    /// 条件审批人
    /// </summary>
    public class Sp_ConditionApprover
    {
        public int ID { get; set; }
        /// <summary>
        /// 条件对象ID
        /// </summary>
        public int ConditionObject_ID { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 账号ID
        /// </summary>
        public int AccID { get; set; }

    }
}
