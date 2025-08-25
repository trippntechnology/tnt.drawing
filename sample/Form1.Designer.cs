namespace Sample;

partial class Form1
{
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	/// Clean up any resources being used.
	/// </summary>
	/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	protected override void Dispose(bool disposing)
	{
		if (disposing && (components != null))
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

  #region Windows Form Designer generated code

  /// <summary>
  /// Required method for Designer support - do not modify
  /// the contents of this method with the code editor.
  /// </summary>
  private void InitializeComponent()
  {
    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
    splitContainer1 = new SplitContainer();
    toolStrip2 = new ToolStrip();
    fitToolStripButton = new ToolStripButton();
    toolStrip1 = new ToolStrip();
    toolStripButton1 = new ToolStripButton();
    toolStripButton2 = new ToolStripButton();
    toolStripButton3 = new ToolStripButton();
    menuStrip1 = new MenuStrip();
    menuToolStripMenuItem = new ToolStripMenuItem();
    fitToolStripMenuItem = new ToolStripMenuItem();
    saveToolStripMenuItem1 = new ToolStripMenuItem();
    openToolStripMenuItem1 = new ToolStripMenuItem();
    alignToolStripMenuItem = new ToolStripMenuItem();
    bringToFrontToolStripMenuItem = new ToolStripMenuItem();
    modeToolStripMenuItem = new ToolStripMenuItem();
    toolStripMenuItem1 = new ToolStripMenuItem();
    toolStripMenuItem2 = new ToolStripMenuItem();
    toolStripMenuItem3 = new ToolStripMenuItem();
    layerToolStripMenuItem = new ToolStripMenuItem();
    propertyGrid1 = new PropertyGrid();
    statusStrip1 = new StatusStrip();
    toolStripStatusLabel1 = new ToolStripStatusLabel();
    toolStripContainer1 = new ToolStripContainer();
    ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
    splitContainer1.Panel1.SuspendLayout();
    splitContainer1.Panel2.SuspendLayout();
    splitContainer1.SuspendLayout();
    toolStrip2.SuspendLayout();
    toolStrip1.SuspendLayout();
    menuStrip1.SuspendLayout();
    statusStrip1.SuspendLayout();
    toolStripContainer1.TopToolStripPanel.SuspendLayout();
    toolStripContainer1.SuspendLayout();
    SuspendLayout();
    // 
    // splitContainer1
    // 
    splitContainer1.Dock = DockStyle.Fill;
    splitContainer1.Location = new Point(0, 0);
    splitContainer1.Margin = new Padding(4, 3, 4, 3);
    splitContainer1.Name = "splitContainer1";
    // 
    // splitContainer1.Panel1
    // 
    splitContainer1.Panel1.Controls.Add(toolStripContainer1);
    splitContainer1.Panel1.Controls.Add(menuStrip1);
    // 
    // splitContainer1.Panel2
    // 
    splitContainer1.Panel2.Controls.Add(propertyGrid1);
    splitContainer1.Panel2.Controls.Add(statusStrip1);
    splitContainer1.Size = new Size(1208, 794);
    splitContainer1.SplitterDistance = 932;
    splitContainer1.SplitterWidth = 5;
    splitContainer1.TabIndex = 2;
    // 
    // toolStrip2
    // 
    toolStrip2.Dock = DockStyle.None;
    toolStrip2.Items.AddRange(new ToolStripItem[] { fitToolStripButton });
    toolStrip2.Location = new Point(84, 0);
    toolStrip2.Name = "toolStrip2";
    toolStrip2.Size = new Size(35, 25);
    toolStrip2.TabIndex = 2;
    toolStrip2.Text = "toolStrip2";
    // 
    // fitToolStripButton
    // 
    fitToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
    fitToolStripButton.Image = (Image)resources.GetObject("fitToolStripButton.Image");
    fitToolStripButton.ImageTransparentColor = Color.Magenta;
    fitToolStripButton.Name = "fitToolStripButton";
    fitToolStripButton.Size = new Size(23, 22);
    fitToolStripButton.Text = "toolStripButton4";
    // 
    // toolStrip1
    // 
    toolStrip1.Dock = DockStyle.None;
    toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButton1, toolStripButton2, toolStripButton3 });
    toolStrip1.Location = new Point(3, 0);
    toolStrip1.Name = "toolStrip1";
    toolStrip1.Size = new Size(81, 25);
    toolStrip1.TabIndex = 1;
    toolStrip1.Text = "toolStrip1";
    // 
    // toolStripButton1
    // 
    toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
    toolStripButton1.Image = (Image)resources.GetObject("toolStripButton1.Image");
    toolStripButton1.ImageTransparentColor = Color.Magenta;
    toolStripButton1.Name = "toolStripButton1";
    toolStripButton1.Size = new Size(23, 22);
    toolStripButton1.Text = "toolStripButton1";
    // 
    // toolStripButton2
    // 
    toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
    toolStripButton2.Image = (Image)resources.GetObject("toolStripButton2.Image");
    toolStripButton2.ImageTransparentColor = Color.Magenta;
    toolStripButton2.Name = "toolStripButton2";
    toolStripButton2.Size = new Size(23, 22);
    toolStripButton2.Text = "toolStripButton2";
    // 
    // toolStripButton3
    // 
    toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.Image;
    toolStripButton3.Image = (Image)resources.GetObject("toolStripButton3.Image");
    toolStripButton3.ImageTransparentColor = Color.Magenta;
    toolStripButton3.Name = "toolStripButton3";
    toolStripButton3.Size = new Size(23, 22);
    toolStripButton3.Text = "toolStripButton3";
    // 
    // menuStrip1
    // 
    menuStrip1.Items.AddRange(new ToolStripItem[] { menuToolStripMenuItem, modeToolStripMenuItem, layerToolStripMenuItem });
    menuStrip1.Location = new Point(0, 0);
    menuStrip1.Name = "menuStrip1";
    menuStrip1.Padding = new Padding(7, 2, 0, 2);
    menuStrip1.Size = new Size(932, 24);
    menuStrip1.TabIndex = 0;
    menuStrip1.Text = "menuStrip1";
    // 
    // menuToolStripMenuItem
    // 
    menuToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { fitToolStripMenuItem, saveToolStripMenuItem1, openToolStripMenuItem1, alignToolStripMenuItem, bringToFrontToolStripMenuItem });
    menuToolStripMenuItem.Name = "menuToolStripMenuItem";
    menuToolStripMenuItem.Size = new Size(50, 20);
    menuToolStripMenuItem.Text = "Menu";
    // 
    // fitToolStripMenuItem
    // 
    fitToolStripMenuItem.Name = "fitToolStripMenuItem";
    fitToolStripMenuItem.Size = new Size(147, 22);
    fitToolStripMenuItem.Text = "Fit";
    // 
    // saveToolStripMenuItem1
    // 
    saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
    saveToolStripMenuItem1.Size = new Size(147, 22);
    saveToolStripMenuItem1.Text = "Save";
    saveToolStripMenuItem1.Click += SaveToolStripMenuItem_Click;
    // 
    // openToolStripMenuItem1
    // 
    openToolStripMenuItem1.Name = "openToolStripMenuItem1";
    openToolStripMenuItem1.Size = new Size(147, 22);
    openToolStripMenuItem1.Text = "Open";
    openToolStripMenuItem1.Click += OpenToolStripMenuItem_Click;
    // 
    // alignToolStripMenuItem
    // 
    alignToolStripMenuItem.Name = "alignToolStripMenuItem";
    alignToolStripMenuItem.Size = new Size(147, 22);
    alignToolStripMenuItem.Text = "Align";
    alignToolStripMenuItem.Click += AlignToolStripMenuItem_Click;
    // 
    // bringToFrontToolStripMenuItem
    // 
    bringToFrontToolStripMenuItem.Name = "bringToFrontToolStripMenuItem";
    bringToFrontToolStripMenuItem.Size = new Size(147, 22);
    bringToFrontToolStripMenuItem.Text = "Bring to Front";
    bringToFrontToolStripMenuItem.Click += bringToFrontToolStripMenuItem_Click;
    // 
    // modeToolStripMenuItem
    // 
    modeToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem1, toolStripMenuItem2, toolStripMenuItem3 });
    modeToolStripMenuItem.Name = "modeToolStripMenuItem";
    modeToolStripMenuItem.Size = new Size(50, 20);
    modeToolStripMenuItem.Text = "Mode";
    // 
    // toolStripMenuItem1
    // 
    toolStripMenuItem1.Name = "toolStripMenuItem1";
    toolStripMenuItem1.Size = new Size(180, 22);
    toolStripMenuItem1.Text = "toolStripMenuItem1";
    // 
    // toolStripMenuItem2
    // 
    toolStripMenuItem2.Name = "toolStripMenuItem2";
    toolStripMenuItem2.Size = new Size(180, 22);
    toolStripMenuItem2.Text = "toolStripMenuItem2";
    // 
    // toolStripMenuItem3
    // 
    toolStripMenuItem3.Name = "toolStripMenuItem3";
    toolStripMenuItem3.Size = new Size(180, 22);
    toolStripMenuItem3.Text = "toolStripMenuItem3";
    // 
    // layerToolStripMenuItem
    // 
    layerToolStripMenuItem.Name = "layerToolStripMenuItem";
    layerToolStripMenuItem.Size = new Size(47, 20);
    layerToolStripMenuItem.Text = "Layer";
    // 
    // propertyGrid1
    // 
    propertyGrid1.BackColor = SystemColors.Control;
    propertyGrid1.Dock = DockStyle.Fill;
    propertyGrid1.Location = new Point(0, 0);
    propertyGrid1.Margin = new Padding(4, 3, 4, 3);
    propertyGrid1.Name = "propertyGrid1";
    propertyGrid1.Size = new Size(271, 772);
    propertyGrid1.TabIndex = 3;
    propertyGrid1.PropertyValueChanged += PropertyGrid1_PropertyValueChanged;
    // 
    // statusStrip1
    // 
    statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
    statusStrip1.Location = new Point(0, 772);
    statusStrip1.Name = "statusStrip1";
    statusStrip1.Padding = new Padding(1, 0, 16, 0);
    statusStrip1.Size = new Size(271, 22);
    statusStrip1.TabIndex = 2;
    statusStrip1.Text = "statusStrip1";
    // 
    // toolStripStatusLabel1
    // 
    toolStripStatusLabel1.Name = "toolStripStatusLabel1";
    toolStripStatusLabel1.Size = new Size(118, 17);
    toolStripStatusLabel1.Text = "toolStripStatusLabel1";
    // 
    // toolStripContainer1
    // 
    // 
    // toolStripContainer1.ContentPanel
    // 
    toolStripContainer1.ContentPanel.Size = new Size(932, 745);
    toolStripContainer1.Dock = DockStyle.Fill;
    toolStripContainer1.Location = new Point(0, 24);
    toolStripContainer1.Name = "toolStripContainer1";
    toolStripContainer1.Size = new Size(932, 770);
    toolStripContainer1.TabIndex = 3;
    toolStripContainer1.Text = "toolStripContainer1";
    // 
    // toolStripContainer1.TopToolStripPanel
    // 
    toolStripContainer1.TopToolStripPanel.Controls.Add(toolStrip2);
    toolStripContainer1.TopToolStripPanel.Controls.Add(toolStrip1);
    // 
    // Form1
    // 
    AutoScaleDimensions = new SizeF(7F, 15F);
    AutoScaleMode = AutoScaleMode.Font;
    AutoScroll = true;
    ClientSize = new Size(1208, 794);
    Controls.Add(splitContainer1);
    MainMenuStrip = menuStrip1;
    Margin = new Padding(4, 3, 4, 3);
    Name = "Form1";
    Text = "Form1";
    splitContainer1.Panel1.ResumeLayout(false);
    splitContainer1.Panel1.PerformLayout();
    splitContainer1.Panel2.ResumeLayout(false);
    splitContainer1.Panel2.PerformLayout();
    ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
    splitContainer1.ResumeLayout(false);
    toolStrip2.ResumeLayout(false);
    toolStrip2.PerformLayout();
    toolStrip1.ResumeLayout(false);
    toolStrip1.PerformLayout();
    menuStrip1.ResumeLayout(false);
    menuStrip1.PerformLayout();
    statusStrip1.ResumeLayout(false);
    statusStrip1.PerformLayout();
    toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
    toolStripContainer1.TopToolStripPanel.PerformLayout();
    toolStripContainer1.ResumeLayout(false);
    toolStripContainer1.PerformLayout();
    ResumeLayout(false);

  }

  #endregion

  private System.Windows.Forms.SplitContainer splitContainer1;
	private System.Windows.Forms.StatusStrip statusStrip1;
	private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
	private System.Windows.Forms.PropertyGrid propertyGrid1;
	private System.Windows.Forms.MenuStrip menuStrip1;
	private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
	private System.Windows.Forms.ToolStripMenuItem fitToolStripMenuItem;
	private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
	private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
	private System.Windows.Forms.ToolStripMenuItem modeToolStripMenuItem;
	private System.Windows.Forms.ToolStripMenuItem layerToolStripMenuItem;
	private System.Windows.Forms.ToolStripMenuItem alignToolStripMenuItem;
  private ToolStripMenuItem bringToFrontToolStripMenuItem;
  private ToolStrip toolStrip1;
  private ToolStripButton toolStripButton1;
  private ToolStripButton toolStripButton2;
  private ToolStripButton toolStripButton3;
  private ToolStripMenuItem toolStripMenuItem1;
  private ToolStripMenuItem toolStripMenuItem2;
  private ToolStripMenuItem toolStripMenuItem3;
  private ToolStrip toolStrip2;
  private ToolStripButton fitToolStripButton;
  private ToolStripContainer toolStripContainer1;
}

