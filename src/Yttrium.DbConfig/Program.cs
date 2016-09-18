using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Xsl;

namespace Yttrium.DbConfig
{
    public static class Program
    {
        public static int ErrorCount
        {
            get;
            set;
        }

        public static CommandLine PO
        {
            get;
            set;
        }


        static void Main( string[] args )
        {
            /*
             *
             */
            CommandLine cl = new CommandLine();

            if ( cl.Parse( args ) == false )
            {
                Environment.Exit( 1001 );
                return;
            }

            if ( cl.Help == true )
            {
                cl.HelpShow();
                Environment.Exit( 1002 );
                return;
            }


            /*
             * 
             */
            string[] list = Glob.Do( cl.FilePatterns.ToArray() );

            if ( list.Length == 0 )
            {
                WL( "dbconfig: no matches found" );
                Environment.Exit( -2 );
                return;
            }


            /*
             * 
             */
            PO = cl;

            foreach ( string file in list )
            {
                string rel = Path2.RelativePath( file );
                WO( "file: {0}", rel );

                WorkFile( file );
            }


            /*
             * 
             */
            Environment.Exit( ErrorCount );
        }


        [SuppressMessage( "Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity" )]
        [SuppressMessage( "Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling" )]
        private static void WorkFile( string file )
        {
            #region Validations

            if ( file == null )
                throw new ArgumentNullException( "file" );

            #endregion

            /*
             * #1. Check if file exists.
             */
            FileInfo finfo = new FileInfo( file );

            if ( finfo.Exists == false )
            {
                WF();
                WL( "* file '{0}' does not exist.", finfo.FullName );
                return;
            }


            /*
             * #2a. Load the document
             */
            if ( PO.VerboseOutput == true )
            {
                WL();
                WO( "  loading... " );
            }

            XmlDocument doc;

            try
            {
                doc = LoadDocument( file );
            }
            catch ( ApplicationException ex )
            {
                WF();
                WEx( ex.InnerException );
                return;
            }


            /*
             * #2b. Does this XML file have the dbconfig processing instruction? If yes,
             * then it is a compatible XML file to be processes.
             */
            XmlNodeList piList = doc.SelectNodes( " processing-instruction('dbconfig') " );

            if ( piList == null || piList.Count == 0 )
            {
                if ( PO.VerboseOutput == true )
                    WL( "processing instruction not found! skip" );
                else
                    WL( " skip" );

                return;
            }


            /*
             * #2c. Does it have a schema associated with it? If yes, then load the schema
             * and confirm that the XML file is correct according to the schema.
             */
            if ( ValidateDocument( finfo, doc ) == false )
                return;

            if ( PO.VerboseOutput == false )
                WO( "." );
            else
                WL( "ok" );


            /*
             * 
             */
            int piCount = 0;

            foreach ( XmlProcessingInstruction pi in piList )
            {
                if ( PO.VerboseOutput == true )
                    WO( "  pi #{0}: ", ++piCount );


                /*
                 * 
                 */
                Match m1 = U.Transform.Match( pi.Value );
                Match m2 = U.Environment.Match( pi.Value );
                Match m3 = U.Suffix.Match( pi.Value );
                Match m4 = U.Multi.Match( pi.Value );

                if ( m1.Success == false )
                {
                    WF();
                    WL( "processing missing @transform" );

                    return;
                }


                /*
                 * 
                 */
                string transformFile;
                string specificSuffix = "";
                bool hasEnvironments = false;
                bool isMultiOutput = false;

                transformFile = m1.Groups[ "transform" ].Value;

                if ( m2.Success == true )
                    hasEnvironments = ( m2.Groups[ "env" ].Value == "true" );

                if ( m3.Success == true )
                    specificSuffix = m3.Groups[ "suffix" ].Value;

                if ( m4.Success == true )
                    isMultiOutput = ( m4.Groups[ "multi" ].Value == "true" );

                List<string> environments = EnvironmentList( hasEnvironments, PO.Environment );


                /*
                 * 
                 */
                if ( PO.VerboseOutput == true )
                    WO( "loading..." );

                string xsltFilename = Path.Combine( finfo.DirectoryName, transformFile );
                XslCompiledTransform xslt;

                try
                {
                    xslt = LoadTransform( xsltFilename );
                }
                catch ( ApplicationException ex )
                {
                    WF();
                    WL( ex.Message );
                    WEx( ex.InnerException );

                    return;
                }


                /*
                 * 
                 */
                foreach ( string env in environments )
                {
                    if ( PO.VerboseOutput == true )
                    {
                        if ( environments.Count > 1 )
                            WO( " " + env );
                        else
                            WO( " transforming... " );
                    }
                    else
                    {
                        WO( "." );
                    }

                    string outputName = OutputFileName( finfo, hasEnvironments, env, specificSuffix, PO.SimpleOutputName );
                    TransformResult result = new TransformResult();

                    XsltArgumentList xsltArgs = new XsltArgumentList();
                    xsltArgs.AddExtensionObject( "urn:dbconfig", new XsltExtensionObject() );
                    xsltArgs.AddParam( "ToolVersion", "", PO.ToolVersionGet() );
                    xsltArgs.AddParam( "Locale", "", PO.Locale );
                    xsltArgs.AddParam( "Environment", "", env );
                    xsltArgs.AddParam( "Enforce", "", PO.Enforce ? "true" : "false" );
                    xsltArgs.XsltMessageEncountered += new XsltMessageEncounteredEventHandler(
                        delegate( object sender, XsltMessageEncounteredEventArgs e )
                        {
                            string message = e.Message;
                            result.Add( message );
                        }
                    );

                    if ( PO.Parameters != null && PO.Parameters.Count > 0 )
                    {
                        foreach ( string kv in PO.Parameters )
                        {
                            string[] p = kv.Split( new char[] { '=' }, 2 );

                            if ( p.Length != 2 )
                                continue;

                            xsltArgs.AddParam( p[ 0 ], "", p[ 1 ] );
                        }
                    }

                    if ( isMultiOutput == true )
                    {
                        StringBuilder sb = new StringBuilder();

                        using ( TextWriter tw = new StringWriter( sb, CultureInfo.InvariantCulture ) )
                        {
                            xslt.Transform( doc, xsltArgs, tw );
                        }

                        EmitTransformationMessages( result );

                        if ( result.Errors.Count > 0 )
                            continue;

                        ExpandMultiOutput( finfo, sb.ToString(), hasEnvironments, env );
                    }
                    else
                    {
                        using ( TextWriter tw = new StreamWriter( outputName ) )
                        {
                            xslt.Transform( doc, xsltArgs, tw );
                        }

                        EmitTransformationMessages( result );

                        if ( result.Errors.Count > 0 )
                            File.Delete( outputName );
                    }
                } // env

                if ( PO.VerboseOutput == true )
                    WL( " ok" );

            } // pi

            if ( PO.VerboseOutput == false )
                WL( " ok" );
        }


        private static void ExpandMultiOutput( FileInfo fileInfo, string content, bool hasEnvironments, string environment )
        {
            #region Validations

            if ( fileInfo == null )
                throw new ArgumentNullException( "fileInfo" );

            if ( content == null )
                throw new ArgumentNullException( "content" );

            if ( environment == null )
                throw new ArgumentNullException( "environment" );

            #endregion

            StringBuilder sb = null;
            string outputName = null;


            /*
             * 
             */
            string[] lines = content.Split( new string[] { Environment.NewLine }, StringSplitOptions.None );

            foreach ( string line in lines )
            {
                if ( line.StartsWith( "-- dbconfig:multi=", StringComparison.Ordinal ) == true )
                {
                    if ( outputName != null )
                    {
                        File.WriteAllText( outputName, sb.ToString() );
                        Console.Write( "^" );
                    }

                    sb = new StringBuilder();
                    outputName = OutputFileName2( fileInfo, line.Substring( "-- dbconfig:multi=".Length ), hasEnvironments, environment );
                }
                else if ( outputName != null )
                {
                    sb.AppendLine( line );
                }
            }

            if ( outputName != null )
            {
                File.WriteAllText( outputName, sb.ToString() );
                Console.Write( "^" );
            }
        }


        private static void EmitTransformationMessages( TransformResult result )
        {
            #region Validations

            if ( result == null )
                throw new ArgumentNullException( "result" );

            #endregion


            /*
             * 
             */
            if ( result.Errors.Count == 0
                && result.Warnings.Count == 0
                && result.Info.Count == 0 )
            {
                return;
            }


            /*
             * 
             */
            Console.WriteLine( "" );

            foreach ( string l in result.Errors )
            {
                Console.WriteLine( "ERR: " + l );
            }

            foreach ( string l in result.Warnings )
            {
                Console.WriteLine( "WRN: " + l );
            }

            foreach ( string l in result.Info )
            {
                Console.WriteLine( "INF: " + l );
            }
        }


        private static bool ValidateDocument( FileInfo fileInfo, XmlDocument document )
        {
            #region Validations

            if ( fileInfo == null )
                throw new ArgumentNullException( "fileInfo" );

            if ( document == null )
                throw new ArgumentNullException( "document" );

            #endregion

            /*
             * 
             */
            XmlNode schema = document.SelectSingleNode( " */@xsi:schemaLocation ", U.Manager );

            if ( schema == null )
                return true;


            /*
             * 
             */
            string[] parts = schema.Value.Replace( "\n", " " ).Replace( "\r", "" ).Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

            if ( ( parts.Length % 2 ) != 0 )
            {
                // invalid value for xsi:schemaLocation
                return false;
            }

            if ( PO.VerboseOutput == true )
                WO( "building schema... " );

            for ( int i = 0; i < parts.Length; i += 2 )
            {
                string xsdPath = Path.Combine( fileInfo.DirectoryName, parts[ i + 1 ] );

                try
                {
                    document.Schemas.Add( parts[ i ], xsdPath );
                }
                catch ( XmlSchemaException ex )
                {
                    WF();
                    WL( "schema file '{0}' is invalid", xsdPath );
                    WEx( ex );

                    return false;
                }
                catch ( FileNotFoundException ex )
                {
                    WF();
                    WL( "schema file '{0}' not found", xsdPath );
                    WEx( ex );

                    return false;
                }
            }

            if ( PO.VerboseOutput == false )
                WO( "." );


            /*
             * 
             */
            List<string> validationErrors = new List<string>();

            if ( PO.VerboseOutput == true )
                WO( "validating... " );

            try
            {
                document.Validate(
                    new ValidationEventHandler(
                        delegate( Object sender, ValidationEventArgs e )
                        {
                            validationErrors.Add( e.Exception.Message );
                        }
                    )
                );
            }
            catch ( XmlSchemaValidationException ex )
            {
                WF();
                WL( "schema validation error" );
                WEx( ex );

                return false;
            }

            if ( validationErrors.Count > 0 )
            {
                WF();

                foreach ( string error in validationErrors )
                {
                    WL( error );
                }

                return false;
            }

            return true;
        }


        private static string OutputFileName( FileInfo fileInfo, bool hasEnvironments, string environment, string specificSuffix, bool simpleFileName )
        {
            #region Validations

            if ( fileInfo == null )
                throw new ArgumentNullException( "fileInfo" );

            if ( environment == null )
                throw new ArgumentNullException( "environment" );

            if ( specificSuffix == null )
                throw new ArgumentNullException( "specificSuffix" );

            #endregion

            /*
             * 
             */
            string output;

            if ( string.IsNullOrEmpty( fileInfo.Extension ) == true )
                output = fileInfo.Name;
            else
                output = fileInfo.Name.Substring( 0, fileInfo.Name.Length - fileInfo.Extension.Length );


            /*
             * 
             */
            if ( string.IsNullOrEmpty( specificSuffix ) == false )
                output = string.Concat( output, specificSuffix );


            /*
             * 
             */
            if ( simpleFileName == false && hasEnvironments == true )
            {
                string suffixFormat = ConfigurationManager.AppSettings[ "Environment.FileNameSuffix" ];

                output = string.Concat( output,
                            string.Format( CultureInfo.InvariantCulture, suffixFormat, environment ) );
            }


            /*
             * 
             */
            output = string.Concat( output, ".sql" );

            return Path.Combine( fileInfo.DirectoryName, output );
        }


        private static string OutputFileName2( FileInfo fileInfo, string relativePath, bool hasEnvironments, string environment )
        {
            #region Validations

            if ( fileInfo == null )
                throw new ArgumentNullException( "fileInfo" );

            if ( relativePath == null )
                throw new ArgumentNullException( "relativePath" );

            if ( environment == null )
                throw new ArgumentNullException( "environment" );


            #endregion


            /*
             * As an example, consider that the original XML file is at the following
             * location on the filesystem:
             *     C:\Data\File.xml
             * 
             * If the XSLT emits a line such as:
             * -- dbconfig:multi=sql\Test.sql
             * 
             * The idea is to return the following output filename:
             *     C:\Data\TST\sql\Test.sql     , If hasEnvironments and environment = TST
             *     C:\Data\sql\Test.sql
             * 
             * This method will also *ensure* that the directory does exist on the filesystem.
             * Ie, it will create:
             *     C:\Data\TST\sql              , If hasEnvironments and environment = TST
             *     C:\Data\sql
             */
            string dir = fileInfo.DirectoryName;

            if ( hasEnvironments == true )
                dir = Path.Combine( dir, environment );

            string fullPath = Path.Combine( dir, relativePath );


            /*
             * 
             */
            FileInfo outputInfo = new FileInfo( fullPath );

            Directory.CreateDirectory( outputInfo.DirectoryName );

            return outputInfo.FullName;
        }


        private static List<string> EnvironmentList( bool hasEnvironments, string environment )
        {
            List<string> list = new List<string>();

            if ( hasEnvironments == false )
            {
                list.Add( "na" );
            }
            else if ( environment == null || environment == "all" )
            {
                string[] ce = ConfigurationManager.AppSettings[ "Environment.List" ].Split( ',' );

                foreach ( string c in ce )
                {
                    string cn = c.Trim();

                    if ( string.IsNullOrEmpty( cn ) == true )
                        continue;

                    list.Add( cn );
                }
            }
            else
            {
                list.Add( environment );
            }

            return list;
        }


        private static XmlDocument LoadDocument( string fileName )
        {
            #region Validations

            if ( fileName == null )
                throw new ArgumentNullException( "fileName" );

            #endregion

            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load( fileName );
            }
            catch ( XmlException ex )
            {
                throw new ApplicationException( "failed to load", ex );
            }
            catch ( UnauthorizedAccessException ex )
            {
                throw new ApplicationException( "failed to load", ex );
            }
            catch ( SecurityException ex )
            {
                throw new ApplicationException( "failed to load", ex );
            }
            catch ( DirectoryNotFoundException ex )
            {
                throw new ApplicationException( "failed to load", ex );
            }
            catch ( FileNotFoundException ex )
            {
                throw new ApplicationException( "failed to load", ex );
            }
            catch ( IOException ex )
            {
                throw new ApplicationException( "failed to load", ex );
            }
            catch ( ArgumentException ex )
            {
                throw new ApplicationException( "failed to load", ex );
            }

            return doc;
        }


        private static XslCompiledTransform LoadTransform( string xsltFilename )
        {
            #region Validations

            if ( xsltFilename == null )
                throw new ArgumentNullException( "xsltFilename" );

            #endregion

            XslCompiledTransform xslt = new XslCompiledTransform( false );

            XsltSettings settings = new XsltSettings();
            settings.EnableDocumentFunction = true;
            settings.EnableScript = true;

            XmlUrlResolver resolver = new XmlUrlResolver();

            try
            {
                xslt.Load( xsltFilename, settings, resolver );
            }
            catch ( XsltException ex )
            {
                throw new ApplicationException( "invalid xslt", ex );
            }
            catch ( FileNotFoundException ex )
            {
                throw new ApplicationException( "transformation not found", ex );
            }
            catch ( DirectoryNotFoundException ex )
            {
                throw new ApplicationException( "transformation not found", ex );
            }
            catch ( XmlException ex )
            {
                throw new ApplicationException( "xml exception", ex );
            }

            return xslt;
        }



        /* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
         ~ 
         ~ Console stuff
         ~ 
         ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ */

        /// <summary>
        /// (W)rite (O)ut
        /// </summary>
        private static void WO( string format, params object[] args )
        {
            #region Validations

            if ( format == null )
                throw new ArgumentNullException( "format" );

            #endregion

            string message = string.Format( CultureInfo.InvariantCulture, format, args );
            Console.Write( message );
        }

        /// <summary>
        /// (W)rite (L)ine
        /// </summary>
        private static void WL( string format, params object[] args )
        {
            #region Validations

            if ( format == null )
                throw new ArgumentNullException( "format" );

            #endregion

            string message = string.Format( CultureInfo.InvariantCulture, format, args );
            Console.WriteLine( message );
        }

        /// <summary>
        /// (W)rite (L)ine
        /// </summary>
        private static void WL()
        {
            Console.WriteLine();
        }

        /// <summary>
        /// (W)rite (F)ail
        /// </summary>
        private static void WF()
        {
            Program.ErrorCount++;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine( "fail" );
            Console.ResetColor();
        }

        /// <summary>
        /// (W)rite (Ex)ception
        /// </summary>
        /// <param name="exception"></param>
        private static void WEx( Exception exception )
        {
            #region Validations

            if ( exception == null )
                throw new ArgumentNullException( "exception" );

            #endregion

            Exception ex = exception;

            while ( ex != null )
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine( ex.GetType().FullName );
                Console.WriteLine( string.Concat( "Message: ", ex.Message ) );

                if ( PO.IncludeStackTrace == true )
                    Console.WriteLine( ex.StackTrace );

                Console.WriteLine();
                Console.ResetColor();

                ex = ex.InnerException;
            }
        }
    }
}

/* eof */