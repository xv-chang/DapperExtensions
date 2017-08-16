using System;
using System.Collections.Generic;
using System.Text;
using DapperExtensions.FluentMap.Mapping;

namespace DapperExtensions.Test.Model
{
    public class ClassRoom
    {
        public int Id { set; get; }
        public string ClassName { set; get; }
        public int Member { set; get; }

    }

    public class ClassRoomMap:EntityMap<ClassRoom>
    {
        public ClassRoomMap()
        {
            Table("ClassRoom");
            Id(x => x.Id);
            Map(x => x.ClassName).Column("ClassName");
            Map(x => x.Member).Column("Member");
        }
    }
}
