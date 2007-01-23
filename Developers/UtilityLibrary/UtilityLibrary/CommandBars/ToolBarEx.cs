#region Using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using UtilityLibrary.Collections;
using UtilityLibrary.General;
using UtilityLibrary.Menus;
using UtilityLibrary.Win32;
using UtilityLibrary.WinControls;
#endregion

namespace UtilityLibrary.CommandBars
{
  #region Enumerations
  public enum TBarType
  {
    ToolBar = 0,
    MenuBar = 1
  }
  #endregion

  #region Interfaces
  public interface IChevron
  {
    void Show( Control control, Point point );
  }
  #endregion
  
  [ ToolboxItem( false ) ]
  [ Designer( "UtilityLibrary.Designers.ToolBarExDesigner, UtilityLibrary.Designers, Version=1.0.4.0, Culture=neutral, PublicKeyToken=fe06e967d3cf723d"  ) ]
  public class ToolBarEx : System.Windows.Forms.Control, IChevron
  {
    #region Enumerations
    // To be used only when we have a menubar type
    enum TBarState 
    { 
      None, 
      Hot, 
      HotTracking 
    }
    #endregion
  
    #region Class constants
    private const int DROWPDOWN_ARROW_WIDTH = 14;
    private const int MARGIN = 3;
    private const int MENUTEXT_MARGIN = 8;
    #endregion

    #region Class Variables
    private ToolBarItemCollection m_items;
    private ToolBarItem[]         handledItems = new ToolBarItem[0];
    private bool[]                handledItemsVisible = new bool[0];
    private ChevronMenu           chevronMenu = new ChevronMenu();
    private bool                  m_bShowChevron;
    private TBarType              m_enBarType;
    private ImageList             imageListHelper;
    private ImageList             m_imgList;
    private TBarState             state = TBarState.None;
    private TBarState             lastState = TBarState.None;
    private Point                 m_pntLastMouse = Point.Empty;
    private int                   trackHotItem      = -1;
    private int                   trackNextItem     = -1;
    private bool                  trackEscapePressed = false;
    private IntPtr                hookHandle        = IntPtr.Zero;
    private bool                  doKeyboardSelect  = false;
    private bool                  m_bUseNewRow = true;
    
    internal ReBar                m_parent;
    internal ToolBarItem          placeHolderToolBarItem;
    internal bool                 placeHolderAdded;

    // used for check DLL Version 
    static private bool                  bGotIsCommonCtrl6;
    static private bool                  isCommonCtrl6;
    #endregion

    #region Class Events
    [ Category( "Property Changed" ) ]
    public event EventHandler ImageListChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler BarTypeChanged;
    #endregion

    #region Class Properties
    [ Browsable( false ), DefaultValue( "" ) ]
    public override string Text 
    { 
      get 
      { 
        return string.Empty; 
      }
      set
      {
        base.Text = string.Empty;
      }
    }

    [ DefaultValue( true ), Browsable( false ) ]
    public bool       UseNewRow
    {
      get 
      { 
        return m_bUseNewRow; 
      }
    }
      
    [ DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
    Category( "Behaviour" ), 
    Description( "GET toolbar m_items collection" ) ]
    public ToolBarItemCollection Items
    {
      get
      { 
        return m_items; 
      }
    }

    [ Category( "Behaviour" ), 
    DefaultValue( typeof( TBarType ), "ToolBar" ), 
    Description( "GET/SET How bar will act on form" ) ]
    public TBarType   BarType
    {
      set 
      { 
        if( value != m_enBarType )
        {
          m_enBarType = value; 
          OnBarTypeChanged();
        }
      }
      get 
      { 
        return m_enBarType; 
      }
    }
    
    [ Category( "Appearance" ), 
    DefaultValue( null ), 
    Description( "GET/SET Image List for Bar m_items" ) ]
    public ImageList  ImageList
    {
      set 
      { 
        if( value != m_imgList )
        {
          m_imgList = value;
          OnImageListChanged();
        }
      }
      get 
      { 
        return m_imgList; 
      }
    }

    [DefaultValue( false )]
    internal bool     PlaceHolderAdded
    {
      set 
      { 
        placeHolderAdded = value; 
      }
      get 
      { 
        return placeHolderAdded; 
      }
    }


    internal int      HotItemIndex
    {
      get
      {
        return WindowsAPI.SendMessage( Handle, ( int )ToolBarMessages.TB_GETHOTITEM, 0, 0 );
      }
      set
      {
        WindowsAPI.SendMessage( Handle, ( int )ToolBarMessages.TB_SETHOTITEM, value, 0 );
      }
    }
    #endregion

    #region Class Initialize/Finilize methods
    public ToolBarEx()
    {
      InitializeToolBar();
    }

    public ToolBarEx( bool m_bUseNewRow ) : this()
    {
      this.m_bUseNewRow = m_bUseNewRow;
    }

    public ToolBarEx( TBarType type ) : this()
    {
      m_enBarType = type;
    }

    public ToolBarEx( TBarType type, bool m_bUseNewRow ) : this()
    {
      m_enBarType = type;
      this.m_bUseNewRow = m_bUseNewRow;
    }

    
    private void InitializeToolBar()
    {
      m_items = new ToolBarItemCollection( this );
    
      // We'll let the toolbar to send us messages for drawing
      SetStyle( ControlStyles.UserPaint, false );
      
      TabStop = false;
      
      // Always on top
      Dock = DockStyle.Top;
      Attach();
    }

    ~ToolBarEx()
    {
      Detach();
    }
    #endregion
    
    #region Class Overrides
    protected override void CreateHandle() 
    {
      // Make sure common control library initilizes toolbars and rebars
      if( !RecreatingHandle )
      {
        INITCOMMONCONTROLSEX icex = new INITCOMMONCONTROLSEX();
        icex.dwSize = Marshal.SizeOf( typeof( INITCOMMONCONTROLSEX ) );
        icex.dwICC = ( int )( CommonControlInitFlags.ICC_BAR_CLASSES | CommonControlInitFlags.ICC_COOL_CLASSES );
        
        WindowsAPI.InitCommonControlsEx( icex );
      }
      
      // Handle is being created or recreated, reset placeHolder flag
      placeHolderAdded = false;
      
      base.CreateHandle();
    }
    
    protected override CreateParams CreateParams
    {
      get
      {
        CreateParams createParams = base.CreateParams;
        createParams.ClassName = WindowsAPI.TOOLBARCLASSNAME;
        createParams.ExStyle = 0;
        
        // Windows specific flags
        createParams.Style = ( int )(WindowStyles.WS_CHILD | 
          WindowStyles.WS_VISIBLE |
          WindowStyles.WS_CLIPCHILDREN | 
          WindowStyles.WS_CLIPSIBLINGS);

        // Common Control specific flags
        createParams.Style |= ( int )(CommonControlStyles.CCS_NODIVIDER | 
          CommonControlStyles.CCS_NORESIZE | 
          CommonControlStyles.CCS_NOPARENTALIGN);

        // ToolBar specific flags
        createParams.Style |= ( int )(ToolBarStyles.TBSTYLE_TOOLTIPS | 
          ToolBarStyles.TBSTYLE_FLAT | 
          ToolBarStyles.TBSTYLE_TRANSPARENT);

        if( HasText() ) 
        {
          createParams.Style |= ( int )ToolBarStyles.TBSTYLE_LIST;
        }
        
        return createParams;
      }
    }

    protected override Size DefaultSize
    {
      get 
      { 
        return new Size( 1, 1 ); 
      }
    }

    protected override void OnHandleCreated( EventArgs e )
    {
      // Send message needed for the toolbar to work properly before any other messages are sent
      WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_BUTTONSTRUCTSIZE, Marshal.SizeOf(typeof( TBBUTTON )), 0);
      
      // Setup extended styles
      int extendedStyle = ( int )(ToolBarExStyles.TBSTYLE_EX_HIDECLIPPEDBUTTONS | 
        ToolBarExStyles.TBSTYLE_EX_DOUBLEBUFFER );
      
      if( BarType == TBarType.ToolBar ) 
        extendedStyle |= ( int )ToolBarExStyles.TBSTYLE_EX_DRAWDDARROWS;

      WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_SETEXTENDEDSTYLE, 0, extendedStyle);
      RealizeItems();

      // Add Temporary place holder item so that the ToolBarEx has a height
      // when it is first inserted in the IDE designer, otherwise, it does not look right
      // -- only if there if there is a designer involved --
      if( m_parent.RebarDesigner != null && placeHolderAdded == false && m_items.Count == 0 )
      {
        AddPlaceHolderToolBarItem();
      }

      base.OnHandleCreated( e );
    }

    protected override void OnMouseMove( MouseEventArgs e )
    {
      if( m_enBarType == TBarType.MenuBar )
      {
        Point point = new Point( e.X, e.Y );
        
        if( state == TBarState.Hot )
        {
          int index = HitTest( point );
          if( ( IsValid( index ) ) && ( point != m_pntLastMouse) )
            HotItemIndex = index ;
          
          return;
        }
        
        m_pntLastMouse = point;
      }
      
      base.OnMouseMove( e );
    }

    protected override void OnMouseDown( MouseEventArgs e )
    {
      /*
      if( m_enBarType == TBarType.MenuBar)
      {
        if( ( e.Button == MouseButtons.Left ) && ( e.Clicks == 1 ) )
        {
          Point point = new Point( e.X, e.Y );
          int index = HitTest( point );
          
          if( IsValid( index ) )
          {
            TrackDropDown( index );
            return;
          }
        }
      }
      */
      base.OnMouseDown( e );
    }
    
    protected override void OnMouseUp( System.Windows.Forms.MouseEventArgs e )
    {
      if( m_enBarType == TBarType.ToolBar )
      {
        Point pnt = Control.MousePosition;
        int index = GetItemAtPoint( this.PointToClient( pnt ) );
        
        if( index >= 0 )
          UpdateItem( index );
      }

      base.OnMouseUp( e );
    }

    protected override void WndProc( ref Message message )
    {
      base.WndProc( ref message );
           
      int index = -1;
      ToolBarItem item = null;

      switch ( message.Msg )
      {
        case ( int )ReflectedMessages.OCM_COMMAND:
          index = ( int ) message.WParam & 0xFFFF;
          if( m_items.Count != 0 )
          {
            item = m_items[index];
            item.FireClickEventHandler();
            base.WndProc( ref message );
            ResetMouseEventArgs();
          }
          break;

        case ( int )Msg.WM_MENUCHAR:
          MenuChar( ref message );
          break;

        case ( int )Msg.WM_ERASEBKGND:
          break;

        case ( int )Msg.WM_LBUTTONDOWN:
          // Pass it down to the parent Rebar so that
          // the rebar can select itself
          if( m_parent.Bands.Count == 0 && m_parent.RebarDesigner != null )
          {
            // If the count for the RebarBand collections is zero
            // but we still get this messages, it means that it is the
            // "place holder" toolbar the one generating this message
            Point pt = new Point(message.LParam.ToInt32());
            pt = PointToScreen( pt );
            m_parent.PointToClient( pt );
            m_parent.RebarDesigner.PassMsg( ref message );
          }
          break;

        case ( int )Msg.WM_NOTIFY:
        case ( int )ReflectedMessages.OCM_NOTIFY:
          NMHDR nm = ( NMHDR ) message.GetLParam(typeof( NMHDR ));
        switch ( nm.code )
        {
          case ( int )ToolBarNotifications.TTN_NEEDTEXTA:
            NotifyNeedTextA( ref message );
            break;
      
          case ( int )ToolBarNotifications.TTN_NEEDTEXTW:
            NotifyNeedTextW( ref message );
            break;        
      
          case ( int )ToolBarNotifications.TBN_QUERYINSERT:
            message.Result = ( IntPtr ) 1;
            break;
      
          case ( int )ToolBarNotifications.TBN_DROPDOWN:
            NMTOOLBAR nmt = ( NMTOOLBAR ) message.GetLParam(typeof( NMTOOLBAR ));
            index = nmt.iItem;
            if( m_items.Count != 0 )
            {
              item = m_items[index];
              item.Dropped = true;
            }
            break;
                
          case ( int )NotificationMessages.NM_CUSTOMDRAW:
            NotifyCustomDraw( ref message );
            break;

          case ( int )ToolBarNotifications.TBN_HOTITEMCHANGE:
            break;
        }
          break;
      }
    }

    protected override void OnFontChanged( EventArgs e ) 
    {
      base.OnFontChanged( e );
      UpdateItems();
    }

    public    override bool PreProcessMessage( ref Message message )
    {
      if( message.Msg == ( int )Msg.WM_KEYDOWN || message.Msg == ( int )Msg.WM_SYSKEYDOWN )
      {
        // Check for shortcuts in ToolBarItems in this toolbar
        Keys keys = ( Keys )( int ) message.WParam  | ModifierKeys;
        if( m_items.Count == 0 ) return true;
        
        ToolBarItem shortcutHit = m_items[keys];
        if( shortcutHit != null && shortcutHit.Enabled  )
        {
          shortcutHit.FireClickEventHandler();
          return true;
        }

        // Check for shortcuts in the menuitems of the popup menu
        // currently being displayed
        if( m_enBarType == TBarType.MenuBar )
        {
          MenuItem hitItem = null;
          
          foreach ( ToolBarItem tbi in m_items ) 
          {
            hitItem = FindShortcutItem( tbi.MenuItems, keys );
            
            if( hitItem != null) break;
          }
          
          if( hitItem != null ) hitItem.PerformClick();
        }
  
        //  Check if we have a mnemonic
        bool alt = (( keys & Keys.Alt ) != 0);
        if( alt )
        {
          Keys keyCode = keys & Keys.KeyCode;
          char key = ( char )( int )keyCode;
          
          if( ( Char.IsDigit( key ) || (Char.IsLetter( key )) ) )
          {
            ToolBarItem mnemonicsHit = m_items[key];        
            
            if( mnemonicsHit != null ) 
            {
              if( m_enBarType == TBarType.MenuBar )
                TrackDropDown( mnemonicsHit.Index );
              else
                mnemonicsHit.FireClickEventHandler();
              
              return true;
            }
          }
        }
      }
      
      return false;
    }
    #endregion

    #region Event Raisers
    internal void RaiseImageListChangedEvent()
    {
      if( ImageListChanged != null )
      {
        ImageListChanged( this, EventArgs.Empty );
      }
    }

    internal void RaiseBarTypeChangedEvent()
    {
      if( BarTypeChanged != null )
      {
        BarTypeChanged( this, EventArgs.Empty );
      }
    }

    
    protected virtual void OnImageListChanged()
    {
      foreach( ToolBarItem item in m_items )
      {
        item.QuietMode = true;
        item.ImageList = m_imgList;
        item.QuietMode = false;
      }

      RaiseImageListChangedEvent();
    }

    protected virtual void OnBarTypeChanged()
    {
      RaiseBarTypeChangedEvent();
    }

    #endregion

    #region Class Methods
    static public bool IsCommonCtrl6()
    {
      // Cache this value for efficenty
      if( bGotIsCommonCtrl6 == false )
      {     
        DLLVERSIONINFO dllVersion = new DLLVERSIONINFO();
        
        // We are assummng here that anything greater or equal than 6
        // will have the new XP theme drawing enable
        dllVersion.cbSize = Marshal.SizeOf(typeof( DLLVERSIONINFO ));
        WindowsAPI.GetCommonControlDLLVersion( ref dllVersion );
        bGotIsCommonCtrl6 = true;
        isCommonCtrl6 = ( dllVersion.dwMajorVersion >= 6 );
      }
      
      return isCommonCtrl6;
    }

    public void UpdateToolBarItems()
    { 
      UpdateImageList();  

      for ( int i = 0; i < m_items.Count; i++ )
      {
        // The toolbar handle is going to be destroy to correctly update the toolbar 
        // itself. We need to detach the toolbar as the parent of the comboboxes -- 
        // otherwise the comboboxes does not behave appropiately after we pull the 
        // plug on the parent The combobox will again parented to the toolbar once 
        // the handle is recreated and when the RealizeItem routine gets the 
        // information for the toolbaritems
        if( m_items[i].Style == ToolBarItemStyle.ComboBox && m_items[i].ComboBox != null )
        {
          WindowsAPI.ShowWindow( m_items[i].ComboBox.Handle, ( short )ShowWindowStyles.SW_HIDE );
          WindowsAPI.SetParent( m_items[i].ComboBox.Handle, IntPtr.Zero );
          m_items[i].ComboBox.Parent = null;
        }
      }
      
      UpdateItems();
    }

    public void BeginUpdate()
    {
      WindowsAPI.SendMessage(Handle, ( int )Msg.WM_SETREDRAW, 0, 0);  
    }
  
    public void EndUpdate()
    {
      WindowsAPI.SendMessage(Handle, ( int )Msg.WM_SETREDRAW, 1, 0);
    }

    #endregion

    #region Implementation
    internal void AddPlaceHolderToolBarItem()
    {
      placeHolderAdded = true;
      
      // Add it to the "native" ToolBar but not to the collection
      ToolBarItem item = new ToolBarItem();
      item.ToolBar = this;
      TBBUTTON button = new TBBUTTON();
      button.idCommand = 0;
      WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_INSERTBUTTON, 0, ref button);
  
      int pos = m_items.IndexOf( item );
      TBBUTTONINFO tbi = (TBBUTTONINFO)m_items[ pos ];   //GetButtonInfo( 0, item );
      WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_SETBUTTONINFOW, 0, ref tbi);

      UpdateSize();
    }

    internal void RemovePlaceHolderToolBarItem()
    {
      placeHolderAdded = false;
      int result = WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_DELETEBUTTON, 0, 0);
    }

    
    internal void RemoveToolBarItem( int index )
    {
      int result = WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_DELETEBUTTON, index, 0);
    }

    private  void Attach()
    {
      m_items.Changed += new EventHandler( Items_Changed );
      
      int count = Items.Count;
      handledItems = new ToolBarItem[count];
      handledItemsVisible = new bool[count];

      for( int i = 0; i < count; i++ )
      {
        ToolBarItem item = Items[i];
        item.Changed += new EventHandler( Item_Changed );
        handledItems[i] = item;
        handledItemsVisible[i] = item.Visible;
      }
    }

    private  void Detach()
    {
      foreach ( ToolBarItem item in handledItems )
      {
        item.Changed -= new EventHandler( Item_Changed );
      }
      
      handledItems = null;
      handledItemsVisible = null;
      m_items.Changed -= new EventHandler( Items_Changed );
    }

    private  bool HasText()
    {
      if( m_items != null )
      {
        for( int i = 0; i < m_items.Count; i++ )
        {
          if( m_items[i].IsTextShown == true ) return true;
        }
      }

      return false;
    }
    
    private  bool IsValid( int index )
    {
      int count = WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_BUTTONCOUNT, 0, 0);
      return (( index >= 0 ) && ( index < count ));
    }

    private  int  HitTest( Point point )
    {
      POINT pt = (POINT)point;
      
      int hit = WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_HITTEST, 0, ref pt);
      
      if( hit > 0 )
      {
        point = PointToScreen( point );
        Rectangle bounds = RectangleToScreen(new Rectangle( 0, 0, Width, Height ));
        if( !bounds.Contains( point ) ) return -1;
      }
      
      return hit;
    }

    private  int  GetNextItem( int index )
    {
      if( index == -1 ) throw new Exception();
      int count = WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_BUTTONCOUNT, 0, 0);
      index++;
      if( index >= count ) index = 0;
      return index;
    }
      
    private  int  GetPreviousItem( int index )
    {
      if( index == -1 ) throw new Exception();
      int count = WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_BUTTONCOUNT, 0, 0);
      index--;
      if( index < 0 ) index = count - 1;
      return index;
    }

    private  void TrackDropDown( int index )
    {
      while( index >= 0 )
      {
        trackNextItem = -1;
    
        BeginUpdate();

        // Raise event
        ToolBarItem item = ( ToolBarItem )m_items[index];
        item.FireDropDownEventHandler();
        
        // Item state
        WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_PRESSBUTTON, index, -1);

        // Trick to get the first menu item selected
        if( doKeyboardSelect )
        {
          WindowsAPI.PostMessage(Handle, ( int )Msg.WM_KEYDOWN, ( int ) Keys.Down, 1);
          WindowsAPI.PostMessage(Handle, ( int )Msg.WM_KEYUP, ( int ) Keys.Down, 1);
        }
        
        doKeyboardSelect = false;
        SetState( TBarState.HotTracking, index );

        // Hook
        WindowsAPI.HookProc hookProc = new WindowsAPI.HookProc( DropDownHook );
        GCHandle hookProcHandle = GCHandle.Alloc( hookProc );
        hookHandle = WindowsAPI.SetWindowsHookEx(( int )WindowsHookCodes.WH_MSGFILTER, 
          hookProc, IntPtr.Zero, WindowsAPI.GetCurrentThreadId());
        if( hookHandle == IntPtr.Zero ) throw new SecurityException();

        // Ask for position
        RECT rect = new RECT();
        WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_GETRECT, index, ref rect);
        Point position = new Point( rect.left, rect.bottom-1 );
        
        EndUpdate();
        Update();
        
        CommandBarMenu menu = item.ToolBarItemMenu;
        if( menu != null )
        {
          menu.Show( this, position );
        }
                
        // Unhook   
        WindowsAPI.UnhookWindowsHookEx( hookHandle );
        hookProcHandle.Free();
        hookHandle = IntPtr.Zero;

        // Item state
        WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_PRESSBUTTON, index, 0);
        SetState( trackEscapePressed ? TBarState.Hot : TBarState.None, index );

        index = trackNextItem;
      }
    }

    private  void TrackDropDownNext( int index )
    {
      if( index != trackHotItem )
      {
        WindowsAPI.PostMessage(Handle, ( int )Msg.WM_CANCELMODE, 0, 0);
        trackNextItem = index;
      }
    }

    private  IntPtr DropDownHook( int code, IntPtr wparam, IntPtr lparam ) 
    {
      if(code == ( int )MouseHookFilters.MSGF_MENU)
      {
        MSG msg = ( MSG ) Marshal.PtrToStructure(lparam, typeof( MSG ));
        Message message = Message.Create( msg.hwnd, msg.message, msg.wParam, msg.lParam );
        if( DropDownFilter( ref message ) )
          return ( IntPtr ) 1;
      }
      return WindowsAPI.CallNextHookEx( hookHandle, code, wparam, lparam );
    }

    private  bool DropDownFilter( ref Message message )
    {
      if( state != TBarState.HotTracking ) throw new Exception();

      // comctl32 sometimes steals the hot item for unknown reasons.
      HotItemIndex = trackHotItem;

      if(message.Msg == ( int )Msg.WM_KEYDOWN)
      {
        Keys keyData = ( Keys )( int ) message.WParam | ModifierKeys;
        if( keyData == Keys.Left || keyData == Keys.Right )
          doKeyboardSelect = true;
                
        if( keyData == Keys.Left )
        {
          TrackDropDownNext(GetPreviousItem( trackHotItem ));
          return true;
        }

        // Only move right if there is no submenu on the current selected item.
        ToolBarItem item = m_items[trackHotItem];
        if(( keyData == Keys.Right ) && (( item.ToolBarItemMenu.SelectedMenuItem == null ) 
          || ( item.ToolBarItemMenu.SelectedMenuItem.MenuItems.Count == 0 )))
        {
          TrackDropDownNext(GetNextItem( trackHotItem ));
          return true;
        }

        if( keyData == Keys.Escape )
        {
          trackEscapePressed = true;
        }
      }
      else if((message.Msg == ( int )Msg.WM_MOUSEMOVE) || (message.Msg == ( int )Msg.WM_LBUTTONDOWN))
      {
        Point point = WindowsAPI.GetPointFromLPARAM(( int )message.LParam);
        point = this.PointToClient( point );

        if(message.Msg == ( int )Msg.WM_MOUSEMOVE)
        {
          if( point != m_pntLastMouse )
          {
            int index = HitTest( point );
            
            if((IsValid( index )) && ( index != trackHotItem ))
              TrackDropDownNext( index );
            
            m_pntLastMouse = point;
          }
        }
        else if(message.Msg == ( int )Msg.WM_LBUTTONDOWN)
        {
          if(HitTest( point ) == trackHotItem)
          {
            TrackDropDownNext( -1 );
            return true;
          }
        }
      }
  
      return false;
    }

    private  void SetState( TBarState state, int index )
    {
      if( this.state != state )
      {
        if( state == TBarState.None )
          index = -1;
          
        HotItemIndex = index;
        
        if( state == TBarState.HotTracking )
        {
          trackEscapePressed = false;
          trackHotItem = index;
        }
      }
      
      this.lastState = this.state;
      this.state = state;
    }
    
    void IChevron.Show( Control control, Point point )
    {
      if( m_bShowChevron )
      {
        // Don't try to show it again
        // if we are showing it already
        return;
      }
            
      m_bShowChevron = true;
      ToolBarItemCollection chevronItems = new ToolBarItemCollection();
      ToolBarItem   lastItem;
      Size          size = ClientSize;
      int           currentCount = 0;
      bool          addItem = true;
      bool          hasComboBox = false;

      for( int i = 0; i < m_items.Count; i++ )
      {
        bool IsSeparator = false;
        
        RECT rect = new RECT();
        WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_GETITEMRECT, i, ref rect);

        if( rect.right > size.Width )
        {
          ToolBarItem item = m_items[i];
          if( item.ComboBox != null ) hasComboBox = true;

          IsSeparator = ( item.Style == ToolBarItemStyle.Separator );
          
          if( item.Visible ) 
          {
            if( ( !IsSeparator  ) || ( chevronItems.Count != 0 ) )
            {
              // don't add it if previous item was a separator
              currentCount = chevronItems.Count;
              
              if( currentCount > 0 ) 
              {
                lastItem = chevronItems[currentCount-1];
                if( lastItem.Style == ToolBarItemStyle.Separator && IsSeparator )
                {
                  addItem = false;
                }
              }

              if( addItem ) chevronItems.Add( item );
              addItem = true;
            }
          }
        }
      }

      // Don't show a separator as the last item of the context menu
      int itemsCount = chevronItems.Count;
      
      if( itemsCount > 0 )
      {
        lastItem = chevronItems[itemsCount-1];
        if( lastItem.Style == ToolBarItemStyle.Separator )
        {
          chevronItems.RemoveAt( itemsCount-1 );
        }
      }
      
      chevronMenu.Items = chevronItems;
      chevronMenu.Style = VisualStyle.IDE;
      chevronMenu.TrackPopup(control.PointToScreen( point ));

      // Need to reparent the combobox to this toolbar in case
      // there was a combobox that was displayed by the popup menu
      if( hasComboBox )
      {
        // Run the logic for combobox visibility before reposition it
        ToolbarSizeChanged( new NMREBARCHILDSIZE() );      

        for ( int i = 0; i < m_items.Count; i++ )
        {
          ToolBarItem item = m_items[i];
          
          if( item.Style == ToolBarItemStyle.ComboBox && item.ComboBox != null )
          {
            WindowsAPI.SetParent( item.ComboBox.Handle, Handle );
            WindowsAPI.ShowWindow( item.ComboBox.Handle, ( short )ShowWindowStyles.SW_SHOWNOACTIVATE );
            ComboBoxBase cbb = ( ComboBoxBase )item.ComboBox;
            cbb.ToolBarUse = true;
            UpdateItem( i );
            cbb.Invalidate();
          }
        }
      }

      m_bShowChevron = false;
    }

    internal void ToolbarSizeChanged( NMREBARCHILDSIZE sizeRebar )
    {
      if( BarType == TBarType.MenuBar ) return;
      
      // Make sure that comboboxes are either visible
      // or invisible depending wheater they are showing
      // all of the client area or they are partially hidden
      Size size = ClientSize;
      if( sizeRebar.hdr.hwndFrom != IntPtr.Zero )
      {
        size = (( Rectangle )sizeRebar.rcBand).Size;
      }

      for ( int i = 0; i < m_items.Count; i++ )
      {
        ToolBarItem item = m_items[i];
        
        if( item.Style == ToolBarItemStyle.ComboBox && item.ComboBox != null )
        {
          RECT rect = new RECT();
          WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_GETITEMRECT, i, ref rect);
          
          item.ComboBox.Visible = !( rect.right > size.Width );
        }
      }
    }

    private  void MenuChar( ref Message message )
    {
      ToolBarItem item = m_items[trackHotItem];
      Menu menu = item.ToolBarItemMenu.FindMenuItem( MenuItem.FindHandle, message.LParam );
      if( item.ToolBarItemMenu.Handle == message.LParam ) menu = item.ToolBarItemMenu;

      if( menu != null )
      {
        char key = char.ToUpper(( char ) (( int ) message.WParam & 0x0000FFFF));
        int index = 0;
        
        foreach ( MenuItem menuItem in menu.MenuItems )
        {
          if(( menuItem != null ) && ( menuItem.OwnerDraw ) && ( menuItem.Mnemonic == key ))
          {
            message.Result = ( IntPtr ) ((( int )MenuCharReturnValues.MNC_EXECUTE << 16) | index);
            return;
          }
          
          if( menuItem.Visible ) index++;
        }
      }
    }

    private  void NotifyNeedTextA( ref Message m )
    {
      TOOLTIPTEXTA ttt = ( TOOLTIPTEXTA ) m.GetLParam(typeof( TOOLTIPTEXTA ));
      if( m_items.Count == 0 ) return;

      ToolBarItem item = ( ToolBarItem ) m_items[ttt.hdr.idFrom];
      string toolTip = item.ToolTip;
      
      if( toolTip != null && toolTip != string.Empty )
      {
        ttt.szText = toolTip;
        ttt.hinst = IntPtr.Zero;
        if( RightToLeft == RightToLeft.Yes ) ttt.uFlags |= ( int )ToolTipFlags.TTF_RTLREADING;
        Marshal.StructureToPtr( ttt, m.LParam, true );
        m.Result = ( IntPtr ) 1;
      }
    }
  
    private  void NotifyNeedTextW( ref Message m )
    {
      if( Marshal.SystemDefaultCharSize != 2 ) return;
      if( m_items.Count == 0 ) return;

      // This code is a duplicate of NotifyNeedTextA
      TOOLTIPTEXT ttt = ( TOOLTIPTEXT ) m.GetLParam(typeof( TOOLTIPTEXT ));
      ToolBarItem item = ( ToolBarItem ) m_items[ttt.hdr.idFrom];
      string toolTip = item.ToolTip;
      
      if( toolTip != null && toolTip != string.Empty )
      {
        ttt.szText = toolTip;
        ttt.hinst = IntPtr.Zero;
        if( RightToLeft == RightToLeft.Yes ) ttt.uFlags |= ( int )ToolTipFlags.TTF_RTLREADING;
        Marshal.StructureToPtr( ttt, m.LParam, true );
        m.Result = ( IntPtr ) 1;
      }
    }

    private  void NotifyCustomDraw( ref Message m )
    {
      m.Result = ( IntPtr ) CustomDrawReturnFlags.CDRF_DODEFAULT;
      NMTBCUSTOMDRAW tbcd = ( NMTBCUSTOMDRAW )m.GetLParam(typeof( NMTBCUSTOMDRAW ));

      switch ( tbcd.nmcd.dwDrawStage )
      {
        case ( int )CustomDrawDrawStateFlags.CDDS_PREPAINT:
          // Tell toolbar control that we want to do the painting ourselves
          m.Result = ( IntPtr ) CustomDrawReturnFlags.CDRF_NOTIFYITEMDRAW;
          break;
  
        case ( int )CustomDrawDrawStateFlags.CDDS_ITEMPREPAINT:
          // Do custom painting
          NotifyCustomDrawToolBar( ref m );
          break;
      }
    }

    private  void NotifyCustomDrawToolBar( ref Message m )
    {
      // This toolbar could be the one that we are just using to provide
      // some "height" to the rebar control -- the place holder bar -- if
      // that is the case no need to do any painting
      // or it could be a ToolBar that has a ToolBarItem place holder, skip this case too
      if( m_parent.Bands.Count == 0 || m_items.Count == 0) 
      {
        m.Result = ( IntPtr ) CustomDrawReturnFlags.CDRF_SKIPDEFAULT;
        return;
      }

      m.Result = ( IntPtr ) CustomDrawReturnFlags.CDRF_DODEFAULT;
      
      // See if use wants the VSNet look or let XP dictate
      // the toolbar look
      if( IsCommonCtrl6() )
      {
        // Let the operating system do the drawing
        return;
      }
          
      NMTBCUSTOMDRAW tbcd = ( NMTBCUSTOMDRAW ) m.GetLParam(typeof( NMTBCUSTOMDRAW ));
      RECT rc = tbcd.nmcd.rc;
      Rectangle rectangle = new Rectangle( rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top );

      Graphics g = Graphics.FromHdc( tbcd.nmcd.hdc );
      int index = tbcd.nmcd.dwItemSpec;
      ToolBarItem item = m_items[index];

      if( item.Style == ToolBarItemStyle.ComboBox && item.ComboBox != null )
      {
        // ComboBoxes paint themselves
        // the combox new size, after changing the combobox font,
        // does not get updated until later in the drawing logic
        // pick up the right size here and update the combobox position
        UpdateComboBoxPosition( index );
        m.Result = ( IntPtr ) CustomDrawReturnFlags.CDRF_SKIPDEFAULT;
        return;
      }

      bool hot = ( bool )((tbcd.nmcd.uItemState & ( int )CustomDrawItemStateFlags.CDIS_HOT) != 0);
      bool selected = ( bool )((tbcd.nmcd.uItemState & ( int )CustomDrawItemStateFlags.CDIS_SELECTED) != 0);
      bool disabled = ( bool )((tbcd.nmcd.uItemState & ( int )CustomDrawItemStateFlags.CDIS_DISABLED) != 0);
      string tempString = item.Text;

      // NOTE: modified by ALEXK
      bool hasText = ( tempString != string.Empty && tempString != null && item.HideText == false  );
  
      if( item.Checked ) 
      {
        if( hot )
          g.FillRectangle( ColorUtil.VSNetPressedBrush, rectangle );
        else
          g.FillRectangle( ColorUtil.VSNetCheckedBrush, rectangle );
        
        g.DrawRectangle( ColorUtil.VSNetBorderPen, 
          rectangle.Left, rectangle.Top, rectangle.Width-1, rectangle.Height-1);
      }
      else if( selected ) 
      {
        if( item.Style == ToolBarItemStyle.DropDownButton ) 
        {
          // Draw background
          g.FillRectangle( ColorUtil.VSNetSelectionBrush, rectangle);
          g.FillRectangle( ColorUtil.VSNetPressedBrush, rectangle.Left, rectangle.Top, 
            rectangle.Width-DROWPDOWN_ARROW_WIDTH+1, rectangle.Height);

          g.DrawRectangle( ColorUtil.VSNetBorderPen, 
            rectangle.Left, rectangle.Top, rectangle.Width-1, rectangle.Height-1);
        }
        else 
        {
          if( m_enBarType == TBarType.MenuBar)
          {
            g.FillRectangle( ColorUtil.VSNetControlBrush, rectangle );
            if( ColorUtil.UsingCustomColor )
            {
              // Use same color for both sides to make it look flat
              g.DrawRectangle( ColorUtil.VSNetBorderPen, 
                rectangle.Left, rectangle.Top, rectangle.Width-1, rectangle.Height-1);
            }
            else 
            {
              ControlPaint.DrawBorder3D(g, rectangle.Left, rectangle.Top, rectangle.Width-1, 
                rectangle.Height-1, Border3DStyle.Flat, Border3DSide.Top | Border3DSide.Left | Border3DSide.Right);
            }
          }
          else 
          {
            g.FillRectangle( ColorUtil.VSNetPressedBrush, rectangle );
            g.DrawRectangle( ColorUtil.VSNetBorderPen, 
              rectangle.Left, rectangle.Top, rectangle.Width-1, rectangle.Height-1);
          }
        }
      }
      else if( item.Style == ToolBarItemStyle.DropDownButton && item.Dropped )
      {
        g.FillRectangle( ColorUtil.VSNetControlBrush, rectangle);
        g.DrawRectangle( SystemPens.ControlDark, 
          rectangle.Left, rectangle.Top, rectangle.Width-1, rectangle.Height-1);
      }
      else if( hot )
      {
        g.FillRectangle( ColorUtil.VSNetSelectionBrush, rectangle );
        g.DrawRectangle( ColorUtil.VSNetBorderPen, 
          rectangle.Left, rectangle.Top, rectangle.Width-1, rectangle.Height-1);
      }
      else 
      {
        if( item.Style == ToolBarItemStyle.DropDownButton )
        {
          
          IntPtr hreBar = WindowsAPI.GetParent( Handle );
          IntPtr hMainWindow = IntPtr.Zero;
          bool mainHasFocus = false;
          
          if( hreBar != IntPtr.Zero )
          {
            hMainWindow = WindowsAPI.GetParent( hreBar );
            if( hMainWindow != IntPtr.Zero ) 
              mainHasFocus = ( hMainWindow == WindowsAPI.GetFocus());
          }

          if( hMainWindow != IntPtr.Zero &&  mainHasFocus)
          {
            Point pos = Control.MousePosition;
            Point clientPoint = PointToClient( pos );
            
            if( rectangle.Contains( clientPoint ))
            {
              g.FillRectangle( ColorUtil.VSNetSelectionBrush, rectangle);
              g.DrawRectangle( ColorUtil.VSNetBorderPen, 
                rectangle.Left, rectangle.Top, rectangle.Width-1, rectangle.Height-1);

              rc.right -= DROWPDOWN_ARROW_WIDTH;
              g.DrawLine( ColorUtil.VSNetBorderPen, 
                rc.right+1, rc.top, rc.right+1, rc.top + ( rc.bottom-rc.top ));
              rc.right += DROWPDOWN_ARROW_WIDTH;
            }
          }
        }
      }

      if( item.Style == ToolBarItemStyle.DropDownButton ) 
      {
        DrawButtonArrowGlyph( g, rectangle );

        // Draw line that separates the arrow from the button
        rc.right -= DROWPDOWN_ARROW_WIDTH;
        if( hot && !item.Dropped )
          g.DrawLine( ColorUtil.VSNetBorderPen, 
            rc.right+1, rc.top, rc.right+1, rc.top + ( rc.bottom-rc.top ));
        item.Dropped = false;

      }
      
      // If the toolBar has an assign image list use that instead
      Image image = null;
      if( m_imgList != null && item.ImageIndex != -1 )
      {
        if( item.ImageIndex < m_imgList.Images.Count )
          image = m_imgList.Images[item.ImageIndex];
      }
      else
      {
        image = item.Image;
      }
      
      if( image != null )
      {
        Size size = image.Size;
        Point point = new Point(rc.left + (( rc.right - rc.left - size.Width ) / 2), 
          rc.top + (( rc.bottom - rc.top - size.Height ) / 2));
        if( hasText ) point.X = rc.left + MARGIN;
        if( disabled ) 
          ControlPaint.DrawImageDisabled( g, image, point.X, point.Y, ColorUtil.VSNetSelectionColor );
        else if( hot && !selected && !item.Checked )
        {
          ControlPaint.DrawImageDisabled( g, image, point.X+1, point.Y, ColorUtil.VSNetSelectionColor );
          g.DrawImage( image, point.X, point.Y-1 );
        }
        else 
        {
          if( item.Checked  )
          {
            if  ( !selected ) point.Y -= 1;
          }
          g.DrawImage( image, point.X, point.Y );
        }
      }

      // Draw Text 
      if( hasText )
      {
        string currentText = item.Text;
        int amperSandIndex = currentText.IndexOf( '&' );
        if( m_enBarType == TBarType.MenuBar && amperSandIndex != -1 )
          currentText = item.Text.Remove( amperSandIndex, 1 );
        Size textSize = TextUtil.GetTextSize( g, currentText, SystemInformation.MenuFont );
        Point pos;
        
        if( m_enBarType == TBarType.MenuBar || image == null) 
        {
          int offset = rc.left + (( rc.right - rc.left ) - textSize.Width)/2;
          pos = new Point(offset, rc.top + (( rc.bottom - rc.top - textSize.Height ) / 2));
        }
        else
        {
          pos = new Point(rc.left, rc.top + (( rc.bottom - rc.top - textSize.Height ) / 2));
          pos.X = rc.left + MARGIN + image.Size.Width + MARGIN;
        }

        StringFormat stringFormat = new StringFormat();
        stringFormat.HotkeyPrefix = HotkeyPrefix.Show;
        g.DrawString( item.Text, SystemInformation.MenuFont, Brushes.Black, pos, stringFormat ); 
      }
    
      m.Result = ( IntPtr ) CustomDrawReturnFlags.CDRF_SKIPDEFAULT;
    
    }

    protected void DrawButtonArrowGlyph( Graphics g, Rectangle rectangle )
    {
      // Draw arrow glyph
      Point[] pts = new Point[3];
      int leftEdge = rectangle.Left + ( rectangle.Width-DROWPDOWN_ARROW_WIDTH+1 );
      int middle = rectangle.Top + rectangle.Height/2-1;
      pts[0] = new Point( leftEdge + 4, middle );
      pts[1] = new Point( leftEdge + 9,  middle );
      pts[2] = new Point( leftEdge + 6, middle+3 );
      g.FillPolygon( Brushes.Black, pts );

    }
    private  MenuItem FindShortcutItem( MenuItemExCollection m_items, Keys keys )
    {
      MenuItem resultItem = null;
      foreach ( MenuItemEx item in m_items  )
      {
        if( (( int )item.Shortcut == ( int )keys) && ( item.Enabled ) && ( item.Visible ) )
        {
          return item;
        }
        else
        {
          resultItem =  FindShortcutItem( item.MenuItems, keys );
          if( resultItem != null ) break;
        }
      }
      
      return resultItem;
    }

    private  MenuItem FindShortcutItem( Menu.MenuItemCollection collection, Keys keys )
    {
      int count = collection.Count;
      foreach ( MenuItem item in collection  )
      {
        if( (( int )item.Shortcut == ( int )keys) && ( item.Enabled ) && ( item.Visible ) )
        {
          return item;
        }
        else
        {
          return FindShortcutItem( item.MenuItems, keys );
        }
      }
      return null;
    }

    private  void Items_Changed( Object s, EventArgs e )
    {
      UpdateItems();
    }

    private  void Item_Changed( Object s, EventArgs e )
    {
      ToolBarItem[] handledItems = this.handledItems;
      
      foreach( ToolBarItem item in handledItems )
      {
        if( item == s )
        {
          if( item == null ) return;
          int index = m_items.IndexOf( item );
          if( index == -1 ) return;

          if( m_items[index].Style == ToolBarItemStyle.ComboBox && m_items[index].ComboBox != null )
          {
            WindowsAPI.ShowWindow( m_items[index].ComboBox.Handle, ( short )ShowWindowStyles.SW_HIDE );
            WindowsAPI.SetParent( m_items[index].ComboBox.Handle, IntPtr.Zero);
            m_items[index].ComboBox.Parent = null;
            
            WindowsAPI.SetParent( m_items[index].ComboBox.Handle, Handle );
            WindowsAPI.ShowWindow( m_items[index].ComboBox.Handle, ( short )ShowWindowStyles.SW_SHOWNOACTIVATE );
          }
          
          UpdateItem( index );
        }
      }
    }
  
    private  void UpdateItems()
    {
      Detach();
      Attach();
      if( IsHandleCreated ) RecreateHandle();
    }
  
    private  void RealizeItems()
    {
      UpdateImageList();
            
      for ( int i = 0; i < m_items.Count; i++ )
      {
        m_items[i].Index = i;
        m_items[i].ToolBar = this;
        TBBUTTON button = new TBBUTTON();
        button.idCommand = i;
        WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_INSERTBUTTON, i, ref button);
  
        TBBUTTONINFO tbi = ( TBBUTTONINFO )m_items[ i ];
        
        WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_SETBUTTONINFOW, i, ref tbi);
        if( m_items[i].Style == ToolBarItemStyle.ComboBox )
        {
          UpdateComboBoxPosition( i );
        }
      }
            
      UpdateSize();
    }

    private  void UpdateComboBoxPosition( int index )
    {
      if( Items[ index ].ComboBox != null )
      {
        RECT rect = new RECT();
        WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_GETRECT, index, ref rect);

        Rectangle rc = ( Rectangle )rect;
        ComboBox box = Items[ index ].ComboBox;
        int topOffset = rect.top + ( rc.Height - box.Height )/2;
        box.Bounds = new Rectangle( rect.left + 1, topOffset, box.Width, box.Height );
        
        WindowsAPI.InvalidateRect( box.Handle, IntPtr.Zero, 1 );
      }
    }

    private  void UpdateImageList()
    {
      Size size = new Size( 16, SystemInformation.MenuFont.Height );
      for ( int i = 0; i < m_items.Count; i++ )
      {
        Image image = m_items[i].Image;
        if( image != null )
        {
          if( image.Width > size.Width ) size.Width = image.Width;
          if( image.Height > size.Height ) size.Height = image.Height;
        }
      }
  
      imageListHelper = new ImageList();
      
      if( m_imgList == null )
        imageListHelper.ImageSize = size;
      else
        imageListHelper.ImageSize = m_imgList.ImageSize;
      
      imageListHelper.ColorDepth = ColorDepth.Depth32Bit;   

      for ( int i = 0; i < m_items.Count; i++ )
      {
        // Take combobox size into consideration too
        if( m_items[i].Style == ToolBarItemStyle.ComboBox && m_items[i].ComboBox != null )
        {
          // update combobox to use the current system menu font
          m_items[i].ComboBox.Font = SystemInformation.MenuFont;
          ComboBoxBase cbb = ( ComboBoxBase )m_items[i].ComboBox;
          int decrease = 2;
          
          if( SystemInformation.MenuFont.Height >= 20) decrease = 5;
          
          cbb.SetFontHeight( SystemInformation.MenuFont.Height-decrease );
        }
        
        Image image = m_items[i].Image;
        imageListHelper.Images.Add(( image != null ) ? image : new Bitmap( size.Width, size.Height ));
      }

      if( m_parent.RebarDesigner != null && m_items.Count == 0 )
      {
        // If we are faking the ToolBarItem, add at least one image
        // so that the m_imgList has a size
        imageListHelper.Images.Add(new Bitmap( size.Width, size.Height ));
      }

      //IntPtr handle = ( TBarType == TBarType.MenuBar ) ? IntPtr.Zero : m_imgList.Handle;
      IntPtr handle = imageListHelper.Handle;
      WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_SETIMAGELIST, 0, handle);
    }

    private  void UpdateSize()
    {
      Size size = new Size( 0, 0 );
      for ( int i = 0; i < m_items.Count; i++ )
      {
        size.Height = Math.Max( size.Height, m_items[i].ItemRectangle.Height );
        size.Width += m_items[i].ItemRectangle.Width;
      }

/*
      if( m_parent.RebarDesigner != null && m_items.Count == 0 )
      {
        // If we are faking the ToolBarItem get the size for the ghost ToolBarItem
        RECT rect = new RECT();
        WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_GETRECT, 0, ref rect);
        int height = rect.bottom - rect.top;
        if( height > size.Height ) size.Height = height;
        size.Width = rect.right - rect.left;
      }
*/

      Size = size;
    }

    internal int  GetItemAtPoint( Point p )
    {
      for( int i = 0; i < m_items.Count; i++ )
      {
        Rectangle rect = m_items[i].ItemRectangle;
        if( rect.Contains( p ) ) return i;
      }

      return -1;
    }

    public   int  GetIdealSize()
    {
      Size size = new Size( 0, 0 );
      
      for ( int i = 0; i < m_items.Count; i++ )
      {
        RECT rect = new RECT();
        WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_GETRECT, i, ref rect);
        int height = rect.bottom - rect.top;
        if( height > size.Height ) size.Height = height;
        size.Width += rect.right - rect.left;
      }
      
      return size.Width;
    }

    private  void UpdateItem( int index )
    {
      if( !IsHandleCreated ) return;

      if( m_items[index].Visible == handledItemsVisible[index] )
      {
        TBBUTTONINFO tbi = ( TBBUTTONINFO )m_items[ index ];
        
        WindowsAPI.SendMessage(Handle, ( int )ToolBarMessages.TB_SETBUTTONINFOW, index, ref tbi);
        if( m_items[index].Style == ToolBarItemStyle.ComboBox )
        {
          UpdateComboBoxPosition( index );
        }
        
        // When in design mode the text is added after a ToolBarItem has been inserted producing
        // a mismatch between the actual size of the toolbar and the size the Rebar think it is
        // force size update if in design mode
        if( m_parent.RebarDesigner != null )
        {
          UpdateSize();
          m_parent.UpdateBands();
        }

      }     
      else
      {
        UpdateItems();
      }
    }

    internal void PassMsg( ref Message message )
    {
      WndProc( ref message );
    }
    #endregion
  }
}

