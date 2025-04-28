using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using TextParsing.Interfaces;
using TextParsing.Model;

namespace TextParsing.Services
{
    public static class ParserEvaliationService
    {
        public static bool EvaluateValueAndConstantNotEqual(object fieldValue, ExpressionNode node, string propertyType, CultureInfo cultureInfo)
        {
            switch (propertyType)
            {
                case "String":
                    return fieldValue.ToString() != ((NodeString)node).ConstantValue;
                case "Int32":
                    return Convert.ToInt32(fieldValue.ToString(), cultureInfo) !=
                    ((NodeInt32)node).ConstantValue;
                case "Int64":
                    return Convert.ToInt64(fieldValue.ToString(), cultureInfo) !=
                    ((NodeInt64)node).ConstantValue;
                case "UInt64":
                    return Convert.ToUInt64(fieldValue.ToString(), cultureInfo) !=
                    ((NodeUInt64)node).ConstantValue;
                case "UInt32":
                    return Convert.ToUInt32(fieldValue.ToString(), cultureInfo) !=
                    ((NodeUInt32)node).ConstantValue;
                case "Decimal":
                    return Convert.ToDecimal(fieldValue.ToString(), cultureInfo) !=
                    ((NodeDecimal)node).ConstantValue;
                case "Double":
                    return Convert.ToDouble(fieldValue.ToString(), cultureInfo) !=
                    ((NodeDouble)node).ConstantValue;
                case "Bool":
                    return Convert.ToBoolean(fieldValue.ToString(), cultureInfo) !=
                    ((NodeBool)node).ConstantValue;
                case "Single":
                    return Convert.ToSingle(fieldValue.ToString(), cultureInfo) !=
                    ((NodeSingle)node).ConstantValue;
                case "Char":
                    return Convert.ToChar(fieldValue.ToString(), cultureInfo) !=
                    ((NodeChar)node).ConstantValue;
                case "Byte":
                    return Convert.ToByte(fieldValue.ToString(), cultureInfo) !=
                    ((NodeByte)node).ConstantValue;
                case "DateTime":
                    return Convert.ToDateTime(fieldValue.ToString(), cultureInfo) !=
                    ((NodeDateTime)node).ConstantValue;
                default:
                    throw new ArgumentOutOfRangeException();
            };
        }

        public static bool EvaluateValueAndConstantEqual(object fieldValue, ExpressionNode node, string propertyType, CultureInfo cultureInfo)
        {

            switch (propertyType)
            {
                case "String":
                    return fieldValue.ToString() == ((NodeString)node).ConstantValue;
                case "Int32":
                    return Convert.ToInt32(fieldValue.ToString(), cultureInfo) == ((NodeInt32)node).ConstantValue;
                case "Int64":
                    return Convert.ToInt64(fieldValue.ToString(), cultureInfo) ==
                    ((NodeInt64)node).ConstantValue;
                case "UInt64":
                    return Convert.ToUInt64(fieldValue.ToString(), cultureInfo) ==
                    ((NodeUInt64)node).ConstantValue;
                case "UInt32":
                    return Convert.ToInt32(fieldValue.ToString(), cultureInfo) ==
                    ((NodeUInt32)node).ConstantValue;
                case "Decimal":
                    return Convert.ToDecimal(fieldValue.ToString(), cultureInfo) ==
                    ((NodeDecimal)node).ConstantValue;
                case "Double":
                    return Convert.ToDouble(fieldValue.ToString(), cultureInfo) ==
                    ((NodeDouble)node).ConstantValue;
                case "Bool":
                    return Convert.ToBoolean(fieldValue.ToString(), cultureInfo) ==
                    ((NodeBool)node).ConstantValue;
                case "Single":
                    return Convert.ToSingle(fieldValue.ToString(), cultureInfo) ==
                    ((NodeSingle)node).ConstantValue;
                case "Char":
                    return Convert.ToChar(fieldValue.ToString(), cultureInfo) ==
                    ((NodeChar)node).ConstantValue;
                case "Byte":
                    return Convert.ToByte(fieldValue.ToString(), cultureInfo) ==
                    ((NodeByte)node).ConstantValue;
                case "DateTime":
                    return Convert.ToDateTime(fieldValue.ToString(), cultureInfo) ==
                    ((NodeDateTime)node).ConstantValue;
                default:
                    throw new ArgumentOutOfRangeException();
            };
        }

        public static bool EvaluateValueAndConstantGreaterThan(object fieldValue, ExpressionNode node, string propertyType, CultureInfo cultureInfo)
        {
            switch (propertyType)
            {
                case "Int32":
                    return Convert.ToInt32(fieldValue.ToString(), cultureInfo) >
                    ((NodeInt32)node).ConstantValue;
                case "Int64":
                    return Convert.ToInt64(fieldValue.ToString(), cultureInfo) >
                    ((NodeInt64)node).ConstantValue;
                case "UInt64":
                    return Convert.ToUInt64(fieldValue.ToString(), cultureInfo) >
                    ((NodeUInt64)node).ConstantValue;
                case "UInt32":
                    return Convert.ToUInt32(fieldValue.ToString(), cultureInfo) >
                    ((NodeUInt32)node).ConstantValue;
                case "Decimal":
                    return Convert.ToDecimal(fieldValue.ToString(), cultureInfo) >
                    ((NodeDecimal)node).ConstantValue;
                case "Double":
                    return Convert.ToDouble(fieldValue.ToString(), cultureInfo) >
                    ((NodeDouble)node).ConstantValue;
                case "Single":
                    return Convert.ToSingle(fieldValue.ToString(), cultureInfo) >
                    ((NodeSingle)node).ConstantValue;
                case "DateTime":
                    return Convert.ToDateTime(fieldValue.ToString(), cultureInfo) >
                    ((NodeDateTime)node).ConstantValue;
                default:
                    throw new ArgumentOutOfRangeException();
            };
        }

        public static bool EvaluateValueAndConstantGreaterThanEqual(object fieldValue, ExpressionNode node, string propertyType, CultureInfo cultureInfo)
        {
            switch (propertyType)
            {
                case "Int32":
                    return Convert.ToInt32(fieldValue.ToString(), cultureInfo) >=
                    ((NodeInt32)node).ConstantValue;
                case "Int64":
                    return Convert.ToInt64(fieldValue.ToString(), cultureInfo) >=
                    ((NodeInt64)node).ConstantValue;
                case "UInt64":
                    return Convert.ToUInt64(fieldValue.ToString(), cultureInfo) >=
                    ((NodeUInt64)node).ConstantValue;
                case "UInt32":
                    return Convert.ToUInt32(fieldValue.ToString(), cultureInfo) >=
                    ((NodeUInt32)node).ConstantValue;
                case "Decimal":
                    return Convert.ToDecimal(fieldValue.ToString(), cultureInfo) >=
                    ((NodeDecimal)node).ConstantValue;
                case "Double":
                    return Convert.ToDouble(fieldValue.ToString(), cultureInfo) >=
                    ((NodeDouble)node).ConstantValue;
                case "Single":
                    return Convert.ToSingle(fieldValue.ToString(), cultureInfo) >=
                    ((NodeSingle)node).ConstantValue;
                case "DateTime":
                    return Convert.ToDateTime(fieldValue.ToString(), cultureInfo) >=
                    ((NodeDateTime)node).ConstantValue;
                default:
                    throw new ArgumentOutOfRangeException();
            };
        }

        public static bool EvaluateValueAndConstantLessThan(object fieldValue, ExpressionNode node, string propertyType, CultureInfo cultureInfo)
        {
            switch (propertyType)
            {
                case "Int32":
                    return Convert.ToInt32(fieldValue.ToString(), cultureInfo) <
                    ((NodeInt32)node).ConstantValue;
                case "Int64":
                    return Convert.ToInt64(fieldValue.ToString(), cultureInfo) <
                    ((NodeInt64)node).ConstantValue;
                case "UInt64":
                    return Convert.ToUInt64(fieldValue.ToString(), cultureInfo) <
                    ((NodeUInt64)node).ConstantValue;
                case "UInt32":
                    return Convert.ToUInt32(fieldValue.ToString(), cultureInfo) <
                    ((NodeUInt32)node).ConstantValue;
                case "Decimal":
                    return Convert.ToDecimal(fieldValue.ToString(), cultureInfo) <
                    ((NodeDecimal)node).ConstantValue;
                case "Double":
                    return Convert.ToDouble(fieldValue.ToString(), cultureInfo) <
                    ((NodeDouble)node).ConstantValue;
                case "Single":
                    return Convert.ToSingle(fieldValue.ToString(), cultureInfo) <
                    ((NodeSingle)node).ConstantValue;
                case "DateTime":
                    return Convert.ToDateTime(fieldValue.ToString(), cultureInfo) <
                    ((NodeDateTime)node).ConstantValue;
                default:
                    throw new ArgumentOutOfRangeException();
            };
        }

        public static bool EvaluateValueAndConstantLessThanEqual(object fieldValue, ExpressionNode node, string propertyType, CultureInfo cultureInfo)
        {
            switch (propertyType)
            {
                case "Int32":
                    return Convert.ToInt32(fieldValue.ToString(), cultureInfo) <=
                    ((NodeInt32)node).ConstantValue;
                case "Int64":
                    return Convert.ToInt64(fieldValue.ToString(), cultureInfo) <=
                    ((NodeInt64)node).ConstantValue;
                case "UInt64":
                    return Convert.ToUInt64(fieldValue.ToString(), cultureInfo) <=
                    ((NodeUInt64)node).ConstantValue;
                case "UInt32":
                    return Convert.ToUInt32(fieldValue.ToString(), cultureInfo) <=
                    ((NodeUInt32)node).ConstantValue;
                case "Decimal":
                    return Convert.ToDecimal(fieldValue.ToString(), cultureInfo) <=
                    ((NodeDecimal)node).ConstantValue;
                case "Double":
                    return Convert.ToDouble(fieldValue.ToString(), cultureInfo) <=
                    ((NodeDouble)node).ConstantValue;
                case "Single":
                    return Convert.ToSingle(fieldValue.ToString(), cultureInfo) <=
                    ((NodeSingle)node).ConstantValue;
                case "DateTime":
                    return Convert.ToDateTime(fieldValue.ToString(), cultureInfo) <=
                    ((NodeDateTime)node).ConstantValue;
                default:
                    throw new ArgumentOutOfRangeException();
            };
        }

        public static bool EvaluateCall(object fieldValue, ExpressionNode node, string callFunction)
        {
            string constant = ((NodeString)node).ConstantValue;
            return callFunction switch
            {
                "StartWith" => fieldValue.ToString().StartsWith(constant.Substring(1, constant.Length - 2)),
                "EndWith" => fieldValue.ToString().EndsWith(constant.Substring(1, constant.Length - 2)),
                "Contains" => fieldValue.ToString().Contains(constant.Substring(1, constant.Length - 2))
            };
        }

    }
}
