# YellowJAutoInjection 

依赖注入功能独立
Nuget 获取YellowJAutoInjection 

#### 程序集自动注入说明：满足IOC开发

案列：Net6

`builder.Services.YJDiInJect();`

![输入图片说明](1.png)


类库中满足如：

`[AutoInject(typeof(ILoginServer))]`

声明的都将自动注入程序集

![输入图片说明](2.png)


# YellowJHelp

#### 介绍

[Gitee]: v2.2.0	"https://gitee.com/xiaoyi1314/yellow-jhelp"

通用解决方案：
		3.5：新增对象深拷贝，对象映射器，雪花id等功能，优化减少依赖，删除sqlsugar等功能
        3.4：针对大数据集合计算多线程处理，新增IYJHelpCache缓存接口
		3.0.0版本后方法基本改成异步。
		2.2.0版本：自动依赖注入功能转移至：YellowJAutoInjection
		（Redis更换NewLife-日均百亿次项目验证）缓存，MD5加密，字段截取，集合类型差集计算
		Kafka使用
		webapi调用
		Redis使用

#### 软件架构
netstandard2.1
Framework 4.6.1+

| 集成第三方      |
| --------------- |
| Confluent.Kafka |
| Mapster         |
| NewLife.Redis   |



#### 版本说明
2.0.0：优化结构，新增Server层和Iserver层，实现接口层封装

#### 方法库函数介绍
##### YJHelp
#### 核心功能模块说明

| 方法名                | 参数说明                                                     | 返回值                        | 功能详细描述                                           |
| --------------------- | ------------------------------------------------------------ | ----------------------------- | ------------------------------------------------------ |
| **加密模块**          |                                                              |                               |                                                        |
| `MD5EncryptAsync`     | `strText`: 要加密的原始字符串<br>`IsLower`: 是否返回小写格式 | `Task<string>`                | 生成32位MD5哈希值，适用于密码存储/数据校验             |
| `EncodeAsync`         | `data`: 明文数据<br>`KEY_64`: 8位密钥<br>`IV_64`: 8位初始化向量 | `Task<string>`                | DES对称加密，用于敏感数据传输                          |
| `DecodeAsync`         | `data`: 密文数据<br>`KEY_64`: 8位密钥<br>`IV_64`: 8位初始化向量 | `Task<string>`                | DES解密，需与加密使用相同密钥                          |
| **日志与文件**        |                                                              |                               |                                                        |
| `YellowJLogAsync`     | `text`: 日志内容<br>`address`: 日志文件存储路径              | `Task`                        | 按日期自动创建日志文件，UTF8编码存储                   |
| `MidStrExAsync`       | `sourse`: 源字符串<br>`startstr`: 起始标识<br>`endstr`: 结束标识 | `Task<string>`                | 提取两个标识符之间的内容，适用于解析特定格式文本       |
| **HTTP状态管理**      |                                                              |                               |                                                        |
| `SetCookies`          | `ctx`: HTTP上下文<br>`key`: 键<br>`value`: 值<br>`minutes`: 过期时间(分钟) | `void`                        | 设置客户端Cookie，默认30分钟过期                       |
| `DeleteCookies`       | `ctx`: HTTP上下文<br>`key`: 要删除的Cookie键                 | `void`                        | 清除指定Cookie                                         |
| `GetCookies`          | `ctx`: HTTP上下文<br>`key`: 要获取的Cookie键                 | `string`                      | 读取Cookie值，不存在时返回空字符串                     |
| `SessionAdd`          | `ctx`: HTTP上下文<br>`strSessionName`: 键<br>`strValue`: 二进制值 | `void`                        | 存储Session数据(二进制格式)                            |
| `SessionGet`          | `ctx`: HTTP上下文<br>`cancellationToken`: 取消令牌           | `Task`                        | 异步加载Session数据                                    |
| `SessionDel`          | `ctx`: HTTP上下文<br>`strSessionName`: 要删除的Session键     | `void`                        | 移除指定Session                                        |
| **数据处理**          |                                                              |                               |                                                        |
| `IsString`            | `data`: 源字符串<br>`value`: 查找内容                        | `bool`                        | 检查字符串是否包含指定内容(不区分大小写)               |
| `YAlloctionlist`      | `yAllocations`: 可分配资源列表<br>`yAllocations1`: 需求列表  | `List<List<YAllocationInfo>>` | 资源分配核心算法，返回[剩余资源, 已分配明细, 分配结果] |
| `YAlloctionlistThred` | 参数同上                                                     | `List<List<YAllocationInfo>>` | 多线程版分配算法，提升大数据量处理效率                 |
| **ID与对象**          |                                                              |                               |                                                        |
| `NextId`              | `workerId`: 工作节点ID                                       | `long`                        | 生成分布式雪花ID(基于WorkerID)                         |
| `Mapper`              | -                                                            | `FastMapper`                  | 获取高性能对象映射器实例                               |
| `IsDateInTargetMonth` | `date`: 检查日期<br>`targetDate`: 目标日期                   | `bool`                        | 判断日期是否在目标年月内                               |
| `Copy<T>`             | `data`: 要克隆的对象                                         | `T?`                          | 深度克隆对象(使用DeepCloner库)                         |
| `YJMerge<T>`          | `list1`: 集合1<br>`list2`: 集合2                             | `List<T>`                     | 合并两个集合并去重(JSON序列化比对)                     |
| `Distinct<T>`         | `list`: 原始集合                                             | `List<T>`                     | 集合去重(基于哈希表实现)                               |

---

### 🚨 已过时方法 (建议改用YJHelpCache)

| 方法名   | 参数说明                  | 功能替代方案                   |
| -------- | ------------------------- | ------------------------------ |
| `Add`    | `key`: 键, `value`: 值    | 使用`IMemoryCache.Set`         |
| `Get<T>` | `key`: 缓存键             | 使用`IMemoryCache.Get<T>`      |
| `Exsits` | `key`: 检查键             | 使用`IMemoryCache.TryGetValue` |
| `Clear`  | `key`: 指定键 或 清空全部 | 使用`IMemoryCache.Remove`      |

---

### 
##### YJHelpCache
|  YJHelpCache |  IYJHelpCache  |
|---|---|
| TryGetValue       |  判断是否存在 |
| Get               |  获取 |
| Set               |  写入 |
| Remove            |  删除 |

##### YJHelpKafka
|  YJHelpKafka |  IYJHelpKafka  |
|---|---|
| Produce                  |  发布者 |
| Produce                  |  发布者-随机分区 |
| ProduceAdmin             |  发布者（带账号密码） |
| ProduceAdminPartition    |  发布者-随机分区（带账号密码） |

##### YJHelpRedis
|  YJHelpRedis | IYJHelpRedis   |
|---|---|
| RedisCli           |  Redis 配置中心 |
| Get                |  获取单个实体 |
| GetList            |  获取列表 |
| Add                |  添加（已存在时不更新） |
| ListSetW           |  写入集合：尾部增加 |
| ListSetT           |  写入集合：头部增加 |
| Clear              |  清空所有缓存项 |
| Remove             |  按Key值清空缓存项 |
| GetExpire          |  获取缓存项有效期 |
| ContainsKey        |  判断是否存在 |
| Count              |  获取缓存个数 |
| SCARD              |  返回集合个数 |

##### YJHelpT
| YJHelpT  | IYJHelpT    |
|---|---|
| SpliteSourceBySize    |  将集合按大小分组 |
| SpliteSourceByCount   |  将集合按组数分组 |
| DistinctList          |  集合去重（哈希，只针对数值类型） |
| Merge                 |  合并两个集合（不允许有重复项） |
| MergeC                |  合并两个集合（允许出现重复项） |
| DiffsetT              |  获取差集（集合） |
| Diffset               |  获取差集（集合） |
| Intersect             |  获取交集（集合） |
| Union                 |  获取并集（集合） |
| ...                   |  更多功能请查看注释介绍 |

#### 安装教程
![输入图片说明](https://images.gitee.com/uploads/images/2021/1122/134200_7e13f9f5_1731777.png "_Z5NP71B0T52]1@8PY4(}7J.png")


#### 使用说明
方法库

YJHelp
YJHelpT
YJHelpCache
YJHelpKafka
YJHelpRedis
YJHelpWebApi

集成库
#### 参与贡献
