﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalProject.Model.ViewModel
{
    /// <summary>
    /// 审批流程规则对象
    /// </summary>
    public class ApprovalTypeModel
    {
        public int Approval_ID { get; set; }
        public List<ApprovalVisrange> Visrange { get; set; }
        public List<ApprovalCondition> Condition { get; set; }
    }
    /// <summary>
    /// 可见范围
    /// </summary>
    public class ApprovalVisrange
    {
        public int ApprovalType_ID { get; set; }
        /// <summary>
        /// 0 部门 1 员工  2 公司
        /// </summary>
        public int IsAccount { get; set; }

        /// <summary>
        /// 部门或员工ID
        /// </summary>
        public int DepartmentID { get; set; }

        public string Name { get; set; }
    }
    /// <summary>
    /// 条件审批
    /// </summary>
    public class ApprovalCondition
    {
        public int ID { get; set; }
        public int ApprovalType_ID { get; set; }
        public int ConditionType { get; set; }
        public int FieldId { get; set; }
        public string ConditionOp { get; set; }
        public string ContrastValue { get; set; }
        public string DeptId { get; set; }
        public string AccId { get; set; }
        public int Priority { get; set; }
        public List<ApprovalConditionObject> ApprovalConditionObject { get; set; }
        public List<ApprovalConditionObject> ApprovalConditionCC { get; set; }
        
        public string FieldName { get; set; }
        public string Names { get; set; }
    }

    /// <summary>
    /// 审批人
    /// </summary>
    public class ApprovalConditionObject
    {
        public int ApprovalCondition_ID { get; set; }
        public int CounterSign { get; set; }
        public int Type { get; set; }
        public int AccId { get; set; }
        public string AccIds { get; set; }
        public string AccName { get; set; }
        public int Priority { get; set; }
    }

    public class Customfield
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string FieldName { get; set; }
        public int FieldType { get; set; }
        public string DataSource { get; set; }
        public string DataSourceCode { get; set; }
        public int Required { get; set; }
        public int PID { get; set; }
        public string Value { get; set; }
        public List<Customfield> Detail { get; set; }
    }


    public class Department
    {
        public int Department_ID { get; set; }
        public int Account_ID { get; set; }
        public string AccountName { get; set; }
        public int IsMain { get; set; }
        public int IsManager { get; set; }
    }
    public class UserApproval
    {
        public int EditApprover { get; set; } 
        public List<Customfield> Field { get; set; }
        public List<ApprovalCondition> Condition { get; set; }
    }

    public class FinanceModel
    {
        public List<Sp_FinanceType> list { get; set; }
    }

}
