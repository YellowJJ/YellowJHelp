
namespace YellowJHelp.IServer
{
    /// <summary>
    /// 通用方法(范型)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IYJHelpT<T>
    {
        #region YJ版本
        /// <summary>
        /// 集合去重
        /// </summary>
        /// <param name="list">集合</param>
        /// <returns></returns>
        List<T> Distinct(List<T> list);
        /// <summary>
        /// YJ版本：合并两个集合的函数-不允许有重复项
        /// </summary>
        /// <param name="list1">第一个集合</param>
        /// <param name="list2">第二个集合</param>
        /// <returns>返回结果</returns>
        List<T> YJMerge(List<T> list1, List<T> list2);
        #endregion

        #region 原版
        /// <summary>
        /// 将集合按条数分
        /// </summary>
        /// <param name="source">数据集</param>
        /// <param name="pageSiez">每一组数量条</param>
        /// <returns>将集合按照pageSiez拆分数</returns>
        List<List<T>> SpliteSourceBySize(List<T> source, int pageSiez);

        /// <summary> 
        /// 将集合分组
        /// </summary> 
        /// <param name="source">数据集</param> 
        /// <param name="count">组数</param> 
        /// <returns>将集合按照count分组</returns>
        List<List<T>> SpliteSourceByCount(List<T> source, int count);

        /// <summary>
        /// 集合去重(哈西,只针对数值类型)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        List<T> DistinctList(List<T> list);

        /// <summary>
        /// 合并两个集合的函数-不允许有重复项
        /// </summary>
        /// <param name="list1">第一个集合</param>
        /// <param name="list2">第二个集合</param>
        /// <returns>返回第union的合并结果</returns>
        [Obsolete("引用类型建议使用YJ版")]
        List<T> Merge(List<T> list1, List<T> list2);

        /// <summary>
        /// 合并两个集合的函数-允许出现重复项
        /// </summary>
        /// <param name="list1">第一个集合</param>
        /// <param name="list2">第二个集合</param>
        /// <returns>返回第union的合并结果</returns>
        List<T> MergeC(List<T> list1, List<T> list2);


        /// <summary>
        /// 获取差集（集合）
        /// </summary>
        /// <param name="left">左边的数据</param>
        /// <param name="right">右边的数据</param>
        /// <returns></returns>
        List<T>? DiffsetT(List<T> left, List<T> right);

        /// <summary>
        /// 对象副本
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>将对象复制成全新的对象，且不互相影响</returns>
        T? Copy(T data);

        #endregion


    }
}
