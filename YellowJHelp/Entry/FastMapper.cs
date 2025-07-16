using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace YellowJHelp.Entry
{
    /// <summary>
    ///  高性能对象映射器（Mapster风格，支持属性名匹配、类型兼容、集合映射、嵌套对象、缓存优化，兼容netstandard2.0）
    /// </summary>
    /// 优势与市场主流对比
    ///自动属性名匹配、类型兼容、集合/嵌套递归：无需配置，适合绝大多数DTO/VO/POCO映射。
    ///性能高：表达式树+委托缓存，和Mapster/AutoMapper同级，远高于反射。
    ///零依赖、轻量级：无外部包，兼容netstandard2.0，适合微服务、工具库、性能敏感场景。
    ///Mapster/AutoMapper：支持复杂配置、条件映射、自定义规则、深度嵌套、集合等，适合复杂业务。
    ///本实现：专注零配置、自动递归、类型安全，适合绝大多数常规对象映射，维护成本低。
    public static class FastMapper
        {
        #region 内部核心缓存结构
        // 缓存映射委托（线程安全，提升多次映射性能）
        private static readonly ConcurrentDictionary<TypePair, Delegate> _cache = new ConcurrentDictionary<TypePair, Delegate>();

        // 类型对键（源类型 + 目标类型）
        private struct TypePair : IEquatable<TypePair>
        {
            public Type Source { get; }
            public Type Target { get; }
            public TypePair(Type source, Type target)
            {
                Source = source;
                Target = target;
            }
            public bool Equals(TypePair other) => Source == other.Source && Target == other.Target;
            public override bool Equals(object obj) => obj is TypePair other && Equals(other);
            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + (Source?.GetHashCode() ?? 0);
                    hash = hash * 23 + (Target?.GetHashCode() ?? 0);
                    return hash;
                }
            }
        }
        #endregion
        #region Mapster风格扩展方法

        /// <summary>
        /// 将源对象映射为新对象（Mapster风格，source.Adapt&lt;TTarget&gt;()）
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <returns>新目标对象</returns>
        /// <remarks>
        /// 优势：
        /// 1. 用法与Mapster一致，支持 source.Adapt&lt;TTarget&gt;()。
        /// 2. 支持属性名自动匹配、类型自动转换、嵌套对象、集合递归映射。
        /// 3. 性能高，表达式树+委托缓存，适合高并发。
        /// 4. 兼容netstandard2.0，无外部依赖。
        /// 5. 轻量级，易维护，适合DTO/VO/POCO等常规对象映射。
        /// 
        /// 与市场主流对比：
        /// - Mapster/AutoMapper支持复杂配置、条件映射、自定义规则、深度嵌套、集合等，适合复杂业务。
        /// - 本实现专注于零配置、自动属性名匹配、类型兼容、集合/嵌套对象自动递归，适合绝大多数DTO/VO场景。
        /// - 性能与Mapster接近，远高于反射，零依赖，适合对性能和体积有要求的项目。
        /// </remarks>
        public static TTarget Adapt<TSource, TTarget>(this TSource source)
            where TSource : class
            where TTarget : class, new()
        {
            if (source == null) return null;
            var key = new TypePair(typeof(TSource), typeof(TTarget));
            var func = (Func<TSource, TTarget>)_cache.GetOrAdd(key, _ =>
            {
                var param = Expression.Parameter(typeof(TSource), "src");
                var body = BuildMapExpression(typeof(TSource), typeof(TTarget), param);
                var lambda = Expression.Lambda<Func<TSource, TTarget>>(body, param);
                return lambda.Compile();
            });
            return func(source);
        }

        /// <summary>
        /// 将源对象映射到已存在的目标对象（属性覆盖，source.Adapt(dest)）
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="destination">目标对象（属性会被覆盖）</param>
        /// <returns>目标对象（同传入实例）</returns>
        /// <remarks>
        /// - 用法与Mapster一致，支持 source.Adapt(dest)。
        /// - 只会覆盖目标对象中与源对象同名且类型兼容的属性。
        /// - 适合更新已有对象（如EF实体、ViewModel等）。
        /// </remarks>
        public static TTarget Adapt<TSource, TTarget>(this TSource source, TTarget destination)
            where TSource : class
            where TTarget : class
        {
            if (source == null || destination == null) return destination;
            var key = new TypePair(typeof(TSource), typeof(TTarget));
            var func = (Action<TSource, TTarget>)_cache.GetOrAdd(key, _ =>
            {
                var srcParam = Expression.Parameter(typeof(TSource), "src");
                var destParam = Expression.Parameter(typeof(TTarget), "dest");
                var assigns = new List<Expression>();

                var targetProps = typeof(TTarget).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanWrite && p.GetSetMethod() != null);

                foreach (var targetProp in targetProps)
                {
                    var sourceProp = typeof(TSource).GetProperty(targetProp.Name, BindingFlags.Public | BindingFlags.Instance);
                    if (sourceProp == null || !sourceProp.CanRead) continue;

                    Expression valueExpr = Expression.Property(srcParam, sourceProp);
                    var sourcePropType = sourceProp.PropertyType;
                    var targetPropType = targetProp.PropertyType;

                    // 类型兼容处理
                    if (targetPropType == sourcePropType)
                    {
                        // null安全
                        valueExpr = AddNullSafe(srcParam, valueExpr, targetPropType);
                    }
                    else if (IsNullableType(targetPropType) && Nullable.GetUnderlyingType(targetPropType) == sourcePropType)
                    {
                        valueExpr = Expression.Convert(valueExpr, targetPropType);
                        valueExpr = AddNullSafe(srcParam, valueExpr, targetPropType);
                    }
                    else if (IsNullableType(sourcePropType) && Nullable.GetUnderlyingType(sourcePropType) == targetPropType)
                    {
                        valueExpr = Expression.Condition(
                            Expression.Equal(valueExpr, Expression.Constant(null, sourcePropType)),
                            Expression.Default(targetPropType),
                            Expression.Convert(valueExpr, targetPropType)
                        );
                        valueExpr = AddNullSafe(srcParam, valueExpr, targetPropType);
                    }
                    else if (IsSimpleType(sourcePropType) && IsSimpleType(targetPropType))
                    {
                        valueExpr = Expression.Convert(valueExpr, targetPropType);
                        valueExpr = AddNullSafe(srcParam, valueExpr, targetPropType);
                    }
                    else if (IsClassType(sourcePropType) && IsClassType(targetPropType))
                    {
                        var method = typeof(FastMapper).GetMethod(nameof(Adapt), new[] { sourcePropType, targetPropType });
                        valueExpr = Expression.Call(method, valueExpr, Expression.Property(destParam, targetProp));
                    }
                    else if (IsEnumerableType(sourcePropType, out var srcElemType) && IsEnumerableType(targetPropType, out var tgtElemType))
                    {
                        var method = typeof(FastMapper).GetMethod(nameof(MapCollection), BindingFlags.NonPublic | BindingFlags.Static)
                            .MakeGenericMethod(srcElemType, tgtElemType);
                        valueExpr = Expression.Call(method, valueExpr);
                    }
                    else
                    {
                        continue;
                    }

                    assigns.Add(Expression.Assign(Expression.Property(destParam, targetProp), valueExpr));
                }

                var body = Expression.Block(assigns);
                return Expression.Lambda<Action<TSource, TTarget>>(body, srcParam, destParam).Compile();
            });
            func(source, destination);
            return destination;
        }

        /// <summary>
        /// 将源集合映射为目标集合（Mapster风格，sources.Adapt&lt;List&lt;TTarget&gt;&gt;()）
        /// </summary>
        /// <typeparam name="TSource">源集合元素类型</typeparam>
        /// <typeparam name="TTarget">目标集合元素类型</typeparam>
        /// <param name="sourceList">源集合</param>
        /// <returns>目标集合</returns>
        public static List<TTarget> Adapt<TSource, TTarget>(this IEnumerable<TSource> sourceList)
            where TSource : class
            where TTarget : class, new()
        {
            if (sourceList == null) return null;
            var result = new List<TTarget>();
            foreach (var item in sourceList)
            {
                result.Add(item.Adapt<TSource, TTarget>());
            }
            return result;
        }

        #endregion
        #region 表达式树构建核心
        // 构建对象映射表达式（支持嵌套对象、集合、类型兼容、null安全）
        private static Expression BuildMapExpression(Type sourceType, Type targetType, Expression sourceExpr)
        {
            if (IsEnumerableType(sourceType, out var sourceElemType) && IsEnumerableType(targetType, out var targetElemType))
            {
                var method = typeof(FastMapper).GetMethod(nameof(MapCollection), BindingFlags.NonPublic | BindingFlags.Static)
                    .MakeGenericMethod(sourceElemType, targetElemType);
                return Expression.Call(method, sourceExpr);
            }

            var newExpr = Expression.New(targetType);
            var bindings = new List<MemberBinding>();

            var targetProps = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && p.GetSetMethod() != null);

            foreach (var targetProp in targetProps)
            {
                var sourceProp = sourceType.GetProperty(targetProp.Name, BindingFlags.Public | BindingFlags.Instance);
                if (sourceProp == null || !sourceProp.CanRead) continue;

                Expression valueExpr = Expression.Property(sourceExpr, sourceProp);
                var sourcePropType = sourceProp.PropertyType;
                var targetPropType = targetProp.PropertyType;

                if (targetPropType == sourcePropType)
                {
                    valueExpr = AddNullSafe(sourceExpr, valueExpr, targetPropType);
                }
                else if (IsNullableType(targetPropType) && Nullable.GetUnderlyingType(targetPropType) == sourcePropType)
                {
                    valueExpr = Expression.Convert(valueExpr, targetPropType);
                    valueExpr = AddNullSafe(sourceExpr, valueExpr, targetPropType);
                }
                else if (IsNullableType(sourcePropType) && Nullable.GetUnderlyingType(sourcePropType) == targetPropType)
                {
                    valueExpr = Expression.Condition(
                        Expression.Equal(valueExpr, Expression.Constant(null, sourcePropType)),
                        Expression.Default(targetPropType),
                        Expression.Convert(valueExpr, targetPropType)
                    );
                    valueExpr = AddNullSafe(sourceExpr, valueExpr, targetPropType);
                }
                else if (IsSimpleType(sourcePropType) && IsSimpleType(targetPropType))
                {
                    valueExpr = Expression.Convert(valueExpr, targetPropType);
                    valueExpr = AddNullSafe(sourceExpr, valueExpr, targetPropType);
                }
                else if (IsClassType(sourcePropType) && IsClassType(targetPropType))
                {
                    var method = typeof(FastMapper).GetMethod(nameof(Adapt), new[] { sourcePropType, targetPropType });
                    valueExpr = Expression.Call(method, valueExpr);
                }
                else if (IsEnumerableType(sourcePropType, out var srcElemType) && IsEnumerableType(targetPropType, out var tgtElemType))
                {
                    var method = typeof(FastMapper).GetMethod(nameof(MapCollection), BindingFlags.NonPublic | BindingFlags.Static)
                        .MakeGenericMethod(srcElemType, tgtElemType);
                    valueExpr = Expression.Call(method, valueExpr);
                }
                else
                {
                    continue;
                }

                bindings.Add(Expression.Bind(targetProp, valueExpr));
            }

            return Expression.MemberInit(newExpr, bindings);
        }

        // 集合类型递归映射（表达式树调用）
        private static List<TTarget> MapCollection<TSource, TTarget>(IEnumerable<TSource> sourceList)
            where TSource : class
            where TTarget : class, new()
        {
            if (sourceList == null) return null;
            var result = new List<TTarget>();
            foreach (var item in sourceList)
            {
                result.Add(item.Adapt<TSource, TTarget>());
            }
            return result;
        }

        // null安全处理（源对象为null时目标属性为默认值）
        private static Expression AddNullSafe(Expression source, Expression valueExpr, Type targetType)
        {
            if (source.Type.IsClass)
            {
                return Expression.Condition(
                    Expression.Equal(source, Expression.Constant(null, source.Type)),
                    Expression.Default(targetType),
                    valueExpr
                );
            }
            return valueExpr;
        }

        // 判断是否为简单类型（值类型、字符串、可空类型）
        private static bool IsSimpleType(Type type)
        {
            var t = Nullable.GetUnderlyingType(type) ?? type;
            return t.IsPrimitive || t.IsEnum || t == typeof(string) || t == typeof(decimal) || t == typeof(DateTime);
        }

        // 判断是否为可空类型
        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        // 判断是否为集合类型（支持List<T>、T[]）
        private static bool IsEnumerableType(Type type, out Type elementType)
        {
            if (type.IsArray)
            {
                elementType = type.GetElementType();
                return true;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }
            elementType = null;
            return false;
        }

        // 判断是否为可递归映射的类（非string，非简单类型）
        private static bool IsClassType(Type type)
        {
            return type.IsClass && type != typeof(string);
        }
        #endregion
    }
}
