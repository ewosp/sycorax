using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Resources;
using System.ComponentModel;

using UtilityLibrary.Win32;
using UtilityLibrary.WinControls;
using UtilityLibrary.Collections;
using UtilityLibrary.General;
using UtilityLibrary.CommandBars;


namespace UtilityLibrary.Menus
{
  public class MenuItemEx : MenuItem
  {
    #region Class Static and Contants Variables 
    public const int BITMAP_SIZE = 16; /* static */

    // Smooth Colors
    static ColorGroup group = ColorGroup.GetColorGroup();
    
    static Color bgColor            = group.bgColor;
    static Color stripeColor        = group.stripeColor;
    static Color selectionColor     = group.selectionColor;
    static Color borderColor        = group.borderColor;
    static Color darkSelectionColor = group.darkSelectionColor;
    
    static int   itemHeight;
    static int   iconSize = SystemInformation.SmallIconSize.Width + 5;
    static int   STRIPE_WIDTH = iconSize + 2;
    static bool  doColorUpdate = false;
    static Color glyphsTransparentColor = Color.FromArgb( 192, 192, 192 );
    
    static ImageList glyphsImages;

    // cache brushes of menuItem to boost performance
    static SolidBrush brushSelection  = new SolidBrush( selectionColor );
    static SolidBrush brushDarkSelct  = new SolidBrush( darkSelectionColor );
    static SolidBrush brushStripe     = new SolidBrush( stripeColor );
    static SolidBrush brushBgColor    = new SolidBrush( bgColor );
    static SolidBrush brushBlack      = new SolidBrush( Color.Black );
    static SolidBrush brushContrast1  = new SolidBrush( Color.FromArgb( 120, SystemColors.MenuText ) );
    static SolidBrush brushContrast2  = new SolidBrush( Color.FromArgb( 255, SystemColors.MenuText ) );
    static SolidBrush brushContrast3  = new SolidBrush( Color.FromArgb( 255, Color.White ) );
    #endregion

    #region Class Memebers
    private string        shortcuttext = "";
    private Bitmap        icon = null;
    private Color         iconTransparentColor = Color.Fuchsia;
    private EventHandler  clickHandler = null;
    
    // We could use an image list to associate
    // the menu items with an bitmap instead of
    // assigning a whole Bitmap object to the menu item
    private ImageList     imageList = null;
    private int           imageIndex = -1;

    private object        m_tag;

    private MenuItemExCollection  m_items = null;
    #endregion
                
    #region Initialize/Finilize methods
    static MenuItemEx()
    {
      // Initialize menu glyphs: checkmark and bullet
      glyphsImages = new ImageList();
      glyphsImages.ImageSize = new Size( BITMAP_SIZE, BITMAP_SIZE );
      Assembly thisAssembly = Assembly.GetAssembly( Type.GetType("UtilityLibrary.Menus.MenuItemEx"));
      ResourceManager rm = new ResourceManager("UtilityLibrary.Resources.ImagesMenu", thisAssembly );
      Bitmap glyphs = ( Bitmap )rm.GetObject("Glyphs");
      glyphs.MakeTransparent( glyphsTransparentColor );
      glyphsImages.Images.AddStrip( glyphs );
    }
    
    public MenuItemEx() : this( null, null )
    {
    } 

    public MenuItemEx( string name, EventHandler handler, Shortcut shortcut ) 
      : this( name, handler )
    {
      Initialize( null, shortcut, handler, null, -1);
    }
    
    public MenuItemEx( string name, Bitmap icon, Shortcut shortcut, EventHandler handler ) 
      : this( name, handler )
    {
      Initialize( icon, shortcut, handler, null, -1);
    }

    public MenuItemEx( string name, ImageList imageList, int imageIndex, 
      Shortcut shortcut, EventHandler handler ) 
      : this( name, handler )
    {
      Initialize( icon, shortcut, handler, imageList, imageIndex );
    }

    public MenuItemEx( string name, EventHandler handler ) : base( name, handler )
    {
      Initialize( null, Shortcut.None, handler, null, -1);
    }

    private void Initialize( Bitmap bitmap, Shortcut shortcut, EventHandler handler, 
      ImageList list, int imageIndex )
    {
      m_items = new MenuItemExCollection();
      m_items.Changed += new EventHandler( OnCollectionChange );
      m_items.Cleared += new CollectionClear( OnCollectionClear );

      OwnerDraw = true;
      this.Shortcut = shortcut;
      icon = bitmap;
      clickHandler = handler;
      imageList = list;
      this.imageIndex = imageIndex;
    }

    protected override void Dispose(bool disposing)
    {
      if( disposing == true )
      {
        if( brushSelection != null ) brushSelection.Dispose();
        if( brushDarkSelct != null ) brushDarkSelct.Dispose();
        if( brushStripe != null ) brushStripe.Dispose();
        if( brushBgColor != null ) brushBgColor.Dispose();
      } 
      
      base.Dispose( disposing );
    }
    #endregion

    #region Overrides
    private void OnCollectionClear()
    {
      base.MenuItems.Clear();
    }

    private void OnCollectionChange( object parent, System.EventArgs e )
    {
      base.MenuItems.Clear();

      for( int i=0; i<m_items.Count; i++ )
      {
        MenuItemEx item = m_items[i];
        base.MenuItems.Add( item );
      }
    }

    protected override void OnSelect( EventArgs e )
    {
      // This is to support popup menus when using this class
      // in conjunction with a toolbar that behaves like a menu
      Menu parent = Parent;
      
      while( !( parent is CommandBarMenu ) && !( parent == null ) )
      {
        if( parent is MenuItemEx )
        {
          parent = ( parent as MenuItemEx ).Parent;
        }
        else if( parent is MenuItem )
        {
          parent = ( parent as MenuItem ).Parent;
        }
        else if( parent == Parent.GetMainMenu() )
        {
          parent = null;
        }
        else
        {
          parent = null;
        }
      }
        
      if( parent is CommandBarMenu )
      {
        CommandBarMenu cbm = ( CommandBarMenu )parent;
        cbm.SelectedMenuItem = this;
      }

      base.OnSelect( e );
    }

    protected override void OnMeasureItem( MeasureItemEventArgs e )
    {
      base.OnMeasureItem( e );
      
      // if separator then set default values
      if( Text == "-" )
      {
        e.ItemHeight = 8;
        e.ItemWidth  = 4;
        return;
      }

      // measure shortcut text
      if( Shortcut != Shortcut.None ) 
      {
        shortcuttext = Shortcut.ToString();
        shortcuttext = shortcuttext.Replace( "Ctrl", "Ctrl+" );
        shortcuttext = shortcuttext.Replace( "Shift", "Shift+" );
        shortcuttext = shortcuttext.Replace( "Alt", "Alt+" );
      } 
        
      bool    topLevel = ( Parent == Parent.GetMainMenu() );
      string  tempShortcutText = ( topLevel == true ) ? "" : shortcuttext;
      int     extraHeight = 2;

      e.ItemHeight  = SystemInformation.MenuHeight + extraHeight;

      int textwidth = ( int )( e.Graphics.MeasureString( Text + tempShortcutText, 
        SystemInformation.MenuFont ).Width );
      
      if( topLevel == true )
      {
        e.ItemWidth = textwidth - 5; 
      }
      else
      {
        e.ItemWidth =  textwidth + 45;
      }

      // save menu item heihgt for later use
      itemHeight = e.ItemHeight;
    }

    protected override void OnDrawItem( DrawItemEventArgs e )
    {
      base.OnDrawItem( e );

      if( doColorUpdate ) 
      {
        DoUpdateMenuColors();
      }

      Graphics g = e.Graphics;
      Rectangle bounds = e.Bounds;
      
      bool selected = ( e.State & DrawItemState.Selected ) == DrawItemState.Selected;
      bool toplevel = ( Parent == Parent.GetMainMenu() );
      bool hasIcon  = ( Icon != null );
      bool enabled  = Enabled;
      bool isSeparator = ( Text == "-" );

      // Try to  speed up drawing top level a little bit
      if( toplevel )
      {
        DrawBackground( g, bounds, e.State, toplevel, hasIcon, enabled );
        DrawMenuText( g, bounds, Text, shortcuttext, Enabled, toplevel, e.State );
        return;
      }
      
      DrawBackground( g, bounds, e.State, toplevel, hasIcon, enabled );
      
      if( isSeparator == true ) // if separator then no need in Icon drawing
      {
        DrawSeparator( g, bounds );
      } 
      else if( hasIcon == true )
      {
        DrawIcon( g, Icon, bounds, selected, Enabled, Checked );
      }
      else if( imageList != null && imageIndex != -1 )
      {
        DrawIcon( g, imageList.Images[imageIndex], bounds, selected, Enabled, Checked );
      }
      else if( Checked && !hasIcon )
      {
        DrawMenuGlyph( g, bounds, selected, true );
      }
      else if( RadioCheck )
      {
        DrawMenuGlyph( g, bounds, selected, false );
      }

      if( isSeparator == false )
      {
        DrawMenuText( g, bounds, Text, shortcuttext, Enabled, toplevel, e.State );
      }
    }
    #endregion

    #region Properties
    [DefaultValue(null)]
    public Bitmap Icon 
    {
      get 
      { 
        return icon; 
      }
      set 
      { 
        icon = value; 
      }
    }

    [DefaultValue(null)]
    public ImageList ImageList
    {
      set 
      { 
        imageList = value; 
      }
      get 
      { 
        return imageList; 
      }
    }

    [DefaultValue(-1)]
    [Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
    [TypeConverter("System.Windows.Forms.ImageIndexConverter")]
    public int ImageIndex
    {
      set 
      { 
        imageIndex = value; 
      }
      get 
      { 
        return imageIndex;  
      }
    }

    [DefaultValue(null), Browsable(false)]
    public EventHandler ClickHandler
    {
      set 
      {
        clickHandler = value;
      }
      get 
      { 
        return clickHandler; 
      }
    }

    [DefaultValue( typeof( Color ), "Fuchsia")]
    public Color IconTransparentColor
    {
      set 
      { 
        iconTransparentColor = value;
      }
      get 
      { 
        return iconTransparentColor; 
      }
    }

    [DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
    new public MenuItemExCollection MenuItems
    {
      get
      {
        return m_items;  
      }
    }

    [DefaultValue(-1)]
    new public int Index
    {
      get
      {
        return base.Index;
      }
      set
      {
        base.Index = value;
      }
    }
    
    [ Category( "Data" ), 
    DefaultValue( null ), 
    Description( "GET/SET User data which associated with menu item" ) ]
    public object Tag
    {
      get
      {
        return m_tag;
      }
      set
      {
        m_tag = value;
      }
    }
    #endregion

    #region Methods
    static public MenuItemEx CloneMenu( MenuItemEx currentItem )
    {
      MenuItemEx clonedItem = new MenuItemEx( currentItem.Text, 
        ( Bitmap )currentItem.Icon, 
        ( Shortcut )currentItem.Shortcut, 
        currentItem.ClickHandler );

      // Preserve the enable and check state
      clonedItem.Enabled = currentItem.Enabled;
      clonedItem.Checked = currentItem.Checked;
      clonedItem.RadioCheck = currentItem.RadioCheck;

      foreach ( MenuItemEx item in currentItem.MenuItems )
      {
        clonedItem.MenuItems.Add( CloneMenu( item ) );
      }
      
      return clonedItem;
    }

    static public void UpdateMenuColors( object sender, EventArgs e )
    {
      doColorUpdate = true;
    }

    static public void UpdateMenuColors()
    {
      doColorUpdate = true;
    }
    #endregion

    #region Implementation
    void DoUpdateMenuColors()
    {
      ColorGroup group    = ColorGroup.GetColorGroup();
      darkSelectionColor  = group.darkSelectionColor;

      bgColor         = group.bgColor;
      stripeColor     = group.stripeColor;
      selectionColor  = group.selectionColor;
      borderColor     = group.borderColor;

      brushSelection  = new SolidBrush( selectionColor );
      brushDarkSelct  = new SolidBrush( darkSelectionColor );
      brushStripe     = new SolidBrush( stripeColor );
      brushBgColor    = new SolidBrush( bgColor );
      
      doColorUpdate   = false;
    }
                  
    void DrawMenuGlyph( Graphics g, Rectangle bounds, bool selected,  bool bCheckMark )
    {
      int checkTop  = bounds.Top  + ( itemHeight - BITMAP_SIZE ) / 2;
      int checkLeft = bounds.Left + ( STRIPE_WIDTH - BITMAP_SIZE ) / 2;
      
      if( Enabled ) 
      {
        g.FillRectangle( ( selected == true ) ? brushDarkSelct : brushSelection, 
          bounds.Left+1, bounds.Top+1, 
          STRIPE_WIDTH-3, bounds.Height-3 );
        
        glyphsImages.Draw( g, checkLeft, checkTop, bCheckMark?0:1);
        
        g.DrawRectangle( new Pen( borderColor ), bounds.Left+1, bounds.Top+1, STRIPE_WIDTH-3, bounds.Height-3);
      }
      else
      {
        int imageIndex = ( bCheckMark == true ) ? 0 : 1;
        
        ControlPaint.DrawImageDisabled( g, glyphsImages.Images[imageIndex], checkLeft, checkTop, Color.Black );
      }
    }

    void DrawIcon( Graphics g, Image icon, Rectangle bounds, bool selected, 
      bool enabled, bool isChecked )
    {
      // make icon transparent
      Bitmap tempIcon = ( Bitmap )icon;
      tempIcon.MakeTransparent( iconTransparentColor );

      int iconTop = bounds.Top + ( itemHeight - BITMAP_SIZE )/2;
      int iconLeft = bounds.Left + ( STRIPE_WIDTH - BITMAP_SIZE )/2;
      
      if( enabled ) 
      {
        if( selected ) 
        {
          if( isChecked ) 
          {
            DrawCheckedRectangle( g, bounds );
            g.DrawImage( icon, iconLeft + 1, iconTop );
          }
          else 
          {
            ControlPaint.DrawImageDisabled( g, icon, iconLeft + 1, iconTop, Color.Black );
            g.DrawImage( icon, iconLeft, iconTop-1);
          }
        } 
        else 
        {
          if( isChecked ) 
            DrawCheckedRectangle( g, bounds );
          
          g.DrawImage( icon, iconLeft + 1, iconTop );
        }
      } 
      else 
      {
        ControlPaint.DrawImageDisabled( g, icon, iconLeft + 1, iconTop, SystemColors.HighlightText );
      }
    }

    void DrawCheckedRectangle( Graphics g, Rectangle bounds )
    {
      int checkTop = bounds.Top + ( itemHeight - BITMAP_SIZE )/2;
      int checkLeft = bounds.Left + ( STRIPE_WIDTH - BITMAP_SIZE )/2;

      g.FillRectangle( brushSelection, bounds.Left+1, 
        bounds.Top+1, STRIPE_WIDTH-3, bounds.Height-3 );
      
      g.DrawRectangle( new Pen( borderColor ), bounds.Left+1, 
        bounds.Top+1, STRIPE_WIDTH-3, bounds.Height-3);
    }

    void DrawSeparator( Graphics g, Rectangle bounds )
    {
      int y = bounds.Y + bounds.Height / 2;
      
      g.DrawLine( SystemPens.ControlDark, bounds.X + iconSize + 7, y, bounds.X + bounds.Width - 2, y );
    }
    
    void DrawBackground( Graphics g, Rectangle bounds, DrawItemState state, 
      bool toplevel, bool hasicon, bool enabled )
    {
      bool selected = ( ( state & DrawItemState.Selected ) == DrawItemState.Selected );
      bool hot = ( ( state & DrawItemState.HotLight ) == DrawItemState.HotLight );
      
      if( selected || hot ) 
      {
        if( toplevel && selected ) // draw toplevel, selected menuitem
        {   
          bounds.Inflate(-1, 0);
          g.FillRectangle( brushStripe, bounds );
          
          if( ColorUtil.UsingCustomColor )
          {
            GDIUtils.Draw3DRect( g, bounds, ColorUtil.VSNetBorderColor, ColorUtil.VSNetControlColor );
          }
          else 
          {
            ControlPaint.DrawBorder3D( g, bounds.Left, bounds.Top, bounds.Width, 
              bounds.Height, Border3DStyle.Flat, 
              Border3DSide.Top | Border3DSide.Left | Border3DSide.Right );
          }
        } 
        else 
        {   // draw menuitem hotlighted
          if( enabled ) 
          {   
            g.FillRectangle( brushSelection, bounds );
            g.DrawRectangle( new Pen( borderColor ), bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
          }
          else 
          {
            // Check if menu item was selected by using the mouse or the keyboard
            RECT rc = new RECT();
            IntPtr parentHandle = Parent.Handle;
            
            uint index = ( uint )Index;
            bool success = WindowsAPI.GetMenuItemRect( IntPtr.Zero, parentHandle, index, ref rc );
            
            Rectangle menuRect = new Rectangle( rc.left, rc.top, rc.right-rc.left, rc.bottom-rc.top );
            Point mp = Control.MousePosition;
            
            if( !menuRect.Contains( mp ) ) // Menu was selected by using keyboard
            {
              g.FillRectangle( brushBgColor, bounds );
              g.DrawRectangle( new Pen( borderColor ), bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }
            else // Menu was selected by using mouse
            {
              g.FillRectangle( brushStripe, bounds );
              bounds.X += STRIPE_WIDTH;
              bounds.Width -= STRIPE_WIDTH;
              g.FillRectangle( brushBgColor, bounds );
            }
          }
        }
      } 
      else 
      {
        if( !toplevel ) // draw menuitem, unselected
        {   
          g.FillRectangle( brushStripe, bounds );
          bounds.X += STRIPE_WIDTH;
          bounds.Width -= STRIPE_WIDTH;
          g.FillRectangle( brushBgColor, bounds );
        } 
        else // draw toplevel, unselected menuitem
        {
          g.FillRectangle( SystemBrushes.Control, bounds );
        }
      }
    }

    void DrawMenuText( Graphics g, Rectangle bounds, string text, string shortcut, 
      bool enabled, bool toplevel, DrawItemState state )
    {
      StringFormat stringformat = new StringFormat();
      stringformat.HotkeyPrefix = HotkeyPrefix.Show;
    
      // if 3D background happens to be black, as it is the case when
      // using a high contrast color theme, then make sure text is white
      bool highContrast = false;
      bool whiteHighContrast = false;
      if( SystemColors.Control.ToArgb() == Color.FromArgb(255,0,0,0).ToArgb() ) highContrast = true;
      if( SystemColors.Control.ToArgb() == Color.FromArgb(255,255,255,255).ToArgb() ) whiteHighContrast = true;

      // if menu is a top level, extract the ampersand that indicates the shortcut character
      // so that the menu text is centered
      string textTopMenu = text;
      if( toplevel ) 
      {
        int index = text.IndexOf("&");
        if( index != -1 ) 
        {
          // remove it
          text = text.Remove( index,1);
        }
      }
      
      int textwidth = ( int )( g.MeasureString( text, SystemInformation.MenuFont ).Width );
      int x = toplevel ? bounds.Left + ( bounds.Width - textwidth ) / 2: bounds.Left + iconSize + 10;
      int topGap = 4;
      if( toplevel ) topGap = 2;
      int y = bounds.Top + topGap;
      Brush brush = null;
      
      if( !enabled )
      {
        brush = brushContrast1;
      }
      else if( highContrast )
      {
        brush = brushContrast2;
      }
      else
      {
        brush = brushBlack;
      }

      if( whiteHighContrast && ( ( state & DrawItemState.HotLight ) > 0 
        || ( ( state & DrawItemState.Selected ) > 0 && !toplevel )) )
      {
        brush = brushContrast3;
      }
      
      if( toplevel ) text = textTopMenu;
      g.DrawString( text, SystemInformation.MenuFont, brush, x, y, stringformat );
          
      // don't draw the shortcut for top level menus
      // in case there was actually one
      if( !toplevel ) 
      {
        // draw shortcut right aligned
        stringformat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
        g.DrawString( shortcut, SystemInformation.MenuFont, brush, bounds.Width - 10 , bounds.Top + topGap, stringformat );
      }
    }
    #endregion

    #region Class public helper methods
    public void RaiseClick()
    {
      base.OnClick( EventArgs.Empty );
    }
    #endregion
  }
}