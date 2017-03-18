using System;
using System.Collections.Generic;
using System.IO;
using Yttrium.Scaffold.Description;

namespace Yttrium.Scaffold
{
    /// <summary />
    public static class Extensions
    {
        /// <summary />
        public static void Add( this Dictionary<string, string> dict, placeholder[] placeholders, string name, string value )
        {
            #region Validations

            if ( dict == null )
                throw new ArgumentNullException( nameof( dict ) );

            if ( placeholders == null )
                throw new ArgumentNullException( nameof( placeholders ) );

            if ( name == null )
                throw new ArgumentNullException( nameof( name ) );

            if ( value == null )
                throw new ArgumentNullException( nameof( value ) );

            #endregion

            foreach ( var ph in placeholders )
            {
                string keyN = string.Format( ph.format, name );
                string keyL = string.Format( ph.format, name.ToLowerInvariant() );
                string keyU = string.Format( ph.format, name.ToUpperInvariant() );

                dict.Add( keyN, value );

                if ( dict.ContainsKey( keyL ) == false )
                    dict.Add( keyL, value.ToLowerInvariant() );

                if ( dict.ContainsKey( keyU ) == false )
                    dict.Add( keyU, value.ToUpperInvariant() );
            }
        }


        private static List<string> binary = new List<string>()
        {
            ".exe",
            ".dll",
            ".ico",
            ".png",
            ".gif"
        };


        /// <summary>
        /// Checks whether the current file is a binary file.
        /// </summary>
        /// <remarks>
        /// True if the file is binary, False otherwise.
        /// </remarks>
        public static bool IsBinaryFile( this FileInfo file )
        {
            #region Validations

            if ( file == null )
                throw new ArgumentNullException( nameof( file ) );

            #endregion

            return binary.Contains( file.Extension );
        }
    }
}
