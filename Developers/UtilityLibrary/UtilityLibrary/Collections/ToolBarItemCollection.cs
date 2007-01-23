using System;
using System.Diagnostics;
using System.Collections;
using System.Windows.Forms;

using UtilityLibrary.CommandBars;

namespace UtilityLibrary.Collections
{
  /// <summary>
  /// Summary description for ToolbarButtonCollection.
  /// </summary>
  public class ToolBarItemCollection : System.Collections.CollectionBase, IEnumerable
  {
    #region Class Events
    public event EventHandler Changed;
    public event EventHandler ItemAdded;
    public event EventHandler ItemRemoved;
    public event EventHandler ItemSetted;
    public event EventHandler OnCleared;
    #endregion

    #region Class Variable
    private ToolBarEx parentToolBar;
    #endregion

    #region Class Constructors
    public ToolBarItemCollection()
    {

    }
    public ToolBarItemCollection( ToolBarEx parent )
    {
      parentToolBar = parent;
    }

    #endregion
      
    #region IList Methods
    public void Add( ToolBarItem item )
    {
      RemovePlaceHolder();
      
      List.Add( item );
    }

    public void AddRange( ToolBarItem[] array )
    {
      RemovePlaceHolder();

      foreach( ToolBarItem item in array )
      {
        List.Add( item );
      }
    }

    public void Insert( int index, ToolBarItem item )
    {
      RemovePlaceHolder();
      List.Insert( index, item );
    }

    public void Remove( ToolBarItem item )
    {
      // Remove the item from the ToolBar first
      int index = IndexOf( item );
      
      if( parentToolBar != null )
      {
        parentToolBar.RemoveToolBarItem( index );
      }
      
      if( !InnerList.Contains( item ) ) return;

      List.Remove( item );

      // If we don't have any items left
      if ( InnerList.Count == 0 && parentToolBar.PlaceHolderAdded == false )
      {
        parentToolBar.AddPlaceHolderToolBarItem();
      }
    }

    public bool Contains( ToolBarItem item )
    {
      return InnerList.Contains( item );
    }

    public int  IndexOf( ToolBarItem item )
    {
      return InnerList.IndexOf(item);
    }

    
    public   ToolBarItem this[int index]
    {
      get 
      { 
        return (ToolBarItem)List[ index ]; 
      }
    }

    internal ToolBarItem this[Keys shortcut]
    {
      get
      {
        foreach (ToolBarItem item in InnerList)
        {
          if( ( item.Shortcut == shortcut ) && ( item.Enabled ) && ( item.Visible ) )
          {
            return item;
          }
        }

        return null;
      }
    }

    internal ToolBarItem this[char mnemonic]
    {
      get
      {
        string text;
        char mnemo = Char.ToUpper( mnemonic );

        foreach( ToolBarItem item in InnerList )
        {
          text = item.Text;
          
          if( text != string.Empty && text != null )
          {          
            int index = text.IndexOf( '&' );
            if( index >= 0 && index < text.Length-1 )
            {
              if( mnemo == Char.ToUpper( text[ index+1 ] ) )
              {
                return item;
              }
            }
          }
        }

        return null;
      }
    }
    #endregion
      
    #region Internals Implementation
    private void RemovePlaceHolder()
    {
      if( parentToolBar != null && parentToolBar.PlaceHolderAdded == true )
      {
        parentToolBar.RemovePlaceHolderToolBarItem();
      }
    }

    internal void RaiseChanged()
    {
      if( Changed != null )
      {
        Changed( this, EventArgs.Empty );
      }
    }

    internal void RaiseItemAdded()
    {
      if( ItemAdded != null )
      {
        ItemAdded( this, EventArgs.Empty );
      }
      
      RaiseChanged();
    }

    internal void RaiseItemRemoved()
    {
      if( ItemRemoved != null )
      {
        ItemRemoved( this, EventArgs.Empty );
      }

      RaiseChanged();
    }

    internal void RaiseItemSetted()
    {
      if( ItemSetted != null )
      {
        ItemSetted( this, EventArgs.Empty );
      }
    
      RaiseChanged();
    }

    internal void RaiseOnCleared()
    {
      if( OnCleared != null )
      {
        OnCleared( this, EventArgs.Empty );
      }
      
      RaiseChanged();
    }
    #endregion

    #region Events Catchers
    protected override void OnInsertComplete(int index, object value)
    {
      base.OnInsert( index, value );

      RaiseItemAdded();
    }

    protected override void OnRemoveComplete(int index, object value)
    {
      base.OnRemoveComplete( index, value );

      RaiseItemRemoved();
    }

    protected override void OnSetComplete(int index, object oldValue, object newValue)
    {
      base.OnSetComplete( index, oldValue, newValue );
      
      RaiseItemSetted();
    }

    protected override void OnClearComplete()
    {
      RaiseOnCleared();
    }

    protected override void OnClear()
    {
      // Before wiping out all items from the ToolBarItem collection make sure we also
      // delete the items from the toolBar itself
      for( int i = 0; i < InnerList.Count; i++ )
      {
        if ( parentToolBar != null )
        {
          parentToolBar.RemoveToolBarItem(i);
        }
      }

      base.OnClear();

      // We don't have any bands left, add
      // the ToolBar place holder
      if ( InnerList.Count == 0 && parentToolBar.PlaceHolderAdded == false )
      {
        parentToolBar.AddPlaceHolderToolBarItem();
      }
    }
    #endregion
  }
}
