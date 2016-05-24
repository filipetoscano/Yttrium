using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace Yttrium.FsManifest
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
            XDocument doc = new XDocument();
            doc.Add( new XElement( "fsmanifest" ) );

            Walk( doc.Root, cl.InputDirectory );


            /*
             * 
             */
            if ( cl.WriteToFile == true )
            {
                doc.Save( cl.OutputFileName );
            }
            else
            {
                doc.Save( Console.Out );
            }
        }


        private static void Walk( XElement parent, string currentDirectory )
        {
            #region Validations

            if ( parent == null )
                throw new ArgumentNullException( nameof( parent ) );

            if ( currentDirectory == null )
                throw new ArgumentNullException( nameof( currentDirectory ) );

            #endregion

            DirectoryInfo dir = new DirectoryInfo( currentDirectory );

            foreach ( var sdir in dir.GetDirectories().OrderBy( x => x.Name ) )
            {
                if ( sdir.Name == ".git" )
                    continue;

                if ( sdir.Name == ".svn" )
                    continue;

                XElement el = new XElement( "dir", new XAttribute( "name", sdir.Name ) );
                parent.Add( el );

                Walk( el, sdir.FullName );
            }

            foreach ( var file in dir.GetFiles().OrderBy( x => x.Name ) )
            {
                AddFile( parent, file );
            }
        }


        private static void AddFile( XElement parent, FileInfo file )
        {
            #region Validations

            if ( parent == null )
                throw new ArgumentNullException( nameof( parent ) );

            if ( file == null )
                throw new ArgumentNullException( nameof( file ) );

            #endregion

            /*
             * 
             */
            MemoryStream ms = new MemoryStream();

            using ( Stream s = file.OpenRead() )
            {
                s.CopyTo( ms );
            }

            XElement el = new XElement( "file" );
            el.Add( new XAttribute( "md5", ToHash( ms ) ) );
            el.Add( new XAttribute( "name", file.Name ) );


            /*
             * Does the current file have a version? Use Win32 API to get it.
             */
            if ( HasVersion( file.Extension ) == true )
            {
                FileVersionInfo version = FileVersionInfo.GetVersionInfo( file.FullName );
                el.Add( new XAttribute( "version", version.FileVersion ) );
            }


            /*
             * 
             */
            if ( IsImage( file.Extension ) == true )
                AddImageAttributes( el, ms );


            /*
             * Special handling of ZIP archives
             */
            if ( IsArchive( file.Extension ) == true )
                AddArchiveContent( el, file );

            parent.Add( el );
        }


        private static bool HasVersion( string extension )
        {
            #region Validations

            if ( extension == null )
                throw new ArgumentNullException( nameof( extension ) );

            #endregion

            var e = extension.ToLowerInvariant();
            return e == ".exe" || e == ".dll";
        }


        private static bool IsImage( string extension )
        {
            #region Validations

            if ( extension == null )
                throw new ArgumentNullException( nameof( extension ) );

            #endregion

            var e = extension.ToLowerInvariant();
            return e == ".jpg" || e == ".gif" || e == ".png";
        }


        private static bool IsArchive( string extension )
        {
            #region Validations

            if ( extension == null )
                throw new ArgumentNullException( nameof( extension ) );

            #endregion

            var e = extension.ToLowerInvariant();
            return e == ".zip" || e == ".jar" || e == ".ear";
        }


        private static void AddImageAttributes( XElement file, MemoryStream stream )
        {
            #region Validations

            if ( file == null )
                throw new ArgumentNullException( nameof( file ) );

            if ( stream == null )
                throw new ArgumentNullException( nameof( stream ) );

            #endregion

            try
            {
                stream.Position = 0;
                Image img = Image.FromStream( stream );

                file.Add( new XAttribute( "w", img.Size.Width ) );
                file.Add( new XAttribute( "h", img.Size.Height ) );
            }
            catch
            {
            }
        }


        private static void AddArchiveContent( XElement parent, FileInfo file )
        {
            #region Validations

            if ( parent == null )
                throw new ArgumentNullException( nameof( parent ) );

            if ( file == null )
                throw new ArgumentNullException( nameof( file ) );

            #endregion

            using ( ZipArchive archive = ZipFile.OpenRead( file.FullName ) )
            {
                foreach ( ZipArchiveEntry entry in archive.Entries )
                {
                    if ( entry.FullName.EndsWith( "/" ) == true )
                        continue;

                    MemoryStream ms = new MemoryStream();

                    using ( Stream s = entry.Open() )
                    {
                        s.CopyTo( ms );
                    }


                    /*
                     * 
                     */
                    var el = new XElement( "file" );
                    el.Add( new XAttribute( "md5", ToHash( ms ) ) );
                    el.Add( new XAttribute( "name", entry.FullName ) );

                    if ( IsImage( Path.GetExtension( entry.FullName ) ) == true )
                        AddImageAttributes( el, ms );

                    parent.Add( el );
                }
            }
        }


        private static string ToHash( MemoryStream stream )
        {
            #region Validations

            if ( stream == null )
                throw new ArgumentNullException( nameof( stream ) );

            #endregion

            stream.Position = 0;

            using ( var md5 = MD5.Create() )
            {
                return BitConverter.ToString( md5.ComputeHash( stream ) ).Replace( "-", "" ).ToLower();
            }
        }
    }
}
