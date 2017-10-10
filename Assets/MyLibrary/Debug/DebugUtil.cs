using UnityEngine;
using System.Collections;
using System.Diagnostics;


/// <summary>
/// デバッグクラス
/// </summary>
public static class DebugUtil
{
	private static ILogger mLogger = DefaultLogger;

	/// <summary>
	/// ログ出力に使用するロガー
	/// </summary>
	public static ILogger Logger	{ 
		set {
			mLogger = value;
		}
		get {
			return mLogger;
		}
	}
	/// <summary>
	/// 標準ロガー
	/// </summary>
	public static ILogger DefaultLogger {
		get {
			return UnityEngine.Debug.unityLogger;
		}
	}

	public static bool developerConsoleVisible																								{ get { return UnityEngine.Debug.developerConsoleVisible; } set { UnityEngine.Debug.developerConsoleVisible = value; } }
	public static bool isDebugBuild																											{ get { return UnityEngine.Debug.isDebugBuild; } }

	[Conditional("ENABLE_ASSERT")]	public static void Assert(bool condition)																			{ UnityEngine.Debug.Assert(condition); }
	[Conditional("ENABLE_ASSERT")]	public static void Assert(bool condition, UnityEngine.Object context)												{ UnityEngine.Debug.Assert(condition, context); }
	[Conditional("ENABLE_ASSERT")]	public static void Assert(bool condition, object message)															{ UnityEngine.Debug.Assert(condition, message); }	
	[Conditional("ENABLE_ASSERT")]	public static void Assert(bool condition, string message)															{ UnityEngine.Debug.Assert(condition, message); }
	[Conditional("ENABLE_ASSERT")]	public static void Assert(bool condition, object message, UnityEngine.Object context)								{ UnityEngine.Debug.Assert(condition, message, context); }
	[Conditional("ENABLE_ASSERT")]	public static void Assert(bool condition, string message, UnityEngine.Object context)								{ UnityEngine.Debug.Assert(condition, message, context); }
	[Conditional("ENABLE_ASSERT")]	public static void AssertFormat(bool condition, string format, params object[] args)								{ UnityEngine.Debug.AssertFormat(condition, format, args); }
	[Conditional("ENABLE_ASSERT")]	public static void AssertFormat(bool condition,UnityEngine.Object context, string format, params object[] args)		{ UnityEngine.Debug.AssertFormat(condition, context, format, args); }

	[Conditional("ENABLE_LOG")]		public static void Log(object message)																	{ mLogger.Log(LogType.Log, message); }
	[Conditional("ENABLE_LOG")]		public static void LogFormat(string format, params object[] args)										{ mLogger.LogFormat(LogType.Log, format, args); }
	[Conditional("ENABLE_LOG")]		public static void LogError(object message)																{ mLogger.Log(LogType.Error, message); }
	[Conditional("ENABLE_LOG")]		public static void LogErrorFormat(string format, params object[] args)									{ mLogger.LogFormat(LogType.Error, format, args); }
	[Conditional("ENABLE_LOG")]		public static void LogException(System.Exception exception)												{ mLogger.LogException(exception); }
	[Conditional("ENABLE_LOG")]		public static void LogWarning(object message)															{ mLogger.Log(LogType.Warning, message); }
	[Conditional("ENABLE_LOG")]		public static void LogWarningFormat(string format, params object[] args)								{ mLogger.LogFormat(LogType.Warning, format, args); }

	//-------------------------------------------------------------------------
	//	独自追加関数
	//-------------------------------------------------------------------------
	[Conditional("ENABLE_ASSERT")]	
	public static void NullAssert(System.Object obj)
	{
		Assert(obj != null);
	}

	[Conditional("ENABLE_ASSERT")]	
	public static void NullAssert(System.Object obj, string message)
	{
		Assert(obj != null, message);
	}
	[Conditional("ENABLE_ASSERT")]	
	public static void NullAssertFormat(System.Object obj, string format, params object[] args)
	{
		AssertFormat(obj != null, format, args);
	}

	/// <summary>
	/// 呼び出し元メソッド情報文字列を取得
	/// </summary>
	public static string GetCalledMethodInfo()
	{
		var stack = new System.Diagnostics.StackTrace(2, true);
		var frame = stack.GetFrame(0);

		return string.Format("{0}:{1} {2}", frame.GetFileName(), frame.GetFileLineNumber(), frame.GetMethod().Name);
	}

	/// <summary>
	/// スタックトレース全部文字列化
	/// </summary>
	public static string GetStackTraceFullText()
	{
		System.Text.StringBuilder builder = new System.Text.StringBuilder();

		var stack = new System.Diagnostics.StackTrace(1, true);
		for (int i = 0 ; i < stack.FrameCount ; i++) {
			var frame = stack.GetFrame(i);

			builder.AppendFormat("[{0:D2}] | {1}:{2} {3}\n", i, frame.GetFileName(), frame.GetFileLineNumber(), frame.GetMethod().Name);
		}

		return builder.ToString();
	}
}
