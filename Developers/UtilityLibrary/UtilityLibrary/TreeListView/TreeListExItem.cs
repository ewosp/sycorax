using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace UtilityLibrary.WinControls
{
  /// <summary>
  /// TreeListViewItem
  /// </summary>
  public class TreeListViewItem : ListViewItem
  {
    #region Events
    /// <summary>
    /// TreeListViewItemHandler delegate
    /// </summary>
    public delegate void TreeListViewItemHanlder(object sender);
    /// <summary>
    /// TreeListViewItemCheckedHandler delegate
    /// </summary>
    public delegate void TreeListViewItemCheckedHandler(object sender, bool ischecked);
    /// <summary>
    /// Occurs after the tree node is collapsed
    /// </summary>
    public event TreeListViewItemHanlder AfterCollapse;
    /// <summary>
    /// Occurs after the tree node is expanded
    /// </summary>
    public event TreeListViewItemHanlder AfterExpand;
    /// <summary>
    /// Occurs after the tree node is checked
    /// </summary>
    public event TreeListViewItemCheckedHandler AfterCheck;
    #endregion

    #region APIs
    [DllImport("user32.dll")]
    internal static extern bool SendMessage(IntPtr hWnd, ListViewMessages msg,
      Int32 wParam, ref LV_ITEM lParam);
    [StructLayoutAttribute(LayoutKind.Sequential)]
      internal struct LV_ITEM
    {
      public ListViewMessages mask;
      public Int32 iItem;
      public Int32 iSubItem;
      public UInt32 state;
      public UInt32 stateMask;
      public String pszText;
      public Int32 cchTextMax;
      public Int32 iImage;
      public IntPtr lParam;
      public Int32 iIndent;
    }
    internal enum ListViewMessages : int
    {
      LVM_FIRST       = 0x1000,
      LVM_GETITEM       = LVM_FIRST + 75,
      LVM_SETITEM       = LVM_FIRST + 76,
      LVIF_INDENT       = 0x0010,
      LVIF_TEXT       = 0x0001,
      LVM_GETITEMTEXTA    = LVM_FIRST + 45,
      LVM_GETITEMTEXT     = LVM_GETITEMTEXTA,
      LVM_SETCOLUMNWIDTH    = LVM_FIRST + 30,
      LVSCW_AUTOSIZE      = -1,
      LVSCW_AUTOSIZE_USEHEADER= -2,
      LVM_SETITEMSTATE    = LVM_FIRST + 43,
      LVM_INSERTITEMA     = LVM_FIRST + 77,
      LVM_DELETEITEM      = LVM_FIRST + 8,
      LVM_GETITEMCOUNT    = LVM_FIRST + 4,
      LVM_GETSUBITEMRECT    = LVM_FIRST + 56,
      LVM_SUBITEMHITTEST    = LVM_FIRST + 57,
      LVHT_ONITEMICON     = 0x0002,
      LVHT_ONITEMLABEL    = 0x0004,
      LVHT_ONITEMSTATEICON  = 0x0008,
      LVHT_ONITEM       = LVHT_ONITEMICON | LVHT_ONITEMLABEL | LVHT_ONITEMSTATEICON,
      LVIR_BOUNDS       = 0,
      LVIR_ICON       = 1,
      LVIR_LABEL        = 2,
      LVM_GETCOLUMN     = LVM_FIRST + 25,
      LVM_SETCOLUMN     = LVM_FIRST + 26,
      LVM_GETCOLUMNORDERARRAY = LVM_FIRST + 59,
      LVM_SETCOLUMNORDERARRAY = LVM_FIRST + 58,
      LVM_SETITEMTEXT     = LVM_FIRST + 116
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets if this item is checked
    /// </summary>
    new public bool Checked
    {
      get{return(base.Checked);}
      set
      {
        base.Checked = value;
        if(AfterCheck != null) AfterCheck(this, value);}
    }
    /// <summary>
    /// Gets a collection of the parent of this item
    /// </summary>
    public TreeListViewItemCollection ParentsInHierarch
    {
      get
      {
        TreeListViewItemCollection temp = (this.Parent == null ? new TreeListViewItemCollection() : temp = this.Parent.ParentsInHierarch);
        if(this.Parent != null)
          temp.Add(this.Parent);
        return(temp);
      }
    }
    /// <summary>
    /// Gets the fullpath of an item (Parents.Text + \ + this.Text)
    /// </summary>
    public string FullPath
    {
      get
      {
        if(this.Parent != null)
        {
          string strPath = this.Parent.FullPath + @"\" + this.Text;
          return(strPath.Replace(@"\\", @"\"));
        }
        else
          return(this.Text);
      }
    }
    /// <summary>
    /// Get or Set the Text property
    /// </summary>
    new public string Text
    {
      get
      {
        return base.Text;
      }
      set
      {
        base.Text = value;
        TreeListViewItemCollection collection = this.Container;
        if ( collection != null )
          collection.Sort( false );
      }
    }
    /// <summary>
    /// Get the collection that contains this item
    /// </summary>
    public TreeListViewItemCollection Container
    {
      get
      {
        if(Parent != null) return(Parent.Items);
        if(ListView != null) return(ListView.Items);
        return(null);
      }
    }
    /// <summary>
    /// Get the biggest index in the listview of the visible childs of this item
    /// including this item
    /// </summary>
    public int LastChildIndexInListView
    {
      get
      {
        int index = this.Index;
        int temp;
        Items.ReadWriteLock.AcquireReaderLock(-1);
        foreach(TreeListViewItem item in Items)
          if(item.Visible)
          {
            temp = item.LastChildIndexInListView;
            if(temp > index) index = temp;
          }
        Items.ReadWriteLock.ReleaseReaderLock();
        return(index);
      }
    }
    private bool _isexpanded;
    /// <summary>
    /// Returns true if this item is expanded
    /// </summary>
    public bool IsExpanded{get{return(_isexpanded);}}
    /// <summary>
    /// Get the level of the item in the treelistview
    /// </summary>
    public int Level
    {
      get
      {
        return(this.Parent == null ? 0 : this.Parent.Level + 1);
      }
    }
    
    private TreeListViewItemCollection _items;
    /// <summary>
    /// Get the items contained in this item
    /// </summary>
    public TreeListViewItemCollection Items{get{return(_items);}}
    private TreeListViewItem _parent;
    /// <summary>
    /// Get the parent of this item
    /// </summary>
    public TreeListViewItem Parent{get{return(_parent);}}
    /// <summary>
    /// Get the TreeListView containing this item
    /// </summary>
    public new TreeListEx ListView{get{return((TreeListEx) base.ListView);}}
    /// <summary>
    /// Returns true if this item is visible in the TreeListView
    /// </summary>
    public bool Visible{get{return(base.Index > -1);}}
    #endregion

    #region Constructors
    /// <summary>
    /// Create a new instance of a TreeListViewItem
    /// </summary>
    public TreeListViewItem()
    {
      _items = new TreeListViewItemCollection(this);
    }
    /// <summary>
    /// Create a new instance of a TreeListViewItem
    /// </summary>
    public TreeListViewItem(string value) : this()
    {
      this.Text = value;
    }
    /// <summary>
    /// Create a new instance of a TreeListViewItem
    /// </summary>
    public TreeListViewItem(string value, int imageindex) : this(value)
    {
      this.ImageIndex = imageindex;
    }
    #endregion

    #region Functions
    internal void SetParent(TreeListViewItem parent)
    {
      _parent = parent;
    }
    /// <summary>
    /// Remove this item from its associated collection
    /// </summary>
    public new void Remove()
    {
      TreeListViewItemCollection collection = Container;
      if(collection != null) collection.Remove(this);
    }
    /// <summary>
    /// Check if this node is one of the parents of an item (recursively)
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool IsAParentOf(TreeListViewItem item)
    {
      TreeListViewItemCollection parents = item.ParentsInHierarch;
      foreach(TreeListViewItem parent in parents)
        if(parent == this) return(true);
      return(false);
    }
    /// <summary>
    /// Ensure that the node is visible
    /// </summary>
    new public void EnsureVisible()
    {
      if(Parent != null)
      {
        if(!Parent._isexpanded)
          Parent.Expand();
        Parent.EnsureVisible();
      }
      base.EnsureVisible();
    }
    #endregion

    #region Indentation
    /// <summary>
    /// Set the indentation using the level of this item
    /// </summary>
    public void SetIndentation()
    {
      if(this.ListView == null) return;
      LV_ITEM lvi = new LV_ITEM();
      lvi.iItem = this.Index;
      lvi.iIndent = this.Level;
      lvi.mask = ListViewMessages.LVIF_INDENT;
      SendMessage(
        this.ListView.Handle,
        ListViewMessages.LVM_SETITEM,
        0,
        ref lvi);
    }
    /// <summary>
    /// Refresh indentation of this item and of its children (recursively)
    /// </summary>
    /// <param name="recursively">Recursively</param>
    public void RefreshIndentation(bool recursively)
    {
      if(this.Visible)
      {
        SetIndentation();
        if(recursively)
        {
          try
          {
            foreach(TreeListViewItem item in this.Items)
              item.RefreshIndentation(true);
          }
          catch{}
        }
      }
    }
    #endregion

    #region Expand / Collapse
    /// <summary>
    /// Expand
    /// </summary>
    public void Expand()
    {
      // The item wasn't expanded -> raise an event
      if(Visible && !_isexpanded && ListView != null)
      {
        TreeListViewCancelEventArgs e = new TreeListViewCancelEventArgs(
          this, TreeListViewAction.Expand);
        ListView.RaiseBeforeExpand(e);
        if(e.Cancel) return;
      }
      if(this.Visible)
      {
        Items.ReadWriteLock.AcquireReaderLock(-1);
        for(int i = Items.Count - 1 ; i >= 0 ;i--)
        {
          TreeListViewItem item = this.Items[i];
          if(!item.Visible)
          {
            ListView LView = this.ListView;
            LView.Items.Insert(
              this.Index + 1,
              item);
            item.SetIndentation();
          }
          if(item.IsExpanded)
            item.Expand();
        }
        Items.ReadWriteLock.ReleaseReaderLock();
      }
      // The item wasn't expanded -> raise an event
      if(Visible && !_isexpanded && ListView != null)
      {
        this._isexpanded = true;
        TreeListViewEventArgs e = new TreeListViewEventArgs(
          this, TreeListViewAction.Expand);
        ListView.RaiseAfterExpand(e);
        if(AfterExpand != null) AfterExpand(this);
      }
      this._isexpanded = true;
    }
    /// <summary>
    /// Expand all sub nodes
    /// </summary>
    public void ExpandAll()
    {
      this.Expand();
      // Expand canceled -> stop expandall for the children of this item
      if(!this.IsExpanded) return;
      Items.ReadWriteLock.AcquireReaderLock(-1);
      for(int i = 0 ; i < this.Items.Count ; i++)
        this.Items[i].ExpandAll();
      Items.ReadWriteLock.ReleaseReaderLock();
    }
    /// <summary>
    /// Collapse
    /// </summary>
    public void Collapse()
    {
      // The item was expanded -> raise an event
      if(Visible && _isexpanded && ListView != null)
      {
        TreeListViewCancelEventArgs e = new TreeListViewCancelEventArgs(
          this, TreeListViewAction.Collapse);
        ListView.RaiseBeforeCollapse(e);
        if(e.Cancel) return;
      }

      // Collapse
      Items.ReadWriteLock.AcquireReaderLock(-1);
      if(this.Visible)
        foreach(TreeListViewItem item in Items)
          item.Hide();
      Items.ReadWriteLock.ReleaseReaderLock();
      
      // The item was expanded -> raise an event
      if(Visible && _isexpanded && ListView != null)
      {
        this._isexpanded = false;
        TreeListViewEventArgs e = new TreeListViewEventArgs(
          this, TreeListViewAction.Collapse);
        ListView.RaiseAfterCollapse(e);
        if(AfterCollapse != null) AfterCollapse(this);
      }
      this._isexpanded = false;
    }
    /// <summary>
    /// Collapse all sub nodes
    /// </summary>
    public void CollapseAll()
    {
      this.Collapse();
      Items.ReadWriteLock.AcquireReaderLock(-1);
      foreach(TreeListViewItem item in this.Items)
        item.CollapseAll();
      Items.ReadWriteLock.ReleaseReaderLock();
    }
    /// <summary>
    /// Hide this node (remove from TreeListView but
    /// not from associated Parent items)
    /// </summary>
    private void Hide()
    {
      if(this.Visible) base.Remove();
      Items.ReadWriteLock.AcquireReaderLock(-1);
      foreach(TreeListViewItem item in Items) item.Hide();
      Items.ReadWriteLock.ReleaseReaderLock();
    }
    #endregion

    /// <summary>
    /// Check
    /// </summary>
    /// <param name="ischecked"></param>
    public void Check(bool ischecked)
    {
      Items.ReadWriteLock.AcquireReaderLock(-1);
      foreach(TreeListViewItem item in Items)
      {
        item.Check(ischecked);
        item.Checked = ischecked;
      }
      Items.ReadWriteLock.ReleaseReaderLock();
    }
  }
}
