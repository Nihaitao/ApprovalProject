using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalProject.Model
{
    /// <summary>
    /// 部门表
    /// </summary>
    public class H_Department
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        [AutoIncrement]
        [PrimaryKey]
        public int ID { get; set; }

        /// <summary>
        /// 父ID
        /// </summary>
        public int? CID { get; set; }

        /// <summary>
        /// 公司ID
        /// </summary>
        public int Company_ID { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 添加账号
        /// </summary>
        public int AddPerson { get; set; }

        /// <summary>
        /// 是否是总经办 1是 0不是 默认为0
        /// </summary>
        public int IsGeneralManager { get; set; }

        /// <summary>
        /// 部门编制   0不限制人数
        /// </summary>
        public int Organization { get; set; }
    }
}
