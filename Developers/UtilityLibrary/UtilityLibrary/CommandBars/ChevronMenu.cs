using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using UtilityLibrary.Collections;
using UtilityLibrary.Menus;
using UtilityLibrary.WinControls;

namespace UtilityLibrary.CommandBars
{
  /// <summary>
  /// Summary description for ToolBarItemMenu.
  /// </summary>
  public class ChevronMenu : PopupMenu
  {
    #region Class Variables
    private ToolBarItemCollection m_items = new ToolBarItemCollection();
    #endregion
    
    #region Properties
    public ToolBarItemCollection Items
    {
      set
      { 
        m_items = value; 
        Attach(); 
      }
      get 
      { 
        return m_items; 
      }
    }
    #endregion

    #region Implementation
    private   void AddSubMenu( MenuCommand parentMenuCommand, MenuItemExCollection items )
    {
      for ( int i = 0; i < items.Count; i++ )
      {
        // I know these menu items are actually MenuItemExs
        MenuItemEx item = (MenuItemEx)items[i];

        Bitmap bmp = ( item.Icon != null ) ? (Bitmap)item.Icon : 
          ( ( item.ImageList != null ) ? 
          (Bitmap)item.ImageList.Images[ item.ImageIndex ] : null ); 

        EventHandler hndl = item.ClickHandler;

        // if menu item does not have any ClickHandler then attach own
        if( hndl == null )
        {
          hndl = new EventHandler( RaiseMenuItemClick );
        }

        MenuCommand currentMenuCommand = new MenuCommand(item.Text, bmp,
          (Shortcut)item.Shortcut, hndl, item);

        currentMenuCommand.Checked = item.Checked;
        currentMenuCommand.Enabled = item.Enabled;

        parentMenuCommand.MenuCommands.Add(currentMenuCommand);
        
        if ( item.MenuItems.Count > 0 )
        {
          AddSubMenu( currentMenuCommand, item.MenuItems );
        }
      }
    }

    private   void AddSubMenu( MenuCommand parentMenuCommand, Menu.MenuItemCollection items )
    {
      for ( int i = 0; i < items.Count; i++ )
      {
        // I know these menu items are actually MenuItemExs
        MenuItemEx item = (MenuItemEx)items[i];

        Bitmap bmp = ( item.Icon != null ) ? (Bitmap)item.Icon : 
          ( ( item.ImageList != null ) ? 
          (Bitmap)item.ImageList.Images[ item.ImageIndex ] : null ); 

        EventHandler hndl = item.ClickHandler;

        // if menu item does not have any ClickHandler then attach own
        if( hndl == null )
        {
          hndl = new EventHandler( RaiseMenuItemClick );
        }

        MenuCommand currentMenuCommand = new MenuCommand(item.Text, bmp,
          (Shortcut)item.Shortcut, hndl, item);
        
        currentMenuCommand.Checked = item.Checked;
        currentMenuCommand.Enabled = item.Enabled;
        
        parentMenuCommand.MenuCommands.Add(currentMenuCommand);
        
        if ( item.MenuItems.Count > 0 )
          AddSubMenu(currentMenuCommand, item.MenuItems);
      }
    }
    
    private   void Attach()
    {
      // Cleanup previous menus
      MenuCommands.Clear();
            
      foreach( ToolBarItem item in m_items )
      {
        string text = item.Text;
        
        if ( text == string.Empty || text == null )
          text = item.ToolTip;
        
        if ( item.Style == ToolBarItemStyle.Separator )
          text = "-";

        // If this is a combobox
        if ( item.ComboBox != null )
        {
          MenuCommands.Add( new MenuCommand( item.ComboBox ) );
          item.ComboBox.Visible = true;
          
          // I know where this combobox comes from
          ComboBoxBase cbb = ( ComboBoxBase )item.ComboBox;
          cbb.ToolBarUse = false;
          continue;
        }

        Bitmap bmp = ( item.Image != null ) ? (Bitmap)item.Image : 
          ( ( item.ImageIndex > -1 ) ? 
          (Bitmap)item.ToolBar.ImageList.Images[ item.ImageIndex ] : null ); 

        EventHandler hndl = new EventHandler( RaiseItemClick );

        MenuCommand currentMenuCommand = new MenuCommand( text, bmp, 
          (Shortcut)item.Shortcut, hndl, item );

        currentMenuCommand.Checked = item.Checked;
        currentMenuCommand.Enabled = item.Enabled;

        MenuCommands.Add(currentMenuCommand);

        // If we have a menubar
        if ( item.MenuItems != null)
          AddSubMenu(currentMenuCommand, item.MenuItems);

      }
    }

    protected void RaiseItemClick( object owner, System.EventArgs e )
    {
      if( owner != null && owner is ToolBarItem )
      {
        (( ToolBarItem )owner).FireClickEventHandler();
      }
    }

    protected void RaiseMenuItemClick( object owner, System.EventArgs e )
    {
      if( owner != null && owner is MenuItemEx )
      {
        (( MenuItemEx )owner).RaiseClick();
      }
    }
    #endregion
  }
}
