using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Yttrium.VisualStudio
{
    public partial class WsdlTool
    {
        protected override string DoGenerateCode( string fileContent )
        {
            #region Validation

            if ( fileContent == null )
                throw new ArgumentNullException( "fileContent" );

            #endregion

            /*
             * 
             */
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml( fileContent );
            }
            catch ( XmlException )
            {
                return ErrorEmit( "File is not a valid Xml document" );
            }

            XPathNavigator xpNav = doc.DocumentElement.CreateNavigator();


            /*
             * 
             */
            string tempFile = Path.GetTempFileName();
            FileInfo finfo = new FileInfo( this.FileName );


            /*
             * 
             */
            string ns = this.FileNameSpace;


            /*
             * 
             */
            bool namespaceAdded = false;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat( " /out:\"{0}\" ", tempFile );

            XPathNodeIterator argumentsIter = xpNav.Select( " /services/arguments/* " );

            while ( argumentsIter.MoveNext() )
            {
                string name = argumentsIter.Current.LocalName;
                string value = argumentsIter.Current.InnerXml;
                sb.AppendFormat( " /{0}:{1} ", name, value );

                if ( name == "namespace" || name == "n" )
                    namespaceAdded = true;
            }

            if ( namespaceAdded == false )
            {
                sb.AppendFormat( " /namespace:{0} ", ns );
            }


            XPathNodeIterator flagsIter = xpNav.Select( " /services/flags/* " );

            while ( flagsIter.MoveNext() )
            {
                string name = flagsIter.Current.LocalName;
                sb.AppendFormat( " /{0} ", name );
            }


            XPathNodeIterator serviceIter = xpNav.Select( " /services/list/service/@url " );

            while ( serviceIter.MoveNext() )
            {
                string value = serviceIter.Current.Value;

                sb.AppendFormat( " {0} ", value );
            }


            /*
             * 
             */
            string baseClass = null;

            XPathNodeIterator baseClassIter = xpNav.Select( " /services/baseClass[ 1 ]/@moniker " );

            while ( baseClassIter.MoveNext() )
            {
                baseClass = baseClassIter.Current.Value;
            }


            /*
             * 
             */
            VisualStudioTool tool = VisualStudioSdk.ToolGet( "wsdl.exe" );

            if ( tool.Found == false )
            {
                return ErrorEmit( "wsdl.exe not found in any location", string.Join( "\n", tool.Locations ) );
            }


            /*
             * 
             */
            ProcessStartInfo psinfo = new ProcessStartInfo();
            psinfo.FileName = tool.Path;
            psinfo.LoadUserProfile = false;
            psinfo.CreateNoWindow = true;
            psinfo.RedirectStandardError = true;
            psinfo.UseShellExecute = false;
            psinfo.WorkingDirectory = finfo.DirectoryName;
            psinfo.Arguments = sb.ToString();

            using ( Process p = new Process() )
            {
                p.StartInfo = psinfo;
                p.Start();
                p.WaitForExit();


                /*
                 * 
                 */
                if ( p.ExitCode != 0 )
                {
                    string er = p.StandardError.ReadToEnd();
                    return ErrorEmit( er, "executing wsdl.exe command" );
                }
            }




            /*
             * 
             */
            FileInfo tempFileInfo = new FileInfo( tempFile );

            if ( tempFileInfo.Exists == false )
                return ErrorEmit( "Temporary file not found!" );

            string content = File.ReadAllText( tempFile );


            /*
             * 
             */
            if ( string.IsNullOrEmpty( baseClass ) == false )
            {
                content = content.Replace( "System.Web.Services.Protocols.SoapHttpClientProtocol", baseClass );
            }


            /*
             * The wsdl tool always fully prefixes class names: find/replace some of them.
             */
            XPathNodeIterator shrinkIter = xpNav.Select( " /services/shrink/add[ @find and @replace ] " );

            while ( shrinkIter.MoveNext() )
            {
                XPathNodeIterator findAttr = shrinkIter.Current.Select( " @find " );
                findAttr.MoveNext();

                XPathNodeIterator replAttr = shrinkIter.Current.Select( " @replace " );
                replAttr.MoveNext();

                content = content.Replace( findAttr.Current.Value, replAttr.Current.Value );
            }

            return content;
        }
    }
}

/* eof */