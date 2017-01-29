using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;

namespace Yttrium.WebDeploy
{
    /// <summary />
    public class Program
    {
        /// <summary />
        static int Main( string[] args )
        {
            /*
             * 
             */
            if ( args.Length == 0 )
            {
                Console.Error.Write( "err: ywebdeploy requires one command. list of commands: replace" );
                Environment.ExitCode = 400;
                return 400;
            }




            /*
             * 
             */
            int exitCode;

            switch ( args[ 0 ] )
            {
                case "replace":
                    exitCode = ReplaceFile( args.Skip( 1 ).ToArray() );
                    break;

                default:
                    Console.Error.Write( $"err: unsupported command '{ args[ 0 ] }'." );
                    exitCode = 500;
                    break;
            }


            /*
             * 
             */
            Environment.ExitCode = exitCode;
            return exitCode;
        }


        /// <summary />
        static int ReplaceFile( string[] args )
        {
            /*
             *
             */
            var cl = new ReplaceFileCommandLine();

            if ( cl.Parse( args ) == false )
                return 1000;

            if ( cl.Help == true )
            {
                cl.HelpShow();
                return 1001;
            }


            /*
             * 
             */
            string packagePath = cl.Package;
            string replaceFile = cl.Replace;
            string replaceWith = cl.With;


            /*
             * 
             */
            if ( File.Exists( replaceWith ) == false )
            {
                Console.WriteLine( "err: file '{0}' does not exist.", replaceWith );
                return 1000;
            }



            /*
             * 
             */
            using ( ZipArchive package = ZipFile.Open( packagePath, ZipArchiveMode.Update ) )
            {
                /*
                 * 
                 */
                var archive = package.Entries.Where( x => x.FullName == "archive.xml" ).SingleOrDefault();

                if ( archive == null )
                {
                    Console.Error.WriteLine( "err: archive.xml file not found in Zip package." );
                    return 1001;
                }


                /*
                 * 
                 */
                XmlDocument archiveDoc = new XmlDocument();

                using ( var xr = XmlReader.Create( archive.Open() ) )
                {
                    try
                    {
                        archiveDoc.Load( xr );
                    }
                    catch ( Exception )
                    {
                        Console.Error.WriteLine( "err: archive.xml not a valid XML file." );
                        return 1002;
                    }
                }


                /*
                 * 
                 */
                XmlAttribute pathAttr = (XmlAttribute) archiveDoc.SelectSingleNode( " /sitemanifest/iisApp/@path " );

                if ( pathAttr == null )
                {
                    Console.Error.WriteLine( "err: archive.xml is not valid sitemanifest." );
                    return 1003;
                }


                /*
                 * 
                 */
                var filePath = ToDeployFolder( Path.Combine( pathAttr.Value, replaceFile ) );
                var fileEntry = package.Entries.Where( x => x.FullName == filePath ).FirstOrDefault();

                if ( fileEntry != null )
                {
                    Console.Write( $"replacing '{ replaceFile }' file... " );
                    fileEntry.Delete();
                }
                else
                {
                    Console.Write( $"adding '{ replaceFile }' file... " );
                }

                package.CreateEntryFromFile( replaceWith, filePath );
                Console.WriteLine( "done" );

                return 0;
            }
        }


        /// <summary />
        private static string ToDeployFolder( string fsPath )
        {
            #region Validations

            if ( fsPath == null )
                throw new ArgumentNullException( nameof( fsPath ) );

            #endregion

            Uri uri = new Uri( fsPath );
            string path = uri.AbsolutePath;

            if ( path.StartsWith( "C:" ) == true )
                return @"Content/C_C" + path.Substring( 2 );

            throw new NotImplementedException( "NI001" );
        }
    }
}
