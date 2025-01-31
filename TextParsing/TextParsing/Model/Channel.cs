using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextParsing.Model
{
    public class Channel
    {
        public string Location { get; set; }
        public string Direction { get; set; }
        public string SensorLocation { get; set; }
        public int Sum { get; set; }
        public DateTime Date { get; set; }
        public double DoubleSum { get; set; }
        public Decimal DecimalSum { get; set; }
        public float FloatSum { get; set; }
        public Int64 Int64Sum { get; set; }
        public bool BoolType { get; set; }
    }
}
