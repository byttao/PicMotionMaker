namespace PicMotionMaker
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            tableLayoutPanel1 = new TableLayoutPanel();
            listBoxFiles = new ListBox();
            richTextBoxLog = new RichTextBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            label1 = new Label();
            txtOutputFolder = new TextBox();
            btnFloder = new Button();
            btnStartConversion = new Button();
            btnClear = new Button();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            tableLayoutPanel1.Controls.Add(listBoxFiles, 0, 0);
            tableLayoutPanel1.Controls.Add(richTextBoxLog, 0, 2);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 1);
            tableLayoutPanel1.Controls.Add(btnStartConversion, 1, 2);
            tableLayoutPanel1.Controls.Add(btnClear, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(10);
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(784, 611);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // listBoxFiles
            // 
            listBoxFiles.AllowDrop = true;
            listBoxFiles.Dock = DockStyle.Fill;
            listBoxFiles.FormattingEnabled = true;
            listBoxFiles.ItemHeight = 17;
            listBoxFiles.Location = new Point(13, 13);
            listBoxFiles.Name = "listBoxFiles";
            listBoxFiles.Size = new Size(638, 204);
            listBoxFiles.TabIndex = 0;
            listBoxFiles.DragDrop += PanelDropTarget_DragDrop;
            listBoxFiles.DragEnter += PanelDropTarget_DragEnter;
            // 
            // richTextBoxLog
            // 
            richTextBoxLog.Dock = DockStyle.Fill;
            richTextBoxLog.Location = new Point(13, 273);
            richTextBoxLog.Name = "richTextBoxLog";
            richTextBoxLog.ReadOnly = true;
            richTextBoxLog.Size = new Size(638, 325);
            richTextBoxLog.TabIndex = 1;
            richTextBoxLog.Text = "";
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            tableLayoutPanel2.Controls.Add(label1, 0, 0);
            tableLayoutPanel2.Controls.Add(txtOutputFolder, 1, 0);
            tableLayoutPanel2.Controls.Add(btnFloder, 2, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(13, 223);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(638, 44);
            tableLayoutPanel2.TabIndex = 2;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(29, 13);
            label1.Name = "label1";
            label1.Size = new Size(68, 17);
            label1.TabIndex = 0;
            label1.Text = "视频保存：";
            // 
            // txtOutputFolder
            // 
            txtOutputFolder.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtOutputFolder.BorderStyle = BorderStyle.FixedSingle;
            txtOutputFolder.Location = new Point(103, 10);
            txtOutputFolder.Name = "txtOutputFolder";
            txtOutputFolder.ReadOnly = true;
            txtOutputFolder.Size = new Size(462, 23);
            txtOutputFolder.TabIndex = 1;
            txtOutputFolder.TextChanged += txtOutputFolder_TextChanged;
            // 
            // btnFloder
            // 
            btnFloder.Anchor = AnchorStyles.None;
            btnFloder.Location = new Point(584, 10);
            btnFloder.Name = "btnFloder";
            btnFloder.Size = new Size(37, 23);
            btnFloder.TabIndex = 2;
            btnFloder.Text = "...";
            btnFloder.UseVisualStyleBackColor = true;
            btnFloder.Click += btnFloder_Click;
            // 
            // btnStartConversion
            // 
            btnStartConversion.Anchor = AnchorStyles.None;
            btnStartConversion.Location = new Point(665, 416);
            btnStartConversion.Name = "btnStartConversion";
            btnStartConversion.Size = new Size(98, 39);
            btnStartConversion.TabIndex = 3;
            btnStartConversion.Text = "开始合成";
            btnStartConversion.UseVisualStyleBackColor = true;
            btnStartConversion.Click += btnStartConversion_Click;
            // 
            // btnClear
            // 
            btnClear.Anchor = AnchorStyles.None;
            btnClear.Location = new Point(660, 101);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(108, 27);
            btnClear.TabIndex = 4;
            btnClear.Text = "清空图片列表";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 611);
            Controls.Add(tableLayoutPanel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "图片短视频生成 V1.0 250923";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private ListBox listBoxFiles;
        private RichTextBox richTextBoxLog;
        private TableLayoutPanel tableLayoutPanel2;
        private Label label1;
        private TextBox txtOutputFolder;
        private Button btnFloder;
        private Button btnStartConversion;
        private Button btnClear;
    }
}
