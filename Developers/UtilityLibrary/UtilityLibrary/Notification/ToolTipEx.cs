using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

using UtilityLibrary.Win32;


namespace UtilityLibrary.Notification
{
  [ ProvideProperty( "ToolTip", typeof( Control ) ),
  ToolboxBitmap( typeof( UtilityLibrary.Notification.ToolTipEx ), "UtilityLibrary.Notification.ToolTipEx.bmp" ), 
  ToolboxItem( true ) ]
  public class ToolTipEx : Component, IExtenderProvider
  {
    #region Class constants
    private const int     DEFAULT_DELAY = 500;
    private const int     RESHOW_RATIO  = 100;
    private const int     AUTOPOP_RATIO = 5000;    

    enum TToolAction
    {
      INSERT,
      REMOVE,
      UPDATE
    };
    #endregion

    #region Class members
    private IDictionary m_parents  = new Hashtable();
    private IDictionary m_controls = new Hashtable();
    
    private bool  m_bActive           = true;
    private bool  m_bShowAlways       = false;
    private int   m_iReshowDelay      = RESHOW_RATIO;
    private int   m_iInitialDelay     = DEFAULT_DELAY;
    private int   m_iAutoPopDelay     = AUTOPOP_RATIO;
    private int   m_iAutomaticDelay   = 500;

    private ToolTipWrapper  m_tooltip;
    private Regex           m_regex = new Regex( @"([^<]*)<(.*)>(.*)</\2>" );
    private StringFormat    m_format;
    #endregion

    #region Class events
    [ Category( "Property Changed" ) ]
    public event EventHandler ActiveChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler AutomaticDelayChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler AutoPopDelayChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler InitialDelayChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler ReshowDelayChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler ShowAlwaysChanged;
    [ Category( "Actions" ) ]
    public event EventHandler OnShow;
    [ Category( "Actions" ) ]
    public event EventHandler OnHide;
    #endregion

    #region Class Properties
    /// <summary>
    /// Gets or sets a value indicating whether the ToolTip is currently active
    /// </summary>
    [ Category( "Behaviour" ), 
    DefaultValue( true ), 
    Description( "Gets or sets a value indicating whether the ToolTip is currently active" ) ]
    public bool Active
    {
      get
      {
        return m_bActive;
      }
      set
      {
        if( value != m_bActive )
        {
          m_bActive = value;
          OnActiveChanged();
        }
      }
    }

    /// <summary>
    /// Gets or sets the automatic delay for the ToolTip.
    /// </summary>
    [ Category( "Behaviour" ), 
    DefaultValue( 500 ),
    RefreshProperties( RefreshProperties.All ),
    Description( "Gets or sets the automatic delay for the ToolTip." ) ]
    public int AutomaticDelay
    {
      get
      {
        return m_iAutomaticDelay;
      }
      set
      {
        if( value != m_iAutomaticDelay )
        {
          m_iAutomaticDelay = value;
          OnAutomaticDelayChanged();
        }
      }
    }
    
    /// <summary>
    /// Gets or sets the period of time the ToolTip remains visible if 
    /// the mouse pointer is stationary within a control with specified 
    /// ToolTip text.
    /// </summary>
    [ Category( "Behaviour" ), 
    DefaultValue( 5000 ), 
    Description( "Gets or sets the period of time the ToolTip remains visible if the mouse pointer is stationary within a control with specified ToolTip text." ) ]
    public int AutoPopDelay
    {
      get
      {
        return m_iAutoPopDelay;
      }
      set
      {
        if( value != m_iAutoPopDelay )
        {
          m_iAutoPopDelay = value;
          OnAutoPopDelayChanged();
        }
      }
    }

    /// <summary>
    /// Gets or sets the time that passes before the ToolTip appears.
    /// </summary>
    [ Category( "Behaviour" ), 
    DefaultValue( 500 ), 
    Description( "Gets or sets the time that passes before the ToolTip appears" ) ]
    public int InitialDelay
    {
      get
      {
        return m_iInitialDelay;
      }
      set
      {
        if( value != m_iInitialDelay )
        {
          m_iInitialDelay = value;
          OnInitialDelayChanged();
        }
      }
    }

    /// <summary>
    /// Gets or sets the length of time that must transpire before subsequent 
    /// ToolTip windows appear as the mouse pointer moves from one control 
    /// to another.
    /// </summary>
    [ Category( "Behaviour" ), 
    DefaultValue( 100 ), 
    Description( "Gets or sets the length of time that must transpire before subsequent ToolTip windows appear as the mouse pointer moves from one control to another." ) ]
    public int ReshowDelay
    {
      get
      {
        return m_iReshowDelay;
      }
      set
      {
        if( value != m_iReshowDelay )
        {
          m_iReshowDelay = value;
          OnReshowDelayChanged();
        }

      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether a ToolTip window is displayed 
    /// even when its parent control is not active.
    /// </summary>
    [ Category( "Behaviour" ), 
    DefaultValue( false ), 
    Description( "Gets or sets a value indicating whether a ToolTip window is displayed even when its parent control is not active" ) ]
    public bool ShowAlways
    {
      get
      {
        return m_bShowAlways;
      }
      set
      {
        if( value != m_bShowAlways )
        {
          m_bShowAlways = value;
          OnShowAlwaysChanged();
        }

      }
    }
    #endregion

    #region Internal Classes
    internal class SubclassRedirector : NativeWindow
    {
      #region Class members
      private ToolTipEx m_parent;
      #endregion

      #region Class Overrides
      public SubclassRedirector( ToolTipEx parent )
      {
        m_parent = parent;
      }

      protected override void WndProc(ref System.Windows.Forms.Message m)
      {
        base.WndProc( ref m );
        m_parent.WndProc( ref m );
      }
      #endregion
    }

    internal class ToolTipWrapper : Control
    {
      #region Class constants
      private const string  TOOLTIPS_CLASS = "tooltips_class32";
      #endregion
      
      #region Class members
      private ToolTipEx m_parent;
      #endregion

      #region Class constructor and overrided methods
      public ToolTipWrapper( ToolTipEx parent )
      {
        ControlStyles styleTrue = ControlStyles.AllPaintingInWmPaint |
          ControlStyles.DoubleBuffer |
          ControlStyles.EnableNotifyMessage |
          ControlStyles.UserPaint;

        SetStyle( styleTrue, true );
        SetStyle( ControlStyles.Selectable, false );

        m_parent = parent;
      }

      /// <summary>
      /// Params which used in creation of tooltip
      /// </summary>
      protected override System.Windows.Forms.CreateParams CreateParams
      {
        get
        {
          CreateParams param = base.CreateParams;

          param.ClassName = TOOLTIPS_CLASS;
          param.ExStyle = (int)WindowExStyles.WS_EX_TOPMOST;

          // param.Style = -2147483645;
          unchecked 
          {
            param.Style = (int)WindowStyles.WS_POPUP | 
              (int)ToolTipStyles.TTS_NOPREFIX | 
              (int)ToolTipStyles.TTS_ALWAYSTIP;
          }
 
          return param;
        }
      }

      /// <summary>
      /// Initialize Common control and tooltip window
      /// </summary>
      protected override void CreateHandle()
      {
        InitializeCommonControls();
        base.CreateHandle();

        WindowsAPI.SetWindowPos( this.Handle, new IntPtr( (int)SetWindowPosZOrder.HWND_TOPMOST ),
          0, 0, 0, 0, (uint)SetWindowPosFlags.SWP_NOMOVE | 
          (uint)SetWindowPosFlags.SWP_NOSIZE | 
          (uint)SetWindowPosFlags.SWP_NOACTIVATE );
      }
      /// <summary>
      /// Initialize Windows common controls before try to use them
      /// </summary>
      /// <returns>TRUE - if successful, FALSE - fail</returns>
      private bool InitializeCommonControls()
      {
        INITCOMMONCONTROLSEX icex = new INITCOMMONCONTROLSEX();
        icex.dwSize = Marshal.SizeOf( typeof( INITCOMMONCONTROLSEX ) );
        icex.dwICC = (int)CommonControlInitFlags.ICC_WIN95_CLASSES;
      
        bool bStatus = WindowsAPI.InitCommonControlsEx( icex );

        return bStatus;
      }

      /// <summary>
      /// Redirect painting to Component
      /// </summary>
      /// <param name="e"></param>
      protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
      {
        m_parent.OnPaint( this.Text, e );
      }

      /// <summary>
      /// Redirect painting to Component
      /// </summary>
      /// <param name="pevent"></param>
      protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
      {
        m_parent.OnPaintBackground( this.Text, pevent );
      }
      #endregion
    }

    internal protected class ToolTipStringFormatter : ICloneable
    {
      private ToolTipEx m_parent;

      public FontFamily family;
      public FontStyle  style;
      public float      size;
      public Color      color;

      private ToolTipStringFormatter(){}
      public  ToolTipStringFormatter( ToolTipEx parent )
      {
        m_parent = parent;
        Reset();
      }

      
      public void   Reset()
      {
        family= m_parent.m_tooltip.Font.FontFamily;
        style = m_parent.m_tooltip.Font.Style;
        size  = m_parent.m_tooltip.Font.Size;
        color = SystemColors.InfoText;
      }

      public object Clone()
      {
        return this.MemberwiseClone();
      }

      public void   CopyFrom( ToolTipStringFormatter obj )
      {
        if( obj == this ) return;

        family = obj.family; 
        style  = obj.style;  
        size   = obj.size;   
        color  = obj.color;  
      }


      public static implicit operator Color( ToolTipStringFormatter obj )
      {
        return obj.color;
      }

      public static implicit operator Font( ToolTipStringFormatter obj )
      {
        return new Font( obj.family, obj.size, obj.style );
      }

    }
    #endregion

    #region Class Initialize/Finilize methods
    public ToolTipEx()
    {
      m_format = new StringFormat();
      m_format.Alignment      = StringAlignment.Near;
      m_format.LineAlignment  = StringAlignment.Near;
      m_format.Trimming       = StringTrimming.None;
      m_format.FormatFlags    = StringFormatFlags.LineLimit;
      m_format.HotkeyPrefix   = HotkeyPrefix.Hide;

      m_tooltip = new ToolTipWrapper( this );
      SetTimerSettings( ToolTipsDelays.TTDT_AUTOMATIC, AutomaticDelay );
    }

    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
        m_tooltip.Dispose();
        m_controls.Clear();

        foreach( SubclassRedirector tool in m_parents.Values )
        {
          tool.ReleaseHandle();
        }
      }
      
      base.Dispose( disposing );
    }
    #endregion

    #region ToolTip Property Provider methods
    public string GetToolTip( Control ctrl )
    {
      return (string)m_controls[ ctrl ];
    }

    public void   SetToolTip( Control ctrl, string value )
    {
      bool bNew = ( m_controls.Contains( ctrl ) == false );
      m_controls[ ctrl ] = value;

      if( ctrl.IsHandleCreated == false )
      {
        if( value == null || value.Length == 0 )
        { 
          m_controls.Remove( ctrl );
          return;
        }

        ctrl.HandleCreated += new EventHandler( OnControlHandleCreated );
      }
      else
      {
        TooltipAction( ctrl, value, 
          ( value == null || value.Length == 0 ) ? TToolAction.REMOVE : 
          ( ( bNew == true ) ? TToolAction.INSERT : TToolAction.UPDATE ) );
      }
    }

    public bool   CanExtend( object extendee )
    {
      // if extendee is Control class child then we can provide our
      // services for it
      return ( extendee is Control );
    }

    
    private void OnControlHandleCreated( object sender, EventArgs e )
    {
      Control ctrl = ( Control )sender;
      string  text = (string)m_controls[ sender ];

      // detach event
      ctrl.HandleCreated -= new EventHandler( OnControlHandleCreated );

      TooltipAction( ctrl, text, 
        ( text == null || text.Length == 0 ) ? TToolAction.REMOVE : TToolAction.INSERT );
    }

    private void TooltipAction( Control ctrl, string text, TToolAction action )
    {
      if( ctrl.Parent != null )
      {
        AttachToParent( ctrl.Parent.Handle );

        Rectangle clientRect = new Rectangle( ctrl.Location, ctrl.Size );

        TOOLINFO info = new TOOLINFO();
        info.cbSize   = (uint)Marshal.SizeOf( typeof( TOOLINFO ) );
        info.uFlags   = (uint)ToolTipFlags.TTF_IDISHWND | (uint)ToolTipFlags.TTF_SUBCLASS;
        info.hwnd     = ctrl.Parent.Handle;
        info.uId      = ctrl.Handle;
        info.lpszText = text; //( text != null ) ? Marshal.StringToHGlobalAuto( text ) : IntPtr.Zero;
        info.rect     = (RECT)clientRect;

        int msg = 0;

        switch( action )
        {
          case TToolAction.INSERT: 
            msg = (int)ToolTipMsg.TTM_ADDTOOLW; 
            Trace.WriteLine( "Add", "TOOL ACTION" );
            break;
          
          case TToolAction.UPDATE: 
            msg = (int)ToolTipMsg.TTM_UPDATETIPTEXTW; 
            Trace.WriteLine( "Update Text", "TOOL ACTION" );
            break;
          
          case TToolAction.REMOVE: 
            msg = (int)ToolTipMsg.TTM_DELTOOLW; 
            Trace.WriteLine( "Add", "TOOL ACTION" );
            break;
        }

        int result = WindowsAPI.SendMessage( m_tooltip.Handle, msg, 0, ref info );

        if( result == 0 )
        {
          Trace.WriteLine( "Tool Action status FAIL!!!", "Notification" );
        }
      }
    }

    private void AttachToParent( IntPtr parentHandle )
    {
      if( m_parents.Contains( parentHandle ) == false )
      {
        // create subclass utility class
        SubclassRedirector tool = new SubclassRedirector( this );
        
        // subclass parentHandle window
        tool.AssignHandle( parentHandle );

        // store in list parents
        m_parents[ parentHandle ] = tool;
      }
    }
    private void SetTimerSettings( ToolTipsDelays flag, int time )
    {
      WindowsAPI.SendMessage( m_tooltip.Handle, 
        (int)ToolTipMsg.TTM_SETDELAYTIME, (int)flag, time );
    }
    #endregion

    #region Class event raisers
    private void RaiseActiveChangedEvent()
    {
      if( ActiveChanged != null )
      {
        ActiveChanged( this, EventArgs.Empty );
      }
    }
    
    private void RaiseAutomaticDelayChangedEvent()
    {
      if( AutomaticDelayChanged != null )
      {
        AutomaticDelayChanged( this, EventArgs.Empty );
      }
    }
    private void RaiseAutoPopDelayChangedEvent()
    {
      if( AutoPopDelayChanged != null )
      {
        AutoPopDelayChanged( this, EventArgs.Empty );
      }
    }
    private void RaiseInitialDelayChangedEvent()
    {
      if( InitialDelayChanged != null )
      {
        InitialDelayChanged( this, EventArgs.Empty );
      }
    }
    private void RaiseReshowDelayChangedEvent()
    {
      if( ReshowDelayChanged != null )
      {
        ReshowDelayChanged( this, EventArgs.Empty );
      }
    }
    private void RaiseShowAlwaysChangedEvent()
    {
      if( ShowAlwaysChanged != null )
      {
        ShowAlwaysChanged( this, EventArgs.Empty );
      }
    }

    private void RaiseOnShowEvent()
    {
      if( OnShow != null )
      {
        OnShow( this, EventArgs.Empty );
      }
    }

    private void RaiseOnHideEvent()
    {
      if( OnHide != null )
      {
        OnHide( this, EventArgs.Empty );
      }
    }
    #endregion

    #region Class virtual methods
    protected virtual void OnActiveChanged()
    {
      RaiseActiveChangedEvent();
    }

    protected virtual void OnAutomaticDelayChanged()
    {
      AutoPopDelay = AutomaticDelay * 10;
      m_iInitialDelay = AutomaticDelay;
      m_iReshowDelay  = (int)(AutomaticDelay / 5);

      SetTimerSettings( ToolTipsDelays.TTDT_AUTOMATIC, AutomaticDelay );

      RaiseAutomaticDelayChangedEvent();
    }

    protected virtual void OnAutoPopDelayChanged()
    {
      SetTimerSettings( ToolTipsDelays.TTDT_AUTOPOP, AutoPopDelay );

      RaiseAutoPopDelayChangedEvent();
    }

    protected virtual void OnInitialDelayChanged()
    {
      SetTimerSettings( ToolTipsDelays.TTDT_INITIAL, InitialDelay );

      RaiseInitialDelayChangedEvent();
    }

    protected virtual void OnReshowDelayChanged()
    {
      SetTimerSettings( ToolTipsDelays.TTDT_RESHOW, ReshowDelay );

      RaiseReshowDelayChangedEvent();
    }

    protected virtual void OnShowAlwaysChanged()
    {
      RaiseShowAlwaysChangedEvent();
    }
    #endregion

    #region Class worker methods
    /// <summary>
    /// Function try detect is input string has formatting or not
    /// </summary>
    /// <param name="text">String for check</param>
    /// <returns>TRUE - if formatting detected</returns>
    protected bool IsStringWithFormat( string text )
    {
      MatchCollection coll = m_regex.Matches( text );
      return ( coll.Count > 0 );
    }

    /// <summary>
    /// Our WndProc which raise to user OnShow and OnHide events
    /// </summary>
    /// <param name="m"></param>
    protected virtual void WndProc( ref System.Windows.Forms.Message m )
    {
      if( m.Msg == (int)Msg.WM_NOTIFY )
      {
        NMHDR note = (NMHDR)m.GetLParam(typeof(NMHDR));

        if( note.code == (int)ToolTipNotifyMsg.TTN_SHOW )
        {
          m.Result = IntPtr.Zero;
          RaiseOnShowEvent();
        }
        else if( note.code == (int)ToolTipNotifyMsg.TTN_POP )
        {
          RaiseOnHideEvent();
        }
      }
    }

    /// <summary>
    /// Paint text and background of tooltip
    /// </summary>
    /// <param name="text"></param>
    /// <param name="e"></param>
    protected virtual void OnPaint( string text, System.Windows.Forms.PaintEventArgs e )
    {
      OnPaintBackground( text, e );

      Rectangle rc = m_tooltip.ClientRectangle;
      Graphics  g = e.Graphics;
      string[]  multi = text.Split( '\n' );
      Point     pnt = new Point( 0 );

      foreach( string txt in multi )
      {
        DrawToolTipString( g, pnt, txt );
        pnt.Y += MeasureToolTipString( g, txt ).Height;
      }
    }

    /// <summary>
    /// Paint background of tooltip
    /// </summary>
    /// <param name="text"></param>
    /// <param name="pevent"></param>
    protected virtual void OnPaintBackground( string text, System.Windows.Forms.PaintEventArgs pevent )
    {
      Size size = MeasureToolTip( text );

      if( m_tooltip.Width != size.Width || m_tooltip.Height != size.Height )
      {
        WindowsAPI.SetWindowPos( m_tooltip.Handle, 
          new IntPtr( (int)SetWindowPosZOrder.HWND_TOPMOST ),
          0, 0, size.Width, size.Height, (uint)SetWindowPosFlags.SWP_NOMOVE | 
          (uint)SetWindowPosFlags.SWP_NOACTIVATE );

        return;
      }

      Graphics g = pevent.Graphics;
      Rectangle rc = m_tooltip.ClientRectangle;

      g.FillRectangle( SystemBrushes.Info, rc );
    }

    /// <summary>
    /// Function detect formatting and accordingly to it change formatting 
    /// class states
    /// </summary>
    /// <param name="frmt">formatting class</param>
    /// <param name="id">string Identificator of formatter</param>
    protected virtual void DetectFormatting( ref ToolTipStringFormatter frmt, string id )
    {
      if( string.Compare( id, "b", true ) == 0 ) // bold
      {
        frmt.style |= FontStyle.Bold;
      }
      else if( string.Compare( id, "i", true ) == 0 ) // italic
      {
        frmt.style |= FontStyle.Italic;
      }
      else if( string.Compare( id, "u", true ) == 0 ) // underline
      {
        frmt.style |= FontStyle.Underline;
      }
      else // try to detect is it a color name
      {
        try
        {
          Color clr = Color.FromName( id );
          frmt.color = clr;
        }
        catch{}
      }
    }

    /// <summary>
    /// Calculate size which needed to display tooltip
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    protected virtual Size MeasureToolTip( string text )
    {
      string[] multi = text.Split( '\n' );

      int      maxWidth = 0;
      int      totalHeight = 0;

      Bitmap bmp = new Bitmap( 1, 1 );
      Graphics g = Graphics.FromImage( bmp );
      Size sizeTmp;

      foreach( string txt in multi )
      {
        sizeTmp = MeasureToolTipString( g, txt );
        maxWidth = Math.Max( maxWidth, sizeTmp.Width );
        totalHeight += sizeTmp.Height;
      }

      g.Dispose();
      bmp.Dispose();

      return new Size( maxWidth, totalHeight + 4 );
    }

    /// <summary>
    /// Measure one line of tooltip text calcuating formatting
    /// </summary>
    /// <param name="g"></param>
    /// <param name="txt"></param>
    /// <returns></returns>
    protected virtual Size MeasureToolTipString( Graphics g, string txt )
    {
      Size sizeText;
      int maxHeight = 0;
      int totalWidht = 0;

      string  tempString = txt;
      Stack   stack = new Stack();

      ToolTipStringFormatter frmtString = new ToolTipStringFormatter( this );
      
      stack.Push( frmtString.Clone() );
      
      do
      {
        Match   mt = m_regex.Match( tempString );
        
        if( mt.Success == true )
        {
          string pretext  = mt.Groups[1].Value;
          string formatId = mt.Groups[2].Value;
          string text     = mt.Groups[3].Value;

          tempString = tempString.Remove( 0, mt.Groups[0].Value.Length );
          frmtString.CopyFrom( (ToolTipStringFormatter)stack.Peek() );

          sizeText = MeasureStringByFormat( g, frmtString, pretext );
          maxHeight = Math.Max( maxHeight, sizeText.Height );
          totalWidht += sizeText.Width;

          DetectFormatting( ref frmtString, formatId );

          if( IsStringWithFormat( text ) )
          {
            stack.Push( frmtString.Clone() );
            tempString = text + "</stack>" + tempString;
            continue;
          }
          else
          {
            int pos = text.IndexOf( "</stack>" );
            int left = pos + "</stack>".Length;
            
            if( pos != -1 )
            {
              tempString = text.Substring( left, text.Length - left ) + tempString;
              text = text.Substring( 0, pos );
            }

            sizeText = MeasureStringByFormat( g, frmtString, text );
            maxHeight = Math.Max( maxHeight, sizeText.Height );
            totalWidht += sizeText.Width;
            
            if( pos != -1 ) stack.Pop();
          }
        }
        else
        {
          int pos = 0;
          
          while( -1 != ( pos = tempString.IndexOf( "</stack>" ) ) )
          {
            int left = pos + "</stack>".Length;
            string text = tempString.Substring( 0, pos );
            
            tempString = tempString.Substring( left, tempString.Length - left );

            sizeText = MeasureStringByFormat( g, (ToolTipStringFormatter)stack.Peek(), text );
            maxHeight = Math.Max( maxHeight, sizeText.Height );
            totalWidht += sizeText.Width;

            stack.Pop();
          }
          
          sizeText = MeasureStringByFormat( g, (ToolTipStringFormatter)stack.Peek(), tempString );
          maxHeight = Math.Max( maxHeight, sizeText.Height );
          totalWidht += sizeText.Width;
          tempString = "";
        }
        
        if( tempString.Length == 0 ) break;        
      }
      while( true );

      stack.Clear();      
      
      return new Size( totalWidht, maxHeight );
    }

    /// <summary>
    /// Measure size of tooltip string accorfing to formatting
    /// </summary>
    /// <param name="g"></param>
    /// <param name="frmt"></param>
    /// <param name="txt"></param>
    /// <returns></returns>
    protected virtual Size MeasureStringByFormat( Graphics g, ToolTipStringFormatter frmt, string txt )
    {
      SizeF size = g.MeasureString( txt, (Font)frmt, 0, m_format );
      return new Size( (int)( size.Width + 0.5 ), (int)( size.Height + 0.5 ) );
    }

    /// <summary>
    /// Draw one line of tooltip text with formatting
    /// </summary>
    /// <param name="g"></param>
    /// <param name="pntStart"></param>
    /// <param name="txt"></param>
    protected virtual void DrawToolTipString( Graphics g, Point pntStart, string txt )
    {
      string  tempString = txt;
      Stack   stack = new Stack();

      ToolTipStringFormatter frmtString = new ToolTipStringFormatter( this );
      
      stack.Push( frmtString.Clone() );
      
      do
      {
        Match   mt = m_regex.Match( tempString );
        
        if( mt.Success == true )
        {
          string pretext  = mt.Groups[1].Value;
          string formatId = mt.Groups[2].Value;
          string text     = mt.Groups[3].Value;

          tempString = tempString.Remove( 0, mt.Groups[0].Value.Length );
          frmtString.CopyFrom( (ToolTipStringFormatter)stack.Peek() );

          DrawStringByFormat( g, ref pntStart, frmtString, pretext );
          DetectFormatting( ref frmtString, formatId );

          if( IsStringWithFormat( text ) )
          {
            stack.Push( frmtString.Clone() );
            tempString = text + "</stack>" + tempString;
            continue;
          }
          else
          {
            int pos = text.IndexOf( "</stack>" );
            int left = pos + "</stack>".Length;
            
            if( pos != -1 )
            {
              tempString = text.Substring( left, text.Length - left ) + tempString;
              text = text.Substring( 0, pos );
            }

            DrawStringByFormat( g, ref pntStart, frmtString, text );
            
            if( pos != -1 ) stack.Pop();
          }
        }
        else
        {
          int pos = 0;
          
          while( -1 != ( pos = tempString.IndexOf( "</stack>" ) ) )
          {
            int left = pos + "</stack>".Length;
            string text = tempString.Substring( 0, pos );
            
            tempString = tempString.Substring( left, tempString.Length - left );

            DrawStringByFormat( g, ref pntStart, (ToolTipStringFormatter)stack.Peek(), text );
            stack.Pop();
          }
          
          DrawStringByFormat( g, ref pntStart, (ToolTipStringFormatter)stack.Peek(), tempString );
          tempString = "";
        }
        
        if( tempString.Length == 0 ) break;        
      }
      while( true );

      stack.Clear();
    }

    /// <summary>
    /// Draw string accordingly to formatting
    /// </summary>
    /// <param name="g"></param>
    /// <param name="start"></param>
    /// <param name="frmt"></param>
    /// <param name="text"></param>
    protected virtual void DrawStringByFormat( Graphics g, ref Point start, 
      ToolTipStringFormatter frmt, string text )
    {
      Font  newFont = (Font)frmt;
      Color clr = (Color)frmt;

      SizeF size = g.MeasureString( text, newFont, 0, m_format );
      g.DrawString( text, newFont, new SolidBrush( clr ), start );
      start.X += ( int )( size.Width + 0.5 );
    }
    #endregion
  }
}
