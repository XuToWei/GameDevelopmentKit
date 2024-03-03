using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace YIUIBind
{
    [HideLabel]
    [HideReferenceObjectPicker]
    [Serializable]
    public class UIDataValueListInt : UIDataValueBase<List<int>>, IEquatable<UIDataValueListInt>
    {
        public override string ToString()
        {
            return GetValue().ToString();
        }

        public override EUIBindDataType UIBindDataType => EUIBindDataType.List_Int;

        public UIDataValueListInt()
        {
            if (GetValue() == null)
            {
                SetValue(new List<int>());
            }
        }

        public override Type UIDataValueType => typeof(List<int>);

        protected override void SetValueFrom(List<int> value)
        {
            //因为List是引用类型，所以这里要做一下特殊处理，如果要设置的值为0元素的List
            //否则就拷贝全部元素
            GetValue().Clear();
            GetValue().AddRange(value);
        }

        #region 对比函数

        private bool EqualsList(List<int> value)
        {
            if (GetValue().Count != value.Count)
            {
                return false;
            }

            return !GetValue().Where((t, i) => t != value[i]).Any();
        }

        public bool Equals(UIDataValueListInt other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (GetType() != other.GetType())
            {
                return false;
            }

            return EqualsList(other.GetValue());
        }

        protected override bool EqualsValue(List<int> value)
        {
            return EqualsList(value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != UIDataValueType)
            {
                return false;
            }

            var value = (List<int>)obj;

            return EqualsList(value);
        }

        public override int GetHashCode()
        {
            return GetValue().GetHashCode();
        }

        public static bool operator ==(UIDataValueListInt lhs, UIDataValueListInt rhs)
        {
            if (ReferenceEquals(lhs, null))
            {
                if (ReferenceEquals(rhs, null))
                {
                    return true;
                }

                return false;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(UIDataValueListInt lhs, UIDataValueListInt rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator >(UIDataValueListInt lhs, UIDataValueListInt rhs)
        {
            return lhs.GetValue().Count > rhs.GetValue().Count;
        }

        public static bool operator <(UIDataValueListInt lhs, UIDataValueListInt rhs)
        {
            return lhs.GetValue().Count < rhs.GetValue().Count;
        }

        public static bool operator >=(UIDataValueListInt lhs, UIDataValueListInt rhs)
        {
            return lhs.GetValue().Count >= rhs.GetValue().Count;
        }

        public static bool operator <=(UIDataValueListInt lhs, UIDataValueListInt rhs)
        {
            return lhs.GetValue().Count <= rhs.GetValue().Count;
        }

        #endregion
    }
}