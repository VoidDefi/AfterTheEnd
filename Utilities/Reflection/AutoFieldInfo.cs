using FullSerializer.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AfterTheEnd.Utilities.Reflection
{
    public class AutoFieldInfo : AutoMemberInfo
    {
        private FieldInfo value;

        private bool useLinq = false;

        public Type Type { get; set; }
        
        public string Name { get; set; }
        
        public BindingFlags Flags { get; set; }

        public FieldInfo Value
        {
            get
            {
                if (value == null)
                {
                    if (useLinq)
                    {
                        List<FieldInfo> fields = Type.GetDeclaredFields().ToList();

                        value = fields.Find(f => f.Name == Name);
                    }

                    else value = Type.GetField(Name, Flags);

                    if (value == null) throw new Exception("Not found field!");
                }

                return value;
            }
        }

        public AutoFieldInfo(Type type, string name, BindingFlags? flags = null)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("name");

            Type = type;
            Name = name;
            Flags = flags ?? BindingFlags.Default;
            useLinq = !flags.HasValue;
        }
    }
}
