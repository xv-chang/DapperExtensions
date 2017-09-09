using System;
using DapperExtensions;
using System.Data;
using DapperExtensions.Test.Model;
using DapperExtensions.Query;
using MySql.Data.MySqlClient;
using DapperExtensions.FluentMap.Mapping;


namespace DapperExtensions.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            MapConfig.AddMap(new StudentMap());
            MapConfig.AddMap(new ClassRoomMap());
            using (var conn = new MySqlConnection("server=127.0.0.1;uid=root;pwd=1234;database=test"))
            {
                //conn.Update<Student>().Where(x => x.Id == 3)
                //   .SetField(x=>x.Name=="张三")
                //   .SetField(x => x.Gender == Gender.女)
                //   .SetField(x => x.ClassId == 1)
                //   .Execute();

                conn.Delete<Student>()
                    .Where(x => x.Id==1)
                    .Execute();

                //for (int i = 0; i < 1000; i++)
                //{
                //    var stu = new Student()
                //    {
                //        Name = $"李四{i}",
                //        Gender = Gender.男,
                //        Age = 23,
                //        Birth = DateTime.Now
                //    };
                //    conn.Insert(stu).Execute();
                //}




                //var list= conn.CreateQuery<Student>()
                //    .Where(x => x.Id ==3)
                //    .ToList();

                //list.ForEach(x => Console.WriteLine(x.Name));

                Console.ReadLine();
              

            }


        }
    }
}
