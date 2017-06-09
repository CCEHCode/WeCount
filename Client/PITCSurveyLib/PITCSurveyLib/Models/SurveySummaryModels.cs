using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITCSurveyLib.Models
{
    public class SurveySummaryModel
    {
        public int ID { get; set; }
        public String Description { get; set; }
        public int Version { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
    }
}
