//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.6.1055.0.
// 
namespace Yttrium.Scaffold.Description {
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [GeneratedCode("xsd", "4.6.1055.0")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType=true, Namespace="urn:yttrium/scaffold")]
    [XmlRoot(Namespace="urn:yttrium/scaffold", IsNullable=false)]
    public partial class config {
        
        private placeholder[] placeholdersField;
        
        private variable[] variablesField;
        
        private configValues valuesField;
        
        /// <remarks/>
        [XmlArrayItem("add", IsNullable=false)]
        public placeholder[] placeholders {
            get {
                return this.placeholdersField;
            }
            set {
                this.placeholdersField = value;
            }
        }
        
        /// <remarks/>
        [XmlArrayItem("add", IsNullable=false)]
        public variable[] variables {
            get {
                return this.variablesField;
            }
            set {
                this.variablesField = value;
            }
        }
        
        /// <remarks/>
        public configValues values {
            get {
                return this.valuesField;
            }
            set {
                this.valuesField = value;
            }
        }
    }
    
    /// <remarks/>
    [GeneratedCode("xsd", "4.6.1055.0")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:yttrium/scaffold")]
    public partial class placeholder {
        
        private string formatField;
        
        /// <remarks/>
        [XmlAttribute()]
        public string format {
            get {
                return this.formatField;
            }
            set {
                this.formatField = value;
            }
        }
    }
    
    /// <remarks/>
    [GeneratedCode("xsd", "4.6.1055.0")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:yttrium/scaffold")]
    public partial class guidValue {
        
        private string nameField;
        
        private string findField;
        
        /// <remarks/>
        [XmlAttribute()]
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string find {
            get {
                return this.findField;
            }
            set {
                this.findField = value;
            }
        }
    }
    
    /// <remarks/>
    [GeneratedCode("xsd", "4.6.1055.0")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:yttrium/scaffold")]
    public partial class dateValue {
        
        private string nameField;
        
        /// <remarks/>
        [XmlAttribute()]
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
    }
    
    /// <remarks/>
    [GeneratedCode("xsd", "4.6.1055.0")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace="urn:yttrium/scaffold")]
    public partial class variable {
        
        private string nameField;
        
        private string textField;
        
        private bool requiredField;
        
        private string defaultField;
        
        public variable() {
            this.requiredField = true;
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string text {
            get {
                return this.textField;
            }
            set {
                this.textField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool required {
            get {
                return this.requiredField;
            }
            set {
                this.requiredField = value;
            }
        }
        
        /// <remarks/>
        [XmlAttribute()]
        public string @default {
            get {
                return this.defaultField;
            }
            set {
                this.defaultField = value;
            }
        }
    }
    
    /// <remarks/>
    [GeneratedCode("xsd", "4.6.1055.0")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType=true, Namespace="urn:yttrium/scaffold")]
    public partial class configValues {
        
        private dateValue[] dateField;
        
        private guidValue[] guidField;
        
        /// <remarks/>
        [XmlElement("date")]
        public dateValue[] date {
            get {
                return this.dateField;
            }
            set {
                this.dateField = value;
            }
        }
        
        /// <remarks/>
        [XmlElement("guid")]
        public guidValue[] guid {
            get {
                return this.guidField;
            }
            set {
                this.guidField = value;
            }
        }
    }
}
