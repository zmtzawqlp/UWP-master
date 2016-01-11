using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XamlDemo.Model
{
    public class TestData
    {
        /// <summary>
        /// 返回一个 Employee 数据集合，可用于测试
        /// </summary>
        public static List<Employee> GetEmployees()
        {
            var employees = new List<Employee>();

            for (int i = 0; i < 1000; i++)
            {
                employees.Add(
                    new Employee
                    {
                        Name = "Name " + i.ToString().PadLeft(4, '0'),
                        Age = new Random(i).Next(20, 60),
                        IsMale = Convert.ToBoolean(i % 2)
                    });
            }

            return employees;
        }
    }
}
