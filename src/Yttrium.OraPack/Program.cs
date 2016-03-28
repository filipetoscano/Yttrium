using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Yttrium.OraPack
{
    public class Program
    {
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
                Console.WriteLine( "orapack: no matching files." );
                Environment.Exit( 2 );
            }


            /*
             *
             */
            StringBuilder headerGlobals = new StringBuilder();
            StringBuilder header = new StringBuilder();
            StringBuilder bodyGlobals = new StringBuilder();
            StringBuilder bodyHeader = new StringBuilder();
            StringBuilder body = new StringBuilder();
            StringBuilder footer = new StringBuilder();
            Regex versionHeader = new Regex( @"\$(?<inner>Id: (.*) (?<rev>\d+) (.*)) \$", RegexOptions.Compiled | RegexOptions.Multiline );
            Regex fileNameOverload = new Regex( @"(?<contentName>.*?)_\d+$", RegexOptions.Compiled );

            int latestRevision = 1;
            string latestRevisionInfo = "#undef#";


            /*
             *
             */
            foreach ( string fileName in list )
            {
                Console.Write( Path2.RelativePath( fileName ) );
                Console.Write( ": " );


                /*
                 *
                 */
                FileInfo fileInfo = new FileInfo( fileName );
                string contentName = fileInfo.Name;

                if ( fileInfo.Extension.Length > 0 )
                    contentName = fileInfo.Name.Substring( 0, fileInfo.Name.Length - fileInfo.Extension.Length );

                string fileContent;

                using ( StreamReader sr = fileInfo.OpenText() )
                {
                    fileContent = sr.ReadToEnd();
                }


                /*
                 * Support for overloaded functions/procedures: if the file ends in _DIGIT, then
                 * we know that it is the Nth overload with that name.
                 */
                Match fm = fileNameOverload.Match( contentName );

                if ( fm.Success == true )
                {
                    contentName = fm.Groups[ "contentName" ].Value;
                    Console.Write( "++ " );
                }


                /*
                 *
                 */
                Match m = versionHeader.Match( fileContent );
                String revisionInfo = "#undef#";

                if ( m.Success == true )
                {
                    revisionInfo = m.Groups[ "inner" ].Value;

                    int revision = int.Parse( m.Groups[ "rev" ].Value, CultureInfo.InvariantCulture );

                    if ( revision > latestRevision )
                    {
                        latestRevision = revision;
                        latestRevisionInfo = revisionInfo;
                    }
                }


                /*
                 *
                 */
                FileContent fc;

                if ( fileContent.Contains( "-- [orapack:globals" ) == true )
                {
                    Console.Write( "globals... " );
                    fc = PackGlobals( fileContent, revisionInfo, cl.KeepMarkedComments, cl.Debug );
                }
                else if ( fileContent.Contains( "-- [orapack:grants]" ) == true )
                {
                    Console.Write( "grants... " );
                    fc = PackGrants( fileContent, cl.PackageName, revisionInfo );
                }
                else if ( fileContent.Contains( "procedure " + contentName ) == true )
                {
                    Console.Write( "procedure... " );
                    fc = PackProcedure( fileContent, contentName, revisionInfo, cl.KeepMarkedComments );
                }
                else if ( fileContent.Contains( "function " + contentName ) == true )
                {
                    Console.Write( "function... " );
                    fc = PackFunction( fileContent, contentName, revisionInfo, cl.KeepMarkedComments );
                }
                else
                {
                    Console.WriteLine( "unknown content type. skip!" );
                    continue;
                }

                if ( fc == null )
                {
                    Console.WriteLine( "parse failed. skip!" );
                    continue;
                }


                /*
                 *
                 */
                if ( fc.HeaderGlobals != null )
                    headerGlobals.AppendLine( fc.HeaderGlobals );

                if ( fc.Header != null )
                    header.AppendLine( fc.Header );

                if ( fc.BodyGlobals != null )
                    bodyGlobals.AppendLine( fc.BodyGlobals );

                if ( fc.BodyHeader != null )
                    bodyHeader.AppendLine( fc.BodyHeader );

                if ( fc.Body != null )
                    body.AppendLine( fc.Body );

                if ( fc.Footer != null )
                    footer.AppendLine( fc.Footer );

                Console.WriteLine( "done" );
            }


            /*
             *
             */
            if ( cl.SeparateFiles == true )
            {
                string headerName = string.Concat( ".\\", cl.PackageName, ".pks" );
                string bodyName = string.Concat( ".\\", cl.PackageName, ".pkb" );

                Console.Write( "writing header file... " );

                using ( StreamWriter sw = File.CreateText( headerName ) )
                {
                    sw.Write( FileHeader( cl.PackageName, latestRevisionInfo ) );

                    sw.Write( HeaderStart( cl.PackageName, latestRevisionInfo ) );
                    sw.Write( headerGlobals.ToString() );
                    sw.Write( header.ToString() );
                    sw.Write( HeaderEnd( cl.PackageName ) );

                    if ( footer.Length > 0 )
                    {
                        sw.WriteLine( "" );
                        sw.WriteLine( "" );
                        sw.Write( footer.ToString() );
                    }

                    sw.WriteLine( "" );
                    sw.WriteLine( "/* eof */" );
                }

                Console.WriteLine( "done" );
                Console.Write( "writing body file... " );

                using ( StreamWriter sw = File.CreateText( bodyName ) )
                {
                    sw.Write( FileHeader( cl.PackageName, latestRevisionInfo ) );

                    sw.Write( BodyStart( cl.PackageName ) );
                    sw.Write( bodyGlobals.ToString() );
                    sw.Write( bodyHeader.ToString() );
                    sw.Write( body.ToString() );
                    sw.Write( BodyEnd( cl.PackageName ) );

                    sw.WriteLine( "" );
                    sw.WriteLine( "/* eof */" );
                }

                Console.WriteLine( "done" );
            }
            else
            {
                string fileName = string.Concat( ".\\", cl.PackageName, ".sql" );
                Console.Write( "writing file... " );

                using ( StreamWriter sw = File.CreateText( fileName ) )
                {
                    sw.Write( FileHeader( cl.PackageName, latestRevisionInfo ) );

                    sw.Write( HeaderStart( cl.PackageName, latestRevisionInfo ) );
                    sw.Write( headerGlobals.ToString() );
                    sw.Write( header.ToString() );
                    sw.Write( HeaderEnd( cl.PackageName ) );

                    sw.WriteLine( "" );
                    sw.WriteLine( "" );

                    sw.Write( BodyStart( cl.PackageName ) );
                    sw.Write( bodyGlobals.ToString() );
                    sw.Write( bodyHeader.ToString() );
                    sw.WriteLine( "" );
                    sw.Write( body.ToString() );
                    sw.Write( BodyEnd( cl.PackageName ) );

                    if ( footer.Length > 0 )
                    {
                        sw.WriteLine( "" );
                        sw.WriteLine( "" );
                        sw.Write( footer.ToString() );
                    }

                    sw.WriteLine( "" );
                    sw.WriteLine( "/* eof */" );
                }

                Console.WriteLine( "done" );
            }


            /*
             *
             */
            Environment.Exit( 0 );
        }


        private static string FileHeader( string packageName, string latestRevisionInfo )
        {
            #region Validations

            if ( packageName == null )
                throw new ArgumentNullException( "packageName" );

            if ( latestRevisionInfo == null )
                throw new ArgumentNullException( "latestRevisionInfo" );

            #endregion

            StringBuilder sb = new StringBuilder();
            sb.AppendLine( "/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" );
            sb.AppendLine( "*" );
            sb.AppendLine( "* Package: " + packageName );
            sb.AppendLine( "* [orapack:" + latestRevisionInfo + "]" );
            sb.AppendLine( "*" );
            sb.AppendLine( "* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" );
            sb.AppendLine( "*" );
            sb.AppendLine( "* This file has been automatically generated. Do NOT edit this file directly," );
            sb.AppendLine( "* any changes made WILL be lost!" );
            sb.AppendLine( "*" );
            sb.AppendLine( "*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/" );
            sb.AppendLine( "" );

            return sb.ToString();
        }


        private static string HeaderStart( string packageName, string latestRevisionInfo )
        {
            #region Validations

            if ( packageName == null )
                throw new ArgumentNullException( "packageName" );

            if ( latestRevisionInfo == null )
                throw new ArgumentNullException( "latestRevisionInfo" );

            #endregion

            StringBuilder sb = new StringBuilder();
            sb.AppendLine( "/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" );
            sb.AppendLine( "~" );
            sb.AppendLine( "~ PACKAGE HEADER" );
            sb.AppendLine( "~ Declares the prototypes of all of the methods in the package." );
            sb.AppendLine( "~" );
            sb.AppendLine( "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/" );
            sb.Append( "create or replace package " );
            sb.AppendLine( packageName );
            sb.AppendLine( "as" );
            sb.AppendLine( "    -- Max" + latestRevisionInfo );

            return sb.ToString();
        }


        private static string HeaderEnd( string packageName )
        {
            #region Validations

            if ( packageName == null )
                throw new ArgumentNullException( "packageName" );

            #endregion

            StringBuilder sb = new StringBuilder();

            sb.Append( "end " );
            sb.Append( packageName );
            sb.AppendLine( ";" );
            sb.AppendLine( "/" );

            return sb.ToString();
        }


        private static string BodyStart( string packageName )
        {
            #region Validations

            if ( packageName == null )
                throw new ArgumentNullException( "packageName" );

            #endregion

            StringBuilder sb = new StringBuilder();
            sb.AppendLine( "/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" );
            sb.AppendLine( "~" );
            sb.AppendLine( "~ PACKAGE BODY" );
            sb.AppendLine( "~ Contains the PL/SQL code of all of the stored procedures." );
            sb.AppendLine( "~" );
            sb.AppendLine( "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/" );
            sb.Append( "create or replace package body " );
            sb.AppendLine( packageName );
            sb.AppendLine( "as" );
            sb.AppendLine( "" );

            return sb.ToString();
        }


        private static string BodyEnd( string packageName )
        {
            #region Validations

            if ( packageName == null )
                throw new ArgumentNullException( "packageName" );

            #endregion

            StringBuilder sb = new StringBuilder();

            sb.Append( "end " );
            sb.Append( packageName );
            sb.AppendLine( ";" );
            sb.AppendLine( "/" );

            return sb.ToString();
        }


        private static FileContent PackGlobals( string fileContent, string revisionInfo, bool keepMarkedComments, bool debugBuild )
        {
            #region Validations

            if ( fileContent == null )
                throw new ArgumentNullException( "fileContent" );

            if ( revisionInfo == null )
                throw new ArgumentNullException( "revisionInfo" );

            #endregion

            /*
             *
             */
            StringBuilder globals = new StringBuilder();
            bool isPrivate = false;


            /*
             *
             */
            string[] lines = fileContent.Split( '\n' );
            bool inConditional = false;
            bool skipContent = false;

            foreach ( string line in lines )
            {
                string l = line.Trim();

                if ( l.Length == 0 )
                    continue;

                if ( l.StartsWith( "-- [orapack:private]", StringComparison.Ordinal ) == true )
                {
                    isPrivate = true;
                    continue;
                }

                if ( l.StartsWith( "-- [orapack:globals:private]", StringComparison.Ordinal ) == true )
                {
                    isPrivate = true;
                    continue;
                }

                if ( l.StartsWith( "-- #if ", StringComparison.Ordinal ) == true )
                {
                    inConditional = true;

                    if ( l.EndsWith( "DEBUG", StringComparison.Ordinal ) )
                        skipContent = ( debugBuild == false );
                    if ( l.EndsWith( "RELEASE", StringComparison.Ordinal ) )
                        skipContent = ( debugBuild == true );

                    continue;
                }

                if ( l.StartsWith( "-- #else", StringComparison.Ordinal ) == true )
                {
                    skipContent = !skipContent;
                    continue;
                }

                if ( l.StartsWith( "-- #endif", StringComparison.Ordinal ) == true )
                {
                    inConditional = false;
                    skipContent = false;
                    continue;
                }

                if ( skipContent == true )
                    continue;

                if ( inConditional == true && skipContent == false && l.StartsWith( "--", StringComparison.Ordinal ) == true )
                {
                    globals.Append( PackLine( line.Substring( 3 ), keepMarkedComments ) );
                    globals.AppendLine( "" );
                    continue;
                }

                if ( l.StartsWith( "--", StringComparison.Ordinal ) == true )
                    continue;

                if ( l == "/* eof */" )
                    continue;

                globals.Append( PackLine( line, keepMarkedComments ) );
                globals.AppendLine( "" );
            }


            /*
             *
             */
            FileContent fc = new FileContent();

            if ( isPrivate == true )
                fc.BodyGlobals = globals.ToString();
            else
                fc.HeaderGlobals = globals.ToString();

            return fc;
        }


        private static FileContent PackGrants( string fileContent, string packageName, string revisionInfo )
        {
            #region Validations

            if ( fileContent == null )
                throw new ArgumentNullException( "fileContent" );

            if ( packageName == null )
                throw new ArgumentNullException( "packageName" );

            if ( revisionInfo == null )
                throw new ArgumentNullException( "revisionInfo" );

            #endregion


            /*
             *
             */
            StringBuilder footer = new StringBuilder();
            footer.AppendLine( "-- " + revisionInfo );


            /*
             *
             */
            string[] lines = fileContent.Split( '\n' );

            foreach ( string line in lines )
            {
                string l = line.TrimEnd();

                if ( l.Length == 0 )
                    continue;

                if ( l.StartsWith( "--", StringComparison.Ordinal ) == true )
                    continue;

                if ( l == "/* eof */" )
                    continue;

                footer.AppendLine( line.TrimEnd().Replace( "$package", packageName ) );
            }

            footer.AppendLine( "/" );


            /*
             *
             */
            FileContent fc = new FileContent();
            fc.Footer = footer.ToString();

            return fc;
        }


        private static FileContent PackProcedure( string fileContent, string contentName, string revisionInfo, bool keepMarkedComments )
        {
            #region Validations

            if ( fileContent == null )
                throw new ArgumentNullException( "fileContent" );

            if ( contentName == null )
                throw new ArgumentNullException( "contentName" );

            if ( revisionInfo == null )
                throw new ArgumentNullException( "revisionInfo" );

            #endregion

            /*
             *
             */
            StringBuilder header = new StringBuilder();
            StringBuilder body = new StringBuilder();
            bool isPrivate = false;
            string indent = "    ";


            /*
             *
             */
            string[] lines = fileContent.Split( '\n' );
            string state = "NotStarted";

            foreach ( string line in lines )
            {
                string l = line.TrimEnd();

                switch ( state )
                {
                    case "NotStarted":
                        {
                            if ( l.StartsWith( "-- [orapack:private]", StringComparison.Ordinal ) == true )
                            {
                                isPrivate = true;
                                continue;
                            }

                            if ( l.StartsWith( "--", StringComparison.Ordinal ) == true )
                                continue;

                            if ( l.StartsWith( "create or replace procedure ", StringComparison.Ordinal ) == true
                                || l.StartsWith( "create procedure ", StringComparison.Ordinal ) == true
                                || l.StartsWith( "procedure ", StringComparison.Ordinal ) == true )
                            {
                                int ix = l.IndexOf( "procedure ", StringComparison.Ordinal );
                                string nl = l.Substring( ix );

                                body.Append( indent );
                                body.Append( "-- " );
                                body.Append( contentName );
                                body.Append( " - #" );
                                body.Append( revisionInfo );
                                body.AppendLine( "#" );

                                body.Append( indent );
                                body.AppendLine( nl );

                                header.Append( indent );
                                header.Append( nl );

                                state = "Preamble";
                            }
                        }
                        break;

                    case "Preamble":
                        {
                            body.Append( indent );
                            body.AppendLine( l );

                            if ( l.StartsWith( "as", StringComparison.Ordinal ) == true || l.StartsWith( "is", StringComparison.Ordinal ) )
                            {
                                state = "Body";
                                header.Append( ";" );
                            }
                            else
                            {
                                if ( l.StartsWith( "(", StringComparison.Ordinal ) == false )
                                    header.Append( " " );
                                header.Append( l.TrimStart() );
                            }
                        }
                        break;

                    case "Body":
                        {
                            body.AppendLine( PackLine( l, keepMarkedComments ) );

                            if ( l.StartsWith( string.Concat( "end ", contentName, ";" ), StringComparison.Ordinal ) == true )
                            {
                                state = "End";
                            }
                        }
                        break;
                }
            }


            /*
             *
             */
            FileContent fc = new FileContent();

            if ( isPrivate == true )
            {
                fc.Header = null;
                fc.BodyHeader = WrapHeaderLine( header.ToString() );
            }
            else
            {
                fc.Header = WrapHeaderLine( header.ToString() );
                fc.BodyHeader = null;
            }

            fc.Body = body.ToString();

            return fc;
        }


        private static FileContent PackFunction( string fileContent, string contentName, string revisionInfo, bool keepMarkedComments )
        {
            #region Validations

            if ( fileContent == null )
                throw new ArgumentNullException( "fileContent" );

            if ( contentName == null )
                throw new ArgumentNullException( "contentName" );

            if ( revisionInfo == null )
                throw new ArgumentNullException( "revisionInfo" );

            #endregion

            /*
             *
             */
            StringBuilder header = new StringBuilder();
            StringBuilder body = new StringBuilder();
            bool isPrivate = false;
            string indent = "    ";


            /*
             *
             */
            string[] lines = fileContent.Split( '\n' );
            string state = "NotStarted";

            foreach ( string line in lines )
            {
                string l = line.TrimEnd();

                switch ( state )
                {
                    case "NotStarted":
                        {
                            if ( l.StartsWith( "-- [orapack:private]", StringComparison.Ordinal ) == true )
                            {
                                isPrivate = true;
                                continue;
                            }

                            if ( l.StartsWith( "--", StringComparison.Ordinal ) == true )
                                continue;

                            if ( l.StartsWith( "create or replace function ", StringComparison.Ordinal ) == true
                                || l.StartsWith( "create function ", StringComparison.Ordinal ) == true
                                || l.StartsWith( "function ", StringComparison.Ordinal ) == true )
                            {
                                int ix = l.IndexOf( "function ", StringComparison.Ordinal );
                                string nl = l.Substring( ix );

                                body.Append( indent );
                                body.Append( "-- " );
                                body.Append( contentName );
                                body.Append( " - #" );
                                body.Append( revisionInfo );
                                body.AppendLine( "#" );

                                body.Append( indent );
                                body.AppendLine( nl );

                                header.Append( indent );
                                header.Append( nl );

                                state = "Preamble";
                            }
                        }
                        break;

                    case "Preamble":
                        {
                            body.Append( indent );
                            body.AppendLine( l );

                            if ( l.StartsWith( "is", StringComparison.Ordinal ) == true )
                            {
                                state = "Body";
                                header.Append( ";" );
                            }
                            else
                            {
                                if ( l.StartsWith( "(", StringComparison.Ordinal ) == false )
                                    header.Append( " " );
                                header.Append( l.TrimStart() );
                            }
                        }
                        break;

                    case "Body":
                        {
                            body.AppendLine( PackLine( l, keepMarkedComments ) );

                            if ( l.StartsWith( string.Concat( "end ", contentName, ";" ), StringComparison.Ordinal ) == true )
                            {
                                state = "End";
                            }
                        }
                        break;
                }
            }


            /*
             *
             */
            FileContent fc = new FileContent();

            if ( isPrivate == true )
            {
                fc.Header = null;
                fc.BodyHeader = WrapHeaderLine( header.ToString() );
            }
            else
            {
                fc.Header = WrapHeaderLine( header.ToString() );
                fc.BodyHeader = null;
            }

            fc.Body = body.ToString();

            return fc;
        }


        private static string PackLine( string line, bool keepMarkedComments )
        {
            if ( line == null )
                return null;

            int marker = line.IndexOf( "--!", StringComparison.Ordinal );

            if ( keepMarkedComments == false && marker > -1 )
                line = line.Substring( 0, marker ).TrimEnd();
            else
                line = line.TrimEnd();

            if ( line.Length > 0 )
                line = string.Concat( "    ", line );

            return line;
        }


        /// <summary>
        /// Formats the header line to ensure that we don't run into any SQL+
        /// execution errors.
        /// </summary>
        /// <param name="line">Single line.</param>
        /// <returns>Possibly multi-line.</returns>
        private static string WrapHeaderLine( string line )
        {
            #region Validations

            if ( line == null )
                throw new ArgumentNullException( "line" );

            #endregion

            int maxLength = 200;

            if ( line.Length < maxLength )
                return line;


            /*
             * Since the package header line is being automatically generated by
             * orapack, we need to ensure that the 'strip all enters and append'
             * approach doesn't generate lines which exceed the limits which
             * SQL*Plus can handle (2500).
             *
             * But let's face it: 2500 is a ridiculously high number anyway. :P
             * Let's settle on a number which makes the header remotely readable
             * for the human developer.
             */
            StringBuilder sb = new StringBuilder();

            while ( true )
            {
                if ( line.Length < maxLength )
                {
                    sb.Append( line );
                    break;
                }

                string b1 = line.Substring( 0, maxLength );
                string a1 = line.Substring( maxLength );

                int ix = b1.LastIndexOf( ", ", StringComparison.Ordinal );

                string b2 = b1.Substring( 0, ix + 1 );
                string a2 = b1.Substring( ix + 2 ) + a1;

                sb.Append( b2 );
                sb.AppendLine();
                sb.Append( "        " );

                line = a2;
            }

            return sb.ToString();
        }
    }
}

/* eof */