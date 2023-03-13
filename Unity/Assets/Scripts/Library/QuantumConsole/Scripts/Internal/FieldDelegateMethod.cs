using QFSW.QC.Utilities;
using System;
using System.Globalization;
using System.Reflection;

namespace QFSW.QC.Internal
{
    internal class FieldDelegateMethod : FieldMethod
    {
        public FieldDelegateMethod(FieldInfo fieldInfo) : base(fieldInfo)
        {
            if (!_fieldInfo.IsStrongDelegate())
            {
                throw new ArgumentException("Invalid delegate type.", nameof(fieldInfo));
            }

            if (_fieldInfo.IsStatic)
            {
                _internalDelegate = (Func<FieldInfo, object[], object>)StaticInvoker;
            }
            else
            {
                _internalDelegate = (Func<object, FieldInfo, object[], object>)NonStaticInvoker;
            }

            _parameters = _fieldInfo.FieldType.GetMethod("Invoke").GetParameters();
            for (int i = 0; i < _parameters.Length; i++)
            {
                _parameters[i] = new CustomParameter(_parameters[i], $"arg{i}");
            }
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
            object[] realParameters = new object[_internalDelegate.Method.GetParameters().Length];
            if (realParameters.Length < 2) { throw new Exception("FieldDelegateMethod's internal delegate must contain at least two paramaters."); }
            if (!IsStatic) { realParameters[0] = obj; }
            realParameters[realParameters.Length - 2] = _fieldInfo;
            realParameters[realParameters.Length - 1] = parameters;
            return _internalDelegate.DynamicInvoke(realParameters);
        }

        private static object StaticInvoker(FieldInfo field, params object[] args)
        {
            Delegate del = (Delegate)field.GetValue(null);
            if (del != null) { return del.DynamicInvoke(args); }
            else { throw new Exception("Delegate was invalid and could not be invoked."); }
        }

        private object NonStaticInvoker(object obj, FieldInfo field, params object[] args)
        {
            Delegate del = (Delegate)field.GetValue(obj);
            if (del != null) { return del.DynamicInvoke(args); }
            else { throw new Exception("Delegate was invalid and could not be invoked."); }
        }
    }
}
