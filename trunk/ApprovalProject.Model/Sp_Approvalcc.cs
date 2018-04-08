using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalProject.Model
{
    /// <summary>
    /// 审批条件抄送对象
    /// </summary>
    public class Sp_Approvalcc
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [AutoIncrement]
        [PrimaryKey]
        
        public int ID { get; set; }
        /// <summary>
        /// 审批条件ID
        /// </summary>
        public int ApprovalCondition_ID { get; set; }
        /// <summary>
        /// 抄送类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 账号ID
        /// </summary>
        public int AccId { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

    }
}
