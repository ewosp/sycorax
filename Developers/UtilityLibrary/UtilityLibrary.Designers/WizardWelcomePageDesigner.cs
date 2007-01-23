using System;
using System.Windows.Forms.Design;
using System.Drawing;
using System.Collections;

using UtilityLibrary.Wizards;

namespace UtilityLibrary.Designers
{
  /// <summary>
  /// Designer of Wizard Page
  /// </summary>
  public class WizardWelcomePageDesigner : ParentControlDesigner
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
    public WizardWelcomePageDesigner()
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
      properties.Remove( "WelcomePage" );
    }

    /// <summary>
    /// Custom Drawing of Wizard Page in IDE Designer
    /// </summary>
    /// <param name="pe"></param>
    protected override void OnPaintAdornments( System.Windows.Forms.PaintEventArgs pe )
    {
      base.OnPaintAdornments( pe );

      Graphics g = pe.Graphics;

      WizardPageBase page = Control as WizardPageBase;
      
      if( page == null )
      {
        page = Component as WizardPageBase;
      }
      
      if( page != null )
      {
        WizardForm parent = page.WizardPageParent;
        
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
      }
    }
    #endregion
  }
}
