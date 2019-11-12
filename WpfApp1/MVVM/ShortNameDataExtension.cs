using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WpfApp1.MVVM
{
    /// <summary>
    /// 
    /// </summary>
    public static class ShortNameDataExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="obj"> </param>
        /// <param name="propertyName"> </param>
        /// <returns> </returns>
        public static string GetPropertyShortName<T>(this T obj, string propertyName) where T : NotifyPropertyBase
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return string.Empty;
            }
            Type tp = obj.GetType();
            PropertyInfo pi = tp.GetProperty(propertyName);
            var value = pi.GetValue(obj, null);
            object[] Attributes = pi.GetCustomAttributes(false);
            string strShortName = "";
            if (Attributes != null && Attributes.Length > 0)
            {
                foreach (object attribute in Attributes)
                {
                    if (attribute is DisplayAttribute)
                    {
                        try
                        {
                            DisplayAttribute vAttribute = attribute as DisplayAttribute;
                            strShortName = vAttribute.GetShortName(); 
                            break;
                        }
                        catch (Exception ex)
                        {
                            ex.ToString();
                        }
                    }
                }
            }
            return strShortName;
        }
    }
}