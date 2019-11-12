using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace WpfApp1.MVVM
{
    /// <summary>
    /// 验证信息数据
    /// </summary>
    public class ValidationErrorData : IDataErrorInfo
    {
        /// <summary>
        /// 拥有本类类型做属性的类
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual NotifyPropertyBase NotifyProperty { get; set; }
         
        /// <summary>
        /// 属性和属性验证值
        /// </summary>
        private Dictionary<string, string> _ErrorDictionary = new Dictionary<string, string>();

        /// <summary>
        /// 所有的错误信息，错误信息。
        /// </summary> 
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual string Error
        {
            get
            {
                string strError = "";
                if (_ErrorDictionary.Count > 0)
                {
                    strError = String.Join(System.Environment.NewLine, _ErrorDictionary.Values.ToArray());
                }
                _ErrorDictionary.Clear();
                return strError;
            }
        }

        /// <summary>
        /// 索引
        /// </summary>
        /// <param name="columnName"> </param>
        /// <returns> </returns>
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual string this[string columnName]
        {
            get
            {
                string strError = "";
                //string strError = this.NotifyProperty.ValidateProperty(columnName);
                //if (!String.IsNullOrWhiteSpace(strError))
                //{
                //    if (!_ErrorDictionary.ContainsKey(columnName))
                //    {
                //        _ErrorDictionary.Add(columnName, strError);
                //    }
                //    else
                //    {
                //        _ErrorDictionary[columnName] = strError;
                //    }
                //}
                //else
                //{
                //    if (!_ErrorDictionary.ContainsKey(columnName))
                //    {
                //        _ErrorDictionary.Remove(columnName);
                //    }
                //}
                if (!_ErrorDictionary.ContainsKey(columnName))
                {
                    strError = _ErrorDictionary[columnName];
                }
                return strError;
            }
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <returns></returns>
        public string Valid(string columnName)
        {
            string strError = "";
            if (NotifyProperty != null)
            {
                strError = this.NotifyProperty.ValidateProperty(columnName);
                if (!String.IsNullOrWhiteSpace(strError))
                {
                    if (!_ErrorDictionary.ContainsKey(columnName))
                    {
                        _ErrorDictionary.Add(columnName, strError);
                    }
                    else
                    {
                        _ErrorDictionary[columnName] = strError;
                    }
                }
                else
                {
                    if (!_ErrorDictionary.ContainsKey(columnName))
                    {
                        _ErrorDictionary.Remove(columnName);
                    }
                }
            }

            return strError;
        }

    }
}