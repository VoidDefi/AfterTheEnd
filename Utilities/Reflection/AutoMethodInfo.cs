using System;
using System.Globalization;
using System.Reflection;

namespace AfterTheEnd.Utilities.Reflection
{
    public class AutoMethodInfo : AutoMemberInfo
    {
        private MethodInfo value;

        public Type Type { get; set; }
        
        public string Name { get; set; }
        
        public BindingFlags Flags { get; set; }

        public Type[] Arguments { get; set; }

        public MethodInfo Value
        {
            get
            {
                if (value == null)
                {
                    if (Arguments != null)
                        value = Type.GetMethod(Name, Flags, Arguments);

                    else 
                        value = Type.GetMethod(Name, Flags);

                    if (value == null) throw new Exception("Not found method!");
                }

                return value;
            }
        }

        public ICustomAttributeProvider ReturnTypeCustomAttributes => Value.ReturnTypeCustomAttributes;

        public MethodAttributes Attributes => Value.Attributes;

        public RuntimeMethodHandle MethodHandle => Value.MethodHandle;

        public Type DeclaringType => Value.DeclaringType;

        public string MethodName => Value.Name;

        public Type ReflectedType => Value.ReflectedType;

        public AutoMethodInfo(Type type, string name, BindingFlags flags, Type[] arguments = null)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("name");

            Type = type;
            Name = name;
            Flags = flags;
            Arguments = arguments;
        }

        public object? Invoke(object? obj, object?[]? parameters)
        {
            return Value.Invoke(obj, parameters);
        }

        public object? Invoke(object? obj, BindingFlags invokeAttr, Binder? binder, object?[]? parameters, CultureInfo culture)
        {
            return Value.Invoke(obj, invokeAttr, binder, parameters, culture);
        }

        public MethodInfo GetBaseDefinition()
        {
            return Value.GetBaseDefinition();
        }

        public MethodImplAttributes GetMethodImplementationFlags()
        {
            return Value.GetMethodImplementationFlags();
        }

        public ParameterInfo[] GetParameters()
        {
            return Value.GetParameters();
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
