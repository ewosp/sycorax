using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace UtilityLibrary.WinControls
{
  /// <summary>
  /// This is a TreeList with columns
  /// </summary>
  public class TreeListEx : System.Windows.Forms.ListView
  {
    #region APIs
    [DllImport("user32.dll")]
    private static extern bool SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, ref 
      LVHITTESTINFO lParam);
    
    [DllImport("user32.dll")]
    private static extern bool SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, ref 
      IntPtr lParam);
    
    [StructLayoutAttribute(LayoutKind.Sequential)]
      private struct POINTAPI
    {
      public Int32 x;
      public Int32 y;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
      private struct LVHITTESTINFO
    {
      public POINTAPI pt;
      public Int32 flags;
      public Int32 iItem;
      public Int32 iSubItem;
    }
    #endregion
    
    #region Events, Delegates, and internal calls
    #region Delegates
    /// <summary>
    /// TreeListViewCancelEventHandler delegate
    /// </summary>
    public delegate void TreeListViewCancelEventHandler(object sender, TreeListViewCancelEventArgs e);
    /// <summary>
    /// TreeListViewEventHandler delegate
    /// </summary>
    public delegate void TreeListViewEventHandler(object sender, TreeListViewEventArgs e);
      #endregion

    #region Events
    /// <summary>
    /// Occurs before the tree node is collapsed
    /// </summary>
    [Description("Occurs before the tree node is collapsed")]
    public event TreeListViewCancelEventHandler BeforeExpand;
    /// <summary>
    /// Occurs before the tree node is collapsed
    /// </summary>
    [Description("Occurs before the tree node is collapsed")]
    public event TreeListViewCancelEventHandler BeforeCollapse;
    /// <summary>
    /// Occurs after the tree node is expanded
    /// </summary>
    [Description("Occurs after the tree node is expanded")]
    public event TreeListViewEventHandler AfterExpand;
    /// <summary>
    /// Occurs after the tree node is collapsed
    /// </summary>
    [Description("Occurs after the tree node is collapsed")]
    public event TreeListViewEventHandler AfterCollapse;
      #endregion
    
    #region Internal calls
    internal void RaiseBeforeExpand(TreeListViewCancelEventArgs e)
    {
      if(BeforeExpand != null)
        BeforeExpand(this, e);
    }
    internal void RaiseBeforeCollapse(TreeListViewCancelEventArgs e)
    {
      if(BeforeCollapse != null)
        BeforeCollapse(this, e);
    }
    internal void RaiseAfterExpand(TreeListViewEventArgs e)
    {
      if(AfterExpand != null)
        AfterExpand(this, e);
    }
    internal void RaiseAfterCollapse(TreeListViewEventArgs e)
    {
      if(AfterCollapse != null)
        AfterCollapse(this, e);
    }
      #endregion
    #endregion

    #region Modified properties
    /// <summary>
    /// Get or set the sort order
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Browsable(true), Description("Get or Set the sort order")]
    new public SortOrder Sorting
    {
      get{return(Items.SortOrder);}
      set{Items.SortOrderRecursively = value;}
    }
    private TreeListViewExpandMethod _expandmethod;
    /// <summary>
    /// Get or set the expand method
    /// </summary>
    [Browsable(true), Description("Get or Set the expand method")]
    public TreeListViewExpandMethod ExpandMethod
    {
      get{return(_expandmethod);}
      set{_expandmethod = value;}
    }
    private TreeListViewItemCollection _items;
    private System.Windows.Forms.ImageList imageList1;
    private System.ComponentModel.IContainer components;
    /// <summary>
    /// View (always Details)
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public new View View
    {
      get{return(base.View);}
      set{base.View = View.Details;}
    }
    /// <summary>
    /// Items
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false), Description("Items")]
    public new TreeListViewItemCollection Items
    {
      get{return(_items);}
    }
    /// <summary>
    /// Get currently selected items
    /// </summary>
    new public SelectedTreeListViewItemCollection SelectedItems
    {
      get
      {
        SelectedTreeListViewItemCollection sel = new SelectedTreeListViewItemCollection(this);
        return(sel);
      }
    }
    /// <summary>
    /// Get currently checked items
    /// </summary>
    [Browsable(false)]
    public new TreeListViewItem[] CheckedItems
    {
      get
      {
        TreeListViewItem[] array = new TreeListViewItem[base.CheckedIndices.Count];
        for(int i = 0 ; i < base.CheckedIndices.Count ; i++)
          array[i] = (TreeListViewItem) base.CheckedItems[i];
        return(array);
      }
    }
    /// <summary>
    /// Not supported (always false)
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    new public bool MultiSelect
    {
      get{return(false);}
      set{base.MultiSelect = false;}
    }
    #endregion
    
    #region Properties
    /// <summary>
    /// Get or set the comparer
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Browsable(true), Description("Get or Set the comparer")]
    public ITreeListViewItemComparer Comparer
    {
      get{return(Items.Comparer);}
      set{Items.Comparer = value;}
    }
    /// <summary>
    /// Get the current column wich the pointer points to
    /// </summary>
    public int HitTestColumn
    {
      get
      {
        LVHITTESTINFO hittest = new LVHITTESTINFO();
        hittest.pt.x = PointToClient(MousePosition).X;
        hittest.pt.y = PointToClient(MousePosition).Y;
        hittest.flags = (Int32) TreeListViewItem.ListViewMessages.LVHT_ONITEM;
        if(SendMessage(
          this.Handle,
          (Int32) TreeListViewItem.ListViewMessages.LVM_SUBITEMHITTEST,
          0,
          ref hittest))
          return(hittest.iSubItem);
        return(-1);
      }
    }
    #endregion
    
    #region WM_MESSAGE
    internal enum WM_MESSAGE
    {
      WM_APP = 32768,
      WM_ACTIVATE = 6,
      WM_ACTIVATEAPP = 28,
      WM_AFXFIRST = 864,
      WM_AFXLAST = 895,
      WM_ASKCBFORMATNAME = 780,
      WM_CANCELJOURNAL = 75,
      WM_CANCELMODE = 31,
      WM_CAPTURECHANGED = 533,
      WM_CHANGECBCHAIN = 781,
      WM_CHAR = 258,
      WM_CHARTOITEM = 47,
      WM_CHILDACTIVATE = 34,
      WM_CLEAR = 771,
      WM_CLOSE = 16,
      WM_COMMAND = 273,
      WM_COMMNOTIFY = 68,
      WM_COMPACTING = 65,
      WM_COMPAREITEM = 57,
      WM_CONTEXTMENU = 123,
      WM_COPY = 769,
      WM_COPYDATA = 74,
      WM_CREATE = 1,
      WM_CTLCOLORBTN = 309,
      WM_CTLCOLORDLG = 310,
      WM_CTLCOLOREDIT = 307,
      WM_CTLCOLORLISTBOX = 308,
      WM_CTLCOLORMSGBOX = 306,
      WM_CTLCOLORSCROLLBAR = 311,
      WM_CTLCOLORSTATIC = 312,
      WM_CUT = 768,
      WM_DEADCHAR = 259,
      WM_DELETEITEM = 45,
      WM_DESTROY = 2,
      WM_DESTROYCLIPBOARD = 775,
      WM_DEVICECHANGE = 537,
      WM_DEVMODECHANGE = 27,
      WM_DISPLAYCHANGE = 126,
      WM_DRAWCLIPBOARD = 776,
      WM_DRAWITEM = 43,
      WM_DROPFILES = 563,
      WM_ENABLE = 10,
      WM_ENDSESSION = 22,
      WM_ENTERIDLE = 289,
      WM_ENTERMENULOOP = 529,
      WM_ENTERSIZEMOVE = 561,
      WM_ERASEBKGND = 20,
      WM_EXITMENULOOP = 530,
      WM_EXITSIZEMOVE = 562,
      WM_FONTCHANGE = 29,
      WM_GETDLGCODE = 135,
      WM_GETFONT = 49,
      WM_GETHOTKEY = 51,
      WM_GETICON = 127,
      WM_GETMINMAXINFO = 36,
      WM_GETTEXT = 13,
      WM_GETTEXTLENGTH = 14,
      WM_HANDHELDFIRST = 856,
      WM_HANDHELDLAST = 863,
      WM_HELP = 83,
      WM_HOTKEY = 786,
      WM_HSCROLL = 276,
      WM_HSCROLLCLIPBOARD = 782,
      WM_ICONERASEBKGND = 39,
      WM_INITDIALOG = 272,
      WM_INITMENU = 278,
      WM_INITMENUPOPUP = 279,
      WM_INPUTLANGCHANGE = 81,
      WM_INPUTLANGCHANGEREQUEST = 80,
      WM_KEYDOWN = 256,
      WM_KEYUP = 257,
      WM_KILLFOCUS = 8,
      WM_MDIACTIVATE = 546,
      WM_MDICASCADE = 551,
      WM_MDICREATE = 544,
      WM_MDIDESTROY = 545,
      WM_MDIGETACTIVE = 553,
      WM_MDIICONARRANGE = 552,
      WM_MDIMAXIMIZE = 549,
      WM_MDINEXT = 548,
      WM_MDIREFRESHMENU = 564,
      WM_MDIRESTORE = 547,
      WM_MDISETMENU = 560,
      WM_MDITILE = 550,
      WM_MEASUREITEM = 44,
      WM_MENUCHAR = 288,
      WM_MENUSELECT = 287,
      WM_NEXTMENU = 531,
      WM_MOVE = 3,
      WM_MOVING = 534,
      WM_NCACTIVATE = 134,
      WM_NCCALCSIZE = 131,
      WM_NCCREATE = 129,
      WM_NCDESTROY = 130,
      WM_NCHITTEST = 132,
      WM_NCLBUTTONDBLCLK = 163,
      WM_NCLBUTTONDOWN = 161,
      WM_NCLBUTTONUP = 162,
      WM_NCMBUTTONDBLCLK = 169,
      WM_NCMBUTTONDOWN = 167,
      WM_NCMBUTTONUP = 168,
      WM_NCMOUSEMOVE = 160,
      WM_NCPAINT = 133,
      WM_NCRBUTTONDBLCLK = 166,
      WM_NCRBUTTONDOWN = 164,
      WM_NCRBUTTONUP = 165,
      WM_NEXTDLGCTL = 40,
      WM_NOTIFY = 78,
      WM_NOTIFYFORMAT = 85,
      WM_NULL = 0,
      WM_PAINT = 15,
      WM_PAINTCLIPBOARD = 777,
      WM_PAINTICON = 38,
      WM_PALETTECHANGED = 785,
      WM_PALETTEISCHANGING = 784,
      WM_PARENTNOTIFY = 528,
      WM_PASTE = 770,
      WM_PENWINFIRST = 896,
      WM_PENWINLAST = 911,
      WM_POWER = 72,
      WM_POWERBROADCAST = 536,
      WM_PRINT = 791,
      WM_PRINTCLIENT = 792,
      WM_QUERYDRAGICON = 55,
      WM_QUERYENDSESSION = 17,
      WM_QUERYNEWPALETTE = 783,
      WM_QUERYOPEN = 19,
      WM_QUEUESYNC = 35,
      WM_QUIT = 18,
      WM_RENDERALLFORMATS = 774,
      WM_RENDERFORMAT = 773,
      WM_SETCURSOR = 32,
      WM_SETFOCUS = 7,
      WM_SETFONT = 48,
      WM_SETHOTKEY = 50,
      WM_SETICON = 128,
      WM_SETREDRAW = 11,
      WM_SETTEXT = 12,
      WM_SETTINGCHANGE = 26,
      WM_SHOWWINDOW = 24,
      WM_SIZE = 5,
      WM_SIZECLIPBOARD = 779,
      WM_SIZING = 532,
      WM_SPOOLERSTATUS = 42,
      WM_STYLECHANGED = 125,
      WM_STYLECHANGING = 124,
      WM_SYSCHAR = 262,
      WM_SYSCOLORCHANGE = 21,
      WM_SYSCOMMAND = 274,
      WM_SYSDEADCHAR = 263,
      WM_SYSKEYDOWN = 260,
      WM_SYSKEYUP = 261,
      WM_TCARD = 82,
      WM_TIMECHANGE = 30,
      WM_TIMER = 275,
      WM_UNDO = 772,
      WM_USER = 1024,
      WM_USERCHANGED = 84,
      WM_VKEYTOITEM = 46,
      WM_VSCROLL = 277,
      WM_VSCROLLCLIPBOARD = 778,
      WM_WINDOWPOSCHANGED = 71,
      WM_WINDOWPOSCHANGING = 70,
      WM_WININICHANGE = 26,
      WM_KEYFIRST = 256,
      WM_KEYLAST = 264,
      WM_SYNCPAINT = 136,
      WM_MOUSEACTIVATE = 33,
      WM_MOUSEMOVE = 512,
      WM_LBUTTONDOWN = 513,
      WM_LBUTTONUP = 514,
      WM_LBUTTONDBLCLK = 515,
      WM_RBUTTONDOWN = 516,
      WM_RBUTTONUP = 517,
      WM_RBUTTONDBLCLK = 518,
      WM_MBUTTONDOWN = 519,
      WM_MBUTTONUP = 520,
      WM_MBUTTONDBLCLK = 521,
      WM_MOUSEWHEEL = 522,
      WM_MOUSEFIRST = 512,
      WM_MOUSELAST = 522,
      WM_MOUSEHOVER = 0x2A1,
      WM_MOUSELEAVE = 0x2A3,
      WM_SHNOTIFY = 0x0401
    }
    #endregion

    #region WndProc
    /// <summary>
    /// WndProc
    /// </summary>
    /// <param name="m"></param>
    protected override void WndProc(ref System.Windows.Forms.Message m)
    {
      TreeListViewItem item = null;
      switch((WM_MESSAGE) m.Msg)
      {
          // Disable this notification to remove the auto-check when
          // the user double-click on an item
        case WM_MESSAGE.WM_LBUTTONDBLCLK:
          if(this.SelectedIndices.Count > 0)
          {
            item = this.SelectedItems[0];
            bool doExpColl = false;
            Rectangle rec;
            switch(ExpandMethod)
            {
              case TreeListViewExpandMethod.IconDbleClick:
                rec = item.GetBounds(ItemBoundsPortion.Icon);
                if(rec.Contains(PointToClient(MousePosition))) doExpColl = true;
                break;
              case TreeListViewExpandMethod.ItemOnlyDbleClick:
                rec = item.GetBounds(ItemBoundsPortion.ItemOnly);
                if(rec.Contains(PointToClient(MousePosition))) doExpColl = true;
                break;
              case TreeListViewExpandMethod.EntireItemDbleClick:
                rec = item.GetBounds(ItemBoundsPortion.Entire);
                if(rec.Contains(PointToClient(MousePosition))) doExpColl = true;
                break;
              default:
                break;
            }
            if(doExpColl)
            {
              FindForm().Cursor = Cursors.WaitCursor;
              BeginUpdate();
              if(item.IsExpanded) item.Collapse();
              else item.Expand();
              EndUpdate();
              FindForm().Cursor = Cursors.Default;
            }
          }
          return;
        
        case WM_MESSAGE.WM_KEYDOWN:
          if(this.SelectedIndices.Count == 0) break;
          item = this.SelectedItems[0];
          if(item == null) break;
        switch((Keys)(int) m.WParam)
        {
          case Keys.Enter:
            break;
          case Keys.Left:
            this.BeginUpdate();
            if(item.IsExpanded)
              item.Collapse();
            else if(item.Parent != null)
            {
              item.Parent.Selected = true;
              item.Parent.Focused = true;
            }
            this.EndUpdate();
            break;
          case Keys.Right:
            if(item.Items.Count == 0) break;
            this.BeginUpdate();
            if(!item.IsExpanded)
              item.Expand();
            else
            {
              item.Items[item.Items.Count-1].Selected = true;
              item.Items[item.Items.Count-1].Focused = true;
            }
            this.EndUpdate();
            break;
        }
          return;
      }
      base.WndProc(ref m);
    }
    #endregion
    
    #region Class Initialization / Finalization
    
    public TreeListEx()
    {
      InitializeComponent();

      _items = new TreeListViewItemCollection(this);
      base.MultiSelect = false;

    }

    
    #endregion
    
    #region Functions
    private void TreeListView_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
    {
      TreeListViewItem item = (TreeListViewItem) base.Items[e.Index];
      item.Check(e.NewValue == CheckState.Checked);
    }
    /// <summary>
    /// Gets an item  at the specified coordinates
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    new public TreeListViewItem GetItemAt(int x, int y)
    {
      return((TreeListViewItem) base.GetItemAt(x, y));
    }
    /// <summary>
    /// Gets the TreeListViewItem from the ListView index of the item
    /// </summary>
    /// <param name="index">Index of the Item</param>
    /// <returns></returns>
    public TreeListViewItem GetTreeListViewItemFromIndex(int index)
    {
      if(base.Items.Count <= index + 1) return(null);
      return((TreeListViewItem) base.Items[index]);
    }
    // Sort the collection by calling the Text property
    private void TreeListView_AfterLabelEdit(object sender, System.Windows.Forms.LabelEditEventArgs e)
    {
      e.CancelEdit = true;
      if(e.Label == null || e.Label == "") return;
      TreeListViewItem item = (TreeListViewItem) base.Items[e.Item];
      item.Text = e.Label;
    }
    // Redraw indentation of the visible items
    private void TreeListView_VisibleChanged(object sender, System.EventArgs e)
    {
      if(this.Visible)
      {
        this.BeginUpdate();
        try
        {
          foreach(TreeListViewItem item in this.Items)
            item.RefreshIndentation(true);
        }
        catch{}
        this.EndUpdate();
      }
    }
    /// <summary>
    /// Not supported (use items.Sort)
    /// </summary>
    new public void Sort(){throw(new Exception("Not Supported"));}
    /// <summary>
    /// Nettoyage des ressources utilisées.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
        if( components != null )
          components.Dispose();
      }
      base.Dispose( disposing );
    }
    #endregion

    #region Component Designer generated code
    /// <summary>
    /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
    /// le contenu de cette méthode avec l'éditeur de code.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.imageList1 = new System.Windows.Forms.ImageList(this.components);
      // 
      // imageList1
      // 
      this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // TreeListView
      // 
      this.FullRowSelect = true;
      this.HideSelection = false;
      this.SmallImageList = this.imageList1;
      this.View = System.Windows.Forms.View.Details;
      this.VisibleChanged += new System.EventHandler(this.TreeListView_VisibleChanged);
      this.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.TreeListView_AfterLabelEdit);
      this.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.TreeListView_ColumnClick);
      this.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.TreeListView_ItemCheck);

    }
    #endregion

    #region Column Order
    
    /// <summary>
    /// Gets the order of a specified column
    /// </summary>
    /// <param name="columnindex"></param>
    /// <returns></returns>
    public int GetColumnOrder(int columnindex)
    {
      if ( this.Columns.Count == 0 ) return -1;
      
      if ( columnindex < 0 || columnindex > this.Columns.Count - 1 ) return -1;

      IntPtr[] colorderarray = new IntPtr[ this.Columns.Count ];

      SendMessage( this.Handle, ( int ) TreeListViewItem.ListViewMessages.LVM_GETCOLUMNORDERARRAY, 
        this.Columns.Count, ref colorderarray[ 0 ] );

      return ( int ) colorderarray[ columnindex ] ;
    }


    /// <summary>
    /// Gets the columns order
    /// </summary>
    /// <returns>Example {3,1,4,2}</returns>
    public int[] GetColumnsOrder()
    {
      if(this.Columns.Count == 0) return new int[] {};

      IntPtr[] colorderarray = new IntPtr[this.Columns.Count];

      try
      {
        SendMessage(this.Handle, (int) TreeListViewItem.ListViewMessages.LVM_GETCOLUMNORDERARRAY, this.Columns.Count, ref colorderarray[0]);
      }
      catch{}

      int[] colorderarrayint = new int[ this.Columns.Count ];

      for( int i = 0 ; i < this.Columns.Count ; i ++ )
        colorderarrayint[i] = ( int ) colorderarray[ i ];

      return colorderarrayint;
    }

    
    /// <summary>
    /// Indicates the column order (for example : {3,1,4,2})
    /// </summary>
    /// <param name="colorderarray"></param>
    public void SetColumnsOrder(int[] colorderarray)
    {
      if ( this.Columns.Count == 0 ) return;

      if ( colorderarray.Length != this.Columns.Count ) return;

      IntPtr[] colorderarrayintptr = new IntPtr[this.Columns.Count];

      for( int i = 0 ; i < this.Columns.Count ; i ++ )
        colorderarrayintptr[ i ] = ( IntPtr ) colorderarray[ i ];

      try
      {
        SendMessage( this.Handle, ( int ) TreeListViewItem.ListViewMessages.LVM_SETCOLUMNORDERARRAY, 
          this.Columns.Count, ref colorderarrayintptr[ 0 ] );
      }
      catch{}

      this.Refresh();
    }

    
    private void TreeListView_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
    {
      FindForm().Cursor = Cursors.WaitCursor;

      BeginUpdate();

      if ( Comparer.Column == e.Column )
        Sorting = Sorting == ( SortOrder.Ascending ) ? SortOrder.Descending : SortOrder.Ascending;
      else
      {
        Comparer.Column = e.Column;
        Items.SortOrderRecursivelyWithoutSort = SortOrder.Ascending;
        Items.Sort( true );
      }
      
      EndUpdate();
      
      FindForm().Cursor = Cursors.Default;
    }
    
    
    /// <summary>
    /// Indicates the column order (for example : "3142")
    /// </summary>
    /// <param name="colorder"></param>
    public void SetColumnsOrder(string colorder)
    {
      if(colorder == null) return;
      int[] colorderarray = new int[colorder.Length];
      for(int i = 0 ; i < colorder.Length ; i++)
        colorderarray[i] = int.Parse(new String(colorder[i], 1));
      SetColumnsOrder(colorderarray);
    }
    
    
    #endregion
  }
  
  #region Addition Classes
  /// <summary>
  /// Arguments of a TreeListViewEvent
  /// </summary>
  public class TreeListViewEventArgs : EventArgs
  {
    private TreeListViewItem _item;
    /// <summary>
    /// Item that will be expanded
    /// </summary>
    public TreeListViewItem Item{get{return(_item);}}
    private TreeListViewAction _action;
    /// <summary>
    /// Action returned by the event
    /// </summary>
    public TreeListViewAction Action{get{return(_action);}}
    /// <summary>
    /// Create a new instance of TreeListViewEvent arguments
    /// </summary>
    /// <param name="item"></param>
    /// <param name="action"></param>
    public TreeListViewEventArgs(TreeListViewItem item, TreeListViewAction action)
    {
      _item = item;
      _action = action;
    }
  }

  
  /// <summary>
  /// Arguments of a TreeListViewCancelEventArgs
  /// </summary>
  
  public class TreeListViewCancelEventArgs : TreeListViewEventArgs
  {
    private bool _cancel = false;
    /// <summary>
    /// True -> the operation is canceled
    /// </summary>
    public bool Cancel
    {
      get{return(_cancel);}
      set{_cancel = value;}
    }
    /// <summary>
    /// Create a new instance of TreeListViewCancelEvent arguments
    /// </summary>
    /// <param name="item"></param>
    /// <param name="action"></param>
    public TreeListViewCancelEventArgs(TreeListViewItem item, TreeListViewAction action) :
      base(item, action)
    {}
  }

  
  /// <summary>
  /// TreeListView actions
  /// </summary>
  [Serializable]
  public enum TreeListViewAction
  {
    /// <summary>
    /// By Keyboard
    /// </summary>
    ByKeyboard,
    /// <summary>
    /// ByMouse
    /// </summary>
    ByMouse,
    /// <summary>
    /// Collapse
    /// </summary>
    Collapse,
    /// <summary>
    /// Expand
    /// </summary>
    Expand,
    /// <summary>
    /// Unknown
    /// </summary>
    Unknown
  }
  
 
  /// <summary>
  /// Expand / Collapse method
  /// </summary>
  [Serializable]
  public enum TreeListViewExpandMethod
  {
    /// <summary>
    /// Expand when double clicking on the icon
    /// </summary>
    IconDbleClick,
    /// <summary>
    /// Expand when double clicking on the entire item
    /// </summary>
    EntireItemDbleClick,
    /// <summary>
    /// Expand when double clicking on the item only
    /// </summary>
    ItemOnlyDbleClick,
    /// <summary>
    /// None
    /// </summary>
    None
  }

  #endregion
}
