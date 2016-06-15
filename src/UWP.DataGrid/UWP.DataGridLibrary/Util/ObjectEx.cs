using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace UWP.DataGrid.Util
{
    internal static class ObjectEx
    {

        private static readonly Regex PropertyPathRegEx = new Regex(@"(?:\w*\[[\w.\s]+\]|\[[\w.\s]+\]|\w+)");


        /// <summary>
        /// Gets the value of a property or property path of the specified object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The target object.</param>
        /// <param name="path">The name of the property or the property path.</param>
        /// <returns></returns>
        internal static T GetPropertyValue<T>(this object target, string path)
        {
            return GetPropertyValue<T>(target, path, null, null, null);
        }

        internal static T GetPropertyValue<T>(this object target, string path, IValueConverter converter, object converterParameter, string culture)
        {
            if (target == null)
                return default(T);
            else if (string.IsNullOrEmpty(path))
            {
                if (target is T)
                    return (T)target;
                else
                    return default(T);
            }

            object propertyValue = GetPropertyValue(target, path);

            if (converter != null)
            {
                propertyValue = converter.Convert(propertyValue, typeof(T), converterParameter, culture);
            }

            if (propertyValue == null)
                return default(T);
            return (T)propertyValue;
        }

        /// <summary>
        /// Gets the value of a property or property path of the specified object.
        /// </summary>
        /// <remarks>
        /// This extension DON'T evaluate ICustomTypeDescriptor interface. 
        /// Use generic extension with the same name if you are evaluating the path 
        /// of a DataTable or any object that implements ICustomTypeDescriptor.
        /// </remarks>
        /// <param name="target">The target object.</param>
        /// <param name="path">The name of the property or the property path.</param>
        /// <returns>The value contained in the property</returns>
        internal static object GetPropertyValue(this object target, string path)
        {
            if (target == null)
                return null;
            else if (string.IsNullOrEmpty(path))
                return target;

            var targetType = target.GetType();
            var groups = PropertyPathRegEx.Matches(path);
            if (groups.Count > 1)
            {
                var firstGroup = groups[0];
                var firstProp = firstGroup.Value;
                var propValue = GetPropertyValue(target, firstProp);
                var restExp = path.Substring(firstGroup.Index + firstGroup.Length + 1);
                return GetPropertyValue(propValue, restExp);
            }
            else if (path.Contains("["))
            {
                // get property name
                string propertyName = path.Substring(0, path.IndexOf('['));
                // get index value
                object indexValue = path.Substring(path.IndexOf('[') + 1, path.IndexOf(']') - path.IndexOf('[') - 1);
                // we try numeric indexes first
                PropertyInfo pInfo = null;
                int numIndexValue;
                if (string.IsNullOrEmpty(propertyName))
                {
                    //Looks for an int indexer
                    if (int.TryParse(indexValue as string, out numIndexValue))
                    {
                        pInfo = targetType.GetDefaultProperty(typeof(int));
                        if (pInfo != null)
                            indexValue = numIndexValue;
                    }
                    //Looks for a string indexer
                    if (pInfo == null)
                    {
                        pInfo = targetType.GetDefaultProperty(typeof(string));
                    }
                    //Looks for an object indexer
                    if (pInfo == null)
                    {
                        pInfo = targetType.GetDefaultProperty(typeof(object));
                    }
                    return pInfo.GetValue(target, new object[] { indexValue });
                }
                else
                {
                    object propValue = GetPropertyValue(target, propertyName);
                    return GetPropertyValue(propValue, path.Substring(path.IndexOf('['), path.IndexOf(']') - path.IndexOf('[') + 1));
                }
            }
            else
            {
                PropertyInfo pInfo = targetType.GetProperty(path);

                if (pInfo == null)
                    return null;
                return pInfo.GetValue(target, null);
            }
        }

        /// <summary>
        /// Sets the value of a property or property path of the specified object.
        /// </summary>
        /// <typeparam name="T">Type of the property to set.</typeparam>
        /// <param name="target">Object that contains the property.</param>
        /// <param name="path">Name or path of the property that contains the value.</param>
        /// <param name="value">New value for the property.</param>
        /// <param name="converter">Converter to use when setting the property.</param>
        /// <param name="converterParameter">Parameter passed to the converter.</param>
        /// <param name="culture">Culture information to use when converting values.</param>
        internal static void SetPropertyValue<T>(this object target, string path, T value, IValueConverter converter, object converterParameter, string culture)
        {
            if (target == null || string.IsNullOrEmpty(path))
                return;
            var targetType = target.GetType();
            var groups = PropertyPathRegEx.Matches(path);
            if (groups.Count > 1)
            {
                var firstGroup = groups[0];
                var firstProp = firstGroup.Value;
                var propValue = GetPropertyValue(target, firstProp);
                var restExp = path.Substring(firstGroup.Index + firstGroup.Length + 1);
                SetPropertyValue(propValue, restExp, value, converter, converterParameter, culture);
            }
            else if (path.Contains("["))
            {
                // get property name
                string propertyName = path.Substring(0, path.IndexOf('['));
                // get index value
                object indexValue = path.Substring(path.IndexOf('[') + 1, path.IndexOf(']') - path.IndexOf('[') - 1);
                // we try numeric indexes first
                PropertyInfo pInfo = null;
                int numIndexValue;
                if (string.IsNullOrEmpty(propertyName))
                {
                    //Looks for an int indexer
                    if (int.TryParse(indexValue as string, out numIndexValue))
                    {
                        pInfo = targetType.GetDefaultProperty(typeof(int));
                        if (pInfo != null)
                            indexValue = numIndexValue;
                    }
                    //Looks for a string indexer
                    if (pInfo == null)
                    {
                        pInfo = targetType.GetDefaultProperty(typeof(string));
                    }
                    //Looks for a object indexer
                    if (pInfo == null)
                    {
                        pInfo = targetType.GetDefaultProperty(typeof(object));
                    }
                    pInfo.SetValue(target, value, new object[] { indexValue }, converter, converterParameter, culture);
                }
                else
                {
                    object propValue = GetPropertyValue(target, propertyName);
                    SetPropertyValue(propValue, path.Substring(path.IndexOf('['), path.IndexOf(']') - path.IndexOf('[') + 1), value, converter, converterParameter, culture);
                }
            }
            else
            {

                PropertyInfo pInfo = targetType.GetProperty(path);

                pInfo.SetValue(target, value, null, converter, converterParameter, culture);
            }
        }

        internal static void SetValue(this PropertyInfo propertyInfo, object target, object value, object[] index, IValueConverter converter, object converterParameter, string culture)
        {
            if (converter != null && (value == null || propertyInfo.PropertyType != value.GetType()))
            {
                value = converter.Convert(value, propertyInfo.PropertyType, converterParameter, culture);
            }
            if (propertyInfo.PropertyType.GetTypeInfo().IsEnum && value is string)
            {
                try
                {
                    value = Enum.Parse(propertyInfo.PropertyType, (string)value, true);
                }
                catch { }
            }
            if (value != null && propertyInfo.PropertyType != value.GetType())
            {
                try
                {

                    value = Convert.ChangeType(value, propertyInfo.PropertyType.GetNonNullableType(), culture != null? new CultureInfo(culture) : CultureInfo.InvariantCulture);

                }
                catch { }
            }
            //Converts String.Empty into null when property type is Nullable<T> where T != string
            if (propertyInfo.PropertyType.IsNullableType() && propertyInfo.PropertyType.GetNonNullableType() != typeof(string) && value == "")
            {
                value = null;
            }

            propertyInfo.SetValue(target, value, index);
        }

        internal static string GetPropertyPath<T>(Expression<Func<T, object>> expression)
        {
            // Working outside in e.g. given p.Spouse.Name - the first node will be Name, then Spouse, then p
            IList<string> propertyNames = new List<string>();
            var currentNode = expression.Body;
            while (currentNode.NodeType != ExpressionType.Parameter)
            {
                switch (currentNode.NodeType)
                {
                    case ExpressionType.MemberAccess:
                    case ExpressionType.Convert:
                        MemberExpression memberExpression;
                        memberExpression = (currentNode.NodeType == ExpressionType.MemberAccess ? (MemberExpression)currentNode : (MemberExpression)((UnaryExpression)currentNode).Operand);

                        if (!(memberExpression.Member is PropertyInfo ||
                             memberExpression.Member is FieldInfo))
                        {
                            throw new InvalidOperationException("The member '" + memberExpression.Member.Name + "' is not a field or property");
                        }
                        propertyNames.Add(memberExpression.Member.Name);
                        currentNode = memberExpression.Expression;
                        break;
                    case ExpressionType.Call:
                        MethodCallExpression methodCallExpression = (MethodCallExpression)currentNode;
                        throw new InvalidOperationException("The member '" + methodCallExpression.Method.Name + "' is a method call but a Property or Field was expected.");

                    // To include method calls, remove the exception and uncomment the following three lines:
                    //propertyNames.Add(methodCallExpression.Method.Name);
                    //currentExpression = methodCallExpression.Object;
                    //break;
                    default:
                        throw new InvalidOperationException("The expression NodeType '" + currentNode.NodeType.ToString() + "' is not supported, expected MemberAccess, Convert, or Call.");
                }
            }
            return string.Join(".", propertyNames.Reverse().ToArray());
        }

        internal static string EscapeName(string name)
        {
            return "[" + name.Replace(".", "^.").Replace(",", "^,") + "]";
        }
        internal static string UnescapeName(string path)
        {
            if (path.StartsWith("[") && path.EndsWith("]"))
            {
                path = path.Substring(1, path.Length - 2);
                path = path.Replace("^", "");
            }
            return path;
        }
    }
}
