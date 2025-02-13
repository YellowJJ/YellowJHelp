namespace YellowJAutoInjection
{
    /// <summary>
    /// 依赖注入基类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AutoInjectAttribute : Attribute
    {

        /// <summary>
        /// 绑定对应接口，自动注入程序集
        /// </summary>
        /// <param name="interfaceType">接口</param>
        /// <param name="injectType">生命周期</param>
        public AutoInjectAttribute(Type interfaceType, InjectType injectType = InjectType.Scope)
        {
            Type = interfaceType;
            InjectType = injectType;
        }
        /// <summary>
        /// 接口类型
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 注入类型
        /// </summary>
        public InjectType InjectType { get; set; }
    }

    /// <summary>
    /// 注入类型（可控生命周期）
    /// </summary>
    public enum InjectType
    {
        /// <summary>
        /// 作用域
        /// </summary>
        Scope,
        /// <summary>
        /// 单例
        /// </summary>
        Single,
        /// <summary>
        /// 暂时/瞬时
        /// </summary>
        Transient
    }
}
