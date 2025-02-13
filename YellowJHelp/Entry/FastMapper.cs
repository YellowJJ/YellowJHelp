using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace YellowJHelp.Entry
{
    /// <summary>
    /// 高性能对象映射器
    /// </summary>
    public class FastMapper
    {
        #region 核心缓存结构
        // 缓存映射委托（线程安全）
        private static readonly ConcurrentDictionary<TypePair, Delegate> _cache = new();

        // 类型对键（源类型 + 目标类型）
        private readonly struct TypePair(Type source, Type target)
        {
            public Type Source { get; } = source;
            public Type Target { get; } = target;
        }
        #endregion

        #region 公共接口
        /// <summary>
        /// 对象映射方法
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <returns>映射后的新对象</returns>
        public TTarget Adapt<TSource, TTarget>(TSource source)
            where TSource : class
            where TTarget : class, new()
        {
            var key = new TypePair(typeof(TSource), typeof(TTarget));

            // 从缓存获取或创建映射委托
            var func = (Func<TSource, TTarget>)_cache.GetOrAdd(key, _ =>
            {
                // 表达式树构建映射逻辑
                var param = Expression.Parameter(typeof(TSource), "src");
                var newExpr = Expression.New(typeof(TTarget));
                var bindings = CreatePropertyBindings(param, typeof(TTarget));

                var memberInit = Expression.MemberInit(newExpr, bindings);
                var lambda = Expression.Lambda<Func<TSource, TTarget>>(memberInit, param);

                // JIT 编译成机器码（性能关键）
                return lambda.Compile();
            });

            return func(source);
        }
        #endregion

        #region 表达式树构建
        /// <summary>
        /// 创建属性绑定表达式集合（性能优化点）
        /// </summary>
        private static MemberBinding[] CreatePropertyBindings(Expression source, Type targetType)
        {
            var targetProps = targetType.GetProperties()
                .Where(p => p.CanWrite);

            var bindings = new List<MemberBinding>();

            foreach (var targetProp in targetProps)
            {
                var sourceProp = source.Type.GetProperty(targetProp.Name);
                if (sourceProp == null) continue;

                // 生成属性赋值表达式：target.Prop = source.Prop
                var sourcePropExpr = Expression.Property(source, sourceProp);
                var binding = Expression.Bind(
                    targetProp,
                    Expression.Convert(sourcePropExpr, targetProp.PropertyType)
                );
                bindings.Add(binding);
            }

            return bindings.ToArray();
        }
        #endregion
    }
}
