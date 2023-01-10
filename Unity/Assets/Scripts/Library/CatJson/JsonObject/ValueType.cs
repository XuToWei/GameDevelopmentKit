using System.Collections;
using System.Collections.Generic;

namespace CatJson
{
    /// <summary>
    /// Json值的类型
    /// </summary>
    public enum ValueType : byte
    {
        Null = 0,
        Boolean = 1,
        Number = 2,
        String = 3,
        Array = 4,
        Object = 5,
    }
}

