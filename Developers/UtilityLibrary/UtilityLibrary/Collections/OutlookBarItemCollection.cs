using System;
using System.Collections;
using System.Windows.Forms;
using UtilityLibrary.WinControls;

namespace UtilityLibrary.Collections
{
  /// <summary>
  /// Summary description for OutlookBarItemCollection.
  /// </summary>
  public class OutlookBarItemCollection : System.Collections.CollectionBase, IEnumerable
  {
    #region Class Internal declarations
    public class OutlookBarItemEventArgs : EventArgs
    {
      private OutlookBarItem m_item;

      public OutlookBarItem Item
      {
        get
        {
          return m_item;
        }
      }

      private OutlookBarItemEventArgs(){}
      public OutlookBarItemEventArgs( OutlookBarItem item )
      {
        m_item = item;
      }
    }

    public delegate void OutlookBarItemEventHandler( object sender, OutlookBarItemEventArgs e );
    #endregion

    #region Class Events
    public event OutlookBarItemEventHandler Changed;
    public event OutlookBarItemEventHandler OnItemAdded;
    public event OutlookBarItemEventHandler OnItemRemoved;
    public event OutlookBarItemEventHandler OnItemSet;
    public event EventHandler Cleared;
    #endregion

    #region Class properties
    public OutlookBarItem this[int index]
    {
      get 
      { 
        return InnerList[index] as OutlookBarItem ; 
      }
      set 
      {  
        List[ index ] = value; 
      }
    }
    #endregion

    #region IList methods overrides
    public int Add( OutlookBarItem item )
    {
      if( Contains( item ) == true ) return -1;
      return List.Add( item );
    }

    public bool Contains( OutlookBarItem item )
    {
      return InnerList.Contains( item );
    }
  
    public int IndexOf( OutlookBarItem item )
    {
      return InnerList.IndexOf(item);
    }
  
    public void Remove( OutlookBarItem item )
    {
      List.Remove( item );
    }
    
    new public void RemoveAt( int index )
    {
      List.RemoveAt( index );
    }

    public void Insert( int index, OutlookBarItem item )
    {
      List.Insert( index, item );
    }
    #endregion
    
    #region Class Event Fire
    protected void RaiseChangedEvent( OutlookBarItem item )
    {
      if( Changed != null )
      {
        Changed( this, new OutlookBarItemEventArgs( item ) );
      }
    }

    protected void RaiseOnItemAddedEvent( OutlookBarItem item )
    {
      if( OnItemAdded != null )
      {
        OnItemAdded( this, new OutlookBarItemEventArgs( item ) );
      }

      RaiseChangedEvent( item );
    }

    protected void RaiseOnItemRemovedEvent( OutlookBarItem item )
    {
      if( OnItemRemoved != null )
      {
        OnItemRemoved( this, new OutlookBarItemEventArgs( item ) );
      }

      RaiseChangedEvent( item );
    }
    
    protected void RaiseOnItemSetEvent( OutlookBarItem item )
    {
      if( OnItemSet != null )
      {
        OnItemSet( this, new OutlookBarItemEventArgs( item ) );
      }

      RaiseChangedEvent( item );
    }
    protected void RaiseClearedEvent()
    {
      if( Cleared != null )
      {
        Cleared( this, EventArgs.Empty );
      }

      RaiseChangedEvent( null );
    }
    #endregion

    #region Class overrides
    protected override void OnInsertComplete(int index, object value)
    {
      base.OnInsertComplete( index, value );
      RaiseOnItemAddedEvent( (OutlookBarItem)value );
    }

    protected override void OnRemoveComplete(int index, object value)
    {
      base.OnRemoveComplete( index, value );
      RaiseOnItemRemovedEvent( (OutlookBarItem)value );
    }

    protected override void OnSetComplete(int index, object oldValue, object newValue)
    {
      base.OnSetComplete( index, oldValue, newValue );
      RaiseOnItemSetEvent( (OutlookBarItem)newValue );
    }

    protected override void OnClearComplete()
    {
      base.OnClearComplete();
      RaiseClearedEvent();
    }
    #endregion
  }
}


