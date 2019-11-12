using System;
using System.Collections.Generic;
using System.Text;
using WpfApp1.MVVM;

namespace WpfApp1
{
    /// <summary>
    /// 
    /// </summary>
    public class TestModel : NotifyBaseModel
    {
        public TestModel()
        {
            ID = Guid.NewGuid().ToString("N");
            Test1 = new int[] { 10, 20, 30 };
        }

        /// <summary>
        /// 哈哈
        /// </summary>
        public string ID
        {
            get
            {
                return this.GetValue<string>();
            }
            set
            {
                this.SetValue(value);
            }
        }

        /// <summary>
        /// 列表
        /// </summary>
        public int[] Test1
        {
            get
            {
                return this.GetValue<int[]>();
            }
            set
            {
                this.SetValue(value);
            }
        }

        //public List<TestModel> TestModels
        //{
        //    get
        //    {
        //        return this.GetValue<List<TestModel>>();
        //    }
        //    set
        //    {
        //        this.SetValue(value);
        //    }
        //}
         
    }
}
