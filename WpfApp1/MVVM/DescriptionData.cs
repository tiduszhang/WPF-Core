using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace WpfApp1.MVVM
{
    /// <summary>
    /// 属性描述信息
    /// </summary>
    public class DescriptionData 
    {
        /// <summary>
        /// 拥有本类类型做属性的类
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual NotifyPropertyBase NotifyProperty { get; set; }

        /// <summary>
        /// 索引
        /// </summary>
        /// <param name="propertyName"> </param>
        /// <returns> </returns>
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual string this[string propertyName]
        {
            get
            {
                string propertyNameValue = "";

                if (NotifyProperty != null)
                {
                    propertyNameValue = NotifyProperty.GetPropertyDescription(propertyName);
                }
                return propertyNameValue;
            }
        } 
    }
}
