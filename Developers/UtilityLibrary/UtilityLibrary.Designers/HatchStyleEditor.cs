using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;


namespace UtilityLibrary.Designers
{
  public class HatchStylesEditor : UITypeEditor
  {
    #region Class members
    private IWindowsFormsEditorService  edSvc = null;
    private ListBox                     clb;
    private ToolTip                     tooltipControl;
    private bool                        handleLostfocus = false;
    #endregion

    #region Class Overrides
    /// <summary>
    /// Overrides the method used to provide basic behaviour for selecting editor.
    /// Shows our custom control for editing the value.
    /// </summary>
    /// <param name="context">The context of the editing control</param>
    /// <param name="provider">A valid service provider</param>
    /// <param name="value">The current value of the object to edit</param>
    /// <returns>The new value of the object</returns>
    public override object EditValue( ITypeDescriptorContext context, 
      IServiceProvider provider, object value) 
    {
      if( context != null && context.Instance != null && provider != null ) 
      {
        edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

        if( edSvc != null ) 
        {         
          // Create a ListBox and populate it with all the enum values
          clb = new ListBox();
          clb.BorderStyle = BorderStyle.FixedSingle;
          clb.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
          clb.MeasureItem += new MeasureItemEventHandler( this.OnMeasureItem );
          clb.DrawItem  += new DrawItemEventHandler( this.OnDrawItem );
          clb.MouseDown += new MouseEventHandler( this.OnMouseDown );
          clb.MouseMove += new MouseEventHandler( this.OnMouseMoved );
          clb.DoubleClick += new EventHandler( this.OnDoubleClick );

          tooltipControl = new ToolTip();
          tooltipControl.ShowAlways = true;

          clb.BeginUpdate();
          int iSelected = 0; // by default will be selected first item

          foreach( string name in Enum.GetNames( context.PropertyDescriptor.PropertyType ) )
          {
            // Get the enum value
            object enumVal = Enum.Parse( context.PropertyDescriptor.PropertyType, name );
            
            // Get the int value 
            int intVal = (int)Convert.ChangeType( enumVal, typeof( int ) );
            
            // Get the description attribute for this field
            System.Reflection.FieldInfo fi = context.PropertyDescriptor.PropertyType.GetField(name);
            DescriptionAttribute[] attrs = ( DescriptionAttribute[] ) 
              fi.GetCustomAttributes( typeof( DescriptionAttribute ), false );
            
            // Store the the description
            string tooltip = attrs.Length > 0 ? attrs[0].Description : string.Empty;

            // Creates a clbItem that stores the name, the int value and the tooltip
            clbItem item = new clbItem( enumVal.ToString(), intVal, tooltip );

            // Add the item with the right check state
            int index = clb.Items.Add( item );

            // set selected item
            if( intVal == (int)value )
            {
              iSelected = index;
            }
          }         
          
          clb.EndUpdate();
          clb.SelectedIndex = iSelected;

          // Show our CheckedListbox as a DropDownControl. 
          // This methods returns only when the dropdowncontrol is closed
          edSvc.DropDownControl( clb );

          // Get the sum of all checked flags
          int result = ((clbItem)clb.SelectedItem).Value;

          // return the right enum value corresponding to the result
          return Enum.ToObject( context.PropertyDescriptor.PropertyType, result );
        }
      }

      return value;
    }

    /// <summary>
    /// Shows a dropdown icon in the property editor
    /// </summary>
    /// <param name="context">The context of the editing control</param>
    /// <returns>Returns <c>UITypeEditorEditStyle.DropDown</c></returns>
    public override UITypeEditorEditStyle GetEditStyle( ITypeDescriptorContext context ) 
    {
      return UITypeEditorEditStyle.DropDown;      
    }

    #endregion

    #region Event handlers
    /// <summary>
    /// Method calculate dropdown item sizes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnMeasureItem( object sender, MeasureItemEventArgs e )
    {
      Graphics g = e.Graphics;
      string text = clb.Items[ e.Index ].ToString();

      float width = g.MeasureString( text, clb.Font ).Width;

      e.ItemWidth = 20 + (int)width + 4;
      e.ItemHeight = 14;
    }

    /// <summary>
    /// Custom draw of items in dropdown control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnDrawItem( object sender, DrawItemEventArgs e )
    {
      Graphics g = e.Graphics;
      Rectangle rc = e.Bounds;

      Rectangle rcPreView = new Rectangle( rc.X + 2, rc.Y + 2, 16, rc.Height - 4 );
      Rectangle rcText = new Rectangle( rcPreView.Right + 2, rc.Y, 
        rc.Width - rcPreView.Width - 4, rc.Height );

      string text = clb.Items[ e.Index ].ToString();

      HatchStyle style = (HatchStyle)Enum.Parse( typeof( HatchStyle ), text, true );
      HatchBrush brush = new HatchBrush( style, e.ForeColor, e.BackColor );

      // draw background
      g.FillRectangle( new SolidBrush( e.BackColor ), rc );

      // draw preview
      g.FillRectangle( brush, rcPreView );
      g.DrawRectangle( Pens.Black, rcPreView.X, rcPreView.Y, 
        rcPreView.Width - 1, rcPreView.Height - 1  );

      // draw string
      StringFormat format = new StringFormat( StringFormatFlags.LineLimit );
      format.Trimming = StringTrimming.EllipsisCharacter;
      format.LineAlignment = StringAlignment.Near;
      format.Alignment = StringAlignment.Near;

      g.DrawString( text, e.Font, new SolidBrush( e.ForeColor ), rcText, format );

      brush.Dispose();
    }

    /// <summary>
    /// When got the focus, handle the lost focus event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnMouseDown(object sender, MouseEventArgs e) 
    {
      if( !handleLostfocus && 
        clb.ClientRectangle.Contains( clb.PointToClient( new Point( e.X, e.Y ) ) ) )
      {
        clb.LostFocus += new EventHandler( this.ValueChanged );
        handleLostfocus = true;
      }
    }

    /// <summary>
    /// Occurs when the mouse is moved over the checkedlistbox. 
    /// Sets the tooltip of the item under the pointer
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnMouseMoved(object sender, MouseEventArgs e) 
    {     
      int index = clb.IndexFromPoint( e.X, e.Y );
      
      if( index >= 0 )
      {
        tooltipControl.SetToolTip( clb, ( ( clbItem )clb.Items[ index ] ).Tooltip );
      }
    }

    /// <summary>
    /// Close the dropdowncontrol when the user has selected a value
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ValueChanged( object sender, EventArgs e ) 
    {
      if( edSvc != null ) 
      {
        edSvc.CloseDropDown();
      }
    }

    /// <summary>
    /// Close dropdown control on double click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnDoubleClick( object sender, EventArgs e )
    {
      if( edSvc != null ) 
      {
        edSvc.CloseDropDown();
      }
    }
    #endregion
  }
}
