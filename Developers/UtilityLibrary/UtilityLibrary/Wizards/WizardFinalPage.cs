using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

using System.Windows.Forms;


namespace UtilityLibrary.Wizards
{
  [ToolboxItem(false)]
  [Designer( "UtilityLibrary.Designers.WizardPageDesigner, UtilityLibrary.Designers, Version=1.0.4.0, Culture=neutral, PublicKeyToken=fe06e967d3cf723d" )]
  public class WizardFinalPage : UtilityLibrary.Wizards.WizardPageBase
  {
    #region Page controls

    private System.Windows.Forms.Label lblFinishHint;
    private System.Windows.Forms.PictureBox imgWatermark;
    private System.Windows.Forms.Label lblDescription;
    private System.Windows.Forms.Label lblDescription2;
    private System.Windows.Forms.PictureBox imgWelcome;
    private System.Windows.Forms.Label lblWizardName;
    private System.ComponentModel.IContainer components = null;
    #endregion

    #region Class Properties
    protected override System.Drawing.Size DefaultSize
    {
      get
      {
        return new Size( 498, 328 );
      }
    }
    [Browsable(true)]
    [Category("Wizard Page")]
    [Description("Gets/Sets wizard page second description Text. This description used only by welocme and final pages")]
    public string Description2
    {
      get
      {
        return lblDescription2.Text;
      }
      set
      {
        if( value != lblDescription2.Text )
        {
          lblDescription2.Text = value;
          OnDescription2Changed();
        }
      }
    }
    #endregion    

    #region Class Constructor/Finilize methods
    public WizardFinalPage()
    {
      InitializeComponent();
      
      this.Size = new Size( 498, 328 );
      this.Name = "wizFinalPage";
      base.WelcomePage = true;

      base.Title       = lblWizardName.Text;
      base.Description = lblDescription.Text;
      base.HeaderImage = imgWelcome.Image;
      base.FinishPage  = true;
    }

    protected override void Dispose( bool disposing )
    {
      if( disposing && components != null )
      {
        components.Dispose();
      }

      base.Dispose( disposing );
    }
    #endregion

    #region Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.imgWatermark = new System.Windows.Forms.PictureBox();
      this.lblDescription = new System.Windows.Forms.Label();
      this.lblWizardName = new System.Windows.Forms.Label();
      this.imgWelcome = new System.Windows.Forms.PictureBox();
      this.lblFinishHint = new System.Windows.Forms.Label();
      this.lblDescription2 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // imgWatermark
      // 
      this.imgWatermark.BackColor = System.Drawing.Color.Transparent;
      this.imgWatermark.Location = new System.Drawing.Point(88, 16);
      this.imgWatermark.Name = "imgWatermark";
      this.imgWatermark.Size = new System.Drawing.Size(61, 61);
      this.imgWatermark.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.imgWatermark.TabIndex = 12;
      this.imgWatermark.TabStop = false;
      // 
      // lblDescription
      // 
      this.lblDescription.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right);
      this.lblDescription.BackColor = System.Drawing.Color.Transparent;
      this.lblDescription.Location = new System.Drawing.Point(184, 64);
      this.lblDescription.Name = "lblDescription";
      this.lblDescription.Size = new System.Drawing.Size(739, 64);
      this.lblDescription.TabIndex = 10;
      // 
      // lblWizardName
      // 
      this.lblWizardName.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right);
      this.lblWizardName.BackColor = System.Drawing.Color.Transparent;
      this.lblWizardName.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
      this.lblWizardName.Location = new System.Drawing.Point(184, 8);
      this.lblWizardName.Name = "lblWizardName";
      this.lblWizardName.Size = new System.Drawing.Size(739, 48);
      this.lblWizardName.TabIndex = 9;
      // 
      // imgWelcome
      // 
      this.imgWelcome.Dock = System.Windows.Forms.DockStyle.Left;
      this.imgWelcome.Name = "imgWelcome";
      this.imgWelcome.Size = new System.Drawing.Size(168, 783);
      this.imgWelcome.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.imgWelcome.TabIndex = 8;
      this.imgWelcome.TabStop = false;
      // 
      // lblFinishHint
      // 
      this.lblFinishHint.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right);
      this.lblFinishHint.BackColor = System.Drawing.Color.Transparent;
      this.lblFinishHint.Location = new System.Drawing.Point(184, 759);
      this.lblFinishHint.Name = "lblFinishHint";
      this.lblFinishHint.Size = new System.Drawing.Size(739, 16);
      this.lblFinishHint.TabIndex = 11;
      this.lblFinishHint.Text = "To close this Wizard, click Finish.";
      // 
      // lblDescription2
      // 
      this.lblDescription2.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right);
      this.lblDescription2.Location = new System.Drawing.Point(184, 136);
      this.lblDescription2.Name = "lblDescription2";
      this.lblDescription2.Size = new System.Drawing.Size(739, 600);
      this.lblDescription2.TabIndex = 13;
      // 
      // WizardFinalPage
      // 
      this.BackColor = System.Drawing.Color.White;
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.lblDescription2,
                                                                  this.imgWatermark,
                                                                  this.lblDescription,
                                                                  this.lblWizardName,
                                                                  this.imgWelcome,
                                                                  this.lblFinishHint});
      this.Name = "WizardFinalPage";
      this.Size = new System.Drawing.Size(941, 783);
      this.ResumeLayout(false);

    }
    #endregion

    #region Class overrides
    protected override void OnImageListChanged()
    {
      if( base.ImageList != null )
      {
        if( base.ImageIndex >= 0 && ImageIndex < base.ImageList.Images.Count )
        {
          imgWatermark.Image = base.ImageList.Images[ base.ImageIndex ];
        }
      }
    }

    protected override void OnImageIndexChanged()
    {
      if( base.ImageList != null )
      {
        if( base.ImageIndex >= 0 && ImageIndex < base.ImageList.Images.Count )
        {
          imgWatermark.Image = base.ImageList.Images[ base.ImageIndex ];
        }
      }
    }

    protected override void OnHeaderImageChanged()
    {
      imgWelcome.Image = base.HeaderImage;
    }

    protected override void OnTitleChanged()
    {
      lblWizardName.Text = base.Title;
    }

    protected override void OnDescriptionChanged()
    {
      lblDescription.Text = base.Description;
    }
    
    protected virtual  void OnDescription2Changed()
    {
    }
    protected override void OnLoad(System.EventArgs e)
    {
      OnImageListChanged();
      OnHeaderImageChanged();
      base.OnLoad(e);
    }
    #endregion
  }
}

