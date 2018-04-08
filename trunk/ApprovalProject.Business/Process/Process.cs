using CommonFramework.IBatisNet;
using ApprovalProject.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ApprovalProject.Business.Process
{
    public class Process : BaseMapper
    {
        //自动获取数据 --服务后台
        public dynamic GetAutoAllProcess()
        {
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
                DateTime dtNow = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                DateTime dtStart = DateTime.Parse(DateTime.Now.ToString("d") + "0:00:00");
                DateTime dtEnd = DateTime.Parse(DateTime.Now.ToString("d") + "23:59:59");
                TimeSpan toStart = dtStart.Subtract(dtNow);
                TimeSpan toEnd = dtEnd.Subtract(dtNow);
                string starttime = toStart.Ticks.ToString();
                string endtime = toEnd.Ticks.ToString();
                starttime = starttime.Substring(0, starttime.Length - 7);
                endtime = endtime.Substring(0, endtime.Length - 7);
                string posturl = "https://qyapi.weixin.qq.com/cgi-bin/corp/getapprovaldata?access_token=" + Access_token;
                //string postData = "?starttime=" + starttime + "&endtime=" + endtime + "&next_spnum=" + next_spnum;
                string postData = "{\"starttime\" : \"" + starttime + "\",\"endtime\" : \"" + endtime + "\",\"next_spnum\":\"" + next_spnum + "\"}";
                string ProcessString = p.ResponseToString(p.doPost(posturl, postData));
                return ProcessString;
            }
            else
            {
                return false;
            }
        }
    }
}