using System;
using System.Collections;


namespace UtilityLibrary.Collections
{
  #region Class delegates
  public delegate void CollectionClear();
  public delegate void CollectionChange( int index, object value );
  public delegate void CollectionSet( int index, object old, object value );
  #endregion


  public class CollectionWithEvents : CollectionBase
  {
    #region Class members
    /// <summary>
    /// TRUE - skip all event raising code in class.
    /// </summary>
    private bool m_bSkipEvents;
    #endregion

    #region Class events
    /// <summary>
    /// Something canged in collection
    /// </summary>
    public event EventHandler     Changed;
    /// <summary>
    /// Raised by class before real cleaning of collection
    /// </summary>
    public event CollectionClear  Clearing;
    /// <summary>
    /// Raised by class after collection clean process
    /// </summary>
    public event CollectionClear  Cleared;
    /// <summary>
    /// Raised by class before Item will be added into collection
    /// </summary>
    public event CollectionChange Inserting;
    /// <summary>
    /// Raised by class after Item add into collection
    /// </summary>
    public event CollectionChange Inserted;
    /// <summary>
    /// Raised by class before real item removing from collection.
    /// </summary>
    public event CollectionChange Removing;
    /// <summary>
    /// Raised by class after item remove from collection storage
    /// </summary>
    public event CollectionChange Removed;
    /// <summary>
    /// Raised by class before item replace in collection. 
    /// </summary>
    public event CollectionSet    Setting;
    /// <summary>
    /// Raised by class after item replace in collection. 
    /// </summary>
    public event CollectionSet    Setted;
    #endregion

    #region Class Properties
    /// <summary>
    /// GET/SET can class raise events or not
    /// </summary>
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
        }
      }
    }
    #endregion
  
    #region Class overrides
    private void RaiseChangedEvent()
    {
      if( Changed != null && m_bSkipEvents == false )
      {
        Changed( this, EventArgs.Empty );
      }
    }

    
    protected override void OnClear()
    {
      // Any attached event handlers?
      if( Clearing != null && m_bSkipEvents == false )
      {
        // Raise event to notify all contents removed
        Clearing();
      }
      
      base.OnClear();
    } 

    protected override void OnClearComplete()
    {
      // Any attached event handlers?
      if( Cleared != null && m_bSkipEvents == false )
      {
        // Raise event to notify all contents removed
        Cleared();
      }

      base.OnClearComplete();

      RaiseChangedEvent();
    } 

    
    protected override void OnInsert( int index, object value )
    {
      // Any attached event handlers?
      if( Inserting != null && m_bSkipEvents == false )
      {
        // Raise event to notify new content added
        Inserting( index, value );
      }
      
      base.OnInsert( index, value );
    }

    protected override void OnInsertComplete( int index, object value )
    {
      // Any attached event handlers?
      if( Inserted != null && m_bSkipEvents == false )
      {
        // Raise event to notify new content added
        Inserted( index, value );
      }

      base.OnInsertComplete( index, value );

      RaiseChangedEvent();
    }

    
    protected override void OnRemove( int index, object value )
    {
      // Any attached event handlers?
      if( Removing != null && m_bSkipEvents == false )
      {
        // Raise event to notify content has been removed
        Removing( index, value );
      }

      base.OnRemove( index, value );
    }

    protected override void OnRemoveComplete( int index, object value )
    {
      // Any attached event handlers?
      if( Removed != null && m_bSkipEvents == false )
      {
        // Raise event to notify content has been removed
        Removed( index, value );
      }

      base.OnRemoveComplete( index, value );

      RaiseChangedEvent();
    }
    
    
    protected override void OnSet( int index, object oldValue, object newValue )
    {
      if( Setting != null && m_bSkipEvents == false )
      {
        Setting( index, oldValue, newValue );
      }

      base.OnSet( index, oldValue, newValue );
    }

    protected override void OnSetComplete( int index, object oldValue, object newValue )
    {
      if( Setted != null && m_bSkipEvents == false )
      {
        Setted( index, oldValue, newValue );
      }

      base.OnSetComplete( index, oldValue, newValue );

      RaiseChangedEvent();
    }
    #endregion
  }
}
