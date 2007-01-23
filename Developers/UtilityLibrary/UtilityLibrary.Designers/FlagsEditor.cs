using System;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace UtilityLibrary.Designers
{
  /// <summary>
  /// Implements a custom type editor for selecting a <see cref="SemiNavigationPage"/> in a list
  /// </summary>
  public class FlagsEditor : UITypeEditor
  {
    #region Class members
    private IWindowsFormsEditorService  edSvc = null;
    private CheckedListBox              clb;
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
    public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value) 
    {
      if( context != null && context.Instance != null && provider != null ) 
      {
        edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

        if( edSvc != null ) 
        {         
          // Create a CheckedListBox and populate it with all the enum values
          clb = new CheckedListBox();
          clb.BorderStyle = BorderStyle.FixedSingle;
          clb.CheckOnClick = true;
          clb.MouseDown += new MouseEventHandler(this.OnMouseDown);
          clb.MouseMove += new MouseEventHandler(this.OnMouseMoved);
          clb.DoubleClick += new EventHandler( this.OnDoubleClick );

          tooltipControl = new ToolTip();
          tooltipControl.ShowAlways = true;

          clb.BeginUpdate();

          foreach( string name in Enum.GetNames( context.PropertyDescriptor.PropertyType ) )
          {
            // Get the enum value
            object enumVal = Enum.Parse(context.PropertyDescriptor.PropertyType, name);
            // Get the int value 
            int intVal = (int) Convert.ChangeType(enumVal, typeof(int));
            
            // Get the description attribute for this field
            System.Reflection.FieldInfo fi = context.PropertyDescriptor.PropertyType.GetField(name);
            DescriptionAttribute[] attrs = ( DescriptionAttribute[] ) 
              fi.GetCustomAttributes( typeof( DescriptionAttribute ), false );
            
            BrowsableAttribute[] skipAttr = ( BrowsableAttribute[] )
              fi.GetCustomAttributes( typeof( BrowsableAttribute ), false );

            // if flag must be skip in desiner
            if( skipAttr.Length > 0 && skipAttr[0].Browsable == false ) continue;

            // Store the the description
            string tooltip = ( attrs.Length > 0 ) ? attrs[0].Description : string.Empty;

            // Get the int value of the current enum value (the one being edited)
            int intEdited = ( int )Convert.ChangeType( value, typeof( int ) );

            // show in tooltip int value of flag
            tooltip += "(value: " + intEdited.ToString() + ")";

            // Creates a clbItem that stores the name, the int value and the tooltip
            clbItem item = new clbItem(enumVal.ToString(), intVal, tooltip);

            // Get the checkstate from the value being edited
            bool checkedItem = (intEdited & intVal) > 0;

            // Add the item with the right check state
            clb.Items.Add( item, checkedItem );
          }         
          
          clb.EndUpdate();

          // Show our CheckedListbox as a DropDownControl. 
          // This methods returns only when the dropdowncontrol is closed
          edSvc.DropDownControl( clb );

          // Get the sum of all checked flags
          int result = 0;
          
          foreach( clbItem obj in clb.CheckedItems )
          {
            result += obj.Value;
          }

          // return the right enum value corresponding to the result
          return Enum.ToObject(context.PropertyDescriptor.PropertyType, result);
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
    /// When got the focus, handle the lost focus event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnMouseDown(object sender, MouseEventArgs e) 
    {
      if( !handleLostfocus && 
        clb.ClientRectangle.Contains( clb.PointToClient( new Point( e.X, e.Y ) ) ) )
      {
        clb.LostFocus += new EventHandler(this.ValueChanged);
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
    private void ValueChanged(object sender, EventArgs e) 
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
