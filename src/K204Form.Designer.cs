namespace k204
{
    partial class K204Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(K204Form));
            this.formsPlot1 = new ScottPlot.FormsPlot();
            this.timer1 = new System.Windows.Forms.Timer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.SerialPortDropDown = new System.Windows.Forms.ToolStripDropDownButton();
            this.SerialPortConnectButton = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // formsPlot1
            // 
            this.formsPlot1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.formsPlot1.BackColor = System.Drawing.Color.Transparent;
            this.formsPlot1.Location = new System.Drawing.Point(0, 0);
            this.formsPlot1.Margin = new System.Windows.Forms.Padding(0);
            this.formsPlot1.Name = "formsPlot1";
            this.formsPlot1.Size = new System.Drawing.Size(720, 498);
            this.formsPlot1.TabIndex = 6;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SerialPortDropDown,
            this.SerialPortConnectButton});
            this.statusStrip1.Location = new System.Drawing.Point(0, 498);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(720, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // SerialPortDropDown
            // 
            this.SerialPortDropDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.SerialPortDropDown.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SerialPortDropDown.Image = ((System.Drawing.Image)(resources.GetObject("SerialPortDropDown.Image")));
            this.SerialPortDropDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SerialPortDropDown.Name = "SerialPortDropDown";
            this.SerialPortDropDown.Size = new System.Drawing.Size(78, 20);
            this.SerialPortDropDown.Text = "Serial Port";
            this.SerialPortDropDown.DropDownOpening += new System.EventHandler(this.SerialPortDropDown_DropDownOpening);
            this.SerialPortDropDown.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.SerialPortDropDown_DropDownItemClicked);
            // 
            // SerialPortConnectButton
            // 
            this.SerialPortConnectButton.Name = "SerialPortConnectButton";
            this.SerialPortConnectButton.Size = new System.Drawing.Size(52, 17);
            this.SerialPortConnectButton.Text = "Connect";
            this.SerialPortConnectButton.Click += new System.EventHandler(this.SerialPortConnectButton_Click);
            // 
            // K204Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 520);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.formsPlot1);
            this.Font = new System.Drawing.Font("Bahnschrift Condensed", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(2, 5, 2, 5);
            this.Name = "K204Form";
            this.Text = "K204 THERMOMETER";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private ScottPlot.FormsPlot formsPlot1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripDropDownButton SerialPortDropDown;
        private System.Windows.Forms.ToolStripStatusLabel SerialPortConnectButton;
    }
}

