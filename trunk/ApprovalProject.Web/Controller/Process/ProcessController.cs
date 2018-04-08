﻿using CT.Common.Mvc;
using ApprovalProject.Business;
using ApprovalProject.Business.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ApprovalProject.Model;
using ApprovalProject.Model.ViewModel;


namespace ApprovalProject.Web.Controller.Process
{
    /// <summary>
    /// 审批控制器
    /// </summary>
    public class ProcessController : StationBaseApiController
    {
        ProcessMapper ProMapper = new ProcessMapper();
        ApprovallistMapper mapper = new ApprovallistMapper();
        //获取审批
        [HttpGet, AllowAnonymous] //AllowAnonymous允许控制器匿名访问
        //我的申请
        public dynamic GetApplyProcess(int id = 0, string starttime = null, string endtime = null)
        {

            ProMapper.GetAutoAllProcess();
            //    ProcessMapper ProMapper = new ProcessMapper();
            //    ProMapper.GetAutoAllProcess();
            //    //TestMapper mapper = new TestMapper();
            //    //using (var session = mapper.OpenOrmSession())
            //    //{
            //    //    var stationAccount = session.Select<StationAccount>(x => x.Account_ID == id).FirstOrDefault();
            //    //    mapper.selectTest("1123");
            //    //}
            return Success("11");
        }

        /// <summary>
        /// 添加自定义客户模板
        /// </summary>
        /// <returns></returns>
        //public dynamic AddTempletCustom(List<Customfield> CustonField, string TempletName = "", int TempletCustomID = 0)
        //{
        //    if (CustonField.Count > 0)
        //    {
        //        //添加自定义模板数据   审批模板主表和自定义字段表   业务类型为6 
        //        ProcessMapper Mapper = new ProcessMapper();
        //        Mapper.AddTempletCustom(CustonField, TempletName, this.AccountID, this.System_Station_ID);
        //        return Success(Mapper.AddTempletCustom(CustonField, TempletName, this.AccountID, this.System_Station_ID));
        //    }
        //    else
        //    {
        //        return Error<Customfield>("参数错误", null);
        //    }

        //}
        /// <summary>
        /// 获取我提交的审批
        /// </summary>
        /// <auther>SZF</auther>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public dynamic GetApprovallist(SelectApprovalModel model)
        {
            var dict = mapper.GetApprovallist(model, model.ApprovalStatus, this.AccountID, this.System_Station_ID);
            return Success(dict, model.PageIndex == 1 ? mapper.GetApprovallistTotalCount(model, model.ApprovalStatus, this.AccountID, this.System_Station_ID) : 0);
        }

        /// <summary>
        /// 获取抄送我的审批
        /// </summary>
        /// <auther>SZF</auther>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public dynamic GetApprovallistcc(SelectApprovalModel model)
        {
            var dict = mapper.GetApprovallistcc(model, model.ApprovalStatus, this.AccountID, this.System_Station_ID);
            return Success(dict, model.PageIndex == 1 ? mapper.GetApprovallistccTotalCount(model, model.ApprovalStatus, this.AccountID, this.System_Station_ID) : 0);
        }

        /// <summary>
        /// 获取我处理的审批
        /// </summary>
        /// <auther>SZF</auther>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public dynamic GetApprovalprocess(SelectApprovalModel model)
        {
            var dict = mapper.GetApprovalprocess(model,model.ApprovalStatus, this.AccountID, this.System_Station_ID);
            return Success(dict, model.PageIndex == 1 ? mapper.GetApprovalprocessTotalCount(model,model.ApprovalStatus, this.AccountID, this.System_Station_ID) : 0);
        }

        /// <summary>
        /// 获取审批条件字段类型
        /// </summary>
        /// <auther>SZF</auther>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public dynamic GetApprovalField(int ApprovalID)
        {
            return Success(mapper.GetApprovalField(this.System_Station_ID, ApprovalID = 8));
        }

        /// <summary>
        /// 获取当前站点下所有人
        /// </summary>
        /// <auther>SZF</auther>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public dynamic GetAllAcoount()
        {
            return Success(mapper.GetAllAcoount(this.System_Station_ID));
        }

    }
}
