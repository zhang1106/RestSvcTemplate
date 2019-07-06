using System;
using NUnit.Framework;

namespace UnitTest
{
    [TestFixture(Category = "Integration")]
    class TestIntegration
    {
        [Test]
        public void TestIntg()
        {
            Assert.IsTrue(1 == 1);
        }
    }
}
