
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace UnitTests {
	[TestClass]
	public class UnitTest1 {
		[TestMethod]
		public async Task TestMethod1() {
			var classes = await WindesheimRooster.BusinessLayer.WindesheimManager.GetAllClasses();
		}
	}
}
