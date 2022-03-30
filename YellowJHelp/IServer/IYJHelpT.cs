
namespace YellowJHelp.IServer
{
    /// <summary>
    /// 通用方法(范型)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IYJHelpT<T>
    {
        /// <summary>
        /// 将集合按大小分
        /// </summary>
        /// <param name="source">数据集</param>
        /// <param name="pageSiez">每一组大小</param>
        List<List<T>> SpliteSourceBySize(List<T> source, int pageSiez);

        /// <summary> 
        /// 将集合安按组数分
        /// </summary> 
        /// <param name="source">数据集</param> 
        /// <param name="count">组数</param> 
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
        List<T> DiffsetT(List<T> left, List<T> right);




    }
}
