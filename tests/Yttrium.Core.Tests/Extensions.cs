using System;

namespace Yttrium.Core.Tests
{
    public static class Extensions
    {
        public static string[] ToArgs( this string line )
        {
            if ( string.IsNullOrEmpty( line ) == true )
                return new string[] { };

            return line.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
        }
    }
}
