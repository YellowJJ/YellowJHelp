using System;
using System.Collections.Generic;
using System.Text;

namespace YellowJHelp.Entry
{
    /// <summary>
    /// 数据分配类
    /// </summary>
    public class YAllocationInfo
    {
        /// <summary>
        /// 数据唯一标识
        /// </summary>
        public List<string> Number { get; set; }
        /// <summary>
        /// 分配标识：分配对比条件
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 分配数量/可分配数量
        /// </summary>
        public decimal Qty { get; set; }
        /// <summary>
        /// 分配数量计算重点：剩余数量：请必填
        /// </summary>
        public decimal RemQty { get; set; }
        /// <summary>
        /// 已分配数量
        /// </summary>
        public decimal AQty { get; set; }
        /// <summary>
        /// 已分配标识
        /// </summary>
        public List<string> FPNumber { get; set; }
    }
}
