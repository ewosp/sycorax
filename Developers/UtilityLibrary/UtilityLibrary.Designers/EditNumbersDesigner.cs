using System;
using System.Windows.Forms.Design;
using System.Collections;

namespace UtilityLibrary.Designers
{
  class EditNumbersDesigner : ParentControlDesigner
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
      properties.Remove("Text");
      properties.Remove("TextAlign");
      properties.Remove("ScrollBars");
      properties.Remove("RightToLeft");
      properties.Remove("Lines");
      properties.Remove("BorderStyle");
    }
  }
}
