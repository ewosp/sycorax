using System;
using System.Windows.Forms.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;

using UtilityLibrary.Wizards;

namespace UtilityLibrary.Designers
{
  /// <summary>
  /// Designer of Wizard Page
  /// </summary>
  public class WizardPageDesigner : ParentControlDesigner
  {
    #region Class Static variables
    /// to boost performance of designer we initialize Variables only 
    /// once and make them static
    private static SolidBrush m_brushBack = new SolidBrush( Color.Navy );
    private static SolidBrush m_brushText = new SolidBrush( Color.White );
    private static Font m_font = new Font( FontFamily.GenericSansSerif, 10 );
    private static RectangleF m_rect = new RectangleF( 0, 0, 32 ,16 );
    #endregion

    #region Class members
    private bool m_bShowLines = true;
    #endregion

    #region Class Properties
    public bool ShowPageLimits
    {
      get
      {
        return m_bShowLines;
      }
      set
      {
        if( value != m_bShowLines )
        {
          m_bShowLines = value;
          
          if( base.Control != null )
          {
            base.Control.Invalidate();
          }
        }
      }
    }
    #endregion

    #region Class constructors
    public WizardPageDesigner()
    {
      //base.DrawGrid = false;
    }
    #endregion

    #region Class Overrides
    /// <summary>
    /// Remove Some properties from designer list
    /// </summary>
    /// <param name="properties">Dicstionary of properties</param>
    protected override void PreFilterProperties( IDictionary properties )
    {
      base.PreFilterProperties(properties);
      base.DrawGrid = false;

      properties.Remove("Dock");
      properties.Remove("Anchor");
      properties.Remove("AutoScroll");
      properties.Remove("AutoScrollMargin");
      properties.Remove("AutoScrollMinSize");
      properties.Remove("DockPadding");
      properties.Remove("Location");
      //properties.Remove( "DrawGrid" );
    }

    /// <summary>
    /// Custom Drawing of Wizard Page in IDE Designer
    /// </summary>
    /// <param name="pe"></param>
    protected override void OnPaintAdornments( System.Windows.Forms.PaintEventArgs pe )
    {
      base.OnPaintAdornments( pe );
      
      Graphics g = pe.Graphics;
      Rectangle rect;// = pe.ClipRectangle;

      WizardPageBase page = Control as WizardPageBase;
      
      if( page == null )
      {
        page = Component as WizardPageBase;
      }
      
      WizardForm parent = ( page != null ) ? page.WizardPageParent : null;
      
      string strPos;
      
      if( page != null && parent != null )
      {
        strPos = ( parent.Pages.IndexOf( page ) + 1 ).ToString();
      }
      else // if something wrong then simply show MINUS
      {
        strPos = "-";
        if( page == null ) strPos += "p";
        if( parent == null ) strPos += "P";
      }

      g.FillRectangle( m_brushBack, m_rect ); 

      StringFormat frmString = new StringFormat();
      frmString.Alignment = StringAlignment.Center;
      frmString.LineAlignment = StringAlignment.Center;

      g.DrawString( strPos, m_font, m_brushText, m_rect, frmString );

      if( parent != null )
      {
        rect = parent.GetCurrentPageRect();
        if( rect.Width % 8 > 0 )
          rect.Width = (int)( rect.Width / 8 ) * 8;

        if( rect.Height % 8 > 0 )
          rect.Height = (int)( rect.Height / 8 ) * 8;

        if( page != null && page.WelcomePage == false && m_bShowLines == true )
        {  
          Point[] pnt = new Point[]
          {
            new Point( new Size( rect.X + 24, rect.Y + 8 ) ),
            new Point( new Size( rect.X + 24, rect.Bottom - 8 ) ),
            new Point( new Size( rect.X + 40, rect.Bottom - 8 ) ),
            new Point( new Size( rect.X + 40, rect.Y + 8 ) ),
            new Point( new Size( rect.X + 56, rect.Y + 8 ) ),
            new Point( new Size( rect.X + 56, rect.Bottom - 8 ) ),
            
            new Point( new Size( rect.Right - 56, rect.Bottom - 8 ) ),
            new Point( new Size( rect.Right - 56, rect.Y + 8 ) ),
            new Point( new Size( rect.Right - 40, rect.Y + 8 ) ),
            new Point( new Size( rect.Right - 40, rect.Bottom - 8 ) ),
            new Point( new Size( rect.Right - 24, rect.Bottom - 8 ) ),
            new Point( new Size( rect.Right - 24, rect.Y + 8 ) ),
            
            new Point( new Size( rect.X + 24, rect.Y + 8 ) ),
            new Point( new Size( rect.X + 24, rect.Bottom - 8 ) ),
            new Point( new Size( rect.Right - 24, rect.Bottom - 8 ) )
          };

          Pen pen = new Pen( Brushes.Black );
          pen.DashStyle = DashStyle.DashDotDot;
          g.DrawLines( pen, pnt );
          pen.Dispose();
        }
      }
    }
    #endregion
  }
}
