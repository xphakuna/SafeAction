using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestSafeAction
{
	[TestClass]
	public class TestSafeAction2
	{
		[TestMethod]
		public async Task TestAddRemove_noObj()
		{
			var sa = new SafeAction.SafeAction<int, int>();
			int n1 = 0;
			int n2 = 0;

			var act1 = (int n, int m) => { n1++; };
			var act2 = (int n, int m) => { n2++; };

			sa.AddAction(null, act1);
			sa.AddAction(null, act2);
			sa.Invoke(0, 0);

			Assert.AreEqual(1, n1);
			Assert.AreEqual(1, n2);

			sa.RemoveAction(null, act1);
			sa.Invoke(0, 0);
			Assert.AreEqual(1, n1);
			Assert.AreEqual(2, n2);
		}

		[TestMethod]
		public async Task TestAddRemove_order_noObj()
		{
			var sa = new SafeAction.SafeAction<int, int>();
			List<int> list = new List<int>();

			var act1 = (int n, int m) => { list.Add(1); };
			var act2 = (int n, int m) => { list.Add(2); };
			var act3 = (int n, int m) => { list.Add(3); };

			list.Clear();
			sa.Clear();
			sa.AddAction(null, act1, -1);
			sa.AddAction(null, act2, 0);
			sa.AddAction(null, act3, 1);
			sa.Invoke(0, 0);
			Assert.AreEqual(list[0], 1);
			Assert.AreEqual(list[1], 2);
			Assert.AreEqual(list[2], 3);

			list.Clear();
			sa.Clear();
			sa.AddAction(null, act1, 1);
			sa.AddAction(null, act2, 0);
			sa.AddAction(null, act3, -1);
			sa.Invoke(0, 0);
			Assert.AreEqual(list[0], 3);
			Assert.AreEqual(list[1], 2);
			Assert.AreEqual(list[2], 1);
		}

		[TestMethod]
		public async Task TestException_noObj()
		{
			var sa = new SafeAction.SafeAction<int, int>();
			int n1 = 0;
			int n2 = 0;

			string strE = null;

			SafeAction.SafeAction.S_addLogFunction((e)=> strE = e.Message);

			var act1 = (int n, int m) => { n1++; };
			var act2 = (int n, int m) => { throw new Exception("test"); };
			var act3 = (int n, int m) => { n1++; };

			sa.AddAction(null, act1);
			sa.AddAction(null, act2);
			sa.AddAction(null, act3);
			sa.Invoke(0, 0);

			/*
			System.Action SystemAction = null;
			SystemAction += act1;
			SystemAction += act2;
			SystemAction += act3;
			SystemAction.Invoke(); // act2 have exception, act3 will not excute
			*/

			Assert.AreEqual(2, n1);
			Assert.AreEqual(strE, "test");
		}

		[TestMethod]
		public async Task TestAddSameAction_noObj()
		{
			var sa = new SafeAction.SafeAction<int, int>();
			int n1 = 0;

			var act1 = (int n, int m) => { n1++; };

			sa.AddAction(null, act1);
			sa.AddAction(null, act1);
			sa.AddAction(null, act1);
			sa.Invoke(0, 0);

			/*
			System.Action SystemAction = null;
			SystemAction += act1;
			SystemAction += act1;
			SystemAction += act1;
			SystemAction.Invoke(); // act1 will excute 3 times
			*/

			Assert.AreEqual(1, n1);
		}

		[TestMethod]
		public async Task TestRemoveActionByObj()
		{
			var sa = new SafeAction.SafeAction<int, int>();
			int n1 = 0;
			int n2 = 0;

			BindObj obj1 = new BindObj { isValide = true };

			var act1 = (int n, int m) => { n1++; };
			var act2 = (int n, int m) => { n2++; };

			sa.AddAction(obj1, act1);
			sa.AddAction(obj1, act2);
			Assert.AreEqual(2, sa.GetActionCount());

			sa.RemoveAction(obj1);
			Assert.AreEqual(0, sa.GetActionCount());
		}

		class BindObj
		{
			public bool isValide;
		}

		[TestMethod]
		public async Task TestProfile()
		{
			var sa = new SafeAction.SafeAction<int, int>("test1");
			int n1 = 0;
			int n2 = 0;

			var act1 = (int n, int m) => { n1++; };
			var act2 = (int n, int m) => { n2++; };

			sa.AddAction(null, act1);
			sa.AddAction(null, act2);

			var sb = new SafeAction.SafeAction<int, int>("test2");
			sb.AddAction(null, act1);

			var sc = new SafeAction.SafeAction<int, int>();
			sc.AddAction(null, act1);

			Dictionary<string, int> dictCount = new Dictionary<string, int>();
			Dictionary<string, long> dictTime = new Dictionary<string, long>();

			SafeAction.SafeAction.S_addProfileFunc(((string name, int count, long elapse_ms) param) =>
			{
				Assert.AreEqual(false, string.IsNullOrEmpty(param.name), "should not null");

				if (dictCount.ContainsKey(param.name))
					dictCount[param.name] += param.count;
				else
					dictCount[param.name] = param.count;

				if (dictTime.ContainsKey(param.name))
					dictTime[param.name] += param.elapse_ms;
				else
					dictTime[param.name] = param.elapse_ms;
			});

			for (int i = 0; i < 10; i++)
			{
				sa.Invoke(0, 0);
				sb.Invoke(0, 0);
				sc.Invoke(0, 0);

				Assert.AreEqual((i + 1) * 2, dictCount["test1"]);
				Assert.AreEqual((i + 1) * 1, dictCount["test2"]);
			}
		}
	}
}
