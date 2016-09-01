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
             *
             */
            Type baseType = typeof( BaseTool );

            var rs = from a in AppDomain.CurrentDomain.GetAssemblies()
                     from t in a.GetTypes()
                     where t.IsAbstract == false
                     where baseType.IsAssignableFrom( t ) == true
                     select t;

            Dictionary<string, Type> tools = new Dictionary<string, Type>();

            foreach ( var t in rs )
                tools.Add( "Y" + t.Name, t );


            /*
             *
             */
            if ( cl.Project != null )
                RunProject( cl, tools );

            if ( cl.File != null )
                RunFile( cl, tools );
        }


        /// <summary>
        /// Runs all of the extension custom tools for the specified project.
        /// </summary>
        /// <param name="cl">Command-line.</param>
        /// <param name="tools">List of custom tools.</param>
        private static void RunProject( CommandLine cl, Dictionary<string, Type> tools )
        {
            #region Validations

            if ( cl == null )
                throw new ArgumentNullException( nameof( cl ) );

            if ( tools == null )
                throw new ArgumentNullException( nameof( tools ) );

            #endregion


            /*
             *
             */
            XmlNamespaceManager manager = new XmlNamespaceManager( new NameTable() );
            manager.AddNamespace( "ns", "http://schemas.microsoft.com/developer/msbuild/2003" );

            XmlDocument csproj = new XmlDocument();
            csproj.Load( cl.Project );

            XmlElement elem = (XmlElement) csproj.SelectSingleNode( " /ns:Project/ns:PropertyGroup/ns:RootNamespace ", manager );
            string rootNs = elem.InnerText;

            foreach ( XmlElement contentElem in csproj.SelectNodes( @" //ns:Content[ @Include and ns:Generator ] | " +
                                                                     " //ns:None[ @Include and ns:Generator ] ", manager ) )
            {
                string relativePath = contentElem.Attributes[ "Include" ].Value;
                string tool = contentElem.SelectSingleNode( " ns:Generator ", manager ).InnerText;

                if ( tools.ContainsKey( tool ) == false )
                    continue;

                string fullName = Path.Combine(
                    Path.GetDirectoryName( cl.Project ),
                    relativePath );
                FileInfo file = new FileInfo( fullName );

                string ns = ToNamespace( rootNs, relativePath );

                Console.WriteLine( "f={0} ns={1} t={2}", relativePath, ns, tool );

                if ( cl.DryRun == false )
                    RunTool( tools, tool, file, ns );
            }
        }


        private static string ToNamespace( string rootNs, string relativePath )
        {
            int ix = relativePath.LastIndexOf( "\\", StringComparison.Ordinal );

            if ( ix == -1 )
                return rootNs;

            return string.Concat( rootNs, ".", relativePath.Substring( 0, ix ).Replace( "\\", "." ) );
        }


        /// <summary>
        /// Runs the custom tool for the specified file.
        /// </summary>
        /// <param name="cl">Command-line.</param>
        /// <param name="tools">List of custom tools.</param>
        public static void RunFile( CommandLine cl, Dictionary<string, Type> tools )
        {
            #region Validations

            if ( cl == null )
                throw new ArgumentNullException( nameof( cl ) );

            if ( tools == null )
                throw new ArgumentNullException( nameof( tools ) );

            #endregion


            /*
             * .Tool
             */
            var rs = tools.Where( t => t.Key.ToLowerInvariant() == cl.Tool.ToLowerInvariant() );

            if ( rs.Count() == 0 )
            {
                Console.Error.WriteLine( "error: tool parameter is invalid, tool must be one of: " );
                Console.Error.WriteLine( string.Join( ", ", tools ) );
                Environment.Exit( 1003 );
            }

            string tool = rs.First().Key;


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
            if ( cl.DryRun == false )
                RunTool( tools, tool, file, ns );
        }


        private static void RunTool( Dictionary<string, Type> tools, string tool, FileInfo file, string ns )
        {
            #region Validations

            if ( tools == null )
                throw new ArgumentNullException( nameof( tools ) );

            if ( tool == null )
                throw new ArgumentNullException( nameof( tool ) );

            if ( file == null )
                throw new ArgumentNullException( nameof( file ) );

            #endregion


            /*
             * 
             */
            Type toolType = tools[ tool ];
            BaseTool bt = (BaseTool) Activator.CreateInstance( toolType );

            string content;

            try
            {
                content = bt.Execute( file, ns );
            }
            catch ( Exception ex )
            {
                Console.Error.WriteLine( "error: unhandled exception from tool." );
                Console.Error.WriteLine( ex.ToString() );
                Environment.Exit( 1006 );
                return;
            }


            /*
             * 
             */
            string outputFile = Path.Combine( file.DirectoryName, Path.GetFileNameWithoutExtension( file.FullName ) + ".cs" );
            File.WriteAllText( outputFile, content, new UTF8Encoding( false ) );
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
