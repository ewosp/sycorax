using System;
using System.Collections;
using System.Windows.Forms;
using UtilityLibrary.WinControls;

namespace UtilityLibrary.Collections
{
  public class OutlookBarBandCollection  : System.Collections.CollectionBase, IEnumerable
  {
    #region Class internal Declarations
    public class OutlookBarBandEventArgs : EventArgs
    {
      private OutlookBarBand m_item;

      public OutlookBarBand Band
      {
        get
        {
          return m_item;
        }
      }

      private OutlookBarBandEventArgs(){}
      public OutlookBarBandEventArgs( OutlookBarBand item )
      {
        m_item = item;
      }
    }

    public delegate void OutlookBarBandEventHandler( object sender, OutlookBarBandEventArgs e );
    #endregion

    #region Class Events
    public event OutlookBarBandEventHandler Changed;
    public event OutlookBarBandEventHandler OnItemAdded;
    public event OutlookBarBandEventHandler OnItemRemoved;
    public event OutlookBarBandEventHandler OnItemSet;
    public event EventHandler Cleared;
    #endregion

    #region Class Constructor
    public OutlookBarBandCollection(){}
    #endregion

    #region IList Methods
    public int Add( OutlookBarBand band )
    {
      if( Contains( band ) ) return -1;
      return List.Add(band);
    }

    public bool Contains(OutlookBarBand band)
    {
      return InnerList.Contains( band );
    }
  
    public int IndexOf( OutlookBarBand band )
    {
      return InnerList.IndexOf(band);
    }
  
    public void Remove( OutlookBarBand band )
    {
      List.Remove( band );
    }
    
    public OutlookBarBand this[ int index ]
    {
      get 
      { 
        return InnerList[ index ] as OutlookBarBand; 
      }
      set
      {
        List[ index ] = value;
      }
    }
    #endregion
  
    #region Class Event Fire
    protected void RaiseChangedEvent( OutlookBarBand item )
    {
      if( Changed != null )
      {
        Changed( this, new OutlookBarBandEventArgs( item ) );
      }
    }

    protected void RaiseOnItemAddedEvent( OutlookBarBand item )
    {
      if( OnItemAdded != null )
      {
        OnItemAdded( this, new OutlookBarBandEventArgs( item ) );
      }

      RaiseChangedEvent( item );
    }

    protected void RaiseOnItemRemovedEvent( OutlookBarBand item )
    {
      if( OnItemRemoved != null )
      {
        OnItemRemoved( this, new OutlookBarBandEventArgs( item ) );
      }

      RaiseChangedEvent( item );
    }
    
    protected void RaiseOnItemSetEvent( OutlookBarBand item )
    {
      if( OnItemSet != null )
      {
        OnItemSet( this, new OutlookBarBandEventArgs( item ) );
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
      RaiseOnItemAddedEvent( (OutlookBarBand)value );
    }

    protected override void OnRemoveComplete(int index, object value)
    {
      RaiseOnItemRemovedEvent( (OutlookBarBand)value );
      base.OnRemoveComplete( index, value );
    }

    protected override void OnSetComplete(int index, object oldValue, object newValue)
    {
      base.OnSetComplete( index, oldValue, newValue );
      RaiseOnItemSetEvent( (OutlookBarBand)newValue );
    }

    protected override void OnClearComplete()
    {
      base.OnClearComplete();
      RaiseClearedEvent();
    }
    #endregion
  }
}
