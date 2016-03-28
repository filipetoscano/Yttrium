using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Yttrium.VisualStudio
{
    [SuppressMessage( "Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors" )]
    public class XsltExtensionObject
    {
        public XsltExtensionObject()
        {
        }


        /// <summary>
        /// Generates a word-wrapped text.
        /// </summary>
        /// <param name="text">The text which is to be wrapped.</param>
        /// <param name="prepend">The text which is prepended to every line.</param>
        /// <param name="lineSize">The maximum length per line.</param>
        /// <returns>The value in 'text', word wrapped to a maximum of 'lineSize' characters.</returns>
        public static string ToWrap( string text, string prepend, int lineSize )
        {
            if ( text == null )
                text = "";
            else
                text = ToSingleLine( text );

            if ( prepend == null )
                prepend = "// ";

            if ( lineSize < 20 )
                lineSize = 20;


            /*
             * 
             */
            lineSize -= prepend.Length;

            if ( lineSize < 20 )
                lineSize = 20;

            
            /*
             * 
             */
            List<string> lines = new List<string>();

            int currentIndex;
            var lastWrap = 0;
            var whitespace = new[] { ' ', '\r', '\n', '\t' };

            do
            {
                currentIndex = lastWrap + lineSize > text.Length ? text.Length : ( text.LastIndexOfAny( new[] { ' ', ',', '.', '?', '!', ':', ';', '-', '\n', '\r', '\t' }, Math.Min( text.Length - 1, lastWrap + lineSize ) ) + 1 );

                if ( currentIndex <= lastWrap )
                    currentIndex = Math.Min( lastWrap + lineSize, text.Length );

                lines.Add( text.Substring( lastWrap, currentIndex - lastWrap ).Trim( whitespace ) );
                lastWrap = currentIndex;
            } while ( currentIndex < text.Length );


            /*
             * 
             */
            StringBuilder sb = new StringBuilder();

            foreach ( string l in lines )
            {
                sb.Append( "\n" );
                sb.Append( prepend );
                sb.Append( l );
            }

            return sb.ToString();
        }


        private static string ToSingleLine( string text )
        {
            if ( text == null )
                return "";

            StringBuilder sb = new StringBuilder();

            foreach ( string l in text.Split( '\n' ) )
            {
                sb.Append( l.Trim() );
                sb.Append( " " );
            }

            return sb.ToString();
        }


        /*
         * SQL
         */
        public static string SafeSql( string value )
        {
            if ( value == null )
                return "";

            if ( value.Length == 0 )
                return "";

            return value.Replace( "'", "''" );
        }


        public static string ToSql( string value )
        {
            if ( value == null )
                return "null";

            return string.Concat( "'", SafeSql( value ), "'" );
        }


        /*
         * String
         */
        public static string SafeString( string value )
        {
            if ( value == null )
                return "";

            if ( value.Length == 0 )
                return "";

            return value.Replace( "\"", "\\\"" );
        }


        public static string ToString( string value )
        {
            if ( value == null )
                return "";

            return string.Concat( "\"", SafeString( value ), "\"" );
        }
    }
}

/* eof */