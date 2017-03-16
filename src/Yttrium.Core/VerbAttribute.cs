using System;

namespace Yttrium.Core
{
    /// <summary>
    /// Marks a property as being a command-line verb.
    /// </summary>
    /// <remarks>
    /// Whenever a class has a single verb, all properties must be verbs.
    /// </remarks>
    [AttributeUsage( AttributeTargets.Property )]
    public class VerbAttribute : Attribute
    {
        /// <summary>
        /// Gets/sets the name of the verb explicitly.
        /// </summary>
        /// <remarks>
        /// If not defined, the name of the verb is derived from the name
        /// of the CLR property.
        /// </remarks>
        public string Name
        {
            get;
            set;
        }


        /// <summary>
        /// Gets the user-description of what the mode does.
        /// </summary>
        public string Description
        {
            get;
            set;
        }
    }
}
