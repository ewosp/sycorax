using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;

using UtilityLibrary.Collections;
using UtilityLibrary.Win32;
using UtilityLibrary.General;

namespace UtilityLibrary.CommandBars
{
  // Just to make this class work without the actual
  // designer which is not being added to this verson of the UtilityLibrary
  public class ReBarDesigner
  {
    public void PassMsg(ref Message message)
    {
    }

  }
  
  /// <summary>
  /// Summary description for Rebar.
  /// </summary>
  [ToolboxBitmap(typeof(UtilityLibrary.CommandBars.ReBar), 
     "UtilityLibrary.CommandBars.ReBar.bmp")]
  public class ReBar : Control, IMessageFilter
  {
    #region Class Variables
    RebarBandCollection bands = null;
    static bool needsColorUpdate = false;
    bool bGotIsCommonCtrl6 = false;
    bool isCommonCtrl6 = false;
    const int MINIMUM_HEIGHT = 30;

    // Designer Support
    ToolBarEx placeHolderToolBar = null;
    bool placeHolderAdded = false;
    ReBarDesigner rebarDesigner = null;
    bool addPlaceHolderToolBar = false;
    bool designerInTransaction = false;

    #endregion

    #region Constructors
    public ReBar()
    {
      SetStyle(ControlStyles.UserPaint, false);
      TabStop = false;
      Dock = DockStyle.Top;
      bands = new RebarBandCollection(this);
      bands.Changed += new EventHandler(Bands_Changed);
      addPlaceHolderToolBar = true;
    }
    #endregion

    #region Overrides
    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if ( Capture )
        return;

      bool hit = HitGripper(e);
      if ( hit )
      {
        if ( ShowMoveCursor(e) )
          Cursor.Current = Cursors.SizeAll;
        else
          Cursor.Current = Cursors.SizeWE;
      }
      else
        Cursor.Current = Cursors.Default;
    }
    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      bool hit = HitGripper(e);
      if ( hit ) 
      {
        Capture = true;
        if ( ShowMoveCursor(e) )
          Cursor.Current = Cursors.SizeAll;
        else
          Cursor.Current = Cursors.VSplit;
      }
      else
        Cursor.Current = Cursors.Default;

    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
      bool hit = HitGripper(e);
      Capture = false;
      if ( hit )
      {
        if ( ShowMoveCursor(e) )
          Cursor.Current = Cursors.SizeAll;
        else
          Cursor.Current = Cursors.SizeWE;
      }
      else
        Cursor.Current = Cursors.Default;
      
    }

    protected override void Dispose(bool disposing)
    {
      if ( disposing )
        bands.Changed -= new EventHandler(Bands_Changed);
    }
    protected override void OnHandleCreated(EventArgs e)
    {
      base.OnHandleCreated(e);
      RealizeBands();
    }
    
    protected override void WndProc(ref Message m) 
    {
      base.WndProc(ref m);

      switch( m.Msg )
      {
        case (int)Msg.WM_PAINT:
          PaintBackground();
          break;
        
        case (int)Msg.WM_NOTIFY:
        case (int)((int)Msg.WM_NOTIFY + (int)Msg.WM_REFLECT):
        {
          NMHDR note = (NMHDR)m.GetLParam(typeof(NMHDR));
          switch (note.code)
          {
            case (int)RebarNotifications.RBN_HEIGHTCHANGE:
              UpdateSize();
              break;

            case (int)RebarNotifications.RBN_CHEVRONPUSHED:
              NotifyChevronPushed(ref m);
              break;
            
            case (int)RebarNotifications.RBN_CHILDSIZE:
              NotifyChildSize( ref m );
              break;
            
            case (int)NotificationMessages.NM_NCHITTEST:
              break;
          }
        }
          break;
      }
    }
    protected override void OnParentChanged(EventArgs e)
    {
      if (Parent != null)
        Application.AddMessageFilter(this);
      else
        Application.RemoveMessageFilter(this);  
    }

    protected override Size DefaultSize
    {
      get 
      { 
        return new Size(100, 44); 
      }
    }

    protected override void CreateHandle() 
    {
      if (!RecreatingHandle)
      {
        INITCOMMONCONTROLSEX icex = new INITCOMMONCONTROLSEX();
        icex.dwSize = Marshal.SizeOf(typeof(INITCOMMONCONTROLSEX));
        icex.dwICC = (int)(CommonControlInitFlags.ICC_BAR_CLASSES | CommonControlInitFlags.ICC_COOL_CLASSES);
        bool  fail = WindowsAPI.InitCommonControlsEx(icex);
      }
      base.CreateHandle();

    }
  
    protected override CreateParams CreateParams
    {
      get
      {
        CreateParams createParams = base.CreateParams;
        createParams.ClassName = WindowsAPI.REBARCLASSNAME;
        createParams.Style = (int)(WindowStyles.WS_CHILD | WindowStyles.WS_VISIBLE
          | WindowStyles.WS_CLIPCHILDREN | WindowStyles.WS_CLIPSIBLINGS);
        createParams.Style |= (int)(CommonControlStyles.CCS_NODIVIDER | CommonControlStyles.CCS_NOPARENTALIGN);
        
        createParams.Style |= (int)(RebarStyles.RBS_VARHEIGHT | RebarStyles.RBS_AUTOSIZE);
        return createParams;
      }
    }
    
    public override bool PreProcessMessage(ref Message msg)
    {
      foreach (Control band in bands)
      {
        if (band.PreProcessMessage(ref msg))
          return true;
      }
      return false;
    }
    #endregion

    #region Properties
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public RebarBandCollection Bands
    {
      get { return bands; }
    }

    internal bool PlaceHolderAdded
    {
      get { return placeHolderAdded; }
    }

    internal bool AddPlaceHolderToolBar
    {
      set { addPlaceHolderToolBar = value; }
      get { return addPlaceHolderToolBar; }
    }

    internal ToolBarEx PlaceHolderToolBar
    {
      get { return placeHolderToolBar; }
    }

    internal ReBarDesigner RebarDesigner
    {
      set { rebarDesigner = value; }
      get { return rebarDesigner; }
    }

    internal bool DesignerInTransaction
    {
      set { designerInTransaction = value; }
      get { return designerInTransaction; }
    }

    #endregion

    #region Methods
    public void UpdateBackgroundColor()
    {
      for ( int i = 0; i < bands.Count; i++ )
      {
        // Update Rebar band information
        // This make sure that the background color and foreground color
        // of the bands are set to the new system colors
        UpdateBand(i);
        ToolBarEx toolBar = (ToolBarEx)bands[i];
        toolBar.Invalidate();
      }
      Invalidate();
    }

    static public void UpdateBandsColors(object sender, EventArgs e)
    {
      needsColorUpdate = true;
    }
  
    public void BeginUpdate()
    {
      WindowsAPI.SendMessage(Handle, (int)Msg.WM_SETREDRAW, 0, 0);
    }

    public void EndUpdate()
    {
      WindowsAPI.SendMessage(Handle, (int)Msg.WM_SETREDRAW, 1, 0);
    }

    internal void UpdateBands()
    {
      if (IsHandleCreated) 
      {
        RecreateHandle();
      }
    }

    public override void Refresh()
    {
      base.Refresh();

      foreach( ToolBarEx tool in Bands )
      {
        tool.UpdateToolBarItems();
      }
    
    }
    #endregion

    #region Implementation
    private bool IsCommonCtrl6()
    {
      // Cache this value for efficenty
      if ( bGotIsCommonCtrl6 == false )
      {     
        DLLVERSIONINFO dllVersion = new DLLVERSIONINFO();
        // We are assummng here that anything greater or equal than 6
        // will have the new XP theme drawing enable
        dllVersion.cbSize = Marshal.SizeOf(typeof(DLLVERSIONINFO));
        WindowsAPI.GetCommonControlDLLVersion(ref dllVersion);
        bGotIsCommonCtrl6 = true;
        isCommonCtrl6 = (dllVersion.dwMajorVersion >= 6);
      }
      return isCommonCtrl6;
    }

    internal void PassMsg(ref Message m)
    {
      WndProc(ref m);
    }

    internal void AddPlaceHolder()
    {
      placeHolderAdded = true;

      // Add place holder toolBar for designer support
      if ( placeHolderToolBar == null )
      {
        placeHolderToolBar = new ToolBarEx();
        placeHolderToolBar.m_parent = this;
      }
            
      REBARBANDINFO bandInfo = GetBandInfo(0, placeHolderToolBar);
      int result = WindowsAPI.SendMessage(Handle, (int)RebarMessages.RB_INSERTBANDW, 0, ref bandInfo);
    }

    internal void RemovePlaceHolder()
    {
      placeHolderAdded = false;
      REBARBANDINFO bandInfo = GetBandInfo(0, placeHolderToolBar);
      int result = WindowsAPI.SendMessage(Handle, (int)RebarMessages.RB_DELETEBAND, 0, 0);
    }

    internal void RemoveBand(int index)
    {
      // index is base on the Band collection
      // but we need the actual index used in the native Rebar control
      int actualIndex = GetBandActualIndex(index);
      int result = WindowsAPI.SendMessage(Handle, (int)RebarMessages.RB_DELETEBAND, actualIndex, 0);
    }

    private void PaintBackground()
    {
      // This is for the very first time the ToolBarEx place holder is added
      if ( bands.Count == 0  &&  addPlaceHolderToolBar == true)
      {
        addPlaceHolderToolBar = false;
        AddPlaceHolder();
      }
            
      if ( needsColorUpdate)
      {
        needsColorUpdate = false;
        for ( int i = 0; i < bands.Count; i++ )
        {
          // Update toolbar specific information
          // This update is to guarantee that the toolbar can resize
          // the buttons appropiately in case the SystemMenuFont was
          // changed along with the system colors
          ToolBarEx toolBar = (ToolBarEx)bands[i];
          toolBar.UpdateToolBarItems();
        }
        for ( int i = 0; i < bands.Count; i++ )
        {
          // Update Rebar band information
          // This make sure that the background color and foreground color
          // of the bands are set to the new system colors
          UpdateBand(i);
        }
      }

      // We don't need to paint the gripper if we are going
      // to let the operating system do the painting
      if ( IsCommonCtrl6() )
        return;
            
      Control c = null;
      Rectangle rc;
      for ( int i = 0; i < bands.Count; i++ )
      {
        Graphics g = CreateGraphics();
        c = bands[i];
        RectangleF rf = g.ClipBounds;
        rc = c.Bounds;
        if ( rf.Contains(rc) )
        {
          ToolBarEx toolBar = ( ToolBarEx )bands[i];
          if ( toolBar.BarType == TBarType.MenuBar ) 
          {
            // The menu bar height is smaller that the other toolbars
            // and if the menubar is in the same row with a toolbar that is bigger in height
            // the toolbar gripper will not be painted correctly if we use the actual height
            // of the menubar. Instead ajust the rectangle to compensate for the actual height
            // of the band
            Rectangle menuRect = GetBandRect(i);
            int offset = (menuRect.Height - rc.Height)/2-1;
            rc = new Rectangle(rc.Left, rc.Top-offset, menuRect.Width, menuRect.Height-2);
          }
          DrawGripper(g, rc);
        }
        g.Dispose();
      }
    }

    private void DrawGripper(Graphics g, Rectangle bounds)
    {
      bounds.X = bounds.Left - 7;
      bounds.Width = 7;

      g.FillRectangle( ColorUtil.VSNetControlBrush, bounds );
      int nHeight = bounds.Height;
      for ( int i = 2; i < nHeight-1; i++) 
      {
        if ( ColorUtil.UsingCustomColor )
          g.DrawLine( ColorUtil.VSNetBorderPen, bounds.Left, bounds.Top+i, bounds.Left+3, bounds.Top+i );
        else 
          g.DrawLine( SystemPens.ControlDark, bounds.Left, bounds.Top+i, bounds.Left+3, bounds.Top+i );
        i++;
      }
    }

    protected bool HitGripper(MouseEventArgs e)
    {
      // Find out if we hit the gripper
      Point mousePoint = new Point(e.X, e.Y);
      Control c = null;
      Rectangle bounds;
      for ( int i = 0; i < bands.Count; i++ )
      {
        c = bands[i];
        bounds = c.Bounds;
        
        // adjust to gripper area
        bounds.X = bounds.Left - 7;
        bounds.Width = 7;
        
        if ( bounds.Contains(mousePoint) )
          return true;
      }
      return false;

    }

    private bool ShowMoveCursor(MouseEventArgs e)
    {
      // Even though we can actually move the toolbars around always
      // sometimes it is more intuive to show the "Move" cursor depending
      // how many bars are in the same row that always showing the resize cursor
      Point mousePoint = new Point(e.X, e.Y);
      Control c = null;
      Rectangle bounds;
      for ( int i = 0; i < bands.Count; i++ )
      {
        c = bands[i];
        bounds = c.Bounds;
        
        // adjust to gripper area
        bounds.X = bounds.Left - 7;
        bounds.Width = 7;
        
        if ( bounds.Contains(mousePoint) )
        {
          if ( bounds.Left <= 5 )
          {   // The left value would be actually at least 2 if the toolbar
            // is on the edge of the main window as opossed to be somewhere in the middle of the
            // strip. The assumption here is that the gripper starts approximately 2 pixel from the edge
            return true;
          }
        }
      }
      
      return false;
    }
        
    private void NotifyChevronPushed(ref Message m)
    {
      NMREBARCHEVRON nrch = (NMREBARCHEVRON) m.GetLParam(typeof(NMREBARCHEVRON)); 
      REBARBANDINFO rb;
      int bandIndex = nrch.uBand;
      rb = GetRebarInfo(bandIndex);
      int actualIndex = rb.wID;
      Control band = (Control)bands[actualIndex];
      Point point = new Point(nrch.rc.left, nrch.rc.bottom);
      (band as IChevron).Show(this, point);
      
    }

    private void NotifyChildSize( ref Message m )
    {
      NMREBARCHILDSIZE size = ( NMREBARCHILDSIZE )m.GetLParam( typeof( NMREBARCHILDSIZE ) );

      if( size.uBand >= 0 && size.uBand < bands.Count )
      {
        ToolBarEx toolBar = ( ToolBarEx )bands[ (int)size.uBand ];
        toolBar.ToolbarSizeChanged( size );
      }
      /*
      for( int i = 0; i < bands.Count; i++ )
      {
        // Update toolbar specific information
        // This update is to guarantee that the toolbar can resize
        // the buttons appropiately in case the SystemMenuFont was
        // changed along with the system colors
        ToolBarEx toolBar = ( ToolBarEx )bands[i];
        toolBar.ToolbarSizeChanged( size );
      }
      */
    }

    bool IMessageFilter.PreFilterMessage(ref Message message)
    {
      ArrayList handles = new ArrayList();
      IntPtr handle = Handle;
      while (handle != IntPtr.Zero)
      {
        handles.Add(handle);
        handle = WindowsAPI.GetParent(handle);  
      }

      handle = message.HWnd;
      while (handle != IntPtr.Zero)
      {
        Msg currentMessage = (Msg)message.Msg;
        if (handles.Contains(handle)) 
          return PreProcessMessage(ref message);
        handle = WindowsAPI.GetParent(handle);
      }
      
      return false;
    }

    private void RealizeBands()
    {
      ReleaseBands();
      BeginUpdate();
    
      for (int i = 0; i < bands.Count; i++)
      {
        REBARBANDINFO bandInfo = GetBandInfo(i, null);
        WindowsAPI.SendMessage(Handle, (int)RebarMessages.RB_INSERTBANDW, i, ref bandInfo);
      }

      UpdateSize();
      EndUpdate();
      CaptureBands();

      if ( bands.Count == 0 && addPlaceHolderToolBar )
      {
        if ( designerInTransaction == false )
        {
          // If the designer is engage in a transaction
          // don't add the place holder toolbar here, defer the
          // addition to the PaintBackground routine so that
          // we avoid some painting problems
          addPlaceHolderToolBar = false;
          AddPlaceHolder();
        }
      }

    }

    internal void UpdateBand(int index)
    {
      if (!IsHandleCreated) return;
        
      BeginUpdate();

      // Make sure we get the right index according to the band position in the rebar
      // and not to the index in the toolbar collections which can or cannot match the order
      // in the rebar control
      int actualIndex = GetBandActualIndex(index);

      REBARBANDINFO rbbi = GetBandInfo(actualIndex, null);
      ToolBarEx tb = (ToolBarEx)bands[actualIndex];
      int idealSize = tb.GetIdealSize();
      rbbi.cxIdeal = idealSize;
      WindowsAPI.SendMessage(Handle, (int)RebarMessages.RB_SETBANDINFOW, index, ref rbbi);

      UpdateSize();
      EndUpdate();
    }

    private int GetBandActualIndex(int bandIndex)
    {
      // This maps between the indexes in the band collection and the actual
      // indexes in the rebar that can actually change as the user moves
      // the bands around
      REBARBANDINFO rb;
      rb = GetRebarInfo(bandIndex);
      return rb.wID;
    }

    private void UpdateSize()
    {
      /*
      Height = WindowsAPI.SendMessage(Handle, (int)RebarMessages.RB_GETBARHEIGHT, 0, 0) + 1;
      if( Height != 1 )
      {
        // Give it some size
        RECT rc = new RECT();
        rc.bottom = Height;
        WindowsAPI.SendMessage( Handle, (int)RebarMessages.RB_SIZETORECT, 0, ref rc );
      }
      */
    }

    public int GetRebarHeight()
    {
      int height = WindowsAPI.SendMessage(Handle, (int)RebarMessages.RB_GETBARHEIGHT, 0, 0) + 1;
      return height;      
    }

    public Rectangle GetBandRect(int bandIndex)
    {
      RECT rect = new RECT();
      int index = GetBandActualIndex(bandIndex);
      WindowsAPI.SendMessage(Handle, (int)RebarMessages.RB_GETRECT, bandIndex, ref rect);
      return new Rectangle(rect.left, rect.top, rect.right-rect.left, rect.bottom-rect.top);
    }

    private REBARBANDINFO GetRebarInfo(int index)
    {
      REBARBANDINFO rbbi = new REBARBANDINFO();
      rbbi.cbSize = Marshal.SizeOf(typeof(REBARBANDINFO));
      rbbi.fMask = (int)(RebarInfoMask.RBBIM_ID|RebarInfoMask.RBBIM_IDEALSIZE);
      WindowsAPI.SendMessage(Handle, (int)RebarMessages.RB_GETBANDINFOW, index, ref rbbi);
      return rbbi;
    }
    
    private REBARBANDINFO GetBandInfo(int index, Control currentBand)
    {
      bool placeHolder = false;
      Control band;
      if ( currentBand != null )
      {
        placeHolder = true;
        band = currentBand;
      }
      else
        band = bands[index];
      REBARBANDINFO rbbi = new REBARBANDINFO();
      rbbi.cbSize = Marshal.SizeOf(typeof(REBARBANDINFO));
          
      if ( !IsCommonCtrl6() )
      {
        rbbi.fMask = (int)RebarInfoMask.RBBIM_COLORS;
        rbbi.clrBack = (int)ColorUtil.RGB( ColorUtil.VSNetControlColor );
        rbbi.clrFore = (int)ColorUtil.RGB( 255, 0, 255 );
      }

      rbbi.iImage = 0;
      rbbi.hbmBack = IntPtr.Zero;
      rbbi.lParam = 0;
      rbbi.cxHeader = 0;

      rbbi.fMask |= (int)RebarInfoMask.RBBIM_ID;
      rbbi.wID = index;
  
      if ((band.Text != null) && (band.Text != string.Empty))
      {
        rbbi.fMask |= (int)RebarInfoMask.RBBIM_TEXT;
        rbbi.lpText = Marshal.StringToHGlobalAnsi(band.Text);
        rbbi.cch = (band.Text == null) ? 0 : band.Text.Length;
      }
  
      rbbi.fMask |= (int)RebarInfoMask.RBBIM_STYLE;
      rbbi.fStyle = (int)(RebarStylesEx.RBBS_CHILDEDGE | RebarStylesEx.RBBS_FIXEDBMP);
      if ( placeHolder == false )
        rbbi.fStyle |= (int)RebarStylesEx.RBBS_GRIPPERALWAYS;

      ToolBarEx tb = (ToolBarEx)band;
      if ( tb.UseNewRow == true)
        rbbi.fStyle |= (int)(RebarStylesEx.RBBS_BREAK);
      rbbi.fStyle |= (band is IChevron) ? (int)RebarStylesEx.RBBS_USECHEVRON : 0;

      rbbi.fMask |= (int)(RebarInfoMask.RBBIM_CHILD);
      rbbi.hwndChild = band.Handle;
      
      rbbi.fMask |= (int)(RebarInfoMask.RBBIM_CHILDSIZE);
      rbbi.cyMinChild = band.Height;
      rbbi.cxMinChild = 0;
      rbbi.cyChild = 0;
      rbbi.cyMaxChild = 0; 
      rbbi.cyIntegral = 0;
      
      rbbi.fMask |= (int)(RebarInfoMask.RBBIM_SIZE);
      rbbi.cx = band.Width;
      rbbi.fMask |= (int)(RebarInfoMask.RBBIM_IDEALSIZE);
      rbbi.cxIdeal = band.Width;
      
      return rbbi;        
    }

    private void Bands_Changed(Object s, EventArgs e)
    {
      UpdateBands();
    }

    private void Band_HandleCreated(Object s, EventArgs e)
    {
      ReleaseBands();
        
      ToolBarEx band = (ToolBarEx) s;
      UpdateBand(bands.IndexOf(band));
      
      CaptureBands();
    }
    
    private void Band_TextChanged(Object s, EventArgs e)
    {
      ToolBarEx band = (ToolBarEx) s;
      UpdateBand(bands.IndexOf(band));
    }

    private void CaptureBands()
    {
      foreach (Control band in bands)
      {
        band.HandleCreated += new EventHandler(Band_HandleCreated);
        band.TextChanged += new EventHandler(Band_TextChanged);
      }
    }
    
    private void ReleaseBands()
    {
      foreach (Control band in bands)
      {
        band.HandleCreated -= new EventHandler(Band_HandleCreated);
        band.TextChanged -= new EventHandler(Band_TextChanged);
      }
    }
    #endregion

  }
}
