using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WpfApp1.MVVM
{
    /// <summary>
    /// 数据验证扩展
    /// </summary>
    public static class ValidationErrorDataExtension
    {
        /// <summary>
        /// 验证方法
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="obj"> </param>
        /// <param name="propertyName"> </param>
        /// <returns> </returns>
        public static string ValidateProperty<T>(this T obj, string propertyName) where T : NotifyPropertyBase
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return string.Empty;
            }
            Type tp = obj.GetType();
            PropertyInfo pi = tp.GetProperty(propertyName);
            var value = pi.GetValue(obj, null);
            object[] Attributes = pi.GetCustomAttributes(false);
            string strErrorMessage = "";
            if (Attributes != null && Attributes.Length > 0)
            {
                foreach (object attribute in Attributes)
                {
                    if (attribute is ValidationAttribute)
                    {
                        try
                        {
                            ValidationAttribute vAttribute = attribute as ValidationAttribute;
                            if (!vAttribute.IsValid(value))
                            {
                                strErrorMessage = !String.IsNullOrWhiteSpace(vAttribute.ErrorMessage) ? vAttribute.ErrorMessage : vAttribute.GetValidationResult(value, new ValidationContext(value, null, null)).ErrorMessage;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.ToString();
                        }
                    }
                }
            }
            return strErrorMessage;
        }


        /// <summary>
        /// 验证属性
        /// </summary>
        /// <returns> 验证是否通过，若通过则返回 true，否则为false。 </returns>
        public static bool Valid<T>(this T obj) where T : NotifyBaseModel
        {
            obj.ErrorMessage = "";
            Type tp = obj.GetType();
            PropertyInfo[] pis = tp.GetProperties();
            obj.IsValid = true;
            foreach (PropertyInfo pi in pis)
            {
                try
                {
                    if (pi.Name.ToUpper() != "ITEM")
                    {
                        string strError = obj.ValidationError.Valid(pi.Name);
                        if (!String.IsNullOrWhiteSpace(strError))
                        {
                            obj.IsValid = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
            if (!obj.IsValid)
            {
                obj.ErrorMessage = obj.ValidationError.Error;
            }
            return obj.IsValid;
        }


        /// <summary>
        /// 验证属性
        /// </summary>
        /// <returns> 验证是否通过，若通过则返回 true，否则为false。 </returns>
        public static bool Valid<T>(this T obj, Func<T, string> Valided) where T : NotifyBaseModel
        {
            if (!obj.Valid())
            {
                return false;
            }
            string message = Valided.Invoke(obj);
            if (!String.IsNullOrWhiteSpace(message))
            {
                obj.ErrorMessage = message;
                return false;
            }
            return true;
        }
    }
}