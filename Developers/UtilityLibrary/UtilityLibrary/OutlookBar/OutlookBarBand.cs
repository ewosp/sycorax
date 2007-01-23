using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Drawing.Design;

using UtilityLibrary.Collections;
using UtilityLibrary.Win32;
using UtilityLibrary.General;
using UtilityLibrary.Menus;


namespace UtilityLibrary.WinControls
{
  [ToolboxItem(false)]
  public class OutlookBarBand : Control
  {
    #region Class variables
    private Control       childControl;
    private ImageList     smallImageList;
    private ImageList     largeImageList;
    private Color         background  = ColorUtil.VSNetControlColor;
    private Color         textColor   = SystemColors.ControlText;
    private IconView      iconView    = IconView.Large;

    private OutlookBarItemCollection items;
    #endregion

    #region Class Events
    [Category("Property Changed")]
    public event EventHandler Changed;
    [Category("Property Changed")]
    public event EventHandler ChildControlChanged;
    [Category("Property Changed")]
    public event EventHandler SmallImageListChanged;
    [Category("Property Changed")]
    public event EventHandler LargeImageListChanged;
    [Category("Property Changed")]
    public event EventHandler BackgroundChanged;
    [Category("Property Changed")]
    public event EventHandler TextColorChanged;
    [Category("Property Changed")]
    public event EventHandler IconViewChanged;
    #endregion
                        
    #region Constructors
    public OutlookBarBand( string text, Control childControl ) : this( text )
    {
      this.childControl = childControl;
    }

    public OutlookBarBand( string text ) : this()
    {
      this.Text = text;
    }

    public OutlookBarBand()
    {
      InitializeComponent();
      Name = ( this.Site != null ) ? Site.Name : null;
    }

    private void InitializeComponent()
    {
      // Constructor for designer support
      items = new OutlookBarItemCollection();

      items.OnItemAdded += new OutlookBarItemCollection.OutlookBarItemEventHandler( Items_OnItemAdded );
      items.OnItemRemoved += new OutlookBarItemCollection.OutlookBarItemEventHandler( Items_OnItemRemoved );
    }
    #endregion

    #region Class Overrides
    protected override void Dispose( bool disposing )
    {
      if( items != null )
      {
        items.Clear();
        items.OnItemAdded -= new OutlookBarItemCollection.OutlookBarItemEventHandler( Items_OnItemAdded );
        items.OnItemRemoved -= new OutlookBarItemCollection.OutlookBarItemEventHandler( Items_OnItemRemoved );
      }

      base.Dispose( disposing);
    }
    #endregion

    #region Class Properties
    [ DefaultValue( null ), Category( "Behavior" ) ]
    public Control ChildControl
    {
      get
      { 
        return childControl; 
      }
      set
      {
        if( value != childControl )
        {
          childControl = value;
          OnChildControlChanged();
        }
      }
    }

    [ Category( "Appearance" ), DefaultValue( null ) ]
    public ImageList SmallImageList
    {
      get 
      { 
        return smallImageList; 
      }
      set 
      { 
        if( value != smallImageList )
        {
          smallImageList = value; 
          OnSmallImageListChanged();
        }
      }
    }

    [ Category( "Appearance" ), DefaultValue( null ) ]
    public ImageList LargeImageList
    {
      get 
      { 
        return largeImageList; 
      }
      set 
      { 
        if( value != largeImageList )
        {
          largeImageList = value; 
          OnLargeImageListChanged();
        }
      }
    }

    [ Category( "Appearance" ) ]
    public Color Background
    {
      get 
      { 
        return background; 
      }
      set 
      { 
        if( value != background )
        {
          background = value; 
          OnBackgroundChanged();
        }
      }
    }

    [ Category( "Appearance" ) ]
    public Color TextColor
    {
      get 
      { 
        return textColor; 
      }
      set 
      { 
        if( value != textColor )
        {
          textColor = value; 
          OnTextColorChanged();
        }
      }
    }

    [ DesignerSerializationVisibility( DesignerSerializationVisibility.Content ), Category( "Behavior" ) ]
    public OutlookBarItemCollection Items
    {
      get 
      { 
        return items; 
      }
    }

    [ Category( "Appearance" ), DefaultValue( typeof(IconView), "Large" ) ]
    public IconView IconView 
    {
      get 
      { 
        return iconView; 
      }
      set 
      { 
        if( value != iconView )
        {
          iconView = value; 
          OnIconViewChanged();
        }
      }
    }
    #endregion

    #region Class Methods && Event Handler
    private void Items_OnItemAdded( object sender, OutlookBarItemCollection.OutlookBarItemEventArgs e)
    {
      e.Item.Changed += new EventHandler( Item_OnItemChanged );

      if( IconView == IconView.Large )
      {
        e.Item.ImageList = LargeImageList;
      }
      else if( IconView == IconView.Small )
      {
        e.Item.ImageList = SmallImageList;
      }
    }
    
    private void Items_OnItemRemoved( object sender, OutlookBarItemCollection.OutlookBarItemEventArgs e)
    {
      e.Item.Changed -= new EventHandler( Item_OnItemChanged );
    }

    private void Item_OnItemChanged( object sender, EventArgs e )
    {
      RaiseChangedEvent();
    }
    #endregion

    #region Implementation
    private void SetItemsImageList( ImageList img )
    {
      foreach( OutlookBarItem item in Items )
      {
        item.ImageList = img;
      }
    }

    protected void RaiseChangedEvent()
    {
      if( Changed != null )
      {
        Changed( this, EventArgs.Empty );
      }
    }
    protected virtual void OnChildControlChanged()
    {
      RaiseChildControlChangedEvent();
    }
    
    protected void RaiseChildControlChangedEvent()
    {
      if( ChildControlChanged != null )
      {
        ChildControlChanged( this, EventArgs.Empty );
      }
      
      RaiseChangedEvent();
    }
    
    protected virtual void OnSmallImageListChanged()
    {
      if( IconView == IconView.Small )
      {
        SetItemsImageList( SmallImageList );
      }

      RaiseSmallImageListChangedEvent();
    }
    
    protected void RaiseSmallImageListChangedEvent()
    {
      if( SmallImageListChanged != null )
      {
        SmallImageListChanged( this, EventArgs.Empty );
      }
      
      RaiseChangedEvent();
    }
    
    protected virtual void OnLargeImageListChanged()
    {
      if( IconView == IconView.Large )
      {
        SetItemsImageList( LargeImageList );
      }

      RaiseLargeImageListChangedEvent();
    }
    
    protected void RaiseLargeImageListChangedEvent()
    {
      if( LargeImageListChanged != null )
      {
        LargeImageListChanged( this, EventArgs.Empty );
      }
      
      RaiseChangedEvent();
    }
    
    protected virtual void OnBackgroundChanged()
    {
      RaiseBackgroundChangedEvent();
    }
    
    protected void RaiseBackgroundChangedEvent()
    {
      if( BackgroundChanged != null )
      {
        BackgroundChanged( this, EventArgs.Empty );
      }
      
      RaiseChangedEvent();
    }
    
    protected virtual void OnTextColorChanged()
    {
      RaiseTextColorChangedEvent();
    }
    
    protected void RaiseTextColorChangedEvent()
    {
      if( TextColorChanged != null )
      {
        TextColorChanged( this, EventArgs.Empty );
      }
      
      RaiseChangedEvent();
    }
    
    protected virtual void OnIconViewChanged()
    {
      if( IconView == IconView.Large )
      {
        SetItemsImageList( LargeImageList );
      }
      else if( IconView == IconView.Small )
      {
        SetItemsImageList( SmallImageList );
      }

      RaiseIconViewChangedEvent();
    }
    
    protected void RaiseIconViewChangedEvent()
    {
      if( IconViewChanged != null )
      {
        IconViewChanged( this, EventArgs.Empty );
      }
      
      RaiseChangedEvent();
    }
    #endregion
  }
}
