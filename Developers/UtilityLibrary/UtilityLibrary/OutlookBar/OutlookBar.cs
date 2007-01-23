using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Drawing.Design;

using UtilityLibrary.Collections;
using UtilityLibrary.Win32;
using UtilityLibrary.General;
using UtilityLibrary.Menus;


namespace UtilityLibrary.WinControls
{
  #region Delegates
  public delegate void OutlookBarPropertyChangedHandler(OutlookBarBand band, OutlookBarProperty property);
  public delegate void OutlookBarItemClickedHandler(OutlookBarBand band, OutlookBarItem item);
  public delegate void OutlookBarItemDroppedHandler(OutlookBarBand band, OutlookBarItem item);
  #endregion

  #region Enumerations
  public enum OutlookBarCheckStyle
  {
    ItemsAsCheckBoxes,
    ItemsAsRadioButtons
  }

  public enum OutlookBarProperty
  {
    CurrentBandChanged,
    ShortcutNameChanged,
    GroupNameChanged
  }

  [Flags]
  enum ItemSizeType
  {
    Icon    = 0x0001,
    Label   = 0x0002,
    All = Icon | Label
  }

  public enum IconView
  {
    Small,
    Large
  }

  public enum HitTestType
  {
    UpScroll,
    DownScroll,
    Header,
    Item,
    DropLine,
    DropLineLastItem,
    Nothing
  }

  public enum ContextMenuType
  {
    Item,
    Header,
    Bar
  }

  public enum BorderType
  {
    None,
    FixedSingle,
    Fixed3D,
    Custom
  }
  #endregion

  [ToolboxBitmap(typeof(UtilityLibrary.WinControls.OutlookBar), "UtilityLibrary.WinControls.OutlookBar.bmp")]
  public class OutlookBar : System.Windows.Forms.Control
  {
    #region Class constants
    private const int BAND_HEADER_HEIGHT = 22;
    private const int X_SMALLICON_LABEL_OFFSET = 2;
    private const int Y_LARGEICON_LABEL_OFFSET = 3;
    private const int Y_SMALLICON_SPACING = 10;
    private const int Y_LARGEICON_SPACING = 8;
    private const int LEFT_MARGIN = 5;
    private const int LARGE_TOP_MARGIN = 10;
    private const int SMALL_TOP_MARGIN = 6;
    private const int ARROW_BUTTON_MARGIN = 5;
    private const int SMALL_ICON_LEFT = 5;
    #endregion

    #region Class variables

    private int   currentBandIndex = -1;
    private int   firstItem;
    private int   selectedHeader = -1;
    private int   animationSpeed = 20;
    private int   lastDrawnLineIndex = -1;
    private int   droppedPosition = -1;
    private int   forceHightlightIndex = -1;
    private int   m_iWheelScroll = 1;

    private bool  upArrowVisible;
    private bool  downArrowVisible;
    private bool  upArrowPressed;
    private bool  downArrowPressed;
    private bool  upTimerTicking;
    private bool  downTimerTicking;
    private bool  doScrollingLoop;
    private bool  buttonPushed;
    private bool  flatArrowButtons;
    private bool  previousPressed;
    private bool  paintedDropLineLastItem;
    private bool  forceHightlight;
    private bool  editingAnItem;
    private bool  m_bSkipEvents;

    private Rectangle   upArrowRect;
    private Rectangle   downArrowRect;
    private BorderType  borderType            = BorderType.None;
    private OutlookBarCheckStyle m_checkStyle = OutlookBarCheckStyle.ItemsAsCheckBoxes;
    private Graphics    m_lastG;

    private DrawState   upFlatArrowState      = DrawState.Normal;
    private DrawState   downFlatArrowState    = DrawState.Normal;
    private int         lastHighlightedHeader = -1;
    private int         lastHighlightedItem   = -1;



    Bitmap      backgroundBitmap = null;
    Color       leftTopColor = Color.Empty;
    Color       rightBottomColor = Color.Empty;
    ContextMenu contextMenu = null;
    Point       lastClickedPoint = Point.Empty;
    Cursor      dragCursor = null;
    TextBoxEx   textBox = new TextBoxEx();

    OutlookBarBandCollection    bands = null;
    System.Windows.Forms.Timer  highlightTimer = new System.Windows.Forms.Timer();
    #endregion

    #region Class Events
    public event OutlookBarPropertyChangedHandler PropertyChanged;
    public event OutlookBarItemClickedHandler ItemClicked;
    public event OutlookBarItemDroppedHandler ItemDropped;

    [ Category( "Property Changed" ) ]
    public event EventHandler BorderTypeChanged;

    [ Category( "Property Changed" ) ]
    public event EventHandler AnimationSpeedChanged;

    [ Category( "Property Changed" ) ]
    public event EventHandler LeftTopColorChanged;

    [ Category( "Property Changed" ) ]
    public event EventHandler RightBottomColorChanged;

    [ Category( "Property Changed" ) ]
    public event EventHandler BackgroundBitmapChanged;

    [ Category( "Property Changed" ) ]
    public event EventHandler FlatArrowButtonsChanged;

    [ Category( "Property Changed" ) ]
    public event EventHandler WheelScrollChanged;

    [ Category( "Property Changed" ) ]
    public event EventHandler CurrentBandChanged;
    #endregion

    #region Class Initialize/Finilize methods
    public OutlookBar()
    {
      // Construct band collection
      bands = new OutlookBarBandCollection();
      bands.Changed += new OutlookBarBandCollection.OutlookBarBandEventHandler( OnBandChanged );
      bands.OnItemAdded += new OutlookBarBandCollection.OutlookBarBandEventHandler( OnBandAdded );
      bands.OnItemRemoved += new OutlookBarBandCollection.OutlookBarBandEventHandler( OnBandRemoved );

      Dock = DockStyle.Left;

      // We are going to do all of the painting so better setup the control
      // to use double buffering
      SetStyle( ControlStyles.AllPaintingInWmPaint |
        ControlStyles.ResizeRedraw |
        //ControlStyles.Opaque |
        ControlStyles.UserPaint |
        ControlStyles.Selectable |
        ControlStyles.DoubleBuffer, true );

      Font = SystemInformation.MenuFont;

      CreateContextMenu();

      // Setup timer
      highlightTimer.Tick += new EventHandler( OnHighlightHeader );
      highlightTimer.Interval = 100;

      // Load drag cursor
      try
      {
        Assembly myAssembly = Assembly.GetAssembly(Type.GetType("UtilityLibrary.WinControls.OutlookBar"));
        Stream cursorStream = myAssembly.GetManifestResourceStream("UtilityLibrary.Resources.DragCursor.cur");
        dragCursor = new Cursor( cursorStream );
      }
      catch
      {
        dragCursor = Cursors.Hand;
      }

      // don't display it until we need to
      textBox.Visible = false;

      // Hook up notification for the Enter and LostFocus events
      textBox.KeyUp += new KeyEventHandler(TextBoxKeyDown);
      textBox.LostFocus += new EventHandler(TextBoxLostFocus);
    }

    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
        if( bands != null )
        {
          bands.Clear();
          bands.Changed -= new OutlookBarBandCollection.OutlookBarBandEventHandler( OnBandChanged );
          bands.OnItemAdded -= new OutlookBarBandCollection.OutlookBarBandEventHandler( OnBandAdded );
          bands.OnItemRemoved -= new OutlookBarBandCollection.OutlookBarBandEventHandler( OnBandRemoved );
        }
      }

      base.Dispose( disposing );
    }
    #endregion

    #region Class Event handlers
    private void OnBandAdded( object sender, OutlookBarBandCollection.OutlookBarBandEventArgs e )
    {
      if( currentBandIndex < 0 )
      {
        currentBandIndex = 0;
      }

      e.Band.Changed += new EventHandler( OnOutlookBarBandChanged );
    }

    private void OnBandRemoved( object sender, OutlookBarBandCollection.OutlookBarBandEventArgs e )
    {
      int index = bands.IndexOf( e.Band );
      e.Band.Changed -= new EventHandler( OnOutlookBarBandChanged );

      if( index == CurrentBand )
      {
        if( index > 0 && bands.Count != 0 )
        {
          CurrentBand = index-1;
        }
      }
    }

    private void OnBandChanged( object sender, OutlookBarBandCollection.OutlookBarBandEventArgs e )
    {
      Invalidate();
    }

    private void OnOutlookBarBandChanged( object sender, EventArgs e )
    {
      int index = bands.IndexOf( (OutlookBarBand)sender );

      if( index == CurrentBand )
      {
        Invalidate();
      }
    }
    #endregion

    #region Class Paint methods
    protected override void OnResize(System.EventArgs e)
    {
      Invalidate();
    }
    protected override void OnPaint(PaintEventArgs pe)
    {
      Graphics g = pe.Graphics;
      m_lastG = g;

      // Manipulate child window in case there is one
      if( currentBandIndex == -1 && bands.Count > 0 )
      {
        SetCurrentBand( 0 );
      }

      // Background if it needs to be painted
      DrawBackground( g );
      DrawBorder(g);

      // The little headers
      DrawHeaders(g);

      // If there is a child window
      // it will paint itself, otherwise we do the painting
      if( !HasChild() )
      {
        // The items themselves
        DrawItems( g, Rectangle.Empty, null );

        // The buttons for scrolling items
        // Drawing second so that they don't get written over by the items
        DrawArrowButtons( g );
      }

      // Highlight last item clicked
      if( forceHightlight )
      {
        ForceHighlightItem( g, forceHightlightIndex );
        forceHightlight = false;
      }
      
    }

    private void DrawBackground( Graphics g )
    {
      Rectangle rc = ClientRectangle;

      // If we don't have any bands, just fill the rectangle
      // with the System Control Color
      if( bands.Count == 0 || currentBandIndex == -1 )
      {
        g.FillRectangle( SystemBrushes.Control, rc );
        return;
      }

      if( backgroundBitmap == null )
      {
        if( currentBandIndex >= 0 && bands[ currentBandIndex ] != null )
        {
          using( SolidBrush b = new SolidBrush( bands[ currentBandIndex ].Background ) )
          {
            // If there is a child control clip the area where the child
            // control will be drawn to avoid flickering
            if( HasChild() )
            {
              Rectangle clipRect = GetViewPortRect();
              g.ExcludeClip( clipRect );
            }

            g.FillRectangle( b, rc );
          }
        }
      }
      else
      {
        //g.FillRectangle( Brushes.White, rc );
        g.DrawImage( backgroundBitmap, rc );

        #region Old code Not Used now
        /*
        // Draw custom background bitmap
        // -- I don't know why but the bitmap is not painted properly if using the right size
        // I use the WindowsAPI to work around the problem
        GDIUtil.StrechBitmap( g, rc, backgroundBitmap );

        if( needBackgroundBitmapResize )
        {
          needBackgroundBitmapResize = false;
          backgroundBitmap = GDIUtil.GetStrechedBitmap( g, rc, backgroundBitmap );
        }
        */
        #endregion
      }
    }

    private void DrawBorder( Graphics g )
    {
      Rectangle rc = ClientRectangle;

      if( borderType == BorderType.FixedSingle )
      {
        g.DrawRectangle( Pens.Black, rc.Left, rc.Top, rc.Width-1, rc.Height-1 );
      }
      else if( borderType == BorderType.Fixed3D )
      {
        ControlPaint.DrawBorder3D( g, rc, Border3DStyle.Sunken );
      }
      else if( borderType == BorderType.Custom )
      {
        Pen p1 = new Pen( leftTopColor );
        Pen p2 = new Pen( RightBottomColor );

        g.DrawRectangle( p1, rc.Left, rc.Top, rc.Width-1, rc.Height-1 );
        g.DrawRectangle( p2, rc.Left+1, rc.Top+1, rc.Width-3, rc.Height-3 );

        p1.Dispose();
        p2.Dispose();
      }
    }

    private void DrawHeaders(Graphics g)
    {
      Rectangle rc;

      for( int i = 0; i<bands.Count; i++ )
      {
        rc = GetHeaderRect( i );
        DrawHeader( g, i, rc, Border3DStyle.RaisedInner );
      }
    }

    private void DrawHeader( Graphics g, int index, Rectangle rc, Border3DStyle style )
    {
      string bandName = bands[ index ].Text;

      Brush b = ( index == lastHighlightedHeader ) ? 
        ColorUtil.VSNetSelectionBrush : ColorUtil.VSNetControlBrush;
      
      g.FillRectangle( b, rc );

      if( ColorUtil.UsingCustomColor )
      {
        Pen p = ColorUtil.VSNetBorderPen;

        if( style != Border3DStyle.RaisedInner )
        {
          g.FillRectangle( ColorUtil.VSNetSelectionBrush, rc );
        }

        g.DrawRectangle( p, rc.Left, rc.Top, rc.Width-1, rc.Height-1 );
      }
      else
      {
        ControlPaint.DrawBorder3D( g, rc, style );
      }

      StringFormat stringFormat = new StringFormat();
      stringFormat.LineAlignment = StringAlignment.Center;
      stringFormat.Alignment = StringAlignment.Center;
      stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
      stringFormat.Trimming = StringTrimming.EllipsisCharacter;

      g.DrawString( bandName, Font, SystemBrushes.WindowText, rc, stringFormat );
    }

    private void DrawItems(Graphics g, Rectangle targetRect, OutlookBarBand drawBand)
    {
      // If we don't have any bands just return
      if ( bands.Count == 0 ) return;

      Rectangle rc = GetViewPortRect();
      OutlookBarBand band = bands[ currentBandIndex ];

      if( drawBand != null ) band = drawBand;
      Debug.Assert(band != null);

      for ( int i = firstItem; i < band.Items.Count; i++ )
      {
        Rectangle itemRect = GetItemRect( g, band, i, targetRect );

        if( itemRect.Top + itemRect.Height > rc.Bottom )
          break;
        else
          DrawItem( g, i, itemRect, ( i == lastHighlightedItem ), false, targetRect, drawBand );
      }
    }

    private void DrawItem( Graphics g, int index, Rectangle itemRect, bool hot,
      bool pressed, Rectangle targetRect, OutlookBarBand drawingBand )
    {
      OutlookBarBand band = bands[ currentBandIndex ];
      if( drawingBand != null ) band = drawingBand;

      Point pt = new Point(0,0);

      // Set clip region so that we don't draw outside the viewport
      Rectangle viewPortRect = GetViewPortRect();

      if( targetRect != Rectangle.Empty ) viewPortRect = targetRect;

      Color textColor = band.TextColor;
      Color backColor = band.Background;
      Color highLight = band.Background;

      if( ColorUtil.UsingCustomColor )
      {
        backColor = ColorUtil.VSNetControlColor;
        highLight = ColorUtil.VSNetControlColor;
      }

      if( hot || band.Items[ index ].Checked )
      {
        backColor = ColorUtil.VSNetCheckedColor;
        highLight = ColorUtil.VSNetBorderColor;
      }

      if( pressed ) backColor = ColorUtil.VSNetPressedColor;

      ImageList imgList = band.Items[ index ].ImageList;
      Size imageSize = (imgList == null) ? new Size(48,48) : imgList.ImageSize;

      if( band.IconView == IconView.Large && band.LargeImageList != null )
        pt.X = itemRect.Left + ( viewPortRect.Width - imageSize.Width ) / 2;
      else if ( band.IconView == IconView.Small && band.SmallImageList != null )
        pt.X = itemRect.Left + SMALL_ICON_LEFT;
      pt.Y = itemRect.Top;

      Rectangle iconRect = new Rectangle( pt, imageSize );

      using( Brush b = new SolidBrush( backColor ) )
      {
        iconRect.Inflate( 2, 2 );

        if( backgroundBitmap == null && ( hot || band.Items[ index ].Checked ) )
        {
          g.FillRectangle( b, iconRect.Left, iconRect.Top, iconRect.Width, iconRect.Height );
        }
        else
        {
          g.FillRectangle( b, iconRect.Left, iconRect.Top, iconRect.Width, iconRect.Height );
        }

        using ( Pen p = new Pen( highLight ) )
        {
          if( backgroundBitmap == null || ( hot || band.Items[ index ].Checked ) )
          {
            g.DrawRectangle( p, iconRect.Left, iconRect.Top, iconRect.Width-1, iconRect.Height-1 );
          }
        }
      }

      int imgIndex = band.Items[ index ].ImageIndex;
      if( imgIndex != -1 && imgList != null && imgIndex < imgList.Images.Count )
      {
        if( hot || band.Items[ index ].Checked ) pt.Offset( -1, -1 );
        g.DrawImage( imgList.Images[ imgIndex ], pt );
      }

      if( band.IconView == IconView.Large && band.LargeImageList != null )
      {
        // Draw the label
        Size textSize = GetLabelSize( g, band, index );
        int top  = itemRect.Top + imageSize.Height + Y_LARGEICON_LABEL_OFFSET;
        int left = itemRect.Left + ( viewPortRect.Width - textSize.Width )/2;

        using( Brush b = new SolidBrush( textColor ) )
        {
          int iHeight = textSize.Height;

          StringFormat format = new StringFormat();
          format.Alignment = StringAlignment.Center;
          format.LineAlignment = StringAlignment.Center;
          format.Trimming = StringTrimming.Word;

          g.DrawString( band.Items[index].Text, Font, b,
            new Rectangle( itemRect.Left, top, viewPortRect.Width, iHeight ),
            format );
        }
      }
      else if ( band.IconView == IconView.Small && band.SmallImageList != null )
      {
        // Draw the label
        Size labelSize = GetLabelSize(g, band, index);
        pt.X = pt.X + imageSize.Width + X_SMALLICON_LABEL_OFFSET;
        pt.Y = itemRect.Top + (itemRect.Height - labelSize.Height)/2;

        using( Brush b = new SolidBrush( textColor ) )
        {
          g.DrawString( band.Items[index].Text, Font, b, pt );
        }
      }
      else // draw if no other drawing we have
      {
        Size labelSize = GetLabelSize(g, band, index);
        
        using( Brush b = new SolidBrush( textColor ) )
        {
          g.DrawString( band.Items[index].Text, Font, b, pt );
        }
      }
    }

    private void DrawArrowButtons( Graphics g )
    {
      // If we don't have any bands just return
      if( bands.Count == 0 ) return;

      int first, last;
      GetVisibleRange( g, out first, out last );

      Rectangle rc = GetViewPortRect();

      upArrowRect = new Rectangle(0, 0, SystemInformation.VerticalScrollBarWidth,
        SystemInformation.VerticalScrollBarWidth);

      downArrowRect = upArrowRect;

      upArrowRect.Offset(rc.Right - ARROW_BUTTON_MARGIN - SystemInformation.VerticalScrollBarWidth,
        rc.Top + ARROW_BUTTON_MARGIN);

      downArrowRect.Offset(rc.Right - ARROW_BUTTON_MARGIN - SystemInformation.VerticalScrollBarWidth,
        rc.Bottom - ARROW_BUTTON_MARGIN - SystemInformation.VerticalScrollBarWidth);

      // Do we need to show top scroll button
      if( first > 0 )
      {
        if( flatArrowButtons )
        {
          DrawFlatArrowButton( g, upArrowRect, true, UpFlatArrowState );
        }
        else
        {
          ControlPaint.DrawScrollButton( g, upArrowRect, ScrollButton.Up, 
            ( upArrowPressed ) ? ButtonState.Pushed : ButtonState.Normal );
        }
        
        upArrowVisible = true;
      }
      else
      {
        upArrowVisible = false;
        UpFlatArrowState = DrawState.Normal;
      }

      // Do we need to show bottom scroll button
      if ( last < bands[ currentBandIndex ].Items.Count - 1 )
      {
        if ( flatArrowButtons )
        {
          DrawFlatArrowButton(g, downArrowRect, false, DownFlatArrowState);
        }
        else
        {
          ControlPaint.DrawScrollButton( g, downArrowRect, ScrollButton.Down, 
            ( downArrowPressed ) ? ButtonState.Pushed : ButtonState.Normal );
        }
        
        downArrowVisible = true;
      }
      else
      {
        downArrowVisible = false;
        DownFlatArrowState = DrawState.Normal;
      }
    }

    private void DrawFlatArrowButton( Graphics g, Rectangle rc, bool up, DrawState state )
    {
      // Dont' draw flat button if there is a band with a child control
      if( HasChild() ) return;

      Brush b = ( state == DrawState.Hot ) ? ColorUtil.VSNetCheckedBrush : ColorUtil.VSNetControlBrush;
      g.FillRectangle( b, rc.Left, rc.Top, rc.Width-1, rc.Height-1 );

      Pen p = ColorUtil.VSNetBorderPen;
      
      if( state == DrawState.Hot )
      {
        g.DrawRectangle( p, rc.Left, rc.Top, rc.Width-2, rc.Height-2 );
      }

      GDIUtils.DrawArrowGlyph( g, rc, 7, 4, up, Brushes.Black);

      // Remember last state of the flat arrow button
      if( up )
      {
        UpFlatArrowState = state;
      }
      else
      {
        DownFlatArrowState = state;
      }
    }

    void DrawBandBitmap(IntPtr hDC, OutlookBarBand band, int bandIndex, Rectangle drawingRect)
    {
      // Don't do GDI+ calls since you cannot mix them with GDI calls
      if( !HasChild( bandIndex ) )
      {
        Color cb = band.Background;
        IntPtr brush = WindowsAPI.CreateSolidBrush(ColorUtil.RGB(cb.R, cb.G, cb.B));

        RECT rect;
        rect.left = drawingRect.Left;
        rect.top = drawingRect.Top;
        rect.right = drawingRect.Left + drawingRect.Width;
        rect.bottom = drawingRect.Top + drawingRect.Height;

        WindowsAPI.FillRect(hDC, ref rect, brush);
        WindowsAPI.DeleteObject(brush);
      }

      if( HasChild( bandIndex ) )
      {
        // Paint child control into memory device context
        Control child = bands[bandIndex].ChildControl;
        bool visible = child.Visible;
        child.Visible = true;

        // Change viewport if needed
        POINT pt = new POINT();
        WindowsAPI.SendMessage( child.Handle, (int)Msg.WM_ERASEBKGND, (int)hDC, 0 );
        WindowsAPI.SendMessage( child.Handle, (int)Msg.WM_PAINT, (int)hDC, 0 );
        if( !visible ) child.Visible = false;
      }
      else
      {
        DrawItems(Graphics.FromHdc(hDC), drawingRect, bands[bandIndex]);
      }
    }

    void DrawDropLine( Graphics g, int index, bool drawLine, HitTestType hit )
    {
      Brush brush;
      Pen pen;
      if ( drawLine )
      {
        droppedPosition  = index;
        lastDrawnLineIndex = index;
        brush = SystemBrushes.ControlText;
        pen = SystemPens.ControlText;
      }
      else
      {
        // If there is nothing painted, no need to erase
        if ( lastDrawnLineIndex == -1) return;

        index = lastDrawnLineIndex;
        brush = new SolidBrush(bands[currentBandIndex].Background);
        pen = new Pen(bands[currentBandIndex].Background);
        lastDrawnLineIndex = -1;
      }

      int itemsCount = bands[currentBandIndex].Items.Count;
      Rectangle itemRect = GetItemRect(g, bands[currentBandIndex], index, Rectangle.Empty);

      Rectangle viewPortRect = GetViewPortRect();
      int y;
      if  ( bands[currentBandIndex].IconView == IconView.Small)
        y = itemRect.Top - Y_SMALLICON_SPACING/2;
      else
        y = itemRect.Top - Y_LARGEICON_SPACING;

      if ( hit == HitTestType.DropLineLastItem )
      {
        // use the bottom of the label if dealing with the last item
        Rectangle labelRect = GetLabelRect(itemsCount-1);
        y = labelRect.Bottom + Y_LARGEICON_SPACING;
        paintedDropLineLastItem = true;
        // the none existing item index
        droppedPosition = itemsCount;

      }
      else if ( paintedDropLineLastItem )
      {
        Rectangle labelRect = GetLabelRect(itemsCount-1);
        y = labelRect.Bottom + Y_LARGEICON_SPACING;
        paintedDropLineLastItem = false;
      }

      // If we are using a bitmap background, erase
      // by painting the portion of the bitmap background
      if ( backgroundBitmap != null && lastDrawnLineIndex == -1 )
      {
        Rectangle rcStrip = new Rectangle(viewPortRect.Left, y-6, viewPortRect.Width, 12);
        g.DrawImage(backgroundBitmap, rcStrip, rcStrip, GraphicsUnit.Pixel);
        return;
      }

      // Draw horizontal line
      Point p1 = new Point(viewPortRect.Left+11, y);
      Point p2 = new Point(viewPortRect.Right-11, y);
      g.DrawLine(pen, p1, p2);

      // Draw left triangle
      Point top;
      Point bottom;
      if ( index == firstItem )
        top = new Point(viewPortRect.Left+5, y);
      else
        top = new Point(viewPortRect.Left+5, y-6);

      if ( hit == HitTestType.DropLineLastItem )
        bottom =  new Point(viewPortRect.Left+5, y+1);
      else
        bottom = new Point(viewPortRect.Left+5, y+6);

      Point middle = new Point(viewPortRect.Left+11, y);
      Point[] points = new Point[3];
      points.SetValue(top, 0);
      points.SetValue(middle, 1);
      points.SetValue(bottom, 2);
      g.FillPolygon(brush, points);

      // Draw right triangle
      if ( index == firstItem )
        top = new Point(viewPortRect.Right-5, y);
      else
        top = new Point(viewPortRect.Right-5, y - 6);

      if ( hit == HitTestType.DropLineLastItem  )
        bottom = new Point(viewPortRect.Right-5, y+1);
      else
        bottom = new Point(viewPortRect.Right-5, y + 6);

      middle = new Point(viewPortRect.Right-11, y);
      points.SetValue(top, 0);
      points.SetValue(middle, 1);
      points.SetValue(bottom, 2);
      g.FillPolygon(brush, points);

      if ( !drawLine )
      {
        brush.Dispose();
        pen.Dispose();
      }

    }

    
    void ForceHighlightItem( Graphics g, int index )
    {
      // Draw this item hightlighed
      Rectangle itemRect = GetItemRect( g, bands[currentBandIndex], index, Rectangle.Empty );
      DrawItem( g, index, itemRect, true, false, Rectangle.Empty, null );
    }
    #endregion

    #region Parent Class Overrides
    protected override Size DefaultSize
    {
      get
      {
        return new Size( 100, 10 );
      }
    }

    protected override void OnMouseDown( MouseEventArgs e )
    {
      base.OnMouseDown(e);

      if( base.Focused == false ) base.Focus();

      // If the textbox is being displayed cancel editing
      if ( textBox.Visible )
      {
        ProcessTextBoxEditing();
        return;
      }

      int index;
      HitTestType ht = HitTest( new Point( e.X, e.Y ), out index, false );

      if( e.Button == MouseButtons.Left)
      {
        // Handle arrow button hit
        if( ht == HitTestType.UpScroll || ht == HitTestType.DownScroll )
        {
          if ( ht == HitTestType.UpScroll )
          {
            ProcessArrowScrolling(upArrowRect,
              ref upArrowVisible, ref upArrowPressed, ref upTimerTicking);
            return;
          }
          else
          {
            ProcessArrowScrolling(downArrowRect,
              ref downArrowVisible, ref downArrowPressed, ref downTimerTicking);
            return;
          }
        }

        // Handle header hit
        if ( ht == HitTestType.Header )
        {
          ProcessHeaderHit(index);
          return;
        }

        // Handle item hit
        if ( ht == HitTestType.Item )
        {
          ProcessItemHit(index, new Point(e.X, e.Y));
          return;
        }
      }
    }

    private void PartialRedraw( int Header_, int Item_ )  //New States
    {
      m_lastG = Graphics.FromHwnd( Handle );

      if ( lastHighlightedHeader != Header_ ) 
      {
        Rectangle rc;
        int oldH = lastHighlightedHeader;

        lastHighlightedHeader = Header_;

        if ( oldH != -1 ) 
        {
          rc = GetHeaderRect( oldH );
          DrawHeader( m_lastG, oldH, rc, Border3DStyle.RaisedInner );
        }

        if ( Header_ != -1 ) 
        {
          rc = GetHeaderRect( Header_ );
          DrawHeader( m_lastG, Header_, rc, Border3DStyle.RaisedInner );
        }
      }

      if ( lastHighlightedItem != Item_ ) 
      {
        // If we don't have any bands just return
        if ( bands.Count == 0 ) return;

        Rectangle rc = GetViewPortRect();
        OutlookBarBand band = bands[ currentBandIndex ];
        Debug.Assert(band != null);
        Rectangle itemRect;
        int OldI = lastHighlightedItem;
        lastHighlightedItem = Item_;

        if ( OldI != -1 )
        {
          itemRect = GetItemRect( m_lastG, band, OldI, Rectangle.Empty );
          DrawItem( m_lastG, OldI, itemRect, false, false, Rectangle.Empty, band );
        }


        if ( Item_ != -1 )
        {
          itemRect = GetItemRect( m_lastG, band, Item_, Rectangle.Empty );
          DrawItem( m_lastG, Item_, itemRect, true, false, Rectangle.Empty, band );
        }
      }
    }

    protected override void OnMouseMove( MouseEventArgs e )
    {
      base.OnMouseMove(e);
      Point MousePos = new Point( e.X, e.Y );

      // Change cursor if over a header
      int index;
      HitTestType ht = HitTest( MousePos, out index, false );
    
      if( ht == HitTestType.UpScroll && flatArrowButtons && upFlatArrowState != DrawState.Hot )
      {
        upFlatArrowState = DrawState.Hot;
        downFlatArrowState = DrawState.Normal;
        
        lastHighlightedHeader = -1;
        lastHighlightedItem = -1;

        Invalidate();
      }
      else if( ht == HitTestType.DownScroll && flatArrowButtons && downFlatArrowState != DrawState.Hot )
      {
        downFlatArrowState = DrawState.Hot;
        upFlatArrowState = DrawState.Normal;
        lastHighlightedHeader = -1;
        lastHighlightedItem = -1;
        
        Invalidate();
      }
      else if( ht == HitTestType.Header && lastHighlightedHeader != index )
      {
        Cursor.Current = Cursors.Hand;
        
        upFlatArrowState = DrawState.Normal;
        downFlatArrowState = DrawState.Normal;
        PartialRedraw( index, -1 );
      }
      else if( ht == HitTestType.Item && lastHighlightedItem != index )
      {
        upFlatArrowState = DrawState.Normal;
        downFlatArrowState = DrawState.Normal;
        PartialRedraw( -1, index );
      }
      else if( ht == HitTestType.Nothing )
      {
        PartialRedraw( -1, -1 );
      } 
    }

    protected override void OnSizeChanged( EventArgs e )
    {
      base.OnSizeChanged(e);
      if ( HasChild() )
      {
        Rectangle rc = GetViewPortRect();
        OutlookBarBand newBand = bands[currentBandIndex];
        Control childControl = newBand.ChildControl;
        childControl.Bounds = rc;
      }

      // flag that we need to update the bitmap background
      // if there is one
      //needBackgroundBitmapResize = true;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
      base.OnHandleCreated(e);
      
      // Make text box a child of the outlookbar control
      WindowsAPI.SetParent( textBox.Handle, Handle );
    }

    protected override void OnMouseWheel( MouseEventArgs e )
    {
      base.OnMouseWheel( e );

      int itemCount = bands[currentBandIndex].Items.Count;
      firstItem += e.Delta;

      if( firstItem < 0 ) firstItem = 0;
      if( firstItem > itemCount ) firstItem = itemCount-1;

      Rectangle rc = GetViewPortRect();
      Invalidate(rc);
    }

    protected override void OnGotFocus( EventArgs e )
    {
      //m_bFocused = true;
    }

    protected override void OnLostFocus( EventArgs e )
    {
      //m_bFocused = false;
    }

    protected override void WndProc( ref Message m )
    {
      bool callBase = true;

      switch( m.Msg )
      {
        case (int)Msg.WM_CONTEXTMENU:
        {
          Point pt = WindowsAPI.GetPointFromLPARAM((int)m.LParam);
          pt = PointToClient(pt);
          Rectangle viewPortRect = GetViewPortRect();

          // Save the point so that we can update our popup menu correctly
          lastClickedPoint = pt;

          // If the user click on the child window don't
          // show a context menu
          if ( HasChild() && viewPortRect.Contains(pt))
          {
            callBase = false;
            break;
          }
        }
          break;

        case (int)Msg.WM_MOUSEWHEEL:
          short delta = (short)(m.WParam.ToInt32() >> 16);
          
          OnMouseWheel( new MouseEventArgs( MouseButtons.None, 0, MousePosition.X,
            MousePosition.Y, ( delta < 0 ) ? m_iWheelScroll : -m_iWheelScroll ) );
          
          callBase = false;
          break;

        default:
          break;
      }

      if( callBase == true ) base.WndProc(ref m);
    }

    protected override void OnMouseLeave(System.EventArgs e)
    {
      bool bRefresh = false;

      bRefresh = ( upFlatArrowState != DrawState.Normal || 
        downFlatArrowState != DrawState.Normal || lastHighlightedItem != -1 ||
        lastHighlightedHeader != -1 );

      upFlatArrowState = DrawState.Normal;
      downFlatArrowState = DrawState.Normal;
      lastHighlightedItem = -1;
      lastHighlightedHeader = -1;
      
      if( bRefresh == true ) Invalidate();
    }
    #endregion

    #region Class Properties
    public OutlookBarCheckStyle ItemsCheckStyle 
    {
      get
      {
        return m_checkStyle;
      }
      set
      {
        if ( value != m_checkStyle ) 
        {
          m_checkStyle = value;
        }
      }
    }
    
    [ DesignerSerializationVisibility( DesignerSerializationVisibility.Content ) ]
    [ Category( "Behavior" ) ]
    public OutlookBarBandCollection Bands
    {
      get { return bands; }
    }

    [ Category( "Appearance" ), DefaultValue( typeof( BorderType ), "None" ), Description( "Border type of control" ) ]
    public BorderType BorderType
    {
      get
      {
        return borderType;
      }
      set
      {
        if( value != borderType )
        {
          borderType = value;
          OnBorderTypeChanged();
        }
      }
    }

    [ Category( "Behaviour" ), DefaultValue( 20 ), Description( "How fast control will switch bands" ) ]
    public int AnimationSpeed
    {
      get                                   
      {
        return animationSpeed;
      }
      set
      {
        if( value != animationSpeed )
        {
          animationSpeed = value;
          OnAnimationSpeedChanged();
        }
      }
    }

    [ Category( "Appearance" ), DefaultValue( typeof( Color ), "Empty" ), Description( "Color of Left Top corner of band" ) ]
    public Color LeftTopColor
    {
      get
      {
        return leftTopColor;
      }
      set
      {
        if( value != leftTopColor )
        {
          leftTopColor = value;
          OnLeftTopColorChanged();
        }
      }
    }

    [ Category( "Appearance" ), DefaultValue( typeof( Color ), "Empty" ), Description( "Color of Bottom Right corner of band" ) ]
    public Color RightBottomColor
    {
      get
      {
        return rightBottomColor;
      }
      set
      {
        if( value != rightBottomColor )
        {
          rightBottomColor = value;
          OnRightBottomColorChanged();
        }
      }
    }

    [ Category( "Appearance" ), DefaultValue( null ), Description( "Background bitmap of control" ) ]
    public Bitmap BackgroundBitmap
    {
      get
      {
        return backgroundBitmap;
      }
      set
      {
        if( value != backgroundBitmap )
        {
          backgroundBitmap = value;
          OnBackgroundBitmapChanged();
        }
      }
    }

    [ Category( "Appearance" ), DefaultValue( false ) ]
    public bool FlatArrowButtons
    {
      get
      {
        return flatArrowButtons;
      }
      set
      {
        if( value != flatArrowButtons )
        {
          flatArrowButtons = value;
          OnFlatArrowButtonsChanged();
        }
      }
    }

    [ Category("Behavior"), DefaultValue(1) ]
    public int WheelScroll
    {
      get
      {
        return m_iWheelScroll;
      }
      set
      {
        if( value != m_iWheelScroll )
        {
          m_iWheelScroll = value;
          OnWheelScrollChanged();
        }
      }
    }

    [ Browsable( false ), Description( "GET/SET currunt index of band which must be shown by control" ) ]
    public int CurrentBand
    {
      get
      {
        return currentBandIndex;
      }
      set
      {
        if( value != currentBandIndex )
        {
          currentBandIndex = value;
          SetCurrentBand( value );
          OnCurrentBandChanged();
        }
      }
    }

    [ Browsable( false ), Description( "GET/SET does control throw any event or not" ) ]
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


    internal DrawState UpFlatArrowState
    {
      set { upFlatArrowState = value; }
      get { return upFlatArrowState; }
    }

    internal DrawState DownFlatArrowState
    {
      set { downFlatArrowState = value; }
      get { return downFlatArrowState; }
    }
    #endregion

    #region Event Raisers
    protected void RaiseBorderTypeChangedEvent()
    {
      if( BorderTypeChanged != null && QuietMode == false )
      {
        BorderTypeChanged( this, EventArgs.Empty );
      }
    }

    protected void RaiseAnimationSpeedChangedEvent()
    {
      if( AnimationSpeedChanged != null && QuietMode == false )
      {
        AnimationSpeedChanged( this, EventArgs.Empty );
      }
    }

    protected void RaiseLeftTopColorChangedEvent()
    {
      if( LeftTopColorChanged != null && QuietMode == false )
      {
        LeftTopColorChanged( this, EventArgs.Empty );
      }
    }

    protected void RaiseRightBottomColorChangedEvent()
    {
      if( RightBottomColorChanged != null && QuietMode == false )
      {
        RightBottomColorChanged( this, EventArgs.Empty );
      }
    }

    protected void RaiseBackgroundBitmapChangedEvent()
    {
      if( BackgroundBitmapChanged != null && QuietMode == false )
      {
        BackgroundBitmapChanged( this, EventArgs.Empty );
      }
    }

    protected void RaiseFlatArrowButtonsChangedEvent()
    {
      if( FlatArrowButtonsChanged != null && QuietMode == false )
      {
        FlatArrowButtonsChanged( this, EventArgs.Empty );
      }
    }

    protected void RaiseWheelScrollChangedEvent()
    {
      if( WheelScrollChanged != null && QuietMode == false )
      {
        WheelScrollChanged( this, EventArgs.Empty );
      }
    }

    protected void RaiseCurrentBandChangedEvent()
    {
      if( CurrentBandChanged != null && QuietMode == false )
      {
        CurrentBandChanged( this, EventArgs.Empty );
      }
    }


    void FirePropertyChanged( OutlookBarProperty property )
    {
      if ( PropertyChanged != null )
      {
        PropertyChanged( bands[currentBandIndex], property );
      }
    }

    void FireItemClicked( int index )
    {
      if( ItemClicked != null )
      {
        if( currentBandIndex == -1 ) SetCurrentBand(0);

        if ( index >= 0 && index < bands[currentBandIndex].Items.Count )
        {
          ItemClicked( bands[ currentBandIndex ], bands[ currentBandIndex ].Items[ index ] );
        }
      }
    }

    void FireItemDropped( int index )
    {
      if( ItemDropped != null )
      {
        if( currentBandIndex == -1 ) SetCurrentBand(0);

        if( index >= 0 && index < bands[ currentBandIndex ].Items.Count )
        {
          ItemDropped( bands[ currentBandIndex ], bands[ currentBandIndex ].Items[ index ] );
        }
      }
    }

    #endregion

    #region Class overrides OnChange methods
    protected virtual void OnBorderTypeChanged()
    {
      Invalidate();
      RaiseBorderTypeChangedEvent();
    }

    protected virtual void OnAnimationSpeedChanged()
    {
      RaiseAnimationSpeedChangedEvent();
    }

    protected virtual void OnLeftTopColorChanged()
    {
      if( this.BorderType == BorderType.Custom )
      {
        Invalidate();
      }

      RaiseLeftTopColorChangedEvent();
    }

    protected virtual void OnRightBottomColorChanged()
    {
      if( this.BorderType == BorderType.Custom )
      {
        Invalidate();
      }

      RaiseRightBottomColorChangedEvent();
    }

    protected virtual void OnBackgroundBitmapChanged()
    {
      Invalidate();
      RaiseBackgroundBitmapChangedEvent();
    }

    protected virtual void OnFlatArrowButtonsChanged()
    {
      Invalidate();
      RaiseFlatArrowButtonsChangedEvent();
    }

    protected virtual void OnWheelScrollChanged()
    {
      RaiseWheelScrollChangedEvent();
    }

    protected virtual void OnCurrentBandChanged()
    {
      RaiseCurrentBandChangedEvent();
    }
    protected virtual void OnQuietModeChanged()
    {
      // for inheritors
    }
    #endregion

    #region Methods
    public HitTestType HitTest( Point point, out int index, bool doingDragging )
    {
      // If we don't have any bands just return
      index = 0;
      if ( bands.Count == 0 || currentBandIndex == -1 ) return HitTestType.Nothing;

      // Check if we hit the arrow buttons
      if( upArrowVisible && upArrowRect.Contains( point ) ) return HitTestType.UpScroll;
      if( downArrowVisible && downArrowRect.Contains( point ) ) return HitTestType.DownScroll;

      // Check to see if we hit a header
      for( int i = 0; i < bands.Count; i++ )
      {
        Rectangle rc = GetHeaderRect(i);
        if ( rc.Contains(point) )
        {
          index = i;
          return HitTestType.Header;
        }
      }

      // Don't do any hit testing for items if
      // the current band has a child
      if ( HasChild() ) return HitTestType.Nothing;

      // Check to see if we hit an item
      int itemCount = bands[currentBandIndex].Items.Count;
      Rectangle viewPortRect = GetViewPortRect();
      Rectangle iconRect = Rectangle.Empty;
      Rectangle labelRect = Rectangle.Empty;

      for( int i = firstItem; i < itemCount; i++ )
      {
        // Check if we hit the icon
        iconRect = GetIconRect( i );
        
        if( iconRect.Contains( point ) )
        {
          index = i;
          return HitTestType.Item;
        }
        else
          if( iconRect.Bottom > viewPortRect.Bottom && !doingDragging ) break;

        // Check to see if we hit the label
        labelRect = GetLabelRect( i );
        
        if( labelRect.Contains(point) )
        {
          index = i;
          return HitTestType.Item;
        }
        else
          if ( labelRect.Bottom > viewPortRect.Bottom && !doingDragging) break;

        // If we are dragging, hit test for the drop line
        if ( doingDragging )
        {
          Rectangle dragRect = new Rectangle( viewPortRect.Left, 
            iconRect.Top - Y_LARGEICON_SPACING,
            viewPortRect.Width, Y_LARGEICON_SPACING );

          if( dragRect.Contains( point ) )
          {
            index = i;
            return HitTestType.DropLine;
          }

          // if this is the last icon and the point is farther down the last item
          if ( (i == itemCount - 1) && point.Y > labelRect.Bottom )
          {
            index = itemCount - 1;
            return HitTestType.DropLineLastItem;
          }
        }
      }

      return HitTestType.Nothing;
    }

    public Rectangle GetItemRect( Graphics g, OutlookBarBand band, int index, Rectangle targetRect )
    {
      Rectangle rc = GetViewPortRect();
      if ( targetRect != Rectangle.Empty ) rc = targetRect;
      Size itemSize = new Size(0,0);
      int top = rc.Top;
      int y = 0;

      for( int i = 0; i < index; i++ )
      {
        itemSize = GetItemSize( g, band, i, ItemSizeType.All );
        top += itemSize.Height;
        top += ( band.IconView == IconView.Small ) ? Y_SMALLICON_SPACING : Y_LARGEICON_SPACING;
        
        if( i == ( firstItem - 1 ) )
        {
          // Subtract the hidden items height
          y = top - rc.Top;
        }
      }

      itemSize = GetItemSize( g, band, index, ItemSizeType.All );
      int margin = ( band.IconView == IconView.Large ) ? LARGE_TOP_MARGIN : SMALL_TOP_MARGIN;

      // Work with Windows Rect is easier to change settings
      RECT rcItem = new RECT();
      rcItem.left = rc.Left;
      rcItem.top = top;
      rcItem.right = rc.Left + itemSize.Width;
      rcItem.bottom = top + itemSize.Height;

      // Adjust rectangle
      rcItem.top -= y;
      rcItem.bottom -= y;
      rcItem.top += margin;
      rcItem.bottom += margin;

      if( band.IconView == IconView.Small )
      {
        rcItem.left  = rc.Left + LEFT_MARGIN;
        rcItem.right = rc.Right;
      }

      // Construct final rectangle
      Rectangle actualRect = new Rectangle( rcItem.left,
        rcItem.top, rcItem.right - rcItem.left, rcItem.bottom - rcItem.top);

      return actualRect;
    }

    public OutlookBarItem GetItemAt ( int x, int y )
    {
      int itemCount = bands[ currentBandIndex ].Items.Count;
      Rectangle itemRect = Rectangle.Empty;
      Point point = new Point(x, y);

      for( int i = firstItem; i < itemCount; i++ )
      {
        itemRect = GetIconRect( i );

        if( itemRect.Contains( point ) )
        {
          return bands[ currentBandIndex ].Items[ i ];
        };

        itemRect = GetLabelRect( i );

        if( itemRect.Contains( point ) )
        {
          return bands[ currentBandIndex ].Items[ i ];
        };
      }

      return null;
    }

    public void ProcessOnMouseDown(MouseEventArgs e)
    {
      // To be used from the designer
      OnMouseDown(e);
    }

    public void OnContextMenu(object sender, EventArgs e)
    {
      OutlookBarBand band = bands[currentBandIndex];

      if (typeof(MenuItemEx).IsInstanceOfType(sender))
      {
        MenuItemEx item = (MenuItemEx)sender;
        if ( item.Text == "Large Icons")
        {
          band.IconView = IconView.Large;
          item.RadioCheck = true;
          contextMenu.MenuItems[1].RadioCheck = false;
        }
        else if ( item.Text == "Small Icons")
        {
          item.RadioCheck = true;
          band.IconView = IconView.Small;
          contextMenu.MenuItems[0].RadioCheck = false;
        }
        else if ( item.Text == "Rename Shortcut" )
        {
          RenameItem();
        }
        else if ( item.Text == "Rename Group" )
        {
          RenameHeader();
        }
      }

      Invalidate();
    }

    private void SetCurrentBand( int index )
    {
      // If in design mode and index equals -1 just don't do anything
      if( DesignMode && index == -1 ) return;

      // Make sure index is valid
      Debug.Assert(index >= 0 && index < bands.Count, "Invalid Band Index");

      // Don't do anything if requested to set it to the same one
      if ( currentBandIndex == index) return;

      // Hide current child control if any
      Control childControl;

      if( currentBandIndex != -1 && bands.Count > 0)
      {
        OutlookBarBand oldBand = bands[ currentBandIndex ];
        childControl = oldBand.ChildControl;
        if( childControl != null ) childControl.Visible = false;
      }

      // Animate showing the new band
      if( index != currentBandIndex )
      {
        AnimateScroll( currentBandIndex, index );
      }

      // Reset parameter
      currentBandIndex = index;
      firstItem = 0;
      lastHighlightedItem = -1;

      OutlookBarBand newBand = bands[currentBandIndex];
      childControl = newBand.ChildControl;

      if( childControl != null )
      {
        // Make the outlookbar the parent of the child control
        // so that when we change the bounds of the control they
        // would be relative to the parent control
        // Don't do this every time since it causes flickering
        IntPtr hParent = WindowsAPI.GetParent( childControl.Handle );

        if( hParent != Handle )
        {
          WindowsAPI.SetParent(childControl.Handle, Handle);

          // Hook up into control recreation
          childControl.HandleCreated += new EventHandler(HandleRecreated);
        }

        Rectangle rc = GetViewPortRect();
        childControl.Bounds = rc;
        childControl.Visible = true;
        childControl.Focus();
      }

      // update the bar
      Invalidate();

      // Fire property changed
      FirePropertyChanged( OutlookBarProperty.CurrentBandChanged );
    }

    public void BeginUpdate()
    {
    }

    public void EndUpdate()
    {
      Rectangle rc = GetViewPortRect();
      Invalidate( rc );
    }

    public void ScrollUp()
    {
      ProcessArrowScrolling( upArrowRect,
        ref upArrowVisible,
        ref upArrowPressed,
        ref upTimerTicking );
    }

    public void ScrollDown()
    {
      ProcessArrowScrolling( downArrowRect,
        ref downArrowVisible,
        ref downArrowPressed,
        ref downTimerTicking );
    }
    #endregion

    #region Class Sizes calculations
    void GetVisibleRange(Graphics g, out int first, out int last)
    {
      first = firstItem;
      last = 0;

      /// added by ALEXK: check of currentBandIndex value
      if( currentBandIndex < 0 && bands.Count > 0 ) currentBandIndex = 0;

      OutlookBarBand band = bands[currentBandIndex];
      Rectangle rc = GetViewPortRect();
      Rectangle itemRect;
      for ( int i = firstItem; i < band.Items.Count; i++ )
      {
        itemRect = GetItemRect(g, band, i, Rectangle.Empty);
        if ( itemRect.Bottom > rc.Bottom )
        {
          last = i-1;
          break;
        }
        else
        {
          last = i;
        }
      }
    }

    Size GetItemSize( Graphics g, OutlookBarBand band, int itemIndex, ItemSizeType itemSizeType )
    {
      Size iconSize = new Size(0,0);
      Size labelSize = new Size(0,0);

      if( itemSizeType == ItemSizeType.Icon || itemSizeType == ItemSizeType.All )
      {
        iconSize = GetIconSize( band );
        if( itemSizeType == ItemSizeType.Icon ) return iconSize;
      }

      if ( itemSizeType == ItemSizeType.Label || itemSizeType == ItemSizeType.All )
      {
        labelSize = GetLabelSize( g, band, itemIndex );
        if ( itemSizeType == ItemSizeType.Label ) return labelSize;
      }

      if ( itemSizeType == ItemSizeType.All )
      {
        if ( band.IconView == IconView.Small )
        {
          return new Size(iconSize.Width + labelSize.Width + X_SMALLICON_LABEL_OFFSET,
            iconSize.Height > labelSize.Height?iconSize.Height:labelSize.Height);
        }
        else
        {
          return new Size( ( iconSize.Width > labelSize.Width ) ? iconSize.Width : labelSize.Width,
            iconSize.Height + labelSize.Height + Y_LARGEICON_LABEL_OFFSET + Y_LARGEICON_SPACING );
        }
      }

      return new Size(0,0);
    }

    Size GetIconSize(OutlookBarBand band)
    {

      if ( band.IconView == IconView.Large && band.LargeImageList != null )
        return band.LargeImageList.ImageSize;
      else if ( band.IconView == IconView.Small && band.SmallImageList != null )
        return band.SmallImageList.ImageSize;
      return new Size(0,0);
    }

    Size GetLabelSize(Graphics g, OutlookBarBand band, int itemIndex)
    {
      Size textSize = new Size(0,0);
      
      if( band.IconView == IconView.Large )
      {
        // Calculate text rectangle including word breaks if needed
        Rectangle rect = GetViewPortRect();
        Rectangle textRect = Rectangle.FromLTRB( rect.Left, rect.Top, rect.Width, rect.Top );

        // The TextUtil function is going to call GDI, but the Graphics object
        // is already being used by GDI+. Pass a null reference so that the TextUtil
        // function uses the Screen Device to calculate the text size
        if ( band.Items[itemIndex].Text != null )
        {
          StringFormat format = new StringFormat();
          format.LineAlignment = StringAlignment.Center;
          format.Alignment = StringAlignment.Center;
          format.Trimming = StringTrimming.Word;

          SizeF size = g.MeasureString( band.Items[itemIndex].Text, Font, rect.Width, format );
          
          textSize = new Size( (int)(size.Width + 0.5), (int)(size.Height + 0.5) );

          /*
          textSize = TextUtil.GetTextSize( null,
            band.Items[itemIndex].Text,
            Font, ref textRect, DrawTextFormatFlags.DT_CALCRECT |
            DrawTextFormatFlags.DT_CENTER |
            DrawTextFormatFlags.DT_WORDBREAK );
          //*/
        }

        return textSize;
      }
      else
      {
        // Same as above
        // Calculate text rectangle single line
        if ( band.Items[ itemIndex ].Text != null )
        {
          textSize = TextUtil.GetTextSize( null, band.Items[ itemIndex ].Text, Font );
        }

        return textSize;
      }
    }

    Rectangle GetIconRect(int index)
    {
      Rectangle viewPortRect = GetViewPortRect();
      OutlookBarBand band = bands[currentBandIndex];
      Debug.Assert(band != null);
      Size imageSize = Size.Empty;
      Rectangle itemRect = Rectangle.Empty;
      Point pt = new Point(0,0);

      using ( Graphics g = Graphics.FromHwnd(Handle) )
      {
        itemRect = GetItemRect(g, band, index, Rectangle.Empty);
      }

      if (  band.IconView == IconView.Small )
      {
        if ( band.SmallImageList != null )
        {
          imageSize = band.SmallImageList.ImageSize;
          pt.X = itemRect.Left;
          pt.Y = itemRect.Top + (itemRect.Height - imageSize.Height)/2;
        }
        return new Rectangle(pt, imageSize);
      }
      else
      {
        if ( band.LargeImageList != null )
        {
          imageSize = band.LargeImageList.ImageSize;
          pt.X = itemRect.Left + (viewPortRect.Width - imageSize.Width) / 2;
          pt.Y = itemRect.Top;
        }
        return new Rectangle(pt, imageSize);
      }
    }

    Rectangle GetLabelRect(int index)
    {
      Rectangle viewPortRect = GetViewPortRect();
      OutlookBarBand band = bands[currentBandIndex];
      Debug.Assert(band != null);
      Size imageSize = Size.Empty;
      Rectangle itemRect = Rectangle.Empty;
      Size labelSize = Size.Empty;
      Point pt = new Point(0,0);

      using ( Graphics g = Graphics.FromHwnd(Handle) )
      {
        itemRect = GetItemRect(g, band, index, Rectangle.Empty);
        labelSize = GetLabelSize(g, band, index);
      }

      if (  band.IconView == IconView.Small )
      {
        if ( band.SmallImageList != null )
        {
          imageSize = band.SmallImageList.ImageSize;
          // Don't include the offset between the icon and the label
          // so that we don't leave a miss hit gap
          pt.X = itemRect.Left + imageSize.Width;
          pt.Y = itemRect.Top + (itemRect.Height - labelSize.Height)/2;
        }
        return new Rectangle(pt.X, pt.Y  , labelSize.Width + X_SMALLICON_LABEL_OFFSET+2 , labelSize.Height);
      }
      else
      {
        if ( band.LargeImageList != null )
        {
          imageSize = band.LargeImageList.ImageSize;
          pt.X = itemRect.Left + (viewPortRect.Width - labelSize.Width) / 2;
          // Don't include the offset between the icon and the label
          // so that we don't leave a miss hit gap
          pt.Y = itemRect.Top + imageSize.Height;
        }
        return new Rectangle(pt.X, pt.Y, labelSize.Width, labelSize.Height+ Y_LARGEICON_LABEL_OFFSET+2);
      }
    }

    #endregion

    #region Implementation
    void ProcessArrowScrolling(Rectangle arrowButton, ref bool arrowVisible, ref bool arrowPressed, ref bool timerTicking)
    {
      // Capture the mouse
      Capture = true;

      // Draw the arrow button pushed
      timerTicking = true;

      // Draw pushed button
      buttonPushed = true;

      //DrawArrowButton( arrowButton, ButtonState.Pushed );

      // Start timer
      System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
      timer.Tick += new EventHandler( ScrollingTick );
      timer.Interval = 300;
      timer.Start();

      doScrollingLoop = true;

      while( doScrollingLoop )
      {
        // Check messages until we find a condition to break out of the loop
        Win32.MSG msg = new Win32.MSG();
        WindowsAPI.GetMessage(ref msg, 0, 0, 0);
        Point point = new Point(0,0);

        if( msg.message == (int)Msg.WM_MOUSEMOVE || msg.message == (int)Msg.WM_LBUTTONUP )
        {
          point = WindowsAPI.GetPointFromLPARAM((int)msg.lParam);
        }

        switch( msg.message )
        {
          case (int)Msg.WM_MOUSEMOVE:
          {
            if ( arrowButton.Contains(point) )
            {
              if ( !buttonPushed )
              {
                //DrawArrowButton(arrowButton, ButtonState.Pushed);
                arrowVisible =  true;
                arrowPressed = true;
                buttonPushed =  true;
              }
            }
            else
            {
              if ( buttonPushed )
              {
                //DrawArrowButton(arrowButton, ButtonState.Normal);
                arrowVisible =  false;
                arrowPressed = false;
                buttonPushed =  false;
              }
            }
            break;
          }
          case (int)Msg.WM_LBUTTONUP:
          {
            if ( buttonPushed )
            {
              if ( !flatArrowButtons )
              {
                // Only if using regular buttons
                //DrawArrowButton(arrowButton, ButtonState.Normal);
              }
              buttonPushed =  false;
            }
            arrowVisible = false;
            if ( arrowButton.Contains(point) )
            {
              if ( arrowButton.Equals(upArrowRect) )
              {
                if ( firstItem > 0 )
                {
                  firstItem--;
                  Rectangle rc = GetViewPortRect();
                  Invalidate(rc);
                }
              }
              else if ( arrowButton.Equals(downArrowRect) )
              {
                using ( Graphics g = Graphics.FromHwnd(Handle) )
                {
                  // Get the last item rectangle
                  OutlookBarBand band = bands[currentBandIndex];
                  if ( band != null )
                  {
                    Rectangle rcItem = GetItemRect(g, band, band.Items.Count - 1, Rectangle.Empty);
                    Rectangle rc = GetViewPortRect();
                    if ( rcItem.Bottom > rc.Bottom )
                    {
                      firstItem++;
                      Invalidate(rc);
                    }
                  }
                }
              }
            }
            doScrollingLoop = false;
            break;
          }
          case (int)Msg.WM_KEYDOWN:
          {
            if ( (int)msg.wParam == (int)VirtualKeys.VK_ESCAPE)
              doScrollingLoop = false;
            break;
          }
          default:
            WindowsAPI.DispatchMessage(ref msg);
            break;
        }
      }

      // Release the capture
      Capture = false;

      // Stop timer
      timer.Stop();
      timer.Dispose();

      // Reset flags
      arrowVisible = false;
      arrowPressed = false;
      timerTicking = false;

      Rectangle viewPortRect = GetViewPortRect();
      Invalidate(viewPortRect);
    }

    void ScrollingTick(Object timeObject, EventArgs eventArgs)
    {
      // Get mouse coordinates
      Point point = Control.MousePosition;
      point = PointToClient(point);

      if ( upTimerTicking )
      {
        if ( buttonPushed )
        {
          if ( upArrowRect.Contains(point))
          {
            upArrowPressed = true;
            if ( firstItem > 0 )
            {
              firstItem--;
              Rectangle rc = GetViewPortRect();
              Invalidate(rc);
            }
            else
              doScrollingLoop = false;
          }
          else
            upArrowPressed = false;
        }
      }
      else if ( downTimerTicking )
      {
        if ( buttonPushed )
        {
          if ( downArrowRect.Contains(point))
          {
            downArrowPressed = true;
            using ( Graphics g = Graphics.FromHwnd(Handle) )
            {
              // Get the last item rectangle
              OutlookBarBand band = bands[currentBandIndex];
              Rectangle rcItem = GetItemRect(g, band, band.Items.Count - 1, Rectangle.Empty);
              Rectangle rc = GetViewPortRect();
              if ( rcItem.Bottom > rc.Bottom )
              {
                firstItem++;
                Invalidate(rc);
              }
            }
          }
          else
            doScrollingLoop = false;
        }
        else
          downArrowPressed = false;
      }

    }

    void ProcessHeaderHit(int index)
    {
      Capture = true;
      Rectangle rc = GetHeaderRect(index);
      // Draw header pressed
      Graphics g = Graphics.FromHwnd(Handle);
      bool headerPressed = true;
      DrawHeader(g, index, rc, Border3DStyle.Sunken);

      bool doLoop = true;
      while (doLoop)
      {
        // Check messages until we find a condition to break out of the loop
        Win32.MSG msg = new Win32.MSG();
        WindowsAPI.GetMessage(ref msg, 0, 0, 0);
        Point point = new Point(0,0);
        if ( msg.message == (int)Msg.WM_MOUSEMOVE || msg.message == (int)Msg.WM_LBUTTONUP )
        {
          point = WindowsAPI.GetPointFromLPARAM((int)msg.lParam);
        }

        switch(msg.message)
        {
          case (int)Msg.WM_MOUSEMOVE:
          {
            int currentIndex;
            HitTestType hit = HitTest(point, out currentIndex, false);
            if (hit == HitTestType.Header && currentIndex == index)
            {
              if (!headerPressed)
              {
                DrawHeader(g, index, rc, Border3DStyle.Sunken);
                headerPressed = true;
              }
            }
            else
            {
              if (headerPressed)
              {
                DrawHeader(g, index, rc, Border3DStyle.RaisedInner);
                headerPressed = false;
              }
            }
            break;
          }
          case (int)Msg.WM_LBUTTONUP:
          {
            if (headerPressed)
            {
              DrawHeader(g, index, rc, Border3DStyle.RaisedInner);
              headerPressed = false;
            }
            int newIndex;
            HitTestType ht = HitTest(point, out newIndex, false);
            if ( ht == HitTestType.Header && newIndex != selectedHeader )
              SetCurrentBand(newIndex);
            doLoop = false;
            break;
          }
          case (int)Msg.WM_KEYDOWN:
          {
            if ( (int)msg.wParam == (int)VirtualKeys.VK_ESCAPE)
              doLoop = false;
            break;
          }
          default:
            WindowsAPI.DispatchMessage(ref msg);
            break;
        }
      }

      // Reset flags
      Capture = false;
      g.Dispose();

    }

    void ProcessItemHit(int index, Point pt)
    {
      bool itemPressed = true;
      Capture = true;
      Graphics g = Graphics.FromHwnd(Handle);
      Rectangle itemRect = GetItemRect(g, bands[currentBandIndex], index, Rectangle.Empty);
      DrawItem(g, index, itemRect, true, true, Rectangle.Empty, null);
      bool dragging = false;
      int deltaX = 0;
      int deltaY = 0;
      bool itemHighlighted = false;
      int dragItemIndex = -1;

      bool doLoop = true;
      while ( doLoop )
      {
        // Check messages until we find a condition to break out of the loop
        Win32.MSG msg = new Win32.MSG();
        WindowsAPI.GetMessage(ref msg, 0, 0, 0);
        Point point = new Point(0,0);
        if ( msg.message == (int)Msg.WM_MOUSEMOVE || msg.message == (int)Msg.WM_LBUTTONUP )
        {
          point = WindowsAPI.GetPointFromLPARAM((int)msg.lParam);
          if ( msg.message == (int)Msg.WM_MOUSEMOVE )
          {
            deltaX = pt.X - point.X;
            deltaY = pt.Y - point.Y;
          }
        }

        switch(msg.message)
        {
          case (int)Msg.WM_MOUSEMOVE:
          {
            int currentIndex;
            HitTestType hit = HitTest(point, out currentIndex, dragging);
            if ( dragging )
            {

              if ( hit == HitTestType.DropLine || hit == HitTestType.DropLineLastItem  )
              {
                Cursor.Current = dragCursor;
                // Draw the Dragline
                DrawDropLine(g, currentIndex, true, hit);
              }
              else
              {
                Cursor.Current = Cursors.No;
                // Erase the Dragline
                DrawDropLine(g, index, false, hit);
              }

              if ( hit == HitTestType.Item && currentIndex == index)
              {
                if ( itemHighlighted == false )
                {
                  DrawItem(g, index, itemRect,
                    true, false, Rectangle.Empty, null);
                  itemHighlighted = true;
                }
              }
              else
              {
                if ( hit == HitTestType.Item )
                {

                  // Erase previous highlighting first
                  if ( itemHighlighted )
                  {
                    DrawItem(g, index, itemRect,
                      false, false, Rectangle.Empty, null);
                  }

                  // Highlight new item
                  itemRect = GetItemRect(g, bands[currentBandIndex], currentIndex, Rectangle.Empty);
                  index = currentIndex;
                  DrawItem(g, index, itemRect,
                    true, false, Rectangle.Empty, null);
                }
                else
                {
                  // the mouse did not hit an item
                  if ( itemHighlighted )
                  {
                    DrawItem(g, index, itemRect,
                      false, false, Rectangle.Empty, null);
                    itemHighlighted = false;
                  }
                }
              }
            }
            else
            {
              if (hit == HitTestType.Item && currentIndex == index)
              {

                // Set no drag cursor if there have been at least
                // a 5 pixel movement
                int pixelmov = 5;
                if ( bands[currentBandIndex].IconView == IconView.Small ) pixelmov = 2;
                bool unpressed = false;
                if ( Math.Abs(deltaX) >= pixelmov || Math.Abs(deltaY) >= pixelmov )
                {
                  unpressed = true;
                  Cursor.Current = Cursors.No;
                  dragging = true;
                  dragItemIndex = index;
                }

                if (itemPressed && unpressed)
                {
                  DrawItem(g, index, itemRect,
                    true, false, Rectangle.Empty, null);
                  itemPressed = false;
                  itemHighlighted = true;
                }
              }
            }
            break;
          }
          case (int)Msg.WM_LBUTTONUP:
          {

            // Highlight the item
            if (itemPressed)
            {
              DrawItem(g, index, itemRect, true, false, Rectangle.Empty, null);
              itemPressed = false;
            }

            int newIndex;
            HitTestType ht = HitTest(point, out newIndex, true);
            bool doDrop = false;
            if ( dragging && (ht == HitTestType.DropLine || ht == HitTestType.DropLineLastItem) )
            {
              // Delete dropline
              Cursor.Current = Cursors.Default;
              // Erase the Dragline
              DrawDropLine(g, index, false, ht);

              // Move the dragged item to the new location
              // only if the new location is not contiguous to its
              // own location
              if ( dragItemIndex > droppedPosition
                && Math.Abs(dragItemIndex - droppedPosition) > 0 )
                doDrop = true;
              else if ( dragItemIndex < droppedPosition
                && Math.Abs(dragItemIndex - droppedPosition) > 1 )
                doDrop = true;

              if ( doDrop )
              {
                // Remove item from its old location
                OutlookBarItem dragItem = bands[currentBandIndex].Items[dragItemIndex];
                bands[currentBandIndex].Items.RemoveAt(dragItemIndex);
                // Insert item in its new location
                if ( dragItemIndex < droppedPosition )
                  droppedPosition--;

                bands[currentBandIndex].Items.Insert(droppedPosition, dragItem);
              }
            }

            // Repaint the bar just in case we had a dropline painted
            Invalidate();

            // Highlight item
            if ( !dragging )
            {
              // do not highlight if we are dropping
              forceHightlight = true;
              forceHightlightIndex = index;
              // Fire item clicked property
              FireItemClicked(index);
            }
            else
            {
              // Fire item dropped event
              if ( droppedPosition != -1 && doDrop )
                FireItemDropped(droppedPosition);
            }

            doLoop = false;
            break;
          }
          case (int)Msg.WM_KEYDOWN:
          {
            if ( (int)msg.wParam == (int)VirtualKeys.VK_ESCAPE)
              doLoop = false;
            break;
          }
          default:
            WindowsAPI.DispatchMessage(ref msg);
            break;
        }
      }

      g.Dispose();
      Capture = false;
    }

    void HighlightHeader(int index)
    {
      if ( lastHighlightedHeader == index ) return;

      Graphics g = Graphics.FromHwnd(Handle);
      Rectangle rc;
      if ( lastHighlightedHeader >= 0 )
      {
        rc = GetHeaderRect(lastHighlightedHeader);
        DrawHeader(g, lastHighlightedHeader, rc, Border3DStyle.RaisedInner);
      }

      lastHighlightedHeader = index;
      if ( lastHighlightedHeader >= 0 )
      {
        rc = GetHeaderRect(lastHighlightedHeader);
        DrawHeader(g, lastHighlightedHeader, rc, Border3DStyle.Raised);
      }
    }

    void OnHighlightHeader(Object timeObject, EventArgs eventArgs)
    {
      if( this.IsDisposed == true )
      {
        highlightTimer.Stop();
        return;
      }

      Point mousePos = Control.MousePosition;
      mousePos = PointToClient(mousePos);
      Rectangle rc = ClientRectangle;

      if( !rc.Contains( mousePos ) )
      {
        HighlightHeader(-1);
        HighlightItem(-1, false);
        highlightTimer.Stop();
      }
    }

    void HighlightItem(int index, bool pressed)
    {
      // Exit if item state has not changed
      if ( lastHighlightedItem == index && previousPressed == pressed ) return;
      // Remember if we were pressed or not
      previousPressed = pressed;

      if ( lastHighlightedItem >= 0 )
      {
        // Draw the previously highlighted item in normal state
        using ( Graphics g = Graphics.FromHwnd(Handle) )
        {
          Rectangle itemRect = GetItemRect(g, bands[currentBandIndex], lastHighlightedItem, Rectangle.Empty);
          DrawItem(g, lastHighlightedItem, itemRect, false, false, Rectangle.Empty, null);
        }
      }

      lastHighlightedItem = index;
      if ( lastHighlightedItem >= 0 )
      {
        // Draw this item hightlighed
        using ( Graphics g = Graphics.FromHwnd(Handle) )
        {
          Rectangle itemRect = GetItemRect(g, bands[currentBandIndex], lastHighlightedItem, Rectangle.Empty);
          DrawItem(g, lastHighlightedItem, itemRect, true, pressed, Rectangle.Empty, null);
        }
      }
    }

    public void PerformClick(int index)
    {
      FireItemClicked(index);
    }

    public void CheckItem ( OutlookBarItem item )
    {
      if ( m_checkStyle == OutlookBarCheckStyle.ItemsAsRadioButtons )
        foreach ( OutlookBarItem tempItem in Bands[currentBandIndex].Items ) 
        {
          tempItem.Checked = false;
        }
      item.Checked = true;
      Invalidate();
    }
    public void UnCheckItem ( OutlookBarItem item )
    {
      item.Checked = false;
      Invalidate();
    }
    Rectangle GetWorkRect()
    {
      Rectangle rc = ClientRectangle;
      if ( borderType != BorderType.None )
        rc.Inflate(-2,-2);
      return rc;
    }

    Rectangle GetViewPortRect()
    {
      // This is the area of the control minus
      // the bands headers
      Rectangle rect = GetWorkRect();
      if ( bands.Count > 0 )
      {
        // Decrease the client area by the number of headers
        int top = rect.Top + 1 + BAND_HEADER_HEIGHT * (currentBandIndex + 1);
        int bottom = rect.Bottom - 1 - BAND_HEADER_HEIGHT * (bands.Count - currentBandIndex - 1);
        return new Rectangle(rect.Left, top, rect.Width, bottom - top);
      }

      return rect;
    }

    Rectangle GetHeaderRect(int index)
    {
      // Make sure we are within bounds
      Debug.Assert((index >= 0 && index <= bands.Count-1), "Invalid Header Index");
      Rectangle rect = GetWorkRect();

      int top = rect.Top;
      int bottom = rect.Bottom;
      int max = bands.Count;

      if ( index > currentBandIndex )
      {
        top = rect.Bottom - 1 - (max - index) * BAND_HEADER_HEIGHT;
        bottom = rect.Bottom - 1 - (max - index - 1) * BAND_HEADER_HEIGHT;
      }
      else
      {
        top = rect.Top + 1 + index * BAND_HEADER_HEIGHT;
        bottom = rect.Top + 1 + (index+1) * BAND_HEADER_HEIGHT;
      }
      return  new Rectangle(rect.Left, top, rect.Width, bottom-top);
    }

    void AnimateScroll(int From, int To)
    {
      if ( currentBandIndex == -1 || bands.Count == 0 ) return;

      OutlookBarBand band = bands[currentBandIndex];
      // Make sure we are whithin bounds
      Debug.Assert(From >= 0 && From < bands.Count);
      Debug.Assert(To >= 0 &&  To < bands.Count);

      // Get needed dimensions
      Rectangle viewPortRect = GetViewPortRect();
      Rectangle headerRect = new Rectangle(0, 0, viewPortRect.Width, BAND_HEADER_HEIGHT);
      Rectangle drawingRect = new Rectangle(0, 0, viewPortRect.Width, viewPortRect.Height + headerRect.Height * 2);

      // Use straight GDI to do the drawing
      IntPtr hDC = WindowsAPI.GetDC(Handle);
      IntPtr hDCFrom = WindowsAPI.CreateCompatibleDC(hDC);
      IntPtr hDCTo = WindowsAPI.CreateCompatibleDC(hDC);
      IntPtr bmFrom = WindowsAPI.CreateCompatibleBitmap(hDC, drawingRect.Width, drawingRect.Height);
      IntPtr bmTo = WindowsAPI.CreateCompatibleBitmap(hDC, drawingRect.Width, drawingRect.Height);

      // Select in the drawing surface
      IntPtr hOldFromBitmap = WindowsAPI.SelectObject(hDCFrom, bmFrom);
      IntPtr hOldToBitmap = WindowsAPI.SelectObject(hDCTo, bmTo);

      // Draw in the memory device context
      Graphics gHeaderDC;
      if ( To > From )
        gHeaderDC = Graphics.FromHdc(hDCTo);
      else
        gHeaderDC = Graphics.FromHdc(hDCFrom);

      DrawBandBitmap(hDCFrom, bands[From], From, drawingRect);
      DrawBandBitmap(hDCTo,  bands[To], To, drawingRect);
      DrawHeader(gHeaderDC, To, new Rectangle(drawingRect.Left, drawingRect.Top,
        drawingRect.Width, drawingRect.Top + BAND_HEADER_HEIGHT), Border3DStyle.RaisedInner);

      Rectangle rectFrom = GetHeaderRect(From);
      Rectangle rectTo = GetHeaderRect(To);
      int headerHeight = rectFrom.Height;

      // Do the animation with the bitmaps
      if ( To > From)
      {
        for (int y = rectTo.Top - headerHeight; y > rectFrom.Bottom; y -= headerHeight)
        {
          // Draw From bitmap
          WindowsAPI.BitBlt(hDC, viewPortRect.Left ,rectFrom.Bottom + 1,
            viewPortRect.Width, y - rectFrom.Bottom - 1, hDCFrom, 0 , 0, (int)PatBltTypes.SRCCOPY);

          // Draw To Bitmap
          WindowsAPI.BitBlt(hDC, viewPortRect.Left, y, viewPortRect.Width,
            viewPortRect.Bottom - y + headerHeight, hDCTo, 0, 0, (int)PatBltTypes.SRCCOPY);
          Thread.Sleep(animationSpeed);
        }
      }
      else
      {
        Rectangle rcTo = new Rectangle(viewPortRect.Left,
          viewPortRect.Bottom, viewPortRect.Width, viewPortRect.Bottom - headerHeight);
        for (int y = rectFrom.Top + 1; y < rcTo.Top - headerHeight; y += headerHeight)
        {

          // Draw To Bitmap
          WindowsAPI.BitBlt(hDC, viewPortRect.Left, rectFrom.Top,  viewPortRect.Width,
            y - rectFrom.Top - 1, hDCTo, 0, 0, (int)PatBltTypes.SRCCOPY);

          // Draw From bitmap
          WindowsAPI.BitBlt(hDC, viewPortRect.Left , y,
            viewPortRect.Width, viewPortRect.Bottom - y, hDCFrom, 0 , 0, (int)PatBltTypes.SRCCOPY);
          Thread.Sleep(animationSpeed);

        }
      }

      // Cleanup
      WindowsAPI.ReleaseDC(Handle, hDC);
      WindowsAPI.DeleteDC(hDCFrom);
      WindowsAPI.DeleteDC(hDCTo);
      WindowsAPI.SelectObject(hDCFrom, bmFrom);
      WindowsAPI.SelectObject(hDCTo, bmTo);
      WindowsAPI.DeleteObject(bmFrom);
      WindowsAPI.DeleteObject(bmTo);

    }

    private bool HasChild()
    {
      return HasChild( currentBandIndex );
    }

    private bool HasChild( int index )
    {
      // Flag that tell us if the current band
      // has a child window
      Control childControl = null;

      if( index != -1 && bands.Count > 0 && index >= 0 && index < bands.Count )
      {
        OutlookBarBand band = bands[ index ];
        childControl = band.ChildControl;
      }

      return childControl != null;
    }

    private void HandleRecreated(object sender, EventArgs e)
    {
      // If any of the child destroy itself and recreate
      // reattach the Outlookbar as the parent
      Control c = (Control)sender;
      IntPtr hParent = WindowsAPI.GetParent(c.Handle);
      if ( hParent != Handle )
      {
        WindowsAPI.SetParent(c.Handle, Handle);
      }
    }

    void CreateContextMenu()
    {
      // context menu
      MenuItemEx largeIconsMenu = new MenuItemEx("Large Icons", new EventHandler(OnContextMenu));
      MenuItemEx smallIconsMenu = new MenuItemEx("Small Icons", new EventHandler(OnContextMenu));
      MenuItemEx separator1 = new MenuItemEx("-", new EventHandler(OnContextMenu));
      MenuItemEx renameGroup = new MenuItemEx("Rename Group", new EventHandler(OnContextMenu));
      MenuItemEx separator2 = new MenuItemEx("-", new EventHandler(OnContextMenu));
      MenuItemEx renameShortcut = new MenuItemEx("Rename Shortcut", new EventHandler(OnContextMenu));

      contextMenu = new ContextMenu();
      contextMenu.MenuItems.Add(0, largeIconsMenu);
      contextMenu.MenuItems.Add(1, smallIconsMenu);
      contextMenu.MenuItems.Add(2, separator1);
      contextMenu.MenuItems.Add(3, renameGroup);
      contextMenu.MenuItems.Add(4, separator2);
      contextMenu.MenuItems.Add(5, renameShortcut);

      contextMenu.Popup += new EventHandler(ContextMenuPopup);
      this.ContextMenu = contextMenu;
    }

    public void ContextMenuPopup(object sender, EventArgs e)
    {
      // Update the menu state before displaying it
      OutlookBarBand band = bands[currentBandIndex];

      MenuItemEx largeIconsMenu = (MenuItemEx)ContextMenu.MenuItems[0];
      MenuItemEx smallIconsMenu = (MenuItemEx)ContextMenu.MenuItems[1];
      MenuItemEx renameGroup    = (MenuItemEx)ContextMenu.MenuItems[3];
      MenuItemEx renameShortcut = (MenuItemEx)ContextMenu.MenuItems[5];


      int index;
      HitTestType hit = HitTest(lastClickedPoint, out index, false);
      if ( hit == HitTestType.Header )
      {
        renameGroup.Enabled = true;
        largeIconsMenu.Enabled = false;
        smallIconsMenu.Enabled = false;
        renameShortcut.Enabled = false;
      }
      else if ( hit == HitTestType.Item )
      {
        renameGroup.Enabled = false;
        largeIconsMenu.Enabled = false;
        smallIconsMenu.Enabled = false;
        renameShortcut.Enabled = true;
      }
      else
      {
        renameGroup.Enabled = false;
        largeIconsMenu.Enabled = true;
        smallIconsMenu.Enabled = true;
        renameShortcut.Enabled = false;
      }

      if ( HasChild() )
      {
        largeIconsMenu.RadioCheck = false;
        smallIconsMenu.RadioCheck = false;
      }
      else
      {
        largeIconsMenu.RadioCheck = (band.IconView == IconView.Large);
        smallIconsMenu.RadioCheck = (band.IconView == IconView.Small);
      }
    }

    void RenameItem()
    {
      // Display a edit control that will do the editing of  the item
      // Get the item index first
      int index;
      HitTestType hit = HitTest(lastClickedPoint, out index, false);
      using ( Graphics g = Graphics.FromHwnd(Handle) )
      {
        Rectangle itemRect = GetLabelRect(index);
        // Empty the text box first
        textBox.Text = "";
        // Move it to the top of the item rectangle
        if ( bands[currentBandIndex].IconView == IconView.Large )
          itemRect.Inflate(5, 0);
        else
          itemRect = new Rectangle(itemRect.Left, itemRect.Top, itemRect.Right, itemRect.Height + 5);

        textBox.Bounds = itemRect;
        // initialize the text box to the item text
        textBox.Text = bands[currentBandIndex].Items[index].Text;
        // Flag that we are editing an item and not a header
        // --We'll use this on the textbox event handlers
        editingAnItem = true;
        textBox.Visible = true;
        textBox.Focus();
      }
    }

    void RenameHeader()
    {
      // Display a edit control that will do the editing of the header
      // Get the item index first
      int index;
      HitTestType hit = HitTest(lastClickedPoint, out index, false);
      using ( Graphics g = Graphics.FromHwnd(Handle) )
      {
        Rectangle headerRect = GetHeaderRect(index);
        // Empty the text box first
        textBox.Text = "";
        textBox.Bounds = headerRect;
        // initialize the text box to the item text
        textBox.Text = bands[index].Text;
        // Flag that we are editing an header and not a item
        // --We'll use this on the textbox event handlers
        editingAnItem = false;
        textBox.Visible = true;
        textBox.Focus();
      }
    }

    void TextBoxKeyDown(object sender, KeyEventArgs e)
    {
      if ( e.KeyCode == Keys.Enter )
        ProcessTextBoxEditing();
    }

    void TextBoxLostFocus(object sender, EventArgs e)
    {
      ProcessTextBoxEditing();
    }

    void ProcessTextBoxEditing()
    {
      // Only if the textbox is actually visible
      if ( textBox.Visible == false ) return;

      string text = textBox.Text;
      textBox.Visible = false;
      int index;
      HitTestType hit = HitTest(lastClickedPoint, out index, false);
      if ( editingAnItem )
      {
        bands[currentBandIndex].Items[index].Text = text;
        // Fire property changed
        FirePropertyChanged(OutlookBarProperty.ShortcutNameChanged);
      }
      else
      {
        bands[index].Text = text;
        // Fire property changed
        FirePropertyChanged(OutlookBarProperty.GroupNameChanged);
      }
      // Invalidate control so that new label size
      // is recalculated
      Invalidate();
    }

    #endregion

  }
}

