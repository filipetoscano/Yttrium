using CustomToolGenerator;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Yttrium.VisualStudio
{
    [ComVisible( true )]
    [Guid( "916e345d-f9fd-4187-8dc9-610f24d402f9" )]
    public abstract class BaseTool : BaseCodeGeneratorWithSite
    {
        protected string FileNameSpace
        {
            get;
            private set;
        }


        protected string FileName
        {
            get;
            private set;
        }


        [SuppressMessage( "Microsoft.Usage", "CA2202:Do not dispose objects multiple times" )]
        public string Execute( FileInfo info, string @namespace )
        {
            #region Validation

            if ( info == null )
                throw new ArgumentNullException( "info" );

            if ( @namespace == null )
                throw new ArgumentNullException( "@namespace" );

            #endregion

            this.FileNameSpace = @namespace;
            this.FileName = info.FullName;

            string content;

            using ( FileStream fs = info.OpenRead() )
            {
                using ( StreamReader sr = new StreamReader( fs ) )
                {
                    content = sr.ReadToEnd();
                }
            }

            return DoGenerateCode( content );
        }


        [SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes" )]
        protected override byte[] GenerateCode( string fileNamespace, string inputFileName, string inputFileContent )
        {
            this.FileName = inputFileName;
            this.FileNameSpace = fileNamespace;

            string s;

            try
            {
                s = DoGenerateCode( inputFileContent );
            }
            catch ( ToolException ex )
            {
                s = ErrorEmit( "", ex );
            }
            catch ( Exception ex )
            {
                s = ErrorEmit( "// Unhandled exception", ex );
            }

            return Encoding.UTF8.GetBytes( s );
        }


        protected abstract string DoGenerateCode( string inputFileContent );



        public static string ErrorEmit( string error )
        {
            #region Validation

            if ( error == null )
                throw new ArgumentNullException( "error" );

            #endregion

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat( CultureInfo.InvariantCulture, "// Error: {0}\n", ML( error ) );

            return sb.ToString();
        }


        public static string ErrorEmit( string context, string error )
        {
            #region Validation

            if ( context == null )
                throw new ArgumentNullException( "context" );

            if ( error == null )
                throw new ArgumentNullException( "error" );

            #endregion

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat( CultureInfo.InvariantCulture, "// Error: {0}\n", context );
            sb.AppendFormat( CultureInfo.InvariantCulture, "//\n" );
            sb.AppendFormat( CultureInfo.InvariantCulture, "// {0}", ML( error ) );

            return sb.ToString();
        }


        public static string ErrorEmit( string context, Exception exception )
        {
            #region Validation

            if ( context == null )
                throw new ArgumentNullException( "context" );

            if ( exception == null )
                throw new ArgumentNullException( "exception" );

            #endregion

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat( CultureInfo.InvariantCulture, "// Error: {0}\n", context );

            Exception ex = exception;

            do
            {
                sb.AppendFormat( CultureInfo.InvariantCulture, "//\n" );
                sb.AppendFormat( CultureInfo.InvariantCulture, "// Type={0}\n", exception.GetType().FullName );
                sb.AppendFormat( CultureInfo.InvariantCulture, "// Message={0}\n", ML( exception.Message ) );
                sb.AppendFormat( CultureInfo.InvariantCulture, "// StackTrace={0}\n", ML( exception.StackTrace ) );

                ex = ex.InnerException;
            } while ( ex != null );

            return sb.ToString();
        }


        private static string ML( string value )
        {
            return ML( "// ", value );
        }


        private static string ML( string prefix, string value )
        {
            if ( value == null )
                return "";

            return value.Replace( "\n", "\n" + prefix );
        }
    }
}

/* eof */