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
  [ToolboxBitmap(typeof(UtilityLibrary.Menus.ContextMenuEx), 
     "UtilityLibrary.Menus.ContextMenuEx.bmp")]
  public class ContextMenuEx : ContextMenu
  {
    private ImageList m_imgList = null;
    private MenuItemExCollection  m_items;

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


    public ContextMenuEx()
    {
      m_items = new MenuItemExCollection();
      m_items.Changed += new EventHandler( OnCollectionChange );
    }

    public ContextMenuEx( ImageList img, MenuItemEx[] items ) : this()
    {
      int iCount = 0;

      for( int i=0; i<items.Length; i++ )
      {
        MenuItemEx item = items[i];
        item.ImageList = img;

        if( item.Text.Length > 1 && item.Text != "-" )
        {
          item.ImageIndex = iCount;
          iCount++;
        }
      }

      base.MenuItems.AddRange( ( MenuItem[] )items );
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    new public MenuItemExCollection MenuItems
    {
      get
      {
        return m_items;
      }
    }

    private void OnCollectionChange( object parent, System.EventArgs e )
    {
      base.MenuItems.Clear();

      for( int i=0; i<m_items.Count; i++ )
      {
        MenuItemEx item = m_items[i];
        
        if( item.ImageList == null )
          item.ImageList = m_imgList;
        
        base.MenuItems.Add( item );
      }
    }

  }

}