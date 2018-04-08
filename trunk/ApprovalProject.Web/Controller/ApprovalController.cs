﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CommonFramework;
using CommonFramework.Exceptions;
using CommonFramework.IBatisNet.SqlMap;
using CommonFramework.Mvc;
using CommonFramework.Mvc.Attribute;
using CT.Common.Mvc;
using ApprovalProject.Business;
using ApprovalProject.Model;
using ApprovalProject.Model.ViewModel;

namespace ApprovalProject.Web.Controller
{
    public class ApprovalController : StationBaseApiController
    {
        ApprovalMapper mapper = new ApprovalMapper();

        #region 审批模板
        /// <summary>
        /// 添加审批模板 BY nht
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        public dynamic AddApprovalTemplet(ApprovalTemplet templet)
        {
            return Success(mapper.AddApprovalTemplet(templet, this.System_Station_ID, this.AccountID));
        }

        /// <summary>
        /// 编辑审批模板 BY nht
        /// </summary>
        /// <param name="templet"></param>
        /// <returns></returns>
        [HttpPost]
        public dynamic EditApprovalTemplet(ApprovalTemplet templet)
        {
            return Success(mapper.EditApprovalTemplet(templet, this.System_Station_ID));
        }

        /// <summary>
        /// 添加条件审批 by SZF
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        public dynamic AddApprovalType(ApprovalTypeModel templet)
        {
            return Success(mapper.AddApprovalType(templet, this.System_Station_ID, this.AccountID) ? "操作成功" : "操作失败");
        }

        /// <summary>
        /// 编辑条件审批
        /// </summary>
        /// <param name="templet"></param>
        /// <returns></returns>
        [HttpPost]
        public dynamic EditApprovalType(ApprovalTypeModel templet)
        {
            return Success(mapper.EditApprovalType(templet, this.System_Station_ID, this.AccountID) ? "操作成功" : "操作失败");
        }

        /// <summary>
        /// 获取模板信息
        /// </summary>
        /// <param name="Approval_ID"></param>
        /// <returns></returns>
        [HttpGet]
        public dynamic GetApprovalInfo(int Approval_ID)
        {
            return Success(mapper.GetApprovalInfo(Approval_ID));
        }


        /// <summary>
        /// 获取发起申请列表 根据可见范围规定列表内容 BY SZF
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public dynamic GetInitiateApplicationList()
        {
            return Success(mapper.GetInitiateApplicationList(this.AccountID, this.System_Station_ID));
        }

        /// <summary>
        /// 根据模板ID，获取模板内容 by nht
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public dynamic GetApplication(int approvalTypeID)
        {
            return Success(mapper.GetApplication(approvalTypeID,this.AccountID, this.System_Station_ID));
        }
        #endregion
    }
}