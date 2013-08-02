namespace LaserGame
{
    partial class SettingsDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDlg));
            this._txtRows = new System.Windows.Forms.NumericUpDown();
            this._txtCols = new System.Windows.Forms.NumericUpDown();
            this._txtLasers = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._chkAttemptUnique = new System.Windows.Forms.CheckBox();
            this._chkDisallowZeros = new System.Windows.Forms.CheckBox();
            this._chkDisallowOutward = new System.Windows.Forms.CheckBox();
            this._chkDisallowParallel = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this._txtRows)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._txtCols)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._txtLasers)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _txtRows
            // 
            this._txtRows.Location = new System.Drawing.Point(67, 12);
            this._txtRows.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this._txtRows.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._txtRows.Name = "_txtRows";
            this._txtRows.Size = new System.Drawing.Size(55, 20);
            this._txtRows.TabIndex = 0;
            this._txtRows.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._txtRows.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // _txtCols
            // 
            this._txtCols.Location = new System.Drawing.Point(67, 36);
            this._txtCols.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this._txtCols.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._txtCols.Name = "_txtCols";
            this._txtCols.Size = new System.Drawing.Size(55, 20);
            this._txtCols.TabIndex = 1;
            this._txtCols.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._txtCols.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // _txtLasers
            // 
            this._txtLasers.Location = new System.Drawing.Point(67, 62);
            this._txtLasers.Name = "_txtLasers";
            this._txtLasers.Size = new System.Drawing.Size(55, 20);
            this._txtLasers.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Rows:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Columns:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Lasers:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(144, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(78, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(144, 42);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(78, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._chkAttemptUnique);
            this.groupBox1.Controls.Add(this._chkDisallowZeros);
            this.groupBox1.Controls.Add(this._chkDisallowOutward);
            this.groupBox1.Controls.Add(this._chkDisallowParallel);
            this.groupBox1.Location = new System.Drawing.Point(13, 92);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(209, 123);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Constraints (experimental)";
            // 
            // _chkAttemptUnique
            // 
            this._chkAttemptUnique.AutoSize = true;
            this._chkAttemptUnique.Enabled = false;
            this._chkAttemptUnique.Location = new System.Drawing.Point(7, 92);
            this._chkAttemptUnique.Name = "_chkAttemptUnique";
            this._chkAttemptUnique.Size = new System.Drawing.Size(101, 17);
            this._chkAttemptUnique.TabIndex = 3;
            this._chkAttemptUnique.Text = "Unique Solution";
            this._chkAttemptUnique.UseVisualStyleBackColor = true;
            // 
            // _chkDisallowZeros
            // 
            this._chkDisallowZeros.AutoSize = true;
            this._chkDisallowZeros.Location = new System.Drawing.Point(7, 68);
            this._chkDisallowZeros.Name = "_chkDisallowZeros";
            this._chkDisallowZeros.Size = new System.Drawing.Size(134, 17);
            this._chkDisallowZeros.TabIndex = 2;
            this._chkDisallowZeros.Text = "Disallow Starting Zeros";
            this._chkDisallowZeros.UseVisualStyleBackColor = true;
            // 
            // _chkDisallowOutward
            // 
            this._chkDisallowOutward.AutoSize = true;
            this._chkDisallowOutward.Location = new System.Drawing.Point(7, 44);
            this._chkDisallowOutward.Name = "_chkDisallowOutward";
            this._chkDisallowOutward.Size = new System.Drawing.Size(190, 17);
            this._chkDisallowOutward.TabIndex = 1;
            this._chkDisallowOutward.Text = "Disallow Outward Boundary Lasers";
            this._chkDisallowOutward.UseVisualStyleBackColor = true;
            // 
            // _chkDisallowParallel
            // 
            this._chkDisallowParallel.AutoSize = true;
            this._chkDisallowParallel.Location = new System.Drawing.Point(7, 20);
            this._chkDisallowParallel.Name = "_chkDisallowParallel";
            this._chkDisallowParallel.Size = new System.Drawing.Size(174, 17);
            this._chkDisallowParallel.TabIndex = 0;
            this._chkDisallowParallel.Text = "Disallow Parallel Source Blocks";
            this._chkDisallowParallel.UseVisualStyleBackColor = true;
            // 
            // SettingsDlg
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(239, 227);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._txtLasers);
            this.Controls.Add(this._txtCols);
            this.Controls.Add(this._txtRows);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsDlg";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Grid/Lock Settings";
            ((System.ComponentModel.ISupportInitialize)(this._txtRows)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._txtCols)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._txtLasers)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown _txtRows;
        private System.Windows.Forms.NumericUpDown _txtCols;
        private System.Windows.Forms.NumericUpDown _txtLasers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox _chkDisallowZeros;
        private System.Windows.Forms.CheckBox _chkDisallowOutward;
        private System.Windows.Forms.CheckBox _chkDisallowParallel;
        private System.Windows.Forms.CheckBox _chkAttemptUnique;
    }
}