Cyjb
====

我开发的 C# 基础类库，以 BSD 协议发布。基于 .Net Framework 4.5，使用 Visual Studio 2013 作为开发工具。

这里使用了 [Code Contracts for .NET](https://visualstudiogallery.msdn.microsoft.com/1ec7db13-3363-46c9-851f-1ce455f66970) 作为 debug 模式的代码约束工具，因此请安装此工具，或者使用 release 模式编译，或者在“项目属性”的“编译”选项卡中，将条件编译符号 `CONTRACTS_FULL` 移除。

本项目包含一些常用的基础类，主要功能包括：

- 常用功能：
  - 对数字、字符串和枚举等的扩展方法。
  - 常用的基本类型，如文本-值对、三态枚举等。
  - 常用的异常辅助方法。
  - 更易于使用的泛型集合基类。
  - 对集合类的扩展。
  - 对输入输出类的扩展。
- 完整的类型转换解决方案：
  - 支持隐式、显式和自定义类型转换。
  - 支持 [System.Converter<TInput, TOutput>](https://msdn.microsoft.com/zh-cn/library/kt456a2y.aspx) 接口，能够直接用在泛型类型中。
  - 能够动态添加额外的类型转换。
  - 能够在 IL 中使用类型转换功能。
- 对反射的高级支持：
  - 更强大的类型成员查找。
  - 很容易的动态创建委托。
  - IL 的扩展方法。
- 缓冲池。

详细的类库文档，请参见 [Wiki](https://github.com/CYJB/Cyjb/wiki)。

欢迎访问我的[博客](http://www.cnblogs.com/cyjb/)获取更多信息。