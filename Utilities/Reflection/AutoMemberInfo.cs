using System;
using System.Reflection;

namespace AfterTheEnd.Utilities.Reflection
{
    public interface AutoMemberInfo
    {
        public abstract Type Type { get;  set; }

        public abstract string Name { get;  set; }

        public abstract BindingFlags Flags { get; set; }
    }
}
