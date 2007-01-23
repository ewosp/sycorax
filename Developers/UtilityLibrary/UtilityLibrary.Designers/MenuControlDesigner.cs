
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace UtilityLibrary.Designers
{
  public class MenuControlDesigner :  System.Windows.Forms.Design.ParentControlDesigner
  {
    public override ICollection AssociatedComponents
    {
      get 
      {
        if (base.Control is UtilityLibrary.Menus.MenuControl)
          return ((UtilityLibrary.Menus.MenuControl)base.Control).MenuCommands;
        else
          return base.AssociatedComponents;
      }
    }

    protected override bool DrawGrid
    {
      get { return false; }
    }
  }

  public class MainMenuExDesigner : ComponentDesigner
  {

  }

}
