using System;

namespace UtilityLibrary.Designers
{
  /// <summary>
  /// Internal class used for storing custom data in listviewitems
  /// </summary>
  internal class clbItem
  {
    #region Class members
    private string  m_strString;
    private int     m_iValue;
    private string  m_strTooltip;
    #endregion

    #region Class Constructor
    /// <summary>
    /// Creates a new instance of the <c>clbItem</c>
    /// </summary>
    /// <param name="str">The string to display in the <c>ToString</c> method. 
    /// It will contains the name of the flag</param>
    /// <param name="value">The integer value of the flag</param>
    /// <param name="tooltip">The tooltip to display in the <see cref="CheckedListBox"/></param>
    public clbItem( string str, int value, string tooltip )
    {
      this.m_strString  = str;
      this.m_iValue     = value;
      this.m_strTooltip = tooltip;
    }
    #endregion

    #region Class Properties
    /// <summary>
    /// Gets the int value for this item
    /// </summary>
    public int Value
    {
      get
      {
        return m_iValue;
      }
    }

    /// <summary>
    /// Gets the tooltip for this item
    /// </summary>
    public string Tooltip
    {
      get
      {
        return m_strTooltip;
      }
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Gets the name of this item
    /// </summary>
    /// <returns>The name passed in the constructor</returns>
    public override string ToString()
    {
      return m_strString;
    }
    #endregion
  }
}
