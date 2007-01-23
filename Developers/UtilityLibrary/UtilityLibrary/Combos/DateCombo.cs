#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

using UtilityLibrary.Win32;
using UtilityLibrary.General;
using UtilityLibrary.WinControls;
using UtilityLibrary.DateTimeControls;
#endregion

namespace UtilityLibrary.Combos
{
  [ToolboxItem(true)]
  [ToolboxBitmap(typeof(UtilityLibrary.Combos.DateCombo), "UtilityLibrary.Combos.DateCombo.bmp")]
  public class DateCombo : CustomCombo
  {
    #region Class members
    protected CalendarViewEx  m_calendar = new CalendarViewEx();
    #endregion

    #region Class Properties
    [Browsable(false)]
    public CalendarViewEx CalendarDropDown
    {
      get
      {
        return m_calendar;
      }
    }

    new public DateTime Value
    {
      get
      {  
        return m_calendar.Value;
      }
      set
      {
        m_calendar.Value = value;
      }
    }
    #endregion

    #region Class constructor
    public DateCombo()
    { 
      m_calendar.DropDownTab = true;
      m_calendar.ValueChanged += new EventHandler( OnCalendarValueChanged );
      m_calendar.ClickAction += new CalendarViewEx.CalendarClickHandler( OnCalendarItem_ClickAction );
      base.Value = "None";
    }
    #endregion
    
    #region Class public methods
    public void NextDay()
    {
      OnPrevScrollItems();
    }

    public void PrevDay()
    {
      OnNextScrollItems();
    }
    #endregion

    #region Class Overrides
    protected override void OnDropDownControlBinding( CustomCombo.EventArgsBindDropDownControl e )
    {
      e.BindedControl = m_calendar;
      
      // in case when we do data load on scroll message then 
      m_ctrlBinded = m_calendar;
      m_bControlBinded = true;
    }

    protected override void OnPrevScrollItems()
    {
      if( m_calendar.Value != DateTime.MinValue )
      {
        base.Value = m_calendar.Value.AddDays(1).ToShortDateString();
      }
      else
      {
        base.Value = "None";
      }
    }

    protected override void OnNextScrollItems()
    {
      if( m_calendar.Value != DateTime.MinValue )
      {
        base.Value = m_calendar.Value.AddDays(-1).ToShortDateString();
      }
      else
      {
        base.Value = "None";
      }
    }

    protected override void OnValueChanged()
    {
      Trace.WriteLine( "DateCombo::OnValueChanged", "method call" );

      if( base.Value == "None" )
      {
        m_calendar.Value = DateTime.MinValue;
      }
      else
      {
        m_calendar.Value = DateTime.Parse( base.Value );
      }

      base.OnValueChanged();
    }

    protected override void OnDropDownSizeChanged()
    {
      if( m_dropDownForm != null )
      {
        m_dropDownForm.Size = m_calendar.Size;
      }
    }

    protected override void OnDropDownFormLocation()
    {
      Rectangle pos = RectangleToScreen( ClientRectangle );
      Rectangle work = Screen.PrimaryScreen.WorkingArea;

      if( work.Bottom > pos.Bottom + 1 + m_dropDownForm.Height )
      {
        m_dropDownForm.Location = new Point( pos.Right - m_dropDownForm.Width, pos.Bottom + 1 );
      }
      else
      {
        m_dropDownForm.Location = new Point( pos.Right - m_dropDownForm.Width, pos.Y - 1 - m_dropDownForm.Height );
      }
    }

    
    protected virtual  void OnCalendarValueChanged( object sender, EventArgs e )
    {
      Trace.WriteLine( "OnCalendarValueChanged", "method call" );

      if( m_calendar.Value == DateTime.MinValue )
      {
        base.Value = "None";
      }
      else
      {
        base.Value = m_calendar.Value.ToShortDateString();
      }
    }

    protected virtual  void OnCalendarItem_ClickAction( object sender, CalendarViewEx.TRectangleAction e )
    {
      if( e == CalendarViewEx.TRectangleAction.MonthDay )
      {
        if( base.DroppedDown == true )
        {
          base.DroppedDown = false;
        }
      }
    }
    #endregion
  }
}
