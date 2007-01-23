using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

using UtilityLibrary.Menus;
using UtilityLibrary.Collections;


namespace UtilityLibrary.CommandBars
{
  /// <summary>
  /// Summary description for CommandBaMenu
  /// </summary>
  [ToolboxItem(false)] 
  public class CommandBarMenu : ContextMenu
  {
    #region Class Variables
    /// <summary>
    /// This is just to keep track of the selected
    /// menu as well as hold the menuitems in the menubar
    /// </summary>
    private Menu                  m_selMenu;
    /// <summary>
    /// Store reference on attcahed to class collection. Any change of data in collection
    /// will recreate menu items collection of Context Menu.
    /// </summary>
    private MenuItemExCollection  m_attach;
    #endregion
    
    #region Constructors
    /// <summary>
    /// Default constructor
    /// </summary>
    public CommandBarMenu()
    {
    }

    /// <summary>
    /// Create class and attach it to collection of items
    /// </summary>
    /// <param name="items">Collection of Menu Items</param>
    /// <param name="bAttach">TRUE - attach class to collection, otherwise 
    /// skip any collection context changes</param>
    public CommandBarMenu( MenuItemExCollection items, bool bAttach )
    {
      if( bAttach == true )
      {
        AttachOnMenuCollection( items );
      }
      else
      {
        UpdateContextMenuItems( items );
      }
    }

    /// <summary>
    /// Attach custom collection of menu items to context menu. Any changes
    /// of collection context will be catched by context menu. Context menu
    /// will be always up to date
    /// </summary>
    /// <param name="items"></param>
    public void AttachOnMenuCollection( MenuItemExCollection items )
    {
      if( m_attach != null ) Detach();

      m_attach = items;
      m_attach.Changed += new EventHandler( OnAttacherDataChanged );

      // get items from collection
      UpdateContextMenuItems( m_attach );
    }

    /// <summary>
    /// Detach context menu from outsource collection of menu items
    /// </summary>
    public void Detach()
    {
      if( m_attach != null )
      {
        m_attach.Changed -= new EventHandler( OnAttacherDataChanged );
      }
    }
    #endregion

    #region Overrides
    /// <summary>
    /// Method will reac on any context change of attached collection
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnAttacherDataChanged( object sender, EventArgs e )
    {
      UpdateContextMenuItems( m_attach );
    }

    /// <summary>
    /// Helper Method which update internal Collection of ContextMenu
    /// </summary>
    /// <param name="coll"></param>
    private void UpdateContextMenuItems( MenuItemExCollection coll )
    {
      MenuItems.Clear();

      for( int i = 0; i<m_attach.Count; i++ )
      {
        MenuItems.Add( ( MenuItemEx )m_attach[ i ] );
      }
    }

    protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
    {
      return base.ProcessCmdKey( ref msg, keyData );
    }
    #endregion
    
    #region Properties
    /// <summary>
    /// GET/SET selected Menu Item from Context menu
    /// </summary>
    internal Menu SelectedMenuItem
    {
      get 
      { 
        return m_selMenu; 
      }
      set 
      { 
        m_selMenu = value; 
      }
    }
    #endregion
  }
}
