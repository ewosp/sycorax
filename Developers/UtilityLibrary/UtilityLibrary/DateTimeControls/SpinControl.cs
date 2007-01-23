using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;

using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;

using UtilityLibrary.Win32;


namespace UtilityLibrary.DateTimeControls
{
  [ToolboxItem(false)]
  public class SpinControl : Control
  {
    #region Class constants
    private const string DEF_UPDOWN_CLASSW = "msctls_updown32";
    private const int ARROW_WIDTH = 12;
    #endregion

    #region Class members
    private UDACCEL m_stAccel;
    private int     m_iBase = 10;
    private Control m_cBuddy;
    private int     m_iPos;
    private int     m_iMin = 0;
    private int     m_iMax = 100;

    private bool    m_bSkipEvents;

    private Rectangle m_rcUpArrow = Rectangle.Empty;
    private Rectangle m_rcDownArrow = Rectangle.Empty;

    private ButtonState m_stateUp = ButtonState.Flat;
    private ButtonState m_stateDown = ButtonState.Flat;
    #endregion

    #region Class Properties
    [Browsable(false)]
    public UDACCEL Accel
    {
      get
      {
        return m_stAccel;
      }
      set
      {
        m_stAccel = value;
        OnAccelChanged();
      }
    }

    [DefaultValue(10)]
    public int Base
    {
      get
      {
        return m_iBase;
      }
      set
      {
        if( value != m_iBase )
        {
          m_iBase = value;
          OnBaseChanged();
        }
      }
    }

    [DefaultValue(null)]
    public Control Buddy
    {
      get
      {
        return m_cBuddy;
      }
      set
      {
        //if( value != m_cBuddy )
        //{
        m_cBuddy = value;
        OnBuddyChanged();
        //}
      }
    }

    [DefaultValue(0)]
    public int Pos
    {
      get
      {
        return m_iPos;
      }
      set
      {
        if( value != m_iPos )
        {
          m_iPos = value;
          OnPosChanged();
        }
      }
    }

    [DefaultValue(0)]
    public int Min
    {
      get
      {
        return m_iMin;
      }
      set
      {
        if( value != m_iMin )
        {
          m_iMin = value;
          OnMinChanged();
        }
      }
    }

    [DefaultValue(100)]
    public int Max
    {
      get
      {
        return m_iMax;
      }
      set
      {
        if( value != m_iMax )
        {
          m_iMax = value;
          OnMaxChanged();
        }
      }
    }
    
    [Browsable(false), DefaultValue(false)]
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

    #endregion

    #region Class Events
    public event EventHandler Changed;
    public event EventHandler AccelChanged;
    public event EventHandler BaseChanged;
    public event EventHandler BuddyChanged;
    public event EventHandler PosChanged;
    public event EventHandler MinChanged;
    public event EventHandler MaxChanged;
    #endregion

    #region Class Initialize/Finalize methods
    public SpinControl()
    {
      ControlStyles styleTrue = ControlStyles.AllPaintingInWmPaint |
        ControlStyles.DoubleBuffer |
        ControlStyles.EnableNotifyMessage |
        ControlStyles.UserPaint;

      SetStyle( styleTrue, true );

      SetStyle( ControlStyles.Selectable, false );
      
      this.TabStop = false;
    }

    protected override System.Windows.Forms.CreateParams CreateParams
    {
      get
      {
        CreateParams param = base.CreateParams;

        param.ClassName = DEF_UPDOWN_CLASSW;
        param.ExStyle = 0;

        param.Style = (int)WindowStyles.WS_CHILD |
          (int)WindowStyles.WS_VISIBLE |
          (int)WindowStyles.WS_CLIPCHILDREN |
          (int)WindowStyles.WS_CLIPSIBLINGS |
          (int)SpinControlStyles.UDS_SETBUDDYINT |
          (int)SpinControlStyles.UDS_ARROWKEYS |
          //(int)SpinControlStyles.UDS_AUTOBUDDY |
          (int)SpinControlStyles.UDS_NOTHOUSANDS;

        return param;
      }
    }

    protected override void CreateHandle() 
    {
      // Make sure common control library initilizes updown control
      if( !RecreatingHandle )
      {
        INITCOMMONCONTROLSEX icex = new INITCOMMONCONTROLSEX();
        icex.dwSize = Marshal.SizeOf(typeof(INITCOMMONCONTROLSEX));
        icex.dwICC = (int)CommonControlInitFlags.ICC_WIN95_CLASSES;

        WindowsAPI.InitCommonControlsEx(icex);
      }
      
      base.CreateHandle();
    }
    
    protected override void WndProc(ref System.Windows.Forms.Message m)
    {
      if( m.Msg == (int)Msg.WM_USER + 7246 )
      {
        Pos = WindowsAPI.SendMessage( base.Handle, (int)SpinControlMsg.UDM_GETPOS, 0, 0 ) & 0xff;
      }
      else if( m.Msg == (int)Msg.WM_USER + 7445 )
      {
        int oldPos = m_iPos;
        base.WndProc( ref m );
        m_iPos = ( ( m.WParam.ToInt32() >> 16 ) & 0xffff );

        if( oldPos != m_iPos )
        {
          RaisePosChangedEvent();
        }

        return;
      }

      base.WndProc( ref m );
    }
    #endregion

    #region Class Helper methods
    protected virtual void OnAccelChanged()
    {
      WindowsAPI.SendMessage( base.Handle, SpinControlMsg.UDM_SETACCEL, 1, ref m_stAccel );
      RaiseAccelChangedEvent();
    }
    
    protected virtual void OnBaseChanged()
    {
      WindowsAPI.SendMessage( base.Handle, (int)SpinControlMsg.UDM_SETBASE, m_iBase, 0 );
      RaiseBaseChangedEvent();
    }
    
    protected virtual void OnBuddyChanged()
    {
      WindowsAPI.SendMessage( base.Handle, (int)SpinControlMsg.UDM_SETBUDDY, 
        (m_cBuddy != null) ? m_cBuddy.Handle : IntPtr.Zero, IntPtr.Zero );

      //if( m_cBuddy != null )
      //{
      //  this.Left = m_cBuddy.Right - this.Width;
      //  this.Height = m_cBuddy.Height;
      //  this.Top = m_cBuddy.Top;
      //}
      
      RaiseBuddyChangedEvent();
    }

    protected virtual void OnPosChanged()
    {
      WindowsAPI.SendMessage( base.Handle, (int)SpinControlMsg.UDM_SETPOS, 0, m_iPos );
      RaisePosChangedEvent();
    }
    
    protected virtual void OnMinChanged()
    {
      int result = WindowsAPI.SendMessage( base.Handle, (int)SpinControlMsg.UDM_GETRANGE, 0, 0 );
      IntPtr tmp = new IntPtr( (m_iMin << 16) + m_iMax );
      WindowsAPI.SendMessage( base.Handle, (int)SpinControlMsg.UDM_SETRANGE, 0, tmp );
      RaiseMinChangedEvent();
    }
    
    protected virtual void OnMaxChanged()
    {
      int result = WindowsAPI.SendMessage( base.Handle, (int)SpinControlMsg.UDM_GETRANGE, 0, 0 );
      IntPtr tmp = new IntPtr( (m_iMin << 16) + m_iMax );
      WindowsAPI.SendMessage( base.Handle, (int)SpinControlMsg.UDM_SETRANGE, 0, tmp );
      RaiseMaxChangedEvent();
    }

    protected virtual void OnQuietModeChanged()
    {
      // nothing here  
    }

    
    protected void RaiseAccelChangedEvent()
    {
      if( AccelChanged != null && m_bSkipEvents == false )
      {
        AccelChanged( this, EventArgs.Empty );
      }
        
      RaiseChangedEvent();
    }

    protected void RaiseBaseChangedEvent()
    {
      if( BaseChanged != null && m_bSkipEvents == false )
      {
        BaseChanged( this, EventArgs.Empty );
      }
        
      RaiseChangedEvent();
    }

    protected void RaiseBuddyChangedEvent()
    {
      if( BuddyChanged != null && m_bSkipEvents == false )
      {
        BuddyChanged( this, EventArgs.Empty );
      }
        
      RaiseChangedEvent();
    }

    protected void RaisePosChangedEvent()
    {
      if( PosChanged != null && m_bSkipEvents == false )
      {
        PosChanged( this, EventArgs.Empty );
      }
        
      RaiseChangedEvent();
    }

    protected void RaiseMinChangedEvent()
    {
      if( MinChanged != null && m_bSkipEvents == false )
      {
        MinChanged( this, EventArgs.Empty );
      }
        
      RaiseChangedEvent();
    }

    protected void RaiseMaxChangedEvent()
    {
      if( MaxChanged != null && m_bSkipEvents == false )
      {
        MaxChanged( this, EventArgs.Empty );
      }
        
      RaiseChangedEvent();
    }

    protected void RaiseChangedEvent()
    {
      if( Changed != null && m_bSkipEvents == false )
      {
        Changed( this, EventArgs.Empty );
      }
    }

    #endregion

    protected override void OnPaint( System.Windows.Forms.PaintEventArgs e )
    {
      Graphics g = e.Graphics;
      Rectangle rc = Rectangle.Inflate( ClientRectangle, -1, -1 );

      g.FillRectangle( ( Enabled == true ) ? SystemBrushes.Window : SystemBrushes.Control, 
        ClientRectangle );

      if( Rectangle.Empty == m_rcUpArrow )
      {
        m_rcUpArrow = new Rectangle( rc.X, rc.Y, rc.Width, (rc.Height-1)/2 );
      }

      if( Rectangle.Empty == m_rcDownArrow )
      {
        m_rcDownArrow = new Rectangle( rc.X, rc.Y + (rc.Height-1)/2 + 1, rc.Width, (rc.Height-1)/2 );
      }

      ControlPaint.DrawScrollButton( g, m_rcUpArrow, ScrollButton.Up, 
        ( Enabled ) ? m_stateUp : ButtonState.Inactive | ButtonState.Flat );
      
      ControlPaint.DrawScrollButton( g, m_rcDownArrow, ScrollButton.Down, 
        ( Enabled ) ? m_stateDown : ButtonState.Inactive | ButtonState.Flat );
    }

    protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
    {
      Point pnt = new Point( new Size(e.X, e.Y) );
      
      if( m_rcUpArrow.Contains( pnt ) )
      {
        m_stateUp = ButtonState.Pushed | ButtonState.Flat;
        m_stateDown = ButtonState.Flat;
        Invalidate();
      }
      else if( m_rcDownArrow.Contains( pnt ) )
      {
        m_stateUp = ButtonState.Flat;
        m_stateDown = ButtonState.Pushed | ButtonState.Flat;
        Invalidate();
      }
    }

    protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
    {
      if( m_stateUp != ButtonState.Flat )
      {
        m_stateUp = ButtonState.Flat;
        Invalidate( m_rcUpArrow );
      }
      
      if( m_stateDown != ButtonState.Flat )
      {
        m_stateDown = ButtonState.Flat;
        Invalidate( m_rcDownArrow );
      }
    }
  }
}
