using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Yttrium.Core
{
    public class Command
    {
        /// <summary />
        public static T Parse<T>( string[] args )
        {
            T command;

            TryParse( args, out command );

            return command;
        }


        /// <summary />
        public static bool TryParse<T>( string[] args, out T command )
        {
            command = Activator.CreateInstance<T>();


            /*
             * 
             */
            var @short = new Dictionary<string, PropertyInfo>();
            var @long = new Dictionary<string, PropertyInfo>();

            foreach ( var prop in command.GetType().GetProperties() )
            {
                OptionAttribute option = prop.GetCustomAttribute<OptionAttribute>();

                if ( option != null )
                {
                    if ( option.Short != 0 )
                        @short.Add( option.Short.ToString(), prop );

                    if ( option.Long != null )
                        @long.Add( option.Long, prop );
                }
                else
                { 
                    @long.Add( prop.Name.ToLowerInvariant(), prop );
                }
            }


            /*
             * 
             */
            for ( int i = 0; i < args.Length; i++ )
            {
                var arg = args[ i ];


                /*
                 * Long options:
                 *    --boolean
                 *    --value=VALUE
                 */
                if ( arg.StartsWith( "--" ) == true )
                {
                    string longName = null;
                    string longValue = null;

                    int equal = arg.IndexOf( '=' );

                    if ( equal > -1 )
                    {
                        longName = arg.Substring( 2, equal - 2 );
                        longValue = arg.Substring( equal + 1 );
                    }
                    else
                    {
                        longName = arg.Substring( 2 );
                    }


                    /*
                     * 
                     */
                    if ( @long.ContainsKey( longName ) == false )
                    {
                        Console.Error.WriteLine( "err: unknown option '{0}'.", longName );
                        return false;
                    }

                    var option = @long[ longName ];

                    if ( option.PropertyType == typeof( bool ) )
                    {
                        option.SetValue( command, true );
                    }
                    else
                    {
                        if ( longValue == null )
                        {
                            Console.Error.WriteLine( "err: options '{0}' expects value.", longName );
                            return false;
                        }
                        else if ( option.PropertyType == typeof( string ) )
                        {
                            option.SetValue( command, longValue );
                        }
                        else
                        {
                            object v;
                            v = Convert.ChangeType( longValue, option.PropertyType, CultureInfo.InvariantCulture );
                            option.SetValue( command, v );
                        }
                    }
                }


                /*
                 * Short options:
                 *    -v       (boolean)
                 *    -o VALUE (values)
                 */
                else if ( arg.StartsWith( "-" ) )
                {
                    string shortName = null;
                    string shortValue = null;

                    shortName = arg.Substring( 1 );


                    /*
                     * 
                     */
                    if ( shortName.Length > 1 )
                    {
                        foreach ( char s in shortName )
                        {
                            string so = s.ToString();

                            if ( @short.ContainsKey( so ) == false )
                            {
                                Console.Error.WriteLine( "err: unknown option '{0}'.", shortName );
                                return false;
                            }

                            var option = @short[ so ];

                            if ( option.PropertyType != typeof( bool ) )
                            {
                                Console.Error.WriteLine( "err: option '{0}'.requires parameter", shortName );
                                return false;
                            }

                            option.SetValue( command, true );
                        }
                    }
                    else
                    {
                        if ( @short.ContainsKey( shortName ) == false )
                        {
                            Console.Error.WriteLine( "err: unknown option '{0}'.", shortName );
                            return false;
                        }

                        var option = @short[ shortName ];

                        if ( option.PropertyType == typeof( bool ) )
                        {
                            option.SetValue( command, true );
                        }
                        else
                        {
                            if ( (i + 1) == args.Length )
                            {
                                Console.Error.WriteLine( "err: missing value for option '{0}'.", shortName );
                                return false;
                            }

                            shortValue = args[ ++i ];
                            option.SetValue( command, shortValue );
                        }
                    }
                }
            }

            return true;
        }
    }
}
