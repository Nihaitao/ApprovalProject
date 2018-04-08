using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalProject.Model
{
    [Alias("h_station_account")]
    public class StationAccount
    {
        public int Account_ID { get; set; }
        public string WechatUserId { get; set; }
    }
}
