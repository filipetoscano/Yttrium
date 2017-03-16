using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yttrium.Core.Tests
{
    [TestClass]
    public class TestBool
    {
        public class CL1
        {
            [Option( Short = 'a' )]
            public bool PropertyA { get; set; }

            [Option( Short = 'b' )]
            public bool PropertyB { get; set; }

            [Option( Short = 'c', Long = "propc" )]
            public bool PropertyC { get; set; }

            [Option( Short = 'D', Long = "propd" )]
            public bool PropertyD { get; set; }

            public bool PropertyE { get; set; }
        }


        [TestMethod]
        public void TestEmpty()
        {
            var args = "".ToArgs();

            var cli = Command.Parse<CL1>( args );

            Assert.IsFalse( cli.PropertyA );
            Assert.IsFalse( cli.PropertyB );
            Assert.IsFalse( cli.PropertyC );
            Assert.IsFalse( cli.PropertyD );
            Assert.IsFalse( cli.PropertyE );
        }


        [TestMethod]
        public void TestShortSeperated()
        {
            var args = "-a -b -c -D".ToArgs();

            var cli = Command.Parse<CL1>( args );

            Assert.IsTrue( cli.PropertyA );
            Assert.IsTrue( cli.PropertyB );
            Assert.IsTrue( cli.PropertyC );
            Assert.IsTrue( cli.PropertyD );
            Assert.IsFalse( cli.PropertyE );
        }


        [TestMethod]
        public void TestShortPaired()
        {
            var args = "-ab -cD".ToArgs();

            var cli = Command.Parse<CL1>( args );

            Assert.IsTrue( cli.PropertyA );
            Assert.IsTrue( cli.PropertyB );
            Assert.IsTrue( cli.PropertyC );
            Assert.IsTrue( cli.PropertyD );
            Assert.IsFalse( cli.PropertyE );
        }


        [TestMethod]
        public void TestShortTogether()
        {
            var args = "-abcD".ToArgs();

            var cli = Command.Parse<CL1>( args );

            Assert.IsTrue( cli.PropertyA );
            Assert.IsTrue( cli.PropertyB );
            Assert.IsTrue( cli.PropertyC );
            Assert.IsTrue( cli.PropertyD );
            Assert.IsFalse( cli.PropertyE );
        }


        [TestMethod]
        public void TestExplicitLong()
        {
            var args = "--propc --propd".ToArgs();

            var cli = Command.Parse<CL1>( args );

            Assert.IsFalse( cli.PropertyA );
            Assert.IsFalse( cli.PropertyB );
            Assert.IsTrue( cli.PropertyC );
            Assert.IsTrue( cli.PropertyD );
            Assert.IsFalse( cli.PropertyE );
        }


        [TestMethod]
        public void TestImplicitLong()
        {
            var args = "--propertye".ToArgs();

            var cli = Command.Parse<CL1>( args );

            Assert.IsFalse( cli.PropertyA );
            Assert.IsFalse( cli.PropertyB );
            Assert.IsFalse( cli.PropertyC );
            Assert.IsFalse( cli.PropertyD );
            Assert.IsTrue( cli.PropertyE );
        }
    }
}
