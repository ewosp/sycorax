using System;
using System.Windows.Forms.Design;
using System.Collections;

namespace UtilityLibrary.Designers
{
  public class CalendarViewExDesigner : ParentControlDesigner
  {
    protected override void PreFilterProperties( IDictionary properties )
    {
      this.DrawGrid = false;

      base.PreFilterProperties(properties);

      properties.Remove("Dock");
      properties.Remove("Anchor");
      properties.Remove("AutoScroll");
      properties.Remove("AutoScrollMargin");
      properties.Remove("AutoScrollMinSize");
      properties.Remove("DockPadding");
      properties.Remove("Size");
      properties.Remove("DrawGrid");
    }
  }
}
