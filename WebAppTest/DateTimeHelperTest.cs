using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApp.Helpers;

namespace WebAppTest
{
    [TestClass]
    public class DateTimeHelperTest
    {
        [TestMethod]
        public void GetUnixTimeStamp_Valid()
        {
            var unixTimeStamp = DateTimeHelpers.GetUnixTimeStamp(DateTime.Parse("2017-03-19 18:34:15"));
            Assert.AreEqual(1489948455, unixTimeStamp);
        }
    }
}
