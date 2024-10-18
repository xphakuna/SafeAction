﻿
using System.Collections;
using System.Collections.Generic;
using System;


namespace SafeAction
{
	internal class SafeActionImpl<T1>
	{
		class OneObjAct
		{
			public System.Object m_obj;
			public List<Action<T1>> m_allAction = new List<Action<T1>>();
		}
		List<OneObjAct> m_actionWithObj = new List<OneObjAct>();
		List<Action<T1>> m_allActionNoObj = new List<Action<T1>>();

		public void Clear()
		{
			m_actionWithObj.Clear();
			m_allActionNoObj.Clear();
		}

		public void AddAction(System.Object go, Action<T1> act)
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
						m_allAction = new List<Action<T1>> { act },
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

		public void RemoveAction(System.Object go, Action<T1> act)
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

		public void Invoke(T1 t1, string debugStr = "")
		{
			foreach (var action in m_allActionNoObj)
			{
				action?.InvokeSafely(t1);
			}
			for (int i = m_actionWithObj.Count - 1; i >= 0; i--)
			{
				var checkValideFunc = SafeAction.GetCheckValideFunc(m_actionWithObj[i].m_obj.GetType());
				if (checkValideFunc(m_actionWithObj[i].m_obj) == false)
				{
					m_actionWithObj.RemoveAt(i);
				}
				else
				{
					foreach (var action in m_actionWithObj[i].m_allAction)
					{
						action?.InvokeSafely(t1);
					}
				}
			}
		}

		internal int GetActionCount()
		{
			return m_actionWithObj.Sum(x => x.m_allAction.Count) + m_allActionNoObj.Count;
		}
	}

	public class SafeAction<T1>
	{
		SafeActionImpl<T1> actionBefore = new SafeActionImpl<T1>();
		SafeActionImpl<T1> action = new SafeActionImpl<T1>();
		SafeActionImpl<T1> actionAfter = new SafeActionImpl<T1>();

		public void AddAction(System.Object go, Action<T1> act, int seq = 0)
		{
			if (seq > 0)
				actionAfter.AddAction(go, act);
			else if (seq == 0)
				action.AddAction(go, act);
			else
				actionBefore.AddAction(go, act);

		}

		public void RemoveAction(System.Object go, Action<T1> act)
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

		public void Invoke(T1 t1, string debugStr = "")
		{
			actionBefore.Invoke(t1, debugStr);
			action.Invoke(t1, debugStr);
			actionAfter.Invoke(t1, debugStr);
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