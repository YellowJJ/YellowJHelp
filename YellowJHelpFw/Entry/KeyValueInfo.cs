﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YellowJHelpFw.Entry
{
    /// <summary>
    /// 键值类
    /// </summary>
    public class KeyValueInfo<T, T1>
    {
        /// <summary>
        /// 键
        /// </summary>
        public T Key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public T1 Value { get; set; }
    }
}
