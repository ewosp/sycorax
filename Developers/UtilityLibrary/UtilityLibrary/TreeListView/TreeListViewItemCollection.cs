using System;
using System.Collections;
using System.Windows.Forms;

namespace UtilityLibrary.WinControls
{
  /// <summary>
  /// Collection of TreeListView item
  /// </summary>
  public class TreeListViewItemCollection : CollectionBase
  {
    /// <summary>
    /// Comparer for TreeListViewItem
    /// </summary>
    public class TreeListViewItemCollectionComparer : ITreeListViewItemComparer
    {
      #region Order Property
      private SortOrder _sortorder = SortOrder.Ascending;
      /// <summary>
      /// Sort order
      /// </summary>
      public SortOrder SortOrder
      {
        get{return(_sortorder);}
        set{_sortorder = value;}
      }
      private int _column;
      /// <summary>
      /// Column for the comparison
      /// </summary>
      public int Column
      {
        get{return(_column);}
        set{_column = value;}
      }
      #endregion
      #region Constructor
      /// <summary>
      /// Create a new instance of  Comparer
      /// </summary>
      public TreeListViewItemCollectionComparer() : this(SortOrder.Ascending, 0)
      {}
      /// <summary>
      /// Create a new instance of  Comparer
      /// </summary>
      /// <param name="order"></param>
      public TreeListViewItemCollectionComparer(SortOrder order) : this(order, 0)
      {
        SortOrder = order;
      }
      /// <summary>
      /// Create a new instance of  Comparer
      /// </summary>
      /// <param name="order"></param>
      /// <param name="column"></param>
      public TreeListViewItemCollectionComparer(SortOrder order, int column)
      {
        SortOrder = order;
        _column = column;
      }
      #endregion
      #region Compare
      /// <summary>
      /// Compare two TreeListViewItems
      /// </summary>
      /// <param name="x"></param>
      /// <param name="y"></param>
      /// <returns></returns>
      public int Compare(object x, object y)
      {
        TreeListViewItem a = (TreeListViewItem) x;
        TreeListViewItem b = (TreeListViewItem) y;
        int res = 0;
        try
        {
          res = string.CompareOrdinal(a.SubItems[Column].Text.ToUpper(), b.SubItems[Column].Text.ToUpper());
        }
        catch{}
        switch(SortOrder)
        {
          case SortOrder.Ascending:
            return(res);
          case SortOrder.Descending:
            return(-res);
          default:
            return(0);
        }
      }
      #endregion
    }
    #region Properties
    /// <summary>
    /// Transforms the collection to an array
    /// </summary>
    public TreeListViewItem[] Array
    {
      get
      {
        ReadWriteLock.AcquireReaderLock(-1);
        int size = this.Count;
        TreeListViewItem[] eltsArray = new TreeListViewItem[size];
        for(int i = 0 ; i < size ; i++)
          eltsArray[i] = this[i];
        ReadWriteLock.ReleaseReaderLock();
        return(eltsArray);
      }
    }
    /// <summary>
    /// Get or set the new sortorder (apply automatically the sort function
    /// if the sortorder value is changed)
    /// </summary>
    public SortOrder SortOrder
    {
      get{return(Comparer.SortOrder);}
      set
      {
        Comparer.SortOrder = value;
        Sort(false);}
    }
    private ITreeListViewItemComparer _comparer = new TreeListViewItemCollectionComparer(SortOrder.Ascending);
    /// <summary>
    /// Gets the comparer used in the Sort and Add functions
    /// </summary>
    public ITreeListViewItemComparer Comparer
    {
      get{return(_comparer);}
      set{_comparer = value;}
    }
    /// <summary>
    /// Set the new sortorder (apply automatically the sort function
    /// if the sortorder value is changed) for each collection recursively
    /// </summary>
    public SortOrder SortOrderRecursively
    {
      set
      {
        SortOrder = value;
        ReadWriteLock.AcquireReaderLock(-1);
        foreach(TreeListViewItem item in this) item.Items.SortOrderRecursively = value;
        ReadWriteLock.ReleaseReaderLock();}
    }
    internal SortOrder SortOrderRecursivelyWithoutSort
    {
      set
      {
        Comparer.SortOrder = value;
        ReadWriteLock.AcquireReaderLock(-1);
        foreach(TreeListViewItem item in this) item.Items.SortOrderRecursivelyWithoutSort = value;
        ReadWriteLock.ReleaseReaderLock();}
    }
    private System.Threading.ReaderWriterLock _readwritelock =
      new System.Threading.ReaderWriterLock();
    /// <summary>
    /// ReaderWriterLock
    /// </summary>
    public System.Threading.ReaderWriterLock ReadWriteLock
    {get{return(_readwritelock);}}
    private TreeListEx _owner;
    /// <summary>
    /// TreeListView control that directly contains this collection
    /// </summary>
    public TreeListEx Owner{get{return(_owner);}}
      
    private TreeListViewItem _parent;
    /// <summary>
    /// TreeListViewItem that contains this collection
    /// </summary>
    public TreeListViewItem Parent{get{return(_parent);}}
      
    /// <summary>
    /// Returns the TreeListView set in Owner or in Parent
    /// </summary>
    private TreeListEx TreeListView
    {
      get
      {
        if(Owner != null) return(Owner);
        if(Parent != null) return(Parent.ListView);
        return(null);
      }
    }
    /// <summary>
    /// Get an item in the collection
    /// </summary>
    public virtual TreeListViewItem this[int index]
    {
      get
      {
        return((TreeListViewItem) this.List[index]);}
    }
      
    #endregion

    #region Constructors
    /// <summary>
    /// Create a collection in the root of a TreeListView (first level items)
    /// </summary>
    /// <param name="owner"></param>
    public TreeListViewItemCollection(TreeListEx owner)
    {
      _owner = owner;
    }
    /// <summary>
    /// Create a collection within a TreeListViewItem
    /// </summary>
    /// <param name="parent"></param>
    public TreeListViewItemCollection(TreeListViewItem parent)
    {
      _parent = parent;
    }
    /// <summary>
    /// Create a free TreeListViewItemCollection (items will not be
    /// displayed in a TreeListView
    /// </summary>
    public TreeListViewItemCollection()
    {
    }
    #endregion

    #region Sort Functions
    /// <summary>
    /// Sort the items in this collection (recursively or not)
    /// </summary>
    /// <param name="recursively">Recursively</param>
    public void Sort(bool recursively)
    {
      ReadWriteLock.AcquireWriterLock(-1);
      // Gets an array of the items
      TreeListViewItem[] thisarray = Array;
      //      System.Array.Sort(thisarray, new TreeListViewItemCollectionComparer(order));
      // Removes the items
      this.Clear();
      // Adds the items
      foreach(TreeListViewItem item in thisarray)
        this.Add(item);
      if(recursively)
        foreach(TreeListViewItem item in thisarray)
          item.Items.Sort(true);
      ReadWriteLock.ReleaseWriterLock();
    }
    #endregion
    #region Add Function
    /// <summary>
    /// Returns true if this collection contains an item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(TreeListViewItem item)
    {
      bool res = false;
      ReadWriteLock.AcquireReaderLock(-1);
      foreach(TreeListViewItem elt in this)
        if(item == elt)
        {
          res = true;
          break;
        }
      ReadWriteLock.ReleaseReaderLock();
      return(res);
    }
    internal delegate ListViewItem InsertListViewItem(int index, ListViewItem item);
    /// <summary>
    /// Adds an item in the collection and in the TreeListView
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Index of the item in the collection</returns>
    public virtual int Add(TreeListViewItem item)
    {
      int index = GetInsertCollectionIndex(item);
      if(index == -1) return(-1);
      if(Parent != null) item.SetParent(Parent);
      item.Items.Comparer = this.Comparer;
      ReadWriteLock.AcquireWriterLock(-1);
      int treelistviewindex = GetInsertTreeListViewIndex(item);
      // Insert in the ListView
      if(treelistviewindex > -1)
      {
        ListView listview = (ListView) TreeListView;
        InsertListViewItem insert = new InsertListViewItem(listview.Items.Insert);
        if(listview.InvokeRequired)
          listview.Invoke(insert, new object[] {treelistviewindex, (ListViewItem) item});
        else
          listview.Items.Insert(treelistviewindex, (ListViewItem) item);
        if(item.IsExpanded) item.Expand();
        item.SetIndentation();
      }
      // Insert in this collection
      if(index > -1)
        List.Insert(index, item);
      ReadWriteLock.ReleaseWriterLock();
      return(index);
    }
    /// <summary>
    /// Adds an item in the collection and in the TreeListView
    /// </summary>
    /// <param name="value"></param>
    /// <param name="imageindex"></param>
    /// <returns></returns>
    public virtual TreeListViewItem Add(string value, int imageindex)
    {
      TreeListViewItem item = new TreeListViewItem(value, imageindex);
      this.Add(item);
      return(item);
    }
    /// <summary>
    /// Adds an item in the collection and in the TreeListView
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual TreeListViewItem Add(string value)
    {
      TreeListViewItem item = new TreeListViewItem(value);
      this.Add(item);
      return(item);
    }
    /// <summary>
    /// Adds a collection to this collection
    /// </summary>
    /// <param name="collection"></param>
    public void AddRange(TreeListViewItemCollection collection)
    {
      collection.ReadWriteLock.AcquireReaderLock(-1);
      foreach(TreeListViewItem item in collection)
        Add(item);
      collection.ReadWriteLock.ReleaseReaderLock();
    }
    #endregion
    #region Remove & Clear Functions
    /// <summary>
    /// Removes each node of the collection
    /// </summary>
    public new void Clear()
    {
      ReadWriteLock.AcquireWriterLock(-1);
      while(this.Count > 0) this[0].Remove();
      ReadWriteLock.ReleaseWriterLock();
    }
    /// <summary>
    /// Remove an item from the collection and the TreeListView
    /// </summary>
    /// <param name="item"></param>
    public virtual void Remove(TreeListViewItem item)
    {
      int index = -1;
      ReadWriteLock.AcquireWriterLock(-1);
      for(int i = 0 ; i < this.Count ; i++)
        if(this[i] == item) index = i;
      if(index > -1) RemoveAt(index);
      ReadWriteLock.ReleaseWriterLock();
    }
    internal delegate void RemoveListViewItem(ListViewItem item);
    /// <summary>
    /// Remove an item from the collection and the TreeListView
    /// </summary>
    /// <param name="index"></param>
    public new void RemoveAt(int index)
    {
      ReadWriteLock.AcquireWriterLock(-1);
      TreeListViewItem item = this[index];
      if(this[index].Visible && this.TreeListView != null)
      {
        ListView listview = (ListView) TreeListView;
        RemoveListViewItem remove = new RemoveListViewItem(listview.Items.Remove);
        if(listview.InvokeRequired)
          listview.Invoke(remove, new Object[] {(ListViewItem) item});
        else
          listview.Items.Remove((ListViewItem) item);
      }
      List.RemoveAt(index);
      item.SetParent(null);
      ReadWriteLock.ReleaseWriterLock();
    }
    #endregion
    #region Internal Functions
    private int GetInsertTreeListViewIndex(TreeListViewItem item)
    {
      if(TreeListView == null) return(-1);
      ReadWriteLock.AcquireReaderLock(-1);
      int collectionindex = GetInsertCollectionIndex(item);
      if(Owner != null)
      {
        int a = 0;
        a++;
      }
      int index = -1;
      // First level item (no parent)
      if(Owner != null && collectionindex != -1)
      {
        if(collectionindex == 0) index = 0;
        else index =
               this[collectionindex - 1].LastChildIndexInListView + 1;
      }
      else if(Parent != null && collectionindex != -1)
      {
        if(!Parent.Visible || !Parent.IsExpanded) index = -1;
        else
        {
          if(collectionindex == 0) index = Parent.Index + 1;
          else index =
                 Parent.Items[collectionindex - 1].LastChildIndexInListView + 1;
        }
      }
      ReadWriteLock.ReleaseReaderLock();
      return(index);
    }
    private int GetInsertCollectionIndex(TreeListViewItem item)
    {
      int index = -1;
      ReadWriteLock.AcquireReaderLock(-1);
      if(!this.Contains(item))
        switch(SortOrder)
        {
            // No sortorder -> at the end of the collection
          case System.Windows.Forms.SortOrder.None:
            index = this.Count;
            break;
          default:
            for(int i = 0 ; i < this.Count ; i++)
            {
              // Change the index for the compare if the order is descending
              int indexcompare = i;
              //                SortOrder == System.Windows.Forms.SortOrder.Ascending ?
              //              i : this.Count - (1 + i);
              int comp = Comparer.Compare(item, this[indexcompare]);
              if(comp <= 0)
              {
                index = indexcompare;
                break;
              }
            }
            index = index == -1 ? this.Count : index;
            break;
        }
      ReadWriteLock.ReleaseReaderLock();
      return(index);
    }
    #endregion
  }
}
