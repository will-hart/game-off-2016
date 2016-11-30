using System.Diagnostics;
using System.Reflection;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

/* Wrapper class for Debug.Log to allow excluding it easily from final build */

public static class D
{
	private static StringBuilder sb = new StringBuilder();
	private static StackFrame sf;
	private static MethodBase sfMethod;
#if UNITY_EDITOR
	public static void Log(object o)
	{
		//sf = new StackFrame(1);
		//sfMethod = sf.GetMethod();
		//Debug.Log(sfMethod.DeclaringType.Name + "::" + sfMethod.Name + "()--" +o);
		Debug.Log(o);
	}

	public static void Log(object o, params string[] values)
	{
		sb.Append(o);
		for (int i = 0; i < values.Length; i++)
		{
			sb.Append(" ").Append(values[i]);
		}
		Debug.Log(sb.ToString());
	}

	public static void Log(object o, Object context)
	{
		sf = new StackFrame(1);
		sfMethod = sf.GetMethod();
		Debug.Log(sfMethod.DeclaringType.Name + "::" + sfMethod.Name + "()--" + o, context);
	}

	public static void LogWarning(object o)
	{
		sf = new StackFrame(1);
		sfMethod = sf.GetMethod();
		Debug.LogWarning(sfMethod.DeclaringType.Name + "::" + sfMethod.Name + "()--" + o);
	}

	public static void LogWarning(object o, Object context)
	{
		sf = new StackFrame(1);
		sfMethod = sf.GetMethod();
		Debug.LogWarning(sfMethod.DeclaringType.Name + "::" + sfMethod.Name + "()--" + o, context);
	}

	public static void LogError(object o)
	{
		sf = new StackFrame(1);
		sfMethod = sf.GetMethod();
		Debug.LogError(sfMethod.DeclaringType.Name + "::" + sfMethod.Name + "()--" + o);
	}

	public static void LogError(object o, Object context)
	{
		sf = new StackFrame(1);
		sfMethod = sf.GetMethod();
		Debug.LogError(sfMethod.DeclaringType.Name + "::" + sfMethod.Name + "()--" + o, context);
	}

	public static void LogAssertion(object o)
	{
		sf = new StackFrame(1);
		sfMethod = sf.GetMethod();
		Debug.LogAssertion(sfMethod.DeclaringType.Name + "::" + sfMethod.Name + "()--" + o);
	}

	public static void LogAssertion(object o, Object context)
	{
		sf = new StackFrame(1);
		sfMethod = sf.GetMethod();
		Debug.LogAssertion(sfMethod.DeclaringType.Name + "::" + sfMethod.Name + "()--" + o, context);
	}

	public static void Break()
	{
		Debug.Break();
	}

	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.0F, bool depthTest = true)
	{
		Debug.DrawLine(start, end, color, duration, depthTest);
	}

	public static void DrawLine(Vector3 start, Vector3 end, float duration = 0.0F, bool depthTest = true)
	{
		Debug.DrawLine(start, end, Color.yellow, duration, depthTest);
	}

#else
	public static void Log(object o) { }
	public static void Log(object o, Object context) {}
    public static void LogError(object o) { }
	public static void LogError(object o, Object context) { }
    public static void LogWarning(object o) { }
	public static void LogWarning(object o, Object context) { }
	
	public static void LogAssertion(object o) { }

	public static void LogAssertion(object o, Object context) { }

	public static void Break() { }

	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.0F, bool depthTest = true) { }
	public static void DrawLine(Vector3 start, Vector3 end, float duration = 0.0F, bool depthTest = true) {}
	
#endif
}
