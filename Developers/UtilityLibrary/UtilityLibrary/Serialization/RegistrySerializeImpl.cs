using System;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml;
using System.Globalization;
using System.Reflection;
using System.Runtime.Remoting;

using Microsoft.Win32;
using System.Diagnostics;


namespace UtilityLibrary.Serialization
{
  public class RegistrySerialize : ISerializator
  {
    #region Class Constants
    const string DEF_REGISTRY_KEY = @"Software\ELEKS Software Ltd.\TeamPro\Settings";
    #endregion

    #region Class members
    private RegistryKey m_registry;
    private string      m_strSubKey;
    private static RegistryKey   m_key = null;
    #endregion

    #region Class Initialize/Finalize methods
    private RegistrySerialize()
    {
    }

    public RegistrySerialize( RegistryKey parent, string subkey )
    {
      m_registry = parent;
      m_strSubKey = subkey;
    }

    /// <summary>
    /// Create in CurentUser key
    /// </summary>
    /// <param name="subkey"></param>
    public RegistrySerialize ( string subkey )
    {
      if( m_key == null )
      {
        m_key = Registry.CurrentUser.OpenSubKey( DEF_REGISTRY_KEY + @"\" + subkey, true );
          
        if( m_key == null )
        {
          m_key = Registry.CurrentUser.CreateSubKey( DEF_REGISTRY_KEY + @"\" + subkey );
            
          if( m_key == null )
          {
            throw new ApplicationException( "Cannot create registy keys. Please check user permissions." );
          }
        }
      }
      m_registry = m_key;
      m_strSubKey = subkey;
    }
    #endregion
    
    #region Add Value overrides
    public void AddValue( string name, object value )
    {
      m_registry.SetValue( name, value );
    }

    public void AddValue( string name, int value )
    {
      m_registry.SetValue( name, value );
    }

    public void AddValue( string name, bool value )
    {
      m_registry.SetValue( name, value.ToString() );
    }

    public void AddValue( string name, Array value )
    {
      string[] tmp = new string[ value.Length + 1 ];
      int iCount = 1;

      // save datatype
      Type type = ( value.Length > 0 ) ? value.GetValue(0).GetType() : typeof(string);
      tmp[0] = type.Assembly.FullName + ":" + type.ToString();

      foreach( object obj in value )
      {
        tmp[ iCount ] = obj.ToString();
        iCount++;
      }

      string output = string.Join( ";", tmp );

      m_registry.SetValue( name, output );
    }

    public void AddValue( string name, DateTime value )
    {
      m_registry.SetValue( name, value.ToUniversalTime() );
    }

    public void AddValue( string name, string value )
    {
      m_registry.SetValue( name, value );
    }

    public void AddValue( string name, double value )
    {
      m_registry.SetValue( name, value.ToString( NumberFormatInfo.InvariantInfo  ) );
    }
    #endregion
    
    #region GetValue Overrides
    public Array GetArrayValue( string name )
    {
      return GetArrayValue( name, null );
    }

    public Array GetArrayValue( string name, Array defValue )
    {
      string read = GetStringValue( name, "" );
      if( read.Length == 0 ) return defValue;

      string[]  tmp = read.Split( ';' );

      // check data (minimaly must be two items)
      if( tmp.Length < 2 ) return defValue;

      object[]  output = new object[ tmp.Length - 1 ];
      string[]  types = tmp[0].Split( ':' );

      // check format of saved data
      if( types.Length != 2 ) return defValue;

      ObjectHandle  typeHandle = null;
      
      if( types[1] != "System.String" )
      {
        typeHandle = Activator.CreateInstance( types[0], types[1] );
        
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
      }
      else // simply copy strings
      {
        for( int i=1; i<tmp.Length; i++ )
        {
          output[ i-1 ] = tmp[i];
        }
      }

      return output;
    }

    public int GetIntValue( string name )
    {
      return GetIntValue( name, (int)0 );
    }

    public int GetIntValue( string name, int defValue )
    {
      string output = GetStringValue( name );
      if( output.Length == 0 ) return defValue;
      return int.Parse( output );
    }

    public bool GetBoolValue( string name )
    {
      return GetBoolValue( name, false );
    }

    public bool GetBoolValue( string name, bool defValue )
    {
      string output = GetStringValue( name );
      if( output.Length == 0 ) return defValue;

      return bool.Parse( output );
    }

    public DateTime GetDateTimeValue( string name )
    {
      return GetDateTimeValue( name, DateTime.MinValue );
    }

    public DateTime GetDateTimeValue( string name, DateTime defValue )
    {
      string output = GetStringValue( name );
      if( output.Length == 0 ) return defValue;
      
      return DateTime.Parse( output );
    }

    public string GetStringValue( string name )
    {
      return GetStringValue( name, "" );
    }

    public string GetStringValue( string name, string defValue )
    {
      return m_registry.GetValue( name, defValue ).ToString();
    }

    public double GetDoubleValue( string name )
    {
      return GetDoubleValue( name, (double)0 );
    }

    public double GetDoubleValue( string name, double defValue )
    {
      string output = GetStringValue( name );
      if( output.Length == 0 ) return defValue;
      
      return double.Parse( output, NumberFormatInfo.InvariantInfo );
    }

    public object GetValue( string name )
    {
      return GetValue( name, (object)null );
    }

    public object GetValue( string name, object defValue )
    {
      return m_registry.GetValue( name, defValue );
    }
    #endregion
  }
}
