using System;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

using UtilityLibrary.General;
using UtilityLibrary.Win32;
using UtilityLibrary.WinControls;


namespace UtilityLibrary.Combos
{
  [ToolboxItem(false)]
  [Designer( "UtilityLibrary.Designers.ComboBoxBaseDesigner, UtilityLibrary.Designers, Version=1.0.4.0, Culture=neutral, PublicKeyToken=fe06e967d3cf723d" )]
  public abstract class CustomCombo :  System.Windows.Forms.Control
  {
    #region Class members
    protected Form      m_dropDownForm;
    protected bool      m_bFirstShow = true;
    protected bool      m_bControlBinded;
    protected Control   m_ctrlBinded;
    protected bool      m_bCanInvalidate = true;

    private bool        m_bHighlight;
    private bool        m_bOwnValuePaint;
    private TextBox     m_editbox;
    private Rectangle   m_clickButton;
    private bool        m_bDropDown;
    private int         m_iDropDownHeight = 120;
    private int         m_iDropDownWidth;

    private bool        m_bSkipClick;

    private IntPtr        m_mouseHookHandle = IntPtr.Zero;
    private GCHandle      m_mouseProcHandle;
    private Brush         m_brushSelected = ColorUtil.VSNetSelectionBrush;
    private StringFormat  m_strFormat;
    #endregion

    #region Class Propertis
    [ Category( "Appearance" ), 
    DefaultValue( "" ), 
    Description( "GET/SET Current value of combobox" ) ]
    public string Value
    {
      get
      {
        return m_editbox.Text;
      }
      set
      {
        if( value != m_editbox.Text )
        {
          if( OnValueValidate( value ) == true )
          {
            m_editbox.Text = value;
            OnValueChanged();
            
            if( m_bCanInvalidate == true )
              Invalidate();
          }
          else
          {
            throw new ApplicationException( "You try to assign incorect data to Value property." );
          }
        } 
      }
    }

    [ Browsable( false ), DefaultValue( false ) ]
    public bool   IsHightlited
    {
      get
      {
        return m_bHighlight;
      }
    }
    
    [ Browsable( false ), DefaultValue( false ) ]
    public bool   DroppedDown
    {
      get
      {
        return m_bDropDown;
      }
      set
      {
        if( value != m_bDropDown )
        {
          m_bDropDown = value;
          
          if( m_bDropDown == true )
          {
            ShowDropDownForm();
          }
          else
          {
            HideDropDownForm();
          }
        } 
      }
    }
    
    [ Category( "Appearance" ), 
    DefaultValue( 120 ), 
    Description( "GET/SET maximum height of dropdown part of combobox" ) ]
    public int    DropDownHeigth
    {
      get
      {
        return m_iDropDownHeight;
      }
      set
      {
        if( value != m_iDropDownHeight )
        {
          m_iDropDownHeight = value;
          OnDropDownSizeChanged();
        }
      }
    }
    
    [ Category( "Appearance" ), 
    DefaultValue( 0 ), 
    Description( "GET/SET minimum width of dropdown part of combo. Zero value - means AutoSize of dropdown" ) ]
    public int    DropDownWidth
    {
      get
      {
        return m_iDropDownWidth;
      }
      set
      {
        if ( value != m_iDropDownWidth )
        {
          m_iDropDownWidth = value;
          OnDropDownSizeChanged();
        }
      }
    }
    
    [ Category( "Appearance" ), 
    DefaultValue( false ), 
    Description( "GET/SET value of combobox can be edited by user or not" ) ]
    public bool   ReadOnly
    {
      get
      {
        return m_editbox.ReadOnly;
      }
      set
      {
        if( value != m_editbox.ReadOnly )
        {
          m_editbox.ReadOnly = value;
          OnReadOnlyStateChanged();

          if( m_bCanInvalidate == true )
            Invalidate();
        }
      }
    }
    #endregion

    #region Internal Classes
    public class EventArgsBindDropDownControl : EventArgs
    {
      #region Class members
      private CustomCombo m_parent;
      private Form    m_frm;
      private Control m_ctrl;
      #endregion

      #region Class Properties
      public Form DropDownForm
      {
        get{ return m_frm; }
      }

      public Control BindedControl
      {
        get
        {
          return m_ctrl;
        }
        set
        {
          m_ctrl = value;
        }
      }

      public CustomCombo  Combo
      {
        get{ return m_parent; }
      }
      #endregion

      #region Class constructor
      private EventArgsBindDropDownControl(){}

      public EventArgsBindDropDownControl( CustomCombo parent, Form form )
      {
        m_parent = parent;
        m_frm = form;
      }

      public EventArgsBindDropDownControl( CustomCombo parent, Form form, Control ctrl )
      {
        m_parent = parent;
        m_frm  = form;
        m_ctrl = ctrl;
      }
      #endregion
    }

    public class EventArgsCloseDropDown : EventArgs
    {
      #region Class members
      private bool m_bClose;
      private Keys m_KeyCode;
      #endregion

      #region Class Properties
      public bool Close
      {
        get
        {
          return m_bClose;
        }
        set
        {
          m_bClose = value;
        }
      }
      public Keys KeyCode
      {
        get
        {
          return m_KeyCode;
        }
      }
      #endregion

      #region Class constructor
      private EventArgsCloseDropDown()
      {
      
      }

      public EventArgsCloseDropDown( bool close )
      {
        m_bClose = close;
      }
      
      public EventArgsCloseDropDown( bool close, Keys keycode )
      {
        m_bClose = close;
        m_KeyCode = keycode;
      }
      #endregion
    }

    public class EventArgsEditCustomSize : EventArgs
    {
      #region Class members
      private int iWidth;
      private int iHeight;
      private int iXPos;
      private int iYPos;
      #endregion

      #region Class Properties
      public int xPos
      {
        get
        {
          return iXPos;
        }
        set
        {
          iXPos = value;
        }
      }

      public int yPos
      {
        get
        {
          return iYPos;
        }
        set
        {
          iYPos = value;
        }
      }

      public int Width
      {
        get
        {
          return iWidth;
        }
        set
        {
          iWidth = value;
        }
      }

      public int Height
      {
        get
        {
          return iHeight;
        }
        set
        {
          iHeight = value;
        }
      }
      #endregion

      #region Class Constructors
      private EventArgsEditCustomSize(){}
      public EventArgsEditCustomSize( int x, int y, int width, int height )
      {
        iXPos = x;
        iYPos = y;
        iWidth = width;
        iHeight = height;
      }
      
      public EventArgsEditCustomSize( Rectangle rc )
      {
        iXPos = rc.X;
        iYPos = rc.Y;
        iWidth = rc.Width;
        iHeight = rc.Height;
      }
      #endregion
    }


    public delegate void CloseDropDownHandler( object sender, EventArgsCloseDropDown e );
    public delegate void EditControlResizeHandler( object sender, EventArgsEditCustomSize e );
    #endregion

    #region Class Events
    [ Category( "Paint" ), 
    Description( "Raised by class when calculated size of rectangle where value must be displayed" ) ]
    protected event EditControlResizeHandler CustomEditSize;
    [ Category( "Action" ), 
    Description( "Raise by class before real close of dropdown form" ) ]
    public event CloseDropDownHandler     CloseDropDown;
    [ Category( "Action" ), 
    Description( "Raise By class after dropdown form show" ) ]
    public event EventHandler             DropDownShown;
    [ Category( "Action" ), 
    Description( "Raised after dropdown form hide" ) ]
    public event EventHandler             DropDownHided;
    [ Category( "Property Changed" ), 
    Description( "Value of combo changed" ) ]
    public event EventHandler             ValueChanged;
    #endregion

    #region Class Constructor
    public CustomCombo()
    {
      ControlStyles styleTrue = ControlStyles.AllPaintingInWmPaint |
        ControlStyles.DoubleBuffer |
        ControlStyles.FixedHeight |
        ControlStyles.ResizeRedraw |
        ControlStyles.UserPaint;

      SetStyle( styleTrue, true );
      SetStyle( ControlStyles.Selectable, false ); 

      m_editbox = new TextBox();
      m_editbox.BorderStyle = BorderStyle.None;
      m_editbox.MouseEnter += new EventHandler( OnEditMouseEnter );
      m_editbox.KeyDown += new KeyEventHandler( OnEditKeyDown );
      m_editbox.MouseWheel += new MouseEventHandler( OnEditMouseWheel );
      m_editbox.Leave += new System.EventHandler( OnEditLeave );
      m_editbox.Enter += new System.EventHandler( OnEditEnter );

      this.Height = 21;

      m_dropDownForm = new Form();
      m_dropDownForm.FormBorderStyle = FormBorderStyle.None;
      m_dropDownForm.StartPosition = FormStartPosition.Manual;
      m_dropDownForm.TopMost = true;
      m_dropDownForm.ShowInTaskbar = false;
      m_dropDownForm.BackColor = SystemColors.Highlight;
      m_dropDownForm.Deactivate += new EventHandler( OnDropDownLostFocus );

      m_strFormat = new StringFormat();
      m_strFormat.Alignment = StringAlignment.Near;
      m_strFormat.LineAlignment = StringAlignment.Center;
      m_strFormat.FormatFlags = StringFormatFlags.LineLimit;
    }
    
    protected override void OnHandleCreated(System.EventArgs e)
    {
      base.OnHandleCreated( e );

      m_editbox.Parent = this;
      int yPos = (ClientRectangle.Height - m_editbox.Height) / 2;
      m_editbox.Location = new Point( ClientRectangle.X + 2, ClientRectangle.Y + yPos );
      m_editbox.Size = new Size( ClientRectangle.Width - 20, ClientRectangle.Height - 4 );
    }
    #endregion

    #region Class Mouse Hook Methods
    private void    StartHook()
    {     
      // Mouse hook
      WindowsAPI.HookProc mouseHookProc = new WindowsAPI.HookProc(MouseHook);
      m_mouseProcHandle = GCHandle.Alloc(mouseHookProc);
      m_mouseHookHandle = WindowsAPI.SetWindowsHookEx((int)WindowsHookCodes.WH_MOUSE, 
        mouseHookProc, IntPtr.Zero, WindowsAPI.GetCurrentThreadId());
    }

    private void    EndHook()
    {
      // Unhook   
      WindowsAPI.UnhookWindowsHookEx( m_mouseHookHandle );
      m_mouseProcHandle.Free();
      m_mouseHookHandle = IntPtr.Zero;
    }

    private IntPtr  MouseHook(int code, IntPtr wparam, IntPtr lparam) 
    {
      MOUSEHOOKSTRUCT mh = (MOUSEHOOKSTRUCT )Marshal.PtrToStructure(lparam, typeof(MOUSEHOOKSTRUCT));
      
      // if user set focus on edit control embedded by us then do not open DropDown box
      if( mh.hwnd == Handle && 
        wparam == (IntPtr)Msg.WM_LBUTTONDOWN && 
        m_bDropDown == false && ReadOnly == false )
      {
        m_bSkipClick = true;
      }

      return WindowsAPI.CallNextHookEx( m_mouseHookHandle, code, wparam, lparam );
    }
    #endregion

    #region Class Overrides
    protected abstract void OnPrevScrollItems();
    protected abstract void OnNextScrollItems();
    protected abstract void OnDropDownControlBinding( EventArgsBindDropDownControl e );
    
    protected virtual  void OnValueChanged()
    {
      RaiseValueChanged();
    }

    protected virtual  void OnDropDownSizeChanged()
    {
      if( m_dropDownForm != null )
      {
        m_dropDownForm.Size = new Size( ( DropDownWidth == 0 || ClientRectangle.Width > DropDownWidth ) ? ClientRectangle.Width : DropDownWidth, m_iDropDownHeight );
      }
    }

    protected virtual  void OnDropDownFormLocation()
    {
      if( m_dropDownForm != null )
      {
        Rectangle CtrlPos = RectangleToScreen( ClientRectangle );

        if ( Screen.PrimaryScreen.WorkingArea.Bottom > CtrlPos.Bottom + 1 + m_dropDownForm.Height ) 
        {
          m_dropDownForm.Location = new Point( CtrlPos.X, CtrlPos.Bottom + 1 );
        }
        else
        {
          m_dropDownForm.Location = new Point( CtrlPos.X, CtrlPos.Y - 1 - m_dropDownForm.Height );
        }
      }
    }

    protected virtual  bool OnValueValidate( string value )
    {
      // TODO: implement values validation
      return true;
    }

    protected virtual  void OnReadOnlyStateChanged()
    {
      if( m_editbox.ReadOnly == true )
      {
        m_bOwnValuePaint = true;
        m_editbox.Visible = false;
        SetStyle( ControlStyles.Selectable, true ); 
      }
      else
      {
        m_bOwnValuePaint = false;
        m_editbox.Visible = true;
        SetStyle( ControlStyles.Selectable, false ); 
      }
    }
    protected virtual  void OnSetFindItem( char ch )
    {
      // do nothing. inheritors must implement this if they support 
      // item find on key press
    }
    #endregion
    
    #region Control Paint methods
    protected override void OnPaint( System.Windows.Forms.PaintEventArgs e )
    {
      OnPaintBackground( e );
      OnPaintComboButton( e );
      OnPaintEditItem( e );
      OnPaintCustomData( e );
    }

    protected virtual  void OnPaintComboButton( System.Windows.Forms.PaintEventArgs pevent )
    {
      Graphics g = pevent.Graphics;
      Rectangle rc = ClientRectangle;
      
      int width = 12;
      int left = rc.Right - width - 2;
      int top = rc.Top + 2;
      int height = rc.Height - 4;

      m_clickButton = new Rectangle( left, top, width, height );

      // Draw arrow glyph
      Point[] pts = new Point[3];
      pts[0] = new Point(left + width/2 - 2, top + height/2-1);
      pts[1] = new Point(left + width/2 + 3,  top + height/2-1);
      pts[2] = new Point(left + width/2, (top + height/2-1) + 3);
      
      if( this.Enabled == false ) 
      {
        g.FillPolygon( SystemBrushes.ControlDark, pts ); 
      }
      else 
      {
        g.FillPolygon( Brushes.Black, pts );
      }
    }

    protected override void OnPaintBackground( System.Windows.Forms.PaintEventArgs pevent )
    {
      Graphics g = pevent.Graphics;
      Rectangle rc = ClientRectangle;

      g.FillRectangle( ( Enabled == true ) ? SystemBrushes.Window : SystemBrushes.Control, rc );

      if( IsHightlited == true || DroppedDown == true || Enabled == false )
      {
        g.DrawRectangle( ( Enabled ) ? SystemPens.Highlight : SystemPens.GrayText, 
          rc.X, rc.Y, rc.Width-1, rc.Height-1 );
      }

      OnPaintComboButtonBackground( pevent );
    }

    protected virtual  void OnPaintComboButtonBackground(System.Windows.Forms.PaintEventArgs pevent)
    {
      Graphics g = pevent.Graphics;
      Rectangle rc = ClientRectangle;

      if( IsHightlited == true || DroppedDown == true )
      {
        g.FillRectangle( m_brushSelected, rc.Right - 15, rc.Y + 1, 16, rc.Height - 2 );
        g.DrawRectangle( SystemPens.Highlight, rc.Right - 16, rc.Y, 15, rc.Height - 1 );
      }
      else
      {
        g.FillRectangle( SystemBrushes.Control, rc.Right - 14, rc.Y + 2, 12, rc.Height - 4 );
      }
    }

    protected virtual  void OnPaintEditItem( System.Windows.Forms.PaintEventArgs pevent )
    {
      Graphics g = pevent.Graphics;
      //Rectangle rc = pevent.ClipRectangle;
      Rectangle rc = ClientRectangle;
      
      // if control not shown to user then change text centering
      int yPos = ClientRectangle.Y + ( ClientRectangle.Height - m_editbox.Height ) / 2;
      if( m_bOwnValuePaint == true ) yPos = ClientRectangle.Y + 2;
      
      int xPos = ClientRectangle.X + 2;
      int iWidth = ClientRectangle.Width - 20;
      int iHeight = ClientRectangle.Height - 4;
      
      EventArgsEditCustomSize ev = new EventArgsEditCustomSize( xPos, yPos, iWidth, iHeight );
      RaiseCustomEditSize( ev );

      m_editbox.Location = new Point( ev.xPos, ev.yPos );
      m_editbox.Size = new Size( ev.Width, ev.Height );

      if( m_bOwnValuePaint == true )
      {
        Rectangle rcOut = new Rectangle( ev.xPos, ev.yPos, ev.Width, ev.Height );
        SolidBrush brush = new SolidBrush( ( Enabled ) ? ForeColor : SystemColors.GrayText );
        g.DrawString( Value, Font, brush, rcOut, m_strFormat );
        brush.Dispose();
      }
    }

    protected virtual  void OnPaintCustomData( System.Windows.Forms.PaintEventArgs pevent )
    {
      // do nothing here by default
    }
    #endregion

    #region User input catchers methods
    protected override void OnGotFocus(System.EventArgs e)
    {
      m_bHighlight = true;

      if( m_bCanInvalidate == true )
        Invalidate();
    }

    protected override void OnLostFocus(System.EventArgs e)
    {
      m_bHighlight = false;
      
      if( m_bCanInvalidate == true )
        Invalidate();
    }
    protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
    {
      OnEditKeyDown( this, e );
    }
    protected override void OnMouseEnter(System.EventArgs e)
    {
      m_bHighlight = true;

      if( m_bCanInvalidate == true )
        Invalidate();
    }

    protected override void OnMouseLeave(System.EventArgs e)
    {
      m_bHighlight = false;

      if( m_bCanInvalidate == true )
        Invalidate();
    }

    protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
    {
      if( e.Delta < 0 )
      {
        OnPrevScrollItems();
      }
      else
      {
        OnNextScrollItems();
      }
    }
    
    protected override void OnClick( System.EventArgs e )
    {
      Focus();

      base.OnClick( e );

      if( m_bSkipClick == false )
      {
        Point pnt = MousePosition;
        pnt = PointToClient( pnt );

        if( ( m_clickButton.Contains( pnt ) == true ) || 
          ( m_editbox.ReadOnly == true && ClientRectangle.Contains( pnt ) )
          )
        {
          DroppedDown = true;
        }
      }

      m_bSkipClick = false;
    }
    
    
    private void OnEditMouseEnter( object sender, System.EventArgs e)
    {
      m_bHighlight = true;
      
      if( m_bCanInvalidate == true )
        Invalidate();
    }

    private void OnEditKeyDown( object sender, KeyEventArgs e )
    {
      if( e.Alt == true && e.KeyCode == Keys.Down && DroppedDown == false )
      {
        DroppedDown = true;
      }
      else if( (e.Modifiers & (Keys.Control | Keys.Shift | Keys.Alt)) == 0 )
      {
        if( e.KeyCode == Keys.F4 )
        {
          DroppedDown = !DroppedDown;
        }
        else if( e.KeyCode == Keys.Down )
        {
          OnNextScrollItems();
          e.Handled = true;
          m_editbox.SelectAll();
        }
        else if( e.KeyCode == Keys.Up )
        {
          OnPrevScrollItems();
          e.Handled = true;
          m_editbox.SelectAll();
        } 
        else
        {
          
          if( IsKeyAChar( e.KeyCode ) == true )
          {
            OnSetFindItem( GetKeyChar( e.KeyCode ) );
          }
        }
      }
    }
    private void OnEditMouseWheel( object sender, MouseEventArgs e )
    {
      if( e.Delta < 0 )
      {
        OnPrevScrollItems();
      }
      else
      {
        OnNextScrollItems();
      }
    }
    private void OnEditEnter( object sender, System.EventArgs e )
    {
      m_bHighlight = true;
      
      if( m_bCanInvalidate == true )
        Invalidate();
    }

    private void OnEditLeave( object sender, System.EventArgs e )
    {
      m_bHighlight = false;

      if( m_bCanInvalidate == true )
        Invalidate();
    }


    protected bool IsKeyAChar( Keys key )
    {
      string keys = key.ToString();

      if( keys.Length == 1 ) return true;
      if( keys.Length == 2 && keys[0] == 'D' ) return true;
      if( keys.Length == "NumPad1".Length && keys.IndexOf( "NumPad" ) == 0 ) return true;
      if( keys.Length == "Space".Length && keys.IndexOf( "Space" ) == 0 ) return true;

      return false;
    }

    protected char GetKeyChar( Keys key )
    {
      string keys = key.ToString();

      if( keys.Length == 1 ) return keys[0];
      if( keys.Length == 2 && keys[0] == 'D' ) return keys[1];
      if( keys.Length == "NumPad1".Length && keys.IndexOf( "NumPad" ) == 0 ) return keys[6];
      if( keys.Length == "Space".Length && keys.IndexOf( "Space" ) == 0 ) return ' ';

      return ' ';
    }
    #endregion
    
    #region Show/Hide dropdown window methods
    public void BindDropDownControl()
    {
      if( m_bControlBinded == false )
      {
        EventArgsBindDropDownControl ev = new EventArgsBindDropDownControl( this, m_dropDownForm );
        OnDropDownControlBinding( ev );
        m_ctrlBinded = ev.BindedControl;
        m_bControlBinded = true;
      }
    }
    protected virtual void ShowDropDownForm()
    {
      // create form on first click
      if( m_bFirstShow == true )
      {
        // first calculate size of form
        OnDropDownSizeChanged();

        BindDropDownControl();

        m_ctrlBinded.Size = new Size( m_dropDownForm.Width-2, m_dropDownForm.Height-2 );
        m_ctrlBinded.Location = new Point( 1, 1 );

        m_ctrlBinded.Anchor = AnchorStyles.Bottom | 
          AnchorStyles.Left | 
          AnchorStyles.Right | 
          AnchorStyles.Top;
        
        m_ctrlBinded.Parent = m_dropDownForm;
        m_ctrlBinded.KeyDown += new KeyEventHandler( OnDropDownControlKeyDown );
        m_ctrlBinded.DoubleClick += new EventHandler( OnDropDownControlDoubleClick );
        m_bFirstShow = false;
      }
      else
      {
        OnDropDownSizeChanged();
      }

      OnDropDownFormLocation();
      
      m_dropDownForm.Show();
      
      // if control has smaller size and cannot be resized then resize form
      if( (m_ctrlBinded.Height + 2) < m_dropDownForm.Height )
        m_dropDownForm.Height = m_ctrlBinded.Height + 2;

      m_ctrlBinded.Focus();
      StartHook();
      
      RaiseDropDownShown();
    }

    protected virtual void HideDropDownForm()
    {
      if( m_dropDownForm != null )
      {
        m_dropDownForm.Hide();
        EndHook();

        RaiseDropDownHided();

        if( m_bCanInvalidate == true )
          Invalidate();
      }
    }
    #endregion

    #region Class Helper methods
    protected void RaiseCloseDropDown( EventArgsCloseDropDown e )
    {
      if( CloseDropDown != null )
      {
        CloseDropDown( this, e ); 
      }
    }

    protected void RaiseCustomEditSize( EventArgsEditCustomSize e )
    {
      if( CustomEditSize != null )
      {
        CustomEditSize( this, e );
      }
    }
    protected void RaiseDropDownShown()
    {
      if( DropDownShown != null )
      {
        DropDownShown( this, EventArgs.Empty );
      }
    }

    protected void RaiseDropDownHided()
    {
      if( DropDownHided != null )
      {
        DropDownHided( this, EventArgs.Empty );
      }
    }
    protected void RaiseValueChanged()
    {
      if( ValueChanged != null )
      {
        ValueChanged( this, EventArgs.Empty );
      }
    }
    #endregion

    #region Drop Down Event handlers
    private void OnDropDownLostFocus( object sender, EventArgs e )
    {
      DroppedDown = false;
    }

    private void OnDropDownControlKeyDown( object sender, KeyEventArgs e )
    {
      if( e.Alt == true && ( e.KeyCode == Keys.Down || e.KeyCode == Keys.Up ) )
      {
        DroppedDown = false;
      }
      else if( ( e.Modifiers & (Keys.Shift | Keys.Alt | Keys.Control) ) == 0 )
      {
        EventArgsCloseDropDown ev = new EventArgsCloseDropDown( true, e.KeyCode );

        if( e.KeyCode == Keys.Escape )
        {
          DroppedDown = false;
        } 
        else if( e.KeyCode == Keys.F4 )
        {
          RaiseCloseDropDown( ev );
          DroppedDown = !ev.Close;
        }
        else if( e.KeyCode == Keys.Enter )
        {
          RaiseCloseDropDown( ev );
          DroppedDown = !ev.Close;
        }
      }
    }

    private void OnDropDownControlDoubleClick( object sender, EventArgs e )
    {
      if( ( ModifierKeys & (Keys.Shift | Keys.Alt | Keys.Control) ) == 0 )
      {
        EventArgsCloseDropDown ev = new EventArgsCloseDropDown( true );
        RaiseCloseDropDown( ev );
        DroppedDown = !ev.Close;
      }
    }
    #endregion

    #region Class Public methods
    public void BeginUpdate()
    {
      m_bCanInvalidate = false;
    }

    public void EndUpdate()
    {
      m_bCanInvalidate = true;
    }
    #endregion

    #region Class WNDProc - enable arrow keys catching
    protected override void WndProc( ref System.Windows.Forms.Message m )
    {
      base.WndProc( ref m );

      if( m.Msg == (int)Msg.WM_GETDLGCODE )
      {
        m.Result = new IntPtr( (int)DialogCodes.DLGC_WANTCHARS | 
          (int)DialogCodes.DLGC_WANTARROWS | 
          m.Result.ToInt32() );
      }
    }
    #endregion
  }
}
