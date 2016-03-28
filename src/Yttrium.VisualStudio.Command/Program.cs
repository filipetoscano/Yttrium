using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Yttrium.VisualStudio.Command
{
    public static class Program
    {
        static void Main( string[] args )
        {
            /*
             *
             */
            CommandLine cl = new CommandLine();

            if ( cl.Parse( args ) == false )
                Environment.Exit( 1001 );

            if ( cl.Help == true )
            {
                cl.HelpShow();
                Environment.Exit( 1002 );
            }


            /*
             * .Tool
             */
            List<string> tools = new List<string>();
            tools.Add( "ConfigGenTool" );
            tools.Add( "FileGetTool" );
            tools.Add( "ResxErrorTool" );
            tools.Add( "ResxExceptionTool" );
            tools.Add( "WsdlTool" );
            tools.Add( "XsdTool" );
            tools.Add( "XsltTool" );

            string tool = tools.FirstOrDefault( t => t.ToLowerInvariant() == cl.Tool.ToLowerInvariant() );

            if ( tool == null )
            {
                Console.Error.WriteLine( "error: tool parameter is invalid, tool must be one of: " );
                Console.Error.WriteLine( string.Join( ", ", tools ) );
                Environment.Exit( 1003 );
            }


            /*
             * .File
             */
            FileInfo file = new FileInfo( cl.File );

            if ( file.Exists == false )
            {
                Console.Error.WriteLine( "error: file '{0}' not found.", file.FullName );
                Environment.Exit( 1004 );
            }


            /*
             * .Namespace
             * If the namespace is provided, great. If not, then we need to 
             * crawl up the directory until a .csproj is found and then
             * infer the namespace.
             */
            string ns = cl.Namespace;

            if ( ns == null )
            {
                ns = WalkUp( file.Directory, file.Directory );

                if ( ns == null )
                {
                    Console.Error.WriteLine( "error: failed to find any .csproj file in parent directories." );
                    Environment.Exit( 1005 );
                }
            }

            Console.WriteLine( "file={0}", file.FullName );
            Console.WriteLine( "tool={0}", tool );
            Console.WriteLine( "namespace={0}", ns );

            /*
             * 
             */
            BaseTool bt = (BaseTool) Activator.CreateInstance( "Yttrium.VisualStudio", "Yttrium.VisualStudio." + tool ).Unwrap();

            string content;

            try
            {
                content = bt.Execute( file, ns );
            }
            catch ( Exception ex )
            {
                Console.Error.WriteLine( "error: unhandled exception from tool." );
                Console.Error.WriteLine( ex.GetType().FullName );
                Console.Error.WriteLine( ex.Message );
                Console.Error.WriteLine( ex.StackTrace );
                Environment.Exit( 1006 );
                return;
            }


            /*
             * 
             */
            string outputFile = Path.Combine( file.DirectoryName, Path.GetFileNameWithoutExtension( file.FullName ) + ".cs" );
            File.WriteAllText( outputFile, content, Encoding.UTF8 );
        }



        private static string WalkUp( DirectoryInfo fileDirectory, DirectoryInfo directory )
        {
            #region Validations

            if ( fileDirectory == null )
                throw new ArgumentNullException( "fileDirectory" );

            if ( directory == null )
                throw new ArgumentNullException( "directory" );

            #endregion


            /*
             * 
             */
            FileInfo[] info = directory.GetFiles( "*.csproj" );

            if ( info.Length > 0 )
            {
                XmlNamespaceManager mgr = new XmlNamespaceManager( new NameTable() );
                mgr.AddNamespace( "ns", "http://schemas.microsoft.com/developer/msbuild/2003" );

                XmlDocument doc = new XmlDocument();
                doc.Load( info[ 0 ].FullName );

                XmlElement elem = (XmlElement) doc.SelectSingleNode( " /ns:Project/ns:PropertyGroup/ns:RootNamespace ", mgr );
                string rootNs = elem.InnerText;

                if ( fileDirectory.FullName == directory.FullName )
                    return rootNs;

                return rootNs + "." + fileDirectory.FullName.Substring( directory.FullName.Length + 1 ).Replace( "\\", "." );
            }


            /*
             * Stop criteria / recurse
             */
            if ( directory == directory.Root )
                return null;

            return WalkUp( fileDirectory, directory.Parent );
        }
    }
}

/* eof */
