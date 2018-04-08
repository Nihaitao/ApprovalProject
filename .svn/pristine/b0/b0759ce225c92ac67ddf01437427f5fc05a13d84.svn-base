using ApprovalProject.Model.ViewModel;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalProject.Model.ViewModel
{
    public class Sp_ApprovallistModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 审批类型
        /// </summary>
        public int ApprovalType_ID { get; set; }
        /// <summary>
        /// 机构ID
        /// </summary>
        public int System_Station_ID { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public int AddPerson { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime FinishTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int ApprovalStatus { get; set; }

        /// <summary>
        /// 审批最大深度
        /// </summary>
        public int MaxDeepth { get; set; }
        /// <summary>
        /// 当前深度
        /// </summary>
        public int CurrentDeepth { get; set; }
        public string ApprovalTypeName { get; set; }
        public string AddPersonName { get; set; }
        /// <summary>
        /// 当前审批方式
        /// </summary>
        public int CurrentCounterSign { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public int operationStatus { get; set; }

        public string ApprovalContent { get; set; }
        public string ShowDetailOne { get; set; }
        public string ShowDetailTwo { get; set; }
        public string ShowDetailThree { get; set; }


        public DepartmentData DepartData { set; get; }


    }
    public class DepartmentData
    {
        /// <summary>
        /// 用户所属部门信息
        /// </summary>
        public dynamic Departments { set; get; }

        /// <summary>
        /// 用户部门信息集
        /// </summary>
        public dynamic Data { set; get; }
    }

}
