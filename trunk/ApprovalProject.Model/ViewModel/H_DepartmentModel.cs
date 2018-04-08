using ApprovalProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRProject.Model.ViewModel
{
    public class H_DepartmentModel : H_Department
    {
        /// <summary>
        /// 父节点名称 
        /// </summary>
        public string CName { get; set; }

        /// <summary>
        /// 站点名称
        /// </summary>
        public string StationName { get; set; }

        /// <summary>
        /// 是否存在子节点
        /// </summary>
        public int IsExistChild { get; set; }


        /// <summary>
        /// 关系，是否是主要部门 1 主部门 0 从部门
        /// </summary>
        public int IsMain { set; get; }

        /// <summary>
        /// 是否为管理者 1 部门负责人 0 非部门负责人
        /// </summary>
        public int IsManager { set; get; }

        /// <summary>
        /// 部门现已编制的人数
        /// </summary>
        public int ExistOrganization { set; get; }

        /// <summary>
        /// 是否为部门 1 个人  0 部门
        /// </summary>
        public int IsAccount { set; get; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int Account_ID { set; get; }


    }
}
