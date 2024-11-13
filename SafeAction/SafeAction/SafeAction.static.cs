
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SafeAction
{
	public partial class SafeAction
	{
		// function to log exception
		static Action<Exception> s_logExceptionFunc;
		public static void S_addLogFunction(Action<Exception> func)
		{
			s_logExceptionFunc = func;
		}
		internal static Action<Exception> S_getLogFunction()
		{
			return s_logExceptionFunc;
		}
		// function to profile, first param is the name of the action, second param is the call count, third param is elpased time in ms
		static Action<(string name, int count, long elapse_ms)> s_profileFunc;
		public static void S_addProfileFunc(Action<(string name, int count, long elapse_ms)> func)
		{
			s_profileFunc = func;
		}
		internal static Action<(string name, int count, long elapse_ms)> S_getProfileFunc()
		{
			return s_profileFunc;
		}
		//
		static Dictionary<System.Type, Func<System.Object, bool>> s_allCachedCheckFunc = new Dictionary<System.Type, Func<System.Object, bool>>();
		static List<(System.Type type, Func<System.Object, bool> func)> s_allCheckFunc = new List<(Type type, Func<System.Object, bool> func)>();

		public static void S_clearCheckValideFunc()
		{
			s_allCheckFunc.Clear();
			s_allCachedCheckFunc.Clear();
		}
		public static void S_addCheckValideFunc(System.Type t, Func<System.Object, bool> func)
		{
			if (s_allCheckFunc.Exists(a => a.type == t))
				return;
			s_allCheckFunc.Add((type: t, func: func));
			s_allCheckFunc.Sort((a, b) =>
			{
				// a inherit from b, a is before b
				if (a.type.IsSubclassOf(b.type))
					return -1;
				if (b.type.IsSubclassOf(a.type))
					return 1;
				return a.type.GUID.CompareTo(b.type.GUID);
			});
			s_allCachedCheckFunc.Clear();
		}

		internal static Func<System.Object, bool> GetCheckValideFunc(System.Type t)
		{
			// from cache
			if (s_allCachedCheckFunc.ContainsKey(t))
				return s_allCachedCheckFunc[t];
			var func = GetCheckValideFunc2(t);
			s_allCachedCheckFunc[t] = func;
			return func;
		}

		static Func<System.Object, bool> GetCheckValideFunc2(System.Type t)
		{
			foreach (var checkFunc in s_allCheckFunc)
			{
				if (t.IsSubclassOf(checkFunc.type) || t == checkFunc.type)
					return checkFunc.func;
			}
			return SimpleCheckValide;
		}

		static bool SimpleCheckValide(System.Object obj)
		{
			return obj != null;
		}
	}
}