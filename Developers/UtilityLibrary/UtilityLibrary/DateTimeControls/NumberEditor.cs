using System;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;

using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.Runtime.InteropServices;

using UtilityLibrary.Win32;


namespace UtilityLibrary.DateTimeControls
{
  [ToolboxItem(false), Designer( "UtilityLibrary.Designers.EditNumbersDesigner, UtilityLibrary.Designers, Version=1.0.4.0, Culture=neutral, PublicKeyToken=fe06e967d3cf723d" )]
  public class EditNumbers : TextBox
  {
    #region Class constants
    private const int EDIT_MARGINS  = 2;
    #endregion

    #region Class members
    private bool  m_bCharMsg;
    private bool  m_bSkipEvents;
    private int   m_iValue = 0;
    private int   m_nMin = 0;
    private int   m_nMax = 60;
    private char  m_cPad = '0';

    private System.Drawing.Color oldColor;
    #endregion

    #region Class events
    [Category("Action")]
    public event EventHandler Changed;
    
    [Category("Property Changed")]
    public event EventHandler MinChanged;
    
    [Category("Property Changed")]
    public event EventHandler MaxChanged;
    
    [Category("Property Changed")]
    public event EventHandler ValueChanged;
    
    [Category("Property Changed")]
    public event EventHandler PaddingCharChanged;
    #endregion

    #region Class Properties
    [DefaultValue(0), Category("Appearance")]
    [Description("Minimal value which can be entered into control")]
    public int Min
    {
      get
      {
        return m_nMin;
      }
      set
      {
        if( value != m_nMin )
        {
          m_nMin = value;
          OnMinChanged();
        }
      }
    }

    
    [DefaultValue(60), Category("Appearance")]
    [Description("Maximal value which can be entered into control")]
    public int Max
    {
      get
      {
        return m_nMax;
      }
      set
      {
        if( value != m_nMax )
        {
          m_nMax = value;
          OnMaxChanged();
        }
      }
    }

    
    [DefaultValue(0), Category("Appearance")]
    [Description("Current value which control contains")]
    public int Value
    {
      get
      {
        if( base.Text.Length == 0 )
        { 
          Value = m_nMin;
          return m_nMin;
        }

        return CalculateValue();
      }
      set
      {
        if( value >= m_nMin && value <= m_nMax )
        {
          int nLimit = MaxLength;
          
          string number = value.ToString();
          string valueSet = "";
          
          if( nLimit > number.Length )
          {
            for( int i=0; i < nLimit - number.Length; i++ ) valueSet += m_cPad;
          }

          valueSet += number;
          
          if( base.Text != valueSet || m_iValue != CalculateValue() )
          {
            m_iValue = CalculateValue();
            base.Text = valueSet;
            OnValueChanged();
          }
        }
      }
    }

    
    [DefaultValue('0'), Category("Appearance")]
    [Description("Padding Char which fill empty positions of number")]
    public char PaddingChar
    {
      get
      {
        return m_cPad;
      }
      set
      {
        if( value != m_cPad )
        {
          int valOld = Value;
          m_cPad = value;
          Value = valOld;

          OnPaddingCharChanged();
        }
      }
    }

    
    [Browsable(false)]
    public bool QuietMode
    {
      get
      {
        return m_bSkipEvents;
      }
      set
      {
        if( value != m_bSkipEvents )
        {
          m_bSkipEvents = value;
          OnQuietModeChanged();
        }
      }
    }
    #endregion
    
    #region Class constructor
    public EditNumbers()
    {
      base.BorderStyle = BorderStyle.None;
      MaxLength = m_nMax.ToString().Length;
      Value = m_nMin;
      EnabledChanged += new EventHandler ( OnEnabledChanged );
      oldColor = ForeColor;
      OnEnabledChanged( this, new EventArgs() );
    }
    #endregion

    #region Class virtual metthods and event raisers
    protected virtual void OnEnabledChanged( object sender, EventArgs e )
    {
      if ( this.Enabled ) 
      {
        this.ForeColor = oldColor;
      }
      else
      {
        oldColor = this.ForeColor;
        this.ForeColor = System.Drawing.Color.Gray;
      }
    }
    protected virtual void OnMinChanged()
    {
      if( Value < m_nMin ) Value = m_nMin;

      RaiseMinChangedEvent();
    }

    protected virtual void OnMaxChanged()
    {
      if( Value > m_nMax ) Value = m_nMax;

      RaiseMaxChangedEvent();
    }
    
    protected virtual void OnValueChanged()
    {
      RaiseValueChangedEvent();
    }

    protected virtual void OnPaddingCharChanged()
    {
      RaisePaddingCharChangedEvent();
    }

    protected virtual void OnQuietModeChanged()
    {
      // empty method
    }

    
    protected void RaiseMinChangedEvent()
    {
      if( MinChanged != null && m_bSkipEvents == false )
      {
        MinChanged( this, EventArgs.Empty );
      }
        
      RaiseChangedEvent();
    }

    protected void RaiseMaxChangedEvent()
    {
      if( MaxChanged != null && m_bSkipEvents == false )
      {
        MaxChanged( this, EventArgs.Empty );
      }
        
      RaiseChangedEvent();
    }

    protected void RaiseValueChangedEvent()
    {
      if( ValueChanged != null && m_bSkipEvents == false )
      {
        ValueChanged( this, EventArgs.Empty );
      }
        
      RaiseChangedEvent();
    }

    protected void RaisePaddingCharChangedEvent()
    {
      if( PaddingCharChanged != null && m_bSkipEvents == false )
      {
        PaddingCharChanged( this, EventArgs.Empty );
      }
        
      RaiseChangedEvent();
    }

    protected void RaiseChangedEvent()
    {
      if( Changed != null && m_bSkipEvents == false )
      {
        Changed( this, EventArgs.Empty );
      }
    }
    #endregion
    
    #region Window Messenges handlers
    protected override System.Windows.Forms.CreateParams CreateParams
    {
      get
      {
        CreateParams param = base.CreateParams;
        
        param.Style = (int)WindowStyles.WS_CHILD | 
          (int)WindowStyles.WS_VISIBLE | 
          (int)WindowStyles.WS_TABSTOP | 
          (int)WindowStyles.WS_CLIPCHILDREN | 
          (int)WindowStyles.WS_CLIPSIBLINGS | 
          (int)EditControlStyles.ES_NUMBER |
          (int)EditControlStyles.ES_CENTER;

        return param;
      }
    }

    protected override void WndProc( ref System.Windows.Forms.Message m )
    {
      switch( m.Msg )
      {
        case (int)Msg.WM_CREATE:
          base.WndProc( ref m ) ;
          OnCreateMsg( ref m );
          break;

        case (int)Msg.WM_SETFONT:
          base.WndProc( ref m ) ;
          OnSetFontMsg( ref m );
          break;

        case (int)Msg.WM_CHAR:
          m_bCharMsg = true;
          int iSelStart = 0, iSelEnd = 0;
          WindowsAPI.SendMessage( this.Handle, (int)EditControlMsg.EM_GETSEL, ref iSelStart, ref iSelEnd ); 
          
          base.WndProc( ref m );
          
          int iValue = CalculateValue();
          
          // if value is wrong then restore old value
          if( iValue < m_nMin || iValue > m_nMax )
          { 
            WindowsAPI.SendMessage( this.Handle, (int)EditControlMsg.EM_UNDO, IntPtr.Zero, IntPtr.Zero );
            WindowsAPI.SendMessage( this.Handle, (int)EditControlMsg.EM_EMPTYUNDOBUFFER, IntPtr.Zero, IntPtr.Zero );
            WindowsAPI.SendMessage( this.Handle, (int)EditControlMsg.EM_SETSEL, iSelStart, iSelEnd );
          }

          m_bCharMsg = false;
          break;

        /*
        case (int)EditControlMsg.EM_UNDO:
          if( m_bCharMsg == true )
          {
            WindowsAPI.PostMessage( base.Handle, (int)Msg.WM_CHAR, (int)VirtualKeys.VK_BACK, 1 );
            return;
          }
          goto default;
        */

        case (int)ReflectedMessages.OCM_COMMAND:
          int hiword = ((m.WParam.ToInt32() >> 16) & 0xFFFF);
          WndCommandProc( ( EditConrolNotifyMsg )hiword, ref m );
          break;

        default:
          base.WndProc( ref m );
          break;
      }
    }

    protected void WndCommandProc( EditConrolNotifyMsg msg, ref Message m )
    {
      switch( msg )
      {
        case EditConrolNotifyMsg.EN_CHANGE:
          OnChangeMsg( ref m );
          break;

        case EditConrolNotifyMsg.EN_KILLFOCUS:
          OnKillFocusMsg( ref m );
          base.WndProc( ref m ) ;
          break;

        default:
          base.WndProc( ref m ) ;
          break;
      }
    }
    #endregion
    
    #region Class Helper methods
    protected void SetMargins( int left, int right )
    {
      WindowsAPI.SendMessage( base.Handle, 
        (int)EditControlMsg.EM_SETMARGINS, 
        (int)(EditControlSetMargin.EC_LEFTMARGIN | EditControlSetMargin.EC_RIGHTMARGIN), 
        ( left << 16 + right ) );
    }

    private int CalculateValue()
    {
      if( base.Text.Length > 0 )
      {
        if( char.IsDigit( m_cPad ) == true )
        {
          return Convert.ToInt32( base.Text );
        }
        else 
        {
          return Convert.ToInt32( base.Text.Replace( m_cPad, ' ' ) );
        }
      }

      return 0;
    }
    #endregion
    
    #region Special messeges handlers
    protected void OnCreateMsg( ref Message m )
    {
      if( m.Result.ToInt32() >= 0 )
      {
        int nLimit = 1;
        
        for( long n=m_nMax/10; n!=0; n = n/10 )
        {
          nLimit++;
        }
  
        MaxLength = nLimit;
        
        SetMargins( EDIT_MARGINS, EDIT_MARGINS );
      }
    }

    protected void OnSetFontMsg( ref Message m ) 
    {
      SetMargins( EDIT_MARGINS, EDIT_MARGINS );      
    }
    protected void OnChangeMsg( ref Message m )
    {
      string text = base.Text;
      bool bValid = ( text.Length == 0 );

      if( bValid == false )
      {
        int nVal = Value;
        bValid = ( nVal >= m_nMin && nVal <= m_nMax );
      }

      WindowsAPI.SendMessage( base.Handle, 
        ( bValid == true ) ? (int)EditControlMsg.EM_EMPTYUNDOBUFFER : (int)EditControlMsg.EM_UNDO, 
        0, 0 );
    }
    
    protected void OnKillFocusMsg( ref Message m )
    {
      if( m_cPad != 0 )
      {
        Value = Value;
      }
    }
    #endregion
  }
}
