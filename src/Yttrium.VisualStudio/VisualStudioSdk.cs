using System;
using System.IO;

namespace Yttrium.VisualStudio
{
    public static class VisualStudioSdk
    {
        public static VisualStudioTool ToolGet( string toolName )
        {
            #region Validations

            if ( toolName == null )
                throw new ArgumentNullException( "toolName" );

            #endregion


            /*
             * Finding out where Visual Studio places 
             */
            string basePath = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools";


            /*
             * 
             */
            string toolPath = Path.Combine( basePath, toolName );
            FileInfo finfo = new FileInfo( toolPath );

            if ( finfo.Exists == false )
            {
                return new VisualStudioTool()
                {
                    Found = false,
                    Locations = new string[] { finfo.FullName }
                };
            }

            return new VisualStudioTool()
            {
                Found = true,
                Path = finfo.FullName,
                Locations = new string[] { finfo.FullName }
            };
        }
    }
}

/* eof */