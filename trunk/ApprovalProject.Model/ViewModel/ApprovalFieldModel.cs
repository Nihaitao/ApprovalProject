﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApprovalProject.Model.ViewModel
{
    public class ApprovalFieldModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string DataSource { get; set; }

        public string[] DataSourceArr
        {
            get
            {
                if (!string.IsNullOrEmpty(DataSource))
                    return DataSource.Split('|');
                else
                    return null;
            }
        }

        public int FieldType { get; set; }
    }
}