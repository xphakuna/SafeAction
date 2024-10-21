
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SafeAction
{
	internal class SafeActionImpl
	{
		class OneObjAct
		{
			public System.Object m_obj;
			public List<Action> m_allAction = new List<Action>();
		}
		List<OneObjAct> m_actionWithObj = new List<OneObjAct>();
		List<Action> m_allActionNoObj = new List<Action>();

		public void Clear()
		{
			m_actionWithObj.Clear();
			m_allActionNoObj.Clear();
		}

		public void AddAction(System.Object go, Action act)
		{
			RemoveAction(go, act);
			//
			if (go == null)
			{
				m_allActionNoObj.Add(act);
			}
			else
			{
				var idx = m_actionWithObj.FindIndex(a => a.m_obj == go);
				if (idx >= 0)
				{
					m_actionWithObj[idx].m_allAction.Add(act);
				}
				else
				{
					m_actionWithObj.Add(new OneObjAct
					{
						m_obj = go,
						m_allAction = new List<Action> { act },
					});
				}
			}

		}

		public void RemoveAction(System.Object go)
		{
			var idx = m_actionWithObj.FindIndex(a => a.m_obj == go);
			if (idx >= 0)
			{
				m_actionWithObj.RemoveAt(idx);
			}
		}


		public void RemoveAction(System.Object go, Action act)
		{
			if (go == null)
			{
				m_allActionNoObj.Remove(act);
			}
			else
			{
				var idx = m_actionWithObj.FindIndex(a => a.m_obj == go);
				if (idx >= 0)
				{
					m_actionWithObj[idx].m_allAction.Remove(act);
				}
			}
		}

		public void Invoke(string debugStr = "")
		{
			foreach (var action in m_allActionNoObj)
			{
				action?.InvokeSafely();
			}
			for (int i = m_actionWithObj.Count - 1; i >= 0; i--)
			{
				var checkFunc = SafeAction.GetCheckValideFunc(m_actionWithObj[i].m_obj.GetType());
				if (checkFunc(m_actionWithObj[i].m_obj) == false)
				{
					m_actionWithObj.RemoveAt(i);
				}
				else
				{
					foreach (var action in m_actionWithObj[i].m_allAction)
					{
						action?.InvokeSafely();
					}
				}
			}
		}

		internal int GetActionCount()
		{
			return m_actionWithObj.Sum(x=>x.m_allAction.Count) + m_allActionNoObj.Count;
		}
	}


	public class SafeAction
	{
		SafeActionImpl actionBefore = new SafeActionImpl();
		SafeActionImpl action = new SafeActionImpl();
		SafeActionImpl actionAfter = new SafeActionImpl();

		static Action<Exception> s_logExceptionFunc;
		public static void S_addLogFunction(Action<Exception> e)
		{
			s_logExceptionFunc = e;
		}
		public static Action<Exception> S_getLogFunction()
		{
			return s_logExceptionFunc;
		}

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

		public static Func<System.Object, bool> GetCheckValideFunc(System.Type t)
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
			return CheckValide;
		}

		static bool CheckValide(System.Object obj)
		{
			return obj != null;
		}

		public void AddAction(System.Object go, Action act, int seq = 0)
		{
			if (seq > 0)
				actionAfter.AddAction(go, act);
			else if (seq == 0)
				action.AddAction(go, act);
			else
				actionBefore.AddAction(go, act);

		}

		public void RemoveAction(System.Object go, Action act)
		{
			actionBefore.RemoveAction(go, act);
			action.RemoveAction(go, act);
			actionAfter.RemoveAction(go, act);
		}

		public void RemoveAction(System.Object go)
		{
			actionBefore.RemoveAction(go);
			action.RemoveAction(go);
			actionAfter.RemoveAction(go);
		}

		public void Invoke(string debugStr = "")
		{
			actionBefore.Invoke(debugStr);
			action.Invoke(debugStr);
			actionAfter.Invoke(debugStr);
		}

		public int GetActionCount()
		{
			return actionBefore.GetActionCount() + action.GetActionCount() + actionAfter.GetActionCount();
		}

		public void Clear()
		{
			actionBefore.Clear();
			action.Clear();
			actionAfter.Clear();
		}
	}
}