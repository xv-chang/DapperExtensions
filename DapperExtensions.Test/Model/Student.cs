using System;
using System.Collections.Generic;
using System.Text;
using DapperExtensions.FluentMap.Mapping;

namespace DapperExtensions.Test.Model
{
    public class Student
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public Gender Gender { set; get; }
        public int Age { set; get; }
        public DateTime? Birth { set; get; }
        public int ClassId { set; get; }
        public ClassRoom ClassRoom { set; get; }
    }

    public enum Gender
    {
        男,
        女,
        保密
    }
    public class StudentMap:EntityMap<Student>
    {
        public StudentMap()
        {
            Table("Student");
            Id(x => x.Id).Column("StudentId");
            Map(x => x.Name).Column("Name");
            Map(x => x.Gender).Column("Gender");
            Map(x => x.Age).Column("Age");
            Map(x => x.Birth).Column("Birth");
            Map(x => x.ClassId).Column("ClassId");

            HasOne(x => x.ClassRoom).Table("ClassRoom").Column("ClassId");


        }
    }
}
