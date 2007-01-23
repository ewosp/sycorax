using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security;
using System.Runtime.InteropServices;

using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;

using UtilityLibrary.General;
using UtilityLibrary.Win32;


namespace UtilityLibrary.WinControls
{
  /// <summary>
  /// Summary description for ComboBoxBase.
  ///   /// </summary>
  [ToolboxItem(false)]
  [Designer( "UtilityLibrary.Designers.ComboBoxBaseDesigner, UtilityLibrary.Designers, Version=1.0.4.0, Culture=neutral, PublicKeyToken=fe06e967d3cf723d" )]
  public class ComboBoxBase : System.Windows.Forms.ComboBox
  {
    #region Class constants
    public const int ARROW_WIDTH = 12;
    #endregion

    #region Class Variables
    private   IntPtr        mouseHookHandle = IntPtr.Zero;
    private   GCHandle      mouseProcHandle;
    private   bool          toolBarUse = false;
    private   bool          hooked = false;
    internal  bool          highlighted = false;
    private   bool          fireOnSelectedIndexChanged = true;
    private   bool          m_bDrawTempText;
    private   string        m_TempText;
    #endregion
    
    #region Constructors
    // I wanted to make this constructor either protected
    // or internal so that the ComboBoxBase could not be used
    // as a stand alone class, however the IDE designer complains about it
    // if the constructors are not public
    public ComboBoxBase( bool toolBarUse )
    {
      // Flag to indicate that combo box will be used
      // in a toolbar --which means we will use some window hooks
      // to reset the focus of the combobox--
      this.toolBarUse = toolBarUse;
      DrawMode = DrawMode.OwnerDrawFixed;
      
      SetStyle(
        ControlStyles.AllPaintingInWmPaint | 
        ControlStyles.UserPaint | 
        ControlStyles.DoubleBuffer, true );

      // Use Menu font so that the combobox uses the same font as the toolbar buttons
      Font = SystemInformation.MenuFont;
      
      // When use in a toolbar we don't need tab stop
      TabStop = false;
    }

    // I wanted to make this constructor either protected
    // or internal so that the ComboBoxBase could not be used
    // as a stand alone class, however the IDE designer complains about it
    // if the constructors are not public
    public ComboBoxBase()
    {
      DrawMode = DrawMode.OwnerDrawFixed;
      
      SetStyle(
        ControlStyles.AllPaintingInWmPaint | 
        ControlStyles.UserPaint |
//        ControlStyles.Opaque |
        ControlStyles.DoubleBuffer, true );
    }
    #endregion

    #region Class Paint methods
    protected override void OnPaint( PaintEventArgs pe )
    {
      Graphics g = pe.Graphics;
      Rectangle rc = pe.ClipRectangle;

      if( ContainsFocus ) highlighted = true;
      Color clr = ( Enabled == false ) ? SystemColors.Control : SystemColors.Window;

      PaintComboBoxBackground( g, clr );
      
      if( SelectedIndex == -1 && DropDownStyle != ComboBoxStyle.DropDown ) 
      {
        if( Items.Count > 0 )
        {
          // Select first item as the current item
          fireOnSelectedIndexChanged = false;
          SelectedIndex = 0;
        }
      }

      DrawComboBoxItemEx( g, rc, ( m_bDrawTempText ) ? -1 : SelectedIndex, false, true );
      
      if( highlighted )
      {
        DrawComboBoxArrowSelected( g, false );
      }
      else
      {
        DrawComboBoxArrowNormal( g, true );
      }
    }

    protected override void OnDrawItem( DrawItemEventArgs e )
    {
      // Draw bitmap strech to the size of the size of the combobox
      Graphics g = e.Graphics;
      Rectangle bounds = e.Bounds;
      
      bool selected = ( e.State & DrawItemState.Selected ) == DrawItemState.Selected;
      bool editSel  = ( e.State & DrawItemState.ComboBoxEdit ) == DrawItemState.ComboBoxEdit;
      
      if( e.Index != -1 )
      {
        DrawComboBoxItem( g, bounds, e.Index, selected, editSel );
      }
    }

    protected void PaintComboBoxBackground( Graphics g, Color backColor )
    {
      Rectangle rc = ClientRectangle;
      g.FillRectangle( new SolidBrush( backColor ), rc );

      if( Enabled == false )
      {
        g.DrawRectangle( SystemPens.GrayText, rc.X, rc.Y, rc.Width-1, rc.Height-1 );
      }
      else if( highlighted )
      {
        Pen pen = ColorUtil.VSNetBorderPen;
        g.DrawRectangle( pen, rc.X, rc.Y, rc.Width-1, rc.Height-1 );
      }
    }

    internal  void DrawComboBoxArrowNormal( Graphics g, bool disable )
    {
      int left, top, arrowWidth, height;
      CalculateArrowBoxCoordinates(out left, out top, out arrowWidth, out height);

      Brush stripeColorBrush, brush;
      int width = SystemInformation.VerticalScrollBarWidth - ARROW_WIDTH + 1;

      if( Enabled ) 
      {
        stripeColorBrush = ColorUtil.VSNetControlBrush;
        brush = SystemBrushes.Window;
      }
      else
      {
        stripeColorBrush = brush = SystemBrushes.Control;
      }
      
      g.FillRectangle( brush, new Rectangle( left - width, top, 
        SystemInformation.VerticalScrollBarWidth, height ) );

      g.FillRectangle( stripeColorBrush, left, top, arrowWidth, height );
      
      DrawArrowGlyph( g, !Enabled );
    }

    internal  void DrawComboBoxArrowSelected( Graphics g, bool erase )
    {
      int left, top, arrowWidth, height;
      CalculateArrowBoxCoordinates( out left, out top, out arrowWidth, out height );

      if( Enabled )
      {
        int width = SystemInformation.VerticalScrollBarWidth - ARROW_WIDTH + 1;
        
        g.FillRectangle( SystemBrushes.Window, new Rectangle( left-width, top, 
          SystemInformation.VerticalScrollBarWidth, height ) );
      }
                  
      if( !erase )
      {
        if( DroppedDown ) 
        {
          Graphics cbg = CreateGraphics();
          cbg.FillRectangle( ColorUtil.VSNetPressedBrush, 
            left-1, top-1, arrowWidth+2, height+2 );
          
          cbg.DrawRectangle( ColorUtil.VSNetBorderPen, 
            left-1, top-2, arrowWidth+2, height+3 );
          
          DrawArrowGlyph( cbg, false );
          cbg.Dispose();
          return;
        }
        else
        {
          g.FillRectangle( ColorUtil.VSNetSelectionBrush, 
            left-1, top-1, arrowWidth+2, height+2 );

          g.DrawRectangle( ColorUtil.VSNetBorderPen, 
            left-1, top-2, arrowWidth+2, height+3);
        }
      }
      else 
      {
        g.FillRectangle( Brushes.White, left-1, top-1, arrowWidth+2, height+2 );
      }

      DrawArrowGlyph( g, false );
    }

    protected void DrawArrowGlyph( Graphics g, bool disable )
    {
      int left, top, arrowWidth, height;
      CalculateArrowBoxCoordinates(out left, out top, out arrowWidth, out height);

      // Draw arrow glyph
      Point[] pts = new Point[3];
      pts[0] = new Point(left + arrowWidth/2 - 2, top + height/2-1);
      pts[1] = new Point(left + arrowWidth/2 + 3,  top + height/2-1);
      pts[2] = new Point(left + arrowWidth/2, (top + height/2-1) + 3);
      
      g.FillPolygon( ( Enabled == false ) ? SystemBrushes.ControlDark : Brushes.Black, pts ); 
    }

    #endregion

    #region Class Overrides
    protected override void OnEnabledChanged(System.EventArgs e)
    {
      base.OnEnabledChanged( e );
      Invalidate();
    }

    protected override void WndProc( ref Message m )
    {
      base.WndProc(ref m);
      
      switch( ( int )m.Msg )
      {
        case ( int )Msg.WM_PAINT:
          OnPaint( new PaintEventArgs( CreateGraphics(), ClientRectangle ) );
          m.Result = IntPtr.Zero;
          return;
      }
    }

    protected override void OnMouseEnter( EventArgs e )
    {
      base.OnMouseEnter(e);

      highlighted = true;
      
      if( Enabled ) Invalidate();
    }

    protected override void OnMouseLeave( EventArgs e )
    {
      base.OnMouseLeave(e);
      
      if( !ContainsFocus )
      {
        highlighted = false;
        
        if( Enabled ) Invalidate();
      }
    }
  
    protected override void OnLostFocus( EventArgs e )
    {
      base.OnLostFocus(e);
      
      // if mouse not under us then remove highlighting
      if( ClientRectangle.Contains( PointToClient( Control.MousePosition ) ) == false )
      {
        highlighted = false;
        Invalidate();
      }

      if( toolBarUse && hooked )
      {
        hooked = false;
        EndHook();
      }
    }

    protected override void OnGotFocus( EventArgs e )
    {
      base.OnGotFocus(e);
      
      highlighted = true;
      Invalidate();

      // Mouse hook is needed for correct catch of mouse enter/leave messeges
      // when control used in toolbar...
      if( toolBarUse && !hooked )
      {
        hooked = true;
        StartHook();
      }
    }

    protected override void OnSelectedIndexChanged( EventArgs e )
    {
      if( fireOnSelectedIndexChanged )
      {
        base.OnSelectedIndexChanged(e);
      }
      
      m_bDrawTempText = ( SelectedIndex == -1 );
      fireOnSelectedIndexChanged = true;
    }

    protected override void OnTextChanged(System.EventArgs e)
    {
      //      if ( m_bDrawTempText ) return;

      base.OnTextChanged( e );

      if ( !Items.Contains( Text ) )
      {
        m_bDrawTempText = true;
        m_TempText = Text;
      }
    }

    #endregion

    #region Class Properties
    public bool ToolBarUse
    {
      get 
      { 
        return toolBarUse; 
      }
      set 
      { 
        if( value != toolBarUse )
        {
          toolBarUse = value;
          
          if( toolBarUse == true )
          {
            BringToFront();
          }
        }
      }
    }
    #endregion

    #region Helper Methods
    public void SetFontHeight( int newHeight )
    {
      FontHeight = newHeight;
    }
    
    protected void CalculateArrowBoxCoordinates( out int left, out int top, out int width, out int height )
    {
      Rectangle rc = ClientRectangle;
      width = ARROW_WIDTH;
      left = rc.Right - width - 2;
      top = rc.Top + 2;
      height = rc.Height - 4;
    }
    #endregion
    
    #region Inheritors overrides
    protected virtual void DrawDisableState(){}
    protected virtual void DrawComboBoxItem( Graphics g, Rectangle bounds, int Index, bool selected, bool editSel )
    {
      // Draw the the combo item
      Brush b = ( Enabled == true ) ? SystemBrushes.Window : SystemBrushes.Control;
      g.FillRectangle( b, bounds.Left, bounds.Top, bounds.Width, bounds.Height);
            
      if( selected && !editSel )
      {
        // Draw highlight rectangle
        Pen p1 = ColorUtil.VSNetBorderPen;
        Brush b1 = ColorUtil.VSNetSelectionBrush;
        g.FillRectangle(b1, bounds.Left, bounds.Top, bounds.Width, bounds.Height);
        g.DrawRectangle(p1, bounds.Left, bounds.Top, bounds.Width-1, bounds.Height-1);
      }
      else
      {
        // Erase highlight rectangle
        b = ( Enabled == true ) ? SystemBrushes.Window : SystemBrushes.Control;
        g.FillRectangle( b, bounds.Left, bounds.Top, bounds.Width, bounds.Height);
        
        if ( editSel && ContainsFocus ) 
        {
          DrawComboBoxArrowSelected(g, false);
        }
      }
    }

    protected virtual void DrawComboBoxItemEx( Graphics g, Rectangle bounds, int Index, bool selected, bool editSel )
    {
      // This function is only called form the OnPaint handler and the Graphics object passed is the one
      // for the combobox itself as opossed to the one for the edit control in the combobox
      // doing this allows us to be able to avoid clipping problems with text strings
      
      // Draw the the combo item
      bounds.Inflate(-3, -3);
      
      Brush b = ( Enabled == true ) ? SystemBrushes.Window : SystemBrushes.Control;
      g.FillRectangle( b, bounds.Left, bounds.Top, bounds.Width, bounds.Height);
            
      if( selected && !editSel )
      {
        // Draw highlight rectangle
        Pen p1 = ColorUtil.VSNetBorderPen;
        Brush b1 = ColorUtil.VSNetSelectionBrush;
        
        g.FillRectangle(b1, bounds.Left, bounds.Top, bounds.Width, bounds.Height);
        g.DrawRectangle(p1, bounds.Left, bounds.Top, bounds.Width-1, bounds.Height-1);
      }
      else
      {
        b = ( Enabled == true ) ? SystemBrushes.Window : SystemBrushes.Control;
        
        // Erase highlight rectangle
        g.FillRectangle( b, bounds.Left, bounds.Top, bounds.Width, bounds.Height );
        
        if( editSel && ContainsFocus ) 
        {
          // Draw higlighted arrow
          DrawComboBoxArrowSelected( g, false );
        }
      }
    }
    
    #endregion

    #region Class mouse hook
    private void StartHook()
    {     
      // Mouse hook
      WindowsAPI.HookProc mouseHookProc = new WindowsAPI.HookProc(MouseHook);
      mouseProcHandle = GCHandle.Alloc(mouseHookProc);
      mouseHookHandle = WindowsAPI.SetWindowsHookEx((int)WindowsHookCodes.WH_MOUSE, 
        mouseHookProc, IntPtr.Zero, WindowsAPI.GetCurrentThreadId());
      
      if( mouseHookHandle == IntPtr.Zero ) 
      {
        throw new SecurityException();
      }
    }

    private void EndHook()
    {
      // Unhook   
      WindowsAPI.UnhookWindowsHookEx(mouseHookHandle);
      mouseProcHandle.Free();
      mouseHookHandle = IntPtr.Zero;
    }

    private IntPtr MouseHook( int code, IntPtr wparam, IntPtr lparam ) 
    {
      MOUSEHOOKSTRUCT mh = (MOUSEHOOKSTRUCT )Marshal.PtrToStructure(lparam, typeof(MOUSEHOOKSTRUCT));
      
      Msg msg = (Msg)wparam.ToInt32();

      if( mh.hwnd != Handle && 
        !DroppedDown && 
        ( msg == Msg.WM_LBUTTONDOWN || msg == Msg.WM_RBUTTONDOWN ) || 
        msg == Msg.WM_NCLBUTTONDOWN )
      {
        // Loose focus
        WindowsAPI.SetFocus( IntPtr.Zero );
      }
      else if ( mh.hwnd != Handle && 
        !DroppedDown && 
        ( msg == Msg.WM_LBUTTONUP || msg == Msg.WM_RBUTTONUP ) || 
        msg == Msg.WM_NCLBUTTONUP )
      {
        WindowsAPI.SetFocus(IntPtr.Zero);
      }
      
      return WindowsAPI.CallNextHookEx( mouseHookHandle, code, wparam, lparam );
    }
    #endregion

  }
}
