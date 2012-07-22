using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Migrations.Tests.SampleAssembly
{
    public class FirstView
    {
        public long FirstViewId { get; set; }
        public Guid EntityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class SecondView
    {
        public long SecondViewId { get; set; }
        public Guid EntityId { get; set; }
        public string Description { get; set; }
        public string Synopsis { get; set; }
    }

    public class ThirdView
    {
        public long ThirdViewId { get; set; }
        public Guid EntityId { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
    }
}
