﻿using ApprovalProject.Model.ViewModel;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalProject.Model
{
    public class Sp_Approvallist
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [AutoIncrement]
        [PrimaryKey]
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
        /// 内容
        /// </summary>
        public string ApprovalContent { get; set; }

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
        /// <summary>
        /// 当前审批方式
        /// </summary>
        public int CurrentCounterSign { get; set; }

        /// <summary>
        /// 报销凭证ID
        /// </summary>
        public string VoucherID { get; set; }
    }
}