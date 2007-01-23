using System;
using System.Collections;
using System.Resources;

using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using UtilityLibrary.General;


namespace UtilityLibrary.WinControls
{ 
  [ToolboxItem(true)]
  [ToolboxBitmap(typeof(UtilityLibrary.WinControls.FontComboBox), 
     "UtilityLibrary.WinControls.FontComboBox.bmp")]
  public class FontComboBox : ComboBoxBase
  {
    #region Class members
    private Bitmap          m_imgTTF;
    private ResourceManager m_rm;
    #endregion

    #region Class Properties
    public Font SelectedFont
    {
      get
      {
        return new Font( base.SelectedItem.ToString(), 10 );
      }
    }
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    new public ObjectCollection Items
    {
      get
      {
        return base.Items;
      }
    }   
    #endregion

    #region Class Initialize/FinlizeMethods
    public FontComboBox(  bool toolBarUse ) : base( toolBarUse )
    {
      InitializeClass();
    }

    public FontComboBox()
    {       
      InitializeClass();
    }

    private void InitializeClass()
    {
      SetStyle(ControlStyles.AllPaintingInWmPaint
        |ControlStyles.UserPaint|ControlStyles.Opaque, false);

      m_rm = new ResourceManager( 
        "UtilityLibrary.Resources.Controls", 
        this.GetType().Assembly );

      MaxDropDownItems = 20;
      IntegralHeight = false;
      Sorted = false;
      DropDownStyle = ComboBoxStyle.DropDownList;
      DrawMode = DrawMode.OwnerDrawVariable;              
      
      m_imgTTF = (Bitmap)m_rm.GetObject( "TTFIcon" );
      m_imgTTF.MakeTransparent( m_imgTTF.GetPixel( m_imgTTF.Size.Width-1, m_imgTTF.Size.Height-1 ) );

      Populate();
    }
    #endregion

    #region Class Methods
    protected void Populate()
    {
      foreach( FontFamily ff in FontFamily.Families )
      {
        if( ff.IsStyleAvailable( FontStyle.Regular ) )
          Items.Add( ff.Name );                       
      }     
      
      if( Items.Count > 0 ) SelectedIndex=0;
    }

    protected override void DrawComboBoxItem( Graphics g, Rectangle bounds, int Index, 
      bool selected, bool editSel )
    {
      base.DrawComboBoxItem( g, bounds, Index, selected, editSel );
    
      if( Index != -1 )
      {
        Brush brush = SystemBrushes.FromSystemColor( SystemColors.MenuText );

        Font currFont = new Font( Items[Index].ToString(), 10 );

        Size textSize = TextUtil.GetTextSize( g, Items[Index].ToString(), currFont );
        int top = bounds.Top + (bounds.Height - textSize.Height)/2;

        g.DrawImage( m_imgTTF, 1, top, bounds.Height-1, bounds.Height-1 );

        g.DrawString(Items[Index].ToString(), currFont, brush, 
          new Point( bounds.Left + 1 + bounds.Height, top ) );
        
        brush.Dispose();        
      }
    }

    protected override void DrawComboBoxItemEx( Graphics g, Rectangle bounds, 
      int Index, bool selected, bool editSel )
    {
      base.DrawComboBoxItemEx( g, bounds, Index, selected, editSel );

      if( Index != -1 )
      {
        SolidBrush brush = new SolidBrush( ( Enabled ) ? SystemColors.MenuText : SystemColors.GrayText );

        Font currFont = new Font( Items[Index].ToString(), 10 );

        Size textSize = TextUtil.GetTextSize( g, Items[Index].ToString(), currFont );
        int top = bounds.Top + (bounds.Height - textSize.Height)/2;

        g.DrawImage( m_imgTTF, 1, top );

        g.DrawString( Items[Index].ToString(), currFont, brush, 
          new Point( bounds.Left + 1 + bounds.Height, top ) );
        
        brush.Dispose();        
      }
    }

    protected override void DrawDisableState()
    {
      base.DrawDisableState();
    }
    #endregion
  }

}
