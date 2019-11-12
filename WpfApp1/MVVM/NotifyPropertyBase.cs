using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace WpfApp1.MVVM
{
    /// <summary>
    /// 属性更改通知
    /// </summary>
    public class NotifyPropertyBase : DynamicObject, INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary> 
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 属性修改
        /// </summary>
        /// <param name="propertyName"> </param>
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 属性和属性值
        /// </summary>
        private IDictionary<string, object> _ValueDictionary = new Dictionary<string, object>();

        /// <summary>
        /// 构造函数
        /// </summary>
        protected NotifyPropertyBase()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dictionary"></param>
        public NotifyPropertyBase(IDictionary<string, object> dictionary)
        {
            if (dictionary != null)
            {
                //dictionary = new Dictionary<string, object>();
                _ValueDictionary = dictionary;
            }
        }

        #region 根据属性名得到属性值

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this._ValueDictionary.FirstOrDefault(dictionary => dictionary.Key.ToLower() == binder.Name.ToLower()).Value;

            if (result is IDictionary<string, object>)
            {
                result = new NotifyPropertyBase(result as IDictionary<string, object>);
            }
            else if (result is ArrayList && (result as ArrayList) is IDictionary<string, object>)
            {
                result = new List<NotifyPropertyBase>((result as ArrayList).Cast<IDictionary<string, object>>().Select(x => new NotifyPropertyBase(x)));
            }
            else if (result is ArrayList)
            {
                result = new List<NotifyPropertyBase>((result as ArrayList).Cast<NotifyPropertyBase>());
            }

            //return base.TryGetMember(binder, out result);
            return true;
        }

        /// <summary>
        /// 属性访问
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="propertyName"> </param>
        /// <returns> </returns>
        public virtual T GetValue<T>([CallerMemberName]string propertyName = "")
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("invalid " + propertyName);
            }
            object _propertyValue;
            if (!_ValueDictionary.TryGetValue(propertyName, out _propertyValue))
            {
                _propertyValue = default(T);
                _ValueDictionary.Add(propertyName, _propertyValue);
            }
            return (T)_propertyValue;
        }

        #endregion 根据属性名得到属性值

        #region 设置指定的属性值

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var data = this._ValueDictionary.FirstOrDefault(dictionary => dictionary.Key.ToLower() == binder.Name.ToLower());

            if (!String.IsNullOrWhiteSpace(data.Key))
            {
                this._ValueDictionary.Remove(data.Key);
            }

            this._ValueDictionary[binder.Name] = value;

            OnPropertyChanged(binder.Name);
            //return base.TrySetMember(binder, value);
            return true;
        }

        /// <summary>
        /// 属性访问
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="value"> </param>
        /// <param name="propertyName"> </param>
        public virtual void SetValue<T>(T value, [CallerMemberName]string propertyName = "")
        {
            if (!_ValueDictionary.ContainsKey(propertyName) || _ValueDictionary[propertyName] != (object)value)
            {
                _ValueDictionary[propertyName] = value;
            }
            OnPropertyChanged(propertyName);
        }

        #endregion 设置指定的属性值

    }
}