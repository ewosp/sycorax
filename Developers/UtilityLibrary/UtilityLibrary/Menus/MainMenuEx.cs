using System;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Drawing.Text;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;

using UtilityLibrary.Menus;
using UtilityLibrary.Win32;
using UtilityLibrary.Collections;
using UtilityLibrary.General;


namespace UtilityLibrary.Menus
{
  [ToolboxItem(true)]
  [ToolboxBitmap(typeof(UtilityLibrary.Menus.MainMenuEx), "UtilityLibrary.Menus.MainMenuEx.bmp")]
  /*[Designer( typeof( UtilityLibrary.Menus.MainMenuExDesigner ) )]*/
  public class MainMenuEx : System.Windows.Forms.MainMenu
  {
    #region Class Members
    private ImageList m_imgList = null;
    private MenuItemExCollection  m_items;
    #endregion

    #region Class Properties
    public ImageList ImagesList
    {
      get
      {
        return m_imgList;
      }
      set
      {
        m_imgList = value;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    new public MenuItemExCollection MenuItems
    {
      get
      {
        return m_items;
      }
    }
    #endregion

    #region Class Initialize methods
    public MainMenuEx()
    {
      m_items = new MenuItemExCollection();
      m_items.Changed += new EventHandler( OnCollectionChange );
    }
    #endregion

    #region Class Helper methods
    private void OnCollectionChange( object parent, System.EventArgs e )
    {
      base.MenuItems.Clear();

      for( int i=0; i<m_items.Count; i++ )
      {
        MenuItemEx item = m_items[i];
        item.ImageList = m_imgList;
        base.MenuItems.Add( item );
      }
    }
    #endregion
  }
}
