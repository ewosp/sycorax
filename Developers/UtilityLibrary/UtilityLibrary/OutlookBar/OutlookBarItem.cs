using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Drawing.Design;

using UtilityLibrary.Collections;
using UtilityLibrary.Win32;
using UtilityLibrary.General;
using UtilityLibrary.Menus;


namespace UtilityLibrary.WinControls
{
  [ToolboxItem(false)]
  public class OutlookBarItem : Component
  {
    #region Class variables
    private string    m_strText = "";
    private string    m_strToolTip = "";
    private int       m_iImageIndex = -1;
    private object    m_tag;
    private ImageList m_imgList;
    private bool      m_checked;  
    private OutlookBar m_bar;
    #endregion

    #region Class Events
    [Category("Property Changed")]
    public event EventHandler Changed;
    
    [Category("Property Changed")]
    public event EventHandler ImageIndexChanged;
    
    [Category("Property Changed")]
    public event EventHandler ImageListChanged;
    
    [Category("Property Changed")]
    public event EventHandler TextChanged;
    
    [Category("Property Changed")]
    public event EventHandler TagChanged;
    
    [Category("Property Changed")]
    public event EventHandler ToolTipChanged;
    #endregion
    
    #region Class Constructors
    public OutlookBarItem( OutlookBar bar, string text, int ImageIndex )
    {
      m_bar = bar;
      m_strText = text;
      m_iImageIndex = ImageIndex;
    }

    public OutlookBarItem( OutlookBar bar, string text, int imageIndex, object tag )
    {
      m_bar = bar;
      m_strText = text;
      m_iImageIndex = imageIndex;
      m_tag = tag;
    }

    public OutlookBarItem( )
    {
      m_bar = null;
      // To support designer
      m_iImageIndex = -1;
    }
    #endregion

    #region Overrides
    protected virtual void OnImageIndexChanged()
    {
      RaiseImageIndexChangedEvent();
    }
    
    protected virtual void OnImageListChanged()
    {
      RaiseImageListChangedEvent();
    }
    
    protected virtual void OnTextChanged()
    {
      RaiseTextChangedEvent();
    }
    
    protected virtual void OnToolTipChanged()
    {
      RaiseToolTipChangedEvent();
    }
    
    protected virtual void OnTagChanged()
    {
      RaiseTagChangedEvent();
    }
    
    #endregion

    #region Methods 
    protected void RaiseChangedEvent()
    {
      if( Changed != null )
      {
        Changed( this, EventArgs.Empty );
      }
    }

    protected void RaiseImageIndexChangedEvent()
    {
      if( ImageIndexChanged != null )
      {
        ImageIndexChanged( this, EventArgs.Empty );
      }
      
      RaiseChangedEvent();
    }
    protected void RaiseImageListChangedEvent()
    {
      if( ImageListChanged != null )
      {
        ImageListChanged( this, EventArgs.Empty );
      }
      
      RaiseChangedEvent();
    }
    
    protected void RaiseTextChangedEvent()
    {
      if( TextChanged != null )
      {
        TextChanged( this, EventArgs.Empty );
      }
      
      RaiseChangedEvent();
    }
    
    protected void RaiseToolTipChangedEvent()
    {
      if( ToolTipChanged != null )
      {
        ToolTipChanged( this, EventArgs.Empty );
      }
      
      RaiseChangedEvent();
    }
    
    protected void RaiseTagChangedEvent()
    {
      if( TagChanged != null )
      {
        TagChanged( this, EventArgs.Empty );
      }
      
      RaiseChangedEvent();
    }
    
    #endregion
    
    #region Properties
    [DefaultValue(-1), Category( "Appearance" )]
    [Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
    [TypeConverter("System.Windows.Forms.ImageIndexConverter")]
    public int        ImageIndex
    {
      get 
      { 
        return m_iImageIndex; 
      }
      set 
      { 
        if( value != m_iImageIndex )
        {
          m_iImageIndex = value; 
          OnImageIndexChanged();
        }
      }
    }

    [ DefaultValue( null ), Category( "Appearance" )]
    public ImageList  ImageList
    {
      get
      {
        return m_imgList;
      }
      set
      {
        if( value != m_imgList )
        {
          m_imgList = value;
          OnImageListChanged();
        }
      }
    }
    
    [ DefaultValue(""), Category( "Appearance" ), Description( "GET/SET text to display" )]
    public string     Text
    {
      get 
      { 
        return m_strText; 
      }
      set 
      { 
        if( value != m_strText )
        {
          m_strText = value; 
          OnTextChanged();
        }
      }
    }

    [DefaultValue(""), Category( "Appearance" ), Description( "GET/SET item tooltip" )]
    public string     ToolTip
    {
      get
      {
        return m_strToolTip;
      }
      set
      {
        if( value != m_strToolTip )
        {
          m_strToolTip = value;
          OnToolTipChanged();
        }
      }
    }
    
    [ DefaultValue( null ), Category( "Data" ), Description( "User defined data asociated with item" ) ]
    public object     Tag
    {
      get 
      { 
        return m_tag; 
      }
      set 
      { 
        if( value != m_tag )
        {
          m_tag = value; 
          OnTagChanged();
        }
      }
    }
    
    [DefaultValue(false), Category( "Appearance" ), Description( "GET/SET item checked state" )]
    public bool       Checked 
    {
      get
      {
        return m_checked;
      }
      set
      {
        if ( value != m_checked ) 
        {
          m_checked = value;
          
          if ( m_bar != null ) 
            m_bar.Invalidate();
        }
      }
    }
    #endregion
  }
}
