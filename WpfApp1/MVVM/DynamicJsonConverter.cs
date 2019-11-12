using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WpfApp1.MVVM
{
    /// <summary>
    /// 动态类型对象
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// </summary>  
    public sealed class DynamicJsonConverter : JsonConverter<object>
    {
        public DynamicJsonConverter()
        {
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return true;
        }

        /// <summary>
        /// 读取属性
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            object obj = Activator.CreateInstance(typeToConvert);
            FieldInfo? _ValueDictionaryField = null;
            if (obj is NotifyPropertyBase)
            {
                _ValueDictionaryField = typeof(NotifyPropertyBase).GetField("_ValueDictionary", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();

                    if (!reader.Read())
                    {
                        break;
                    }

                    if (reader.TokenType == JsonTokenType.String)
                    {
                        var value = reader.GetString();

                        var propertyInfo = typeToConvert.GetProperty(propertyName);

                        if (propertyInfo != null)
                        {
                            propertyInfo.SetValue(obj, value);
                        }
                        else if (_ValueDictionaryField != null)
                        {
                            IDictionary<string, object> _ValueDictionary = _ValueDictionaryField.GetValue(obj) as IDictionary<string, object>;
                            if (_ValueDictionary == null)
                            {
                                _ValueDictionary = new Dictionary<string, object>();
                            }
                            if (!_ValueDictionary.ContainsKey(propertyName))
                            {
                                _ValueDictionary.Add(propertyName, value);
                                _ValueDictionaryField.SetValue(obj, _ValueDictionary);
                            }
                        }
                    }
                    else if (reader.TokenType == JsonTokenType.Number)
                    {
                        JsonDocument jsonDocument = JsonDocument.ParseValue(ref reader);
                        var valueText = jsonDocument.RootElement.GetRawText();

                        var propertyInfo = typeToConvert.GetProperty(propertyName);

                        if (propertyInfo != null)
                        {
                            var value = ConvertObject(valueText, propertyInfo.PropertyType);
                            propertyInfo.SetValue(obj, value);
                        }
                        else if (_ValueDictionaryField != null)
                        {
                            IDictionary<string, object> _ValueDictionary = _ValueDictionaryField.GetValue(obj) as IDictionary<string, object>;
                            if (_ValueDictionary == null)
                            {
                                _ValueDictionary = new Dictionary<string, object>();
                            }
                            if (!_ValueDictionary.ContainsKey(propertyName))
                            {
                                _ValueDictionary.Add(propertyName, valueText);
                                _ValueDictionaryField.SetValue(obj, _ValueDictionary);
                            }
                        }
                    }
                    else if (reader.TokenType == JsonTokenType.StartArray)
                    {
                        var propertyInfo = typeToConvert.GetProperty(propertyName);

                        if (propertyInfo != null)
                        {
                            if (typeof(Array).IsAssignableFrom(propertyInfo.PropertyType))
                            {
                                JsonDocument jsonDocument = JsonDocument.ParseValue(ref reader);
                                var array = jsonDocument.RootElement.EnumerateArray();
                                var values = array.ToArray();

                                var value = Array.CreateInstance(propertyInfo.PropertyType.GetElementType(), values.Length);
                                for (int i = 0; i < values.Length; i++)
                                {
                                    var data = values[i].GetRawText();
                                    var oData = ConvertObject(values[i].GetRawText(), propertyInfo.PropertyType.GetElementType());
                                    value.SetValue(oData, i);
                                }
                                propertyInfo.SetValue(obj, value);
                            }
                            else
                            {
                                //复杂类型  
                                IList lstObject = Activator.CreateInstance(propertyInfo.PropertyType) as IList;
                                while (reader.Read())
                                {
                                    if (reader.TokenType == JsonTokenType.EndArray)
                                    {
                                        break;
                                    }
                                    var vObj = Read(ref reader, propertyInfo.PropertyType.GetGenericArguments()[0], options);
                                    lstObject.Add(vObj);
                                }
                                //var value = Array.CreateInstance(propertyInfo.PropertyType.GetGenericArguments()[0], lstObject.Count);
                                propertyInfo.SetValue(obj, lstObject);
                            }
                        }
                        else if (_ValueDictionaryField != null)
                        {
                            IDictionary<string, object> _ValueDictionary = _ValueDictionaryField.GetValue(obj) as IDictionary<string, object>;
                            if (_ValueDictionary == null)
                            {
                                _ValueDictionary = new Dictionary<string, object>();
                            }
                            if (!_ValueDictionary.ContainsKey(propertyName))
                            {
                                object value = Read(ref reader, typeof(object), options);
                                _ValueDictionary.Add(propertyName, value);
                                _ValueDictionaryField.SetValue(obj, _ValueDictionary);
                            }
                        }
                    }
                    else if (reader.TokenType == JsonTokenType.StartObject)
                    { 
                        var propertyInfo = typeToConvert.GetProperty(propertyName);
                        if (propertyInfo != null)
                        {
                            var vObj = Read(ref reader, propertyInfo.PropertyType, options);
                            propertyInfo.SetValue(obj, vObj);
                        }
                    }
                }
                else if (reader.TokenType == JsonTokenType.EndObject || reader.TokenType == JsonTokenType.EndArray)
                {
                    break;
                }
            }

            return obj;
        }
        #region 序列化成字符串

        /// <summary>
        /// 写入属性
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }
            WriteArrayValue(writer, "", value);//数组类型
            WriteObjectValue(writer, "", value);//其他复杂类型
        }

        /// <summary>
        /// 写入数组类型属性
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void WriteArrayValue(Utf8JsonWriter writer, string name, object value)
        {
            if (!String.IsNullOrWhiteSpace(name))//没有名称标识顶层 则跳过名称写入
            {
                writer.WritePropertyName(name);
            }
            if (value == null) // 没有值则直接写入 null
            {
                writer.WriteNullValue();
            }

            if (value is IEnumerable)//数组类型
            {
                writer.WriteStartArray();

                var values = value as IEnumerable;

                foreach (var v in values)//递归子对象
                {
                    //这里判断子对象是否为简单类型
                    this.WriteObjectValue(writer, "", v);
                }

                writer.WriteEndArray();

                return;
            }
        }

        /// <summary>
        /// 写入自定义类型属性
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void WriteObjectValue(Utf8JsonWriter writer, string name, object value)
        {
            if (!String.IsNullOrWhiteSpace(name))
            {
                writer.WritePropertyName(name);
            }

            if (value == null)//空值
            {
                writer.WriteNullValue();
                return;
            }

            if (value is NotifyPropertyBase) //NotifyPropertyBase
            {
                var baseType = typeof(NotifyPropertyBase);
                var _ValueDictionaryField = baseType.GetField("_ValueDictionary", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                IDictionary<string, object> _ValueDictionaryObject = (IDictionary<string, object>)_ValueDictionaryField.GetValue(value);

                var valueType = value.GetType();
                var propertyInfos = valueType.GetProperties();

                bool canWriter = false;
                foreach (var _valueObject in _ValueDictionaryObject)//循环写入键值关系
                {
                    var propertyInfo = valueType.GetProperty(_valueObject.Key);
                    if (propertyInfo != null)
                    {
                        var attribute = propertyInfo.GetCustomAttributes(typeof(JsonIgnoreAttribute), true);
                        if (attribute != null || attribute.Length <= 0)//只要有一个没有用JsonIgnoreAttribute标注的属性则设置成true
                        {
                            canWriter = true;
                            break;
                        }
                    }
                    else //只要有一个动态属性则设置成true
                    {
                        canWriter = true;
                        break;
                    }
                }

                if (canWriter)//存在可序列化的属性则写入
                {
                    writer.WriteStartObject();
                    foreach (var _valueObject in _ValueDictionaryObject)//循环写入键值关系
                    {
                        var propertyInfo = valueType.GetProperty(_valueObject.Key);
                        if (propertyInfo != null)
                        {
                            var attribute = propertyInfo.GetCustomAttributes(typeof(JsonIgnoreAttribute), true);
                            if (attribute != null && attribute.Length > 0)//没有被JsonIgnoreAttribute标注的值
                            {
                                continue;
                            }
                        }
                        if (_valueObject.Value != null)
                        {
                            this.WriteValue(writer, _valueObject.Key, _valueObject.Value);
                        }
                    }
                    writer.WriteEndObject();
                }
                else
                {
                    writer.WriteNullValue();
                }
            }
            else  //其他类型
            {
                var valueType = value.GetType();

                var propertyInfos = valueType.GetProperties();

                if (propertyInfos == null
                    || propertyInfos.Length <= 0)
                {
                    this.WriteValue(writer, "", value); //直接写入值
                }
                else
                {
                    bool canWriter = false;
                    foreach (var propertyInfo in propertyInfos)//检查是否存在可序列化的属性
                    {
                        var attribute = propertyInfo.GetCustomAttributes(typeof(JsonIgnoreAttribute), true);
                        if (attribute != null || attribute.Length <= 0)//只要有一个没有用JsonIgnoreAttribute标注的属性则设置成true
                        {
                            canWriter = true;
                            break;
                        }
                    }

                    if (canWriter)//存在可序列化的属性则写入
                    {
                        writer.WriteStartObject();
                        foreach (var propertyInfo in propertyInfos)
                        {
                            var attribute = propertyInfo.GetCustomAttributes(typeof(JsonIgnoreAttribute), true);
                            if (attribute == null || attribute.Length <= 0)//没有被JsonIgnoreAttribute标注的值
                            {
                                var propertyValue = propertyInfo.GetValue(value);
                                this.WriteValue(writer, propertyInfo.Name, propertyValue);
                            }
                        }
                        writer.WriteEndObject();
                    }
                    else
                    {
                        writer.WriteNullValue();
                    }
                }
            }
        }


        /// <summary>
        /// 写入简单类型属性
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void WriteValue(Utf8JsonWriter writer, string name, object value)
        {
            if (!String.IsNullOrWhiteSpace(name))
            {
                writer.WritePropertyName(name);
            }

            if (value == null)//空值
            {
                writer.WriteNullValue();
                return;
            }

            #region 处理简单类型

            //bool
            if (value is bool)
            {
                writer.WriteBooleanValue((bool)value);
            }

            //数字类型
            else if (value is short) //ulong float long decimal int uint double
            {
                writer.WriteNumberValue((short)value);
            }
            else if (value is int) //ulong float long decimal int uint double
            {
                writer.WriteNumberValue((int)value);
            }
            else if (value is float) //ulong float long decimal int uint double
            {
                writer.WriteNumberValue((float)value);
            }
            else if (value is double) //ulong float long decimal int uint double
            {
                writer.WriteNumberValue((double)value);
            }
            else if (value is long) //ulong float long decimal int uint double
            {
                writer.WriteNumberValue((long)value);
            }
            else if (value is decimal) //ulong float long decimal int uint double
            {
                writer.WriteNumberValue((decimal)value);
            }

            //字符串类型
            else if (value is DateTime)
            {
                DateTime v = (DateTime)value;
                writer.WriteStringValue(v);
            }
            else if (value is DateTimeOffset)
            {
                DateTimeOffset v = (DateTimeOffset)value;
                writer.WriteStringValue(v);
            }
            else if (value is JsonEncodedText)
            {
                JsonEncodedText v = (JsonEncodedText)value;
                writer.WriteStringValue(v);
            }
            else if (value is Guid)
            {
                Guid v = (Guid)value;
                writer.WriteStringValue(v);
            }
            else if (value is string)
            {
                writer.WriteStringValue((string)value);
            }

            //数组
            else if (value is IEnumerable)
            {
                WriteArrayValue(writer, "", value);
            }

            //其他类型
            else
            {
                WriteObjectValue(writer, "", value);
            }

            #endregion
        }

        #endregion


        /// <summary>
        /// 将一个对象转换为指定类型
        /// </summary>
        /// <param name="obj">待转换的对象</param>
        /// <param name="type">目标类型</param>
        /// <returns>转换后的对象</returns>
        private object ConvertObject(object obj, Type type)
        {
            if (type == null) return obj;
            if (obj == null) return type.IsValueType ? Activator.CreateInstance(type) : null;

            Type underlyingType = Nullable.GetUnderlyingType(type);
            if (type.IsAssignableFrom(obj.GetType())) // 如果待转换对象的类型与目标类型兼容，则无需转换
            {
                return obj;
            }
            else if ((underlyingType ?? type).IsEnum) // 如果待转换的对象的基类型为枚举
            {
                if (underlyingType != null && string.IsNullOrEmpty(obj.ToString())) // 如果目标类型为可空枚举，并且待转换对象为null 则直接返回null值
                {
                    return null;
                }
                else
                {
                    return Enum.Parse(underlyingType ?? type, obj.ToString());
                }
            }
            else if (typeof(IConvertible).IsAssignableFrom(underlyingType ?? type)) // 如果目标类型的基类型实现了IConvertible，则直接转换
            {
                try
                {
                    return Convert.ChangeType(obj, underlyingType ?? type, null);
                }
                catch
                {
                    return underlyingType == null ? Activator.CreateInstance(type) : null;
                }
            }
            else
            {
                System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(type);
                if (converter.CanConvertFrom(obj.GetType()))
                {
                    return converter.ConvertFrom(obj);
                }
                ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                {
                    object o = constructor.Invoke(null);
                    PropertyInfo[] propertys = type.GetProperties();
                    Type oldType = obj.GetType();
                    foreach (PropertyInfo property in propertys)
                    {
                        PropertyInfo p = oldType.GetProperty(property.Name);
                        if (property.CanWrite && p != null && p.CanRead)
                        {
                            property.SetValue(o, ConvertObject(p.GetValue(obj, null), property.PropertyType), null);
                        }
                    }
                    return o;
                }
            }
            return obj;
        }
    }

}