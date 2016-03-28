using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Yttrium.DbConfig
{
    public class XsltExtensionObject
    {
        public XsltExtensionObject()
        {
        }


        [SuppressMessage( "Microsoft.Performance", "CA1822:MarkMembersAsStatic" )]
        public string Escape( string value )
        {
            if ( value == null )
                return "";

            return value.Replace( "'", "''" );
        }


        [SuppressMessage( "Microsoft.Performance", "CA1822:MarkMembersAsStatic" )]
        public string Today()
        {
            return DateTime.UtcNow.ToString( "yyyy-MM-dd", CultureInfo.InvariantCulture );
        }


        [SuppressMessage( "Microsoft.Performance", "CA1822:MarkMembersAsStatic" )]
        public string Now( string format )
        {
            return DateTime.UtcNow.ToString( format, CultureInfo.InvariantCulture );
        }
    }
}

/* eof */
