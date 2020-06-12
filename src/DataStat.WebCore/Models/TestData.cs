using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataStat.WebCore.Models
{
    public class TestData
    {
        //[Key]
        public int kid { get; set; }

        public string Name { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
