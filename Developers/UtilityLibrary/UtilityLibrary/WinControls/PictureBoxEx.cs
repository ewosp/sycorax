using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Drawing.Design;


namespace UtilityLibrary.WinControls
{
  [ToolboxItem(true)]
  [ToolboxBitmap(typeof(UtilityLibrary.WinControls.PictureBoxEx), "UtilityLibrary.WinControls.PictureBoxEx.bmp")]
  public class PictureBoxEx : System.Windows.Forms.PictureBox
  {
    #region Class Members
    private int m_iImageIndex = -1;
    private ImageList m_imgList = null;
    private bool m_bImageSetted;
    #endregion

    #region Class Properties
    [Browsable(true)]
    [Category("Images")]
    [Description("Gets/Sets ImageList source")]
    [DefaultValue(null)]
    public ImageList ImageList
    {
      get
      {
        return m_imgList;
      }
      set
      {
        m_imgList = value;
        OnChangeImage();
      }
    }

    [Browsable(true)]
    [Category("Images")]
    [Description("Gets/Sets Image index from ImageList")]
    [DefaultValue(-1)]
    [Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
    [TypeConverter("System.Windows.Forms.ImageIndexConverter")]
    public int ImageIndex
    {
      get
      {
        return m_iImageIndex;
      }
      set
      {
        if( value != m_iImageIndex )
        {
          m_iImageIndex = value;
          OnChangeImage();
        }
      }
    }

    #endregion 

    #region Class Initialize/Finilize methods
    public PictureBoxEx() : base()
    {
      BackColor = Color.Transparent;
      SizeMode = PictureBoxSizeMode.StretchImage;
    }

    #endregion

    #region Class Overrides
    protected virtual void OnChangeImage()
    {
      if( m_imgList != null && DesignMode == false )
      {
        if( m_iImageIndex >= 0 && m_iImageIndex < m_imgList.Images.Count )
        {
          base.Image = m_imgList.Images[ m_iImageIndex ];
        }
      }
    }

    protected override void OnPaint(System.Windows.Forms.PaintEventArgs pe)
    {
      if( m_bImageSetted == false )
      {
        m_bImageSetted = true;
        OnChangeImage();
      }
      
      pe.Graphics.FillRectangle( new SolidBrush( this.BackColor ), pe.ClipRectangle );
      base.OnPaint( pe );
      
      if( DesignMode == true )
      {
        //pe.Graphics.FillRectangle( SystemBrushes.Control, pe.ClipRectangle );
        
        if( m_imgList != null )
        {
          if( m_iImageIndex >= 0 && m_iImageIndex < m_imgList.Images.Count )
          {
            Image img = m_imgList.Images[ m_iImageIndex ];
            pe.Graphics.DrawImage( img, pe.ClipRectangle );
          }
        }
      }
    }
    #endregion
  }
}
