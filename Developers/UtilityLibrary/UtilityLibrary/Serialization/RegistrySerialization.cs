using System;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml;
using System.Globalization;
using System.Reflection;
using System.Diagnostics;

using Microsoft.Win32;
using UtilityLibrary.Serialization;


namespace UtilityLibrary.Serialization
{
  [ToolboxItem(true)]
  public class RegistrySerialization : Component
  {
    #region Class constants
    const string DEF_REGISTRY_KEY = @"Software\ELEKS Software Ltd.\TeamPro\Settings";
    #endregion

    #region Class members
    private static RegistryKey   m_key = null;
    private static IDictionary   m_subkeys = null;
    
    private string m_strStorePath = DEF_REGISTRY_KEY;
    private Form   m_form;
    #endregion

    #region Internal declarations and classes
    public delegate void SerializationEventHandler( object sender, SerializationEventArgs data );
    #endregion

    #region Class Properties
    [ Browsable( false ) ]
    public ISerializator Storage
    {
      get
      { 
        CreateKeyByStorePath();
        return new RegistrySerialize( ( RegistryKey )m_subkeys[ m_strStorePath ], "" );
      }
    }
    
    [ Browsable( true ), 
    DefaultValue( null ),
    Description( "GET/SET form which need settings serialization" ) ]
    public Form AttachedForm
    {
      get
      {
        return m_form;
      }
      set
      {
        if( value != m_form )
        {
          DetachSerialiazationEvents();
          m_form = value;
          AttachSerialiazationEvents();
        }
      }
    }
    
    [ Browsable( false ) ]
    public ISerializator this[ string subkey ]
    {
      get
      {
        if( m_subkeys.Contains( subkey ) == false )
        {
          // try to open key first
          if( m_key == null )
          {
            // create if no m_key
            CreateKeyByStorePath();
            m_key = ( RegistryKey )m_subkeys[ m_strStorePath ];
          }

          RegistryKey sub = m_key.OpenSubKey( subkey, true );
          
          if( sub == null )
          {
            sub = m_key.CreateSubKey( subkey );
            
            if( sub == null )
            {
              throw new ApplicationException( "Cannot create registy keys. Please check user permissions." );
            }
          }

          m_subkeys[ subkey ] = sub;
        }
        
        return new RegistrySerialize( (RegistryKey)m_subkeys[ subkey ], subkey );
      }
    }
    
    [ Browsable( true ),
    Description( "GET/SET where in registry data must be stored" ),
    DefaultValue( DEF_REGISTRY_KEY ) ]
    public string StorePath
    {
      get
      {
        return m_strStorePath;
      }
      set
      {
        if( value != m_strStorePath )
        {
          m_strStorePath = value;
          OnStorePathChanged();
        }
      }
    }
    #endregion

    #region Class Events
    public event SerializationEventHandler RecoverSettings;
    public event SerializationEventHandler SaveSettings;
    #endregion

    #region Class Initialization/Finilize methods
    public RegistrySerialization()
    {
      if( m_subkeys == null ) m_subkeys = new SortedList();

      AttachSerialiazationEvents();
    }

    public RegistrySerialization( Form form )
    {
      if( m_subkeys == null ) m_subkeys = new SortedList();

      m_form = form;
      AttachSerialiazationEvents();
    }
    
    public void ForceDataRecovery()
    {
      OnFormLoadSerialization( this, EventArgs.Empty );
    }

    public void ForceDataSaving()
    {
      OnFormCloseSerialization( this, new CancelEventArgs( false ) );
    }
    #endregion

    #region Class helper methods
    private void AttachSerialiazationEvents()
    {
      if( m_form != null )
      {
        m_form.Load += new EventHandler( OnFormLoadSerialization );
        m_form.Closing += new CancelEventHandler( OnFormCloseSerialization );
      }
    }

    private void DetachSerialiazationEvents()
    {
      if( m_form != null )
      {
        m_form.Load -= new EventHandler( OnFormLoadSerialization );
        m_form.Closing -= new CancelEventHandler( OnFormCloseSerialization );
      }
    }
    #endregion

    #region Event Handlers
    private void CreateKeyByStorePath()
    {
      RegistryKey   key;

      // create in cache new entry if tit does not exist yet
      if( m_subkeys.Contains( m_strStorePath ) == false )
      {
        // first try to open
        key = Registry.CurrentUser.OpenSubKey( m_strStorePath, true );
        
        if( key == null )
        {
          key = Registry.CurrentUser.CreateSubKey( m_strStorePath );
          
          if( key == null )
          {
            throw new ApplicationException( "Cannot create registy keys. Please check user permissions." );
          }
        }
        
        m_subkeys.Add( m_strStorePath, key );
      }
    }

    protected virtual void OnStorePathChanged()
    {
      CreateKeyByStorePath();
    }

    protected virtual void OnFormCloseSerialization( object sender, CancelEventArgs e )
    {
      if( SaveSettings != null )
      {
        SerializationEventArgs ev = new SerializationEventArgs( Storage, this );

        SaveSettings( this, ev );
      }
    }

    protected virtual void OnFormLoadSerialization( object sender, EventArgs e )
    {
      if( RecoverSettings != null )
      {
        SerializationEventArgs ev = new SerializationEventArgs( Storage, this );

#if RELEASE
        try
        {
#endif
        RecoverSettings( this, ev );
#if RELEASE
        }
        catch( Exception ex ) // catch any recovery exception
        {
          Trace.WriteLine( ex.Message + "\r\n" + ex.StackTrace, "Serialization Recovery FATAL" );
        }
#endif
      }
    }
    #endregion
  }

  #region Serialization Event Args
  public class SerializationEventArgs : EventArgs
  {
    private ISerializator m_key;
    private RegistrySerialization m_storage;

    public ISerializator Storage
    {
      get
      { 
        return m_key; 
      }
    }

    public ISerializator this[ string subkey ]
    {
      get
      {
        return m_storage[ subkey ];
      }
    }

    
    private SerializationEventArgs(){}

    public SerializationEventArgs( ISerializator key, RegistrySerialization storage )
    {
      m_key = key;
      m_storage = storage;
    }
  }
  #endregion
}
