using System;
using System.Collections;
using System.Windows.Forms;
using UtilityLibrary.WinControls;
using UtilityLibrary.Menus;

namespace UtilityLibrary.Collections
{
  /// <summary>
  /// Summary description for MenuItemExCollection.
  /// </summary>
  public class MenuItemExCollection : CollectionWithEvents
  {
    #region Constructors
    public MenuItemExCollection()
    {
    }
    #endregion

    #region Methods
    public virtual void AddRange( MenuItem[] items )
    {
      for( int i=0; i<items.Length; i++ )
      {
        Add( (MenuItemEx)items[i] );
      }
    }
    public virtual void AddRange( MenuItemEx[] items )
    {
      for( int i=0; i<items.Length; i++ )
      {
        Add( items[i] );
      }
    }

    public int Add( MenuItemEx item )
    {
      if (Contains(item)) return -1;
      int index = List.Add(item);
      return index;
    }

    public bool Contains( MenuItemEx item )
    {
      return InnerList.Contains( item );
    }
  
    public int IndexOf( MenuItemEx item )
    {
      return InnerList.IndexOf( item );
    }
  
    public void Remove( MenuItemEx item )
    {
      List.Remove( item );
    }

    public void Insert( int index, MenuItemEx item )
    {
      List.Insert( index, item );
    }

    public MenuItemEx this[ int index ]
    {
      get
      { 
        return ( MenuItemEx )InnerList[ index ]; 
      }
      set
      {  
        List[ index ] = value;
      }
    }
    #endregion
  }
}


