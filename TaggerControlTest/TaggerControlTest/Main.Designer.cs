namespace TaggerControlTest {
    partial class Main {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.TaggerGroupBox = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // TaggerGroupBox
            // 
            this.TaggerGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TaggerGroupBox.Location = new System.Drawing.Point(12, 12);
            this.TaggerGroupBox.Name = "TaggerGroupBox";
            this.TaggerGroupBox.Size = new System.Drawing.Size(260, 237);
            this.TaggerGroupBox.TabIndex = 0;
            this.TaggerGroupBox.TabStop = false;
            this.TaggerGroupBox.Text = "Tagger";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.TaggerGroupBox);
            this.Name = "Main";
            this.Text = "Main";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox TaggerGroupBox;
    }
}

