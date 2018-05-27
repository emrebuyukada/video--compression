
namespace AForgeGoruntuIsleme
{
    partial class AnaForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnaForm));
            this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            this.btnVideoYukle = new System.Windows.Forms.Button();
            this.btnFramelereAyir = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
            this.SuspendLayout();
            // 
            // axWindowsMediaPlayer1
            // 
            this.axWindowsMediaPlayer1.Enabled = true;
            this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(12, 12);
            this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
            this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(456, 231);
            this.axWindowsMediaPlayer1.TabIndex = 0;
            // 
            // btnVideoYukle
            // 
            this.btnVideoYukle.Location = new System.Drawing.Point(45, 249);
            this.btnVideoYukle.Name = "btnVideoYukle";
            this.btnVideoYukle.Size = new System.Drawing.Size(75, 23);
            this.btnVideoYukle.TabIndex = 1;
            this.btnVideoYukle.Text = "Video Yükle";
            this.btnVideoYukle.UseVisualStyleBackColor = true;
            this.btnVideoYukle.Click += new System.EventHandler(this.btnVideoYukle_Click);
            // 
            // btnFramelereAyir
            // 
            this.btnFramelereAyir.Location = new System.Drawing.Point(143, 248);
            this.btnFramelereAyir.Name = "btnFramelereAyir";
            this.btnFramelereAyir.Size = new System.Drawing.Size(89, 23);
            this.btnFramelereAyir.TabIndex = 2;
            this.btnFramelereAyir.Text = "Video Sıkıştır";
            this.btnFramelereAyir.UseVisualStyleBackColor = true;
            this.btnFramelereAyir.Click += new System.EventHandler(this.btnFramelereAyir_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 296);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(456, 23);
            this.progressBar1.TabIndex = 4;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // AnaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 331);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnFramelereAyir);
            this.Controls.Add(this.btnVideoYukle);
            this.Controls.Add(this.axWindowsMediaPlayer1);
            this.Name = "AnaForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
        private System.Windows.Forms.Button btnVideoYukle;
        private System.Windows.Forms.Button btnFramelereAyir;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}

