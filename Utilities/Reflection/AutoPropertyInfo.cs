using System;
using System.Globalization;
using System.Reflection;

namespace AfterTheEnd.Utilities.Reflection
{
    public class AutoPropertyInfo : AutoMemberInfo
    {
        private PropertyInfo value;

        public Type Type { get; set; }
        
        public string Name { get; set; }
        
        public BindingFlags Flags { get; set; }

        public PropertyInfo Value
        {
            get
            {
                if (value == null)
                {
                    value = Type.GetProperty(Name, Flags);

                    if (value == null) throw new Exception("Not found property!");
                }

                return value;
            }
        }

        public PropertyAttributes Attributes => Value.Attributes;

        public Type DeclaringType => Value.DeclaringType;

        public string MethodName => Value.Name;

        public Type ReflectedType => Value.ReflectedType;

        public AutoPropertyInfo(Type type, string name, BindingFlags flags)
        {
            if (type == null) throw new ArgumentNullException("type");

            Type = type;
            Name = name;
            Flags = flags;
        }

        public object[] GetCustomAttributes(bool inherit)
        {
            return Value.GetCustomAttributes(inherit);
        }

        public object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return Value.GetCustomAttributes(attributeType, inherit);
        }

        public bool IsDefined(Type attributeType, bool inherit)
        {
            return Value.IsDefined(attributeType, inherit);
        }
    }
}
