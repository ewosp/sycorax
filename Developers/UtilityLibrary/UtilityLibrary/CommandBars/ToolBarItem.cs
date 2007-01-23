using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

using UtilityLibrary.Win32;
using UtilityLibrary.Menus;
using UtilityLibrary.Collections;
using UtilityLibrary.General;


namespace UtilityLibrary.CommandBars
{
  #region Enumerations
  public enum ToolBarItemStyle
  {
    /// <summary>
    /// Toolbar Item has menu and which can be shown on DropDown press
    /// </summary>
    DropDownButton  = 0,
    /// <summary>
    /// Simple button
    /// </summary>
    PushButton      = 1,
    /// <summary>
    /// Separator
    /// </summary>
    Separator       = 2,
    /// <summary>
    /// Button which can contains Combobox as it's context
    /// </summary>
    ComboBox        = 3
  }
  #endregion
    
  /// <summary>
  /// Summary description for ToolBarItem.
  /// </summary>
  [ToolboxItem(false)]
  public class ToolBarItem : Component
  {
    #region Class Events
    [ Category( "Property Changed" ) ]
    public event EventHandler Changed;
    [ Category( "Action" ) ]
    public event EventHandler Click;
    [ Category( "Action" ) ]
    public event EventHandler DropDown;
    [ Category( "Property Changed" ) ]
    public event EventHandler ComboBoxChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler VisibleChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler TagChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler ImageChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler TextChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler EnabledChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler CheckedChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler ShortcutChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler StyleChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler ToolTipChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler ImageIndexChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler ImageListChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler ToolBarChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler HideTextChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler DroppedChanged;
    #endregion
    
    #region Class Variables
    /// <summary>
    /// Style kind of toolbar button. Default is PushButton style
    /// </summary>
    private ToolBarItemStyle      m_enStyle = ToolBarItemStyle.PushButton;
    /// <summary>
    /// Collection of menuitems which belong to toolbar item. In most cases
    /// used for DropDown Buttons.
    /// </summary>
    private MenuItemExCollection  m_arrItems;
    /// <summary>
    /// Context menu in which all m_arrItems collection items added.
    /// </summary>
    private CommandBarMenu        m_mnuSubMenu;
    /// <summary>
    /// Parent of Toolbar Item.
    /// </summary>
    private ToolBarEx   m_parent;
    /// <summary>
    /// Text shown by toolbar item. If m_bHideText is True then text only used
    /// by chevron menu, otherwise item text will be shown in toolbar item rectangle.
    /// </summary>
    private string      m_strText     = string.Empty;
    /// <summary>
    /// TRUE - shown text of toolbar item only in chevron menu only
    /// FALSE - shown text in chevron and on toolbar.
    /// </summary>
    private bool        m_bHideText   = true;
    /// <summary>
    /// TRUE - toolbar item is Enable, influence on GUI drawing of item
    /// </summary>
    private bool        m_bEnabled    = true;
    /// <summary>
    /// TRUE - item is checked. On GUI drawing as a button with select rectangle.
    /// </summary>
    private bool        m_bChecked;
    /// <summary>
    /// Item is visible or not.
    /// </summary>
    private bool        m_bVisible    = true;
    /// <summary>
    /// TRUE - context menu of toolbar item is shown now.
    /// FALSE - default state.
    /// </summary>
    private bool        m_bDropped;
    /// <summary>
    /// TRUE - all event raising code of class must be skipped. can be used for 
    /// batch control items update or etc.
    /// </summary>
    private bool        m_bSkipEvents;
    /// <summary>
    /// Shortcut keys which can be used by user to raise click event of toolbar item.
    /// </summary>
    private Keys        m_sCut        = Keys.None;
    /// <summary>
    /// ToolTip text which will be shown by toolbar
    /// </summary>
    private string      m_strToolTip  = string.Empty;
    /// <summary>
    /// User special data associated with current toolbar item.
    /// </summary>
    private object      m_tag;
    /// <summary>
    /// Reference to combobox control. Used when style of toolbar item is ComboBox.
    /// </summary>
    private ComboBox    m_ctrlCombo;
    /// <summary>
    /// Indicate position of toolbar item on toolbar
    /// </summary>
    private int         m_iIndex      = -1;       // ToolBar index
    /// <summary>
    /// Image which can be shown by toolbar item.
    /// </summary>
    private Image       m_imgIcon;
    /// <summary>
    /// Index of image from Image List
    /// </summary>
    private int         m_iImageIndex = -1;   // ImageList index
    /// <summary>
    /// Image List which can be used to select image for toolbar item.
    /// </summary>
    private ImageList   m_imgList; 

    internal TBBUTTONINFO m_cache;
    internal bool         m_bIsCached;
    internal Size         m_sizeText = Size.Empty;
    internal Rectangle    m_rect;
    internal bool         m_bRectCache;
    #endregion

    #region Class Constructors
    public ToolBarItem()
    {
      Initialize( null, null, null, Keys.None, null, -1 );
    }

    private void Initialize( Image image, string text, EventHandler clickHandler, 
      Keys shortCut, string toolTip, int imageListIndex )
    {
      m_imgIcon = image;
      m_strText = text;
      
      if( clickHandler != null )
      {
        this.Click += clickHandler;
      }
      
      m_sCut = shortCut;
      m_strToolTip = toolTip;
      
      m_arrItems   = new MenuItemExCollection();
      m_mnuSubMenu = new CommandBarMenu( m_arrItems, true );
      m_iImageIndex = imageListIndex;
    }
    #endregion

    #region Class Properties
    /// <summary>
    /// Say control to skip all event raisers methods and work in quiet mode
    /// TRUE - quiet mode is ON, otherwise quiet mode is OFF
    /// </summary>
    [ Browsable( false ), 
    Description( "GET/SET quiet mode of control. TRUE - quiet mode is ON." ) ]
    public bool QuietMode
    {
      set 
      { 
        m_bSkipEvents = value;
      }
      get 
      { 
        return m_bSkipEvents; 
      }
    }

    [ DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
    Category( "Behaviour" ),
    DefaultValue( null ), 
    Description( "GET Menu which will be dropped down when item have Style == DropDownButton" )]
    public MenuItemExCollection MenuItems
    {
      get 
      { 
        return m_arrItems; 
      }
    }

    [ Browsable( false ) ]
    public CommandBarMenu ToolBarItemMenu
    {
      get 
      {
        return m_mnuSubMenu; 
      }
    }

    [ Category( "Behaviour" ), 
    DefaultValue( null ), 
    Description( "GET/SET Combo which will be displayed on toolbar" ) ]
    public ComboBox ComboBox
    {
      set
      { 
        if( value != m_ctrlCombo )
        {
          IsCached = false;
          m_ctrlCombo = value; 
          RaiseComboBoxChanged();
        }
      }
      get
      { 
        return m_ctrlCombo; 
      }
    }
  
    [ Category( "Appearance" ), 
    DefaultValue( true ), 
    Description( "GET/SET is toolbar item Visible" ) ]
    public bool Visible
    {
      get
      { 
        return m_bVisible; 
      }
      set
      { 
        if( m_bVisible != value ) 
        { 
          IsCached = false;
          m_bVisible = value; 
          RaiseVisibleChanged();
        } 
      }
    }

    [ Category( "Data" ), 
    DefaultValue( null ), 
    Description( "GET/SET custom user data associated with this control" ) ]
    public object Tag
    {
      set 
      { 
        if( value != m_tag )
        {
          m_tag = value;
          RaiseTagChanged();
        }
      }
      get 
      { 
        return m_tag; 
      }
    }
  
    [ Category( "Appearance" ), 
    DefaultValue( null ), 
    Description( "GET/SET Image of toolbar item" ) ]
    public Image Image
    {
      set 
      { 
        if( m_imgIcon != value ) 
        { 
          IsCached = false;
          m_imgIcon = value; 
          RaiseImageChanged();
        }  
      }
      get 
      { 
        return m_imgIcon; 
      }
    }
    
    [ Category( "Appearance" ), 
    DefaultValue( "" ), 
    Description( "GET/SET text which can be displayed on toolbar item" ) ]
    public string Text
    {
      set 
      { 
        if( m_strText != value ) 
        { 
          IsCached = false;
          m_strText = value; 
          RaiseTextChanged(); 
        }
      }
      get 
      { 
        return m_strText; 
      }
    }
  
    [ Category( "Behaviour" ), 
    DefaultValue( true ), 
    Description( "GET/SET Is toolbar item enabled or disabled" ) ]
    public bool Enabled
    {
      set
      {
        if( m_bEnabled != value ) 
        { 
          IsCached = false;

          m_bEnabled = value;
          
          if( ComboBox != null )
          {
            ComboBox.Enabled = value;
          }

          RaiseEnableChanged(); 
        } 
      }
      get
      { 
        return m_bEnabled; 
      }
    }
  
    [ Category( "Appearance" ), 
    DefaultValue( false ), 
    Description( "GET/SET is toolbar checked/selected" ) ]
    public bool Checked
    {
      set
      { 
        if( m_bChecked != value ) 
        { 
          m_bChecked = value; 
          RaiseCheckedChanged(); 
        } 
      }
      get
      { 
        return m_bChecked; 
      }
    }

    [ Category( "Behaviour" ), 
    DefaultValue( typeof( Keys ), "None" ), 
    Description( "GET/SET Shortcut by which toolbar action can be raised by keyboard" ) ]
    public Keys Shortcut
    {
      set 
      { 
        if( m_sCut != value ) 
        { 
          m_sCut = value; 
          RaiseShortcutChanged(); 
        } 
      }
      get 
      { 
        return m_sCut; 
      }
    }
  
    [ Category( "Behaviour" ), 
    DefaultValue( typeof( ToolBarItemStyle ), "PushButton" ), 
    Description( "GET/SET Style of toolbar Item" ) ]
    public ToolBarItemStyle Style
    {
      set 
      { 
        if( m_enStyle != value ) 
        { 
          IsCached = false;
          m_enStyle = value; 
          RaiseStyleChanged(); 
        } 
      }
      get 
      { 
        return m_enStyle; 
      }
    }

    [ Category( "Appearance" ), 
    DefaultValue( "" ), 
    Description( "GET/SET Tooltip of toolbar item" ) ]
    public string ToolTip
    {
      set
      { 
        if( m_strToolTip != value ) 
        { 
          m_strToolTip = value; 
          RaiseToolTipChanged(); 
        } 
      }
      get 
      { 
        return m_strToolTip; 
      }
    }

    [ DefaultValue( -1 ), Browsable( false ) ]
    public int Index
    {
      set 
      { 
        IsCached = false;
        m_iIndex = value; 
      }
      get 
      { 
        return m_iIndex; 
      }
    }

    [ DefaultValue( false ), Browsable( false ) ]
    public bool Dropped
    {
      get 
      { 
        return m_bDropped; 
      }
      set 
      { 
        if( value != m_bDropped )
        {
          m_bDropped = value;
          OnDroppedChanged();
        }
      }
    }
    
    [ Category( "Appearance" ), 
    DefaultValue( -1 ),
    Description( "GET/SET toolbar item image from Image List" ),
    Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor)),
    TypeConverter( "System.Windows.Forms.ImageIndexConverter" ) ]
    public int ImageIndex
    {
      set 
      { 
        if( value != m_iImageIndex )
        {
          IsCached = false;
          m_iImageIndex = value; 
          RaiseImageIndexChanged();
        }
      }
      get 
      { 
        return m_iImageIndex; 
      }
    }

    [ Category( "Appearance" ), 
    Description( "GET/SET toolbar item Image List from which can be set item image" ),
    DefaultValue( null ) ]
    public ImageList ImageList
    {
      get
      {
        return m_imgList;
      }
      set
      {
        if( value != m_imgList )
        {
          IsCached = false;
          m_imgList = value;
          RaiseImageListChanged();
        }
      }
    }

    [ Browsable( false ) ]
    public Rectangle ItemRectangle
    {
      get
      {
        // toolBar object must have been setup right before
        // rendering the toolbar by the toolbar itself and for all items
        if( m_parent != null && m_bRectCache == false )
        {
          RECT rect = new RECT();
          WindowsAPI.SendMessage( m_parent.Handle, 
            (int)ToolBarMessages.TB_GETRECT, m_iIndex, ref rect );

          m_bRectCache = true;
          m_rect = (Rectangle)rect;
          return m_rect;
        }
        else if( m_bRectCache ) 
        {
          return m_rect;
        }
          
        return Rectangle.Empty;
      }
    }

    [ Category( "Behaviour" ), 
    DefaultValue( null ),  
    Description( "GET/SET toolbar item Parent" ) ]
    public ToolBarEx ToolBar
    {
      set
      { 
        if( value != m_parent )
        {
          IsCached = false;
          m_parent = value; 
          RaiseToolBarChanged();
          
          m_imgList = m_parent.ImageList;
        }
      }
      get
      {
        return m_parent;
      }
    }

    [ Category( "Appearance" ), 
    DefaultValue( true ), 
    Description( "GET/SET do we need text of toolbar item or not" ) ]
    public bool HideText
    {
      get
      { 
        return m_bHideText; 
      }
      set
      {
        if( value != m_bHideText )
        {
          IsCached = false;
          m_bHideText = value;
          RaiseHideTextChanged();
        }
      }
    }

    
    internal bool IsTextShown
    {
      get
      {
        return ( Text != null && Text != string.Empty && HideText == false );
      }
    }
    internal TBBUTTONINFO Cache
    {
      get
      {
        return m_cache;
      }
      set
      {
        m_cache = value;
      }
    }
    internal bool IsCached
    {
      get
      {
        return m_bIsCached;
      }
      set
      {
        if( value != m_bIsCached )
        {
          m_bIsCached = value;
          
          if( m_bIsCached == false )
          {
            m_bRectCache = false;
            TextSize = Size.Empty;
          }
        }
      }
    }
    internal Size TextSize
    {
      get
      {
        return m_sizeText;
      }
      set
      {
        m_sizeText = value;
      }
    }
    #endregion

    #region Event Raisers
    protected virtual void OnDroppedChanged()
    {
      RaiseDroppedChanged();
    }

    
    protected void RaiseChanged()
    {
      if( Changed != null && m_bSkipEvents == false ) 
      {
        Changed( this, EventArgs.Empty );
      }
    }

    protected void RaiseClick()
    {
      if( Click != null && m_bSkipEvents == false ) 
      {
        Click( this, EventArgs.Empty );
      }
    }
  
    protected void RaiseDropDown()
    {
      if( DropDown != null && m_bSkipEvents == false ) 
      {
        DropDown( this, EventArgs.Empty );
      }
    }

    
    protected void RaiseDroppedChanged()
    {
      if( DroppedChanged != null && m_bSkipEvents == false ) 
      {
        DroppedChanged( this, EventArgs.Empty );
      }

      if( m_bDropped == true )
      {
        RaiseDropDown();
      }
    }

    protected void RaiseComboBoxChanged()
    {
      if( ComboBoxChanged != null && m_bSkipEvents == false )
      {
        ComboBoxChanged( this, EventArgs.Empty );
      }

      RaiseChanged();
    }

    protected void RaiseVisibleChanged()
    {
      if( VisibleChanged != null && m_bSkipEvents == false )
      {
        VisibleChanged( this, EventArgs.Empty );
      }

      RaiseChanged();
    }

    protected void RaiseTagChanged()
    {
      if( TagChanged != null && m_bSkipEvents == false )
      {
        TagChanged( this, EventArgs.Empty );
      }

      RaiseChanged();
    }

    protected void RaiseImageChanged()
    {
      if( ImageChanged != null && m_bSkipEvents == false )
      {
        ImageChanged( this, EventArgs.Empty );
      }

      RaiseChanged();
    }

    protected void RaiseTextChanged()
    {
      if( TextChanged != null && m_bSkipEvents == false )
      {
        TextChanged( this, EventArgs.Empty );
      }

      RaiseChanged();
    }
    
    protected void RaiseEnableChanged()
    {
      if( EnabledChanged != null && m_bSkipEvents == false )
      {
        EnabledChanged( this, EventArgs.Empty );
      }

      RaiseChanged();
    }

    protected void RaiseCheckedChanged()
    {
      if( CheckedChanged != null && m_bSkipEvents == false )
      {
        CheckedChanged( this, EventArgs.Empty );
      }

      RaiseChanged();
    }

    protected void RaiseShortcutChanged()
    {
      if( ShortcutChanged != null && m_bSkipEvents == false )
      {
        ShortcutChanged( this, EventArgs.Empty );
      }

      RaiseChanged();
    }

    protected void RaiseStyleChanged()
    {
      if( StyleChanged != null && m_bSkipEvents == false )
      {
        StyleChanged( this, EventArgs.Empty );
      }

      RaiseChanged();
    }

    protected void RaiseToolTipChanged()
    {
      if( ToolTipChanged != null && m_bSkipEvents == false )
      {
        ToolTipChanged( this, EventArgs.Empty );
      }

      RaiseChanged();
    }

    protected void RaiseImageIndexChanged()
    {
      if( ImageIndexChanged != null && m_bSkipEvents == false )
      {
        ImageIndexChanged( this, EventArgs.Empty );
      }

      RaiseChanged();
    }

    protected void RaiseImageListChanged()
    {
      if( ImageListChanged != null && m_bSkipEvents == false )
      {
        ImageListChanged( this, EventArgs.Empty );
      }

      RaiseChanged();
    }

    protected void RaiseToolBarChanged()
    {
      if( ToolBarChanged != null && m_bSkipEvents == false )
      {
        ToolBarChanged( this, EventArgs.Empty );
      }

      RaiseChanged();
    }

    protected void RaiseHideTextChanged()
    {
      if( HideTextChanged != null && m_bSkipEvents == false )
      {
        HideTextChanged( this, EventArgs.Empty );
      }

      RaiseChanged();
    }
    #endregion

    #region Class External event Raiser
    internal protected virtual void FireClickEventHandler()
    {
      RaiseClick();
    }
    
    internal protected virtual void FireDropDownEventHandler()
    {
      RaiseDropDown();
    }
    #endregion

    #region Class Datatype convertor
    static public implicit operator TBBUTTONINFO( ToolBarItem item )
    {
      if( item.IsCached ) return item.Cache;

      const int MARGIN = 3;
      const int DROWPDOWN_ARROW_WIDTH = 14;
      const int MENUTEXT_MARGIN = 8;

      TBBUTTONINFO tbi = item.Cache;
      
      tbi.cbSize = Marshal.SizeOf( typeof( TBBUTTONINFO ) );
      tbi.dwMask = ( int )( ToolBarButtonInfoFlags.TBIF_IMAGE | 
        ToolBarButtonInfoFlags.TBIF_STATE | ToolBarButtonInfoFlags.TBIF_STYLE | 
        ToolBarButtonInfoFlags.TBIF_COMMAND | ToolBarButtonInfoFlags.TBIF_SIZE );
      
      tbi.idCommand = item.Index;      
      tbi.iImage    = (int)ToolBarButtonInfoFlags.I_IMAGECALLBACK;
      tbi.fsState   = 0;
      tbi.cx        = 0;
      tbi.lParam    = IntPtr.Zero;
      tbi.pszText   = IntPtr.Zero;
      tbi.cchText   = 0;
      
      if( item.Style == ToolBarItemStyle.ComboBox )
      {
        if( item.ComboBox != null && !item.ComboBox.IsDisposed )
        {
          tbi.fsStyle = (int)ToolBarButtonStyles.TBSTYLE_BUTTON;
          tbi.cx = MARGIN;
          tbi.cx += (short)item.ComboBox.Width;
          
          WindowsAPI.SetParent( item.ComboBox.Handle, item.m_parent.Handle );
          WindowsAPI.ShowWindow( item.ComboBox.Handle, (short)ShowWindowStyles.SW_SHOWNOACTIVATE );
        }
      }
      else if( item.IsTextShown == true )
      {
        tbi.fsStyle = (int)( ToolBarButtonStyles.TBSTYLE_BUTTON );
        tbi.cx = MARGIN;
        
        if( item.ImageList != null && item.ImageIndex != -1 && 
          item.ImageIndex < item.ImageList.Images.Count )
        {
          tbi.cx += ( short )( item.ImageList.ImageSize.Width + MARGIN );
        }
        else if( item.Image != null )
        {
          tbi.cx += ( short )( item.Image.Size.Width + MARGIN );
        }
        
        if( item.Style == ToolBarItemStyle.DropDownButton )
          tbi.cx += DROWPDOWN_ARROW_WIDTH;
        
        if( item.TextSize == Size.Empty )
          item.TextSize = TextUtil.MeasureText( item.Text, SystemInformation.MenuFont, 0 );

        tbi.cx += ( short )( item.TextSize.Width + 2 * 
          ( ( item.m_parent.BarType == TBarType.MenuBar ) ? MENUTEXT_MARGIN : MARGIN ) );
        
        tbi.dwMask |= (int)ToolBarButtonInfoFlags.TBIF_TEXT;
        tbi.pszText = Marshal.StringToHGlobalAuto(item.Text + "\0");
        tbi.cchText = item.Text.Length;

        if( ToolBarEx.IsCommonCtrl6() && item.m_parent.BarType != TBarType.MenuBar )
        {
          tbi.cx += 6;
        }
      }
      else 
      {
        tbi.fsStyle = (int)(ToolBarButtonStyles.TBSTYLE_BUTTON | ToolBarButtonStyles.TBSTYLE_AUTOSIZE );
        tbi.cx = 0;
      }

      if( !item.Visible )
      {
        tbi.fsState |= (int)ToolBarButtonStates.TBSTATE_HIDDEN;
      }
            
      if( item.Style == ToolBarItemStyle.Separator )
      {
        tbi.fsStyle |= (int)ToolBarButtonStyles.TBSTYLE_SEP;
        tbi.iImage = (int)ToolBarButtonInfoFlags.I_IMAGENONE;
      }     
      else
      {
        if( item.Enabled )
        {
          tbi.fsState |= (int)ToolBarButtonStates.TBSTATE_ENABLED;
        }

        if( item.Style == ToolBarItemStyle.DropDownButton )
        {
          tbi.fsStyle |= (int)ToolBarButtonStyles.TBSTYLE_DROPDOWN;
        }

        if( item.Style == ToolBarItemStyle.PushButton && item.Checked )
        {
          tbi.fsState |= (int)ToolBarButtonStates.TBSTATE_CHECKED;
        }
        
        if( item.Image != null || item.ImageList != null )
        {
          tbi.iImage = item.Index;
        }
      }

      item.Cache = tbi;
      item.IsCached = true;

      return item.Cache;       
    }
    #endregion
  }
}
