using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Configuration.Tests
{
    [TestClass]
    public class Test_StringStream : TestBase
    {
        [TestMethod]
        public void TestBasics()
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

        [TestMethod]
        public void TestSkipUntil()
        {
            var content = "  \n\v\v\n\r\n\nhello world";
            var ss = new StringStream(content);

            var numSkipped = ss.SkipUntil(c => true);
            Assert.AreEqual(0, numSkipped);
            Assert.AreEqual(0, ss.Index);

            numSkipped = ss.SkipUntil(c => !char.IsWhiteSpace(c));
            Assert.AreEqual(9, numSkipped);
            Assert.AreEqual(9, ss.Index);
            Assert.AreEqual("hello world", ss.CurrentContent);

            numSkipped = ss.SkipUntil(c => false);
            Assert.AreEqual(11, numSkipped);
            Assert.IsTrue(ss.IsAtEndOfStream);
        }

        [TestMethod]
        public void TestSkipWhile()
        {
            var content = "  \n\v\v\n\r\n\nhello world";
            var ss = new StringStream(content);

            var numSkipped = ss.SkipWhile(c => false);
            Assert.AreEqual(0, numSkipped);
            Assert.AreEqual(0, ss.Index);

            numSkipped = ss.SkipWhile(c => char.IsWhiteSpace(c));
            Assert.AreEqual(9, numSkipped);
            Assert.AreEqual(9, ss.Index);
            Assert.AreEqual("hello world", ss.CurrentContent);

            numSkipped = ss.SkipWhile(c => true);
            Assert.AreEqual(11, numSkipped);
            Assert.IsTrue(ss.IsAtEndOfStream);
        }
    }
}
