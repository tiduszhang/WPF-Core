using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1.MVVM
{
    /// <summary>
    /// MVVM基类，实例化成dynamic类型，可以支持动态属性
    /// </summary>
    public class NotifyBaseModel : NotifyPropertyBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public NotifyBaseModel()
        {
            DisplayName = new DisplayNameData()
            {
                NotifyProperty = this
            };
            Description = new DescriptionData()
            {
                NotifyProperty = this
            };
            Prompt = new PromptData()
            {
                NotifyProperty = this
            };
            ShortName = new ShortNameData()
            {
                NotifyProperty = this
            };
            ValidationError = new ValidationErrorData()
            {
                NotifyProperty = this
            };

            this.ErrorMessage = "";
            this.IsValid = true;
        }

        /// <summary>
        /// DisplayNameData
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual DisplayNameData DisplayName
        {
            set
            {
                this.SetValue(value);
            }
            get
            {
                return this.GetValue<DisplayNameData>();
            }
        }

        /// <summary>
        /// ValidationErrorData
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ValidationErrorData ValidationError
        {
            set
            {
                this.SetValue(value);
            }
            get
            {
                return this.GetValue<ValidationErrorData>();
            }
        }

        /// <summary>
        /// ShortNameData
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ShortNameData ShortName
        {
            set
            {
                this.SetValue(value);
            }
            get
            {
                return this.GetValue<ShortNameData>();
            }
        }

        /// <summary>
        /// PromptData
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual PromptData Prompt
        {
            set
            {
                this.SetValue(value);
            }
            get
            {
                return this.GetValue<PromptData>();
            }
        }

        /// <summary>
        /// DescriptionData
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual DescriptionData Description
        {
            set
            {
                this.SetValue(value);
            }
            get
            {
                return this.GetValue<DescriptionData>();
            }
        }

        /// <summary>
        /// 是否验证通过
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual bool IsValid
        {
            set
            {
                this.SetValue(value);
            }
            get
            {
                return this.GetValue<bool>();
            }
        }

        /// <summary>
        /// 错误消息
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual string ErrorMessage
        {
            set
            {
                this.SetValue(value);
            }
            get
            {
                return this.GetValue<string>();
            }
        }

    }
}
