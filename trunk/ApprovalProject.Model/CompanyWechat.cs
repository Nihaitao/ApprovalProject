using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalProject.Model
{
    /// <summary>
    /// 主要返回主句 --主表
    /// </summary>
    [Alias("h_processcondition")]
    public class CompanyWechat
    {
        public string starttime { get; set; }

        public string endtime { get; set; }

        public string next_spnum { get; set; }

        public int count { get; set; }

        public int total { get; set; }

        public List<ProcessList> data { set; get; }
    }

    [Alias("h_process")]
    public class ProcessList
    {
        public string spname { get; set; }

        public string apply_name { get; set; }

        public string apply_org { get; set; }

        [Ignore]
        public string[] approval_name { get; set; }

        public string approvalname
        {
            get
            {
                string name = string.Join(",", approval_name);
                return name;
            }
        }

        public string[] notify_name { get; set; }

        public int sp_status { get; set; }

        public string sp_num { get; set; }

        public string apply_time { get; set; }

        public string apply_user_id { get; set; }

        [Ignore]
        public leave leave { get; set; }
        [Ignore]
        public expense expense { get; set; }
    }

    /// <summary>
    /// 请假数据  --请假表
    /// </summary>
    [Alias("h_leaveprocess")]
    public class leave
    {
        public int timeunit { get; set; }

        public int leave_type { get; set; }

        public string start_time { get; set; }

        public string end_time { get; set; }


        public int duration { get; set; }

        public string reason { get; set; }
    }


    /// <summary>
    /// 报销数据 --报销主表
    /// </summary>
    [Alias("h_expenseprocess")]
    public class expense
    {
        public int expense_type { get; set; }

        public string reason { get; set; }

        public List<item> item { get; set; }
    }


    /// <summary>
    /// 报销明细  --报销明细表
    /// </summary>
    [Alias("h_expenseprocessdetails")]
    public class item
    {
        public int expenseitem_type { get; set; }

        public string time { get; set; }

        public int sums { get; set; }

        public string reason { get; set; }
    }
}
