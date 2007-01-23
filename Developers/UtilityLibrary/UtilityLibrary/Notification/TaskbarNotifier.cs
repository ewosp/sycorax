using System;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Runtime.InteropServices;

using System.ComponentModel;
using System.ComponentModel.Design;

using System.Diagnostics;

using UtilityLibrary.Win32;


namespace UtilityLibrary.WinControls
{
  #region TaskbarNotifier Enums
  public enum TaskbarStates
  {
    HIDDEN,
    APPEARING,
    VISIBLE,
    DISAPPEARING
  }
  #endregion

  [ ToolboxItem( true ),
  ToolboxBitmap( typeof( UtilityLibrary.WinControls.TaskbarNotifier ), 
    "UtilityLibrary.Notification.TaskbarNotifier.bmp") ]
  public class TaskbarNotifier : Component
  {
    #region Class members
    private Rectangle m_rectClose;

    private Form    m_form;
    private Label   m_lblTitle;
    private Label   m_lblContent;
    private Bitmap  m_bmpClose;
    
    private Point   m_pntClose = Point.Empty;
    private Point   m_pntTitle = Point.Empty;
    private Point   m_pntContent = Point.Empty;
    
    // callback timer
    private System.Threading.Timer  m_timerCall;
    private TaskbarStates           m_state = TaskbarStates.HIDDEN;

    private string m_strTitle;
    private string m_strContent;

    private ButtonState m_btnState = ButtonState.Normal;

    private int iHeightIncriment;
    private int iHeightDecriment;

    private int m_iTimeToShow = 500;
    private int m_iTimeToStay = 3000;
    private int m_iTimeToHide = 500;
    #endregion

    #region Class events
    [ Category( "Action" ) ]
    public event EventHandler OnCloseClick;
    [ Category( "Action" ) ]
    public event EventHandler OnTitleClick;
    [ Category( "Action" ) ]
    public event EventHandler OnContentClick;
    [ Category( "Property Change" ) ]
    public event EventHandler TitleChanged;
    [ Category( "Property Change" ) ]
    public event EventHandler ContentChanged;
    [ Category( "Property Change" ) ]
    public event EventHandler CloseButtonChanged;
    [ Category( "Property Change" ) ]
    public event EventHandler ClosePositionChanged;
    [ Category( "Property Change" ) ]
    public event EventHandler TimeToShowChanged;
    [ Category( "Property Change" ) ]
    public event EventHandler TimeToStayChanged;
    [ Category( "Property Change" ) ]
    public event EventHandler TimeToHideChanged;
    [ Category( "Property Change" ) ]
    public event EventHandler TitlePositionChanged;
    [ Category( "Property Change" ) ]
    public event EventHandler ContentPositionChanged;
    #endregion

    #region Constructor
    public TaskbarNotifier()
    {
      m_form = new Form();

      // Window Style
      m_form.FormBorderStyle  = FormBorderStyle.None;
      
      m_form.ShowInTaskbar = 
        m_form.MaximizeBox = 
        m_form.MinimizeBox = 
        m_form.ControlBox  = false;
      
      m_form.TopMost = true;

      m_lblTitle = new Label();
      m_lblContent = new Label();

      m_lblTitle.BackColor = m_lblContent.BackColor = Color.Transparent;
      m_lblTitle.Font = new Font( m_lblTitle.Font.FontFamily, 10, FontStyle.Bold );

      m_form.Controls.Add( m_lblTitle );
      m_form.Controls.Add( m_lblContent );

      m_form.Paint += new PaintEventHandler( this.Form_Paint );
      m_form.Click += new EventHandler( this.Form_Click );
      m_lblTitle.Click += new EventHandler( this.Title_Click );
      m_lblContent.Click += new EventHandler( this.Content_Click );
      
      // initialize timer
      TimerCallback callback = new TimerCallback( Timer_Callback );
      m_timerCall = new System.Threading.Timer( callback, this, Timeout.Infinite, Timeout.Infinite );
    }

    protected override void Dispose( bool disposing  )
    {
      if( disposing )
      {
        StopTimer();
        Hide();
      }

      base.Dispose( disposing );
    }
    #endregion

    #region Class Properties
    [ Browsable( false ) ]
    public TaskbarStates CurrentState
    {
      get
      {
        return m_state;
      }
    }

    [ Category( "Appearance" ), 
    Browsable( true ),
    DefaultValue( "" ), 
    Description( "GET/SET Title of Notification" ) ]
    public string Title
    {
      get
      {
        return m_strTitle;
      }
      set
      {
        if( value != m_strTitle )
        {
          m_strTitle = value;
          OnTitleChanged();
        }
      }
    }

    [ Category( "Appearance" ), 
    DefaultValue( "" ), 
    Browsable( true ),
    Description( "GET/SET Notification Text" ) ]
    public string Content
    {
      get
      {
        return m_strContent;
      }
      set
      {
        if( value != m_strContent )
        {
          m_strContent = value;
          OnContentChanged();
        }
      }
    }

    [ Category( "Appearance" ), 
    Browsable( true ),
    DefaultValue( null ), 
    Description( "GET/SET Close button states image" ) ]
    public Image CloseButton
    {
      get
      {
        return m_bmpClose;
      }
      set
      {
        if( value != m_bmpClose )
        {
          m_bmpClose = new Bitmap( value );
          m_bmpClose.MakeTransparent( Color.Fuchsia );
          OnCloseButtonChanged();
        }
      }
    }

    [ Category( "Appearance" ), 
    Browsable( true ),
    DefaultValue( null ), 
    Description( "GET/SET position of close button" ) ]
    public Point  ClosePosition
    {
      get
      {
        return m_pntClose;
      }
      set
      {
        if( value != m_pntClose )
        {
          m_pntClose = value;
          OnClosePositionChanged();
        }
      }
    }

    [ Category( "Appearance" ), 
    Browsable( true ),
    DefaultValue( null ), 
    Description( "GET/SET background Image" ) ]
    public Image BackgroundImage
    {
      get
      {
        return m_form.BackgroundImage;
      }
      set
      {
        m_form.BackgroundImage = value;
        Bitmap bmp = new Bitmap( value );
        //m_form.Region = BitmapToRegion( bmp, Color.Fuchsia );
        m_form.Width  = bmp.Width;
        m_form.Height = bmp.Height;
      }
    }
    
    [ Category( "Behaviour" ), 
    Browsable( true ),
    DefaultValue( 500 ), 
    Description( "GET/SET Time needed class to show notify form" ) ]
    public int TimeToShow
    {
      get
      {
        return m_iTimeToShow;
      }
      set
      {
        if( value != m_iTimeToShow )
        {
          m_iTimeToShow = value;
          OnTimeToShowChanged();
        }
      }
    }

    [ Category( "Behaviour" ), 
    Browsable( true ),
    DefaultValue( 3000 ), 
    Description( "GET/SET how long class must show notify form" ) ]
    public int TimeToStay
    {
      get
      {
        return m_iTimeToStay;
      }
      set
      {
        if( value != m_iTimeToStay )
        {
          m_iTimeToStay = value;
          OnTimeToStayChanged();
        }
      }
    }

    [ Category( "Behaviour" ), 
    Browsable( true ),
    DefaultValue( 500 ), 
    Description( "GET/SET how fast class must hide notify window" ) ]
    public int TimeToHide
    {
      get
      {
        return m_iTimeToHide;
      }
      set
      {
        if( value != m_iTimeToHide )
        {
          m_iTimeToHide = value;
          OnTimeToHideChanged();
        }
      }
    }

    [ Category( "Appearance" ), 
    Browsable( true ),
    DefaultValue( null ), 
    Description( "GET/SET position of title" ) ]
    public Point TitlePosition
    {
      get
      {
        return m_pntTitle;
      }
      set
      {
        if( value != m_pntTitle )
        {
          m_pntTitle = value;
          OnTitlePositionChanged();
        }
      }
    }

    [ Category( "Appearance" ), 
    Browsable( true ),
    DefaultValue( null ), 
    Description( "GET/SET Content text position" ) ]
    public Point ContentPosition
    {
      get
      {
        return m_pntContent;
      }
      set
      {
        if( value != m_pntContent )
        {
          m_pntContent = value;
          OnContentPositionChanged();
        }
      }
    }
    #endregion

    #region Public Methods
    public void Show()
    {
      Rectangle rcWork = Screen.GetWorkingArea( m_form );

      iHeightIncriment = m_form.BackgroundImage.Height / 10;
      iHeightDecriment = iHeightIncriment;

      if( m_state == TaskbarStates.HIDDEN )
      {
        m_state = TaskbarStates.APPEARING;
        m_form.SetBounds( rcWork.Right - m_form.Width - 17, rcWork.Bottom - 1, m_form.Width, 0 );
        WindowsAPI.SetParent( m_form.Handle, IntPtr.Zero );

        // We Show the popup without stealing focus
        WindowsAPI.ShowWindow( m_form.Handle, (short)ShowWindowStyles.SW_SHOWNOACTIVATE );
        
        m_timerCall.Change( m_iTimeToShow/10, m_iTimeToShow/10 );
      }
    }

    public void Hide()
    {
      if( m_state != TaskbarStates.HIDDEN )
      {
        StopTimer();
        m_state = TaskbarStates.HIDDEN;
        m_form.Hide();
      }
    }
    #endregion

    #region Helper methods
    private void DrawCloseButton( Graphics grfx, ButtonState state )
    {
      if( m_bmpClose != null )
      {
        int iWidth = m_bmpClose.Size.Width / 3;
        Size size = new Size( iWidth, m_bmpClose.Height );

        Rectangle rectDest = new Rectangle( m_pntClose, size );
        Rectangle rectSrc;

        switch( state )
        {
          case ButtonState.Pushed:
            rectSrc = new Rectangle(new Point( iWidth*2, 0 ), size );
            break;

          case ButtonState.Inactive:
            rectSrc = new Rectangle( new Point( iWidth, 0 ), size );
            break;

          case ButtonState.Normal:
          default:
            rectSrc = new Rectangle( new Point( 0, 0 ), size );
            break;
        }

        grfx.DrawImage( m_bmpClose, rectDest, rectSrc, GraphicsUnit.Pixel );
      }
    }

    private Region BitmapToRegion( Bitmap bitmap, Color transparencyColor )
    {
      if( bitmap == null ) return null;

      int height = bitmap.Size.Height;
      int width = bitmap.Size.Width;

      GraphicsPath path = new GraphicsPath();

      for( int j=0; j<height; j++ )
      {
        for( int i=0; i<width; i++ )
        {
          Color clrPx = bitmap.GetPixel( i, j );
          if( clrPx == transparencyColor ) continue;

          int x0 = i;

          while( ( i < width ) && ( bitmap.GetPixel( i, j ) != transparencyColor ) )
          {
            i++;
          }

          path.AddRectangle( new Rectangle( x0, j, i - x0, 1 ) );
        }
      }

      Region region = new Region( path );
      path.Dispose();
      
      return region;
    }

    private void StopTimer()
    {
      m_timerCall.Change( Timeout.Infinite, Timeout.Infinite );
    }
    #endregion

    #region Events Handlers and Overriders
    static private void Timer_Callback( object sender )
    {
      TaskbarNotifier parent = (TaskbarNotifier)sender;
      parent.Timer_OnTick( parent, EventArgs.Empty );
    }

    
    private void Timer_OnTick( object obj, EventArgs ea )
    {
      switch( m_state )
      {
        case TaskbarStates.APPEARING:
          RECT rc = new RECT();
          WindowsAPI.GetWindowRect( m_form.Handle, ref rc );
          Rectangle rcCur = (Rectangle)rc;
          Size sizeImg = m_form.BackgroundImage.Size;

          Trace.WriteLine( rcCur.Height.ToString(), "Hint Height" );

          if( rcCur.Height < sizeImg.Height )
          {
            uint flags = (uint)SetWindowPosFlags.SWP_NOACTIVATE |
              (uint)SetWindowPosFlags.SWP_ASYNCWINDOWPOS | 
              (uint)SetWindowPosFlags.SWP_SHOWWINDOW;

            bool result = WindowsAPI.SetWindowPos( m_form.Handle, 
              new IntPtr( (int)SetWindowPosZOrder.HWND_TOPMOST ),
              rcCur.Left,
              rcCur.Top - iHeightIncriment, sizeImg.Width, 
              rcCur.Height + iHeightIncriment, flags );
          }
          else
          {
            StopTimer();
            m_form.Height = sizeImg.Height;
            m_state = TaskbarStates.VISIBLE;
            
            // execute timeronly once
            m_timerCall.Change( m_iTimeToStay, 0 ); 
          }
          break;

        case TaskbarStates.VISIBLE:
          StopTimer();
          m_state = TaskbarStates.DISAPPEARING;
          m_timerCall.Change( m_iTimeToHide/10, m_iTimeToHide/10 );
          break;

        case TaskbarStates.DISAPPEARING:
          Rectangle rcWork = Screen.GetWorkingArea( m_form );
          
          if( m_form.Top < rcWork.Bottom )
          {
            m_form.SetBounds( m_form.Left, m_form.Top + iHeightDecriment, 
              m_form.Width, m_form.Height - iHeightDecriment );
          }
          else
          {
            Hide();
          }
          break;
      }
    }

    private void Form_Paint( object sender, PaintEventArgs pe )
    {
      Graphics g = pe.Graphics;
      DrawCloseButton( g, m_btnState );
    }
    
    private void Form_Click( object sender, EventArgs e )
    {
      Point pnt = Control.MousePosition;
      pnt = m_form.PointToClient( pnt );

      if( m_rectClose.Contains( pnt ) )
      {
        RaiseOnCloseClickEvent();
      }
    }
    private void Title_Click( object sender, EventArgs e )
    {
      RaiseOnTitleClickEvent();
    }

    private void Content_Click( object sender, EventArgs e )
    {
      RaiseOnContentClickEvent();
    }
    #endregion

    #region Event Raisers
    private void RaiseClosePositionChangedEvent()
    {
      if( ClosePositionChanged != null )
      {
        ClosePositionChanged( this, EventArgs.Empty );
      }
    }

    private void RaiseCloseButtonChangedEvent()
    {
      if( CloseButtonChanged != null )
      {
        CloseButtonChanged( this, EventArgs.Empty );
      }
    }

    private void RaiseTitleChangedEvent()
    {
      if( TitleChanged != null )
      {
        TitleChanged( this, EventArgs.Empty );
      }
    }

    private void RaiseContentChangedEvent()
    {
      if( ContentChanged != null )
      {
        ContentChanged( this, EventArgs.Empty );
      }
    }
    private void RaiseTimeToShowChangedEvent()
    {
      if( TimeToShowChanged != null )
      {
        TimeToShowChanged( this, EventArgs.Empty );
      }
    }
    private void RaiseTimeToStayChangedEvent()
    {
      if( TimeToStayChanged != null )
      {
        TimeToStayChanged( this, EventArgs.Empty );
      }
    }
    private void RaiseTimeToHideChangedEvent()
    {
      if( TimeToHideChanged != null )
      {
        TimeToHideChanged( this, EventArgs.Empty );
      }
    }
    
    private void RaiseOnCloseClickEvent()
    {
      if( OnCloseClick != null )
      {
        OnCloseClick( this, EventArgs.Empty );
      }
    }

    private void RaiseOnTitleClickEvent()
    {
      if( OnTitleClick != null )
      {
        OnTitleClick( this, EventArgs.Empty );
      }
    }

    private void RaiseOnContentClickEvent()
    {
      if( OnContentClick != null )
      {
        OnContentClick( this, EventArgs.Empty );
      }
    }

    private void RaiseTitlePositionChangedEvent()
    {
      if( TitlePositionChanged != null )
      {
        TitlePositionChanged( this, EventArgs.Empty );
      }
    }
    private void RaiseContentPositionChangedEvent()
    {
      if( ContentPositionChanged != null )
      {
        ContentPositionChanged( this, EventArgs.Empty );
      }
    }
    #endregion

    #region Special Methods
    protected void InitializeCloseButton()
    {
      if( m_bmpClose != null )
      {
        int iWidth = m_bmpClose.Size.Width / 3;
        Size size = new Size( iWidth, m_bmpClose.Height );
        m_rectClose = new Rectangle( m_pntClose, size );
      }
    }

    protected void InitializeLabelSizes()
    { /*
      StringFormat format = new StringFormat();
      format.Alignment = StringAlignment.Near;
      format.Trimming = StringTrimming.Word;

      if( m_strTitle!= null && m_strTitle.Length > 0 )
      {
        m_lblTitle.Width = m_form.Width - m_pntTitle.X;
        
        Graphics g = m_lblContent.CreateGraphics();
        SizeF size = g.MeasureString( m_strContent, m_lblTitle.Font, m_lblTitle.Width, format );
        m_lblTitle.Height = (int)size.Height;
        g.Dispose();
      }
      
      if( m_strContent != null && m_strContent.Length > 0 )
      {
        m_lblContent.Width = m_form.Width - m_pntContent.X;
        Graphics g = m_lblContent.CreateGraphics();
        SizeF size = g.MeasureString( m_strContent, m_lblContent.Font, m_lblContent.Width, format );
        m_lblContent.Height = (int)size.Height;
        g.Dispose();           
      }
      //*/
    }

    protected virtual void OnCloseButtonChanged()
    {
      InitializeCloseButton();
      RaiseCloseButtonChangedEvent();
    }

    protected virtual void OnClosePositionChanged()
    {
      InitializeCloseButton();
      RaiseClosePositionChangedEvent();
    }

    protected virtual void OnTitleChanged()
    {
      m_lblTitle.Text = m_strTitle;
      InitializeLabelSizes();
      RaiseTitleChangedEvent();
    }

    protected virtual void OnContentChanged()
    {
      m_lblContent.Text = m_strContent;
      InitializeLabelSizes();
      RaiseContentChangedEvent();
    }

    protected virtual void OnTimeToShowChanged()
    {
      RaiseTimeToShowChangedEvent();
    }

    protected virtual void OnTimeToStayChanged()
    {
      RaiseTimeToStayChangedEvent();
    }

    protected virtual void OnTimeToHideChanged()
    {
      RaiseTimeToHideChangedEvent();
    }

    protected virtual void OnTitlePositionChanged()
    {
      m_lblTitle.Location = m_pntTitle;
      InitializeLabelSizes();
      RaiseTitlePositionChangedEvent();
    }

    protected virtual void OnContentPositionChanged()
    {
      m_lblContent.Location = m_pntContent;
      InitializeLabelSizes();
      RaiseContentPositionChangedEvent();
    }
    #endregion
  }
}
