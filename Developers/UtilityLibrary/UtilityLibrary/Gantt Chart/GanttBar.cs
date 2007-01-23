using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;

using UtilityLibrary.General;


namespace UtilityLibrary.Gantt_Chart
{
  public class GanttAnhorsBar : GanttProgressBar
  {
    #region Class constants
    private const int iOffset = 4;
    private const int iCornerH = 5;
    private const int iCornerW = 10;
    #endregion

    #region Class Members
    // performace boosters
    private PointF[] points = null;
    private PointF[] pointsDark1 = null;
    private PointF[] pointsDark2 = null;
    private PointF[] pointsDark3 = null;
    #endregion

    #region Class Coinstructor
    public GanttAnhorsBar() : base()
    {
    }
    #endregion
    
    #region Class Paint methods
    protected override void OnPaint(PaintEventArgs pe)
    {
      base.OnPaint( pe );

      Graphics g = pe.Graphics;
      Rectangle rc = ClientRectangle;

      rc.Height--;

      int iHeight = rc.Height - iCornerH;
      int iWidth =  rc.Width - 1 - iOffset * 2 - iCornerW * 2;

      if( points == null )
      {
        points = new PointF[]
        {
          /*0*/     new PointF( rc.X,                                    rc.Y ),
          /*1*/     new PointF( rc.X,                                    rc.Y + iHeight ),
          /*2*/     new PointF( rc.X + iOffset,                          rc.Y + iHeight ),
          /*3*/     new PointF( rc.X + iOffset + iCornerW/2,             rc.Y + iHeight + iCornerH ),
          /*4*/     new PointF( rc.X + iOffset + iCornerW,               rc.Y + iHeight ),
          /*5*/     new PointF( rc.X + iOffset + iCornerW + iWidth,      rc.Y + iHeight ),
          /*6*/     new PointF( rc.X + iOffset + iCornerW*3/2 + iWidth,  rc.Y + iHeight + iCornerH ),
          /*7*/     new PointF( rc.X + iOffset + iCornerW*2 + iWidth,    rc.Y + iHeight ),
          /*8*/     new PointF( rc.X + iOffset*2 + iCornerW*2 + iWidth,  rc.Y + iHeight ),
          /*9*/     new PointF( rc.X + iOffset*2 + iCornerW*2 + iWidth,  rc.Y ),
          /*10*/    new PointF( rc.X,                                    rc.Y )
        };

        pointsDark1 = new PointF[]
        { 
          points[1], new PointF( points[2].X - 1, points[2].Y )
        };

        pointsDark2 = new PointF[]
        { 
          points[3], points[4], 
          new PointF( points[5].X - 1, points[5].Y )
        };

        pointsDark3 = new PointF[]
        { 
          points[6], points[7], points[8], points[9] 
        };
      }

      // fill bar rectangle
      g.FillRectangle( new SolidBrush( BackColor ), ClientRectangle );

      Color clrHot = ( base.IsHot ) ? ColorUtil.VSNetSelectionColor : base.BarColor;

      Brush brushCtrl = ( base.StyledBackground ) ?
        (Brush)new HatchBrush( base.BarBackgroundStyle, base.ForeColor, clrHot ) :
        (Brush)new SolidBrush( clrHot );
      
      // fill bar internal space
      g.FillClosedCurve( brushCtrl, points, FillMode.Alternate, 0 );
      
      // draw bar border
      if( base.IsHot )
      {
        g.DrawLines( SystemPens.Highlight, points );
      }
      else
      {
        g.DrawLines( SystemPens.ControlLightLight, points );
        g.DrawLines( SystemPens.ControlDark, pointsDark1 );
        g.DrawLines( SystemPens.ControlDark, pointsDark2 );
        g.DrawLines( SystemPens.ControlDark, pointsDark3 );
      }
      
      brushCtrl.Dispose();

      // draw progress
      float fProgress = ( base.Position - base.Min ) * 100 / ( base.Max - base.Min );
      int iProgress = (int)(( rc.Width - 5 ) * fProgress/100);
      Rectangle rcPrg = new Rectangle( rc.X + 2, rc.Y + 2, iProgress, iHeight - 4 );

      HatchBrush brush = new HatchBrush( base.ProgressStyle, base.ProgressForeColor, base.ProgressBackColor );
      g.FillRectangle( brush, rcPrg );                                                         
      g.DrawRectangle( new Pen( base.ProgressForeColor ), rcPrg.X, rcPrg.Y, rcPrg.Width - 1, rcPrg.Height - 1 );
      brush.Dispose();
    }
    #endregion

    #region Class Helper methods
    protected override void InvalidateProgress()
    {
      Rectangle rc = ClientRectangle;
      int iHeight = rc.Height - iCornerH;
      Invalidate( new Rectangle( rc.X + 2, rc.Y + 2, rc.Width - 4, iHeight - 4 ) );
    }
    protected override bool IsInBarArea()
    {
      Point pnt = Control.MousePosition;
      pnt = this.PointToClient( pnt );

      RectangleF test1 = RectangleF.FromLTRB( points[0].X, points[0].Y, points[7].X, points[7].Y );
      
      if( test1.Contains( pnt ) == false )
      {
        test1 = RectangleF.FromLTRB( points[2].X, points[2].Y, points[4].X, points[3].Y );
        
        if( test1.Contains( pnt ) == false )
        {
          test1 = RectangleF.FromLTRB( points[5].X, points[5].Y, points[7].X, points[6].Y );
          
          return test1.Contains( pnt );
        }
      }

      return true;
    }
    #endregion

    #region Resize reacts
    protected override void OnResize( System.EventArgs e )
    {
      points = null;
      base.OnResize( e );
    }
    #endregion
  }
}
