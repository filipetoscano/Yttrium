using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yttrium.Core.Tests
{
    [TestClass]
    public class TestString
    {
        public class CL2
        {
            [Option( Short = 'a' )]
            public string PropertyA { get; set; }

            [Option( Short = 'B', Long = "propb" )]
            public string PropertyB { get; set; }

            public string PropertyC { get; set; }
        }


        [TestMethod]
        public void TestEmpty()
        {
            var args = "".ToArgs();

            var cli = Command.Parse<CL2>( args );

            Assert.IsNull( cli.PropertyA );
            Assert.IsNull( cli.PropertyB );
            Assert.IsNull( cli.PropertyC );
        }


        [TestMethod]
        public void TestImplicitShort()
        {
            var args = "-a hello".ToArgs();

            var cli = Command.Parse<CL2>( args );

            Assert.IsNotNull( cli.PropertyA );
            Assert.AreEqual( "hello", cli.PropertyA );
            Assert.IsNull( cli.PropertyB );
            Assert.IsNull( cli.PropertyC );
        }


        [TestMethod]
        public void TestExplicitShort()
        {
            var args = "-b hello".ToArgs();

            var cli = Command.Parse<CL2>( args );

            Assert.IsNull( cli.PropertyA );
            Assert.IsNotNull( cli.PropertyB );
            Assert.AreEqual( "hello", cli.PropertyB );
            Assert.IsNull( cli.PropertyC );
        }


        [TestMethod]
        public void TestExplicitLong()
        {
            var args = "--propb=hello".ToArgs();

            var cli = Command.Parse<CL2>( args );

            Assert.IsNull( cli.PropertyA );
            Assert.IsNotNull( cli.PropertyB );
            Assert.AreEqual( "hello", cli.PropertyB );
            Assert.IsNull( cli.PropertyC );
        }


        [TestMethod]
        public void TestImplicitLong()
        {
            var args = "--propertyc=hello".ToArgs();

            var cli = Command.Parse<CL2>( args );

            Assert.IsNull( cli.PropertyA );
            Assert.IsNull( cli.PropertyB );
            Assert.IsNotNull( cli.PropertyC );
            Assert.AreEqual( "hello", cli.PropertyC );
        }
    }
}
