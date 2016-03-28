using Microsoft.Win32;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace CustomToolGenerator
{
    public sealed class Registration
    {
        [SuppressMessage( "Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible" )]
        public static Guid CSharpCategoryGuid = new Guid( "{FAE04EC1-301F-11D3-BF4B-00C04F79EFBC}" );


        [SuppressMessage( "Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters" )]
        public static void RegisterCustomTool( string toolName, Guid category, Type generatorType, string description, Version vsVersion )
        {
            if ( generatorType == null )
                return;

            // Assume GUID is associated with this class via the GuidAttribute.
            // Since this is required for COM interop to work with VS.NET, this seems reasonable
            string generatorGuid = ( (GuidAttribute) ( generatorType.GetCustomAttributes( typeof( GuidAttribute ), true )[ 0 ] ) ).Value;


            /*
             * As per:
             * http://msdn.microsoft.com/en-us/library/bb166527.aspx
             */
            using ( RegistryKey key = Registry.LocalMachine.CreateSubKey( ToolHiveName( generatorGuid, vsVersion ) ) )
            {
                key.SetValue( "@", "COM+ class: " + generatorType.FullName );
                key.SetValue( "InprocServer32", "C:\\WINDOWS\\system32\\mscoree.dll" );
                key.SetValue( "ThreadingModel", "Both" );
                key.SetValue( "Class", generatorType.FullName );
                key.SetValue( "Assembly", generatorType.AssemblyQualifiedName );
            }

            using ( RegistryKey key = Registry.LocalMachine.CreateSubKey( LanguageHiveName( toolName, category, vsVersion ) ) )
            {
                key.SetValue( "@", description );
                key.SetValue( "CLSID", "{" + generatorGuid + "}" );
                key.SetValue( "GeneratesDesignTimeSource", 1 );
            }
        }


        private static string ToolHiveName( string generatorGuid, Version vsVersion )
        {
            string ver = vsVersion.ToString();
            return @"SOFTWARE\Microsoft\VisualStudio\" + ver + @"\CLSID\{" + generatorGuid + @"}";
        }


        private static string LanguageHiveName( string toolName, Guid categoryGuid, Version vsVersion )
        {
            string name = toolName;
            string ver = vsVersion.ToString();
            return @"SOFTWARE\Microsoft\VisualStudio\" + ver + @"\Generators\{" + categoryGuid.ToString() + @"}\" + name;
        }



        [SuppressMessage( "Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters" )]
        public static void RegisterCustomTool( string toolName, Guid category, Type generatorType, string description )
        {
            #region Validations

            if ( toolName == null )
                throw new ArgumentNullException( "toolName" );

            if ( generatorType == null )
                throw new ArgumentNullException( "generatorType" );

            if ( description == null )
                throw new ArgumentNullException( "description" );

            #endregion

            Registration.RegisterCustomTool( toolName, category, generatorType, description, new Version( 12, 0 ) );
            Registration.RegisterCustomTool( toolName, category, generatorType, description, new Version( 13, 0 ) );
            Registration.RegisterCustomTool( toolName, category, generatorType, description, new Version( 14, 0 ) );
        }


        [SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "generatorType" )]
        public static void UnregisterCustomTool( string toolName, Guid category, Type generatorType )
        {
            #region Validations

            if ( toolName == null )
                throw new ArgumentNullException( "toolName" );

            if ( generatorType == null )
                throw new ArgumentNullException( "generatorType" );

            #endregion

            Registration.UnregisterCustomTool( toolName, category, generatorType, new Version( 12, 0 ) );
            Registration.UnregisterCustomTool( toolName, category, generatorType, new Version( 13, 0 ) );
            Registration.UnregisterCustomTool( toolName, category, generatorType, new Version( 14, 0 ) );
        }


        [SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "generatorType" )]
        public static void UnregisterCustomTool( string toolName, Guid category, Type generatorType, Version vsVersion )
        {
            Registry.LocalMachine.DeleteSubKey( LanguageHiveName( toolName, category, vsVersion ), false );
        }


        private Registration()
        {
        }
    }
}

/* eof */