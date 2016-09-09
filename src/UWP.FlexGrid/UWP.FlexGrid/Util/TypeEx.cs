using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace UWP.FlexGrid
{
    //internal static class TypeEx
    //{
    //    public static bool IsNullableType(this Type type)
    //    {
    //        return (((type != null) && type.GetTypeInfo().IsGenericType) && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
    //    }

    //    public static Type GetNonNullableType(this Type type)
    //    {
    //        if (type.IsNullableType())
    //        {
    //            return Nullable.GetUnderlyingType(type);
    //        }
    //        return type;
    //    }
    //    public static bool IsNumeric(this Type type)
    //    {
    //        return
    //            type == typeof(double) || type == typeof(float) ||
    //            type == typeof(int) || type == typeof(uint) ||
    //            type == typeof(long) || type == typeof(ulong) ||
    //            type == typeof(short) || type == typeof(ushort) ||
    //            type == typeof(sbyte) || type == typeof(byte) ||
    //            type == typeof(decimal);
    //    }
    //    public static bool IsNumericIntegral(this Type type)
    //    {
    //        return
    //            type == typeof(int) || type == typeof(uint) ||
    //            type == typeof(long) || type == typeof(ulong) ||
    //            type == typeof(short) || type == typeof(ushort) ||
    //            type == typeof(sbyte) || type == typeof(byte);
    //    }

    //    public static bool IsNumericNonIntegral(this Type type)
    //    {
    //        return
    //            type == typeof(double) || type == typeof(float) ||
    //            type == typeof(decimal);
    //    }
    //    public static PropertyInfo GetDefaultProperty(this Type targetType, Type memberType)
    //    {
    //        foreach (var member in targetType.GetDefaultMembers())
    //        {
    //            var m = member as PropertyInfo;
    //            if (m != null)
    //            {
    //                var parameters = m.GetIndexParameters();
    //                if (parameters.Length == 1 && parameters[0].ParameterType == memberType)
    //                {
    //                    return m;
    //                }
    //            }
    //        }
    //        return targetType.GetIndexedProperty("Item", memberType);
    //    }
    //    public static PropertyInfo GetIndexedProperty(this Type type, string name, Type indexedType)
    //    {
    //        foreach (var p in type.GetProperties())
    //        {
    //            if (p.Name == name)
    //            {
    //                var parameters = p.GetIndexParameters();
    //                if (parameters.Length == 1 && parameters[0].ParameterType == indexedType)
    //                {
    //                    return p;
    //                }
    //            }
    //        }
    //        foreach (var interfaceType in type.GetInterfaces())
    //        {
    //            var indexedProperty = interfaceType.GetIndexedProperty(name, indexedType);
    //            if (indexedProperty != null)
    //                return indexedProperty;
    //        }
    //        return null;
    //    }

    //}
}
