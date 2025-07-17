# YellowJAutoInjection & YellowJHelp

---

## 简介

**YellowJAutoInjection** 和 **YellowJHelp** 是一套面向 .NET 平台的高性能、低依赖、易扩展的开发工具库，涵盖自动依赖注入、对象映射、缓存、分布式ID、日志、加解密、集合算法等常用功能，适配 .NET Framework 4.6.1、.NET Standard 2.0/2.1/9。

---

## YellowJAutoInjection

### 依赖注入自动化

- **零配置自动注入**，通过特性标记和程序集扫描，自动完成服务注册。
- **兼容主流IOC容器**，支持 .NET Core/6/7/8/9 及 .NET Framework。
- **极简用法**，无需手动注册服务，提升开发效率。
- ---

### 主要方法说明

| 方法名                | 参数说明                | 返回值         | 功能描述                   |
|-----------------------|------------------------|----------------|----------------------------|
| YJDiInJect            | -                      | void           | 自动扫描并注入所有特性服务 |
| [AutoInject] 特性     | Type                   | -              | 标记自动注入的接口或类     |

---

#### 快速上手

案列：Net6

`builder.Services.YJDiInJect();`

![输入图片说明](1.png)


类库中满足如：

`[AutoInject(typeof(ILoginServer))]`

声明的都将自动注入程序集

![输入图片说明](2.png)

---
## YellowJHelp

### 通用开发工具库

- **高性能对象映射器（FastMapper）**：属性名自动匹配、类型兼容、集合/嵌套递归，零依赖，性能极高。
- **多线程集合处理**：支持大数据量分配、去重、合并等操作。
- **缓存/分布式ID/日志/加解密**：一站式解决常见开发需求。
- **异步优先**：3.0.0+ 版本方法基本异步，适合现代高并发场景。

---

### 主要方法说明

#### YJHelp（核心方法）

| 方法名                | 参数说明                                                     | 返回值                        | 功能详细描述                                           |
|-----------------------|-------------------------------------------------------------|-------------------------------|--------------------------------------------------------|
| **加密模块**          |                                                              |                               |                                                        |
| MD5EncryptAsync       | strText, IsLower                                            | Task<string>                  | 生成32位MD5哈希值，适用于密码存储/数据校验             |
| EncodeAsync           | data, KEY_64, IV_64                                         | Task<string>                  | DES对称加密，用于敏感数据传输                          |
| DecodeAsync           | data, KEY_64, IV_64                                         | Task<string>                  | DES解密，需与加密使用相同密钥                          |
| **日志与文件**        |                                                              |                               |                                                        |
| YellowJLogAsync       | text, address                                               | Task                          | 按日期自动创建日志文件，UTF8编码存储                   |
| MidStrExAsync         | sourse, startstr, endstr                                    | Task<string>                  | 提取两个标识符之间的内容，适用于解析特定格式文本       |
| **HTTP状态管理**      |                                                              |                               |                                                        |
| SetCookies            | ctx, key, value, minutes                                    | void                          | 设置客户端Cookie，默认30分钟过期                       |
| DeleteCookies         | ctx, key                                                    | void                          | 清除指定Cookie                                         |
| GetCookies            | ctx, key                                                    | string                        | 读取Cookie值，不存在时返回空字符串                     |
| SessionAdd            | ctx, strSessionName, strValue                               | void                          | 存储Session数据(二进制格式)                            |
| SessionGet            | ctx, cancellationToken                                      | Task                          | 异步加载Session数据                                    |
| SessionDel            | ctx, strSessionName                                         | void                          | 移除指定Session                                        |
| **数据处理**          |                                                              |                               |                                                        |
| IsString              | data, value                                                 | bool                          | 检查字符串是否包含指定内容(不区分大小写)               |
| YAlloctionlist        | yAllocations, yAllocations1                                 | List<List<YAllocationInfo>>   | 资源分配核心算法，返回[剩余资源, 已分配明细, 分配结果] |
| YAlloctionlistThred   | yAllocations, yAllocations1                                 | List<List<YAllocationInfo>>   | 多线程版分配算法，提升大数据量处理效率                 |
| DistinctAsync<T, TKey>      | list: 待去重集合<br>keySelector: 唯一性字段选择器                                           | Task<List<T>>                         | 异步根据指定字段去重，生成全新且不扰动原集合的列表，适合大数据量场景 |
| ToDictAsync<TSource, TKey>  | list: 待转换集合<br>keySelector: Key选择器<br>allowDuplicate: 是否允许重复Key（默认false）   | Task<Dictionary<TKey, TSource>>        | 异步将集合转换为字典，支持自定义Key和重复Key处理                     |
| ToDictAsync<TSource, TKeyItem> | list: 待转换集合<br>keySelector: 集合Key选择器<br>allowDuplicate: 是否允许重复Key（默认false） | Task<Dictionary<string, TSource>>      | 异步将集合Key（如List<string>）序列化为字符串作为字典Key             |
| **ID与对象**          |                                                              |                               |                                                        |
| NextId                | workerId                                                    | long                          | 生成分布式雪花ID(基于WorkerID)                         |
| Mapper                | -                                                           | FastMapper                    | 获取高性能对象映射器实例                               |
| IsDateInTargetMonth   | date, targetDate                                            | bool                          | 判断日期是否在目标年月内                               |
| Copy<T>               | data                                                        | T?                            | 深度克隆对象(使用DeepCloner库)                         |
| YJMerge<T>            | list1, list2                                                | List<T>                       | 合并两个集合并去重(JSON序列化比对)                     |
| Distinct<T>           | list                                                        | List<T>                       | 集合去重(基于哈希表实现)                               |

---

#### FastMapper（高性能对象映射器）

| 方法名                | 参数说明                | 返回值         | 功能描述                   |
|-----------------------|------------------------|----------------|----------------------------|
| Adapt<TTarget>        | this object source     | TTarget        | 自动推断源类型，返回新对象 |
| Adapt<TSource,TTarget>| this TSource source    | TTarget        | 泛型映射，类型安全         |
| Adapt<TTarget>        | this object source, TTarget dest | TTarget | 属性覆盖到已存在对象       |
| Adapt<List<TTarget>>  | this IEnumerable<object> sourceList | List<TTarget> | 集合递归映射              |

---

#### YJHelpCache

| 方法名         | 参数说明         | 返回值         | 功能描述           |
|----------------|-----------------|----------------|--------------------|
| TryGetValue    | key             | bool           | 判断是否存在       |
| Get            | key             | object         | 获取缓存           |
| Set            | key, value      | void           | 写入缓存           |
| Remove         | key             | void           | 删除缓存           |

---

#### YJHelpKafka

| 方法名                  | 参数说明         | 返回值         | 功能描述           |
|-------------------------|-----------------|----------------|--------------------|
| Produce                 | topic, value    | void           | 发布消息           |
| ProduceAdmin            | topic, value, user, pwd | void | 发布消息（带账号）  |
| ProduceAdminPartition   | topic, value, user, pwd | void | 随机分区发布（带账号） |

---

#### YJHelpRedis

| 方法名         | 参数说明         | 返回值         | 功能描述           |
|----------------|-----------------|----------------|--------------------|
| RedisCli       | -               | RedisClient    | Redis 配置中心     |
| Get            | key             | object         | 获取单个实体       |
| GetList        | key             | List<object>   | 获取列表           |
| Add            | key, value      | void           | 添加（已存在时不更新） |
| ListSetW/T     | key, value      | void           | 集合写入           |
| Clear/Remove   | key             | void           | 清空/删除          |
| GetExpire      | key             | TimeSpan       | 获取有效期         |
| ContainsKey    | key             | bool           | 判断是否存在       |
| Count/SCARD    | key             | int            | 获取集合个数       |

---

#### YJHelpT

| 方法名                | 参数说明         | 返回值         | 功能描述           |
|-----------------------|-----------------|----------------|--------------------|
| SpliteSourceBySize    | list, size      | List<List<T>>  | 按大小分组         |
| SpliteSourceByCount   | list, count     | List<List<T>>  | 按组数分组         |
| DistinctList          | list            | List<T>        | 集合去重（数值类型）|
| Merge/MergeC          | list1, list2    | List<T>        | 合并集合           |
| DiffsetT/Diffset      | list1, list2    | List<T>        | 差集               |
| Intersect/Union       | list1, list2    | List<T>        | 交集/并集          |

---

## 历史版本（摘自 NuGet 包说明）
- **3.7.4**： 新增Mapster风格的替代方案
- **3.7**：新增lis集合生成字典的方法，方便快速查询：ToDictAsync, 优化其它逻辑和新增集合去重逻辑
- **3.5**：新增对象深拷贝、对象映射器、雪花ID等功能，优化减少依赖，移除 sqlsugar
- **3.4**：新增多线程集合处理、IYJHelpCache 缓存接口
- **3.0.0+**：方法基本改为异步
- **2.2.0**：自动依赖注入功能独立为 YellowJAutoInjection
- **2.0.0**：结构优化，新增 Server 层和 IServer 层

---

## 安装与使用

1. **NuGet 安装**
2. **参考示例代码和注释**
3. **详细API请查阅源码或接口文档**

---

## 参与贡献

欢迎提交 Issue、PR 或建议！

---

## 第三方依赖与致谢

本项目部分功能基于以下优秀开源组件，特此致谢：

| 依赖库        | 说明                    | 链接                                   |
|---------------|------------------------|----------------------------------------|
| Confluent.Kafka | 高性能 Kafka 客户端     | https://github.com/confluentinc/confluent-kafka-dotnet |
| NewLife.Redis   | 分布式缓存/Redis客户端   | https://github.com/NewLifeX/NewLife.Redis |
| DeepCloner      | 对象深拷贝              | https://github.com/force-net/DeepCloner |

感谢所有开源社区的贡献者！

---

## 相关链接

- [Gitee YellowJHelp](https://gitee.com/xiaoyi1314/yellow-jhelp)

---

> 如需更多详细用法、扩展或定制，请查阅源码或联系作者。
