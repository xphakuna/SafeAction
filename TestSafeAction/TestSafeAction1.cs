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
	public class TestSafeAction1
	{
		[TestMethod]
		public async Task TestAddRemove_noObj_0()
		{
			var sa = new SafeAction.SafeAction<int>();
			int n1 = 0;
			int n2 = 0;

			var act1 = (int n) => { n1++; };
			var act2 = (int n) => { n2++; };

			sa.AddAction(null, act1);
			sa.AddAction(null, act2);
			sa.Invoke(0);

			Assert.AreEqual(1, n1);
			Assert.AreEqual(1, n2);

			sa.RemoveAction(null, act1);
			sa.Invoke(0);
			Assert.AreEqual(1, n1);
			Assert.AreEqual(2, n2);
		}

		[TestMethod]
		public async Task TestAddRemove_order_noObj_0()
		{
			var sa = new SafeAction.SafeAction<int>();
			List<int> list = new List<int>();

			var act1 = (int n) => { list.Add(1); };
			var act2 = (int n) => { list.Add(2); };
			var act3 = (int n) => { list.Add(3); };

			list.Clear();
			sa.Clear();
			sa.AddAction(null, act1, -1);
			sa.AddAction(null, act2, 0);
			sa.AddAction(null, act3, 1);
			sa.Invoke(0);
			Assert.AreEqual(list[0], 1);
			Assert.AreEqual(list[1], 2);
			Assert.AreEqual(list[2], 3);

			list.Clear();
			sa.Clear();
			sa.AddAction(null, act1, 1);
			sa.AddAction(null, act2, 0);
			sa.AddAction(null, act3, -1);
			sa.Invoke(0);
			Assert.AreEqual(list[0], 3);
			Assert.AreEqual(list[1], 2);
			Assert.AreEqual(list[2], 1);
		}

		[TestMethod]
		public async Task TestException_noObj_0()
		{
			var sa = new SafeAction.SafeAction<int>();
			int n1 = 0;
			int n2 = 0;

			string strE = null;

			SafeAction.SafeAction.S_addLogFunction((e)=> strE = e.Message);

			var act1 = (int n) => { n1++; };
			var act2 = (int n) => { throw new Exception("test"); };
			var act3 = (int n) => { n1++; };

			sa.AddAction(null, act1);
			sa.AddAction(null, act2);
			sa.AddAction(null, act3);
			sa.Invoke(0);

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
		public async Task TestAddSameAction_noObj_0()
		{
			var sa = new SafeAction.SafeAction<int>();
			int n1 = 0;

			var act1 = (int n) => { n1++; };

			sa.AddAction(null, act1);
			sa.AddAction(null, act1);
			sa.AddAction(null, act1);
			sa.Invoke(0);

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
		public async Task TestRemoveActionByObj_0()
		{
			var sa = new SafeAction.SafeAction<int>();
			int n1 = 0;
			int n2 = 0;

			BindObj obj1 = new BindObj { isValide = true };

			var act1 = (int n) => { n1++; };
			var act2 = (int n) => { n2++; };

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
	}
}
