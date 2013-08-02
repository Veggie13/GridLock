using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LaserGame
{
    public partial class SettingsDlg : Form
    {
        public SettingsDlg(int rows, int cols, int lasers, bool disallowParallel,
            bool disallowOutward, bool disallowZeros, bool attemptUnique)
        {
            InitializeComponent();

            _txtRows.Value = rows;
            _txtCols.Value = cols;
            _txtLasers.Value = lasers;
            _chkAttemptUnique.Checked = attemptUnique;
            _chkDisallowOutward.Checked = disallowOutward;
            _chkDisallowParallel.Checked = disallowParallel;
            _chkDisallowZeros.Checked = disallowZeros;

            button1.DialogResult = DialogResult.OK;
            button2.DialogResult = DialogResult.Cancel;

            _txtLasers.Maximum = 3 * Rows * Columns / 4;
        }

        public int Rows
        {
            get { return (int)_txtRows.Value; }
        }

        public int Columns
        {
            get { return (int)_txtCols.Value; }
        }

        public int Lasers
        {
            get { return (int)_txtLasers.Value; }
        }

        public bool DisallowParallelBlocks
        {
            get { return _chkDisallowParallel.Checked; }
        }

        public bool DisallowOutwardLasers
        {
            get { return _chkDisallowOutward.Checked; }
        }

        public bool DisallowStartingZeros
        {
            get { return _chkDisallowZeros.Checked; }
        }

        public bool AttemptUniqueSolution
        {
            get { return _chkAttemptUnique.Checked; }
        }

        private void SetLaserMax()
        {
            _txtLasers.Maximum = 3 * Rows * Columns / 4;
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            SetLaserMax();
        }
    }
}
