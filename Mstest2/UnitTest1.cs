using TESTME;

namespace Mstest2

{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Database bdd = new Database();
                bdd.GetConnection();
        }
    }
}