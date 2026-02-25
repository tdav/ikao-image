namespace Demo
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
            _tlpMain = new TableLayoutPanel();
            lstImages = new ListBox();
            picMain = new PictureBox();
            dgvData = new DataGridView();
            _tlpMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picMain).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvData).BeginInit();
            SuspendLayout();
            // 
            // _tlpMain
            // 
            _tlpMain.ColumnCount = 3;
            _tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.3338089F));
            _tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42.7960052F));
            _tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40.9415131F));
            _tlpMain.Controls.Add(lstImages, 0, 0);
            _tlpMain.Controls.Add(picMain, 1, 0);
            _tlpMain.Controls.Add(dgvData, 2, 0);
            _tlpMain.Dock = DockStyle.Fill;
            _tlpMain.Location = new Point(0, 0);
            _tlpMain.Name = "_tlpMain";
            _tlpMain.RowCount = 1;
            _tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _tlpMain.Size = new Size(1402, 784);
            _tlpMain.TabIndex = 0;
            // 
            // lstImages
            // 
            lstImages.Dock = DockStyle.Fill;
            lstImages.FormattingEnabled = true;
            lstImages.Location = new Point(3, 3);
            lstImages.Name = "lstImages";
            lstImages.Size = new Size(222, 778);
            lstImages.TabIndex = 0;
            lstImages.SelectedIndexChanged += LstImages_SelectedIndexChanged;
            // 
            // picMain
            // 
            picMain.Dock = DockStyle.Fill;
            picMain.Location = new Point(231, 3);
            picMain.Name = "picMain";
            picMain.Size = new Size(593, 778);
            picMain.SizeMode = PictureBoxSizeMode.Zoom;
            picMain.TabIndex = 1;
            picMain.TabStop = false;
            // 
            // dgvData
            // 
            dgvData.AllowUserToAddRows = false;
            dgvData.AllowUserToDeleteRows = false;
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvData.Dock = DockStyle.Fill;
            dgvData.Location = new Point(830, 3);
            dgvData.Name = "dgvData";
            dgvData.ReadOnly = true;
            dgvData.RowHeadersVisible = false;
            dgvData.Size = new Size(569, 778);
            dgvData.TabIndex = 1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1402, 784);
            Controls.Add(_tlpMain);
            MinimumSize = new Size(900, 600);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Demo";
            _tlpMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picMain).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvData).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel _tlpMain;
        private ListBox lstImages;
        private PictureBox picMain;
        private DataGridView dgvData;
    }
}
