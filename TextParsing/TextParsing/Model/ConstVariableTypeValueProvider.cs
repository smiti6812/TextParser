using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using TextParsing.Interfaces;

namespace TextParsing.Model
{
    public static class ConstVariableTypeValueProvider
    {
        public static ExpressionNode ReturnExpressionNode(string propertyType, string constantValue = "", CultureInfo cultureInfo = null)
        {
            return propertyType switch
            {
                "String" => new NodeString(constantValue),
                "Int32" => new NodeInt32(Convert.ToInt32(constantValue, cultureInfo)),
                "Int64" => new NodeInt64(Convert.ToInt64(constantValue, cultureInfo)),
                "UInt32" => new NodeUInt32(Convert.ToUInt32(constantValue, cultureInfo)),
                "UInt64" => new NodeUInt64(Convert.ToUInt64(constantValue, cultureInfo)),
                "Bool" => new NodeBool(Convert.ToBoolean(constantValue, cultureInfo)),
                "Decimal" => new NodeDecimal(Convert.ToDecimal(constantValue, cultureInfo)),
                "Single" => new NodeSingle(Convert.ToSingle(constantValue, cultureInfo)),
                "DateTime" => new NodeDateTime(new DateTimeValue(constantValue, cultureInfo).Value),
                _ => new ExpressionNode()
            };
        }        

        public static dynamic GetExpressionNodeDescendant(ExpressionNode node) => Convert.ChangeType(node, node.GetType());

        public static dynamic GetPropertyValue<T>(string field, T item)
        {
            Type t = item.GetType();
            PropertyInfo propertyInfo = t.GetProperty(field);
            return Convert.ChangeType(propertyInfo.GetValue(item, null), propertyInfo.PropertyType);
        }

        public static dynamic GetValue<T>(T item)
        {
            Type t = item.GetType();
            return Convert.ChangeType(item, t);
        }
    }
}
