using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using UtilityLibrary.General;
using UtilityLibrary.Win32;

using System.Diagnostics;


namespace UtilityLibrary.DateTimeControls
{
  [ ToolboxItem(true), 
  Designer( "UtilityLibrary.Designers.CalendarViewExDesigner, UtilityLibrary.Designers, Version=1.0.4.0, Culture=neutral, PublicKeyToken=fe06e967d3cf723d" ),
  ToolboxBitmap(typeof(UtilityLibrary.DateTimeControls.CalendarViewEx), 
    "UtilityLibrary.DateTimeControls.CalendarViewEx.bmp") ]
  public class CalendarViewEx : System.Windows.Forms.Control
  {
    #region Class constants
    private const int DEF_HEADER_SIZE = 18;
    private const int DEF_ARROW_SIZE  = 3;
    private const int DEF_FOOTER_SIZE = 24;
    private const int DEF_BUTTON_WIDTH = 46;
    private const int DEF_BUTTON_HEIGHT = 20;
    private const int DEF_WEEK_DAY_HEIGHT = 18;

    private const int DEF_COLUMNS_COUNT = 7;
    private const int DEF_ROWS_COUNT = 6;

    private const int DEF_TODAY_TAB_INDEX = 100;
    private const int DEF_NONE_TAB_INDEX = 101;
    #endregion

    #region Declarations and internal classes
    [Flags]
    public enum TRectangleStatus
    {
      Normal    = 0x0000,
      Active    = 0x0001,
      Selected  = 0x0002,
      Focused   = 0x0004,
      ActiveSelect = Active | Selected,
      FocusSelect = Focused | Selected,
      All = Active | Selected | Focused
    };

    public enum TRectangleAction
    {
      None,
      /// <summary>
      /// Previous month button
      /// </summary>
      MonthDown,
      /// <summary>
      /// Next month button
      /// </summary>
      MonthUp,
      /// <summary>
      /// Previous year button
      /// </summary>
      YearDown,
      /// <summary>
      /// Next Year button
      /// </summary>
      YearUp,
      /// <summary>
      /// Today button
      /// </summary>
      TodayBtn,
      /// <summary>
      /// None Button
      /// </summary>
      NoneBtn,
      /// <summary>
      /// Rectangle represent a single day
      /// </summary>
      MonthDay,
      /// <summary>
      /// Rectangle represent a week day name
      /// </summary>
      WeekDay
    };

    public class ActRect
    {
      #region Class members
      private Control           m_parent;
      private Rectangle         m_rect;
      private TRectangleStatus  m_state;
      private bool              m_bInvalidate = true;
      private TRectangleAction  m_act;
      private object            m_tag;
      #endregion

      #region Class properties
      public Rectangle Rect
      {
        get
        {
          return m_rect;
        }
        set
        {
          m_rect = value;
        }
      }
      
      public TRectangleStatus State
      {
        get
        {
          return m_state;
        }
        set
        {
          if( value != m_state )
          {
            m_state = value;
            if( m_bInvalidate == true )
              m_parent.Invalidate( m_rect );
          }
        }
      }
      
      public bool InvalidateOnChange
      {
        get
        {
          return m_bInvalidate;
        }
        set
        {
          m_bInvalidate = value;
        }
      }

      public TRectangleAction Action
      {
        get
        {
          return m_act;
        }
        set
        {
          m_act = value;
        }
      }
      public object Tag
      {
        get
        { 
          return m_tag; 
        }
        set
        {
          m_tag = value; 
        }
      }

      public bool IsFocused
      {
        get
        {
          return (m_state & TRectangleStatus.Focused) == TRectangleStatus.Focused;
        }
      }
      
      public bool IsSelected
      {
        get
        {
          return (m_state & TRectangleStatus.Selected) == TRectangleStatus.Selected;
        }
      }

      public bool IsActive
      {
        get
        {
          return (m_state & TRectangleStatus.Active) == TRectangleStatus.Active;
        }
      }
      #endregion

      #region Class Constructors
      private ActRect(){} // disable default contructor

      public ActRect( Control parent, Rectangle rc, TRectangleStatus state, TRectangleAction act, bool invalidate )
      {
        m_parent      = parent;
        m_rect        = rc;
        m_state       = state;
        m_bInvalidate = invalidate;
        m_act         = act;
      }

      public ActRect( Control parent, Rectangle rc, TRectangleStatus state, TRectangleAction act )
        : this( parent, rc, state, act, true ) 
      {
      }

      public ActRect( Control parent, Rectangle rc, TRectangleAction act, object tag )
      {
        m_parent      = parent;
        m_rect        = rc;
        m_state       = TRectangleStatus.Normal;
        m_act         = act;
        m_tag         = tag;
      }
      public ActRect( Control parent, Rectangle rc, TRectangleAction act )
        : this( parent, rc, TRectangleStatus.Normal, act, true ) 
      {
      }

      public ActRect( Control parent, Rectangle rc, TRectangleStatus state )
        : this( parent, rc, state, TRectangleAction.None, true )
      {
      }

      public ActRect( Control parent, Rectangle rc )
        : this( parent, rc, TRectangleStatus.Normal, TRectangleAction.None, true )
      {
      }
      
      public ActRect( Control parent )
        : this( parent, Rectangle.Empty, TRectangleStatus.Normal, TRectangleAction.None, true ) 
      {
      
      }
      #endregion
    }

    public class DateTimeCollection : System.Collections.CollectionBase, IEnumerable
    {
      #region class events
      public delegate void OnChange();
      public event OnChange ItemChanged;
      #endregion

      #region Class constructors
      public DateTimeCollection()
      {
      }
      #endregion

      #region IList interface support
      public int Add( DateTime valDate )
      {
        if( Contains( valDate ) ) 
          return -1;

        int index = InnerList.Add( valDate );
        FireItemChanged();
        return index;
      }

      public bool Contains( DateTime valDate )
      {
        return InnerList.Contains( valDate );
      }

      public int IndexOf( DateTime valDate )
      {
        return InnerList.IndexOf( valDate );
      }

      public void Insert(int index, DateTime value)
      {
        if( value.DayOfWeek != DayOfWeek.Sunday )
        {
          InnerList.Insert( index, value );
          FireItemChanged();
        }
      }

      public void Remove(DateTime value)
      {
        InnerList.Remove( value );
        FireItemChanged();
      }

      public DateTime this[int index]
      {
        get
        {
          return ( DateTime )InnerList[index];
        }
        set
        {
          InnerList[ index ] = value;
          FireItemChanged();
        }
      }
      #endregion

      #region Custom functions
      protected void FireItemChanged()
      {
        if( ItemChanged != null )
        {
          ItemChanged();
        }
      }
      #endregion
    }
    #endregion

    #region Class members
    private GDIUtils  m_gdi = new GDIUtils();
    private bool      m_bRectsCreated;
    
    private Rectangle m_rcHeader;
    private Rectangle m_rcFooter;
    private Rectangle m_rcBody;

    // array in which stored active rectangles
    private ArrayList m_rects = new ArrayList( 100 ); 

    private int m_iYear  = DateTime.Now.Year;
    private int m_iMonth = DateTime.Now.Month;
    private int m_iDay   = 1;
    
    private DateTime m_today = DateTime.Now;
    private DateTime m_dtSelected = DateTime.Now;

    private int   m_iLastFocused = 1; // 100 - Today, 101 - None... less values mean Day...
    private bool  m_bDropDownTab;

    private DateTimeCollection m_collSelected;
    private int m_iSelectionHeight;
    private int m_iSelectionWidth;
    private int m_iSelectionStart;
    #endregion

    #region Class events
    public delegate void CalendarClickHandler( object sender, TRectangleAction clickAction );
    
    [ Category( "Action" ) ]
    public event EventHandler         ValueChanged;
    [ Category( "Action" ) ]
    public event CalendarClickHandler ClickAction;
    #endregion

    #region Class Properties
    internal int Year
    {
      get
      {
        return m_iYear;
      }
      set
      {
        if( value != m_iYear )
        {
          m_iYear = value;
          OnYearChanged();
        } 
      }
    }

    internal int Month
    {
      get
      {
        return m_iMonth;
      }
      set
      {
        if( value != m_iMonth )
        {
          if( value <= 0 || value > 12 )
          {
            throw new ArgumentOutOfRangeException( "Month", "Month value must be in diapason from 1 till 12" );
          } 

          m_iMonth = value;
          OnMonthChanged();
        } 
      }
    }

    internal int Day
    {
      get
      {
        return m_iDay;
      }
      set
      {
        if( value != m_iDay )
        {
          if( value <= 0 || value > 31 )
          {
            throw new ArgumentOutOfRangeException( "Day", "Day value must be in diapason from 1 till 31" );
          } 

          m_iDay = value;
          OnDayChanged();
        } 
      }
    }

    [Browsable(false)]
    new public System.Drawing.Size Size
    {
      get
      {
        return new Size( 160, 152 );
      }
      set
      {
        base.Size = new Size( 160, 152 );
      }
    }

    [Browsable(true)]
    public DateTime Value
    {
      get
      {
        if( m_iDay != 0 ) 
          return m_dtSelected;

        return DateTime.MinValue;
      }
      set
      {
        if( value != m_dtSelected || m_iDay == 0 )
        {
          OnValueChanged( value );
          RaiseValueChanged();
        } 
      }
    }
    
    [Browsable(true)]
    public bool DropDownTab
    {
      get
      {
        return m_bDropDownTab;
      }
      set
      {
        if( value != m_bDropDownTab )
        {
          m_bDropDownTab = value;
          OnDropDownTabChanged();
        } 
      }
    }

    [Browsable(false)]
    public DateTimeCollection Selected
    {
      get
      {
        if( m_collSelected != null )
        {
          m_collSelected.ItemChanged -= new DateTimeCollection.OnChange( OnSelectedCollectionChanged );
          m_collSelected.Clear();
        }

        m_collSelected = GetSeletedRects();
        m_collSelected.ItemChanged += new DateTimeCollection.OnChange( OnSelectedCollectionChanged );
        return m_collSelected;
      }
    }
    #endregion

    #region Class constructor
    public CalendarViewEx()
    {
      ControlStyles styleTrue = 
        ControlStyles.AllPaintingInWmPaint |
        ControlStyles.DoubleBuffer |
        ControlStyles.EnableNotifyMessage |
        //        ControlStyles.ContainerControl |
        //ControlStyles.Opaque |
        ControlStyles.ResizeRedraw |
        //ControlStyles.Selectable |
        ControlStyles.FixedHeight |
        ControlStyles.FixedWidth |
        ControlStyles.UserPaint;

      SetStyle( styleTrue, true );

      base.Size = new Size( 160, 152 );
    }
    #endregion

    #region Class paint methods
    protected override void OnPaint( PaintEventArgs pe )
    {
      // active rectangles must be rebuild
      if( m_bRectsCreated == false || DesignMode == true )
      {
        m_rects.Clear();
      }

      OnPaintHeader( new PaintEventArgs( pe.Graphics, m_rcHeader ) );
      OnPaintFooter( new PaintEventArgs( pe.Graphics, m_rcFooter ) );

      if( m_bRectsCreated == false || DesignMode == true ) // recalculate Body
      {
        Rectangle rc = pe.ClipRectangle;
        m_rcBody = new Rectangle( rc.X, m_rcHeader.Bottom, rc.Width, m_rcFooter.Top - m_rcHeader.Bottom );
        m_rcBody = Rectangle.Inflate( m_rcBody, -4, -1 );
      }

      OnPaintBody( new PaintEventArgs( pe.Graphics, m_rcBody ) );

      m_bRectsCreated = true; // indicate that control calculate all own rectangles
    }

    protected override void OnPaintBackground( PaintEventArgs pevent )
    {
      Graphics g = pevent.Graphics;
      Rectangle rc = ClientRectangle;

      g.FillRectangle( Brushes.White, rc );

      // draw header background
      m_rcHeader = new Rectangle( rc.X, rc.Y, rc.Width-1, DEF_HEADER_SIZE );
      g.FillRectangle( SystemBrushes.Control, m_rcHeader );
      m_gdi.Draw3DBox( g, m_rcHeader, Canvas3DStyle.Title );

      // draw footer background
      int yBott = rc.Bottom - DEF_FOOTER_SIZE - 1;
      m_rcFooter = new Rectangle( rc.X+6, yBott, rc.Width-12, DEF_FOOTER_SIZE );
      g.FillRectangle( Brushes.White, m_rcFooter );
      m_gdi.Draw3DLine( g, new Point( new Size( rc.X + 6, yBott ) ), new Point( new Size( rc.Right - 6, yBott ) ) );

      if( m_bRectsCreated == true && m_iDay != 0 )
      {
        foreach( ActRect rect in m_rects )
        {
          switch( rect.State & TRectangleStatus.ActiveSelect )
          {
            case TRectangleStatus.ActiveSelect:
              m_gdi.DrawActiveRectangle( g, rect.Rect, HightlightStyle.Selected, true );
              break;

            case TRectangleStatus.Active:
              m_gdi.DrawActiveRectangle( g, rect.Rect, HightlightStyle.Active, true );
              break;

            case TRectangleStatus.Selected:
              g.FillRectangle( Brushes.Silver, rect.Rect );
              break;
          }
        }
      }
    }

    protected virtual  void OnPaintHeader( PaintEventArgs pevent )
    {
      Rectangle rc = pevent.ClipRectangle;
      Rectangle rcOut = Rectangle.Inflate( rc, -6, -1 ); 

      PaintEventArgs ev = new PaintEventArgs( pevent.Graphics, rcOut ); 

      OnPaintMonthHeader( ev );
      OnPaintYearHeader( ev ); 
    }

    protected virtual  void OnPaintMonthHeader( PaintEventArgs pevent )
    {
      Graphics g = pevent.Graphics;
      Rectangle rc = pevent.ClipRectangle;
      
      DateTime date = new DateTime( m_iYear, m_iMonth, 1, 0, 0, 0 );
      string strMonth = date.ToString( "MMMM" );
      
      // Draw left arrow and add it as Active Rectangle
      Rectangle rect = DrawArrow( g, rc, true ); 
      Rectangle rcOut = Rectangle.Inflate( rect, 1, 1 ); 
      rcOut.X++;
      AddActiveRect( rcOut, TRectangleAction.MonthDown ); 
      
      //SizeF sz = g.MeasureString( strMonth, this.Font );
      SizeF sz = GetMaxMonthSize( g );
      //Rectangle rcText = new Rectangle( rect.Right + 4, rc.Y, (int)sz.Width + 8, rc.Height );
      Rectangle rcText = new Rectangle( rect.Right + 4, rc.Y, (int)sz.Width, rc.Height );
      g.DrawString( strMonth, this.Font, SystemBrushes.WindowText, rcText, GDIUtils.OneLineNoTrimming ); 
      
      // draw  right arrow and add it like Active Rectangle
      rect = DrawArrow( g, new Rectangle( rcText.Right + 4, rc.Y, 100, rc.Height ), false );
      AddActiveRect( Rectangle.Inflate( rect, 1, 1 ), TRectangleAction.MonthUp ); 
    }
    
    protected virtual  void OnPaintYearHeader( PaintEventArgs pevent )
    {
      Graphics g = pevent.Graphics;
      Rectangle rc = pevent.ClipRectangle;
     
      DateTime date = new DateTime( m_iYear, m_iMonth, 1, 0, 0, 0 );
      string strYear = date.ToString( "yyyy" );

      // draw right arrow
      Rectangle rect = DrawArrow( g, 
        new Rectangle( rc.Right - 4 - DEF_ARROW_SIZE - 2, rc.Y, DEF_ARROW_SIZE * 2, rc.Height ), 
        false );
      AddActiveRect( Rectangle.Inflate( rect, 1, 1 ), TRectangleAction.YearUp ); 

      SizeF sz = g.MeasureString( strYear, this.Font );
      Rectangle rcText = new Rectangle( rect.Left - 4 - (int)sz.Width - 8, rc.Y, (int)sz.Width + 8, rc.Height );
      g.DrawString( strYear, this.Font, SystemBrushes.WindowText, rcText, GDIUtils.OneLineNoTrimming ); 
      
      // draw left arrow
      rect = DrawArrow( g,
        new Rectangle( rcText.Left - 4 - DEF_ARROW_SIZE - 2, rc.Y, DEF_ARROW_SIZE*2, rc.Height ),
        true );
      Rectangle rcOut = Rectangle.Inflate( rect, 1, 1 ); 
      rcOut.X++;
      AddActiveRect( rcOut, TRectangleAction.YearDown ); 
    }

    protected virtual  void OnPaintFooter( PaintEventArgs pevent )
    {
      Graphics g = pevent.Graphics;
      Rectangle rc = pevent.ClipRectangle;
      
      Rectangle rcOut = Rectangle.Inflate( rc, -4, -1 ); 
      OnPaintFooterButtons( new PaintEventArgs( g, m_rcFooter ) );
    }

    protected virtual  void OnPaintFooterButtons( PaintEventArgs pevent )
    {
      Graphics g = pevent.Graphics;
      Rectangle rc = pevent.ClipRectangle;
      Rectangle focus;
      
      Rectangle rcToday = new Rectangle( rc.X, rc.Y + rc.Height/2 - DEF_BUTTON_HEIGHT/2, 
        DEF_BUTTON_WIDTH, DEF_BUTTON_HEIGHT );

      g.FillRectangle( SystemBrushes.Control, rcToday ); 
      m_gdi.Draw3DBox( g, rcToday, Canvas3DStyle.Flat );
      g.DrawString( "&Today", this.Font, SystemBrushes.WindowText, rcToday, GDIUtils.OneLineNoTrimming );
      
      if( m_iLastFocused == 100 && base.Focused == true )
      {
        focus = new Rectangle( rcToday.X + 2, rcToday.Y + 2, rcToday.Width - 3, rcToday.Height - 3 );
        ControlPaint.DrawFocusRectangle( g, focus );
      } 

      AddActiveRect( rcToday, TRectangleAction.TodayBtn, DEF_TODAY_TAB_INDEX ); 

      Rectangle rcNone = new Rectangle( rcToday.Right + 4, rcToday.Y, rcToday.Width, rcToday.Height ); 
      g.FillRectangle( SystemBrushes.Control, rcNone ); 
      m_gdi.Draw3DBox( g, rcNone, Canvas3DStyle.Flat );
      g.DrawString( "&None", this.Font, SystemBrushes.WindowText, rcNone, GDIUtils.OneLineNoTrimming );
      
      if( m_iLastFocused == 101 && base.Focused == true )
      {
        focus = new Rectangle( rcNone.X + 2, rcNone.Y + 2, rcNone.Width - 3, rcNone.Height - 3 );
        ControlPaint.DrawFocusRectangle( g, focus );
      } 

      AddActiveRect( rcNone, TRectangleAction.NoneBtn, DEF_NONE_TAB_INDEX ); 
    }
    
    protected virtual  void OnPaintBody( PaintEventArgs pevent )
    {
      Graphics g = pevent.Graphics;
      Rectangle rc = pevent.ClipRectangle;
      DateTime date = new DateTime( m_iYear, m_iMonth, 1, 0, 0, 0 ); 
      
      if( date.DayOfWeek != DayOfWeek.Sunday )
      {
        date = date.Subtract( new TimeSpan( (int)date.DayOfWeek, 0, 0, 0, 0 ) );
      }
      else
      {
        date = date.Subtract( new TimeSpan( 7, 0, 0, 0, 0 ) );
      }

      int iColWidth = rc.Width / DEF_COLUMNS_COUNT;
      int iRowHeight= ( rc.Height - DEF_WEEK_DAY_HEIGHT ) / DEF_ROWS_COUNT;

      m_gdi.Draw3DLine( g, new Point( new Size( rc.X+2, rc.Y + DEF_WEEK_DAY_HEIGHT - 1 ) ),
        new Point( new Size( rc.Right-2, rc.Y + DEF_WEEK_DAY_HEIGHT - 1 ) ) );

      Rectangle rcHead = new Rectangle( rc.X, rc.Y, iColWidth, DEF_WEEK_DAY_HEIGHT - 2 );

      for( int i=0; i<DEF_COLUMNS_COUNT; i++ )
      {
        rcHead.X = rc.X + i*iColWidth;
        string strDayWeek = "" + date.ToString( "dddd" )[0];
        
        g.DrawString( strDayWeek, this.Font, 
          ( date.DayOfWeek != DayOfWeek.Sunday ) ? SystemBrushes.WindowText : Brushes.Red, 
          rcHead, GDIUtils.OneLineNoTrimming ); 

        AddActiveRect( rcHead, TRectangleAction.WeekDay ); 
        
        if( m_iDay != 0 ) // We have correct day
        {
          DateTime dateNew = new DateTime( date.Ticks );
          Brush brush;
          Rectangle rcDay = new Rectangle( rcHead.X, rc.Y + DEF_WEEK_DAY_HEIGHT, rcHead.Width, iRowHeight );
        
          for( int j=0; j<DEF_ROWS_COUNT; j++ )
          {
            rcDay.Y = rc.Y + DEF_WEEK_DAY_HEIGHT + j*iRowHeight;
            string strDay = dateNew.ToString( "dd" );
            int    index = -1;

            if( m_iLastFocused == index && base.Focused == true )
            {
              g.FillRectangle( Brushes.Gray, rcDay );
            }

            if( dateNew.Day == m_today.Day && dateNew.Month == m_today.Month && dateNew.Year == m_today.Year )
            {
              g.DrawRectangle( Pens.Red, rcDay.X, rcDay.Y, rcDay.Width-1, rcDay.Height-1 );
            }
          
            if( dateNew.Month != m_iMonth )
            {
              brush = Brushes.Gray;
              index = -dateNew.Day;
            }
            else if( dateNew.DayOfWeek == DayOfWeek.Sunday )
            {
              brush = Brushes.Red;
              index = dateNew.Day;
            }
            else
            {
              brush = SystemBrushes.WindowText;
              index = dateNew.Day;
            }

            g.DrawString( strDay, this.Font, brush, rcDay, GDIUtils.OneLineNoTrimming );
            AddActiveRect( rcDay, TRectangleAction.MonthDay, index );

            if( m_iLastFocused == index && base.Focused == true )
            {
              ControlPaint.DrawFocusRectangle( g, Rectangle.Inflate( rcDay, -1, -1 ) );
            }

            dateNew = dateNew.AddDays(7);
          } 
        }
        else // None day selected
        {
          Rectangle rcNone = new Rectangle( rc.X, rc.Y + DEF_WEEK_DAY_HEIGHT, 
            rc.Width, rc.Height - DEF_WEEK_DAY_HEIGHT );
          
          if( m_iLastFocused < DEF_TODAY_TAB_INDEX )
            ControlPaint.DrawFocusRectangle( g, Rectangle.Inflate( rcNone, -2, -2 ) );

          g.DrawString( "None", this.Font, SystemBrushes.WindowText, rcNone, GDIUtils.OneLineNoTrimming );
        }

        date = date.AddDays(1);
      } 
    }
    
    
    protected override void OnResize(System.EventArgs e)
    {
      m_bRectsCreated = false;
      Invalidate();
    }

    protected SizeF GetMaxMonthSize( Graphics g )
    {
      SizeF szOut = SizeF.Empty;

      for( int i=1; i<=12; i++ )
      {
        DateTime date = new DateTime( m_iYear, i, 1, 0, 0, 0 );
        string strMonth = date.ToString( "MMMM" );
        SizeF sz = g.MeasureString( strMonth, this.Font );

        szOut.Width = Math.Max( szOut.Width, sz.Width );
        szOut.Height = Math.Max( szOut.Height, sz.Height );
      }

      return szOut;
    }
    protected Rectangle DrawArrow( Graphics g, Rectangle rc, bool isLeft )
    {
      int xLeft, xRight, yTop, yMidd, yBott;

      xLeft   = rc.Left + 1;
      xRight  = xLeft + DEF_ARROW_SIZE;
      yMidd   = rc.Top + (rc.Height / 2);
      yTop    = yMidd - DEF_ARROW_SIZE;
      yBott   = yMidd + DEF_ARROW_SIZE;

      Point[] array;

      if( isLeft == true )
      {
        array = new Point[]
        {
          new Point( new Size( xLeft,  yMidd ) ),
          new Point( new Size( xRight, yTop  ) ),
          new Point( new Size( xRight, yBott ) )
        };
      } 
      else
      {
        array = new Point[]
        {
          new Point( new Size( xLeft,  yTop  ) ),
          new Point( new Size( xLeft,  yBott ) ),
          new Point( new Size( xRight, yMidd ) )
        };
      }

      g.DrawPolygon( SystemPens.WindowText, array );
      g.FillPolygon( SystemBrushes.WindowText, array ); 

      return new Rectangle( xLeft - 2, yTop - 2, DEF_ARROW_SIZE + 4, DEF_ARROW_SIZE * 2 + 4 );
    }
    protected void AddActiveRect( Rectangle rc, TRectangleAction action, object tag )
    {
      if( m_bRectsCreated == false )
      {
        m_rects.Add( new ActRect( this, rc, action, tag ) ); 
      } 
    }
    protected void AddActiveRect( Rectangle rc, TRectangleAction action )
    {
      if( m_bRectsCreated == false )
      {
        m_rects.Add( new ActRect( this, rc, action ) ); 
      } 
    }
    #endregion

    #region Class overrides
    protected virtual void OnMonthChanged()
    {
      Value = new DateTime( m_iYear, m_iMonth, ( m_iDay < 1 ) ? m_iDay : 1, 0, 0, 0 ); 
    }
    
    protected virtual void OnYearChanged()
    {
      Value = new DateTime( m_iYear, m_iMonth, ( m_iDay < 1 ) ? m_iDay : 1, 0, 0, 0 ); 
    }

    protected virtual void OnDayChanged()
    {
      Value = new DateTime( m_iYear, m_iMonth, ( m_iDay < 1 ) ? m_iDay : 1, 0, 0, 0 ); 
    }

    protected virtual void OnValueChanged( DateTime value )
    {
      ResetAllRectangleStates();

      if( value == DateTime.MinValue )
      {
        m_iDay = 0;
        m_dtSelected = value;
        Invalidate();
        return;
      } 

      bool bRecreate = ( m_dtSelected.Month != value.Month || m_dtSelected.Year != value.Year );
      if( m_bRectsCreated == true && bRecreate == true )  m_bRectsCreated = false;
      
      m_dtSelected = value;
      m_iYear = m_dtSelected.Year;
      m_iMonth = m_dtSelected.Month;
      m_iDay = m_dtSelected.Day;
      
      if( bRecreate == true )
      { 
        Invalidate(); 
        Update(); 
      }

      ActRect rect = FindActiveRectByTag( m_iDay );
      if( m_iLastFocused < DEF_TODAY_TAB_INDEX )  m_iLastFocused = m_iDay;
      if( rect != null )  rect.State |= TRectangleStatus.FocusSelect;

      Invalidate();
      Update();
    }
    
    protected virtual void OnDropDownTabChanged()
    {
      
    }
    

    protected void OnSelectedCollectionChanged()
    {
      foreach( DateTime date in m_collSelected )
      {
        if( date.Month == m_dtSelected.Month && date.Year == m_dtSelected.Year )
        {
          ActRect rect = FindActiveRectByTag( date.Day );
          
          if( rect != null )
          {
            rect.State |= TRectangleStatus.Selected;
          } 
        } 
      } 
    }


    protected void ScrollDaysLeft()
    {
      if( m_iLastFocused < DEF_TODAY_TAB_INDEX )
        Value = m_dtSelected.AddDays( -1 );
    }

    protected void ScrollDaysRight()
    {
      if( m_iLastFocused < DEF_TODAY_TAB_INDEX )
        Value = m_dtSelected.AddDays( 1 );
    }

    protected void ScrollDaysUp()
    {
      if( m_iLastFocused < DEF_TODAY_TAB_INDEX )
        Value = m_dtSelected.AddDays( -7 );
    }

    protected void ScrollDaysDown()
    {
      if( m_iLastFocused < DEF_TODAY_TAB_INDEX )
        Value = m_dtSelected.AddDays( 7 );
    }


    protected void SetFocusOnNextControl()
    {
      ResetFocusedRectangleState();

      if( m_iLastFocused < DEF_TODAY_TAB_INDEX )
      {
        m_iLastFocused = DEF_TODAY_TAB_INDEX;
      } 
      else if( m_iLastFocused == DEF_TODAY_TAB_INDEX )
      {
        m_iLastFocused = DEF_NONE_TAB_INDEX;
      }
      else if( m_bDropDownTab == true )
      {
        if( m_iDay != 0 )
        {
          m_iLastFocused = m_iDay;
          
          ActRect rc = FindActiveRectByTag( m_iDay );
          if( rc != null )
          {
            rc.State |= TRectangleStatus.Focused | TRectangleStatus.Selected;
          }
        }
      }
      else
      {
        Control ctrl = this.FindForm().GetNextControl( this, true );
        if( ctrl != null ) ctrl.Focus();
      }

      Invalidate();
    }
    
    protected void SetFocusOnPrevControl()
    {
      ResetFocusedRectangleState();

      if( m_iLastFocused < DEF_TODAY_TAB_INDEX && m_bDropDownTab == true )
      {
        m_iLastFocused = DEF_NONE_TAB_INDEX;
      } 
      else if( m_iLastFocused == DEF_TODAY_TAB_INDEX && m_iDay != 0 )
      {
        m_iLastFocused = m_iDay;
        
        ActRect rc = FindActiveRectByTag( m_iDay );
        if( rc != null )
        {
          rc.State |= TRectangleStatus.Focused | TRectangleStatus.Selected;
        }
      }
      else if( m_iLastFocused == DEF_NONE_TAB_INDEX )
      {
        m_iLastFocused = DEF_TODAY_TAB_INDEX;
      }
      else
      {
        Control ctrl = this.FindForm().GetNextControl( this, false );
        if( ctrl != null ) ctrl.Focus();
      }
      
      Invalidate();
    }


    protected void ToNextYear()
    {
      Value = m_dtSelected.AddYears( 1 );
    }

    protected void ToPrevYear()
    {
      Value = m_dtSelected.AddYears( -1 );
    }

    protected void ToNextMonth()
    {
      Value = m_dtSelected.AddMonths( 1 );
    }
    
    protected void ToPrevMonth()
    {
      Value = m_dtSelected.AddMonths( -1 );
    }


    private void UpdateSelectionRectangle( int start, int width, int height )
    {
      int xLeft = start + width;
      int xTop  = start + height*7;

      // TODO: algorithm which can select rectangle of items
    }

    protected void RecalculateSelectionUp()
    {
      DateTime newDate = m_dtSelected.AddDays( -7 );

      if( newDate.Month != m_dtSelected.Month ) // switch to another month
      {
        ScrollDaysUp();
      }
      else
      {
        ResetFocusedRectangleState();
        if( Selected.Count == 1 ) m_iSelectionStart = m_dtSelected.Day;
        Value = newDate;
        UpdateSelectionRectangle( m_iSelectionStart, m_iSelectionWidth, ++m_iSelectionHeight );
      }
    }
    
    protected void RecalculateSelectionDown()
    {
      DateTime newDate = m_dtSelected.AddDays( 7 );

      if( newDate.Month != m_dtSelected.Month ) // switch to another month
      {
        ScrollDaysDown();
      }
      else
      {
        ResetFocusedRectangleState();
        if( Selected.Count == 1 ) m_iSelectionStart = m_dtSelected.Day;
        Value = newDate;
        UpdateSelectionRectangle( m_iSelectionStart, m_iSelectionWidth, --m_iSelectionHeight );
      }
    }
    
    protected void RecalculateSelectionLeft()
    {
      DateTime newDate = m_dtSelected.AddDays( -1 );

      if( newDate.Month != m_dtSelected.Month ) // switch to another month
      {
        ScrollDaysLeft();
      }
      else
      {
        ResetFocusedRectangleState();
        if( Selected.Count == 1 ) m_iSelectionStart = m_dtSelected.Day;
        Value = newDate;
        UpdateSelectionRectangle( m_iSelectionStart, ++m_iSelectionWidth, m_iSelectionHeight );
      }
    }
    
    protected void RecalculateSelectionRight()
    {
      DateTime newDate = m_dtSelected.AddDays( 1 );

      if( newDate.Month != m_dtSelected.Month ) // switch to another month
      {
        ScrollDaysRight();
      }
      else
      {
        ResetFocusedRectangleState();
        if( Selected.Count == 1 ) m_iSelectionStart = m_dtSelected.Day;
        Value = newDate;
        UpdateSelectionRectangle( m_iSelectionStart, --m_iSelectionWidth, m_iSelectionHeight );
      }
    }

    
    protected void OnRectangleClick( ActRect rc )
    {
      switch( rc.Action )
      {
        case TRectangleAction.MonthDown:  ToPrevMonth(); break;
        case TRectangleAction.MonthUp:    ToNextMonth(); break;
        case TRectangleAction.YearDown:   ToPrevYear();  break;
        case TRectangleAction.YearUp:     ToNextYear();  break;
        case TRectangleAction.TodayBtn:   SetTodayDay(); break;
        case TRectangleAction.NoneBtn:    SetNoneDay();  break;
        
        case TRectangleAction.MonthDay:
          if( m_iDay == 0 ) return;
          int index = (int)rc.Tag;
          DateTime newDate = new DateTime( m_iYear, m_iMonth, m_iDay, 0, 0, 0 ); 

          if( index < 0 && index > -10 )
          { 
            newDate = m_dtSelected.AddMonths(1);
            index = -index; 
          }
          else if( index < 0 && index < -20 )
          { 
            newDate = m_dtSelected.AddMonths(-1);
            index = -index; 
          }

          Value = new DateTime( newDate.Year, newDate.Month, index, 0, 0, 0 ); 
          break;
      }
    }

    protected void OnSelectionClick( ActRect rc )
    {
      if( rc.Action == TRectangleAction.WeekDay )
      {

      }
      else if( rc.Action == TRectangleAction.MonthDay )
      {
        if( rc.IsSelected == false )
          rc.State |= TRectangleStatus.Selected;
        else
          rc.State = (TRectangleStatus)((int)rc.State & ~(int)TRectangleStatus.Selected);

        m_iLastFocused = (int)rc.Tag;
      }
    }

    protected void OnEnterPressed()
    {
      ResetSelectedRectangleState();

      ActRect rect = FindActiveRectByTag( m_iLastFocused );

      if( rect != null )
      {
        switch( rect.Action ) 
        {
          case TRectangleAction.TodayBtn:
            SetTodayDay();
            break;
          
          case TRectangleAction.NoneBtn:
            SetNoneDay();
            break;
        }
      }
    }

    #endregion

    #region Class Public Methods
    public void SetNoneDay()
    {
      Value = DateTime.MinValue;
    }

    public void SetTodayDay()
    {
      Value = DateTime.Today;
    }

    #endregion

    #region Class helper methods
    private ActRect FindActiveRectByPoint( Point pnt )
    {
      foreach( ActRect rc in m_rects )
      {
        if( rc.Rect.Contains( pnt ) == true )
          return rc;
      }

      return null;
    }

    private ActRect FindActiveRectByTag( object tag )
    {
      foreach( ActRect rect in m_rects )
      {
        if( rect.Tag != null && rect.Tag.Equals( tag ) == true )
          return rect;
      }

      return null;
    }
    private void ResetActiveRectanglesState()
    {
      foreach( ActRect rc in m_rects )
      {
        if( (rc.State & TRectangleStatus.Active) > 0 )
        {
          rc.State = (TRectangleStatus)((int)rc.State & ~(int)TRectangleStatus.Active);
        }
      }
    }

    private void ResetSelectedRectangleState()
    {
      foreach( ActRect rc in m_rects )
      {
        if( (rc.State & TRectangleStatus.Selected) > 0 )
        {
          rc.State = (TRectangleStatus)((int)rc.State & ~(int)TRectangleStatus.Selected);
        }
      }
    }
    
    private void ResetFocusedRectangleState()
    {
      foreach( ActRect rc in m_rects )
      {
        if( (rc.State & TRectangleStatus.Focused) > 0 )
        {
          rc.State = (TRectangleStatus)((int)rc.State & ~(int)TRectangleStatus.Focused);
        }
      }
    }

    private void ResetAllRectangleStates()
    {
      foreach( ActRect rc in m_rects )
      {
        rc.State = TRectangleStatus.Normal;
      }
    }
    private DateTimeCollection GetSeletedRects()
    {
      DateTimeCollection coll = new DateTimeCollection();

      foreach( ActRect rc in m_rects )
      {
        if( rc.IsSelected == true && rc.Tag != null )
        {
          int index = (int)rc.Tag;
          DateTime newDate = new DateTime( m_iYear, m_iMonth, m_iDay, 0, 0, 0 ); 

          if( index < 0 && index > -10 )
          { 
            newDate = m_dtSelected.AddMonths(1);
            index = -index; 
          }
          else if( index < 0 && index < -20 )
          { 
            newDate = m_dtSelected.AddMonths(-1);
            index = -index; 
          }

          coll.Add( new DateTime( newDate.Year, newDate.Month, index, 0, 0, 0 ) );
        }
      }

      return coll;
    }


    protected void RaiseValueChanged()
    {
      if( ValueChanged != null )
      {
        ValueChanged( this, EventArgs.Empty );
      } 
    }
    #endregion

    #region Mouse events
    protected override void OnMouseEnter(System.EventArgs e)
    {
      Point pnt = MousePosition;
      pnt = PointToClient( pnt );

      ResetActiveRectanglesState();

      ActRect rect = FindActiveRectByPoint( pnt );
      
      if( rect != null && rect.Action != TRectangleAction.WeekDay )
      {
        rect.State |= TRectangleStatus.Active;
      }
    }

    protected override void OnMouseLeave(System.EventArgs e)
    {
      ResetActiveRectanglesState();
    }

    protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
    {
      if( e.Delta < 0 )
      {
        ScrollDaysLeft();
      }
      else
      {
        ScrollDaysRight();
      }
    }

    protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
    {
      Point pnt = MousePosition;
      pnt = PointToClient( pnt );

      ResetActiveRectanglesState();

      ActRect rect = FindActiveRectByPoint( pnt );
      
      if( rect != null && rect.Action != TRectangleAction.WeekDay )
      {
        rect.State |= TRectangleStatus.Active;
      }
    }
    protected override void OnClick( System.EventArgs e )
    {
      this.Focus();

      Point pnt = MousePosition;
      pnt = PointToClient( pnt );

      ActRect rect = FindActiveRectByPoint( pnt );
      
      if( rect != null && rect.Action != TRectangleAction.WeekDay )
      {
        ResetActiveRectanglesState();
        ResetFocusedRectangleState();

        // if selection begin
        if( (ModifierKeys & (Keys.Control | Keys.Shift)) == 0 )
        {
          ResetSelectedRectangleState();
          OnRectangleClick( rect );
        }
        else
        {
          OnSelectionClick( rect );
        }
      }

      // raise event only when on active rectangle clicked
      if( ClickAction != null && rect != null )
      {
        ClickAction( this, rect.Action );
      }

      base.OnClick( e );
    }

    protected override void OnDoubleClick( System.EventArgs e )
    {
      this.Focus();

      Point pnt = MousePosition;
      pnt = PointToClient( pnt );

      ActRect rect = FindActiveRectByPoint( pnt );
      
      if( rect != null && rect.Action != TRectangleAction.WeekDay )
      {
        ResetActiveRectanglesState();
        ResetSelectedRectangleState();
        ResetFocusedRectangleState();

        OnRectangleClick( rect );
      }

      // send double click event to user only when on day double click catched
      if( rect != null && rect.Action == TRectangleAction.MonthDay )
      {
        base.OnDoubleClick( e );
      }
    }
    #endregion

    #region Keyboard and focus event handlers
    protected override void OnGotFocus(System.EventArgs e)
    {
      Invalidate();
    }

    protected override void OnLostFocus(System.EventArgs e)
    {
      ResetFocusedRectangleState();
    }

    protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
    {
      switch( e.Modifiers & (Keys.Alt | Keys.Control | Keys.Shift) )
      {
          // only shift pressed
        case Keys.Shift:
        switch( e.KeyCode )
        {
          case Keys.Tab:    SetFocusOnPrevControl(); break;
          case Keys.Down:   RecalculateSelectionDown();  break;
          case Keys.Up:     RecalculateSelectionUp();  break;
          case Keys.Left:   RecalculateSelectionLeft();  break;
          case Keys.Right:  RecalculateSelectionRight();  break;
        }
          break;

          // only alt pressed
        case Keys.Alt:
        switch( e.KeyCode )
        {
          case Keys.Left:  ToPrevMonth(); break;
          case Keys.Right: ToNextMonth(); break;
          case Keys.N:     SetNoneDay();  break;
          case Keys.T:     SetTodayDay(); break;
        }
          break;

          // only control pressed
        case Keys.Control:
        switch( e.KeyCode )
        {
          case Keys.Up:   ToNextYear(); break;
          case Keys.Down: ToPrevYear(); break;
        }
          break;

        default:
        switch( e.KeyCode )
        {
          case Keys.Down: 
            if( m_iLastFocused == DEF_TODAY_TAB_INDEX || m_iLastFocused == DEF_NONE_TAB_INDEX )
              SetFocusOnNextControl(); 
            else
              ScrollDaysDown();   
            break;
          case Keys.Up:
            if( m_iLastFocused == DEF_TODAY_TAB_INDEX || m_iLastFocused == DEF_NONE_TAB_INDEX )
              SetFocusOnPrevControl();
            else 
              ScrollDaysUp();     
            break;
          case Keys.Left: 
            if( m_iLastFocused == DEF_TODAY_TAB_INDEX || m_iLastFocused == DEF_NONE_TAB_INDEX )
              SetFocusOnPrevControl();
            else 
              ScrollDaysLeft();   
            break;
          case Keys.Right: 
            if( m_iLastFocused == DEF_TODAY_TAB_INDEX || m_iLastFocused == DEF_NONE_TAB_INDEX )
              SetFocusOnNextControl(); 
            else
              ScrollDaysRight(); 
            break;
          case Keys.Tab:  
            SetFocusOnNextControl(); 
            break;
            
          case Keys.Space:
          case Keys.Enter:  
            OnEnterPressed(); 
            break;
            
        }
          break;
      }

      base.OnKeyDown( e );
    }
    #endregion

    #region Control Creation
    protected override void WndProc(ref System.Windows.Forms.Message m)
    {
      base.WndProc( ref m );

      if( m.Msg == (int)Msg.WM_GETDLGCODE )
      {
        m.Result = new IntPtr( (int)DialogCodes.DLGC_WANTCHARS | 
          (int)DialogCodes.DLGC_WANTARROWS | 
          (int)DialogCodes.DLGC_WANTTAB |
          m.Result.ToInt32() );
      }
    }

    protected override System.Windows.Forms.CreateParams CreateParams
    {
      get
      {
        CreateParams param = base.CreateParams;
        param.ExStyle |= (int)WindowExStyles.WS_EX_CONTROLPARENT;
        return param;
      }
    }
    #endregion
  }
}
