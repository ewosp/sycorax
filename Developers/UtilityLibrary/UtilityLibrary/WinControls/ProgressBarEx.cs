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

namespace UtilityLibrary.WinControls
{
  /// <summary>
  /// This class provide enhancment of traditionall ProgressBar
  /// </summary>
  public class ProgressBarEx : System.Windows.Forms.Control
  {
    #region Class enums
    /// <summary>
    /// We need to know how we are going to draw the progress bar
    /// this won't come from the user setting a flag but how the
    /// progress bar is constructed    
    /// </summary>
    [Flags]
      private enum ProgressBarType 
    { 
      Standard = 0x0001,
      Bitmap   = 0x0002,
      Gradient = 0x0004
    }
    #endregion

    #region Class events
    
    [ Category( "Property Changed" ) ]
    [Browsable(true), Description(" Event fired when the value of BackgroundColor property is changed on Control")]
    public event EventHandler BackgroundColorChanged;
    
    [ Category( "Property Changed" ) ]
    [Browsable(true), Description(" Event fired when the value of ForegroundColor property is changed on Control")]
    public event EventHandler ForegroundColorChanged;
    
    [ Category( "Property Changed" ) ]
    [Browsable(true), Description(" Event fired when the value of BorderColor property is changed on Control")]
    public event EventHandler BorderColorChanged;
    
    [ Category( "Property Changed" ) ]
    [Browsable(true), Description(" Event fired when the value of Border3D property is changed on Control")]
    public event EventHandler Border3DChanged;
    
    [ Category( "Property Changed" ) ]
    [Browsable(true), Description(" Event fired when the value of EnableBorder3D property is changed on Control")]
    public event EventHandler EnableBorder3DChanged;
    
    [ Category( "Property Changed" ) ]
    [Browsable(true), Description(" Event fired when the value of Value property is changed on Control")]
    public event EventHandler ValueChanged;
    
    [ Category( "Property Changed" ) ]
    [Browsable(true), Description(" Event fired when the value of Step property is changed on Control")]
    public event EventHandler StepChanged;
    
    [ Category( "Property Changed" ) ]
    [Browsable(true), Description(" Event fired when the value of Minimun property is changed on Control")]
    public event EventHandler MinimunChanged;
    
    [ Category( "Property Changed" ) ]
    [Browsable(true), Description(" Event fired when the value of Maximun property is changed on Control")]
    public event EventHandler MaximunChanged;
    
    [ Category( "Property Changed" ) ]
    [Browsable(true), Description(" Event fired when the value of Smooth property is changed on Control")]
    public event EventHandler SmoothChanged;
    
    [ Category( "Property Changed" ) ]
    [Browsable(true), Description(" Event fired when the value of ShowProgressText property is changed on Control")]
    public event EventHandler ShowProgressTextChanged;
    
    [ Category( "Property Changed" ) ]
    [Browsable(true), Description(" Event fired when the value of BackgroundBitmap property is changed on Control")]
    public event EventHandler BackgroundBitmapChanged;
    
    [ Category( "Property Changed" ) ]
    [Browsable(true), Description(" Event fired when the value of ForegroundBitmap property is changed on Control")]    
    public event EventHandler ForegroundBitmapChanged;

    [ Category( "Property Changed" ) ]
    [Browsable(true), Description(" Event fired when the value of ProgressTextHiglightColor property is changed on Control")]
    public event EventHandler ProgressTextHiglightColorChanged;
    
    [ Category( "Property Changed" ) ]
    [Browsable(true), Description(" Event fired when the value of ProgressTextColor property is changed on Control")]
    public event EventHandler ProgressTextColorChanged;
    
    [ Category( "Property Changed" ) ]
    [Browsable(true), Description(" Event fired when the value of GradientStartColor property is changed on Control")]
    public event EventHandler GradientStartColorChanged;
    
    [ Category( "Property Changed" ) ]
    [Browsable(true), Description(" Event fired when the value of GradientMiddleColor property is changed on Control")]
    public event EventHandler GradientMiddleColorChanged;   
    
    [ Category( "Property Changed" ) ]
    [Browsable(true), Description(" Event fired when the value of GradientEndColor property is changed on Control")]
    public event EventHandler GradientEndColorChanged;

    #endregion
    
    #region Class members
    private   Color   m_BackgroundColor;
    private   Color   m_ForegroundColor;
    private   Color   m_BorderColor;
    private   int     m_Value = 0;
    private   int     m_iStep = 1;
    private   int     m_iMin = 0;
    private   int     m_iMax = 100;
    private   bool    m_bSmooth = false;
    private   Border3DStyle m_Border3D = Border3DStyle.Flat;
    private   bool    m_bEnableBorder3D = false;
    private   bool    m_bShowProgressText = false;
    private   Color   m_ProgressTextHiglightColor = Color.Empty;
    private   Color   m_ProgressTextColor = Color.Empty;
    private   ProgressBarType m_BarType = ProgressBarType.Standard;
    private   Bitmap  m_ForegroundBitmap = null;
    private   Bitmap  m_BackgroundBitmap = null;
    private   Color   m_GradientStartColor = Color.Empty;
    private   Color   m_GradientMiddleColor = Color.Empty;
    private   Color   m_GradientEndColor = Color.Empty;
    #endregion
        
    #region Class constructors
    /// <summary>
    /// Initializes a new instance of the ProgressBarEx class. Without params
    /// </summary>
    public ProgressBarEx()
    {
      InitializeProgressControl(ProgressBarType.Standard, ColorUtil.VSNetControlColor, 
        ColorUtil.VSNetBorderColor, SystemColors.Highlight, null, null, Color.Empty, Color.Empty, Color.Empty);
    }

    /// <summary>
    /// Initializes a new instance of the ProgressBarEx class. With 
    /// following params
    /// </summary>
    /// <param name="m_ForegroundBitmap">Foreground image of the controls</param>
    /// <param name="backgroundBitmap">Background image of the controls</param>
    public ProgressBarEx(Bitmap foregroundBitmap, Bitmap backgroundBitmap)
    {
      InitializeProgressControl(ProgressBarType.Bitmap, ColorUtil.VSNetControlColor, 
        ColorUtil.VSNetBorderColor, ColorUtil.VSNetBorderColor, 
        foregroundBitmap, backgroundBitmap, Color.Empty, Color.Empty, Color.Empty);
    }

    /// <summary>
    /// Initializes a new instance of the ProgressBarEx class. With 
    /// following param
    /// </summary>
    /// <param name="foregroundBitmap">Foreground image of the controls</param>
    public ProgressBarEx(Bitmap foregroundBitmap)
    {
      InitializeProgressControl(ProgressBarType.Bitmap, ColorUtil.VSNetControlColor, 
        ColorUtil.VSNetBorderColor,ColorUtil.VSNetBorderColor, 
        foregroundBitmap, null, Color.Empty, Color.Empty, Color.Empty);
    }

    /// <summary>
    /// Initializes a new instance of the ProgressBarEx class. With 
    /// following params
    /// </summary>
    /// <param name="m_GradientStartColor">The color of start gradiend </param>
    /// <param name="gradientEndColor">The color of end gradiend </param>
    public ProgressBarEx(Color gradientStartColor, Color gradientEndColor)
    {
      InitializeProgressControl(ProgressBarType.Gradient, ColorUtil.VSNetControlColor, 
        ColorUtil.VSNetBorderColor, ColorUtil.VSNetBorderColor, 
        m_ForegroundBitmap, null, gradientStartColor, Color.Empty, gradientEndColor);
    }

    /// <summary>
    /// Initializes a new instance of the ProgressBarEx class. With 
    /// following params
    /// </summary>
    /// <param name="gradientStartColor">The color of start gradiend </param>
    /// <param name="gradientMiddleColor">The color of middle gradiend </param>
    /// <param name="gradientEndColor">The color of end gradiend </param>
    public ProgressBarEx(Color gradientStartColor, Color gradientMiddleColor, Color gradientEndColor)
    {
      InitializeProgressControl(ProgressBarType.Gradient, ColorUtil.VSNetControlColor, 
        ColorUtil.VSNetBorderColor, ColorUtil.VSNetBorderColor, 
        m_ForegroundBitmap, null, gradientStartColor, gradientMiddleColor, gradientEndColor);
    }

    private void InitializeProgressControl(ProgressBarType barType, Color backgroundColor, 
      Color foregroundColor, Color borderColor, Bitmap foregroundBitmap, 
      Bitmap backgroundBitmap, Color gradientStartColor,
      Color gradientMiddleColor, Color gradientEndColor)
    {
      // Setup Double buffering 
      SetStyle( ControlStyles.AllPaintingInWmPaint|
        ControlStyles.UserPaint|
        ControlStyles.ResizeRedraw |
        ControlStyles.DoubleBuffer, true);
      
      m_BarType = barType;
      m_BackgroundColor = backgroundColor;
      m_ForegroundColor = foregroundColor;
      m_BorderColor = borderColor;
      m_ForegroundBitmap = foregroundBitmap;
      m_BackgroundBitmap = backgroundBitmap;
      m_GradientStartColor = gradientStartColor;
      m_GradientMiddleColor = gradientMiddleColor;
      m_GradientEndColor = gradientEndColor;
    }
    #endregion
    
    #region Control properties
    [ Category( "Appearance" ), Description( "Background color of control" ) ]
    public Color BackgroundColor
    { 
      set 
      { 
        if ( m_BackgroundColor != value )
        {
          m_BackgroundColor = value;
          OnBackgroundColorChanged();
        }
      }
      get 
      { 
        return m_BackgroundColor; 
      }
    }
    
    [ Category( "Appearance" ), Description( "Foreground color of control" ) ]
    public Color ForegroundColor
    { 
      set
      { 
        if ( m_ForegroundColor != value )
        {
          m_ForegroundColor = value;
          OnForegroundColorChanged();
        }
      }
      get
      {
        return m_ForegroundColor; 
      }
    }

    [ Category( "Appearance" ), Description( "Border color of control" ) ]
    public Color BorderColor
    { 
      set
      { 
        if ( m_BorderColor != value )
        {
          m_BorderColor = value;
          OnBorderColorChanged();
        }
      }
      get
      {
        return m_BorderColor; 
      }
    }

    /// <summary>
    /// The current value for the ProgressBarEx, in the rang 
    /// specified by minimun and maximum properties 
    /// </summary>    
    [Category( "Behavior" )]
    [Browsable(true), Description("The current value for the ProgressBarEx, in the rang specified by minimun and maximum properties ")]
    public int Value
    { 
      set
      { 
        if ( m_Value != value )
        {
          m_Value = ( value < m_iMin ) ? m_iMin : ( ( value > m_iMax ) ? m_iMax : value );
          OnValueChanged();        
        }
      }
      get
      {
        return m_Value; 
      }
    }

    [ Category( "Layout" ), Description( "The size of the control in pixels" ) ]
    public new Size Size
    { 
      set
      { 
        // Make sure width and height dimensions are always
        // an even number so that we can do round math
        // when we draw the progress bar segments
        if( base.Size != value )
        {
          Size newSize = value;
          if ( newSize.Width % 2 != 0) newSize.Width++;
          if ( newSize.Height % 2 != 0) newSize.Height++;
          base.Size = newSize;
          OnSizeChanged( EventArgs.Empty );
        }
      }
      get
      {
        return base.Size; 
      }
    }

    /// <summary>
    /// The amount to jump the current value of the control by when the 
    /// Step() method is called
    /// </summary>
    [Category( "Behavior" )]
    [Browsable(true), Description(" The amount to jump the current value of the control by when the Step() method is called")]
    public int Step
    { 
      set
      { 
        if ( m_iStep != value )
        {
          m_iStep = value;
          OnStepChanged();
        }
      }
      get
      {
        return m_iStep; 
      }
    }

    /// <summary>
    /// The lower bound of the the range this ProgressBarEx is working with 
    /// </summary>
    [Category( "Behavior" )]
    [Browsable(true), Description(" The lower bound of the the range this ProgressBarEx is working with ")]
    public int Minimum
    { 
      set
      { 
        if( m_iMin != value )
        {
          m_iMin = value;

          if( m_iMin > m_Value ) m_Value = m_iMin;

          OnMinimumChanged();
        }
      }
      get
      {
        return m_iMin; 
      }
    }

    /// <summary>
    /// The upper bound of the the range this ProgressBarEx is working with 
    /// </summary>
    [Category( "Behavior" )]
    [Browsable(true), Description(" The upper bound of the the range this ProgressBarEx is working with ")]
    public int Maximum
    { 
      set
      {
        if ( m_iMax != value )
        {
          m_iMax = value;
          
          if( m_iMax < m_Value ) m_Value = m_iMax;

          OnMaximumChanged();
        }
      }
      get
      {
        return m_iMax; 
      }
    }

    [ Category( "Appearance" ), Description( "The smooth of the control" ) ]
    public bool Smooth
    {
      set
      {
        if ( m_bSmooth != value )
        {
          m_bSmooth = value;
          OnSmoothChanged();
        }
      }
      get
      {
        return m_bSmooth; 
      }
    }

    [ Category( "Appearance" ), Description( "Border3D style of the control" ) ]
    public Border3DStyle Border3D
    {
      set
      {
        if ( m_Border3D != value )
        {
          m_Border3D = value;
          OnBorder3DChanged();
        }
      }
      get
      {
        return m_Border3D; 
      }
    }

    [ Category( "Appearance" ), Description( "Show or Hide Border3D style of the control" ) ]
    public bool EnableBorder3D
    {
      set
      {
        if ( m_bEnableBorder3D != value )
        {
          m_bEnableBorder3D = value;
          OnEnableBorder3DChanged();
        }
      }
      get
      {
        return m_bEnableBorder3D; 
      }
    }

    [ Category( "Appearance" ), Description( "Show or Hide percent which indicate status of the control" ) ]
    public bool ShowProgressText
    {
      set
      {
        if ( m_bShowProgressText != value )
        {
          m_bShowProgressText = value;
          OnShowProgressTextChanged();
        }
      }
      get
      {
        return m_bShowProgressText; 
      }
    }

    [ Category( "Appearance" ), Description( "Progress Text Higlight color of control" ) ]
    public Color ProgressTextHiglightColor
    {
      set
      {
        if ( m_ProgressTextHiglightColor != value )
        {
          m_ProgressTextHiglightColor = value;
          OnProgressTextHiglightColorChanged();
        }
      }
      get
      {
        return m_ProgressTextHiglightColor; 
      }
    }

    [ Category( "Appearance" ), Description( "Progress Text color of control" ) ]
    public Color ProgressTextColor
    {
      set
      {
        if ( m_ProgressTextColor != value )
        {
          m_ProgressTextColor = value;
          OnProgressTextColorChanged();
        }
      }
      get
      {
        return m_ProgressTextColor; 
      }
    }

    [ Category( "Appearance" ), Description( "Foreground bitmap of control" ) ]
    public Bitmap ForegroundBitmap
    {
      set 
      {
        if ( m_ForegroundBitmap != value )
        {
          m_ForegroundBitmap = value;
          OnForegroundBitmapChanged();
        }
      }
      get
      {
        return m_ForegroundBitmap; 
      }
    }

    [ Category( "Appearance" ), Description( "Background bitmap of control" ) ]
    public Bitmap BackgroundBitmap
    {
      set
      {
        if ( m_BackgroundBitmap != value )
        {
          m_BackgroundBitmap = value;
          OnBackgroundBitmapChanged();
        }
      }
      get
      {
        return m_BackgroundBitmap; 
      }
    }

    [ Category( "Appearance" ), Description( "Gradient start color of control" ) ]
    public Color GradientStartColor
    {
      set
      {
        if ( m_GradientStartColor != value )
        {
          m_GradientStartColor = value;
          OnGradientStartColorChanged();
        }
      }
      get
      {
        return m_GradientStartColor; 
      }
    }

    [ Category( "Appearance" ), Description( "Gradient Middle color of control" ) ]
    public Color GradientMiddleColor
    {
      set
      {
        if ( m_GradientMiddleColor != value )
        {
          m_GradientMiddleColor = value;
          OnGradientMiddleColorChanged();
        }
      }
      get
      {
        return m_GradientMiddleColor; 
      }
    }

    [ Category( "Appearance" ), Description( "Gradient End color of control" ) ]
    public Color GradientEndColor
    {
      set
      {
        if ( m_GradientEndColor != value )
        {
          m_GradientEndColor = value;
          OnGradientEndColorChanged();
        }
      }
      get
      {
        return m_GradientEndColor; 
      }
    }
    #endregion

    #region Class event raisers
    protected void RaiseBackgroundColorChangedEvent()
    {
      if( BackgroundColorChanged != null /* Special Case */ )
      {
        BackgroundColorChanged( this, EventArgs.Empty );
      }
    }
    protected void RaiseForegroundColorChangedEvent()
    {
      if( ForegroundColorChanged != null /* Special Case */ )
      {
        ForegroundColorChanged( this, EventArgs.Empty );
      }
    }
    protected void RaiseBorderColorChangedEvent()
    {
      if( BorderColorChanged != null /* Special Case */ )
      {
        BorderColorChanged( this, EventArgs.Empty );
      }
    }
    protected void RaiseValueChangedEvent()
    {
      if( ValueChanged != null /* Special Case */ )
      {
        ValueChanged( this, EventArgs.Empty );
      }
    }
    protected void RaiseStepChangedEvent()
    {
      if( StepChanged != null /* Special Case */ )
      {
        StepChanged( this, EventArgs.Empty );
      }
    }
    protected void RaiseMinimumChangedEvent()
    {
      if( MinimunChanged != null /* Special Case */ )
      {
        MinimunChanged( this, EventArgs.Empty );
      }
    }
    protected void RaiseMaximumChangedEvent()
    {
      if( MaximunChanged != null /* Special Case */ )
      {
        MaximunChanged( this, EventArgs.Empty );
      }
    }
    protected void RaiseSmoothChangedEvent()
    {
      if( SmoothChanged != null /* Special Case */ )
      {
        SmoothChanged( this, EventArgs.Empty );
      }
    }
    protected void RaiseBorder3DChangedEvent()
    {
      if( Border3DChanged != null /* Special Case */ )
      {
        Border3DChanged( this, EventArgs.Empty );
      }
    }
    protected void RaiseEnableBorder3DChangedEvent()
    {
      if( EnableBorder3DChanged != null /* Special Case */ )
      {
        EnableBorder3DChanged( this, EventArgs.Empty );
      }
    }
    protected void RaiseShowProgressTextChangedEvent()
    {
      if( ShowProgressTextChanged != null /* Special Case */ )
      {
        ShowProgressTextChanged( this, EventArgs.Empty );
      }
    }
    protected void RaiseProgressTextHiglightColorChangedEvent()
    {
      if( ProgressTextHiglightColorChanged != null /* Special Case */ )
      {
        ProgressTextHiglightColorChanged( this, EventArgs.Empty );
      }
    }
    protected void RaiseProgressTextColorChangedEvent()
    {
      if( ProgressTextColorChanged != null /* Special Case */ )
      {
        ProgressTextColorChanged( this, EventArgs.Empty );
      }
    }
    protected void RaiseForegroundBitmapChangedEvent()
    {
      if( ForegroundBitmapChanged != null /* Special Case */ )
      {
        ForegroundBitmapChanged( this, EventArgs.Empty );
      }
    }

    protected void RaiseBackgroundBitmapChangedEvent()
    {
      if( BackgroundBitmapChanged != null /* Special Case */ )
      {
        BackgroundBitmapChanged( this, EventArgs.Empty );
      }
    }
    protected void RaiseGradientStartColorChangedEvent()
    {
      if( GradientStartColorChanged != null /* Special Case */ )
      {
        GradientStartColorChanged( this, EventArgs.Empty );
      }
    }
    protected void RaiseGradientMiddleColorChangedEvent()
    {
      if( GradientMiddleColorChanged != null /* Special Case */ )
      {
        GradientMiddleColorChanged( this, EventArgs.Empty );
      }
    }
    protected void RaiseGradientEndColorChangedEvent()
    {
      if( GradientEndColorChanged != null /* Special Case */ )
      {
        GradientEndColorChanged( this, EventArgs.Empty );
      }
    }

    #endregion

    #region Class overrides
    protected virtual void OnBackgroundColorChanged()
    {
      RaiseBackgroundColorChangedEvent();

      // Force a repaint of the control
      Invalidate();
    }

    protected virtual void OnForegroundColorChanged()
    {
      RaiseForegroundColorChangedEvent();
      
      // Force a repaint of the control
      Invalidate();
    }
    protected virtual void OnBorderColorChanged()
    {
      RaiseBorderColorChangedEvent();

      // Force a repaint of the control
      Invalidate();    
    }

    protected virtual void OnValueChanged()
    {
      RaiseValueChangedEvent();

      // Force a repaint of the control
      Invalidate();
    }

    protected virtual void OnStepChanged()
    {
      RaiseStepChangedEvent();

      // Force a repaint of the control
      Invalidate();
    }
    protected virtual void OnMinimumChanged()
    {
      RaiseMinimumChangedEvent();

      // Force a repaint of the control
      Invalidate();
    }
    protected virtual void OnMaximumChanged()
    {
      RaiseMaximumChangedEvent();

      // Force a repaint of the control
      Invalidate();
    }
    protected virtual void OnSmoothChanged()
    {
      RaiseSmoothChangedEvent();

      // Force a repaint of the control
      Invalidate();
    }
    protected virtual void OnBorder3DChanged()
    {
      RaiseBorder3DChangedEvent();

      // Force a repaint of the control
      Invalidate();
    }
    protected virtual void OnEnableBorder3DChanged()
    {
      RaiseEnableBorder3DChangedEvent();

      // Force a repaint of the control
      Invalidate();
    }
    protected virtual void OnShowProgressTextChanged()
    {
      RaiseShowProgressTextChangedEvent();

      // Force a repaint of the control
      Invalidate();
    }
    protected virtual void OnProgressTextHiglightColorChanged()
    {
      RaiseProgressTextHiglightColorChangedEvent();

      // Force a repaint of the control
      Invalidate();
    }
    protected virtual void OnProgressTextColorChanged()
    {
      RaiseProgressTextColorChangedEvent();

      // Force a repaint of the control
      Invalidate();
    }
    protected virtual void OnForegroundBitmapChanged()
    {
      RaiseForegroundBitmapChangedEvent();

      // Force a repaint of the control
      Invalidate();
    }
    protected virtual void OnBackgroundBitmapChanged()
    {
      RaiseBackgroundBitmapChangedEvent();

      // Force a repaint of the control
      Invalidate();
    }
    protected virtual void OnGradientStartColorChanged()
    {
      RaiseGradientStartColorChangedEvent();

      // Force a repaint of the control
      Invalidate();
    }
    protected virtual void OnGradientMiddleColorChanged()
    {
      RaiseGradientMiddleColorChangedEvent();

      // Force a repaint of the control
      Invalidate();
    }
    protected virtual void OnGradientEndColorChanged()
    {
      RaiseGradientEndColorChangedEvent();

      // Force a repaint of the control
      Invalidate();
    }


    protected override void OnResize(System.EventArgs e)
    {
      Invalidate();
      base.OnResize( e );
    }
    
    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      
      // Get window area
      Win32.RECT rc = new Win32.RECT();
      WindowsAPI.GetWindowRect(Handle, ref rc);

      // Convert to a client size rectangle
      Rectangle rect = new Rectangle(0, 0, rc.right - rc.left, rc.bottom - rc.top);

      Graphics g = e.Graphics;
      DrawBackground(g, rect);
      DrawBorder(g, rect);
      DrawForeground(g, rect);
    
    }
    protected override void OnSizeChanged(System.EventArgs e)
    {
      base.OnSizeChanged( e );
    }
    #endregion
    
    #region Class custom methods

    private int GetScaledValue()
    {
      int scaledValue = m_Value;
      Size currentSize = Size;
      
      if( m_iMax == m_iMin ) return currentSize.Width;
      scaledValue = ( m_Value - m_iMin ) * currentSize.Width /( m_iMax - m_iMin );

      return scaledValue;
    }

    /// <summary>
    /// Advances the current position of the progress bar by the 
    /// amount of the Step property.
    /// </summary>
    public void PerformStep()
    {
      if ( m_Value < m_iMax )
        Value += m_iStep;
      
      if ( m_Value > m_iMax )
        Value = m_iMax;
    }

    #endregion

    #region Paint methods
    private void DrawBorder(Graphics g, Rectangle windowRect)
    {
      if ( m_bEnableBorder3D == false )
      {
        g.DrawRectangle(new Pen(m_BorderColor), windowRect.Left, windowRect.Top,
          windowRect.Width-1, windowRect.Height-1);
      }
      else 
      {
        ControlPaint.DrawBorder3D(g, windowRect, m_Border3D);
      }
    }

    private void DrawBackground(Graphics g, Rectangle windowRect)
    {
      if ( m_BarType == ProgressBarType.Standard )
      {
        DrawStandardBackground(g, windowRect);
      }
      else if ( m_BarType == ProgressBarType.Bitmap ) 
      {
        DrawBitmapBackground(g, windowRect);
      }
      else if ( m_BarType == ProgressBarType.Gradient )
      {
        DrawGradientBackground(g, windowRect);
      }
    }

    private void DrawStandardBackground(Graphics g, Rectangle windowRect)
    {
      windowRect.Inflate(-1, -1);
      g.FillRectangle(new SolidBrush(m_BackgroundColor), windowRect);
    }

    private void DrawBitmapBackground(Graphics g, Rectangle windowRect)
    {
      if (  m_BackgroundBitmap != null )
      {
        // If we strech the bitmap most likely than not the bitmap
        // won't look good. I will draw the background bitmap just 
        // by sampling a portion of the bitmap equal to the segment width
        // -- if we were drawing segments --- and draw this over and over
        // without leaving gaps
        int segmentWidth = (windowRect.Height-4)*3/4;
        segmentWidth -= 2;
        Rectangle drawingRect = new Rectangle(windowRect.Left+1, windowRect.Top+1, segmentWidth, windowRect.Height-2);
        for ( int i = 0; i < windowRect.Width-2; i += segmentWidth) 
        {
          g.DrawImage(m_BackgroundBitmap, drawingRect.Left + i, drawingRect.Top,
            segmentWidth, windowRect.Height);
          // If last segment does not fit, just draw a portion of it
          if ( i + segmentWidth > windowRect.Width-2 )
            g.DrawImage(m_BackgroundBitmap, drawingRect.Left + i + segmentWidth, drawingRect.Top,
              windowRect.Width-2 - (drawingRect.Left + i + segmentWidth), windowRect.Height);
        }
      }
      else 
      {
        windowRect.Inflate(-1, -1);
        g.FillRectangle(new SolidBrush(m_BackgroundColor), windowRect);
      }
    }


    private void DrawGradientBackground(Graphics g, Rectangle windowRect)
    {
      // Same as the standard background
      windowRect.Inflate(-1, -1);
      g.FillRectangle(new SolidBrush(m_BackgroundColor), windowRect);
    }

    private void DrawForeground(Graphics g, Rectangle windowRect)
    {
      if ( m_BarType == ProgressBarType.Standard )
      {
        DrawStandardForeground(g, windowRect);
      }
      else if ( m_BarType == ProgressBarType.Bitmap ) 
      {
        DrawBitmapForeground(g, windowRect);
      }
      else if ( m_BarType == ProgressBarType.Gradient )
      {
        DrawGradientForeground(g, windowRect);
      }
    }

    private void DrawStandardForeground(Graphics g, Rectangle windowRect)
    {
      if ( m_bSmooth )
        DrawStandardForegroundSmooth(g, windowRect);
      else
        DrawStandardForegroundSegmented(g, windowRect);

    }

    private void DrawBitmapForeground(Graphics g, Rectangle windowRect)
    {

      // We should have a valid foreground bitmap if the type of
      // the progress bar is bitmap
      Debug.Assert(m_ForegroundBitmap != null);
      
      // If we strech the bitmap most likely than not the bitmap
      // won't look good. I will draw the foreground bitmap just 
      // by sampling a portion of the bitmap equal to the segment width
      // -- if we were drawing segments --- and draw this over and over
      // without leaving gaps
      int segmentWidth = (windowRect.Height-4)*3/4;
      segmentWidth -= 2;
                        
      Rectangle segmentRect = new Rectangle(2, 
        windowRect.Top + 2, segmentWidth, windowRect.Height-4);
              
      int progressWidth = (GetScaledValue() - 2);
      if ( progressWidth < 0 ) progressWidth = 0;
      int gap = 2;
      if ( m_bSmooth ) gap = 0;
      
      for ( int i = 0; i < progressWidth; i += segmentRect.Width+gap )
      {
        if ( i+segmentRect.Width+gap > progressWidth && (i+segmentRect.Width+gap > windowRect.Width-2-gap) ) 
        {
          // if we are about to leave because next segment does not fit
          // draw the portion that fits
          int partialWidth = progressWidth-i-2;
          Rectangle drawingRect = new Rectangle(segmentRect.Left+i, 
            segmentRect.Top, partialWidth, segmentRect.Height);
          g.DrawImage(m_ForegroundBitmap, drawingRect, 0, 0, drawingRect.Width, drawingRect.Height, GraphicsUnit.Pixel);
          break;
        }
        Rectangle completeSegment = new Rectangle(segmentRect.Left+i, segmentRect.Top, segmentRect.Width, segmentRect.Height);
        g.DrawImage(m_ForegroundBitmap, completeSegment, 0, 0, completeSegment.Width, completeSegment.Height, GraphicsUnit.Pixel);
      }
    }


    private void DrawGradientForeground(Graphics g, Rectangle windowRect)
    {
      // Three color gradient?
      bool useMiddleColor = false;
      if ( m_GradientMiddleColor != Color.Empty )
        useMiddleColor = true;

      if ( useMiddleColor )
        DrawThreeColorsGradient(g, windowRect);
      else
        DrawTwoColorsGradient(g, windowRect);
    }

    private void DrawTwoColorsGradient(Graphics g, Rectangle windowRect)
    {
      // Calculate color distance
      int redStep = Math.Max(m_GradientEndColor.R, m_GradientStartColor.R) 
        - Math.Min(m_GradientEndColor.R, m_GradientStartColor.R);
      int greenStep = Math.Max(m_GradientEndColor.G, m_GradientStartColor.G) 
        - Math.Min(m_GradientEndColor.G, m_GradientStartColor.G);
      int blueStep = Math.Max(m_GradientEndColor.B, m_GradientStartColor.B) 
        - Math.Min(m_GradientEndColor.B, m_GradientStartColor.B);

      // Do we need to increase or decrease
      int redDirection; 
      if ( m_GradientEndColor.R > m_GradientStartColor.R ) 
        redDirection = 1;
      else
        redDirection = -1;

      int greenDirection;
      if (  m_GradientEndColor.G >  m_GradientStartColor.G )
        greenDirection = 1;
      else
        greenDirection = -1;

      int blueDirection;
      if ( m_GradientEndColor.B > m_GradientStartColor.B )
        blueDirection = 1;
      else
        blueDirection = -1;
            
      // The progress control won't allow its height to be anything other than
      // and even number since the width of the segment needs to be a perfect 3/4
      // of the control (height - 4) -- Four pixels are padding --
      int segmentWidth = (windowRect.Height-4)*3/4;
      segmentWidth -= 2;

      // how many segements we need to draw
      int gap = 2;
      if ( m_bSmooth ) gap = 0;
      int numOfSegments = (windowRect.Width - 4)/(segmentWidth + gap);
      
      // calculate the actual RGB steps for every segment
      redStep /= numOfSegments;
      greenStep /= numOfSegments;
      blueStep /= numOfSegments;

      Rectangle segmentRect = new Rectangle(2, 
        windowRect.Top + 2, segmentWidth, windowRect.Height-4);
              
      int progressWidth = (GetScaledValue() - 2);
      if ( progressWidth < 0 ) progressWidth = 0;
      int counter = 0;
      for ( int i = 0; i < progressWidth; i += segmentRect.Width+gap )
      {
        Color currentColor = Color.FromArgb(m_GradientStartColor.R+(redStep*counter*redDirection), 
          m_GradientStartColor.G+(greenStep*counter*greenDirection), m_GradientStartColor.B+(blueStep*counter*blueDirection));
        if ( i+segmentRect.Width+gap > progressWidth && (i+segmentRect.Width+gap > windowRect.Width-2-gap) ) 
        {
          // if we are about to leave because next segment does not fit
          // draw the portion that fits
          int partialWidth = progressWidth-i-2;
          Rectangle drawingRect = new Rectangle(segmentRect.Left+i, 
            segmentRect.Top, partialWidth, segmentRect.Height);
          g.FillRectangle(new SolidBrush(currentColor), drawingRect);
          break;
        }
        Rectangle completeSegment = new Rectangle(segmentRect.Left+i, segmentRect.Top, segmentRect.Width, segmentRect.Height);
        g.FillRectangle(new SolidBrush(currentColor), completeSegment);
        counter++;
      }

    }

    private void DrawThreeColorsGradient(Graphics g, Rectangle windowRect)
    {
      // Calculate color distance for the first half
      int redStepFirst = Math.Max(m_GradientStartColor.R, m_GradientMiddleColor.R) 
        - Math.Min(m_GradientStartColor.R, m_GradientMiddleColor.R);
      int greenStepFirst = Math.Max(m_GradientStartColor.G, m_GradientMiddleColor.G) 
        - Math.Min(m_GradientStartColor.G, m_GradientMiddleColor.G);
      int blueStepFirst = Math.Max(m_GradientStartColor.B, m_GradientMiddleColor.B) 
        - Math.Min(m_GradientStartColor.B, m_GradientMiddleColor.B);
   
      // Calculate color distance for the second half
      int redStepSecond = Math.Max(m_GradientEndColor.R, m_GradientMiddleColor.R) 
        - Math.Min(m_GradientEndColor.R, m_GradientMiddleColor.R);
      int greenStepSecond = Math.Max(m_GradientEndColor.G, m_GradientMiddleColor.G) 
        - Math.Min(m_GradientEndColor.G, m_GradientMiddleColor.G);
      int blueStepSecond = Math.Max(m_GradientEndColor.B, m_GradientMiddleColor.B) 
        - Math.Min(m_GradientEndColor.B, m_GradientMiddleColor.B);
      
      // Do we need to increase or decrease for the first half
      int redDirectionFirst; 
      if ( m_GradientStartColor.R < m_GradientMiddleColor.R ) 
        redDirectionFirst = 1;
      else
        redDirectionFirst = -1;

      int greenDirectionFirst;
      if (  m_GradientStartColor.G <  m_GradientMiddleColor.G )
        greenDirectionFirst = 1;
      else
        greenDirectionFirst = -1;

      int blueDirectionFirst;
      if ( m_GradientStartColor.B < m_GradientMiddleColor.B )
        blueDirectionFirst = 1;
      else
        blueDirectionFirst = -1;

      // Do we need to increase or decrease for the second half
      int redDirectionSecond; 
      if ( m_GradientMiddleColor.R < m_GradientEndColor.R ) 
        redDirectionSecond = 1;
      else
        redDirectionSecond = -1;

      int greenDirectionSecond;
      if (  m_GradientMiddleColor.G <  m_GradientEndColor.G )
        greenDirectionSecond = 1;
      else
        greenDirectionSecond = -1;

      int blueDirectionSecond;
      if ( m_GradientMiddleColor.B < m_GradientEndColor.B )
        blueDirectionSecond = 1;
      else
        blueDirectionSecond = -1;

      // The progress control won't allow its height to be anything other than
      // and even number since the width of the segment needs to be a perfect 3/4
      // of the control (height - 4) -- Four pixels are padding --
      int segmentWidth = (windowRect.Height-4)*3/4;
      segmentWidth -= 2;

      // how many segements we need to draw
      int gap = 2;
      if ( m_bSmooth ) gap = 0;
      int numOfSegments = (windowRect.Width - 4)/(segmentWidth + gap);
      
      // calculate the actual RGB step for every segment
      redStepFirst /= (numOfSegments/2);
      greenStepFirst /= (numOfSegments/2);
      blueStepFirst /= (numOfSegments/2);
      redStepSecond /= (numOfSegments/2);
      greenStepSecond /= (numOfSegments/2);
      blueStepSecond /= (numOfSegments/2);

      Rectangle segmentRect = new Rectangle(2, 
        windowRect.Top + 2, segmentWidth, windowRect.Height-4);
              
      int progressWidth = (GetScaledValue() - 2);
      if ( progressWidth < 0 ) progressWidth = 0;
      int counter = 0;
      bool counterReset = true;
      for ( int i = 0; i < progressWidth; i += segmentRect.Width+gap )
      {
        Color currentColor = Color.Empty;
        if ( i < (windowRect.Width-4)/2 )
        {
          currentColor = Color.FromArgb(m_GradientStartColor.R+(redStepFirst*counter*redDirectionFirst), 
            m_GradientStartColor.G+(greenStepFirst*counter*greenDirectionFirst), 
            m_GradientStartColor.B+(blueStepFirst*counter*blueDirectionFirst));
        }
        else
        {
          if ( counterReset )
          {
            counterReset = false;
            counter = 0;
          }
          currentColor = Color.FromArgb(m_GradientMiddleColor.R+(redStepSecond*counter*redDirectionSecond), 
            m_GradientMiddleColor.G+(greenStepSecond*counter*greenDirectionSecond), 
            m_GradientMiddleColor.B+(blueStepSecond*counter*blueDirectionSecond));
        }
                
        if ( i+segmentRect.Width+gap > progressWidth && (i+segmentRect.Width+gap > windowRect.Width-2-gap) ) 
        {
          // if we are about to leave because next segment does not fit
          // draw the portion that fits
          int partialWidth = progressWidth-i-2;
          Rectangle drawingRect = new Rectangle(segmentRect.Left+i, 
            segmentRect.Top, partialWidth, segmentRect.Height);
          g.FillRectangle(new SolidBrush(currentColor), drawingRect);
          break;
        }
        Rectangle completeSegment = new Rectangle(segmentRect.Left+i, segmentRect.Top, segmentRect.Width, segmentRect.Height);
        g.FillRectangle(new SolidBrush(currentColor), completeSegment);
        counter++;
      }
    }

    private void DrawStandardForegroundSegmented(Graphics g, Rectangle windowRect)
    {
      // The progress control won't allow its height to be anything other than
      // and even number since the width of the segment needs to be a perfect 3/4
      // of the control (height - 4) -- Four pixels are padding --
      int segmentWidth = (windowRect.Height-4)*3/4;
      segmentWidth -= 2;
                        
      Rectangle segmentRect = new Rectangle(2, 
        windowRect.Top + 2, segmentWidth, windowRect.Height-4);
      
      int progressWidth = (GetScaledValue() - 2);
      if ( progressWidth < 0 ) progressWidth = 0;
      for ( int i = 0; i < progressWidth; i += segmentRect.Width+2 )
      {
        if ( i+segmentRect.Width+2 > progressWidth && (i+segmentRect.Width+2 > windowRect.Width-4) ) 
        {
          // if we are about to leave because next segment does not fit
          // draw the portion that fits
          int partialWidth = progressWidth-i-2;
          g.FillRectangle(new SolidBrush(m_ForegroundColor), 
            segmentRect.Left+i, segmentRect.Top, partialWidth, segmentRect.Height);
          break;
        }
        g.FillRectangle(new SolidBrush(m_ForegroundColor), segmentRect.Left+i, segmentRect.Top, 
          segmentRect.Width, segmentRect.Height);
      }
    }

    private void DrawStandardForegroundSmooth(Graphics g, Rectangle windowRect)
    {
      int progressWidth = (GetScaledValue() - 4);
      g.FillRectangle(new SolidBrush(m_ForegroundColor), windowRect.Left + 2, windowRect.Top+2, 
        progressWidth, windowRect.Height-4);
      if ( ShowProgressText)
      {
        int percent = GetScaledValue()*100/windowRect.Width;
        string percentageValue = percent.ToString() + " " + "%";  
        Size size = TextUtil.GetTextSize(g, percentageValue, Font);
      
        // Draw first part of the text in hightlight color in case it needs to be
        Rectangle clipRect = new Rectangle(windowRect.Left + 2, windowRect.Top+2,
          progressWidth, windowRect.Height-4);
        Point pos = new Point((windowRect.Width - size.Width)/2, (windowRect.Height - size.Height)/2);
        g.Clip = new Region(clipRect);
        Color textColor = m_ProgressTextHiglightColor;
        if ( textColor == Color.Empty )
          textColor = SystemColors.HighlightText;
        g.DrawString(percentageValue, Font, new SolidBrush(textColor), pos);

        // Draw rest in control text color if it needs to be
        clipRect = new Rectangle(progressWidth+2, windowRect.Top+2,
          windowRect.Width, windowRect.Height-4);
        g.Clip = new Region(clipRect);
        textColor = m_ProgressTextColor;
        if ( textColor == Color.Empty )
          textColor = SystemColors.ControlText;
        g.DrawString(percentageValue, Font, new SolidBrush(textColor), pos);
      }
    }
        
    #endregion

  }
}
