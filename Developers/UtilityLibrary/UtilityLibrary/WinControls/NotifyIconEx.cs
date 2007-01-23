using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Reflection;

using UtilityLibrary.Win32;


namespace UtilityLibrary.WinControls
{
  [ToolboxItem(true)]
  [ToolboxBitmap(typeof(UtilityLibrary.WinControls.NotifyIconEx), "UtilityLibrary.WinControls.NotifyIconEx.bmp")]
  public class NotifyIconEx : System.ComponentModel.Component
  {
    #region Notify Icon Target Window - internal class
    private class NotifyIconTarget : System.Windows.Forms.Form
    {
      public NotifyIconTarget()
      {
        this.Text = "Hidden NotifyIconTarget Window";
      }

      protected override void DefWndProc(ref Message msg)
      {
        if( (Msg)msg.Msg == Msg.WM_USER ) // WM_USER
        {
          uint msgId = (uint)msg.LParam;
          uint id = (uint)msg.WParam;

          switch( (Msg)msgId )
          {
            case Msg.WM_LBUTTONDOWN:
            case Msg.WM_MOUSEMOVE:
              break;

            case Msg.WM_LBUTTONUP:
              if( ClickNotify != null )
              {
                ClickNotify( this, id );
              }
              break;

            case Msg.WM_LBUTTONDBLCLK:
              if( DoubleClickNotify != null )
              {
                DoubleClickNotify( this, id );
              }
              break;

            case Msg.WM_RBUTTONUP:
              if( RightClickNotify != null )
              {
                RightClickNotify(this, id);
              }
              break;

            case Msg.NIN_BALLOONSHOW:
              break;

              // this should happen when the balloon is closed using the x
              // - we never seem to get this message!
            case Msg.NIN_BALLOONHIDE:
              break;

              // we seem to get this next message whether the balloon times
              // out or whether it is closed using the x
            case Msg.NIN_BALLOONTIMEOUT:
              break;

            case Msg.NIN_BALLOONUSERCLICK:
              if( ClickBalloonNotify != null )
              {
                ClickBalloonNotify(this, id);
              }
              break;
          }
        }
        else if( (Msg)msg.Msg == Msg.WM_TASKBAR_CREATED) // WM_TASKBAR_CREATED
        {
          if( TaskbarCreated != null )
          {
            TaskbarCreated(this, System.EventArgs.Empty);
          }
        }
        else
        {
          base.DefWndProc(ref msg);
        }
      }

      public delegate void NotifyIconHandler(object sender, uint id);
    
      public event NotifyIconHandler ClickNotify;
      public event NotifyIconHandler DoubleClickNotify;
      public event NotifyIconHandler RightClickNotify;
      public event NotifyIconHandler ClickBalloonNotify;
      public event EventHandler TaskbarCreated;
    }
    #endregion

    #region Class Enums
    #endregion
    
    #region Static members
    private static NotifyIconTarget m_messageSink = new NotifyIconTarget();
    private static uint m_nextId = 1;
    #endregion

    #region Class members
    private ContextMenu m_contextMenu = null;
    private uint    m_id      = 0;  // each icon in the notification area has an id
    private IntPtr  m_handle;       // save the handle so that we can remove icon
    private string  m_text    = "";
    private Icon    m_icon;
    private bool    m_visible;
    private bool    m_doubleClick;  // fix for extra mouse up message we want to discard

    private ImageList m_imgList;
    private int       m_iImageIndex = -1;
    private Timer     m_timeAnimation  = new Timer();
    #endregion

    #region Class Events
    [ Category( "Action" ) ]
    public event EventHandler Click;
    [ Category( "Action" ) ]
    public event EventHandler DoubleClick;
    [ Category( "Action" ) ]
    public event EventHandler BalloonClick;
    [ Category( "Property Changed" ) ]
    public event EventHandler TextChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler IconChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler ContextMenuChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler VisibleChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler ImageListChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler ImageIndexChanged;
    #endregion

    #region Class Properties
    [ Category( "Appearance" ), 
    DefaultValue( "" ), 
    Description( "GET/SET Tooltip Text" ) ]
    public string Text
    {
      get
      {
        return m_text;
      }
      set
      {
        if( value != m_text )
        {
          m_text = value;
          OnTextChanged();
        }
      }
    }
    
    [ Category( "Appearance" ), 
    DefaultValue( null ), 
    Description( "GET/SET Icon which will be displayed in tary" ) ]
    public Icon Icon
    {
      get
      {
        return m_icon;
      }
      set
      {
        if( value != m_icon )
        {
          m_icon = value;
          OnIconChanged();
        }
      }
    }

    [ Category( "Behaviour" ), 
    DefaultValue( null ), 
    Description( "GET/SET Context menu which must be shown on tray icon click" ) ]
    public ContextMenu ContextMenu
    {
      get
      {
        return m_contextMenu;
      }
      set
      {
        if( value != m_contextMenu )
        {
          m_contextMenu = value;
          OnContextMenuChanged();
        }
      }
    }

    [ Category( "Behaviour" ), 
    DefaultValue( false ), 
    Description( "GET/SET is tray icon visible" ) ]
    public bool Visible
    {
      get
      {
        return m_visible;
      }
      set
      {
        if( value != m_visible )
        {
          m_visible = value;
          OnVisibleChanged();
        }
      }
    }

    [ Category( "Behaviour" ), 
    DefaultValue( null ),
    Description( "GET/SET list of icons which can be animated in tray" ) ]
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
          m_imgList = value;

        }
      }
    }

    [ Browsable(true), 
    Category( "Behaviour" ), 
    DefaultValue(-1),
    Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor)),
    TypeConverter("System.Windows.Forms.ImageIndexConverter"),
    Description( "GET/SET start icon of animation" ) ]
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

    [ Category( "Behaviour" ), 
    DefaultValue( 100 ), 
    Description( "GET/SET is tray icon visible" ) ]    
    public int AnimationSwitchTime
    {
      get
      {
        return m_timeAnimation.Interval;
      }
      set
      {
        m_timeAnimation.Interval = value;
      }
    }
    #endregion

    #region Class Initialize/Finilize methods
    public NotifyIconEx()
    {
      m_timeAnimation.Interval = 100;
      m_timeAnimation.Tick += new EventHandler( Animate_NextIcon );
    }

    // added to be compatible with NotifyIcon class 
    public NotifyIconEx( IContainer container ) 
      : this()
    {
    }
    
    protected override void Dispose(bool disposing)
    {
      m_timeAnimation.Stop();
      m_timeAnimation.Tick -= new EventHandler( Animate_NextIcon );
      Remove();

      base.Dispose( disposing );
    }
    #endregion

    #region Class Private Helper methods
    /// <summary>
    /// this method adds the notification icon if it has not been added and if we 
    /// have enough data to do so
    /// </summary>
    private void CreateOrUpdate()
    {
      if( this.DesignMode ) return;

      if( m_id == 0 )
      {
        if( m_icon != null )
        {
          // create icon using available properties
          Create( m_nextId++ );
        }
      }
      else
      {
        // update notify icon
        Update();
      }
    }

    /// <summary>
    /// Function Create a new Tray icon object
    /// </summary>
    /// <param name="id"></param>
    private void Create( uint id )
    {
      m_id = id;
      m_handle = m_messageSink.Handle;

      NOTIFYICONDATA data = CreateNotifyStruct( NotifyFlags.NIF_MESSAGE | 
        NotifyFlags.NIF_ICON | 
        NotifyFlags.NIF_TIP );

      WindowsAPI.Shell_NotifyIcon( NotifyCommand.NIM_ADD, ref data );

      // add handlers
      m_messageSink.ClickNotify         += new NotifyIconTarget.NotifyIconHandler( Icon_OnClick );
      m_messageSink.DoubleClickNotify   += new NotifyIconTarget.NotifyIconHandler( Icon_OnDoubleClick );
      m_messageSink.RightClickNotify    += new NotifyIconTarget.NotifyIconHandler( Icon_OnRightClick );
      m_messageSink.ClickBalloonNotify  += new NotifyIconTarget.NotifyIconHandler( Ballon_OnClick );
      m_messageSink.TaskbarCreated      += new EventHandler( OnTaskbarCreated );
    }

    /// <summary>
    /// Method update current state of tray icon
    /// </summary>
    private void Update()
    {
      NOTIFYICONDATA data = CreateNotifyStruct( NotifyFlags.NIF_ICON | 
        NotifyFlags.NIF_TIP | 
        NotifyFlags.NIF_STATE );

      WindowsAPI.Shell_NotifyIcon( NotifyCommand.NIM_MODIFY, ref data );

      RefreshSystemTray();
    }

    /// <summary>
    /// Function create struct which used Shell_NotifyIcon API function
    /// </summary>
    /// <param name="flags"></param>
    /// <returns></returns>
    private NOTIFYICONDATA CreateNotifyStruct( NotifyFlags flags )
    {
      return CreateNotifyStruct( flags, false );
    }
    
    /// <summary>
    /// Function create struct which used Shell_NotifyIcon API function
    /// </summary>
    /// <param name="flags"></param>
    /// <returns></returns>
    private NOTIFYICONDATA CreateNotifyStruct( NotifyFlags flags, bool bSkip )
    {
      NOTIFYICONDATA data = new NOTIFYICONDATA();
      data.cbSize = (uint)Marshal.SizeOf( data );

      data.hWnd = m_handle;
      data.uID = m_id;

      data.uCallbackMessage = 0x400;
      data.uFlags |= flags;

      if( m_icon != null )
      {
        data.hIcon = m_icon.Handle; // this should always be valid
      }
      else
      {
        if( m_imgList != null && m_imgList.Images.Count > 0 )
        {
          Bitmap bmp = new Bitmap( m_imgList.Images[ 0 ], 16, 16 );
          data.hIcon = bmp.GetHicon();
        }
      }
      
      data.szTip = m_text;

      if( !bSkip )
      {
        if( !m_visible ) data.dwState = NotifyState.NIS_HIDDEN;
        data.dwStateMask |= NotifyState.NIS_HIDDEN;
      }

      return data;
    }

    /// <summary>
    /// Method remove/delete icon from tray
    /// </summary>
    private void Remove()
    {
      if( m_id != 0 )
      {
        // remove the notify icon
        NOTIFYICONDATA data = new NOTIFYICONDATA();
        data.cbSize = (uint)Marshal.SizeOf(data);
          
        data.hWnd = m_handle;
        data.uID = m_id;

        WindowsAPI.Shell_NotifyIcon( NotifyCommand.NIM_DELETE, ref data );
        m_id = 0;

        RefreshSystemTray();
      }
    }

    private void RefreshSystemTray()
    {
      IntPtr hShellTrayWnd = WindowsAPI.FindWindowEx( IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", IntPtr.Zero );
      WindowsAPI.InvalidateRect( hShellTrayWnd, IntPtr.Zero, 1 );
    }
    #endregion

    #region Class public methods
    public void ShowBalloon( string title, string text, NotifyInfoFlags type, uint timeoutInMilliSeconds )
    {
      NOTIFYICONDATA data = CreateNotifyStruct( NotifyFlags.NIF_INFO, true );

      data.uTimeoutOrVersion = timeoutInMilliSeconds; // this value does not seem to work - any ideas?
      data.szInfoTitle  = title;
      data.szInfo       = text;
      data.dwInfoFlags  = type;

      WindowsAPI.Shell_NotifyIcon( NotifyCommand.NIM_MODIFY, ref data );
    }

    public void StartAnimation()
    {
      m_timeAnimation.Start();
    }

    public void StopAnimation()
    {
      m_timeAnimation.Stop();

      // recover start icon
      if( m_icon != null )
      {
        Update();
      }
    }
    #endregion

    #region Class Overrides
    protected virtual void OnTextChanged()
    {
      if( m_visible == true ) CreateOrUpdate();
      RaiseTextChangedEvent();
    }

    protected virtual void OnIconChanged()
    {
      if( m_visible == true ) CreateOrUpdate();
      RaiseIconChangedEvent();
    }

    protected virtual void OnContextMenuChanged()
    {
      RaiseContextMenuChangedEvent();
    }

    protected virtual void OnVisibleChanged()
    {
      CreateOrUpdate();
      RaiseVisibleChangedEvent();
    }

    protected virtual void OnImageListChanged()
    {
      RaiseImageListChangedEvent();
    }

    protected virtual void OnImageIndexChanged()
    {
      RaiseImageIndexChangedEvent();
    }

    #endregion

    #region Event Raisers
    protected virtual void RaiseTextChangedEvent()
    {
      if( TextChanged != null )
      {
        TextChanged( this, EventArgs.Empty );
      }
    }

    protected virtual void RaiseIconChangedEvent()
    {
      if( IconChanged != null )
      {
        IconChanged( this, EventArgs.Empty );
      }
    }

    protected virtual void RaiseContextMenuChangedEvent()
    {
      if( ContextMenuChanged != null )
      {
        ContextMenuChanged( this, EventArgs.Empty );
      }
    }

    protected virtual void RaiseVisibleChangedEvent()
    {
      if( VisibleChanged != null )
      {
        VisibleChanged( this, EventArgs.Empty );
      }
    }

    protected virtual void RaiseImageListChangedEvent()
    {
      if( ImageListChanged != null )
      {
        ImageListChanged( this, EventArgs.Empty );
      }
    }

    protected virtual void RaiseImageIndexChangedEvent()
    {
      if( ImageIndexChanged != null )
      {
        ImageIndexChanged( this, EventArgs.Empty );
      }
    }
    #endregion
    
    #region Message Handlers
    private void Icon_OnClick(object sender, uint id)
    {
      if( id == m_id )
      {
        if( !m_doubleClick && Click != null )
        {
          Click( this, EventArgs.Empty );
        }

        m_doubleClick = false;
      }
    }

    private void Icon_OnRightClick(object sender, uint id)
    {
      if( id == m_id )
      {
        // show context menu
        if( m_contextMenu != null )
        {         
          POINT point = new POINT();
          WindowsAPI.GetCursorPos( ref point );
          
          // this ensures that if we show the menu and then click on another window the menu will close
          WindowsAPI.SetForegroundWindow( m_messageSink.Handle ); 

          // call non public member of ContextMenu
          m_contextMenu.GetType().InvokeMember( "OnPopup",
            BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance,
            null, m_contextMenu, new Object[] {System.EventArgs.Empty});

          WindowsAPI.TrackPopupMenuEx( m_contextMenu.Handle, 64, point.x, point.y, 
            m_messageSink.Handle, IntPtr.Zero );
        }
      }
    }

    private void Icon_OnDoubleClick(object sender, uint id)
    {
      if( id == m_id )
      {
        m_doubleClick = true;
        
        if( DoubleClick != null )
        {
          DoubleClick( this, EventArgs.Empty );
        }
      }
    }

    private void Ballon_OnClick(object sender, uint id)
    {
      if( id == m_id )
      {
        if( BalloonClick != null )
        {
          BalloonClick( this, EventArgs.Empty );
        }
      }
    }

    private void Animate_NextIcon( object sender, EventArgs e )
    {
      if( m_imgList != null && m_imgList.Images.Count > 0 )
      {
        int iCount = m_imgList.Images.Count;

        if( m_iImageIndex < iCount - 1 ) 
          m_iImageIndex++;
        else
          m_iImageIndex = 0;

        NOTIFYICONDATA data = CreateNotifyStruct( NotifyFlags.NIF_ICON | 
          NotifyFlags.NIF_TIP | 
          NotifyFlags.NIF_STATE );

        Bitmap bmp = new Bitmap( m_imgList.Images[ m_iImageIndex ], 16, 16 );
        data.hIcon = bmp.GetHicon();

        WindowsAPI.Shell_NotifyIcon( NotifyCommand.NIM_MODIFY, ref data );
      }
    }
    private void OnTaskbarCreated(object sender, EventArgs e)
    {
      if( m_id != 0)
      {
        Create( m_id ); // keep the id the same
      }
    }
    #endregion
  }
}
