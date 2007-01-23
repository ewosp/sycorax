using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.Remoting;

using System.Diagnostics;


namespace UtilityLibrary.Serialization
{
  /// <summary>
  ///  The example of XML Document
  /// </summary>
  /// <example>
  ///   <configuration>
  ///     <subkey name='user subkey name'>
  ///       <value name='User Property Name' datatype='Simple DataType'>... Value ...</value>
  ///       ...
  ///       <value name='User Property Name' datatype='Array'>
  ///         <datatype>... Datatype string ...</datatype>
  ///         <value>... Value ...</value>
  ///       </value>
  ///       ...
  ///       <subkey name='user subkey name'>
  ///       ... other internal subkeys ...
  ///       </subkey>
  ///     </subkey>
  ///     .... other subkeys on the same level ...
  ///   </configuration>
  /// </example>
  public class XMLSerialize : IRootSerializator
  {
    #region Class constants
    private const string DEF_DEFAULT_XML = 
      "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
      "<configuration>" +
      "<subkey name='root'>" +              
      "</subkey>" +
      "</configuration>";   
    private const string DEF_START_POINT = @"/configuration/subkey[@name='root']";
    #endregion

    #region Class members
    static private IDictionary m_subkeys = CollectionsUtil.CreateCaseInsensitiveSortedList();
    
    private XmlDocument m_document;
    private string      m_strFilePath = string.Empty;
    private string      m_strSubkey = DEF_START_POINT;
//    private string      m_strXPathSubkey = string.Empty;
    #endregion

    #region Class Properties
    public string Subkey
    {
      get
      {
        return m_strSubkey;
      }
      set
      {
        if( value != m_strSubkey )
        {
          m_strSubkey = value;
          OnSubkeyChanged();
        }
      }
    }

    public string FilePath
    {
      get
      {
        return m_strFilePath;
      }
      set
      {
        if( value != m_strFilePath )
        {
          m_strFilePath = value;
          OnFilePathChanged();
        }
      }
    }

    #endregion

    #region Class indexer
    public ISerializator this[ string subkey ]
    {
      get
      {
        string newSubkey = string.Empty;
        // if cache does not contains subkey then create a new subkey
        if( m_subkeys.Contains( subkey ) == false )
        {
          newSubkey = CreateXPathSubkey( subkey );
          if( subkey == null || subkey.Trim().Length == 0 )
            m_subkeys[ subkey ] = this;
          else
            m_subkeys[ subkey ] = new XMLSerialize( m_document, newSubkey );
        }

        return ( ISerializator )m_subkeys[ subkey ];
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    public XMLSerialize()
    {
      m_document = new XmlDocument();
    }

    public XMLSerialize( string filePath ) : this()
    {
      m_strFilePath = filePath;
      Stream stream = new FileStream( m_strFilePath, FileMode.OpenOrCreate, FileAccess.Read );
      try
      {
        m_document.Load( stream );
      }
      catch( XmlException )
      {
        //if file is empty or with incorrect format create new XML
        m_document.LoadXml( DEF_DEFAULT_XML );
      }
      stream.Close();
    }

    
    public XMLSerialize( XmlDocument xml )
    {
      m_document = xml;
    }
    
    public XMLSerialize( XmlDocument xml, string subkey )
    {
      m_document = xml;
      m_strSubkey = subkey;
      
      if( ExistSubkey( subkey ) == false )
      {
        CreateNewSubkeyNode( subkey );
      }
    }
    
    
    public void Dispose()
    {
      if( m_strFilePath != null && m_strFilePath.Trim().Length > 0 )
      {
        try
        {
          Stream stream = new FileStream( m_strFilePath, FileMode.Create, FileAccess.ReadWrite );
          m_document.Save( stream );
          stream.Close();
        }
        catch( Exception ex )
        {
          Trace.WriteLine( ex.Message + "\r\n" + ex.StackTrace, "Dispose Exception" );
        }
      }
    }
    #endregion
    
    #region Class Add Methods
    public void AddValue(string name, object value)
    {
      try
      {
        XmlNode node = m_document.SelectSingleNode( m_strSubkey );
        AppendSimpleValNode( node, name, "object", value.ToString() );
      }
      catch( XPathException e )
      {
        Trace.WriteLine( " Error : " + e.Message );
        Trace.WriteLine( " XML : " + m_document.OuterXml);
        throw new Exception(" XPathException in Add Value " + e.Message);
      }          
    }

    public void AddValue(string name, int value)
    {
      try
      {
        XmlNode node = m_document.SelectSingleNode( m_strSubkey );
        AppendSimpleValNode( node, name, "int", value.ToString() );
      }
      catch( XPathException e )
      {
        Trace.WriteLine( " Error : " + e.Message );
        Trace.WriteLine( " XML : " + m_document.OuterXml);
        throw new Exception(" XPathException in Add Value " + e.Message);
      }          
    }

    public void AddValue(string name, bool value)
    {
      try
      {
        XmlNode node = m_document.SelectSingleNode( m_strSubkey );
        AppendSimpleValNode( node, name, "bool", value.ToString() );
      }
      catch( XPathException e )
      {
        Trace.WriteLine( " Error : " + e.Message );
        Trace.WriteLine( " XML : " + m_document.OuterXml);
        throw new Exception(" XPathException in Add Value " + e.Message);
      }          
    
    }

    public void AddValue(string name, System.Array value)
    {
      try
      {
        string strType = string.Empty;
        string tmp = ArrayToString( value, ref strType );
        
        if( IsSimpleType( strType ) == false )
          strType = "object";

        XmlNode node = m_document.SelectSingleNode( m_strSubkey );
        AppendArrayValNode( node, name, strType, tmp );
      }
      catch( XPathException e )
      {
        Trace.WriteLine( " Error : " + e.Message );
        Trace.WriteLine( " XML : " + m_document.OuterXml);
      }
    }

    public void AddValue(string name, System.DateTime value)
    {
      try
      {
        XmlNode node = m_document.SelectSingleNode( m_strSubkey );
        AppendSimpleValNode( node, name, "datetime", value.ToUniversalTime().ToString() );
      }
      catch( XPathException e )
      {
        Trace.WriteLine( " Error : " + e.Message );
        Trace.WriteLine( " XML : " + m_document.OuterXml);
        throw new Exception(" XPathException in Add Value " + e.Message);
      }              
    }

    public void AddValue(string name, string value)
    {
      try
      {
        XmlNode node = m_document.SelectSingleNode( m_strSubkey );
        AppendSimpleValNode( node, name, "string", value );
      }
      catch( XPathException e )
      {
        Trace.WriteLine( " Error : " + e.Message );
        Trace.WriteLine( " XML : " + m_document.OuterXml);
        throw new Exception(" XPathException in Add Value " + e.Message);
      }      
    }

    public void AddValue(string name, double value)
    {
      try
      {
        XmlNode node = m_document.SelectSingleNode( m_strSubkey );
        AppendSimpleValNode( node, name, "double", value.ToString( NumberFormatInfo.InvariantInfo ) );
      }
      catch( XPathException e )
      {
        Trace.WriteLine( " Error : " + e.Message );
        Trace.WriteLine( " XML : " + m_document.OuterXml);
        throw new Exception(" XPathException in Add Value " + e.Message);
      }          
    
    }

    #endregion

    #region Class Get Value methods
    public System.Array GetArrayValue(string name)
    {
      return GetArrayValue( name, null );
    }

    /// <summary>
    ///  Recover Array value, which contain type-insensitive elements.
    ///  Searching in particular subkey only by name
    /// </summary>
    /// <param name="name">The name of value</param>
    /// <param name="defValue">The default value, if value with particular name does not exist</param>
    /// <returns>Array value</returns>
    public System.Array GetArrayValue(string name, System.Array defValue )
    {
      try
      {
        string output = string.Empty;
        XmlNode node = m_document.SelectSingleNode( m_strSubkey + "value[@name = '" + name + "']" );
        if( node != null )
          output = node.InnerText;
        else
          Trace.WriteLine( "Error in GetArrayValue : The 'node' == null,  Non existing name or something else" );


        if( output.Trim().Length == 0 ) return defValue;
        
        return StringToArray( output, defValue );
      }
      catch( XPathException e )
      {
        Trace.WriteLine( " Error : " + e.Message );
        Trace.WriteLine( " XML : " + m_document.OuterXml);
      }              
      catch( FormatException e )
      {
        Trace.WriteLine(" Error:  FormatException ", e.Message );
      }
    
      return defValue;
    }

    /// <summary>
    /// Recover Array value, which contain type-sensitive elements.
    /// Searching in particular subkey by name and by type
    /// </summary>
    /// <param name="name">The name of value</param>
    /// <param name="defValue">The default value, if value with particular name does not exist</param>
    /// <param name="type">The type of value</param>
    /// <returns>Array value</returns>
    public System.Array GetArrayValue(string name, System.Array defValue, Type type )
    {
      try
      {
        string output = string.Empty; 
        XmlNode node = m_document.SelectSingleNode( m_strSubkey + "value[@name = '" + name + "' and datatype = '" + type.ToString() + "']"  );
        if( node != null )
          output = node.InnerText;
        else
          Trace.WriteLine( "Error in GetArrayValue(Type sensitive) : The 'node' == null,  Non existing name or something else" );


        if( output.Trim().Length == 0 ) return defValue;
        
        return StringToArray( output, defValue );
      }
      catch( XPathException e )
      {
        Trace.WriteLine( " Error : " + e.Message );
        Trace.WriteLine( " XML : " + m_document.OuterXml);
      }              
      catch( FormatException e )
      {
        Trace.WriteLine(" Error:  FormatException ", e.Message );
      }
    
      return defValue;
    }

    public bool GetBoolValue(string name)
    {
      return GetBoolValue( name, false );
    }

    public bool GetBoolValue(string name, bool defValue)
    {
      try
      {
        string output = string.Empty;
        XmlNode node = m_document.SelectSingleNode( m_strSubkey + "/value[@name='" + name +"' and @datatype = 'bool' ]" );
        if( node != null )
          output = node.InnerText;
        else
          Trace.WriteLine( "Error in GetBoolValue : The 'node' == null,  Non existing name or something else" );


        if( output.Trim().Length == 0 ) return defValue;
        
        return bool.Parse( output );
      }
      catch( XPathException e )
      {
        Trace.WriteLine( " Error : " + e.Message );
        Trace.WriteLine( " XML : " + m_document.OuterXml);
      }              
      catch( FormatException e )
      {
        Trace.WriteLine(" Error:  FormatException ", e.Message );
      }
    
      return defValue;
    }

    public System.DateTime GetDateTimeValue(string name)
    {
      return GetDateTimeValue( name, DateTime.MinValue );
    }

    public System.DateTime GetDateTimeValue(string name, System.DateTime defValue)
    {
      try
      {
        string output = string.Empty;
        XmlNode node = m_document.SelectSingleNode( m_strSubkey + "/value[@name='" + name +"' and @datatype = 'datetime' ]" );
        if( node != null )
          output = node.InnerText;
        else
          Trace.WriteLine( "Error in GetDateTimeValue : The 'node' == null,  Non existing name or something else" );


        if( output.Trim().Length == 0 ) return defValue;
        
        return DateTime.Parse( output );
      }
      catch( XPathException e )
      {
        Trace.WriteLine( " Error : ", e.Message );
        Trace.WriteLine( " XML : ", m_document.OuterXml);
      }              
      catch( FormatException e )
      {
        Trace.WriteLine(" Error:  FormatException ", e.Message );
      }
    
      return defValue;
    }

    public double GetDoubleValue(string name)
    {
      return GetDoubleValue( name, 0.0 );
    }

    public double GetDoubleValue(string name, double defValue)
    {
      try
      {
        string output = string.Empty;
        XmlNode node = m_document.SelectSingleNode( m_strSubkey + "/value[@name='" + name +"' and @datatype = 'double' ]" );
        if( node != null )
          output = node.InnerText;
        else
          Trace.WriteLine( "Error in GetDoubleValue : The 'node' == null,  Non existing name or something else" );

        if( output.Trim().Length == 0 ) return defValue;
        
        return double.Parse( output, NumberFormatInfo.InvariantInfo );
      }
      catch( XPathException e )
      {
        Trace.WriteLine( " Error : ", e.Message );
        Trace.WriteLine( " XML : ", m_document.OuterXml);
      }              
      catch( FormatException e )
      {
        Trace.WriteLine(" Error:  FormatException ", e.Message );
      }
    
      return defValue;
    }

    public int GetIntValue(string name)
    {
      return GetIntValue( name, 0 );
    }

    public int GetIntValue(string name, int defValue)
    {
      try
      {
        string output = String.Empty;
        XmlNode node = m_document.SelectSingleNode( m_strSubkey + "/value[@name='" + name +"' and @datatype = 'int' ]" );
        if( node != null )
          output = node.InnerText;
        else
          Trace.WriteLine( "Error in GetIntValue : The 'node' == null,  Non existing name or something else" );

        if( output.Trim().Length == 0 ) return defValue;
        
        return Int32.Parse( output );
      }
      catch( XPathException e )
      {
        Trace.WriteLine( " Error : ", e.Message );
        Trace.WriteLine( " XML : ", m_document.OuterXml);
      } 
      catch( FormatException e )
      {
        Trace.WriteLine(" Error:  FormatException ", e.Message );
      }
    
      return defValue;
    }

    public string GetStringValue(string name)
    {
      return GetStringValue( name, string.Empty );
    }

    public string GetStringValue(string name, string defValue)
    {
      try
      {
        string output = String.Empty;   
        XmlNode node = m_document.SelectSingleNode( m_strSubkey + "/value[@name='" + name +"' and @datatype = 'string' ]" );
        if( node != null )
          output = node.InnerText;
        else
          Trace.WriteLine( "Error in GetStringValue : The 'node' == null,  Non existing name or something else" );

        if( output.Trim().Length == 0 ) return defValue;
        
        return output;
      }
      catch( XPathException e )
      {
        Trace.WriteLine( " Error : ", e.Message );
        Trace.WriteLine( " XML : ", m_document.OuterXml);
      }              
      catch( FormatException e )
      {
        Trace.WriteLine(" Error:  FormatException ", e.Message );
      }
    
      return defValue;
    }

    public object GetValue( string name )
    {
      return GetValue( name, null );
    }

    public object GetValue(string name, object defValue)
    {
      try
      {
        string output = String.Empty;   
        XmlNode node = m_document.SelectSingleNode( m_strSubkey + "/value[@name='" + name +"' and @datatype = 'object' ]" );
        if( node != null )
          output = node.InnerText;
        else
          Trace.WriteLine( "Error in GetValue : The 'node' == null,  Non existing name or something else" );

        if( output.Trim().Length == 0 ) return defValue;
        
        return (object)output;
      }
      catch( XPathException e )
      {
        Trace.WriteLine( " Error : ", e.Message );
        Trace.WriteLine( " XML : ", m_document.OuterXml);
      }              
      catch( FormatException e )
      {
        Trace.WriteLine(" Error:  FormatException ", e.Message );
      }
    
      return defValue;
    }

    #endregion

    #region Class Helper methods
    public void Save()
    {
      if( m_strFilePath != null && m_strFilePath.Trim().Length > 0 )
      {
        try
        {
          Stream stream = new FileStream( m_strFilePath, FileMode.Create, FileAccess.ReadWrite );
          m_document.Save( stream );
          stream.Close();
        }
        catch( Exception ex )
        {
          Trace.WriteLine( ex.Message + "\r\n" + ex.StackTrace, "Dispose Exception" );
        }
      }    
    }

    
    private string GetAttributeByName( XmlNode node, string name )
    {
      if( node != null && node.Attributes != null )
      {
        XmlAttribute attr = ( XmlAttribute )node.Attributes.GetNamedItem( name );
        
        if( attr != null ) return attr.Value;
      }

      return string.Empty;
    }


    private string ArrayToString( Array value, ref string strType )
    {
      string[] tmp = new string[ value.Length + 1 ];
      int iCount = 1;

      // save datatype
      Type type = value.GetValue(0).GetType();
      strType = type.ToString();
      tmp[0] = type.Assembly.FullName + ":" + strType;

      foreach( object obj in value )
      {
        tmp[ iCount ] = obj.ToString();
        iCount++;
      }

      return string.Join( ";", tmp );
    
    }

    private Array StringToArray( string str, Array defValue )
    {
      if( str.Length == 0 ) return defValue;

      string[] tmp = str.Split( ';' );

      // check data (minimaly must be two items)
      if( tmp.Length < 2 ) return defValue;

      object[]  output = new object[ tmp.Length - 1 ];
      string[]  types = tmp[0].Split( ':' );

      // check format of saved data
      if( types.Length != 2 ) return defValue;

      ObjectHandle  typeHandle = Activator.CreateInstance( types[0], types[1] );
      Type          type = typeHandle.Unwrap().GetType();
      Type[]        methTypes = new Type[]{ typeof( string ) };
      bool          bIsEnum = type.IsEnum;
      MethodInfo    method = type.GetMethod( "Parse", methTypes );

      // if we does not found Parse method then return default value
      if( method == null && bIsEnum == false )  return defValue;

      for( int i=1; i<tmp.Length; i++ )
      {
        output[ i-1 ] = ( bIsEnum ) ? 
          Enum.Parse( type, tmp[i], true ) :
          method.Invoke( null, new object[]{ tmp[i] } );
      }

      return output;
    
    }


    private XmlNode ExistNode( XmlNode node, string name, string datatype, bool IsArray )
    {
      if( node != null )
      {
      
        if( IsArray == false )
        {
          return node.SelectSingleNode("value[@name = '" + name + "' and @datatype = '" +datatype+ "']");
        }
        else
        {  
          return node.SelectSingleNode( "value[@name = '" + name + "' and datatype = '" +datatype+ "']" );
        }
      }
      else
      {
        Trace.WriteLine(" Error node in ExistNode == null ");
        return null;
      }
    }

    private bool ExistSubkey( string subkey )
    {      
      XmlNode _nd = m_document.SelectSingleNode( subkey );

      return (_nd != null)? true : false;
    }

    private bool IsSimpleType( string  strType )
    {
      if( (strType.ToLower().CompareTo( "system.int32" ) == 0)
          ||(strType.ToLower().CompareTo( "system.bool" ) == 0)
          ||(strType.ToLower().CompareTo( "system.string" ) == 0)
          ||(strType.ToLower().CompareTo( "system.datetime" ) == 0)
          ||(strType.ToLower().CompareTo( "system.double" ) == 0) )
        return true;
      else
        return false;
    }


    private void CreateNewSubkeyNode( string subkey )
    {
      int k = m_strSubkey.LastIndexOf( '/' );
      string strXPath = m_strSubkey.Remove( k, m_strSubkey.Length - k );

      XmlNode _nd = m_document.SelectSingleNode( strXPath );

      XmlAttribute[] attributes = new XmlAttribute[1];
      attributes[0] = m_document.CreateAttribute( "name" );
      attributes[0].Value = GetSubkeyFromXPath( subkey );

      AppendNode( _nd, attributes, "subkey", string.Empty );
    }

    private string CreateXPathSubkey( string subkey )
    {
      return m_strSubkey + "/subkey[@name='" + subkey + "']";
    }


    private string GetSubkeyFromXPath( string XPathSubkey )
    {
      int k = XPathSubkey.LastIndexOf( "'" );
      string strTmp = XPathSubkey.Remove( k, XPathSubkey.Length - k  );

      return strTmp.Substring( strTmp.LastIndexOf( "'" ) + 1 );
    }
    private void AppendSimpleValNode( XmlNode node, string strName, string strType, string strValue )
    {
      if( node != null )
      {
        XmlNode _nd = ExistNode( node, strName, strType, false );

        if( _nd == null )
        {      
          XmlAttribute[] attributes = new XmlAttribute[2];

          attributes[0] = m_document.CreateAttribute( "name" );			
          attributes[0].Value = strName;

          attributes[1] = m_document.CreateAttribute( "datatype" );
          attributes[1].Value = strType;

          AppendNode( node, attributes, "value", strValue );
        }
        else
        {
          _nd.InnerText = strValue;
        }
      }
      else
      {
        Trace.WriteLine(" Error The parent node in AppendSimpleValNode is null ");
      }
    }
    private void AppendArrayValNode( XmlNode node, string strName, string strType, string strValue )
    {      
      XmlNode _nd = ExistNode( node, strName, strType, true );
      
      if( _nd == null )
      {
        XmlAttribute[] attributes = new XmlAttribute[2];

        attributes[0] = m_document.CreateAttribute( "name" );			
        attributes[0].Value = strName;

        attributes[1] = m_document.CreateAttribute( "datatype" );
        attributes[1].Value = "array";

        XmlNode childNode = AppendNode( node, attributes, "value", string.Empty );

        AppendNode( childNode, null, "datatype", strType );
        AppendNode( childNode, null, "value", strValue );
      }
      else
      {
        XmlNode nd =  _nd.ChildNodes[ 1 ];
        nd.InnerText = strValue;

        // set data type
//        nd = _nd.ChildNodes[ 0 ];
//        nd.InnerText = strType;
      }
    }

    private XmlNode AppendNode( XmlNode parentNode, XmlAttribute[] attrs, string strName, string strInnerTxt )
    {
      try
      {            
        XmlElement elem = m_document.CreateElement( strName );

        if( attrs != null )
        {
          foreach( XmlAttribute attr in attrs )
            elem.Attributes.Append( attr );
        }

        elem.InnerText = strInnerTxt;

        // Append this element to current node.
        return parentNode.AppendChild( elem );
      }
      catch( ArgumentException e ) // Append, AppendChild exceptions
      { 
        Trace.WriteLine( e.Message, "Error: ");
        return null; 
      }
      catch( InvalidOperationException e ) // AppendChild exceptions
      { 
        Trace.WriteLine( e.Message, "Error: ");
        return null; 
      }
      catch( XmlException e )
      {
        Trace.WriteLine( e.Message, "Error: ");
        return null;       
      }
    }


    protected virtual void OnSubkeyChanged()
    {
      if( m_strSubkey != null )
      {
        int pos = m_strSubkey.LastIndexOf( @"\" );
        
        // subkey path always must have in end of line slash
        if( pos < 0 || pos != m_strSubkey.Length-1 )
          m_strSubkey += @"";
      }
      else
      {
        m_strSubkey = DEF_START_POINT;
      }
    }
    protected virtual void OnFilePathChanged()
    {    
    }
    #endregion
  }
}
