Cyjb
====

提供基础的功能扩展，基于 .NET 6。

本项目包含一些常用的基础类，主要功能包括：

- 常用的扩展方法。
- 常用的异常辅助方法。
- 更多集合类型：
	- 常用泛型集合基类。
	- 位的压缩列表。
	- 字符集合。
- 完整的类型转换解决方案：
	- 支持隐式、显式和自定义类型转换。
	- 可以直接用于泛型方法的 [System.Converter&lt;TInput, TOutput&gt;](https://docs.microsoft.com/zh-cn/dotnet/api/system.converter-2) 实现。
	- 能够在 IL 中使用类型转换功能。
- 对反射的高级支持：
	- 更强大的类型成员查找。
	- 很简单的动态创建委托。
	- IL 的扩展方法。
- LRU 缓存池。

详细的类库文档，请参见 [Wiki](https://github.com/CYJB/Cyjb/wiki)。

欢迎访问我的[博客](http://www.cnblogs.com/cyjb/)获取更多信息。