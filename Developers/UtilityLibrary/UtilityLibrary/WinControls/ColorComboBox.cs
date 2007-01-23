using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

using UtilityLibrary.Win32;
using UtilityLibrary.General;
using UtilityLibrary.Menus;


namespace UtilityLibrary.WinControls
{
  /// <summary>
  /// Summary description for ColorComboBox.
  /// </summary>
  [ToolboxItem(true)]
  [ToolboxBitmap(typeof(UtilityLibrary.WinControls.ColorComboBox), 
     "UtilityLibrary.WinControls.ColorComboBox.bmp")]
  public class ColorComboBox : ComboBoxBase
  {
    #region Class members
    private const int PREVIEW_BOX_WIDTH = 20;
    
    private bool m_bTransShow = false;
    private IList arrayColors = new ArrayList();
    private bool m_bUseKnown = true;
    #endregion
    
    #region Constructos
    // For use when hosted by a toolbar
    public ColorComboBox(bool toolBarUse) : base(toolBarUse)
    {
      DropDownStyle = ComboBoxStyle.DropDownList;

      InitializeColorsList();      
      OnTransparentShowChanged();
    }

    public ColorComboBox()
    {
      DropDownStyle = ComboBoxStyle.DropDownList;
      InitializeColorsList();
      OnTransparentShowChanged();
    }

    
    private void InitializeColorsList()
    {
      if( m_bUseKnown )
      {
        Items.AddRange( Enum.GetNames( typeof( KnownColor ) ) );
      }
      else
      {
        if( arrayColors.Count == 0 )
        {
          foreach( PropertyInfo prop in typeof( Color ).GetProperties() )
          {
            if( prop.PropertyType == typeof( Color ) )
            {
              arrayColors.Add( prop.Name );
            }
          }
        }

        string[] colors = new string[ arrayColors.Count ];
        arrayColors.CopyTo( colors, 0 );
        Items.AddRange( colors );
      }
    }
    #endregion

    #region Class Properties
    [ Category( "Appearance" ), 
      Browsable( true ),
      DefaultValue( true ), 
      Description( "GET/SET which list of colors we use for combo fill KnownColor enum or Color class properties" ) ]
    public bool UseKnownColors
    {
      get
      {
        return m_bUseKnown;
      }
      set
      {
        if( value != m_bUseKnown )
        {
          m_bUseKnown = value;
          InitializeColorsList();
        }
      }
    }
    [Category("Behavior")]
    public bool ShowTransparentColor
    {
      get
      {
        return m_bTransShow;
      }
      set
      {
        m_bTransShow = value;
        OnTransparentShowChanged();
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

    #region Overrides
    protected override void DrawComboBoxItem(Graphics g, Rectangle bounds, int Index, bool selected, bool editSel)
    {
      
      // Call base class to do the "Flat ComboBox" drawing
      base.DrawComboBoxItem(g, bounds, Index, selected, editSel);
      if ( Index != -1)
      {
        string item = Items[Index].ToString();
        Color currentColor = Color.FromName(item);
        
        Brush brush = new SolidBrush( SystemColors.MenuText );
        Brush currentColorBrush = new SolidBrush(currentColor);

        g.FillRectangle(currentColorBrush, bounds.Left+2, bounds.Top+2, PREVIEW_BOX_WIDTH , bounds.Height-4);
        
        g.DrawRectangle( Pens.Black, new Rectangle(bounds.Left+1, bounds.Top+1, PREVIEW_BOX_WIDTH+1, bounds.Height-3));

        Size textSize = TextUtil.GetTextSize(g, Items[Index].ToString(), Font);
        int top = bounds.Top + (bounds.Height - textSize.Height)/2;
        g.DrawString(item, Font, brush, new Point(bounds.Left + 28, top));
        
        currentColorBrush.Dispose();
        brush.Dispose();
      }
    }

    protected override void DrawComboBoxItemEx(Graphics g, Rectangle bounds, int Index, bool selected, bool editSel)
    {
      
      // This "hack" is necessary to avoid a clipping bug that comes from the fact that sometimes
      // we are drawing using the Graphics object for the edit control in the combobox and sometimes
      // we are using the graphics object for the combobox itself. If we use the same function to do our custom
      // drawing it is hard to adjust for the clipping because of this situation
      base.DrawComboBoxItemEx(g, bounds, Index, selected, editSel);
      
      if( Index != -1 )
      {
        string item = Items[Index].ToString();
        Color currentColor = Color.FromName(item);
        SolidBrush brush = new SolidBrush( ( Enabled ) ? SystemColors.MenuText : SystemColors.GrayText );

        Rectangle rc = Rectangle.Inflate( bounds, -3, -3 );
        Brush currentColorBrush = new SolidBrush( currentColor );

        g.FillRectangle( currentColorBrush, rc.Left+2, rc.Top+2, PREVIEW_BOX_WIDTH , rc.Height-4 );
        g.DrawRectangle( Pens.Black, new Rectangle(rc.Left+1, rc.Top+1, PREVIEW_BOX_WIDTH+1, rc.Height-3) );

        Size textSize = TextUtil.GetTextSize(g, Items[Index].ToString(), Font);
        int top = bounds.Top + (bounds.Height - textSize.Height)/2;

        // Clipping rectangle
        Rectangle clipRect = new Rectangle(bounds.Left + 31, top, bounds.Width - 31 - ARROW_WIDTH - 4, top+textSize.Height);
        g.DrawString( Items[Index].ToString(), Font, brush, clipRect);
        
        brush.Dispose();
      }
    }
    
    protected override void DrawDisableState()
    {
      // Draw the combobox state disable
      base.DrawDisableState();
      
      // Draw the specific disable state to
      // this derive class
      using( Graphics g = CreateGraphics() )
      {
        Brush b = SystemBrushes.ControlDark;
        Rectangle rc = ClientRectangle;
        Rectangle bounds = new Rectangle(rc.Left, rc.Top, rc.Width, rc.Height);
        bounds.Inflate(-3, -3);
        g.DrawRectangle( SystemPens.ControlDark, new Rectangle( bounds.Left+2, 
          bounds.Top+2, PREVIEW_BOX_WIDTH, bounds.Height-4));

        int index = SelectedIndex;
        Size textSize = TextUtil.GetTextSize(g, Items[index].ToString(), Font);
        
        // Clipping rectangle
        int top = rc.Top + (rc.Height - textSize.Height)/2;
        Rectangle clipRect = new Rectangle(rc.Left + 31, 
          top, rc.Width - 31 - ARROW_WIDTH - 4, top+textSize.Height);
        
        g.DrawString(Items[index].ToString(), Font, b, clipRect);
      }
    }
    #endregion

    #region Methods
    public void PassMsg(ref Message m)
    {
      base.WndProc(ref m);
    }

    protected void OnTransparentShowChanged()
    {
      Items.Clear();
      
      InitializeColorsList();
      
      if( m_bTransShow == false )
      {
        Items.RemoveAt(0);
      }
    }
    #endregion
  }
}
