using System;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;

using UtilityLibrary.CommandBars;


namespace UtilityLibrary.Collections
{
  /// <summary>
  /// Summary description for RebarBandCollection.
  /// </summary>
  public class RebarBandCollection : System.Collections.CollectionBase, IEnumerable
  {
    #region Events
    public event EventHandler Changed;
    #endregion

    #region Class Variables
    ReBar parentRebar = null;
    #endregion

    #region Constructors
    public RebarBandCollection(ReBar bar)
    {
      parentRebar = bar;
    }
    #endregion
    
    #region Methods
    public int Add(ToolBarEx toolBar)
    {
      // Remove place Holder ToolBar
      // -- This toolbar makes sure Rebar has
      // some minimum height while on designig time
      if ( parentRebar.PlaceHolderAdded )
        parentRebar.RemovePlaceHolder();

      if (Contains(toolBar)) return -1;
      int index = InnerList.Add(toolBar);
      
      // Set ToolBar parent rebar
      toolBar.m_parent = parentRebar;
      RaiseChanged();
      return index;
    }

    public bool Contains(ToolBarEx toolBar)
    {
      return InnerList.Contains(toolBar);
    }
  
    public int IndexOf(ToolBarEx toolBar)
    {
      return InnerList.IndexOf(toolBar);
    }
  
    public void Remove(ToolBarEx toolBar)
    {
      // Remove band from the native rebar controls first
      parentRebar.RemoveBand(IndexOf(toolBar));
      // Remove it from the collection
      InnerList.Remove(toolBar);
      toolBar.m_parent = null;

      // Inform listeners to update
      RaiseChanged();   

      // If we don't have any bands left, add
      // flag that we need to add the "place holder toolbar
      // --it will be added later when the sizing "sticks"
      if ( InnerList.Count == 0 && parentRebar.PlaceHolderAdded == false )
        parentRebar.AddPlaceHolderToolBar = true;
    }

    public new void Clear()
    {
          
      // Before wiping out all bands from
      // the collection, make sure we also
      // delete the bands from the native rebar control
      for ( int i = 0; i < InnerList.Count; i++ )
      {
        ToolBarEx tbe = (ToolBarEx)InnerList[i];
        tbe.m_parent.RemoveBand(i);
        tbe.m_parent = null;
      }
      // Now from the collection
      InnerList.Clear();

      // Inform listeners to update
      RaiseChanged();

      // We don't have any bands left, add
      // the ToolBar place holder
      if ( parentRebar.PlaceHolderAdded == false )
        parentRebar.AddPlaceHolder();

    }
    
    public ToolBarEx this[int index]
    {
      get { return (ToolBarEx) InnerList[index]; }
  
    }
    #endregion
  
    #region Implementation
    void RaiseChanged()
    {
      if (Changed != null) Changed(this, null);
    }
    #endregion
  }
}
