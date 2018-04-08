﻿using CommonFramework.IBatisNet;
using ApprovalProject.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;

namespace ApprovalProject.Business.Process
{
    public class ProcessMapper : BaseMapper
    {
        //自动获取数据 --服务后台
        public dynamic GetAutoAllProcess()
        {
            bool ret = false;
            string next_spnum = "";
            string Access_token = "";
            string Secret = "Gz1nvsBp97sjXJ4_Dju-0dkgJh8AhQ7oK5GMVEKdLmk";
            string appid = "wwecc3fdd8a0ac5001";
            string url = "https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid=" + appid + "&corpsecret=" + Secret;
            PageHelper p = new PageHelper();
            string AccesstokenString = p.ResponseToString(p.doGetWeb(url));
            JavaScriptSerializer AccesstokenJson = new JavaScriptSerializer();
            WeChatOAuthScan OAuthScanAccess = AccesstokenJson.Deserialize<WeChatOAuthScan>(AccesstokenString);
            if (!string.IsNullOrEmpty(AccesstokenString))
            {
                Access_token = OAuthScanAccess.Access_token;
                var starttime = "1483200000";
                var endtime = ConvertDateTimeInt(DateTime.Today.AddDays(1));
                //微信获取数据网址
                string posturl = "https://qyapi.weixin.qq.com/cgi-bin/corp/getapprovaldata?access_token=" + Access_token;
                //微信获取数据条件
                string postData = "{\"starttime\":" + starttime + ",\"endtime\":" + endtime + ",\"next_spnum\":" + next_spnum + "}";
                string ProcessString = p.ResponseToString(p.doPost(posturl, postData));
                //将获取数据反序列化
                JavaScriptSerializer ProcessJson = new JavaScriptSerializer();
                CompanyWechat ProcessListAll = ProcessJson.Deserialize<CompanyWechat>(ProcessString);
                //算出需要查询几次（该接口最多返回10000条数据，如超过10000条数据则需分多次获取）
                string res = Math.Ceiling(Convert.ToDouble(Convert.ToDouble(ProcessListAll.total) / Convert.ToDouble(ProcessListAll.count))).ToString();
                for (int i = 0; i < (int.Parse(res) - 1); i++)
                {
                    next_spnum = ProcessListAll.next_spnum;
                    ProcessString = p.ResponseToString(p.doPost(posturl, postData));
                    CompanyWechat ProcessListAll1 = ProcessJson.Deserialize<CompanyWechat>(ProcessString);
                    ProcessListAll.data.AddRange(ProcessListAll1.data);
                }
                List<ProcessList> process = ProcessListAll.data;
                List<leave> leavelist = new List<leave>();
                List<expense> expenselist = new List<expense>();
                List<item> itemlist = new List<item>();
                for (int j = 0; j < process.Count; j++)
                {
                    if (process[j].leave != null)
                    {
                        leavelist.Add(process[j].leave);
                    }
                    else if (process[j].expense != null)
                    {
                        expenselist.Add(process[j].expense);
                        if (process[j].expense != null)
                        {
                            for (int k = 0; k < (process[j].expense.item).Count; k++)
                            {
                                itemlist.Add(process[j].expense.item[k]);
                            }
                        }
                    }
                }

                using (var transaction = SqlMapper.BeginTransaction())
                {
                    try
                    {

                        process.ForEach(x => Orm.Insert(x));
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.RollBackTransaction();
                    }
                }
                return 0;
            }
            else
            {
                return false;
            }
        }

        /// <summary>  
        /// DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time"> DateTime时间格式</param>  
        /// <returns>Unix时间戳格式</returns>  
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
    }
}