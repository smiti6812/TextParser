using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using TextParsing.Interfaces;

namespace TextParsing.Model
{
    public class DateTimeValue
    {
        private readonly string datePattern = @"(-)|(:)|(\s)|(\.)";
        public DateTime Value { get; set; }       
        public Type Type { get; set; }
        public DateTimeValue(string value, CultureInfo cultureInfo)
        {            
            Value = ReturnDateTime(value, cultureInfo);
            Type = typeof(DateTime);
        }

        private DateTime ReturnDateTime(string dateStr, CultureInfo cultureInfo)
        {
            string[] dateStrArr = Regex.Split(dateStr, datePattern);
            var dateStrList = dateStrArr.Where(c => c != " " && c != "-" && c != ":" && c != "." && c != "").ToList();

            var dateIntList = new List<int>();
            dateStrList.ForEach(d =>
            {
                if (d.Length == 2 && d.StartsWith("0"))
                {
                    dateIntList.Add(int.Parse(d.Substring(1, 1)));
                }
                else
                {
                    dateIntList.Add(int.Parse(d));
                }
            });

            return cultureInfo.Name switch
            {
                "hu-HU" => new DateTime(dateIntList[0], dateIntList[1], dateIntList[2], dateIntList[3], dateIntList[4], dateIntList[5]),
                "de-DE" => new DateTime(dateIntList[0], dateIntList[1], dateIntList[2], dateIntList[3], dateIntList[4], dateIntList[5]),
                "en-UK" => new DateTime(dateIntList[2], dateIntList[1], dateIntList[0], dateIntList[3], dateIntList[4], dateIntList[5]),
                "en-US" => new DateTime(dateIntList[2], dateIntList[0], dateIntList[1], dateIntList[3], dateIntList[4], dateIntList[5]),
            };
        }
    }
}
