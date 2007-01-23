#region Using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using UtilityLibrary.General;
#endregion

namespace UtilityLibrary.WinControls
{
  /// <summary>
  /// Summary description for VSNetButton.
  /// </summary>
  public class ButtonEx : System.Windows.Forms.Button
  {
    #region Class members
    private bool mouseDown = false;
    private bool mouseEnter = false;

    private StringFormat m_format = new StringFormat();
    #endregion
    
    #region Class Initialize/Finalize methods
    public ButtonEx()
    {
      SetStyle( ControlStyles.AllPaintingInWmPaint |
        ControlStyles.Opaque |
        ControlStyles.UserPaint |
        ControlStyles.DoubleBuffer, true );

      m_format.Trimming     = StringTrimming.None;
      m_format.FormatFlags  = StringFormatFlags.LineLimit;
      m_format.HotkeyPrefix = HotkeyPrefix.Show;

      base.ImageAlign = ContentAlignment.MiddleLeft;
    }
    #endregion

    #region Class Overrides
    protected override void OnMouseEnter(EventArgs e)
    {
      mouseEnter = true;
      base.OnMouseEnter(e);
      Invalidate();
    
    }

    protected override void OnMouseLeave(EventArgs e)
    {
      mouseEnter = false;
      base.OnMouseLeave(e);
      Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      mouseDown = true;
      Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
      mouseDown = false;
      Invalidate();
    }

    protected override void OnGotFocus(EventArgs e)
    {
      base.OnGotFocus(e);
      Invalidate();
    }
        
    protected override void OnLostFocus(EventArgs e)
    {
      base.OnLostFocus(e);
      Invalidate();
    }
    #endregion
    
    #region Class Drawing methods
    private StringAlignment GetHorizontalAlign( ContentAlignment align )
    {
      StringAlignment retValue = StringAlignment.Near;

      switch( align )
      {
        case ContentAlignment.BottomRight:
        case ContentAlignment.MiddleRight:
        case ContentAlignment.TopRight:
          retValue = StringAlignment.Far;
          break;

        case ContentAlignment.BottomCenter:
        case ContentAlignment.MiddleCenter:
        case ContentAlignment.TopCenter:
          retValue = StringAlignment.Center;
          break;

        case ContentAlignment.BottomLeft:
        case ContentAlignment.MiddleLeft:
        case ContentAlignment.TopLeft:
        default:
          retValue = StringAlignment.Near;
          break;
      }

      return retValue;
    }

    private StringAlignment GetVerticalAlign( ContentAlignment align )
    {
      StringAlignment retValue = StringAlignment.Near;

      switch( align )
      {
        case ContentAlignment.BottomRight:
        case ContentAlignment.BottomCenter:
        case ContentAlignment.BottomLeft:
          retValue = StringAlignment.Far;
          break;

        case ContentAlignment.MiddleRight:
        case ContentAlignment.MiddleCenter:
        case ContentAlignment.MiddleLeft:
          retValue = StringAlignment.Center;
          break;

        case ContentAlignment.TopRight:
        case ContentAlignment.TopLeft:
        case ContentAlignment.TopCenter:
        default:
          retValue = StringAlignment.Near;
          break;
      }

      return retValue;
    }


    protected override void OnPaint( PaintEventArgs pe )
    {
      //base.OnPaint(pe);
      Graphics g = pe.Graphics;

      bool gotFocus = Focused;

      if( mouseDown && Enabled )
      {
        DrawButtonState( g, DrawState.Pressed );
        return;
      }
      else if( ( gotFocus || mouseEnter ) && Enabled ) 
      {
        DrawButtonState( g, DrawState.Hot );
        return;
      }
      else
      {
        DrawButtonState( g, ( Enabled ) ? DrawState.Normal : DrawState.Disable );
      }
    }

    protected void DrawButtonState( Graphics g, DrawState state )
    {
      DrawBackground( g, state );
      Rectangle rc = ClientRectangle;
      Rectangle rcText = Rectangle.Inflate( ClientRectangle, -1, -1 );

      // draw Image
      if( Image != null )
      {
        SizeF sizeF   = Image.PhysicalDimension;
        int imgWidth  = (int)sizeF.Width;
        int imgHeight = (int)sizeF.Height;
        
        StringAlignment imgVAlign = GetVerticalAlign( ImageAlign );
        StringAlignment imgHAlign = GetHorizontalAlign( ImageAlign );
        
        int minWid = Width - 2, minHg = Height - 2;
        
        minWid = Math.Min( minWid, imgWidth );
        minHg = Math.Min( minHg, imgHeight );

        int xPos = Math.Min( minHg, minWid ) + 2;

        Bitmap bmp = new Bitmap( Image, new Size( minWid, minHg ) );

        int y = 1; 

        if( imgVAlign == StringAlignment.Center )
        {
          y = ( Height - bmp.Height ) / 2;
        }
        else if( imgVAlign == StringAlignment.Far )
        {
          y = ( Height - bmp.Height - 1 );
        }

        switch( imgHAlign )
        {
          case StringAlignment.Near:
            rcText.X = xPos;  rcText.Width -= xPos;
            DrawImage( g, state, bmp, 2+(xPos-minWid)/2, y );
            break;

          case StringAlignment.Far:
            rcText.Width = rcText.Right - xPos;
            DrawImage( g, state, bmp, rcText.Right + (xPos-minWid)/2 - 2, y );
            break;
        }

        bmp.Dispose();
      }

      // draw Text
      DrawText( g, Text, state, rcText );
    }

    protected void DrawBackground( Graphics g, DrawState state )
    {
      Rectangle rc = ClientRectangle;
      
      // Draw background
      if( state == DrawState.Normal || state == DrawState.Disable )
      {
        g.FillRectangle( SystemBrushes.Control, rc );
        
        Pen p = ( state == DrawState.Disable ) ? SystemPens.ControlDark : SystemPens.ControlDarkDark;
        
        // Draw border rectangle
        g.DrawRectangle( p, rc.Left, rc.Top, rc.Width-1, rc.Height-1);

      }
      else if( state == DrawState.Hot || state == DrawState.Pressed  )
      {
        // Erase whaterver that was there before
        if ( state == DrawState.Hot )
          g.FillRectangle( ColorUtil.VSNetSelectionBrush, rc );
        else
          g.FillRectangle( ColorUtil.VSNetPressedBrush, rc );
        
        // Draw border rectangle
        g.DrawRectangle( SystemPens.Highlight, rc.Left, rc.Top, rc.Width-1, rc.Height-1 );
      }
    }

    protected void DrawImage( Graphics g, DrawState state, Image image, int x, int y )
    {
      SizeF sizeF = Image.PhysicalDimension;
      int imageWidth = (int)sizeF.Width;
      int imageHeight = (int)sizeF.Height;
      
      if( state == DrawState.Normal )
      {
        g.DrawImage(Image, x, y, imageWidth, imageHeight);
      }
      else if( state == DrawState.Disable )
      {
        ControlPaint.DrawImageDisabled(g, Image, x, y, SystemColors.Control);
      }
      else if( state == DrawState.Pressed || state == DrawState.Hot )
      {
        ControlPaint.DrawImageDisabled(g, Image, x+1, y, SystemColors.Control);
        g.DrawImage(Image, x, y-1, imageWidth, imageHeight);                 
      }
    }

    protected void DrawText( Graphics g, string Text, DrawState state, Rectangle rc )
    {
      m_format.Alignment = GetHorizontalAlign( TextAlign );
      m_format.LineAlignment = GetVerticalAlign( TextAlign );

      g.DrawString( Text, Font, 
        ( state == DrawState.Disable ) ? SystemBrushes.ControlDark : SystemBrushes.ControlText, 
        rc, m_format );
    }
    #endregion

    private void ResetFlags()
    {
      mouseDown = false;
      mouseEnter = false;
    }

    protected override void OnVisibleChanged(System.EventArgs e)
    {
      base.OnVisibleChanged( e );
      if( !Visible ) ResetFlags();
    }

    protected override void OnEnabledChanged(System.EventArgs e)
    {
      base.OnEnabledChanged( e );
      if( !Enabled ) ResetFlags();
    }
  }

}
