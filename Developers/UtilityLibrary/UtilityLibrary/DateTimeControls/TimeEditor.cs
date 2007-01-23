using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;


namespace UtilityLibrary.DateTimeControls
{
  [ToolboxItem(true), Designer( "UtilityLibrary.Designers.TimeEditorDesigner, UtilityLibrary.Designers, Version=1.0.4.0, Culture=neutral, PublicKeyToken=fe06e967d3cf723d" )]
  [ToolboxBitmap(typeof(UtilityLibrary.DateTimeControls.TimeEditor), "UtilityLibrary.DateTimeControls.TimeEditor.bmp")]
  public class TimeEditor : Control
  {
    #region Class members
    private const int DEF_WIDTH = 100;
    private const int DEF_WIDTH_NOSEC = 70;
    private const int DEF_HEIGHT = 21;

    private TimeSpan  m_time = TimeSpan.Zero;
    private bool      m_bSkipEvents;
    private bool      m_bShowSeconds = true;
    private bool      m_bMouseOver;
    private bool      m_bHightlight;
    private bool      m_bSetHours = true;
    #endregion

    #region Sub Control items
    private System.Windows.Forms.Label lblDelimeter;
    private UtilityLibrary.DateTimeControls.EditNumbers editMinutes;
    private UtilityLibrary.DateTimeControls.SpinControl spinUpDown;
    private UtilityLibrary.DateTimeControls.EditNumbers editSeconds;
    private System.Windows.Forms.Label lblDelimeter2;
    private UtilityLibrary.DateTimeControls.EditNumbers editHours;
    private System.ComponentModel.Container components = null;
    #endregion

    #region Class event
    [ Category( "Property Changed" ) ]
    public event EventHandler ValueChanged;
    public event EventHandler FocusCheck;
    #endregion

    #region Class Properties
    [DefaultValue(true)]
    [Category("Appearance")]
    public bool ShowSeconds
    {
      get
      {
        return m_bShowSeconds;
      }
      set
      {
        if ( value != m_bShowSeconds ) 
        {
          m_bShowSeconds = value;

          OnShowSecondsChanged();
        }
      }
    }
    
    /// <summary>
    /// GET/SET time control background
    /// </summary>
    public override Color BackColor
    {
      get
      {
        return base.BackColor;
      }
      set
      {
        base.BackColor = value;
        lblDelimeter.BackColor = lblDelimeter2.BackColor = value;
        editHours.BackColor = editMinutes.BackColor = editSeconds.BackColor = value;
      }
    }

    [Browsable(false)]
    protected override Size DefaultSize
    {
      get
      {
        return new Size( ( m_bShowSeconds ) ? DEF_WIDTH : DEF_WIDTH_NOSEC  , DEF_HEIGHT );
      }
    }
    
    [Browsable(true)]
    new public Size Size
    {
      get
      {
        return DefaultSize;
      }
      set
      {
        base.Size = DefaultSize;
      }
    }

    [Browsable(true), Description( "GET/SET Current Time of control" )]
    [Category("Appearance")]
    public TimeSpan Value
    {
      get
      {
        // if timeSpan value contains days then do not loose them
        if( m_time != TimeSpan.Zero )
          m_time = new TimeSpan( m_time.Days, editHours.Value, editMinutes.Value, editSeconds.Value );
        else
          m_time = new TimeSpan( editHours.Value, editMinutes.Value, editSeconds.Value );
        
        return m_time;
      }
      set
      {
        if( value != m_time )
        {
          m_time = value;

          // do not raise any event untill real data change
          bool bMode = QuietMode;
          QuietMode = true;
          
          editHours.Value = value.Hours;
          editMinutes.Value = value.Minutes;
          editSeconds.Value = value.Seconds;

          if( spinUpDown.Buddy == editHours )
          {
            spinUpDown.Pos = editHours.Value;
          }
          else if( spinUpDown.Buddy == editMinutes )
          {
            spinUpDown.Pos = editMinutes.Value;
          }
          else
          {
            spinUpDown.Pos = editSeconds.Value;
          }
          
          QuietMode = bMode;

          RaiseValueChangedEvent();
        }
      }
    }
    
    [ Browsable( false ), DefaultValue( false ) ]
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

    /// <summary>
    /// Inidcate is Control in highlighting state or not
    /// </summary>
    internal bool IsHighlighted
    {
      get
      {
        return editHours.Focused || editMinutes.Focused || m_bMouseOver || 
          editSeconds.Focused || this.Focused;
      }
    }
    /// <summary>
    /// Indicate is Control has focus or not.
    /// </summary>
    public override bool Focused
    {
      get
      {
        return editHours.Focused || editMinutes.Focused || editSeconds.Focused;
      }
    }
    
    /// <summary>
    /// If control Got Focus then it first seleted element will be hour control
    /// </summary>
    [ Category( "Behaviour" ), 
      Browsable( true ),
      DefaultValue( false ), 
      Description( "GET/SET is hours sub control will take focus when control got focus" ) ]
    public bool SetHourFocus
    {
      get
      {
        return m_bSetHours;
      }
      set
      {
        m_bSetHours = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    public TimeEditor()
    {
      SetStyle( ControlStyles.DoubleBuffer | 
        ControlStyles.UserPaint | 
        ControlStyles.AllPaintingInWmPaint, true );

      SetStyle( ControlStyles.FixedHeight | ControlStyles.FixedWidth, true );
      
      SetStyle( ControlStyles.Selectable, true );
      this.TabStop = false;

      InitializeComponent();

      BackColor = SystemColors.Window;

      AttachEvents();

      // after initialize reset option of second view
      OnShowSecondsChanged();
    }

    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
        DetachEvents();

        if( components != null )
        {
          components.Dispose();
        }
      }
      
      base.Dispose( disposing );
    }

    
    protected void AttachEvents()
    {
      EventHandler lost = new EventHandler( Control_OnFocusLost );
      EventHandler enter = new EventHandler( Control_MouseEnter );
      EventHandler leave = new EventHandler( Control_MouseLeave );
      EventHandler change = new EventHandler( Control_TextChanged );

      // attach got focus
      editHours.GotFocus    += new EventHandler( OnHoursGotFocusEventHandler );
      editMinutes.GotFocus  += new EventHandler( OnMinutesGotFocusEventHandler );
      editSeconds.GotFocus  += new EventHandler( OnSecondsGotFocusEventHandler );

      // attch Text changed
      editHours.ValueChanged   += change;
      editMinutes.ValueChanged += change;
      editSeconds.ValueChanged += change;  
      
      // attach highlighting catch events
      editHours.LostFocus   += lost;
      editMinutes.LostFocus += lost;
      editSeconds.LostFocus += lost;

      editHours.MouseEnter    += enter;
      editMinutes.MouseEnter  += enter;
      editSeconds.MouseEnter  += enter;
      spinUpDown.MouseEnter   += enter;

      lblDelimeter.MouseEnter += enter;
      lblDelimeter2.MouseEnter+= enter;

      editHours.MouseLeave    += leave;
      editMinutes.MouseLeave  += leave;
      editSeconds.MouseLeave  += leave;
      spinUpDown.MouseLeave   += leave;

      lblDelimeter.MouseLeave += leave;
      lblDelimeter2.MouseLeave+= leave;

      // attach time editors changes catchers
      editHours.Changed     += new EventHandler( OnEditorsChangedEventHandler );
      editMinutes.Changed   += new EventHandler( OnEditorsChangedEventHandler );
      editSeconds.Changed   += new EventHandler( OnEditorsChangedEventHandler );
    }

    protected void DetachEvents()
    {
      EventHandler lost = new EventHandler( Control_OnFocusLost );
      EventHandler enter = new EventHandler( Control_MouseEnter );
      EventHandler leave = new EventHandler( Control_MouseLeave );
      EventHandler change = new EventHandler( Control_TextChanged );

      editHours.GotFocus    -= new EventHandler( OnHoursGotFocusEventHandler );
      editMinutes.GotFocus  -= new EventHandler( OnMinutesGotFocusEventHandler );
      editSeconds.GotFocus  -= new EventHandler( OnSecondsGotFocusEventHandler );
      
      // detach Text changed
      editHours.TextChanged   -= change;
      editMinutes.TextChanged -= change;
      editSeconds.TextChanged -= change;  

      editHours.LostFocus   -= lost;
      editMinutes.LostFocus -= lost;
      editSeconds.LostFocus -= lost;

      editHours.MouseEnter    -= enter;
      editMinutes.MouseEnter  -= enter;
      editSeconds.MouseEnter  -= enter;
      spinUpDown.MouseEnter   -= enter;

      lblDelimeter.MouseEnter -= enter;
      lblDelimeter2.MouseEnter-= enter;

      editHours.MouseLeave    -= leave;
      editMinutes.MouseLeave  -= leave;
      editSeconds.MouseLeave  -= leave;
      spinUpDown.MouseLeave   -= leave;

      lblDelimeter.MouseLeave -= leave;
      lblDelimeter2.MouseLeave-= leave;

      editHours.Changed     -= new EventHandler( OnEditorsChangedEventHandler );
      editMinutes.Changed   -= new EventHandler( OnEditorsChangedEventHandler );
      editSeconds.Changed   -= new EventHandler( OnEditorsChangedEventHandler );
    }
    #endregion

    #region Component Designer generated code
    private void InitializeComponent()
    {
      this.lblDelimeter = new System.Windows.Forms.Label();
      this.editHours = new UtilityLibrary.DateTimeControls.EditNumbers();
      this.editMinutes = new UtilityLibrary.DateTimeControls.EditNumbers();
      this.lblDelimeter2 = new System.Windows.Forms.Label();
      this.editSeconds = new UtilityLibrary.DateTimeControls.EditNumbers();
      this.spinUpDown = new UtilityLibrary.DateTimeControls.SpinControl();
      this.SuspendLayout();
      // 
      // lblDelimeter
      // 
      this.lblDelimeter.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left);
      this.lblDelimeter.BackColor = System.Drawing.SystemColors.Window;
      this.lblDelimeter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
      this.lblDelimeter.Location = new System.Drawing.Point(22, 4);
      this.lblDelimeter.Name = "lblDelimeter";
      this.lblDelimeter.Size = new System.Drawing.Size(8, 13);
      this.lblDelimeter.TabIndex = 1;
      this.lblDelimeter.Text = ":";
      this.lblDelimeter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // editHours
      // 
      this.editHours.BackColor = System.Drawing.SystemColors.Window;
      this.editHours.ForeColor = System.Drawing.SystemColors.WindowText;
      this.editHours.Location = new System.Drawing.Point(1, 4);
      this.editHours.Max = 23;
      this.editHours.MaxLength = 2;
      this.editHours.Name = "editHours";
      this.editHours.QuietMode = false;
      this.editHours.Size = new System.Drawing.Size(22, 13);
      this.editHours.TabIndex = 0;
      // 
      // editMinutes
      // 
      this.editMinutes.BackColor = System.Drawing.SystemColors.Window;
      this.editMinutes.ForeColor = System.Drawing.SystemColors.WindowText;
      this.editMinutes.Location = new System.Drawing.Point(30, 4);
      this.editMinutes.Max = 59;
      this.editMinutes.MaxLength = 2;
      this.editMinutes.Name = "editMinutes";
      this.editMinutes.QuietMode = false;
      this.editMinutes.Size = new System.Drawing.Size(22, 13);
      this.editMinutes.TabIndex = 2;
      // 
      // lblDelimeter2
      // 
      this.lblDelimeter2.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left);
      this.lblDelimeter2.BackColor = System.Drawing.SystemColors.Window;
      this.lblDelimeter2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
      this.lblDelimeter2.Location = new System.Drawing.Point(52, 4);
      this.lblDelimeter2.Name = "lblDelimeter2";
      this.lblDelimeter2.Size = new System.Drawing.Size(8, 13);
      this.lblDelimeter2.TabIndex = 3;
      this.lblDelimeter2.Text = ":";
      this.lblDelimeter2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // editSeconds
      // 
      this.editSeconds.BackColor = System.Drawing.SystemColors.Window;
      this.editSeconds.ForeColor = System.Drawing.SystemColors.WindowText;
      this.editSeconds.Location = new System.Drawing.Point(60, 4);
      this.editSeconds.Max = 59;
      this.editSeconds.MaxLength = 2;
      this.editSeconds.Name = "editSeconds";
      this.editSeconds.QuietMode = false;
      this.editSeconds.Size = new System.Drawing.Size(22, 13);
      this.editSeconds.TabIndex = 4;
      // 
      // spinUpDown
      // 
      this.spinUpDown.BackColor = System.Drawing.SystemColors.Window;
      this.spinUpDown.Location = new System.Drawing.Point(82, 1);
      this.spinUpDown.Name = "spinUpDown";
      this.spinUpDown.Size = new System.Drawing.Size(17, 19);
      this.spinUpDown.TabIndex = 5;
      this.spinUpDown.Text = "spinControl1";
      // 
      // TimeEditor
      // 
      this.BackColor = System.Drawing.Color.Navy;
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.editHours,
                                                                  this.lblDelimeter,
                                                                  this.lblDelimeter2,
                                                                  this.spinUpDown,
                                                                  this.editMinutes,
                                                                  this.editSeconds});
      this.Name = "TimeEditor";
      this.Size = new System.Drawing.Size(100, 21);
      this.ResumeLayout(false);

    }
    #endregion

    #region Class helper methods
    private void ClearBinding()
    {
      editHours.DataBindings.Clear();
      editMinutes.DataBindings.Clear();
      editSeconds.DataBindings.Clear();
    }

    private void BindControl( EditNumbers num )
    {
      ClearBinding();
      
      spinUpDown.Buddy = num;
      
      spinUpDown.Parent = this;
      spinUpDown.Left = this.Width - spinUpDown.Width - 1;

      spinUpDown.Min = num.Min;
      spinUpDown.Max = num.Max;
      spinUpDown.Pos = num.Value;
      
      num.DataBindings.Add( "Value", spinUpDown, "Pos" );

      CheckValueChange();

      if( IsHighlighted != m_bHightlight ) 
      {
        Invalidate();
      }
    }

    /// <summary>
    /// If value of calculated time and property value is differ then raise
    /// ValueChanged event.
    /// </summary>
    private void CheckValueChange()
    {
      // check is time value changed or not
      TimeSpan temp = m_time;
      if( temp != this.Value )
      {
        RaiseValueChangedEvent();
      }
    }
    
    
    private void OnHoursGotFocusEventHandler( object sender, EventArgs e )
    {
      BindControl( editHours );
      RaiseFocusCheckEvent();
    }
    
    private void OnMinutesGotFocusEventHandler( object sender, EventArgs e )
    {
      BindControl( editMinutes );        
      RaiseFocusCheckEvent();
    }
    
    private void OnSecondsGotFocusEventHandler( object sender, EventArgs e )
    {
      BindControl( editSeconds );
      RaiseFocusCheckEvent();
    }

    private void OnEditorsChangedEventHandler( object sender, EventArgs e )
    {
      RaiseValueChangedEvent();
    }

    protected virtual  void RaiseFocusCheckEvent()
    {
      if( FocusCheck != null )
      {
        FocusCheck( this, EventArgs.Empty );
      } 
    }
    protected virtual  void RaiseValueChangedEvent()
    {
      if( ValueChanged != null && m_bSkipEvents == false )
      {
        ValueChanged( this, EventArgs.Empty );
      }
    }

    protected virtual  void OnQuietModeChanged()
    {
      // do nothing
    }

    protected virtual  void OnShowSecondsChanged()
    {
      lblDelimeter2.Visible = m_bShowSeconds;
      editSeconds.Visible = m_bShowSeconds;
      base.Size = DefaultSize;

      spinUpDown.Left = this.Width - spinUpDown.Width - 1;

      Invalidate();
    }

    
    protected override void OnEnabledChanged(System.EventArgs e)
    {
      base.OnEnabledChanged( e );

      Color clr = ( Enabled == false ) ? SystemColors.Control : SystemColors.Window;
      
      this.BackColor = clr;
      lblDelimeter.BackColor = lblDelimeter2.BackColor = clr;
      editHours.BackColor = editMinutes.BackColor = editSeconds.BackColor = clr;
    }

    protected override void OnVisibleChanged(System.EventArgs e)
    {
      base.OnVisibleChanged( e );
      
      // update visibility of sub controls
      if( Visible == true )
      { 
        lblDelimeter2.Visible = m_bShowSeconds;
        editSeconds.Visible = m_bShowSeconds;
        base.Size = DefaultSize;

        spinUpDown.Left = this.Width - spinUpDown.Width - 1;
      }
    }

    protected override void OnGotFocus(System.EventArgs e)
    {
      if( m_bSetHours ) editHours.Focus();
      base.OnGotFocus( e );
    }
    

    private void Control_OnFocusLost( object sender, EventArgs e )
    {
      CheckValueChange();

      if( IsHighlighted != m_bHightlight ) 
      {
        Invalidate();
      }
    }

    private void Control_MouseEnter( object sender, EventArgs e )
    {
      bool bOld = IsHighlighted;
      m_bMouseOver = true;

      if( bOld != IsHighlighted )
      {
        Invalidate();
      }
    }
    
    private void Control_MouseLeave( object sender, EventArgs e )
    {
      bool bOld = IsHighlighted;
      m_bMouseOver = false;
      
      if( bOld != IsHighlighted )
      {
        Invalidate();
      }
    }
    private void Control_TextChanged( object sender, EventArgs e )
    {
      CheckValueChange();
    }
    #endregion

    #region Control background drawing
    protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
    {
      Graphics g = pevent.Graphics;
      Rectangle rc = this.ClientRectangle;

      g.FillRectangle( ( Enabled == true ) ? SystemBrushes.Window : SystemBrushes.Control, rc );
      
      m_bHightlight = IsHighlighted;

      if( m_bHightlight || Enabled == false )
      {
        g.DrawRectangle( ( Enabled ) ? SystemPens.Highlight : SystemPens.GrayText, 
          rc.X, rc.Y, rc.Width-1, rc.Height-1 );
      }
    }
    #endregion
  }
}
