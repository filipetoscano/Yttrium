// $Id: ProcessOutput.cs 3 2014-09-23 15:22:42Z lft $
using System;
using System.Globalization;
using System.Text;

namespace Safira.Vsnet
{
    public sealed class ProcessOutput
    {
        public static string MakeRemark( string error, string context )
        {
            #region Validation

            if ( error == null )
                throw new ArgumentNullException( "error" );

            if ( context == null )
                throw new ArgumentNullException( "context" );

            #endregion

            string[] errorLines = error.Split( '\n' );

            StringBuilder sbe = new StringBuilder();
            sbe.AppendFormat( CultureInfo.InvariantCulture, "// Error: {0}\n", context );
            sbe.AppendFormat( CultureInfo.InvariantCulture, "//\n" );

            foreach ( string e in errorLines )
            {
                sbe.AppendFormat( CultureInfo.InvariantCulture, "// {0}\n", e.Trim() );
            }

            return sbe.ToString();
        }


        public static string ExceptionRemark( string context, Exception exception )
        {
            #region Validation

            if ( context == null )
                throw new ArgumentNullException( "context" );

            if ( exception == null )
                throw new ArgumentNullException( "exception" );

            #endregion

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat( CultureInfo.InvariantCulture, "// {0}\n", context );

            Exception ex = exception;

            do
            {
                sb.AppendFormat( CultureInfo.InvariantCulture, "//\n" );
                sb.AppendFormat( CultureInfo.InvariantCulture, "// Type={0}\n", exception.GetType().FullName );
                sb.AppendFormat( CultureInfo.InvariantCulture, "// Message={0}\n", exception.Message );
                sb.AppendFormat( CultureInfo.InvariantCulture, "// StackTrace={0}\n", exception.StackTrace.Replace( "\n", "\n// " ) );

                ex = ex.InnerException;
            } while ( ex != null );

            return sb.ToString();
        }


        private ProcessOutput()
        {
        }
    }
}

/* eof */