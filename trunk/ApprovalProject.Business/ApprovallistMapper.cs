﻿using ApprovalProject.Model;
using ApprovalProject.Model.ViewModel;
using CommonFramework.Exceptions;
using CommonFramework.IBatisNet;
using CT.Common.ModuleBridge;
using HRProject.Model.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ApprovalProject.Business
{
    public class ApprovallistMapper : BaseMapper
    {
        #region 我的审批

        private List<H_Department> DepartData = new List<H_Department>();

        /// <summary>
        /// 获取我申请的审批列表
        /// </summary>
        /// <returns></returns>
        public IList GetApprovallist(SelectApprovalModel model, int ApprovalStatus, int AccountID, int System_Station_ID)
        {
            List<Sp_ApprovallistModel> list = SqlMapper.QueryForList<Sp_ApprovallistModel>("GetApprovallist", new { AddPerson = AccountID, Name = model.Name, ApprovalType_ID = model.ApprovalType_ID, System_Station_ID = System_Station_ID, StartTime = model.StartTime, EndTime = model.EndTime, ApprovalStatus = model.ApprovalStatus, pageIndex = (model.PageIndex - 1) * model.PageSize, pageSize = model.PageSize, pageStatus = model.PageStatus }).ToList();
            if (list != null && list.Count > 0)
            {
                List<H_DepartmentModel> departs = SqlMapper.QueryForList<H_DepartmentModel>("GetAllDepartment", new { System_Station_ID = System_Station_ID, CompanyType = 0 }).ToList(); //系统部门集合

                List<H_DepartmentModel> accountdeparts = SqlMapper.QueryForList<H_DepartmentModel>("GetAccountDepartment", new { Account_ID = 0, System_Station_ID = System_Station_ID }).ToList(); //系统用户所在部门集合
                foreach (Sp_ApprovallistModel p in list)
                {
                    //显示前3条明细数据
                    List<FieldShowModel> FieldShow = new JavaScriptSerializer().Deserialize<List<FieldShowModel>>(p.ApprovalContent);
                    if (FieldShow.Count > 0)
                    {
                        p.ShowDetailOne = FieldShow[0].ShowName + ":" + (FieldShow[0].Details != null && FieldShow[0].Details.Count > 0 ? "明细" : FieldShow[0].ShowValue);
                        if (FieldShow.Count > 1)
                        {
                            p.ShowDetailTwo = FieldShow[1].ShowName + ":" + (FieldShow[1].Details != null && FieldShow[1].Details.Count > 0 ? "明细" : FieldShow[1].ShowValue);
                            if (FieldShow.Count > 2)
                                p.ShowDetailThree = FieldShow[2].ShowName + ":" + (FieldShow[2].Details != null && FieldShow[2].Details.Count > 0 ? "明细" : FieldShow[2].ShowValue);
                        }
                    }

                    if (accountdeparts != null)
                    {
                        //显示用户部门信息
                        List<H_DepartmentModel> accountdeparts2 = accountdeparts.FindAll(x => x.Account_ID == p.AddPerson);
                        if (accountdeparts2 == null) continue;

                        if (DepartData != null)
                        {
                            //DepartData.Clear();
                            DepartData = new List<H_Department>();
                        }

                        p.DepartData = new DepartmentData();
                        p.DepartData.Departments = accountdeparts2;
                        foreach (H_DepartmentModel depart in accountdeparts2)
                        {
                            GetDepartData((List<H_DepartmentModel>)departs, depart);
                        }
                        p.DepartData.Data = DepartData;
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 用户部门
        /// </summary>
        /// <param name="departs"></param>
        /// <param name="Depart"></param>
        private void GetDepartData(List<H_DepartmentModel> departs, H_DepartmentModel Depart)
        {
            foreach (H_DepartmentModel model in departs)
            {
                if (model.ID == Depart.ID)
                {
                    H_DepartmentModel parentDepart = new H_DepartmentModel();

                    if (Depart.CID != null && Depart.CID != 0)
                    {
                        parentDepart = departs.FirstOrDefault(p => p.ID == Depart.CID);
                    }
                    if (!DepartData.Contains(model))
                    {
                        DepartData.Add(model);
                    }
                    if (parentDepart != null)
                    {
                        GetDepartData(departs, parentDepart);
                    }
                }
            }
        }

        /// <summary>
        /// 获取我申请的审批列表总条数
        /// </summary>
        /// <param name="request"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public int GetApprovallistTotalCount(SelectApprovalModel model, int ApprovalStatus, int AccountID, int System_Station_ID)
        {
            return SqlMapper.QueryForObject<int>("GetApprovallistTotalCount", new { AddPerson = AccountID, Name = model.Name, ApprovalType_ID = model.ApprovalType_ID, System_Station_ID = System_Station_ID, StartTime = model.StartTime, EndTime = model.EndTime, ApprovalStatus = model.ApprovalStatus, pageStatus = 0 });
        }

        /// <summary>
        /// 获取抄送我的审批列表
        /// </summary>
        /// <returns></returns>
        public IList GetApprovallistcc(SelectApprovalModel model, int ApprovalStatus, int AccountID, int System_Station_ID)
        {
            List<Sp_ApprovallistModel> list = SqlMapper.QueryForList<Sp_ApprovallistModel>("GetApprovallistcc", new { AccID = AccountID, Name = model.Name, ApprovalType_ID = model.ApprovalType_ID, DeptermentID = model.DeptermentID, AddPerson = model.AddPerson, System_Station_ID = System_Station_ID, StartTime = model.StartTime, EndTime = model.EndTime, ApprovalStatus = ApprovalStatus, pageIndex = (model.PageIndex - 1) * model.PageSize, pageSize = model.PageSize, pageStatus = model.PageStatus }).ToList();

            if (list != null && list.Count > 0)
            {
                List<H_DepartmentModel> departs = SqlMapper.QueryForList<H_DepartmentModel>("GetAllDepartment", new { System_Station_ID = System_Station_ID, CompanyType = 0 }).ToList(); //系统部门集合

                List<H_DepartmentModel> accountdeparts = SqlMapper.QueryForList<H_DepartmentModel>("GetAccountDepartment", new { Account_ID = 0, System_Station_ID = System_Station_ID }).ToList(); //系统用户所在部门集合

                foreach (Sp_ApprovallistModel p in list)
                {
                    //显示前3条明细数据
                    List<FieldShowModel> FieldShow = new JavaScriptSerializer().Deserialize<List<FieldShowModel>>(p.ApprovalContent);
                    if (FieldShow.Count > 0)
                    {
                        p.ShowDetailOne = FieldShow[0].ShowName + ":" + (FieldShow[0].Details != null && FieldShow[0].Details.Count > 0 ? "明细" : FieldShow[0].ShowValue);
                        if (FieldShow.Count > 1)
                        {
                            p.ShowDetailTwo = FieldShow[1].ShowName + ":" + (FieldShow[1].Details != null && FieldShow[1].Details.Count > 0 ? "明细" : FieldShow[1].ShowValue);
                            if (FieldShow.Count > 2)
                                p.ShowDetailThree = FieldShow[2].ShowName + ":" + (FieldShow[2].Details != null && FieldShow[2].Details.Count > 0 ? "明细" : FieldShow[2].ShowValue);
                        }
                    }
                    //显示部门信息
                    if (accountdeparts != null)
                    {
                        List<H_DepartmentModel> accountdeparts2 = accountdeparts.FindAll(x => x.Account_ID == p.AddPerson);
                        if (accountdeparts2 == null) continue;

                        if (DepartData != null)
                        {
                            //DepartData.Clear();
                            DepartData = new List<H_Department>();
                        }

                        p.DepartData = new DepartmentData();
                        p.DepartData.Departments = accountdeparts2;
                        foreach (H_DepartmentModel depart in accountdeparts2)
                        {
                            GetDepartData((List<H_DepartmentModel>)departs, depart);
                        }
                        p.DepartData.Data = DepartData;
                    }
                }
            }
            return list;

        }

        /// <summary>
        /// 获取抄送我的审批列表总条数
        /// </summary>
        /// <param name="request"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public int GetApprovallistccTotalCount(SelectApprovalModel model, int ApprovalStatus, int AccountID, int System_Station_ID)
        {
            return SqlMapper.QueryForObject<int>("GetApprovallistccTotalCount", new { AccID = AccountID, Name = model.Name, ApprovalType_ID = model.ApprovalType_ID, DeptermentID = model.DeptermentID, AddPerson = model.AddPerson, System_Station_ID = System_Station_ID, StartTime = model.StartTime, EndTime = model.EndTime, ApprovalStatus = model.ApprovalStatus, pageStatus = 0 });
        }

        /// <summary>
        /// 获取我处理和未处理的审批（传operationStatus为0是未处理的，1为已经处理的）
        /// </summary>
        /// <returns></returns>
        public IList GetApprovalprocessforPhone(SelectApprovalModel model, int AccountID, int System_Station_ID)
        {
            string xmlName = model.operationStatus == 0 ? "GetNotOperatedApprovalList" : "GetOperatedApprovalList";
            List<Sp_ApprovallistModel> list = SqlMapper.QueryForList<Sp_ApprovallistModel>(xmlName, new { AccID = AccountID, Name = model.Name, ApprovalType_ID = model.ApprovalType_ID, AddPerson = model.AddPerson, System_Station_ID = System_Station_ID, StartTime = model.StartTime, EndTime = model.EndTime, ApprovalStatus = model.ApprovalStatus, pageIndex = (model.PageIndex - 1) * model.PageSize, pageSize = model.PageSize, pageStatus = model.PageStatus }).ToList();

            if (list != null && list.Count > 0)
            {
                List<H_DepartmentModel> departs = SqlMapper.QueryForList<H_DepartmentModel>("GetAllDepartment", new { System_Station_ID = System_Station_ID }).ToList(); //系统部门集合

                List<H_DepartmentModel> accountdeparts = SqlMapper.QueryForList<H_DepartmentModel>("GetAccountDepartment", new { Account_ID = 0, System_Station_ID = System_Station_ID }).ToList(); //系统用户所在部门集合

                foreach (Sp_ApprovallistModel p in list)
                {
                    //显示前3条明细数据
                    List<FieldShowModel> FieldShow = new JavaScriptSerializer().Deserialize<List<FieldShowModel>>(p.ApprovalContent);
                    if (FieldShow.Count > 0)
                    {
                        p.ShowDetailOne = FieldShow[0].ShowName + ":" + (FieldShow[0].Details != null && FieldShow[0].Details.Count > 0 ? "明细" : FieldShow[0].ShowValue);
                        if (FieldShow.Count > 1)
                        {
                            p.ShowDetailTwo = FieldShow[1].ShowName + ":" + (FieldShow[1].Details != null && FieldShow[1].Details.Count > 0 ? "明细" : FieldShow[1].ShowValue);
                            if (FieldShow.Count > 2)
                                p.ShowDetailThree = FieldShow[2].ShowName + ":" + (FieldShow[2].Details != null && FieldShow[2].Details.Count > 0 ? "明细" : FieldShow[2].ShowValue);
                        }
                    }
                    if (accountdeparts != null)
                    {
                        //显示部门信息
                        List<H_DepartmentModel> accountdeparts2 = accountdeparts.FindAll(x => x.Account_ID == p.AddPerson);
                        if (accountdeparts2 == null) continue;

                        if (DepartData != null)
                        {
                            DepartData = new List<H_Department>();
                        }

                        p.DepartData = new DepartmentData();
                        p.DepartData.Departments = accountdeparts2;
                        foreach (H_DepartmentModel depart in accountdeparts2)
                        {
                            GetDepartData((List<H_DepartmentModel>)departs, depart);
                        }
                        p.DepartData.Data = DepartData;
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 获取我处理和未处理的审批（传operationStatus为0是未处理的，1为已经处理的）总条数
        /// </summary>
        /// <param name="request"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public int GetApprovalprocessforPhoneCount(SelectApprovalModel model, int AccountID, int System_Station_ID)
        {
            string xmlName = model.operationStatus == 0 ? "GetNotOperatedApprovalListCount" : "GetOperatedApprovalListCount";
            return SqlMapper.QueryForObject<int>(xmlName, new { AccID = AccountID, Name = model.Name, ApprovalType_ID = model.ApprovalType_ID, AddPerson = model.AddPerson, System_Station_ID = System_Station_ID, StartTime = model.StartTime, EndTime = model.EndTime, ApprovalStatus = model.ApprovalStatus, pageStatus = 0 });
        }
        #endregion

        #region 审批模板

        /// <summary>
        /// 审批模板启用禁用
        /// </summary>
        /// <param name="request"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public dynamic EnditableApprovalTemplet(Sp_Approvaltype model)
        {
            Sp_Approvaltype temptle = Orm.Single<Sp_Approvaltype>(x => x.ID == model.ID);
            if (temptle == null)
            {
                throw new ApiException("模板已删除或者不存在");
            }
            return SqlMapper.Update("EnditableApprovalTemplet", new { ID = model.ID, Enditable = model.Enditable });
        }


        /// <summary>
        /// 删除审批模板
        /// </summary>
        /// <param name="request"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public dynamic DeleteApprovalTemplet(Sp_Approvaltype model)
        {
            Sp_Approvaltype temptle = Orm.Single<Sp_Approvaltype>(x => x.ID == model.ID);
            if (temptle == null)
            {
                throw new ApiException("模板已删除或者不存在");
            }
            return SqlMapper.Update("DeleteApprovalTemplet", new { ID = model.ID });
        }

        /// <summary>
        /// 获取审批模板
        /// </summary>
        /// <param name="request"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public IList GetApprovalTemplet(int System_Station_ID)
        {
            return QueryForList("GetApprovalTemplet", new { System_Station_ID });
        }

        /// <summary>
        /// 获取审批条件字段类型
        /// </summary>
        /// <param name="request"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public List<ApprovalFieldModel> GetApprovalField(int System_Station_ID, int ApprovalID)
        {
            string BusType = "";
            if (ApprovalID > 0)
            {
                BusType = "approval_" + ApprovalID;
            }
            List<ApprovalFieldModel> List = SqlMapper.QueryForList<ApprovalFieldModel>("GetApprovalField", new { BusType, System_Station_ID }).ToList();

            List<ApprovalFieldModel> ApprovalFiellist = new List<ApprovalFieldModel>();

            ApprovalFieldModel model = new ApprovalFieldModel();
            model.ID = 0;
            model.Name = "申请人";
            model.FieldType = 0;

            ApprovalFiellist.Add(model);
            ApprovalFiellist.AddRange(List);

            return ApprovalFiellist;
        }

        /// <summary>
        /// 获取当前站点下所有人
        /// </summary>
        /// <param name="request"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public IList GetAllAcoount(int System_Station_ID)
        {
            return QueryForList("GetAllAcoount", new { System_Station_ID });
        }

        /// <summary>
        /// 根据审批ID查找审批的内容
        /// </summary>
        /// <param name="request"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public dynamic GetSelectApproval(int ApprovalListID)
        {
            ApprovalShow show = new ApprovalShow();
            show = SqlMapper.QueryForObject<ApprovalShow>("GetCCPerson", new { ApprovalListID });
            ApprovalInfoModel ApModel = SqlMapper.QueryForObject<ApprovalInfoModel>("GetApprovalInfoModel", new { ApprovalListID });
            if (ApModel == null)
                throw new ApiException("数据异常，请刷新重试");
            show.ApModel = ApModel;
            show.FieldShow = new JavaScriptSerializer().Deserialize<List<FieldShowModel>>(ApModel.ApprovalContent);
            List<ApprovalProcessModel> List = new List<ApprovalProcessModel>();
            List<ApprovalProcessModel> ProcessList = SqlMapper.QueryForList<ApprovalProcessModel>("GetApprovalProcessList", new { ApprovalListID }).ToList();
            List<ApprovalProcessModel> TransferList = SqlMapper.QueryForList<ApprovalProcessModel>("GetApprovalTransferListList", new { ApprovalListID }).ToList();
            if (ProcessList != null)
            {
                foreach (ApprovalProcessModel item in ProcessList)
                {
                    ApprovalProcessModel temp = List.LastOrDefault(x => x.Deepth == item.Deepth);
                    //如果深度一致，说明是同一级审批
                    if (temp != null)
                    {
                        AccountModel model = new AccountModel();
                        model.State = item.OperationStatus;
                        model.Remark = item.Remark;
                        model.OperationTime = item.OperationTime;
                        model.AccountID = item.AccID;
                        model.AccountName = item.AccName;
                        model.AccountType = item.AccIDType;
                        if (item.ApprovalTransfer_ID > 0)//转审
                        {
                            ApprovalProcessModel _temp = TransferList.FirstOrDefault(x => x.ID == item.ApprovalTransfer_ID);
                            if (_temp != null)
                            {
                                model.Transfer_AccountID = _temp.AccID;
                                model.Transfer_AccountName = _temp.AccName;
                                model.Transfer_OperationTime = _temp.OperationTime;
                                model.State = _temp.OperationStatus;
                                model.Transfer_Remark = _temp.Remark;
                            }
                        }
                        temp.AccountList.Add(model);
                    }
                    else //深度不一致，审批级别不同
                    {
                        AccountModel model = new AccountModel();
                        model.State = item.OperationStatus;
                        model.Remark = item.Remark == null ? "" : item.Remark;
                        model.OperationTime = item.OperationTime;
                        model.AccountID = item.AccID;
                        model.AccountName = item.AccName;
                        model.AccountType = item.AccIDType;
                        if (item.ApprovalTransfer_ID > 0)//转审
                        {
                            ApprovalProcessModel _temp = TransferList.FirstOrDefault(x => x.ID == item.ApprovalTransfer_ID);
                            if (_temp != null)
                            {
                                model.Transfer_AccountID = _temp.AccID;
                                model.Transfer_AccountName = _temp.AccName;
                                model.Transfer_OperationTime = _temp.OperationTime;
                                model.State = _temp.OperationStatus;
                                model.Transfer_Remark = _temp.Remark;
                            }
                        }
                        item.AccountList = new List<AccountModel>();
                        item.AccountList.Add(model);
                        List.Add(item);
                    }
                }
                foreach (var item in List)
                {
                    //控制审批流程每层深度的状态显示
                    if (item.AccountList.Count == 1)
                    {
                        item.OperationStatus = item.AccountList[0].State;
                        if (item.OperationStatus == 0 && item.ApprovalTransfer_ID > 0)
                            item.OperationStatus = 3;
                    }
                    else
                    {
                        if (item.CounterSign == 0)//会签
                        {
                            item.OperationStatus = 0;
                            if (item.AccountList.FirstOrDefault(x => x.State == 2) != null)//有人驳回，全部驳回
                                item.OperationStatus = 2;
                            else
                            {
                                List<AccountModel> _list = item.AccountList.Where(x => x.State == 1).ToList();
                                if (_list != null && _list.Count > 0)
                                {
                                    if (_list.Count == item.AccountList.Count)//全部通过
                                        item.OperationStatus = 1;
                                    else
                                        item.OperationStatus = 3;
                                }
                            }
                        }
                        else if (item.CounterSign == 1)//或签
                        {
                            item.OperationStatus = 0;
                            if (item.AccountList.FirstOrDefault(x => x.State == 1) != null)//有人通过，全部通过
                                item.OperationStatus = 1;
                            else
                            {
                                List<AccountModel> _list = item.AccountList.Where(x => x.State == 2).ToList();
                                if (_list != null && _list.Count > 0)
                                {
                                    if (_list.Count == item.AccountList.Count)//全部驳回
                                        item.OperationStatus = 2;
                                    else
                                        item.OperationStatus = 3;
                                }
                            }
                        }
                    }
                }
            }
            show.ProcessList = List;
            return show;
        }
        #endregion

        #region 审批操作
        /// <summary>
        /// 根据审批ID查找审批的按钮是否显示
        /// </summary>
        /// <param name="request"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public dynamic GetIsShow(int ApprovalListID, int AccountID)
        {
            Sp_Approvallist listmodel = Orm.Single<Sp_Approvallist>(x => x.ID == ApprovalListID);
            if (listmodel.ApprovalStatus > 1 || listmodel.ApprovalStatus == -1)//已审核or已驳回or已撤销
                return 0;
            List<Sp_Approvalprocess> processmodel = SqlMapper.QueryForList<Sp_Approvalprocess>("GetApprovalprocessModel", new { ApprovalListID = ApprovalListID, Deepth = listmodel.CurrentDeepth }).ToList();

            Sp_Approvalprocess _temp = processmodel.FirstOrDefault(x => x.AccID == AccountID);

            if (_temp == null || _temp.operationStatus != 0)//我不是当前深度审核人,或者我已操作
            {
                return 0;
            }
            else if (_temp.ApprovalTransfer_ID == 0)
            {
                if (_temp.AccIDType == 2)//我是当前深度会计
                    return 3;
                else if (_temp.AccIDType == 3)//我是当前深度出纳
                    return 4;
                else//我是当前深度正牌审核人
                    return 1;
            }
            else //我是当前深度转审人
            {
                return 2;
            }

        }

        /// <summary>
        /// 根据审批ID查找审批的撤销按钮是否显示
        /// </summary>
        /// <param name="request"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public dynamic GetIsShowRevoke(int ApprovalListID, int AccountID)
        {
            Sp_Approvallist listmodel = Orm.Single<Sp_Approvallist>(x => x.ID == ApprovalListID);
            if (listmodel.AddPerson == AccountID && listmodel.ApprovalStatus != -1 && listmodel.ApprovalStatus != 2 && listmodel.ApprovalStatus != 3)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 保存申请记录，并且根据审批人的个数分别存入流程表 by SZF
        /// </summary>
        /// <param name="request"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public bool SaveApprovalList(ApprovallistModel model, int System_Station_ID, int AccountID)
        {
            if (model.Approvers == null || model.Approvers.Count == 0)
                throw new ApiException("没有审批人，无法提交申请");
            ///开启事物
            try
            {
                SqlMapper.BeginTransaction();
                ///审批记录表
                Sp_Approvallist approvalmodel = new Sp_Approvallist();
                approvalmodel.ApprovalType_ID = model.ID;
                approvalmodel.System_Station_ID = System_Station_ID;
                approvalmodel.AddPerson = AccountID;
                approvalmodel.AddTime = DateTime.Now;
                approvalmodel.FinishTime = DateTime.Now;
                approvalmodel.ApprovalStatus = 0;
                approvalmodel.MaxDeepth = model.Approvers.Count;
                approvalmodel.CurrentDeepth = 1;
                approvalmodel.CurrentCounterSign = model.Approvers[0].AccId > 0 ? 2 : model.Approvers[0].CounterSign;//0会签1或签2个签
                List<FieldShowModel> ContenrList = new List<FieldShowModel>();
                //审批内容
                foreach (var fielditem in model.Field)
                {
                    FieldShowModel fieldvalue = new FieldShowModel();
                    fieldvalue.ShowName = fielditem.Name;
                    fieldvalue.ShowValue = fielditem.Value != null ? string.Join(",", fielditem.Value) : "";
                    if (fielditem.FieldType == 112)
                        fieldvalue.IsReimbursement = 1;
                    if (fielditem.Detail != null && fielditem.Detail.Count > 0)//判断明细里面是否存在数据
                    {
                        fieldvalue.Details = new List<FieldShowModel>();
                        foreach (var item in fielditem.Detail)
                        {
                            FieldShowModel fieldvalue2 = new FieldShowModel();
                            fieldvalue2.ShowName = item.Name;
                            fieldvalue2.ShowValue = item.Value != null ? string.Join(",", item.Value) : "";
                            fieldvalue.Details.Add(fieldvalue2);
                        }
                    }
                    ContenrList.Add(fieldvalue);
                }

                approvalmodel.ApprovalContent = new JavaScriptSerializer().Serialize(ContenrList);
                int ID = (int)Orm.Insert(approvalmodel, true);
                if (ID > 0)
                {
                    //审批人流程
                    List<Sp_Approvalprocess> list = new List<Sp_Approvalprocess>();
                    for (int i = 0; i < model.Approvers.Count; i++)
                    {
                        ApprovalConditionObject approveritem = model.Approvers[i];
                        Sp_Approvalprocess approval = new Sp_Approvalprocess();
                        approval.ApprovalList_ID = ID;
                        approval.Deepth = i + 1;
                        approval.operationTime = DateTime.Now;
                        if (approveritem.AccId > 0)
                        {
                            approval.CounterSign = 2;
                            approval.AccID = approveritem.AccId;
                            approval.AccIDType = approveritem.Type;
                            if (Orm.Insert(approval) <= 0)
                            {
                                throw new ApiException("插入失败");
                            }
                        }
                        else
                        {
                            approval.CounterSign = approveritem.CounterSign;
                            string[] arr = approveritem.AccIds.Split(',');
                            foreach (var item in arr)
                            {
                                approval.AccID = int.Parse(item);
                                if (Orm.Insert(approval) <= 0)
                                {
                                    throw new ApiException("插入失败");
                                }
                            }
                        }
                    }
                    //抄送人
                    List<Sp_Approvallistcc> listCC = new List<Sp_Approvallistcc>();
                    if (model.ApproverCC != null)
                    {
                        foreach (var approveritem in model.ApproverCC)
                        {
                            Sp_Approvallistcc approval = new Sp_Approvallistcc();
                            approval.ApprovalList_ID = ID;
                            approval.AddTime = DateTime.Now;
                            if (approveritem.AccId > 0)
                            {
                                approval.AccID = approveritem.AccId;
                                if (Orm.Insert(approval) <= 0)
                                {
                                    throw new ApiException("插入失败");
                                }
                            }
                            else
                            {
                                string[] arr = approveritem.AccIds.Split(',');
                                foreach (var item in arr)
                                {
                                    approval.AccID = int.Parse(item);
                                    if (Orm.Insert(approval) <= 0)
                                    {
                                        throw new ApiException("插入失败");
                                    }
                                }
                            }
                        }
                    }
                }
                //提交事物
                SqlMapper.CommitTransaction();
                return true;
            }
            catch (Exception)
            {
                ///事物回滚
                SqlMapper.RollBackTransaction();
                return false;
            }
        }

        /// <summary>
        /// 根据审批ID修改审批的状态和批阅意见
        /// </summary>
        /// <param name="request"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public bool UpdateApprovalList(ApprovallistVModel model, int AccountID)
        {
            try
            {
                SqlMapper.BeginTransaction();
                Sp_Approvallist listmodel = Orm.Single<Sp_Approvallist>(x => x.ID == model.ApprovalList_ID);
                if (listmodel.ApprovalStatus > 1)//已审核完
                    throw new ApiException("当前审批状态已更新，请刷新后重试");
                listmodel.ApprovalStatus = 1;
                List<Sp_Approvalprocess> processmodel = SqlMapper.QueryForList<Sp_Approvalprocess>("GetApprovalprocessModel", new { ApprovalListID = model.ApprovalList_ID, Deepth = listmodel.CurrentDeepth }).ToList();

                if (processmodel.Count > 0)
                {
                    //当前审批人
                    Sp_Approvalprocess _temp = processmodel.FirstOrDefault(x => x.AccID == AccountID);
                    if (_temp == null)
                        throw new ApiException("抱歉，您不是当前审核人，没有操作权限");
                    if (_temp.operationStatus > 0)//已审核
                        throw new ApiException("抱歉，您已审核，请勿重复操作");

                    #region 会计审核流程
                    //会计审核流程
                    if (_temp.AccIDType == 2 && model.operationStatus == 1)
                    {
                        DateTime nowTime = DateTime.Now;
                        //保存会计支付信息
                        if (model.Payment != null && model.Payment.Count > 0)
                        {
                            //删除原有的支付信息
                            if (Orm.Single<Sp_Approvalpay>(x => x.ApprovalList_ID == model.ApprovalList_ID) == null || Orm.Delete<Sp_Approvalpay>(x => x.ApprovalList_ID == model.ApprovalList_ID) > 0)
                            {
                                foreach (var item in model.Payment)
                                {
                                    Sp_Approvalpay pay = new Sp_Approvalpay();
                                    pay.ApprovalList_ID = model.ApprovalList_ID;
                                    pay.Code = item.Code;
                                    pay.Money = item.Money;
                                    if (Orm.Insert<Sp_Approvalpay>(pay) < 0)
                                        throw new ApiException("会计支付选择有误，请重试！");
                                }
                            }
                            else
                                throw new ApiException("会计支付信息有误，请重试！");
                        }
                        else
                            throw new ApiException("会计支付不能为空！");
                        //如果当期审核人为最后一位审核的会计，则添加凭证信息
                        if (_temp.Deepth == SqlMapper.QueryForObject<int>("IsTheLastOne", new { ApprovalList_ID = model.ApprovalList_ID, AccIDType = 2 }))
                        {
                            //获取凭证编号
                            var requestBridge = new RequestBridge("finance");
                            string url = "Finance/GetCurrentNo?BeginTime=" + nowTime + "&Stationproxy_ID=0&Other=3&Status=0";
                            CommonFramework.Result result = requestBridge.Get(url);
                            if (!result.SuccessResponse)
                                throw new ApiException(result.Message);
                            List<VoucherDetails> voucherDetails = new List<VoucherDetails>();
                            foreach (var item in model.LeftCode)
                            {
                                VoucherDetails detail = new VoucherDetails();
                                detail.Code = item.Code;
                                detail.Date = item.Time;
                                detail.Title = !string.IsNullOrEmpty(item.Mark) ? item.Mark : "报销费用";
                                detail.LeftMoney = item.Money;
                                detail.Person_ID = AccountID;
                                voucherDetails.Add(detail);
                            }
                            foreach (var item in model.Payment)
                            {
                                VoucherDetails detail = new VoucherDetails();
                                detail.Code = item.Code;
                                detail.Title = "报销费用";
                                if (item.Code.IndexOf("Wx_") >= 0)
                                {
                                    detail.Code = item.Code.Substring(3, item.Code.Length - 3);
                                    detail.Title = "微信支付报销费用";
                                }
                                detail.Date = nowTime.ToString();
                                detail.RightMoney = item.Money;
                                voucherDetails.Add(detail);
                            }
                            //添加凭证
                            url = "Finance/InsertFinanceVoucher";
                            var data = new { Time = nowTime, Type = 3, Handler = AccountID, No = int.Parse(result.Data.ToString()), Stationproxy_ID = 0, IsStudent = 0, ReceiptNum = model.ReceiptNum, VoucherDetails = voucherDetails };
                            requestBridge.ContentType = "application/json";
                            result = requestBridge.Post(url, data);
                            if (!result.SuccessResponse)
                                throw new ApiException(result.Message);
                            listmodel.VoucherID = result.Data.ToString();
                            if (Orm.Update<Sp_Approvallist>(listmodel) <= 0)
                                throw new ApiException("系统异常，操作失败，请重试！");
                        }
                    }
                    #endregion

                    if (_temp.ApprovalTransfer_ID == 0)//不是转审人
                    {
                        _temp.operationStatus = model.operationStatus;
                        _temp.operationTime = DateTime.Now;
                        _temp.Remark = model.Remark;
                        if (Orm.Update<Sp_Approvalprocess>(_temp) <= 0)
                        {
                            throw new ApiException("操作失败");
                        }
                    }
                    else//转审人
                    {
                        Sp_Approvaltransfer transfer = Orm.Single<Sp_Approvaltransfer>(x => x.ID == _temp.ApprovalTransfer_ID);
                        if (transfer == null)
                            throw new ApiException("当前审批状态已更新，请刷新后重试");
                        transfer.operationStatus = model.operationStatus;
                        transfer.operationTime = DateTime.Now;
                        transfer.Remark = model.Remark;
                        if (Orm.Update<Sp_Approvaltransfer>(transfer) <= 0)
                        {
                            throw new ApiException("操作失败");
                        }
                    }
                    processmodel = SqlMapper.QueryForList<Sp_Approvalprocess>("GetApprovalprocessModel", new { ApprovalListID = model.ApprovalList_ID, Deepth = listmodel.CurrentDeepth }).ToList();
                    bool agree = false;//当前深度审核通过
                    bool notAgree = false;//审核驳回，结束流程
                    if (listmodel.CurrentCounterSign == 0)//会签
                    {
                        if (model.operationStatus == 2)
                        {//不同意,会签不通过.同层次其他未审人员操作状态改为-1
                            notAgree = true;
                            List<Sp_Approvalprocess> list = processmodel.Where(x => x.operationStatus == 0).ToList();
                            foreach (var item in list)
                            {
                                item.operationStatus = -1;
                                Orm.Update<Sp_Approvalprocess>(item);
                            }
                        }
                        else
                        {
                            List<Sp_Approvalprocess> list = processmodel.Where(x => x.operationStatus == 1).ToList();
                            if (list != null && list.Count == processmodel.Count) //全部人同意，会签通过      
                                agree = true;
                        }
                    }
                    else if (listmodel.CurrentCounterSign == 1)//或签
                    {
                        if (model.operationStatus == 1) //同意
                        {
                            agree = true;
                            List<Sp_Approvalprocess> list = processmodel.Where(x => x.operationStatus == 0).ToList();
                            foreach (var item in list)
                            {
                                item.operationStatus = -1;
                                Orm.Update<Sp_Approvalprocess>(item);
                            }
                        }
                        else
                        {
                            List<Sp_Approvalprocess> list = processmodel.Where(x => x.operationStatus == 2).ToList();
                            if (list != null && list.Count == processmodel.Count) //所有人不同意
                                notAgree = true;
                        }
                    }
                    else//个人
                    {
                        if (model.operationStatus == 1) //同意
                            agree = true;
                        else
                            notAgree = true;

                    }
                    if (agree)
                    {
                        if (listmodel.CurrentDeepth == listmodel.MaxDeepth)//如果是最后一层
                        {
                            listmodel.ApprovalStatus = 2;//审核通过
                        }
                        else
                        {
                            listmodel.CurrentDeepth = listmodel.CurrentDeepth + 1;
                            listmodel.ApprovalStatus = 1;//审核中
                        }
                    }
                    if (notAgree)
                    {
                        listmodel.ApprovalStatus = 3;//审核驳回
                        listmodel.CurrentDeepth = listmodel.MaxDeepth;
                    }
                    listmodel.FinishTime = DateTime.Now;
                    if (Orm.Update(listmodel) <= 0)
                    {
                        throw new ApiException("操作失败，请刷新重试！");
                    }

                    #region 出纳审核流程(支付放在最后防止审批流程出错出现支付成功审核失败情况)
                    //出纳审核流程
                    if (_temp.AccIDType == 3)
                    {
                        //出纳同意
                        if (model.operationStatus == 1)
                        {
                            //如果当期审核人为最后一位审核的出纳，则修改凭证状态
                            //如果包含微信支付，则自动企业微信发支付
                            if (_temp.Deepth == SqlMapper.QueryForObject<int>("IsTheLastOne", new { ApprovalList_ID = model.ApprovalList_ID, AccIDType = 3 }))
                            {
                                //查询凭证信息
                                if (SqlMapper.QueryForObject<int>("GetVoucherInfo", new { VoucherID = listmodel.VoucherID }) == 0)
                                    throw new ApiException("财务凭证不存在或者已作废，请联系管理员");
                                RequestBridge requestBridge = null;
                                CommonFramework.Result result = null;
                                //判断是否包含微信
                                decimal WeiXinMoney = model.Payment.Where(x => x.Code.IndexOf("Wx_") >= 0).Sum(x => x.Money);
                                if (WeiXinMoney > 0)
                                {

                                    //企业微信支付
                                    requestBridge = new RequestBridge("work");
                                    requestBridge.Timeout = 30000;//设置超时时间
                                    requestBridge.ContentType = "application/json";
                                    //报销人 Account_ID  报销金额 amount  报销描述 desc  报销项目  act_name  审批ID Approval_ID  凭证ID VoucherID
                                    result = requestBridge.Post("api/WxWorkPay", new { Account_ID = listmodel.AddPerson, amount = WeiXinMoney, desc = "费用报销", act_name = "费用报销", Approval_ID = model.ApprovalList_ID, VoucherID = listmodel.VoucherID });
                                    if (!result.SuccessResponse)
                                        throw new ApiException(result.Message);
                                }
                                //审核凭证
                                requestBridge = new RequestBridge("finance");
                                requestBridge.ContentType = "application/json";
                                result = requestBridge.Post("Finance/CheckFinanceVoucher", new { VoucherID = listmodel.VoucherID, Valid = 1 });
                                if (!result.SuccessResponse)
                                    throw new ApiException(result.Message);
                            }
                        }
                        //出纳驳回
                        else if (model.operationStatus == 2)
                        {
                            //凭证审核不通过
                            RequestBridge requestBridge = new RequestBridge("finance");
                            requestBridge.ContentType = "application/json";
                            CommonFramework.Result result = requestBridge.Post("Finance/CheckFinanceVoucher", new { VoucherID = listmodel.VoucherID, Valid = 2 });
                            if (!result.SuccessResponse)
                                throw new ApiException(result.Message);

                        }
                    }
                    #endregion
                }
                else
                {
                    throw new ApiException("操作失败，请刷新重试！");
                }
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
        /// 根据审批ID撤销审批
        /// </summary>
        /// <param name="request"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public dynamic RevokeApprovalList(Sp_Approvalprocess model, int AccountID)
        {
            Sp_Approvallist listmodel = Orm.Single<Sp_Approvallist>(x => x.ID == model.ApprovalList_ID);
            if (listmodel.ApprovalStatus > 1)//已审核完
                throw new ApiException("当前审批已完成，不能撤销");
            if (listmodel.ApprovalStatus == -1)
            {
                throw new ApiException("请勿重复撤销");
            }
            listmodel.ApprovalStatus = -1;
            listmodel.FinishTime = DateTime.Now;
            if (Orm.Update(listmodel) > 0)
            {
                return listmodel.ApprovalType_ID;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 根据审批ID转审给别人
        /// </summary>
        /// <param name="request"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public bool Turnthetrial(int ApprovalListID, int AccountID, int transferID, string Remark)
        {
            try
            {
                SqlMapper.BeginTransaction();

                Sp_Approvallist model = Orm.Single<Sp_Approvallist>(x => x.ID == ApprovalListID);

                List<Sp_Approvalprocess> processmodel = SqlMapper.QueryForList<Sp_Approvalprocess>("GetApprovalprocessModel", new { ApprovalListID = ApprovalListID, Deepth = model.CurrentDeepth }).ToList();//根据当前深度查找当前深度所有的流程

                Sp_Approvalprocess _temp = processmodel.FirstOrDefault(x => x.AccID == AccountID);//查找所有流程里面属于自己的流程
                if (_temp == null)
                    throw new ApiException("抱歉，您不是当前审核人，没有操作权限");
                if (_temp.ApprovalTransfer_ID > 0)
                    throw new ApiException("抱歉，您是转审人，不能再次转审");
                if (transferID == _temp.AccID)
                    throw new ApiException("转审人不能是自己");
                if (processmodel.FirstOrDefault(x => x.AccID == transferID) != null)
                    throw new ApiException("转审人不能为当前审批深度审批人");

                Sp_Approvaltransfer transfer = new Sp_Approvaltransfer();
                transfer.AccID = transferID;
                transfer.AddTime = DateTime.Now;
                transfer.operationTime = DateTime.Now;
                transfer.ApprovalList_ID = model.ID;
                transfer.ApprovalProcess_ID = _temp.ID;
                transfer.operationStatus = 0;
                int ApprovalTransfer_ID = (int)Orm.Insert(transfer, true);

                model.ApprovalStatus = 1;//操作审核记录，转审后为审核中

                if (ApprovalTransfer_ID <= 0 || Orm.Update(model) <= 0)
                {
                    throw new ApiException("转审操作失败");
                }
                else
                {
                    _temp.ApprovalTransfer_ID = ApprovalTransfer_ID;
                    _temp.operationStatus = 3;
                    _temp.operationTime = DateTime.Now;
                    _temp.Remark = Remark;
                    if (Orm.Update(_temp) < 0)
                    {
                        throw new ApiException("转审操作失败");
                    }
                }
                SqlMapper.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                SqlMapper.RollBackTransaction();
                throw new ApiException(ex.Message);
            }
        }
        #endregion

        #region 审批报销财务相关

        /// <summary>
        /// 获取支付方式
        /// </summary>
        /// <param name="System_Station_ID"></param>
        /// <returns></returns>
        public dynamic GetPaymentCode(int System_Station_ID)
        {
            List<PaymentCode> list = new List<PaymentCode>();
            List<PaymentCode> codes = SqlMapper.QueryForList<PaymentCode>("GetPaymentCode", new { System_Station_ID }).ToList();
            foreach (PaymentCode item in codes)
            {
                PaymentCode temp = codes.FirstOrDefault(x => x.Code != item.Code && x.Code.IndexOf(item.Code) == 0);
                if (temp == null)
                    item.IsEnd = 1;
            }
            list = codes.Where(x => x.IsEnd == 1).ToList();
            PaymentCode payment = SqlMapper.QueryForObject<PaymentCode>("GetWeiXinPaymentCode", new { System_Station_ID });
            if (payment != null)
                list.Add(payment);
            return list;
        }

        /// <summary>
        /// 获取会计支付信息
        /// </summary>
        /// <param name="ApprovalListID"></param>
        /// <returns></returns>
        public dynamic GetPaymentList(int ApprovalListID)
        {
            return Orm.Select<Sp_Approvalpay>(x => x.ApprovalList_ID == ApprovalListID);
        }

        /// <summary>
        /// 保存报销类型
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool AddFinanceType(List<Sp_FinanceType> list, int System_Station_ID)
        {
            try
            {
                SqlMapper.BeginTransaction();
                foreach (var item in list)
                {
                    if (item.ID == 0)
                    {
                        item.System_Station_ID = System_Station_ID;
                        if (Orm.Insert<Sp_FinanceType>(item) == 0)
                            throw new ApiException("操作失败");
                    }
                    else
                    {
                        if (Orm.Update<Sp_FinanceType>(item) == 0)
                            throw new ApiException("操作失败");
                    }
                }
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
        /// 删除报销类型
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="System_Station_ID"></param>
        /// <returns></returns>
        public bool DeleteFinanceType(int ID, int System_Station_ID)
        {
            Sp_FinanceType model = Orm.Single<Sp_FinanceType>(x => x.ID == ID && x.System_Station_ID == System_Station_ID);
            if (model == null)
                throw new ApiException("目标不存在或者已删除，请刷新重试！");
            return Orm.Delete<Sp_FinanceType>(model) > 0;
        }


        /// <summary>
        /// 获取报销类型
        /// </summary>
        /// <returns></returns>
        public dynamic GetFinanceType(int System_Station_ID)
        {
            return Orm.Select<Sp_FinanceType>(x => x.System_Station_ID == System_Station_ID && x.Stationproxy_ID == 0);
        }
        #endregion
    }
}
