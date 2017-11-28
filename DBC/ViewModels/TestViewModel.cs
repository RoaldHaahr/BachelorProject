using System.Collections.Generic;
using DBC.Models;

namespace DBC.ViewModels
{
    public class TestViewModel
    {
        public List<TestModel> TestResultList { get; set; }
        public double MinTime { get; set; }
        public double MaxTime { get; set; }
        public double AverageTime { get; set; }
        public double AverageTimeMinFirst { get; set; }

        public TestViewModel()
        {
            TestResultList = new List<TestModel>();
        }
    }
}