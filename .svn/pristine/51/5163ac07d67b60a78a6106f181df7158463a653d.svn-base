using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalProject.Model
{
    /// <summary>
    /// 转审表
    /// </summary>
    public class Sp_Approvaltransfer
    {
        [AutoIncrement]
        [PrimaryKey]
        public int ID { get; set; }
        /// <summary>
        /// 审批数据ID
        /// </summary>
        public int ApprovalList_ID { get; set; }
        /// <summary>
        /// 审批流程ID
        /// </summary>
        public int ApprovalProcess_ID { get; set; }
        /// <summary>
        /// 接收者
        /// </summary>
        public int AccID { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int operationStatus { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime operationTime { get; set; }
        /// <summary>
        /// 意见
        /// </summary>
        public string Remark { get; set; }

    }
}
