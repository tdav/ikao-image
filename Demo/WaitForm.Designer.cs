namespace Demo
{
    partial class WaitForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            _pbProgress = new ProgressBar();
            label1 = new Label();
            SuspendLayout();
            // 
            // _pbProgress
            // 
            _pbProgress.Location = new Point(12, 26);
            _pbProgress.MarqueeAnimationSpeed = 30;
            _pbProgress.Name = "_pbProgress";
            _pbProgress.Size = new Size(276, 20);
            _pbProgress.Style = ProgressBarStyle.Marquee;
            _pbProgress.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 4);
            label1.Name = "label1";
            label1.Size = new Size(65, 15);
            label1.TabIndex = 2;
            label1.Text = "Please wait";
            // 
            // WaitForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(300, 58);
            Controls.Add(label1);
            Controls.Add(_pbProgress);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "WaitForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Please wait";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ProgressBar _pbProgress;
        private Label label1;
    }
}
