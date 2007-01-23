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
  public class GanttProgressBar : System.Windows.Forms.Control
  {
    #region Class Members
    protected static GDIUtils m_gdi = new GDIUtils();

    private int         m_iPos;
    private int         m_iMin;
    private int         m_iMax = 100;
    private bool        m_bSkipEvents;
    private bool        m_bHot;
    private bool        m_bStyled;
    private HatchStyle  m_enStyle = HatchStyle.DarkUpwardDiagonal;
    private HatchStyle  m_enBarStyle = HatchStyle.Percent50;
    private Color       m_clrFore = Color.Black;
    private Color       m_clrBack = Color.White;
    private Color       m_clrBar  = ColorUtil.VSNetControlColor;
    #endregion

    #region Class events
    [ Category( "Property Changed" ) ]
    public event EventHandler MinChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler MaxChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler PositionChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler ProgressStyleChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler ProgressForeColorChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler ProgressBackColorChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler BarColorChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler BarBackgroundStyleChanged;
    [ Category( "Property Changed" ) ]
    public event EventHandler StyledBackgroundChanged;
    #endregion

    #region Class Properties
    [ Browsable( true ), 
    Category( "Behavior" ), 
    DefaultValue( 0 ),
    Description( "GET/SET Min Progress Position" ) ]
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

    [ Browsable( true ), 
    Category( "Behavior" ), 
    DefaultValue( 100 ),
    Description( "GET/SET Max Progress Position" ) ]
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
    
    [ Browsable( true ), 
    Category( "Behavior" ), 
    DefaultValue( 0 ),
    Description( "GET/SET Progress Position" ) ]
    public int Position
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
          OnPositionChanged();
        }
      }
    }
    
    [ Browsable( true ), 
    Category( "Appearance" ), 
    DefaultValue( typeof(ColorUtil), "VSNetControlColor" ),
    Description( "GET/SET Color of Bar Item" ) ]
    public Color BarColor
    {
      get
      {
        return m_clrBar;
      }
      set
      {
        if( value != m_clrBar )
        {
          m_clrBar = value;
          OnBarColorChanged();
        }
      }
    }
    
    [ Browsable( true ), Category( "Appearance" ), 
    DefaultValue( typeof(HatchStyle), "DarkUpwardDiagonal" ),
    Editor( "UtilityLibrary.Designers.HatchStylesEditor, UtilityLibrary.Designers, Version=1.0.4.0, Culture=neutral, PublicKeyToken=fe06e967d3cf723d", typeof( System.Drawing.Design.UITypeEditor ) ),
    Description( "GET/SET Progress Drawing Style" ) ]
    public HatchStyle ProgressStyle
    {
      get
      {
        return m_enStyle;
      }
      set
      {
        if( value != m_enStyle )
        {
          m_enStyle = value;
          OnProgressStyleChanged();
        }
      }
    }
    
    [ Browsable( true ), 
    Category( "Appearance" ), 
    DefaultValue( typeof(HatchStyle), "Percent50" ),
    Editor( "UtilityLibrary.Designers.HatchStylesEditor, UtilityLibrary.Designers, Version=1.0.4.0, Culture=neutral, PublicKeyToken=fe06e967d3cf723d", typeof( System.Drawing.Design.UITypeEditor ) ),
    Description( "GET/SET How Bar background will be drawed" ) ]
    public HatchStyle BarBackgroundStyle
    {
      get
      {
        return m_enBarStyle;
      }
      set
      {
        if( value != m_enBarStyle )
        {
          m_enBarStyle = value;
          OnBarBackgroundStyleChanged();
        }
      }
    }
    
    [ Browsable( true ), 
    Category( "Appearance" ), 
    DefaultValue( typeof(Color), "Black" ),
    Description( "GET/SET Progress bar fore color" ) ]
    public Color ProgressForeColor
    {
      get
      {
        return m_clrFore;
      }
      set
      {
        if( value != m_clrFore )
        {
          m_clrFore = value;
          OnProgressForeColorChanged();
        }
      }
    }

    [ Browsable( true ), 
    Category( "Appearance" ), 
    DefaultValue( typeof(Color), "White" ),
    Description( "GET/SET Progress bar background Color" ) ]
    public Color ProgressBackColor
    {
      get
      {
        return m_clrBack;
      }
      set
      {
        if( value != m_clrBack )
        {
          m_clrBack = value;
          OnProgressBackColorChanged();
        }
      }
    }
    
    [ Browsable( true ), 
    Category( "Appearance" ), 
    DefaultValue( false ),
    Description( "GET/SET does paint method use BarBackgroundStyle or solid brushes" ) ]
    public bool StyledBackground
    {
      get
      {
        return m_bStyled;
      }
      set
      {
        if( value != m_bStyled )
        {
          m_bStyled = value;
          OnStyledBackgroundChanged();
        }
      }
    }
    
    [ Browsable( false ) ]
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
    
    [ Browsable( false ) ]
    public bool IsHot
    {
      get
      {
        return m_bHot;
      }
    }
    
    
    protected override System.Drawing.Size DefaultSize
    {
      get
      {
        return new System.Drawing.Size( 100, 12 );
      }
    }
    #endregion

    #region Class Constructor
    public GanttProgressBar()
    {
      ControlStyles styleTrue = ControlStyles.AllPaintingInWmPaint |
        ControlStyles.DoubleBuffer |
        ControlStyles.Opaque |
        ControlStyles.UserPaint;

      ControlStyles styleFalse = ControlStyles.Selectable;

      SetStyle( styleTrue, true );
      SetStyle( styleFalse, false );

      ResizeRedraw = true;
    }
    #endregion
   
    #region Class Paint methods
    protected override void OnPaint(PaintEventArgs pe)
    {
      base.OnPaint( pe );

      Graphics g = pe.Graphics;
      Rectangle rc = ClientRectangle;

      Color clrHot = ( m_bHot ) ? ColorUtil.VSNetSelectionColor : m_clrBar;

      // fill bar rectangle
      Brush brushCtrl = ( m_bStyled ) ? 
        (Brush)new HatchBrush( m_enBarStyle, ForeColor, clrHot ) :
        (Brush)new SolidBrush( clrHot );

      // draw bar border
      if( m_bHot )
      {
        g.FillRectangle( brushCtrl, rc );
        g.DrawRectangle( SystemPens.Highlight, rc.X, rc.Y, rc.Width - 1, rc.Height - 1 );
      }
      else
      {
        g.DrawRectangle( SystemPens.ControlDark, rc.X, rc.Y, rc.Width-1, rc.Height-1 );
        g.DrawRectangle( SystemPens.ControlLightLight, rc.X, rc.Y, rc.Width-2, rc.Height-2 );
        g.FillRectangle( brushCtrl, new Rectangle( rc.X+1, rc.Y+1, rc.Width-2, rc.Height-2 ) );
      }

      brushCtrl.Dispose();

      // draw progress
      float fProgress = ( m_iPos - m_iMin ) * 100 / ( m_iMax - m_iMin );
      int iProgress = (int)(( rc.Width - 5 ) * fProgress/100);
      Rectangle rcPrg = new Rectangle( rc.X + 2, rc.Y + 2, iProgress, rc.Height - 4 );

      HatchBrush brush = new HatchBrush( m_enStyle, m_clrFore, m_clrBack );
      g.FillRectangle( brush, rcPrg );                                                         
      g.DrawRectangle( new Pen( m_clrFore ), rcPrg.X, rcPrg.Y, rcPrg.Width - 1, rcPrg.Height - 1 );
      brush.Dispose();
    }
    #endregion

    #region Class Helper methods
    protected virtual void InvalidateProgress()
    {
      Invalidate();
    }

    protected virtual bool IsInBarArea()
    {
      Point pnt = Control.MousePosition;
      pnt = this.PointToClient( pnt );

      RectangleF test1 = ClientRectangle;
      return test1.Contains( pnt );
    }
    #endregion

    #region Class Overrides
    protected virtual void OnPositionChanged()
    {
      InvalidateProgress();
      RaisePositionChangedEvent();
    }
    
    protected virtual void OnMaxChanged()
    {
      if( m_iPos > m_iMax ) m_iPos = m_iMax;

      InvalidateProgress();

      RaiseMaxChangedEvent();
    }
    
    protected virtual void OnMinChanged()
    {
      if( m_iPos < m_iMin ) m_iPos = m_iMin;

      InvalidateProgress();

      RaiseMinChangedEvent();
    }
    
    protected virtual void OnQuietModeChanged()
    {
      // NOTE: can be used by inheritors
    }
    protected virtual void OnProgressStyleChanged()
    {
      InvalidateProgress();
      RaiseProgressStyleChangedEvent();
    }
    
    protected virtual void OnProgressForeColorChanged()
    {
      InvalidateProgress();
      RaiseProgressForeColorChangedEvent();
    }

    protected virtual void OnProgressBackColorChanged()
    {
      InvalidateProgress();
      RaiseProgressBackColorChangedEvent();
    }
    
    protected virtual void OnBarColorChanged()
    {
      Invalidate();
      RaiseBarColorChangedEvent();
    }
    protected virtual void OnBarBackgroundStyleChanged()
    {
      Invalidate();
      RaiseBarBackgroundStyleChangedEvent();
    }
    protected virtual void OnStyledBackgroundChanged()
    {
      Invalidate();
      RaiseStyledBackgroundChangedEvent();
    }
    #endregion

    #region Event Raisers
    protected void RaisePositionChangedEvent()
    {
      if( PositionChanged != null && QuietMode == false )
      {
        PositionChanged( this, EventArgs.Empty );
      }
    }
    
    protected void RaiseMaxChangedEvent()
    {
      if( MaxChanged != null && QuietMode == false )
      {
        MaxChanged( this, EventArgs.Empty );
      }
    }
    
    protected void RaiseMinChangedEvent()
    {
      if( MinChanged != null && QuietMode == false )
      {
        MinChanged( this, EventArgs.Empty );
      }
    }

    protected void RaiseProgressStyleChangedEvent()
    {
      if( ProgressStyleChanged != null && QuietMode == false )
      {
        ProgressStyleChanged( this, EventArgs.Empty );
      }
    }
    protected void RaiseProgressForeColorChangedEvent()
    {
      if( ProgressForeColorChanged != null && QuietMode == false )
      {
        ProgressForeColorChanged( this, EventArgs.Empty );
      }
    }
    protected void RaiseProgressBackColorChangedEvent()
    {
      if( ProgressBackColorChanged != null && QuietMode == false )
      {
        ProgressBackColorChanged( this, EventArgs.Empty );
      }
    }
    
    protected void RaiseBarColorChangedEvent()
    {
      if( BarColorChanged != null && QuietMode == false )
      {
        BarColorChanged( this, EventArgs.Empty );
      }
    }
    
    protected void RaiseBarBackgroundStyleChangedEvent()
    {
      if( BarBackgroundStyleChanged != null && QuietMode == false )
      {
        BarBackgroundStyleChanged( this, EventArgs.Empty );
      }
    }
    
    protected void RaiseStyledBackgroundChangedEvent()
    {
      if( StyledBackgroundChanged != null && QuietMode == false )
      {
        StyledBackgroundChanged( this, EventArgs.Empty );
      }
    }
    #endregion

    #region Mouse Events
    protected override void OnMouseEnter( System.EventArgs e )
    {
      base.OnMouseEnter( e );
      
      if( IsInBarArea() == true && m_bHot == false )
      {
        m_bHot = true;
        Invalidate();
      }
    }

    protected override void OnMouseLeave( System.EventArgs e )
    {
      base.OnMouseLeave( e );
      m_bHot = false;
      Invalidate();
    }

    protected override void OnMouseMove( System.Windows.Forms.MouseEventArgs e )
    {
      base.OnMouseMove( e );
      
      if( m_bHot == false && IsInBarArea() == true )
      {
        m_bHot = true;
        Invalidate();
      }
    }
    #endregion
  }
}
