using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Xml;

namespace Yttrium.DbConfig
{
    [SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "U" )]
    public static class U
    {
        private static XmlNamespaceManager _manager;
        private static Regex _regex1;
        private static Regex _regex2;
        private static Regex _regex3;
        private static Regex _regex4;


        public static XmlNamespaceManager Manager
        {
            get
            {
                if ( _manager == null )
                {
                    XmlNamespaceManager manager = new XmlNamespaceManager( new NameTable() );
                    manager.AddNamespace( "xsi", "http://www.w3.org/2001/XMLSchema-instance" );

                    _manager = manager;
                }

                return _manager;
            }
        }


        public static Regex Transform
        {
            get
            {
                if ( _regex1 == null )
                    _regex1 = new Regex( "transform=\"(?<transform>.*?)\"", RegexOptions.ExplicitCapture );

                return _regex1;
            }
        }

        public static Regex Environment
        {
            get
            {
                if ( _regex2 == null )
                    _regex2 = new Regex( "env=\"(?<env>.*?)\"", RegexOptions.ExplicitCapture );

                return _regex2;
            }
        }

        public static Regex Suffix
        {
            get
            {
                if ( _regex3 == null )
                    _regex3 = new Regex( "suffix=\"(?<suffix>.*?)\"", RegexOptions.ExplicitCapture );

                return _regex3;
            }
        }

        [SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi" )]
        public static Regex Multi
        {
            get
            {
                if ( _regex4 == null )
                    _regex4 = new Regex( "multi=\"(?<multi>.*?)\"", RegexOptions.ExplicitCapture );

                return _regex4;
            }
        }
    }
}

/* eof */