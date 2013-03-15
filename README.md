Cyjb
====

My personal C# library.

我的个人 C# 类库。欢迎访问我的[博客](http://www.cnblogs.com/cyjb/)。

本项目包括一些核心类，包括：

* Cyjb 命名空间：包含最基本的类。
	- ArrayExt 类：提供数组的扩展方法。
	- CharExt 类：提供 Char 类的扩展方法。
	- ConvertExt 类：将一个数据类型转换为另一个数据类型的方法。
	- EnumerableExt 类：提供 IEnumerable&lt;T&gt; 接口的扩展方法。
	- EnumExt 类：提供 Enum 类的扩展方法。
	- IntegerExt 类：提供对整数的扩展方法。
	- PowerBinder 类：扩展的参数类型绑定器。
	- RandomExt 类：表示一个全局的伪随机数生成器。
	- StringExt 类：提供 String 类的扩展方法。
	- TextValuePair 类：表示简单的文本-值对。
	- TextValuePair&lt;TValue&gt; 类：表示泛型的文本-值对。
	- TextValuePairCollection 类：表示文本-值对的集合。
	- TextValuePairCollection&lt;TValue&gt; 类：表示文本-值对的集合。
	- Tristate 类：表示一个三态枚举。
	- TypeExt 类：提供对 Type 类的扩展方法。
	- UniqueValue&lt;TValue&gt; 类：用于需要获取唯一值的情况。
* Cyjb.Collections 命名空间：包含与集合有关的类。
	- ArrayAdapter&lt;T&gt; 类：表示数组的一部分的列表。
* Cyjb.Collections.ObjectModel 命名空间：包含通用的集合基类和只读的集合类。
* Cyjb.ComponentModel 命名空间：包含与组件模型有关的类。
	- EnumDescConverter 类：支持枚举值的描述信息的转换器。
* Cyjb.Configurations 命名空间：包含与组件模型有关的类。
	- ConfigurationElementCollection&lt;TElement&gt; 类：强类型的包含一个子元素集合的配置元素。
	- ConfigurationElementCollection&lt;TKey, TElement&gt; 类：强类型的包含一个子元素集合的配置元素。
* Cyjb.Utility 命名空间：包含一些辅助类。
	- ICache&lt;TKey, TValue&gt; 类：表示缓冲池的接口。
	- CacheFactory 类：允许根据配置文件使用不同的缓冲池配置的工厂类。
	- LruCache&lt;TKey, TValue&gt; 类：使用改进的最近最少使用算法的对象缓冲池。
	- LruCacheNoSync&lt;TKey, TValue&gt; 类：使用改进的最近最少使用算法的对象缓冲池，不包含多线程同步。
	- SimplyCache&lt;TKey, TValue&gt; 类：缓存个数不受限制的对象缓冲池。
	- SimplyCacheNoSync&lt;TKey, TValue&gt; 类：缓存个数不受限制的对象缓冲池，不包含多线程同步。









