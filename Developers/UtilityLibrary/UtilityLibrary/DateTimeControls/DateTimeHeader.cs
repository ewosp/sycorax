using System;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;


namespace UtilityLibrary.DateTimeControls
{
  [ ToolboxItem( true ) ]
  public class HeaderControl : Control
  {
    #region Class Constants
    private const int DEF_HEADER_HEIGHT = 21;
    #endregion

    #region Class members
    private int   m_iLevelHeight = DEF_HEADER_HEIGHT;
    private int   m_iLevelsCount = 1; // by default we always have one level of header
    private bool  m_bSkipEvents;

    private ArrayList m_headersColl = new ArrayList( 2 );
    #endregion

    #region Class Internal declarations
    [Flags]
      public enum TCellState
    {
      #region Enums
      Normal    = 0x0000,
      Active    = 0x0001,
      Selected  = 0x0002,
      
      All  = Active | Selected
      #endregion
    };

    public class HeaderCell
    {
      #region Class members
      private Rectangle   m_rect;
      private string      m_strText     = "";
      private string      m_strToolTip  = "";
      private Color       m_backColor   = SystemColors.Control;
      private Color       m_foreColor   = SystemColors.ControlText;
      
      private ImageList   m_imageList;
      private int         m_iImageIndex = -1;
      private bool        m_bSkipEvents;
      private TCellState  m_enState;
      private object      m_tag         = null;
      #endregion

      #region Class events
      [ Category( "Property Changed" ) ]
      public event EventHandler ImageIndexChanged;

      [ Category( "Property Changed" ) ]
      public event EventHandler ImageListChanged;
      
      [ Category( "Property Changed" ) ]
      public event EventHandler TextChanged;
      
      [ Category( "Property Changed" ) ]
      public event EventHandler ToolTipChanged;
      
      [ Category( "Property Changed" ) ]
      public event EventHandler BackColorChanged;
      
      [ Category( "Property Changed" ) ]
      public event EventHandler ForeColorChanged;
      
      [ Category( "Property Changed" ) ]
      public event EventHandler RectChanged;

      [ Category( "Property Changed" ) ]
      public event EventHandler StateChanged;
      
      [ Category( "Property Changed" ) ]
      public event EventHandler TagChanged;
      #endregion

      #region Class properties
      [ DefaultValue( -1 ),
      Editor( "System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", 
        typeof( System.Drawing.Design.UITypeEditor ) ),
      TypeConverter("System.Windows.Forms.ImageIndexConverter"), 
      Category( "Appearance" ) ]
      public int ImageIndex
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
      
      [ DefaultValue( null ), Browsable( true ), Category( "Appearance" ) ]
      public ImageList ImageList
      {
        get
        {
          return m_imageList;
        }
        set
        {
          if( value != m_imageList )
          {
            m_imageList = value;
            OnImageListChanged();
          }
        }
      }
      
      [ DefaultValue( "" ), Browsable( true ), Category( "Appearance" ) ]
      public string Text
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
      
      [ DefaultValue( "" ), Browsable( true ), Category( "Appearance" ) ]
      public string ToolTip
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
      
      [ Browsable( true ), Category( "Appearance" ) ]
      public Color BackColor
      {
        get
        {
          return m_backColor;
        }
        set
        {
          if( value != m_backColor )
          {
            m_backColor = value;
            OnBackColorChanged();
          }
        }
      }
      
      [ Browsable( false ) ]
      public bool QuietMode
      {
        get
        {
          return m_bSkipEvents;
        }
        set
        {
          if( value != m_bSkipEvents )
          {
            m_bSkipEvents = value;
            OnQuietModeChanged();
          }
        }
      }
      
      [ Browsable( true ), Category( "Appearance" ) ]
      public Color ForeColor
      {
        get
        {
          return m_foreColor;
        }
        set
        {
          if( value != m_foreColor )
          {
            m_foreColor = value;
            OnForeColorChanged();
          }
        }
      }
      
      [ Browsable( false ) ]
      public Rectangle Rect
      {
        get
        {
          return m_rect;
        }
        set
        {
          if( value != m_rect )
          {
            m_rect = value;
            OnRectChanged();
          }
        }
      }
      
      [ Browsable( false ) ]
      public TCellState State
      {
        get
        {
          return m_enState;
        }
        set
        {
          if( value != m_enState )
          {
            m_enState = value;
            OnStateChanged();
          }
        }
      }
      
      [ Browsable( false ) ]
      public bool IsActive
      {
        get
        {
          return ( m_enState & TCellState.Active ) == TCellState.Active;
        }
      }
      
      [ Browsable( false ) ]
      public bool IsSelected
      {
        get
        {
          return ( m_enState & TCellState.Selected ) == TCellState.Selected;
        }
      }
      
      [ Browsable( false ) ]
      public bool IsNormal
      {
        get
        {
          return ( m_enState & TCellState.All ) == 0;
        }
      }
      [ Browsable( false ) ]
      public object Tag
      {
        get
        {
          return m_tag;
        }
        set
        {
          m_tag = value;
          OnTagChanged();
        }
      }
      #endregion

      #region Class helper methods
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
      
      protected virtual void OnBackColorChanged()
      {
        RaiseBackColorChangedEvent();
      }
      
      protected virtual void OnQuietModeChanged()
      {
        // NOTE: inheritors can use this method to detect Quiet mode 
      }
      protected virtual void OnForeColorChanged()
      {
        RaiseForeColorChangedEvent();
      }
      
      protected virtual void OnRectChanged()
      {
        RaiseRectChangedEvent();
      }
      protected virtual void OnStateChanged()
      {
        RaiseStateChangedEvent();
      }
      
      protected virtual void OnTagChanged()
      {
        RaiseTagChangedEvent();
      }
      
      
      protected void RaiseImageIndexChangedEvent()
      {
        if( ImageIndexChanged != null && QuietMode == false )
        {
          ImageIndexChanged( this, EventArgs.Empty );
        }
      }

      protected void RaiseImageListChangedEvent()
      {
        if( ImageListChanged != null && QuietMode == false )
        {
          ImageListChanged( this, EventArgs.Empty );
        }
      }

      protected void RaiseTextChangedEvent()
      {
        if( TextChanged != null && QuietMode == false )
        {
          TextChanged( this, EventArgs.Empty );
        }
      }

      protected void RaiseToolTipChangedEvent()
      {
        if( ToolTipChanged != null && QuietMode == false )
        {
          ToolTipChanged( this, EventArgs.Empty );
        }
      }

      protected void RaiseBackColorChangedEvent()
      {
        if( BackColorChanged != null && QuietMode == false )
        {
          BackColorChanged( this, EventArgs.Empty );
        }
      }

      protected void RaiseForeColorChangedEvent()
      {
        if( ForeColorChanged != null && QuietMode == false )
        {
          ForeColorChanged( this, EventArgs.Empty );
        }
      }

      protected void RaiseRectChangedEvent()
      {
        if( RectChanged != null && QuietMode == false )
        {
          RectChanged( this, EventArgs.Empty );
        }
      }

      protected void RaiseStateChangedEvent()
      {
        if( StateChanged != null && QuietMode == false )
        {
          StateChanged( this, EventArgs.Empty );
        }
      }

      protected void RaiseTagChangedEvent()
      {
        if( TagChanged != null && QuietMode == false )
        {
          TagChanged( this, EventArgs.Empty );
        }
      }
      #endregion
    }

    
    public class HeaderCellEventArgs : EventArgs
    {
      #region Class Properties
      private HeaderCell m_cell;
      #endregion
      
      #region Class Properties
      public HeaderCell Item
      {
        get
        {
          return m_cell;
        }
      }
      #endregion
      
      #region Class Constructors
      private HeaderCellEventArgs()
      {
      
      }

      public HeaderCellEventArgs( HeaderCell cell )
      {
        m_cell = cell;
      }
      #endregion
    }

    public delegate void HeaderCellEventHandler( object sender, HeaderCellEventArgs e );

    public class HeaderCellCollection : CollectionBase
    {
      #region Class events
      public event HeaderCellEventHandler Changed;
      public event HeaderCellEventHandler OnItemAdded;
      public event HeaderCellEventHandler OnItemRemoved;
      public event HeaderCellEventHandler OnItemSet;
      
      public event EventHandler OnClearItems;
      #endregion  

      #region IList methods
      public int Add( HeaderCell value )
      {
        return List.Add( value ); 
      }

      public bool Contains( HeaderCell value )
      {
        return InnerList.Contains( value );
      }

      public int IndexOf( HeaderCell value )
      {
        return InnerList.IndexOf( value );
      }

      public void Insert( int index, HeaderCell value )
      {
        List.Insert( index, value );
      }

      public void Remove( HeaderCell value )
      {
        List.Remove( value );
      }

      public HeaderCell this[ int index ]
      {
        get
        {
          return (HeaderCell)InnerList[ index ];
        }
        set
        {
          List[ index ] = value;
        }
      }
      #endregion

      #region Class overrides
      protected override void OnInsertComplete( int index, object value )
      {
        base.OnInsertComplete( index, value );
        RaiseOnItemAdded( (HeaderCell)value );
      }

      protected override void OnRemoveComplete( int index, object value )
      {
        base.OnRemoveComplete( index, value );
        RaiseOnItemRemoved( (HeaderCell)value );
      }

      protected override void OnSetComplete( int index, object oldValue, object newValue )
      {
        base.OnSetComplete( index, oldValue, newValue );
        RaiseOnItemSet( (HeaderCell)newValue );
      }

      protected override void OnClearComplete()
      {
        RaiseOnClear();
      }

      protected void RaiseChanged( HeaderCell item )
      {
        if( Changed != null )
        {
          Changed( this, new HeaderCellEventArgs( item ) );
        }
      }

      protected void RaiseOnItemAdded( HeaderCell item )
      {
        if( OnItemAdded != null )
        {
          OnItemAdded( this, new HeaderCellEventArgs( item ) );
        }

        RaiseChanged( item );
      }

      protected void RaiseOnItemRemoved( HeaderCell item )
      {
        if( OnItemRemoved != null )
        {
          OnItemRemoved( this, new HeaderCellEventArgs( item ) );
        }

        RaiseChanged( item );
      }

      protected void RaiseOnItemSet( HeaderCell item )
      {
        if( OnItemSet != null )
        {
          OnItemSet( this, new HeaderCellEventArgs( item ) );
        }

        RaiseChanged( item );
      }

      protected void RaiseOnClear()
      {
        if( OnClearItems != null )
        {
          OnClearItems( this, EventArgs.Empty );
        }

        RaiseChanged( null );
      }
      
      #endregion
    }
    #endregion

    #region Class Properties
    /// <summary>
    /// Indicate to control does user want that control throw events on any property
    /// change or not.
    /// </summary>
    [ Browsable( false ), Description( "GET/SET flag: can coontrol throw events or not" ) ]
    public bool QuietMode
    {
      get
      {
        return m_bSkipEvents;
      }
      set
      {
        if( value != m_bSkipEvents )
        {
          m_bSkipEvents = value;
          OnQuietModeChanged();
        }
      }
    }

    /// <summary>
    /// Height in pixels of one level of heder
    /// </summary>
    [ Category( "Appearance" ), DefaultValue( DEF_HEADER_HEIGHT ) ]
    [ Browsable( true ), Description( "GET/SET height of header level" )]
    public int LevelHeight
    {
      get
      {
        return  m_iLevelHeight;
      }
      set
      {
        if( value != m_iLevelHeight )
        {
          m_iLevelHeight = value;
          OnLevelHeightChanged();
        }
      }
    }

    /// <summary>
    /// Indicate how many levels of header exists in control
    /// </summary>
    [ Category( "Appearance" ), DefaultValue( 1 ) ]
    [ Browsable( true ), Description( "GET/SET quantity of header levels in control" ) ]
    public int Levels
    {
      get
      {
        return m_iLevelsCount;
      }
      set
      {
        if( value != m_iLevelsCount )
        {
          m_iLevelsCount = value;
          OnLevelsChanged();
        }
      }
    }

    /// <summary>
    /// Retrun Collection of Header Cells 
    /// </summary>
    [ Browsable( false ) ]
    public HeaderCellCollection this[ int level ]
    {
      get
      {
        return ( HeaderCellCollection )m_headersColl[ level ];
      }
    }
    #endregion

    #region Class events
    [Category("Property Changed")]
    public event EventHandler LevelHeightChanged;
    
    [Category("Property Changed")]
    public event EventHandler LevelsChanged;
    #endregion

    #region Class Initialize/Finilize methods
    public HeaderControl()
    {
      ControlStyles styleTrue = 
        ControlStyles.AllPaintingInWmPaint |
        ControlStyles.DoubleBuffer | 
        ControlStyles.UserPaint;

      ControlStyles styleFalse = ControlStyles.Selectable;

      SetStyle( styleTrue, true );
      SetStyle( styleFalse, false ); 

      this.Name = "HeaderControl";
    }
    #endregion

    #region Class Overrides and event raisers
    protected void RaiseLevelHeightChanged()
    {
      if( LevelHeightChanged != null && m_bSkipEvents == false )
      {
        LevelHeightChanged( this, EventArgs.Empty );
      }
    }
    
    protected void RaiseLevelsChanged()
    {
      if( LevelsChanged != null && m_bSkipEvents == false )
      {
        LevelsChanged( this, EventArgs.Empty );
      }
    }

    
    protected virtual void OnLevelHeightChanged()
    {
    
    }
    
    protected virtual void OnLevelsChanged()
    {
    
    }
    
    protected virtual void OnQuietModeChanged()
    {
      // can be used only by inheritors
    }

    #endregion

    #region Paint methods
    protected override void OnPaint( PaintEventArgs e )
    {
      OnPaintBackground( e );
    }

    protected override void OnPaintBackground( PaintEventArgs pevent )
    {
      Graphics g = pevent.Graphics;
      Rectangle rc = pevent.ClipRectangle;

      g.FillRectangle( Brushes.Gray, rc ); 
    }
    #endregion
  }

}
