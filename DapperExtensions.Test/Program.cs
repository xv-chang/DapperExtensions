using System;
using DapperExtensions;
using System.Data;
using DapperExtensions.Test.Model;

namespace DapperExtensions.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            
            IDbConnection conn = null;
            conn.Update<Student>().Where(x => x.Name == "张三")
                .SetField(x => x.Gender).WithValue(23)
                .SetField(x => x.ClassId).WithValue(2)
                .Execute();

            conn.Delete<Student>()
                .Where(x => x.Gender == Gender.男)
                .Execute();


            var stu = new Student()
            {
                Name = "李四",
                Gender = Gender.男,
                Age = 23,
                Birth = DateTime.Now,
                ClassId = 1

            };
            conn.Insert(stu).Execute();



        }
    }
}
