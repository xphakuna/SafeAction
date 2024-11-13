
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


	public partial class SafeAction
	{
		SafeActionImpl actionBefore = new SafeActionImpl();
		SafeActionImpl action = new SafeActionImpl();
		SafeActionImpl actionAfter = new SafeActionImpl();

		string debugName;
		public SafeAction(string debugName = null)
		{
			this.debugName = debugName;
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
			var n = GetActionCount();
			if (n == 0)
				return;
			//
			var profileFunc = SafeAction.S_getProfileFunc();
			System.Diagnostics.Stopwatch sw = null;
			if (profileFunc != null && string.IsNullOrEmpty(debugName) == false)
			{
				sw = new System.Diagnostics.Stopwatch();
				sw.Start();
			}
			//
			actionBefore.Invoke(debugStr);
			action.Invoke(debugStr);
			actionAfter.Invoke(debugStr);
			//
			if (sw != null)
			{
				sw.Stop();
				profileFunc.InvokeSafely((name: debugName, count: n, elapse_ms: sw.ElapsedMilliseconds));
			}
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