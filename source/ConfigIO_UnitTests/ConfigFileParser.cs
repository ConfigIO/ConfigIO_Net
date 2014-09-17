using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Configuration.Tests
{
    [TestClass]
    public class ConfigFileParser
    {
        [TestMethod]
        public void TestStringStream()
        {
            var content = "abcdef";
            var ss = new StringStream(content);
            Assert.AreEqual(content, ss.Content);
            Assert.AreEqual(content, ss.CurrentContent);
            Assert.AreEqual('a', ss.Current);
            Assert.AreEqual(0, ss.Index);
            Assert.AreEqual(false, ss.IsAtEndOfStream);
            Assert.AreEqual(true, ss.IsAt('a'));

            var character = ss.Read();
            Assert.AreEqual('a', character);
            Assert.AreEqual('b', ss.Current);

            var numSkipped = ss.Skip('a');
            Assert.AreEqual(0, numSkipped, "Nothing should have happened.");

            numSkipped = ss.Skip('b');
            Assert.AreEqual(1, numSkipped, "Nothing should have happened.");

            ss.Seek(0, StringStreamPosition.Beginning);
            Assert.AreEqual(0, ss.Index);

            ss.Seek(0, StringStreamPosition.Current);
            Assert.AreEqual(0, ss.Index, "Nothing should have happened.");
            ss.Seek(1, StringStreamPosition.Current);
            Assert.AreEqual(1, ss.Index);
            ss.Seek(1, StringStreamPosition.Current);
            Assert.AreEqual(2, ss.Index);

            ss.Seek(0, StringStreamPosition.End);
            Assert.IsFalse(ss.IsAtEndOfStream);
            Assert.AreEqual(ss.Index, content.Length - 1);

            ss.Seek(-1, StringStreamPosition.End);
            Assert.IsTrue(ss.IsAtEndOfStream);
        }
    }
}
