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
	public class TestSafeActionStatic
	{
		class ValideObj
		{
			public bool isValide;
		}

		[TestMethod]
		public async Task TestValide_obj_0()
		{
			var sa = new SafeAction.SafeAction();
			int n1 = 0;
			int n2 = 0;

			ValideObj obj1 = new ValideObj { isValide = true };
			ValideObj obj2 = new ValideObj { isValide = true };
			SafeAction.SafeAction.S_addCheckValideFunc(typeof(ValideObj), (obj) =>
			{
				return ((ValideObj)obj).isValide;
			});

			var act1 = () => { n1++; };
			var act2 = () => { n2++; };

			sa.AddAction(obj1, act1);
			sa.AddAction(obj2, act2);
			sa.Invoke();
			Assert.AreEqual(1, n1);
			Assert.AreEqual(1, n2);
			Assert.AreEqual(2, sa.GetActionCount());

			obj2.isValide = false;
			sa.Invoke();
			Assert.AreEqual(2, n1);
			Assert.AreEqual(1, n2);
			Assert.AreEqual(1, sa.GetActionCount());

			obj1.isValide = false;
			sa.Invoke();
			Assert.AreEqual(2, n1);
			Assert.AreEqual(1, n2);
			Assert.AreEqual(0, sa.GetActionCount());
		}

		

		[TestMethod]
		public async Task TestCheckValide_0()
		{
			var sa = new SafeAction.SafeAction();
			var objA = new A();
			var objB = new B();
			var objC = new C();

			var act = () => { };

			string str = "";
			var checkA = (System.Object obj) => { str = "A"; return true; };
			var checkB = (System.Object obj) => { str = "B"; return true; };
			var checkC = (System.Object obj) => { str = "C"; return true; };

			void Reset()
			{
				str = "";
				SafeAction.SafeAction.S_clearCheckValideFunc();
				sa.Clear();
			}

			var cfg = new List<(int addFunc, int addAction, string requestResult)>
			{
				(0, 0, "A"),
				(1, 0, ""),
				(2, 0, ""),
				(0, 1, "A"),
				(1, 1, "B"),
				(2, 1, ""),
				(0, 2, "A"),
				(1, 2, "B"),
				(2, 2, "C"),
			};

			for (int i = 0; i < cfg.Count; i++)
			{
				Reset();
				switch (cfg[i].addFunc)
				{
					case 0:
						SafeAction.SafeAction.S_addCheckValideFunc(typeof(A), checkA);
						break;
					case 1:
						SafeAction.SafeAction.S_addCheckValideFunc(typeof(B), checkB);
						break;
					case 2:
						SafeAction.SafeAction.S_addCheckValideFunc(typeof(C), checkC);
						break;
				}
				switch (cfg[i].addAction)
				{
					case 0:
						sa.AddAction(objA, act);
						break;
					case 1:
						sa.AddAction(objB, act);
						break;
					case 2:
						sa.AddAction(objC, act);
						break;
				}
				sa.Invoke();
				Assert.AreEqual(cfg[i].requestResult, str, message: $"the {i}th cfg");
			}
		}

		[TestMethod]
		public async Task TestCheckValide_addOrder_0()
		{
			var sa = new SafeAction.SafeAction();
			var objA = new A();
			var objB = new B();
			var objC = new C();

			var act = () => { };

			string str = "";
			var checkA = (System.Object obj) => { str = "A"; return true; };
			var checkB = (System.Object obj) => { str = "B"; return true; };
			var checkC = (System.Object obj) => { str = "C"; return true; };

			void Reset()
			{
				str = "";
				SafeAction.SafeAction.S_clearCheckValideFunc();
				sa.Clear();
			}
			//
			Reset();
			SafeAction.SafeAction.S_addCheckValideFunc(typeof(A), checkA);
			SafeAction.SafeAction.S_addCheckValideFunc(typeof(B), checkB);
			SafeAction.SafeAction.S_addCheckValideFunc(typeof(C), checkC);
			sa.AddAction(objB, act);
			sa.Invoke();
			Assert.AreEqual("B", str);
			//
			Reset();
			SafeAction.SafeAction.S_addCheckValideFunc(typeof(C), checkC);
			SafeAction.SafeAction.S_addCheckValideFunc(typeof(B), checkB);
			SafeAction.SafeAction.S_addCheckValideFunc(typeof(A), checkA);
			sa.AddAction(objB, act);
			sa.Invoke();
			Assert.AreEqual("B", str);
		}

		class A { public bool isValide = true; }
		class B : A {}
		class C : B {}
	}
}
