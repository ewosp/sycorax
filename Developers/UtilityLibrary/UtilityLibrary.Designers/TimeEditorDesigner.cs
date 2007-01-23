using System;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Collections;

namespace UtilityLibrary.Designers
{
  class TimeEditorDesigner : ParentControlDesigner
  {
    protected override void PreFilterProperties( IDictionary properties )
    {
      this.DrawGrid = false;

      base.PreFilterProperties(properties);

      properties.Remove("Dock");
      properties.Remove("AutoScroll");
      properties.Remove("AutoScrollMargin");
      properties.Remove("AutoScrollMinSize");
      properties.Remove("DockPadding");
      properties.Remove("TextAlign");
      properties.Remove("ScrollBars");
      properties.Remove("RightToLeft");
      properties.Remove("BorderStyle");
    }

    public override System.Windows.Forms.Design.SelectionRules SelectionRules
    {
      get
      {
        return SelectionRules.Moveable | SelectionRules.Visible;
      }
    }
  }
}
