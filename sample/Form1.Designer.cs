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
    splitContainer1 = new SplitContainer();
    menuStrip1 = new MenuStrip();
    menuToolStripMenuItem = new ToolStripMenuItem();
    fitToolStripMenuItem1 = new ToolStripMenuItem();
    saveToolStripMenuItem1 = new ToolStripMenuItem();
    openToolStripMenuItem1 = new ToolStripMenuItem();
    alignToolStripMenuItem = new ToolStripMenuItem();
    modeToolStripMenuItem = new ToolStripMenuItem();
    layerToolStripMenuItem = new ToolStripMenuItem();
    propertyGrid1 = new PropertyGrid();
    statusStrip1 = new StatusStrip();
    toolStripStatusLabel1 = new ToolStripStatusLabel();
    ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
    splitContainer1.Panel1.SuspendLayout();
    splitContainer1.Panel2.SuspendLayout();
    splitContainer1.SuspendLayout();
    menuStrip1.SuspendLayout();
    statusStrip1.SuspendLayout();
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
    menuToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { fitToolStripMenuItem1, saveToolStripMenuItem1, openToolStripMenuItem1, alignToolStripMenuItem });
    menuToolStripMenuItem.Name = "menuToolStripMenuItem";
    menuToolStripMenuItem.Size = new Size(50, 20);
    menuToolStripMenuItem.Text = "Menu";
    // 
    // fitToolStripMenuItem1
    // 
    fitToolStripMenuItem1.Name = "fitToolStripMenuItem1";
    fitToolStripMenuItem1.Size = new Size(103, 22);
    fitToolStripMenuItem1.Text = "Fit";
    fitToolStripMenuItem1.Click += FitToolStripMenuItem_Click;
    // 
    // saveToolStripMenuItem1
    // 
    saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
    saveToolStripMenuItem1.Size = new Size(103, 22);
    saveToolStripMenuItem1.Text = "Save";
    saveToolStripMenuItem1.Click += SaveToolStripMenuItem_Click;
    // 
    // openToolStripMenuItem1
    // 
    openToolStripMenuItem1.Name = "openToolStripMenuItem1";
    openToolStripMenuItem1.Size = new Size(103, 22);
    openToolStripMenuItem1.Text = "Open";
    openToolStripMenuItem1.Click += OpenToolStripMenuItem_Click;
    // 
    // alignToolStripMenuItem
    // 
    alignToolStripMenuItem.Name = "alignToolStripMenuItem";
    alignToolStripMenuItem.Size = new Size(103, 22);
    alignToolStripMenuItem.Text = "Align";
    alignToolStripMenuItem.Click += AlignToolStripMenuItem_Click;
    // 
    // modeToolStripMenuItem
    // 
    modeToolStripMenuItem.Name = "modeToolStripMenuItem";
    modeToolStripMenuItem.Size = new Size(50, 20);
    modeToolStripMenuItem.Text = "Mode";
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
    menuStrip1.ResumeLayout(false);
    menuStrip1.PerformLayout();
    statusStrip1.ResumeLayout(false);
    statusStrip1.PerformLayout();
    ResumeLayout(false);

  }

  #endregion

  private System.Windows.Forms.SplitContainer splitContainer1;
	private System.Windows.Forms.StatusStrip statusStrip1;
	private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
	private System.Windows.Forms.PropertyGrid propertyGrid1;
	private System.Windows.Forms.MenuStrip menuStrip1;
	private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
	private System.Windows.Forms.ToolStripMenuItem fitToolStripMenuItem1;
	private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
	private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
	private System.Windows.Forms.ToolStripMenuItem modeToolStripMenuItem;
	private System.Windows.Forms.ToolStripMenuItem layerToolStripMenuItem;
	private System.Windows.Forms.ToolStripMenuItem alignToolStripMenuItem;
}

