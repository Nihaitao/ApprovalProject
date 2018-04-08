﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonFramework.Exceptions;
using CommonFramework.IBatisNet;
using ApprovalProject.Model;
using ApprovalProject.Model.ViewModel;
using System.Text.RegularExpressions;

namespace ApprovalProject.Business
{
    public class ApprovalMapper : BaseMapper
    {
        #region 审批模板&发起审批
        /// <summary>
        /// 添加模板
        /// </summary>
        /// <param name="templet"></param>
        /// <param name="System_Station_ID"></param>
        /// <param name="AccountID"></param>
        /// <returns></returns>
        public dynamic AddApprovalTemplet(ApprovalTemplet templet, int System_Station_ID, int AccountID)
        {
            try
            {
                SqlMapper.BeginTransaction();
                Sp_Approvaltype typemodel = Orm.Single<Sp_Approvaltype>(x => x.System_Station_ID == System_Station_ID && x.Name == templet.Name && x.IsDelete == 0);
                if (typemodel != null)
                {
                    throw new ApiException("模板名称已存在，请勿重复添加");
                }

                Sp_Approvaltype model = new Sp_Approvaltype();
                model.Name = templet.Name;
                model.AddTime = DateTime.Now;
                model.AddPersion = AccountID;
                model.System_Station_ID = System_Station_ID;
                model.Enditable = 1;//启用
                model.BusType = 1;//非业务审核
                model.EditApprover = templet.EditApprover;
                model.StyleClass = templet.StyleClass;
                model.ID = (int)Orm.Insert(model, true);
                if (model.ID > 0)
                {
                    //添加模板字段明细
                    H_Customfield item = null;
                    List<TempletField> field = templet.Field;
                    for (int i = 0; i < field.Count; i++)
                    {
                        item = new H_Customfield();
                        item.BusType = "approval_" + model.ID;
                        item.CreateTime = DateTime.Now;
                        item.System_Station_ID = System_Station_ID;
                        item.FieldName = Guid.NewGuid().ToString("N");
                        item.IsEnable = 1;
                        item.Name = field[i].fieldName;
                        item.ShowName = field[i].fieldName;
                        item.Required = field[i].Required;
                        item.FieldType = field[i].fieldValue;
                        item.SortID = i;
                        if (field[i].fieldValue == 103 || field[i].fieldValue == 110)
                        {
                            string dataSource = "";
                            foreach (TempletJson js in field[i].Option)
                                dataSource += js.value + "|";
                            item.DataSource = dataSource.TrimEnd('|');
                        }
                        item.ID = (int)Orm.Insert(item, true);
                        if (item.ID > 0 && (field[i].fieldValue == 0 || field[i].fieldValue == 112))
                        {
                            //添加模板字段明细子集（fieldValue == 112为报销明细）
                            List<TempletField> detail = field[i].fieldValue == 0 ? field[i].DetailField : field[i].Reimbursement;
                            for (int j = 0; j < detail.Count; j++)
                            {
                                H_Customfield item2 = new H_Customfield();
                                item2.BusType = "approval_" + model.ID;
                                item2.CreateTime = DateTime.Now;
                                item2.System_Station_ID = System_Station_ID;
                                item2.FieldName = Guid.NewGuid().ToString("N");
                                item2.IsEnable = 1;
                                item2.Name = detail[j].fieldName;
                                item2.ShowName = detail[j].fieldName;
                                item2.Required = detail[j].Required;
                                item2.FieldType = detail[j].fieldValue;
                                item2.SortID = j;
                                item2.PID = item.ID;
                                if (detail[j].fieldValue == 103 || detail[j].fieldValue == 110)
                                {
                                    string dataSource = "";
                                    if (field[i].fieldValue == 0 || field[i].fieldValue == 112)
                                    {
                                        if (detail[j].Option == null)
                                            throw new ApiException("选项不能为空");
                                        foreach (TempletJson js in detail[j].Option)
                                            dataSource += js.value + "|";
                                    }
                                    item2.DataSource = dataSource.TrimEnd('|');
                                }
                                if (Orm.Insert(item2) < 1)
                                {
                                    throw new ApiException("插入失败");
                                }
                            }
                        }
                    }
                }
                SqlMapper.CommitTransaction();
                return model.ID;
            }
            catch (Exception ex)
            {
                SqlMapper.RollBackTransaction();
                throw new ApiException(ex.Message);
            }
        }

        /// <summary>
        /// 修改模板
        /// </summary>
        /// <param name="editTemplet"></param>
        /// <returns></returns>
        public bool EditApprovalTemplet(ApprovalTemplet templet, int System_Station_ID)
        {
            try
            {
                SqlMapper.BeginTransaction();
                Sp_Approvaltype model = Orm.Single<Sp_Approvaltype>(x => x.ID == templet.ID);
                if (model == null)
                    throw new ApiException("审批模板不存在或者已删除");
                if (templet.Name != model.Name && Orm.Single<Sp_Approvaltype>(x => x.System_Station_ID == System_Station_ID && x.Name == templet.Name && x.IsDelete == 0) != null)
                    throw new ApiException("审批模板名称已存在");
                if (templet.Field == null)
                {
                    throw new ApiException("审批模板字段不能为空");
                }

                model.Name = templet.Name;
                model.EditApprover = templet.EditApprover;
                model.StyleClass = templet.StyleClass;
                Orm.Update(model);

                //编辑模板字段
                List<H_Customfield> list = Orm.Select<H_Customfield>(x => x.BusType == "approval_" + templet.ID);
                List<Sp_Approvalcondition> condition = Orm.Select<Sp_Approvalcondition>(x => x.ApprovalType_ID == templet.ID && x.ConditionType == 2);
                H_Customfield item = null;
                List<TempletField> field = templet.Field;
                for (int i = 0; i < field.Count; i++)
                {
                    if (field[i].fieldId == 0)
                    {//新增
                        item = new H_Customfield();
                        item.BusType = "approval_" + model.ID;
                        item.CreateTime = DateTime.Now;
                        item.System_Station_ID = System_Station_ID;
                        item.FieldName = Guid.NewGuid().ToString("N");
                        item.IsEnable = 1;
                        item.Name = field[i].fieldName;
                        item.ShowName = field[i].fieldName;
                        item.Required = field[i].Required;
                        item.FieldType = field[i].fieldValue;
                        item.SortID = i;
                        if (field[i].fieldValue == 103 || field[i].fieldValue == 110)
                        {
                            string dataSource = "";
                            foreach (TempletJson js in field[i].Option)
                                dataSource += js.value + "|";
                            item.DataSource = dataSource.TrimEnd('|');
                        }
                        item.ID = (int)Orm.Insert(item, true);
                        if (item.ID > 0 && (field[i].fieldValue == 0 || field[i].fieldValue == 112))
                        {
                            //添加模板字段明细子集
                            List<TempletField> detail = field[i].fieldValue == 0 ? field[i].DetailField : field[i].Reimbursement;
                            for (int j = 0; j < detail.Count; j++)
                            {
                                H_Customfield item2 = new H_Customfield();
                                item2.BusType = "approval_" + model.ID;
                                item2.CreateTime = DateTime.Now;
                                item2.System_Station_ID = System_Station_ID;
                                item2.FieldName = Guid.NewGuid().ToString("N");
                                item2.IsEnable = 1;
                                item2.Name = detail[j].fieldName;
                                item2.ShowName = detail[j].fieldName;
                                item2.Required = detail[j].Required;
                                item2.FieldType = detail[j].fieldValue;
                                item2.SortID = j;
                                item2.PID = item.ID;
                                if (detail[j].fieldValue == 103 || detail[j].fieldValue == 110)
                                {
                                    string dataSource = "";
                                    if (field[i].fieldValue == 0 || field[i].fieldValue == 112)
                                    {
                                        if (detail[j].Option == null)
                                            throw new ApiException("选项不能为空");
                                        foreach (TempletJson js in detail[j].Option)
                                            dataSource += js.value + "|";
                                    }
                                    item2.DataSource = dataSource.TrimEnd('|');
                                }
                                if (Orm.Insert(item2) < 1)
                                {
                                    throw new ApiException("插入失败");
                                }
                            }
                        }
                    }
                    else
                    {
                        item = list.FirstOrDefault(x => x.ID == field[i].fieldId);
                        list.Remove(item);
                        bool delFalg = false;
                        if (item.FieldType != field[i].fieldValue)
                            delFalg = true;
                        item.Name = field[i].fieldName;
                        item.ShowName = field[i].fieldName;
                        item.Required = field[i].Required;
                        item.FieldType = field[i].fieldValue;
                        item.SortID = i;
                        if (field[i].fieldValue == 103 || field[i].fieldValue == 110)
                        {
                            string dataSource = "";
                            foreach (TempletJson js in field[i].Option)
                                dataSource += js.value + "|";
                            if (item.DataSource != dataSource.TrimEnd('|'))
                                delFalg = true;
                            item.DataSource = dataSource.TrimEnd('|');
                        }
                        Orm.Update(item);
                        //删除对应审批条件
                        Sp_Approvalcondition co = condition.FirstOrDefault(x => x.FieldId == item.ID);
                        if (co != null && delFalg)
                            Orm.Delete<Sp_Approvalcondition>(co);

                        if (field[i].fieldValue == 0 || field[i].fieldValue == 112)
                        {
                            Orm.Delete<H_Customfield>(x => x.PID == item.ID);//先删除明细
                            //添加模板字段明细子集
                            List<TempletField> detail = field[i].fieldValue == 0 ? field[i].DetailField : field[i].Reimbursement;
                            for (int j = 0; j < detail.Count; j++)
                            {
                                H_Customfield item2 = new H_Customfield();
                                item2.BusType = "approval_" + model.ID;
                                item2.CreateTime = DateTime.Now;
                                item2.System_Station_ID = System_Station_ID;
                                item2.FieldName = Guid.NewGuid().ToString("N");
                                item2.IsEnable = 1;
                                item2.Name = detail[j].fieldName;
                                item2.ShowName = detail[j].fieldName;
                                item2.Required = detail[j].Required;
                                item2.FieldType = detail[j].fieldValue;
                                item2.SortID = j;
                                item2.PID = item.ID;
                                if (detail[j].fieldValue == 103 || detail[j].fieldValue == 110)
                                {
                                    string dataSource = "";
                                    if (field[i].fieldValue == 0 || field[i].fieldValue == 112)
                                    {
                                        if (detail[j].Option == null)
                                            throw new ApiException("选项不能为空");
                                        foreach (TempletJson js in detail[j].Option)
                                            dataSource += js.value + "|";
                                    }
                                    item2.DataSource = dataSource.TrimEnd('|');
                                }
                                if (Orm.Insert(item2) < 1)
                                {
                                    throw new ApiException("插入失败");
                                }
                            }
                        }
                    }
                }
                //删除没有传过来的字段
                if (list.Count > 0)
                    list.ForEach(x =>
                    {
                        Orm.Delete<H_Customfield>(y => y.ID == x.ID || y.PID == x.ID);
                        Orm.Delete<Sp_Approvalcondition>(y => y.ConditionType == 2 && y.FieldId == x.ID);
                    });
                SqlMapper.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                SqlMapper.RollBackTransaction();
                throw new ApiException(ex.Message);
            }
        }

        /// <summary>
        /// 添加审批流程
        /// </summary>
        /// <param name="templet"></param>
        /// <param name="System_Station_ID"></param>
        /// <param name="AccountID"></param>
        /// <returns></returns>
        public bool AddApprovalType(ApprovalTypeModel templet, int System_Station_ID, int AccountID)
        {
            try
            {
                SqlMapper.BeginTransaction();

                Sp_Approvalvisrange RangeModel = new Sp_Approvalvisrange();
                RangeModel.ApprovalType_ID = templet.Approval_ID;
                RangeModel.AddTime = DateTime.Now;
                foreach (var item in templet.Visrange)
                {
                    RangeModel.Type = item.IsAccount;
                    RangeModel.TypeID = item.DepartmentID;
                    if (Orm.Insert(RangeModel) < 1)
                    {
                        throw new ApiException("插入失败");
                    }
                }

                foreach (var item in templet.Condition)
                {
                    Sp_Approvalcondition ditionModel = new Sp_Approvalcondition();
                    ditionModel.ApprovalType_ID = templet.Approval_ID;
                    ditionModel.Priority = item.Priority;
                    ditionModel.ConditionType = item.ConditionType;
                    if (item.ConditionType == 0)//默认条件
                    {
                        ditionModel.Priority = 10000;//优先级最低
                    }
                    else if (item.ConditionType == 1)//申请人
                    {
                        ditionModel.DeptIds = item.DeptId;
                        ditionModel.AccIds = item.AccId;
                    }
                    else//自定义字段
                    {
                        ditionModel.FieldId = item.FieldId;
                        ditionModel.ConditionOp = item.ConditionOp;
                        ditionModel.ContrastValue = item.ContrastValue;
                    }
                    ditionModel.AddTime = DateTime.Now;
                    ditionModel.AddPersion = AccountID;
                    int ditionModelID = (int)Orm.Insert(ditionModel, true);
                    if (ditionModelID > 0)
                    {
                        //审批人
                        if (item.ApprovalConditionObject != null)
                        {

                            for (int i = 0; i < item.ApprovalConditionObject.Count; i++)
                            {
                                Sp_ConditionApproveobject objectmodel = new Sp_ConditionApproveobject();
                                objectmodel.ApprovalCondition_ID = ditionModelID;
                                objectmodel.CounterSign = item.ApprovalConditionObject[i].CounterSign;
                                objectmodel.Type = item.ApprovalConditionObject[i].Type;
                                objectmodel.AccId = item.ApprovalConditionObject[i].AccId;
                                objectmodel.AddTime = DateTime.Now;
                                objectmodel.Priority = i;
                                if (Orm.Insert(objectmodel) < 1)
                                {
                                    throw new ApiException("插入失败");
                                }
                            }
                        }
                        //抄送人
                        if (item.ApprovalConditionCC != null)
                        {
                            foreach (var listCCitem in item.ApprovalConditionCC)
                            {
                                Sp_Approvalcc CCmodel = new Sp_Approvalcc();
                                CCmodel.ApprovalCondition_ID = ditionModelID;
                                CCmodel.AccId = listCCitem.AccId;
                                CCmodel.Type = listCCitem.Type;
                                CCmodel.AddTime = DateTime.Now;
                                if (Orm.Insert(CCmodel) < 1)
                                {
                                    throw new ApiException("插入失败");
                                }
                            }
                        }
                    }
                }
                SqlMapper.CommitTransaction();
                return true;
            }
            catch (Exception)
            {
                SqlMapper.RollBackTransaction();
                return false;
            }
        }
        
        /// <summary>
        /// 修改审批流程
        /// </summary>
        /// <param name="templet"></param>
        /// <param name="System_Station_ID"></param>
        /// <param name="AccountID"></param>
        /// <returns></returns>
        public bool EditApprovalType(ApprovalTypeModel templet, int System_Station_ID, int AccountID)
        {
            try
            {
                SqlMapper.BeginTransaction();
                //修改可见范围
                Orm.Delete<Sp_Approvalvisrange>(x => x.ApprovalType_ID == templet.Approval_ID);
                Sp_Approvalvisrange RangeModel = new Sp_Approvalvisrange();
                RangeModel.ApprovalType_ID = templet.Approval_ID;
                RangeModel.AddTime = DateTime.Now;
                foreach (var item in templet.Visrange)
                {
                    RangeModel.Type = item.IsAccount;
                    RangeModel.TypeID = item.DepartmentID;
                    if (Orm.Insert(RangeModel) < 1)
                    {
                        throw new ApiException("修改失败");
                    }
                }
                //修改条件审批流程
                Orm.Delete<Sp_Approvalcondition>(x => x.ApprovalType_ID == templet.Approval_ID);
                foreach (var item in templet.Condition)
                {
                    Sp_Approvalcondition ditionModel = new Sp_Approvalcondition();
                    ditionModel.ApprovalType_ID = templet.Approval_ID;
                    ditionModel.Priority = item.Priority;
                    ditionModel.ConditionType = item.ConditionType;
                    if (item.ConditionType == 0)//默认条件
                    {
                        ditionModel.Priority = 10000;//优先级最低
                    }
                    else if (item.ConditionType == 1)//申请人
                    {
                        ditionModel.DeptIds = item.DeptId;
                        ditionModel.AccIds = item.AccId;
                    }
                    else//自定义字段
                    {
                        ditionModel.FieldId = item.FieldId;
                        ditionModel.ConditionOp = item.ConditionOp;
                        ditionModel.ContrastValue = item.ContrastValue;
                    }
                    ditionModel.AddTime = DateTime.Now;
                    ditionModel.AddPersion = AccountID;
                    int ditionModelID = (int)Orm.Insert(ditionModel, true);
                    if (ditionModelID > 0)
                    {
                        //审批人
                        if (item.ApprovalConditionObject != null)
                        {

                            for (int i = 0; i < item.ApprovalConditionObject.Count; i++)
                            {
                                Sp_ConditionApproveobject objectmodel = new Sp_ConditionApproveobject();
                                objectmodel.ApprovalCondition_ID = ditionModelID;
                                objectmodel.CounterSign = item.ApprovalConditionObject[i].CounterSign;
                                objectmodel.Type = item.ApprovalConditionObject[i].Type;
                                objectmodel.AccId = item.ApprovalConditionObject[i].AccId;
                                objectmodel.AddTime = DateTime.Now;
                                objectmodel.Priority = i;
                                if (Orm.Insert(objectmodel) < 1)
                                {
                                    throw new ApiException("修改失败");
                                }
                            }
                        }
                        //抄送人
                        if (item.ApprovalConditionCC != null)
                        {
                            foreach (var listCCitem in item.ApprovalConditionCC)
                            {
                                Sp_Approvalcc CCmodel = new Sp_Approvalcc();
                                CCmodel.ApprovalCondition_ID = ditionModelID;
                                CCmodel.AccId = listCCitem.AccId;
                                CCmodel.Type = listCCitem.Type;
                                CCmodel.AddTime = DateTime.Now;
                                if (Orm.Insert(CCmodel) < 1)
                                {
                                    throw new ApiException("修改失败");
                                }
                            }
                        }
                    }
                }
                SqlMapper.CommitTransaction();
                return true;
            }
            catch (Exception)
            {
                SqlMapper.RollBackTransaction();
                return false;
            }
        }

        /// <summary>
        /// 获取发起申请列表 根据可见范围规定列表内容 BY SZF
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IList GetInitiateApplicationList(int AccountID, int System_Station_ID)
        {
            return QueryForList("GetInitiateApplicationList", new { AccountID = AccountID, System_Station_ID = System_Station_ID });
        }

        /// <summary>
        /// 发起审批
        /// </summary>
        /// <param name="approvalTypeID"></param>
        /// <param name="accountId"></param>
        /// <param name="System_Station_ID"></param>
        /// <returns></returns>
        public dynamic GetApplication(int approvalTypeID, int accountId, int System_Station_ID)
        {
            string approvalID = "approval_" + approvalTypeID;
            UserApproval approval = new UserApproval();

            Sp_Approvaltype model = Orm.Single<Sp_Approvaltype>(x => x.ID == approvalTypeID);
            approval.EditApprover = model.EditApprover;
            //第一步，查出模板自定义数据
            List<Customfield> fildeList = SqlMapper.QueryForList<Customfield>("GetApplicationCustomfield", new { approvalID }).ToList();
            List<Customfield> parentList = fildeList.Where(x => x.PID == 0).ToList();

            List<Sp_FinanceType> typeList = Orm.Select<Sp_FinanceType>(x => x.System_Station_ID == System_Station_ID && x.Stationproxy_ID == 0);
            foreach (Customfield item in parentList)
            {
                item.Detail = fildeList.Where(x => x.PID == item.ID).ToList();
                //报销
                if (item.FieldType == 112)
                {
                    string[] Ids = item.Detail[0].DataSource.Split('|');
                    string DataSource = "", DataSourceCode="";
                    foreach (var id in Ids)
                    {
                        Sp_FinanceType t = typeList.FirstOrDefault(x => x.ID.ToString() == id);
                        if (t != null)
                        {
                            DataSource += t.Name + "|";
                            DataSourceCode += t.Name + "[" + t.Code + "]|";
                        }
                    }
                    item.Detail[0].DataSource = DataSource.TrimEnd('|');
                    item.Detail[0].DataSourceCode = DataSourceCode.TrimEnd('|');
                }
            }
            approval.Field = parentList;

            //第二步，查出审批条件
            List<ApprovalCondition> conditionList = SqlMapper.QueryForList<ApprovalCondition>("GetApplicationCondition", new { approvalTypeID }).ToList();
            //如果符合申请人条件，则剔除低于申请人优先级别的所有条件
            ApprovalCondition temp = conditionList.FirstOrDefault(x => x.ConditionType == 1);

            List<Department> departs = SqlMapper.QueryForList<Department>("GetDepartment", new { System_Station_ID }).ToList();//查出站点所有部门人员
            if (temp != null)
            {
                int index = conditionList.IndexOf(temp);
                //1符合申请人
                if (("," + temp.AccId + ",").IndexOf("," + accountId + ",") > -1)
                {
                    temp.Priority = 10000;
                    conditionList.RemoveRange(index + 1, conditionList.Count - index - 1);
                }
                else
                {
                    //2符合申请人部门
                    List<Department> myDeparts = departs.Where(x => x.Account_ID == accountId).ToList();
                    if (myDeparts != null)
                    {
                        foreach (Department item in myDeparts)
                        {
                            if (("," + temp.DeptId + ",").IndexOf("," + item.Department_ID + ",") > -1)
                            {
                                temp.Priority = 10000;
                                conditionList.RemoveRange(index + 1, conditionList.Count - index - 1);
                                break;
                            }
                        }
                    }
                }
            }
            //第三步，查出条件对应的审批人与抄送人
            /****
             *当前解决方案：循环所有条件，再循环审批人/抄送人，将出现的‘上级’匹配相应员工，每出现一次‘上级’匹配一次。缺点：重复匹配，效率低。优点：出现‘上级’才匹配，不出现不匹配，部分情况减少内存消耗。
             *优化方案：先查出当前登录用户所有上级集合，然后根据条件匹配。优点：效率提高。缺点：审批人不包含上级的情况下也进行了匹配，消耗内存。TODO...
            ****/
            List<ApprovalConditionObject> objectList = SqlMapper.QueryForList<ApprovalConditionObject>("GetApprovalConditionObject", null).ToList();
            List<ApprovalConditionObject> ccList = SqlMapper.QueryForList<ApprovalConditionObject>("GetApprovalConditionCC", null).ToList();
            string str = SqlMapper.QueryForObject<string>("GetUpDeparts", new { accountId, System_Station_ID });
            string[] upDeparts = !string.IsNullOrEmpty(str) ? str.Split(',') : new string[0]; //获取我主部门的上级部门ID列表（包含我的部门）
            foreach (ApprovalCondition c in conditionList)//循环审批条件
            {
                List<ApprovalConditionObject> tempObject = objectList.Where(x => x.ApprovalCondition_ID == c.ID).ToList();//审批人
                List<ApprovalConditionObject> approvalConditionObject = new List<ApprovalConditionObject>();
                if (tempObject != null)
                {
                    //循环审批人，把‘上级’角色对应进去
                    PutAccountsIntoACO(approvalConditionObject, tempObject, departs, upDeparts, accountId);
                }
                c.ApprovalConditionObject = approvalConditionObject;

                List<ApprovalConditionObject> tempCC = ccList.Where(x => x.ApprovalCondition_ID == c.ID).ToList();//抄送人
                List<ApprovalConditionObject> approvalConditionCC = new List<ApprovalConditionObject>();
                if (tempCC != null)
                {
                    //循环抄送人，把‘上级’角色对应进去
                    PutAccountsIntoACO(approvalConditionCC, tempCC, departs, upDeparts, accountId);
                }
                c.ApprovalConditionCC = approvalConditionCC;
            }
            approval.Condition = conditionList;
            return approval;
        }

        /// <summary>
        /// ‘上级’对应相应员工
        /// </summary>
        /// <param name="approvalConditionObject"></param>
        /// <param name="tempObject"></param>
        /// <param name="departs"></param>
        /// <param name="upDeparts"></param>
        /// <param name="accountId"></param>
        private void PutAccountsIntoACO(List<ApprovalConditionObject> approvalConditionObject, List<ApprovalConditionObject> tempObject, List<Department> departs, string[] upDeparts, int accountId)
        {
            int level = 0;
            foreach (ApprovalConditionObject aco in tempObject)//循环审批人
            {
                if (aco.Type == 1)//个人
                    approvalConditionObject.Add(aco);
                if (aco.Type == 0) //上级
                {
                    if (level == 0)//直接上级
                    {
                        if (upDeparts.Length > 0)
                        {
                            List<Department> managers = departs.Where(x => x.Department_ID.ToString() == upDeparts[0] && x.IsManager == 1).ToList();//上级部门负责人列表
                            if (managers != null && managers.Count > 0)
                            {
                                Department mySelf = managers.FirstOrDefault(x => x.Account_ID == accountId);
                                //如果用户是部门负责人，则找上级部门负责人
                                if (mySelf != null)
                                {
                                    if (upDeparts.Length > 1)
                                    {
                                        ApprovalConditionObject ob = GetUpAccounts(aco, departs, upDeparts[1], 0);//获取部门负责人
                                        if (ob != null)
                                        {
                                            approvalConditionObject.Add(ob);
                                            level += 2;
                                        }
                                        else
                                            level = -1;//表示上级不存在
                                    }
                                    else
                                        level = -1;//表示上级不存在

                                }
                                else//用户不是部门负责人，则找部门负责人
                                {
                                    if (managers.Count == 1)
                                    {
                                        aco.AccId = managers[0].Account_ID;
                                        aco.AccName = managers[0].AccountName;
                                        approvalConditionObject.Add(aco);
                                        level++;
                                    }
                                    else
                                    {
                                        aco.AccId = 0;
                                        aco.AccIds = string.Join(",", managers.Select(x => x.Account_ID));
                                        aco.AccName = "直接上级";
                                        approvalConditionObject.Add(aco);
                                        level++;
                                    }
                                }
                            }
                        }
                    }
                    else if (level > 0)//其他上级
                    {
                        if (upDeparts.Length > level)
                        {
                            ApprovalConditionObject ob = GetUpAccounts(aco, departs, upDeparts[level], level);
                            if (ob != null)
                            {
                                approvalConditionObject.Add(ob);
                                level++;
                            }
                            else
                                level = -1;
                        }
                        else
                            level = -1;
                    }
                }
            }
        }

        /// <summary>
        /// 获取部门负责人
        /// </summary>
        /// <param name="ob"></param>
        /// <param name="managersList">部门人员列表</param>
        /// <param name="departId">部门ID</param>
        /// <param name="level"></param>
        /// <returns></returns>
        private ApprovalConditionObject GetUpAccounts(ApprovalConditionObject ob, List<Department> managersList, string departId, int level)
        {
            List<Department> managers = managersList.Where(x => x.Department_ID.ToString() == departId && x.IsManager == 1).ToList();
            if (managers != null && managers.Count > 0)
            {
                if (managers.Count == 1)
                {
                    ob.AccId = managers[0].Account_ID;
                    ob.AccName = managers[0].AccountName;
                }
                else
                {
                    ob.AccId = 0;
                    ob.AccIds = string.Join(",", managers.Select(x => x.Account_ID));
                    ob.AccName = (level == 0 ? "直接上级" : "第" + level + 1 + "级上级");
                }
                return ob;
            }
            else
                return null;
        }


        /// <summary>
        /// 获取模板信息
        /// </summary>
        /// <param name="Approval_ID"></param>
        /// <returns></returns>
        public dynamic GetApprovalInfo(int Approval_ID)
        {
            //模板字段
            List<TempletField> templetFieldList = new List<TempletField>();
            List<H_Customfield> fieldList = Orm.Select<H_Customfield>(x => x.BusType == "approval_" + Approval_ID);
            if (fieldList != null)
            {
                List<H_Customfield> parents = fieldList.Where(x => x.PID == 0).ToList();
                if (parents != null)
                {
                    foreach (var item in parents)
                    {
                        TempletField field = new TempletField();
                        field.fieldId = item.ID;
                        field.fieldName = item.Name;
                        field.Required = item.Required;
                        field.fieldValue = item.FieldType;
                        string[] starr = item.DataSource != null ? item.DataSource.Split('|') : null;
                        if (starr != null)
                        {
                            List<TempletJson> tjList = new List<TempletJson>();
                            foreach (var str in starr)
                            {
                                TempletJson tj = new TempletJson();
                                tj.value = str;
                                tjList.Add(tj);
                            }
                            field.Option = tjList.ToArray();
                        }
                        //明细
                        List<TempletField> childList = new List<TempletField>();
                        List<H_Customfield> children = fieldList.Where(x => x.PID == item.ID).ToList();
                        if (children != null)
                        {
                            foreach (var child in children)
                            {
                                TempletField childField = new TempletField();
                                childField.fieldName = child.Name;
                                childField.Required = child.Required;
                                childField.fieldValue = child.FieldType;
                                string[] _starr = child.DataSource != null ? child.DataSource.Split('|') : null;
                                if (_starr != null)
                                {
                                    List<TempletJson> _tjList = new List<TempletJson>();
                                    foreach (var str in _starr)
                                    {
                                        TempletJson tj = new TempletJson();
                                        tj.value = str;
                                        //if (field.fieldValue == 112)
                                        //{
                                        //    tj.name = str;
                                        //    Regex reg = new Regex(@".*\[([^)]*)\]");
                                        //    Match m = reg.Match(str);
                                        //    if (m.Success)
                                        //        tj.value = m.Result("$1");
                                        //}
                                        _tjList.Add(tj);
                                    }
                                    childField.Option = _tjList.ToArray();
                                }
                                childList.Add(childField);
                            }
                        }
                        field.DetailField = childList;
                        field.Reimbursement = childList;
                        templetFieldList.Add(field);
                    }
                }
            }
            //模板基本信息
            Sp_Approvaltype approvaltype = Orm.Single<Sp_Approvaltype>(x => x.ID == Approval_ID);

            //返回的模板信息
            ApprovalTemplet templet = new ApprovalTemplet();
            templet.EditApprover = approvaltype.EditApprover;
            templet.Name = approvaltype.Name;
            templet.StyleClass = approvaltype.StyleClass;
            templet.Field = templetFieldList;

            //模板可见范围
            List<ApprovalVisrange> visrangeList = SqlMapper.QueryForList<ApprovalVisrange>("GetApprovalVisrangeList", new { Approval_ID }).ToList();
            //模板审批条件
            List<ApprovalCondition> conditionList = new List<ApprovalCondition>();
            List<Sp_Approvalcondition> condition = Orm.Select<Sp_Approvalcondition>(x => x.ApprovalType_ID == Approval_ID);
            List<ApprovalConditionObject> approvers = SqlMapper.QueryForList<ApprovalConditionObject>("GetApprovalObjectList", null).ToList();//审批人
            List<ApprovalConditionObject> approverCC = SqlMapper.QueryForList<ApprovalConditionObject>("GetApprovalCCList", null).ToList();//抄送人
            List<Department> departs = SqlMapper.QueryForList<Department>("GetDepartmentInfoList", null).ToList();//部门
            List<Department> accounts = SqlMapper.QueryForList<Department>("GetAcountInfoList", new { System_Station_ID = approvaltype.System_Station_ID }).ToList();//用户

            if (condition != null)
            {
                foreach (var item in condition)
                {
                    ApprovalCondition ac = new ApprovalCondition();
                    ac.ID = item.ID;
                    ac.ApprovalType_ID = item.ApprovalType_ID;
                    ac.ConditionType = item.ConditionType;
                    ac.FieldId = item.FieldId;
                    ac.ConditionOp = item.ConditionOp;
                    ac.ContrastValue = item.ContrastValue;
                    ac.DeptId = item.DeptIds;
                    ac.AccId = item.AccIds;
                    ac.Priority = item.Priority;
                    ac.ApprovalConditionObject = approvers.Where(x => x.ApprovalCondition_ID == item.ID).ToList();
                    ac.ApprovalConditionCC = approverCC.Where(x => x.ApprovalCondition_ID == item.ID).ToList();
                    if (item.ConditionType == 1)
                        ac.FieldName = "申请人";
                    else if (item.ConditionType == 2)
                    {
                        H_Customfield cf = fieldList.FirstOrDefault(x => x.ID == item.FieldId);
                        ac.FieldName = cf != null ? cf.Name : "未知字段";
                    }
                    string dName = "", acName = "";
                    if (!string.IsNullOrEmpty(item.DeptIds))
                    {
                        List<Department> names = departs.Where(x => ("," + item.DeptIds + ",").IndexOf("," + x.Department_ID + ",") >= 0).ToList();
                        if (names != null)
                            dName = string.Join(",", names.Select(x => x.AccountName));
                    }
                    if (!string.IsNullOrEmpty(item.AccIds))
                    {
                        List<Department> names = accounts.Where(x => ("," + item.AccIds + ",").IndexOf("," + x.Account_ID + ",") >= 0).ToList();
                        if (names != null)
                            acName = string.Join(",", names.Select(x => x.AccountName));
                    }
                    ac.Names = dName + (dName == "" ? "" : (acName == "" ? "" : ",")) + acName;
                    conditionList.Add(ac);
                }
            }

            //返回的审批规则
            ApprovalTypeModel model = new ApprovalTypeModel();
            model.Approval_ID = approvaltype.ID;
            model.Visrange = visrangeList != null ? visrangeList.Where(x => !string.IsNullOrEmpty(x.Name)).ToList() : visrangeList;
            model.Condition = conditionList;

            return new { ApprovalTypeModel = model, ApprovalTemplet = templet };
        }
        #endregion
    }
}