﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version: 1.1.4322.573
//
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=1.1.4322.573.
// 
namespace Opc.Cpx {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://opcfoundation.org/OPCBinary/1.0/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://opcfoundation.org/OPCBinary/1.0/", IsNullable=false)]
    public class TypeDictionary {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("TypeDescription")]
        public TypeDescription[] TypeDescription;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DictionaryName;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool DefaultBigEndian = true;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("UCS-2")]
        public string DefaultStringEncoding = "UCS-2";
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(typeof(System.UInt32), "2")]
        public System.UInt32 DefaultCharWidth = ((System.UInt32)(2));
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("IEEE-754")]
        public string DefaultFloatFormat = "IEEE-754";
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://opcfoundation.org/OPCBinary/1.0/")]
    public class TypeDescription {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Field")]
        public FieldType[] Field;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TypeID;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool DefaultBigEndian;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DefaultBigEndianSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DefaultStringEncoding;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.UInt32 DefaultCharWidth;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DefaultCharWidthSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DefaultFloatFormat;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://opcfoundation.org/OPCBinary/1.0/")]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(TypeReference))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(CharString))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Unicode))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Ascii))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(FloatingPoint))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Double))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Single))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Integer))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(UInt64))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(UInt32))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(UInt16))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(UInt8))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Int64))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Int32))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Int16))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Int8))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(BitString))]
    public class FieldType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Format;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.UInt32 Length;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool LengthSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.UInt32 ElementCount;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ElementCountSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ElementCountRef;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string FieldTerminator;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://opcfoundation.org/OPCBinary/1.0/")]
    public class TypeReference : FieldType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TypeID;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://opcfoundation.org/OPCBinary/1.0/")]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Unicode))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Ascii))]
    public class CharString : FieldType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.UInt32 CharWidth;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool CharWidthSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string StringEncoding;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CharCountRef;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://opcfoundation.org/OPCBinary/1.0/")]
    public class Unicode : CharString {
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://opcfoundation.org/OPCBinary/1.0/")]
    public class Ascii : CharString {
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://opcfoundation.org/OPCBinary/1.0/")]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Double))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Single))]
    public class FloatingPoint : FieldType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string FloatFormat;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://opcfoundation.org/OPCBinary/1.0/")]
    public class Double : FloatingPoint {
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://opcfoundation.org/OPCBinary/1.0/")]
    public class Single : FloatingPoint {
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://opcfoundation.org/OPCBinary/1.0/")]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(UInt64))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(UInt32))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(UInt16))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(UInt8))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Int64))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Int32))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Int16))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Int8))]
    public class Integer : FieldType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool Signed = true;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://opcfoundation.org/OPCBinary/1.0/")]
    public class UInt64 : Integer {
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://opcfoundation.org/OPCBinary/1.0/")]
    public class UInt32 : Integer {
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://opcfoundation.org/OPCBinary/1.0/")]
    public class UInt16 : Integer {
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://opcfoundation.org/OPCBinary/1.0/")]
    public class UInt8 : Integer {
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://opcfoundation.org/OPCBinary/1.0/")]
    public class Int64 : Integer {
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://opcfoundation.org/OPCBinary/1.0/")]
    public class Int32 : Integer {
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://opcfoundation.org/OPCBinary/1.0/")]
    public class Int16 : Integer {
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://opcfoundation.org/OPCBinary/1.0/")]
    public class Int8 : Integer {
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://opcfoundation.org/OPCBinary/1.0/")]
    public class BitString : FieldType {
    }
}
