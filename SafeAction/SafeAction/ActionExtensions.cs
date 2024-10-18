using System;

namespace SafeAction
{
	public static class ActionExtensions
	{
		public static bool InvokeSafely(this Action action)
		{
			try
			{
				action?.Invoke();
				return true;
			}
			catch (Exception e)
			{
				SafeAction.S_getLogFunction()?.Invoke(e);
				return false;
			}
		}


		public static bool InvokeSafely<T>(this Action<T> action, T arg)
		{
			try
			{
				action?.Invoke(arg);
				return true;
			}
			catch (Exception e)
			{
				SafeAction.S_getLogFunction()?.Invoke(e);
				return false;
			}
		}


		public static bool InvokeSafely<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
		{
			try
			{
				action?.Invoke(arg1, arg2);
				return true;
			}
			catch (Exception e)
			{
				SafeAction.S_getLogFunction()?.Invoke(e);
				return false;
			}
		}


		public static bool InvokeSafely<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
		{
			try
			{
				action?.Invoke(arg1, arg2, arg3);
				return true;
			}
			catch (Exception e)
			{
				SafeAction.S_getLogFunction()?.Invoke(e);
				return false;
			}
		}
		public static bool InvokeSafely<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			try
			{
				action?.Invoke(arg1, arg2, arg3, arg4);
				return true;
			}
			catch (Exception e)
			{
				SafeAction.S_getLogFunction()?.Invoke(e);
				return false;
			}
		}


		public static T1 InvokeSafely<T1>(this Func<T1> action, T1 defaultValue = default(T1))
		{
			try
			{
				return action.Invoke();
			}
			catch (Exception e)
			{
				SafeAction.S_getLogFunction()?.Invoke(e);
				return defaultValue;
			}
		}


		public static T2 InvokeSafely<T1, T2>(this Func<T1, T2> action, T1 arg, T2 defaultValue = default(T2))
		{
			try
			{
				return action.Invoke(arg);
			}
			catch (Exception e)
			{
				SafeAction.S_getLogFunction()?.Invoke(e);
				return defaultValue;
			}
		}
	}
}

