Cyjb
====

我开发的的 C# 类库，以 BSD 协议发布。欢迎访问我的[博客](http://www.cnblogs.com/cyjb/)。

本项目包括一些核心类，包括：

* Cyjb 命名空间：包含最基本的类。
	- ArrayExt 类：提供数组的扩展方法。
	- CharExt 类：提供 Char 类的扩展方法。
	- ConvertExt 类：将一个数据类型转换为另一个数据类型的方法。
	- DelegateBuilder 类：提供动态构造方法、属性或字段委托的方法，[详细说明在这里](http://www.cnblogs.com/cyjb/archive/2013/03/21/DelegateBuilder.html)。
	- EnumerableExt 类：提供 IEnumerable&lt;T&gt; 接口的扩展方法。
	- EnumExt 类：提供 Enum 类的扩展方法。
	- IntegerExt 类：提供对整数的扩展方法。
	- MethodExt 类：提供对 MethodBase 及其子类的扩展方法。
	- PowerBinder 类：扩展的参数类型绑定器，[详细说明在这里](http://www.cnblogs.com/cyjb/archive/2013/01/22/PowerBinder.html)。
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
	- BitList 类：管理位值的压缩列表，[详细说明在这里](http://www.cnblogs.com/cyjb/archive/2013/04/14/BitList.html)。
	- BitListEqualityComparer 类：可以根据内容比较 BitList 的比较器。
	- CharSet 类：表示特定于字符的集合，[详细说明在这里](http://www.cnblogs.com/cyjb/archive/2013/04/04/CharSet.html)。
	- ListEqualityComparer&lt;T&gt; 类：可以根据内容比较 IList&gt;T&gt; 列表的比较器。
	- ListExt 类：提供对 IList&lt;T&gt; 的扩展方法。
	- ListStack&lt;T&gt; 类：允许使用索引访问元素的堆栈。
	- SetEqualityComparer&lt;T&gt; 类：可以根据内容比较 ISet&gt;T&gt; 集合的比较器。
* Cyjb.Collections.ObjectModel 命名空间：包含通用的集合基类和只读的集合类。
* Cyjb.ComponentModel 命名空间：包含与组件模型有关的类。
	- EnumDescConverter 类：支持枚举值的描述信息的转换器。
* Cyjb.Configurations 命名空间：包含与组件模型有关的类。
	- ConfigurationElementCollection&lt;TElement&gt; 类：强类型的包含一个子元素集合的配置元素。
	- ConfigurationElementCollection&lt;TKey, TElement&gt; 类：强类型的包含一个子元素集合的配置元素。
* Cyjb.IO 命名空间：包含与输入输出有关的类。
	- AggregateSourceException 类：表示源文件的异常的集合。
	- SourceException 类：表示源文件中出现异常。
	- SourceLocation 结构：表示源文件中的位置信息。
	- SourceLocator 类：提供可以用于在源文件中定位的方法。
	- SourceReader 类：支持行列计数的源文件读取器。
* Cyjb.Text 命名空间：包含文本相关的类。
	- Token 结构：表示一个词法单元。
* Cyjb.Utility 命名空间：包含一些辅助类。
	- ICache&lt;TKey, TValue&gt; 类：表示缓冲池的接口。
	- CacheFactory 类：允许根据配置文件使用不同的缓冲池配置的工厂类。
	- LruCache&lt;TKey, TValue&gt; 类：使用改进的最近最少使用算法的对象缓冲池，[详细说明在这里](http://www.cnblogs.com/cyjb/archive/2012/11/16/LruCache.html)。
	- LruCacheNoSync&lt;TKey, TValue&gt; 类：使用改进的最近最少使用算法的对象缓冲池，不包含多线程同步。
	- SimplyCache&lt;TKey, TValue&gt; 类：缓存个数不受限制的对象缓冲池。
	- SimplyCacheNoSync&lt;TKey, TValue&gt; 类：缓存个数不受限制的对象缓冲池，不包含多线程同步。









