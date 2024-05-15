using Cyjb.Benchmark;
using BenchmarkDotNet.Running;
using System.Text;
using System.Globalization;
using BenchmarkDotNet.Attributes;
using Cyjb.Collections;
using Cyjb.Text;
using Cyjb;
using Cyjb.Test;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

Console.WriteLine('\u2648'.Width());

string text = File.ReadAllText("spec.txt");
int iteration = 1000;

string result;
for (int i = 0; i < 20; i++)
{
	LineLocator locator = new();
	locator.Read(text);
}

Console.WriteLine("fwefwa");
Console.WriteLine(unchecked(int.MinValue * 2));
Console.WriteLine(unchecked(int.MaxValue * 2));

Performance.Measure("Cyjb.Markdown", iteration, () =>
{
	LineLocator locator = new();
	locator.Read(text);
}).Print();

//BenchmarkRunner.Run<CharWidthBenchmark>();


/// <summary>
/// 进度条类型
/// </summary>
public enum ProgressBarType
{
	/// <summary>
	/// 字符
	/// </summary>
	Character,
	/// <summary>
	/// 彩色
	/// </summary>
	Multicolor
}

public class ProgressBar
{

	/// <summary>
	/// 光标的列位置。将从 0 开始从左到右对列进行编号。
	/// </summary>
	public int Left { get; set; }
	/// <summary>
	/// 光标的行位置。从上到下，从 0 开始为行编号。
	/// </summary>
	public int Top { get; set; }

	/// <summary>
	/// 进度条宽度。
	/// </summary>
	public int Width { get; set; }
	/// <summary>
	/// 进度条当前值。
	/// </summary>
	public int Value { get; set; }
	/// <summary>
	/// 进度条类型
	/// </summary>
	public ProgressBarType ProgressBarType { get; set; }


	private ConsoleColor colorBack;
	private ConsoleColor colorFore;


	public ProgressBar() : this(Console.CursorLeft, Console.CursorTop)
	{

	}

	public ProgressBar(int left, int top, int width = 50, ProgressBarType ProgressBarType = ProgressBarType.Multicolor)
	{
		this.Left = left;
		this.Top = top;
		this.Width = width;
		this.ProgressBarType = ProgressBarType;

		// 清空显示区域；
		Console.SetCursorPosition(Left, Top);
		for (int i = left; ++i < Console.WindowWidth;) { Console.Write(" "); }

		if (this.ProgressBarType == ProgressBarType.Multicolor)
		{
			// 绘制进度条背景；
			colorBack = Console.BackgroundColor;
			Console.SetCursorPosition(Left, Top);
			Console.BackgroundColor = ConsoleColor.DarkCyan;
			for (int i = 0; ++i <= width;) { Console.Write(" "); }
			Console.BackgroundColor = colorBack;
		}
		else
		{
			// 绘制进度条背景；
			Console.SetCursorPosition(left, top);
			Console.Write("[");
			Console.SetCursorPosition(left + width - 1, top);
			Console.Write("]");
		}
	}

	public int Dispaly(int value)
	{
		return Dispaly(value, null);
	}

	public int Dispaly(int value, string msg)
	{
		if (this.Value != value)
		{
			this.Value = value;

			if (this.ProgressBarType == ProgressBarType.Multicolor)
			{
				// 保存背景色与前景色；
				colorBack = Console.BackgroundColor;
				colorFore = Console.ForegroundColor;
				// 绘制进度条进度
				Console.BackgroundColor = ConsoleColor.Yellow;
				Console.SetCursorPosition(this.Left, this.Top);
				Console.Write(new string(' ', (int)Math.Round(this.Value / (100.0 / this.Width))));
				Console.BackgroundColor = colorBack;

				// 更新进度百分比,原理同上.
				Console.ForegroundColor = ConsoleColor.Green;
				Console.SetCursorPosition(this.Left + this.Width + 1, this.Top);
				if (string.IsNullOrWhiteSpace(msg)) { Console.Write("{0}%", this.Value); } else { Console.Write(msg); }
				Console.ForegroundColor = colorFore;
			}
			else
			{
				// 绘制进度条进度
				Console.SetCursorPosition(this.Left + 1, this.Top);
				Console.Write(new string('*', (int)Math.Round(this.Value / (100.0 / (this.Width - 2)))));
				// 显示百分比
				Console.SetCursorPosition(this.Left + this.Width + 1, this.Top);
				if (string.IsNullOrWhiteSpace(msg)) { Console.Write("{0}%", this.Value); } else { Console.Write(msg); }
			}
		}
		return value;
	}
}

