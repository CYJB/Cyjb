using System.Diagnostics;
using System.Reflection;

namespace Cyjb.Reflection
{
	/// <summary>
	/// �ṩ <see cref="BindingFlags"/> �����չ������
	/// </summary>
	internal static class BindingFlagsUtil
	{
		/// <summary>
		/// ����������̬��Ա�İ󶨱�־��
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;
		/// <summary>
		/// ����������˽�о�̬��Ա�İ󶨱�־��
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal const BindingFlags Static = BindingFlags.NonPublic | PublicStatic;
		/// <summary>
		/// ��������ʵ����Ա�İ󶨱�־��
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;
		/// <summary>
		/// ����������˽��ʵ����Ա�İ󶨱�־��
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal const BindingFlags Instance = BindingFlags.NonPublic | PublicInstance;
		/// <summary>
		/// ��������ʵ����̬��Ա�İ󶨱�־��
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal const BindingFlags Public = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
		/// <summary>
		/// ����������˽��ʵ����̬��Ա�İ󶨱�־��
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal const BindingFlags AllMember = BindingFlags.NonPublic | Public;
		/// <summary>
		/// �Կɱ������VarArgs�����а󶨣�ע�� <see cref="Type.InvokeMember(string, BindingFlags, Binder, object, object[])"/> 
		/// ����֧�ֿɱ������
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal const BindingFlags VarArgsParamBinding = (BindingFlags)0x10000000;
	}
}
