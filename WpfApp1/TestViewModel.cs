using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using WpfApp1.Command;
using WpfApp1.MVVM;

namespace WpfApp1
{
    /// <summary>
    /// 
    /// </summary>
    public class TestViewModel
    {
        /// <summary>
        /// ViewModel
        /// </summary>
        public dynamic ViewModel { get; set; }

        public TestViewModel()
        {
            ViewModel = new TestModel();
            ViewModel.DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            ViewModel.TestModels = new List<TestModel>();
            ViewModel.TestModels.Add(new TestModel());
            ViewModel.TestModels.Add(new TestModel());
            ViewModel.TestModel1 = new TestModel();
        }

        public ICommand ButtonCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    ViewModel.AAAA = "ViewModel.AAAA";
                    ViewModel.BBBBB = "ViewModel.BBBBB";
                    ViewModel.TestModel1.CCCCCCC = "ViewModel.TestModel1.CCCCCCC";
                    ViewModel.ErrorMessage = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string viewModelTest = System.Text.Json.JsonSerializer.Serialize(ViewModel, new System.Text.Json.JsonSerializerOptions() { Converters = { new DynamicJsonConverter() } });
                    dynamic notifyBaseModel = System.Text.Json.JsonSerializer.Deserialize<TestModel>(viewModelTest, new System.Text.Json.JsonSerializerOptions() { Converters = { new DynamicJsonConverter() } });
                     
                    ViewModel.DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                });
            }
        }
    }
}
