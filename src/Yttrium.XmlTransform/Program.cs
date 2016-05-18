﻿using Microsoft.Web.XmlTransform;
using System;
using System.IO;
using System.Xml;

namespace Yttrium.XmlTransform
{
    class Program
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
             * Validations
             */
            if ( File.Exists( cl.Input ) == false )
            {
                Console.Error.WriteLine( "error: input file does not exist." );
                Environment.Exit( 1003 );
                return;
            }

            if ( File.Exists( cl.Transform ) == false )
            {
                Console.Error.WriteLine( "error: transform file does not exist." );
                Environment.Exit( 1004 );
                return;
            }


            /*
             * 
             */
            XmlNamespaceManager mgr = new XmlNamespaceManager( new NameTable() );
            mgr.AddNamespace( "xdt", "http://schemas.microsoft.com/XML-Document-Transform" );
            mgr.AddNamespace( "xdc", "urn:xt:config" );

            XmlDocument xdt = new XmlDocument();
            xdt.Load( cl.Transform );


            /*
             * 
             */
            MemoryStream ms = new MemoryStream();
            xdt.Save( ms );

            ms.Flush();
            ms.Position = 0;


            /*
             *
             */
            XmlDocument doc = new XmlDocument();
            doc.Load( cl.Input );


            /*
             * Remove documents from in-memory representation of the input file.
             */
            if ( cl.RemoveComments == true )
            {
                while ( true )
                {
                    XmlComment c = (XmlComment) doc.SelectSingleNode( "//comment() " );

                    if ( c == null )
                        break;

                    c.ParentNode.RemoveChild( c );
                }
            }


            /*
             * Mark file as having been generated by tool.
             */
            if ( true )
            {
                XmlComment comment = doc.CreateComment( " autogenerated " );
                doc.DocumentElement.ParentNode.InsertBefore( comment, doc.DocumentElement );
            }


            /*
             * Apply the XDT transformation to the input file, modifying it.
             *
             * Reference of support XDT transformations:
             * https://msdn.microsoft.com/en-us/library/dd465326.aspx
             */
            XmlTransformation t = new XmlTransformation( ms, null );
            t.Apply( doc );


            /*
             * Write the output XML file.
             */
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.IndentChars = "    ";

            using ( XmlWriter xw = XmlWriter.Create( cl.Output, xws ) )
            {
                doc.Save( xw );
            }
        }
    }
}