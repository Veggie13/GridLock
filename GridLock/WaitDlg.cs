using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace LaserGame
{
    public partial class WaitDlg : Form
    {
        private Thread _thread;
        private System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
        public WaitDlg(Thread t)
        {
            InitializeComponent();

            _thread = t;

            _timer.Interval = 1000;
            _timer.Tick += new EventHandler(Update);

            _progBar.Style = ProgressBarStyle.Marquee;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            _timer.Start();
        }

        void Update(object sender, EventArgs e)
        {
            if (!_thread.IsAlive)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void _btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to abort?", "Abort", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                DialogResult = DialogResult.Abort;
                Close();
            }
        }

    }
}
