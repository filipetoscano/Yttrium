using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Yttrium.Core.Tests
{
    [TestClass]
    public class TestMode
    {
        public class CL31
        {
            [Option( Short = 'a' )]
            public bool PropertyA { get; set; }
        }


        public class CL3
        {
            [Verb]
            public CL31 Mode1 { get; set; }

            [Verb]
            public CL31 Mode2 { get; set; }
        }


        [TestMethod]
        public void TestEmpty()
        {
            var args = "".ToArgs();

            var cli = Command.Parse<CL3>( args );

            Assert.IsTrue( cli.Mode1 == null );
            Assert.IsTrue( cli.Mode2 == null );
        }


        [TestMethod]
        public void TestModeOk()
        {
            var args = "mode1 -a".ToArgs();

            var cli = Command.Parse<CL3>( args );

            Assert.IsTrue( cli.Mode1 != null );
            Assert.IsTrue( cli.Mode1.PropertyA == true );
            Assert.IsTrue( cli.Mode2 == null );
        }
    }
}
