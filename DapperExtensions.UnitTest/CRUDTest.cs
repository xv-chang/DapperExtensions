using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;
using DapperExtensions.UnitTest.Model;
using DapperExtensions.Query;
using DapperExtensions.FluentMap.Mapping;

namespace DapperExtensions.UnitTest
{
    public class CRUDTest
    {
        private int[] ages = { 20,21,22,23,24,25,26,27,28,29 };

        public CRUDTest()
        {
            MapConfig.AddMap(new StudentMap());
            MapConfig.AddMap(new ClassRoomMap());
        }

        private IDbConnection GetConn()
        {
            return new MySqlConnection("server=127.0.0.1;uid=root;pwd=1234;database=test");
        }
        [Fact]
        public void Test_Delete()
        {
            using (var conn = GetConn())
            {
                conn.Delete<Student>().Execute();
                var count = conn.ExecuteScalar<int>("select count(1) from student");
                Assert.Equal(count, 0);
            }
        }

        [Fact]

        public void Test_Insert()
        {
            using (var conn = GetConn())
            {
                conn.Delete<Student>().Execute();
                for (int i = 0; i < 1000; i++)
                {
                    var stu = new Student()
                    {
                        Name = $"李四{i}",
                        Gender = i % 2 == 1 ? Gender.男 : Gender.女,
                        Age = ages[i%10],
                        Birth = DateTime.Now
                    };
                    conn.Insert(stu).Execute();
                }
                var count = conn.ExecuteScalar<int>("select count(1) from student");
                Assert.Equal(count, 1000);
            }
        }
        [Fact]
        public void TestUpdate()
        {
            using (var conn = GetConn())
            {
                conn.Update<Student>().Where(x => x.Gender==Gender.男)
                   .SetField(x => x.Name == "张三")
                   .SetField(x => x.Gender == Gender.女)
                   .SetField(x => x.ClassId == 1)
                   .Execute();
                var count = conn.ExecuteScalar<int>("select count(1) from student where name='张三'");
                Assert.Equal(count, 500);
            }
        }
    }
}
