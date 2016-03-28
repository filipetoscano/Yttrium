using System.Diagnostics.CodeAnalysis;

namespace Yttrium.OraPack
{
    public class FileContent
    {
        /// <summary>
        /// Section of header file: contains global type definitions and
        /// public variables.
        /// </summary>
        [SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Globals" )]
        public string HeaderGlobals
        {
            get;
            set;
        }

        /// <summary>
        /// Section of header file: contains public method declarations.
        /// </summary>
        public string Header
        {
            get;
            set;
        }

        /// <summary>
        /// Section of body file: contains private type definitions and
        /// private variables.
        /// </summary>
        [SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Globals" )]
        public string BodyGlobals
        {
            get;
            set;
        }

        /// <summary>
        /// Section of body file: contains private method declarations.
        /// </summary>
        public string BodyHeader
        {
            get;
            set;
        }

        /// <summary>
        /// Section of body file: contains implementation of all methods
        /// contained in the package.
        /// </summary>
        public string Body
        {
            get;
            set;
        }

        /// <summary>
        /// Footer, after the package creation.
        /// </summary>
        public string Footer
        {
            get;
            set;
        }
    }
}

/* eof */