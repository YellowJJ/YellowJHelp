using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using YellowJHelp.IServer;

namespace YellowJHelp
{

    /// <summary>
    /// 通用方法(范型)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [AutoInject(typeof(IYJHelpT<>))]
    public class YJHelpT<T>: IYJHelpT<T>
    {
        public List<List<T>> SpliteSourceBySize(List<T> source, int pageSiez)
        {
            int listCount = (source.Count() - 1) / pageSiez + 1;

            // 计算组数 
            List<List<T>> pages = new List<List<T>>();
            for (int pageIndex = 1; pageIndex <= listCount; pageIndex++)
            {
                var page = source.Skip((pageIndex - 1) * pageSiez).Take(pageSiez).ToList(); pages.Add(page);
            }
            return pages;
        }

        public List<List<T>> SpliteSourceByCount(List<T> source, int count)
        {
            int pageSiez = source.Count() / count;//取每一页大小 
            int remainder = source.Count() % count;//取余数 
            List<List<T>> pages = new List<List<T>>();
            for (int pageIndex = 1; pageIndex <= count; pageIndex++)
            {
                if (pageIndex != count)
                {
                    var page = source.Skip((pageIndex - 1) * pageSiez).Take(pageSiez).ToList(); pages.Add(page);
                }
                else
                {
                    var page = source.Skip((pageIndex - 1) * pageSiez).Take(pageSiez + remainder).ToList(); pages.Add(page);
                }
            }
            return pages;
        }

        /// <summary>
        /// 集合去重(哈西,只针对数值类型)
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<T> DistinctList(List<T> list)
        {//去除重复
            HashSet<T> ha = new HashSet<T>(list);
            list.Clear();
            list.AddRange(ha);
            return list;
        }
        /// <summary>
        /// 集合去重
        /// </summary>
        /// <param name="list">集合</param>
        /// <returns></returns>
        public List<T> Distinct(List<T> list)
        {
            List<string> strings = new();
            foreach (var item in list)
            {
                var js = JsonConvert.SerializeObject(item);
                strings.Add(js);
            }
            HashSet<string> ha = new HashSet<string>(strings);
            var re= new List<string>(ha);
            List<T> listRet = new();
            foreach (var item in re) 
            {
                listRet.Add(JsonConvert.DeserializeObject<T>(item));
            }
            
            return listRet;
        }
        
        /// <summary>
        /// YJ版本：合并两个集合的函数-不允许有重复项
        /// </summary>
        /// <param name="list1">第一个集合</param>
        /// <param name="list2">第二个集合</param>
        /// <returns>返回结果</returns>
        public List<T> YJMerge(List<T> list1, List<T> list2)
        {
            List<string> strings = new();
            foreach (var item in list1) {
                var js = JsonConvert.SerializeObject(item);
                strings.Add(js);
            }
            foreach (var item in list2)
            {
                var js = JsonConvert.SerializeObject(item);
                strings.Add(js);
            }

            HashSet<string> ha = new HashSet<string>(strings);
            var re = new List<string>(ha);
            List<T> listRet = new();
            foreach (var item in re)
            {
                listRet.Add(JsonConvert.DeserializeObject<T>(item));
            }
            return listRet;//返回第一项
        }
        /// <summary>
        /// 合并两个集合的函数-不允许有重复项
        /// </summary>
        /// <param name="list1">第一个集合</param>
        /// <param name="list2">第二个集合</param>
        /// <returns>返回第union的合并结果</returns>
        public List<T> Merge(List<T> list1, List<T> list2)
        {
            List<T> listMerge1 = list1.Union(list2).ToList();//不允许有重复项
            //listMerge1:(结果){0,1,2,3,4,5,6,7,8,9}
            //List<T> listMerge2 = list1.Concat(list2).ToList();//允许出现重复项
            //listMerge2:(结果){0,1,2,3,4,5,6,7,8,9}
            return listMerge1;//返回第一项
        }

        /// <summary>
        /// 合并两个集合的函数-允许出现重复项
        /// </summary>
        /// <param name="list1">第一个集合</param>
        /// <param name="list2">第二个集合</param>
        /// <returns>返回第union的合并结果</returns>
        public List<T> MergeC(List<T> list1, List<T> list2)
        {
            List<T> listMerge2 = list1.Concat(list2).ToList();//允许出现重复项
            return listMerge2;//返回
        }


        /// <summary>
        /// 获取差集（集合）
        /// </summary>
        /// <param name="left">左边的数据</param>
        /// <param name="right">右边的数据</param>
        /// <returns></returns>
        public List<T>? DiffsetT(List<T> left, List<T> right)
        {
            if (left == null)
            {
                return null;
            }
            if (right == null)
            {
                return left;
            }

            List<string> resleft = new List<string>();
            List<string> resrig = new List<string>();
            foreach (var le in left)
            {
                resleft.Add(JsonConvert.SerializeObject(le));
            }
            foreach (var le in right)
            {
                resrig.Add(JsonConvert.SerializeObject(le));
            }

            var rstr = resleft.Except(resrig).ToList();


            List<T> res = new List<T>();
            foreach (var le in rstr)
            {
                res.Add(JsonConvert.DeserializeObject<T>(le));
            }

            return res;
        }

        /// <summary>
        /// 对象副本
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>将对象复制成全新的对象，且不互相影响</returns>
        public T? Copy(T data)
        {
            var js = JsonConvert.SerializeObject(data);
            var res = JsonConvert.DeserializeObject<T>(js);
            return res;
        }



    }
}
