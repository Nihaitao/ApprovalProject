using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalProject.Model
{
    /// <summary>
    /// 审批流程可见范围表
    /// </summary>
    public class Sp_Approvalvisrange
    {
        [AutoIncrement]
        [PrimaryKey]
        public int ID { get; set; }
        /// <summary>
        /// 审批模板ID
        /// </summary>
        public int ApprovalType_ID { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 数据类型 1员工 、2 部门 、 3公司
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 数据类型ID
        /// </summary>
        public int TypeID { get; set; }

    }
}
