using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Resources;
using System.Reflection;

namespace LaserGame
{
    public partial class GameGrid : UserControl
    {
        #region Imports
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);
        private const int WM_SETREDRAW = 11;
        #endregion

        #region Constants
        private static readonly Color DefaultBack = Color.LightGray;
        private static readonly Color LightUp = Color.LightBlue;
        private static readonly Color LaserBright = Color.Turquoise;
        private static readonly Color ErrCell = Color.Pink;
        private static readonly Color ErrLightUp = Color.Thistle;
        private static readonly Type LabelType = new Label().GetType();
        private static readonly Color[] DisplayColors = new Color[] {
            Color.Black, Color.Blue, Color.Green, Color.Red,
            Color.Purple, Color.Maroon, Color.Turquoise, Color.Gray};
        private const string FontName = "Arial";
        private const FontStyle FontProperties = FontStyle.Bold;
        private static readonly Bitmap[] BigLaserIcons;
        static GameGrid()
        {
            ResourceManager mgr = new ResourceManager("LaserGame.Resources", Assembly.GetExecutingAssembly());
            BigLaserIcons = new Bitmap[5];
            BigLaserIcons[-1 - (int)Dir.NA] = mgr.GetObject("BigLaserNA") as Bitmap;
            BigLaserIcons[-1 - (int)Dir.Up] = mgr.GetObject("BigLaserUp") as Bitmap;
            BigLaserIcons[-1 - (int)Dir.Rt] = mgr.GetObject("BigLaserRt") as Bitmap;
            BigLaserIcons[-1 - (int)Dir.Dn] = mgr.GetObject("BigLaserDn") as Bitmap;
            BigLaserIcons[-1 - (int)Dir.Lt] = mgr.GetObject("BigLaserLt") as Bitmap;
            foreach (Bitmap bm in BigLaserIcons)
                bm.MakeTransparent(Color.White);
        }
        #endregion

        #region Private Members
        private Dictionary<Label, Coord> _lookup = new Dictionary<Label, Coord>();
        private Label[,] _labels;
        private Font _normalFont;
        private Font _smallFont;
        private bool[,] _errors;
        private bool _ready = false;
        private Bitmap[] _icons = new Bitmap[BigLaserIcons.Length];
        private Dictionary<Coord, Dir> _dirs = new Dictionary<Coord, Dir>();
        #endregion

        public GameGrid()
        {
            InitializeComponent();
        }

        #region Events
        internal delegate void LaserDragHandler(Coord srcCoord, Dir dir);
        internal event LaserDragHandler LaserDragged;
        #endregion

        #region Public Methods
        public void SetDimensions(int width, int height)
        {
            Dimensions = new Size(width, height);
        }

        public Size Dimensions
        {
            get
            {
                return new Size(Columns, Rows);
            }
            set
            {
                int width = value.Width;
                int height = value.Height;

                EnableDrawing(false);
                Cursor.Current = Cursors.WaitCursor;

                _grid.Controls.Clear();
                _grid.ColumnCount = width + 2;
                _grid.RowCount = height + 2;
                _grid.ColumnStyles.Clear();
                _grid.RowStyles.Clear();

                _grid.Margin = new Padding(10);

                _labels = new Label[width, height];
                _errors = new bool[height, width];
                _lookup.Clear();

                _grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1f / (2 * (width + 1))));
                for (int x = 0; x < width; x++)
                {
                    _grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1f / (width + 1)));
                }
                _grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1f / (2 * (width + 1))));

                _grid.RowStyles.Add(new RowStyle(SizeType.Percent, 1f / (2 * (height + 1))));
                for (int y = 0; y < height; y++)
                {
                    _grid.RowStyles.Add(new RowStyle(SizeType.Percent, 1f / (height + 1)));
                }
                _grid.RowStyles.Add(new RowStyle(SizeType.Percent, 1f / (2 * (height + 1))));

                for (int x = 0; x < width; x++)
                {
                    Label label = new Label();
                    _lookup[label] = new Coord(-1, x);
                    label.BorderStyle = BorderStyle.FixedSingle;
                    label.Margin = new Padding(0);
                    label.AllowDrop = true;
                    label.BackColor = DefaultBack;
                    label.DragEnter += new DragEventHandler(LabelDrag);
                    label.DragDrop += new DragEventHandler(LabelDrop);
                    label.GiveFeedback += new GiveFeedbackEventHandler(LabelFeedback);
                    label.Dock = DockStyle.Fill;
                    _grid.Controls.Add(label, x + 1, 0);

                    label = new Label();
                    _lookup[label] = new Coord(height, x);
                    label.BorderStyle = BorderStyle.FixedSingle;
                    label.Margin = new Padding(0);
                    label.AllowDrop = true;
                    label.BackColor = DefaultBack;
                    label.DragEnter += new DragEventHandler(LabelDrag);
                    label.DragDrop += new DragEventHandler(LabelDrop);
                    label.GiveFeedback += new GiveFeedbackEventHandler(LabelFeedback);
                    label.Dock = DockStyle.Fill;
                    _grid.Controls.Add(label, x + 1, height + 1);

                    for (int y = 0; y < height; y++)
                    {
                        label = new Label();
                        _lookup[label] = new Coord(y, x);

                        label.BorderStyle = BorderStyle.Fixed3D;
                        label.TextAlign = ContentAlignment.MiddleCenter;
                        label.Margin = new Padding(0);
                        label.AllowDrop = true;
                        label.BackColor = DefaultBack;
                        label.Dock = DockStyle.Fill;

                        label.DragEnter += new DragEventHandler(LabelDrag);
                        label.DragDrop += new DragEventHandler(LabelDrop);
                        label.MouseDown += new MouseEventHandler(LabelMouseDown);
                        label.GiveFeedback += new GiveFeedbackEventHandler(LabelFeedback);

                        _grid.Controls.Add(label, x + 1, y + 1);

                        _labels[x, y] = label;
                    }
                }

                for (int y = 0; y < height; y++)
                {
                    Label label = new Label();
                    _lookup[label] = new Coord(y, -1);
                    label.BorderStyle = BorderStyle.FixedSingle;
                    label.Margin = new Padding(0);
                    label.AllowDrop = true;
                    label.BackColor = DefaultBack;
                    label.DragEnter += new DragEventHandler(LabelDrag);
                    label.DragDrop += new DragEventHandler(LabelDrop);
                    label.GiveFeedback += new GiveFeedbackEventHandler(LabelFeedback);
                    label.Dock = DockStyle.Fill;
                    _grid.Controls.Add(label, 0, y + 1);

                    label = new Label();
                    _lookup[label] = new Coord(y, width);
                    label.BorderStyle = BorderStyle.FixedSingle;
                    label.Margin = new Padding(0);
                    label.AllowDrop = true;
                    label.BackColor = DefaultBack;
                    label.DragEnter += new DragEventHandler(LabelDrag);
                    label.DragDrop += new DragEventHandler(LabelDrop);
                    label.GiveFeedback += new GiveFeedbackEventHandler(LabelFeedback);
                    label.Dock = DockStyle.Fill;
                    _grid.Controls.Add(label, width + 1, y + 1);
                }

                _ready = true;
                GetFittedFonts();
                ScaleIcons();
                RenderPuzzle();

                Cursor.Current = Cursors.Default;
                EnableDrawing(true);
            }
        }

        public int Columns
        {
            get { return _grid.ColumnCount - 2; }
            set
            {
                Dimensions = new Size(value, Rows);
            }
        }

        public int Rows
        {
            get { return _grid.RowCount - 2; }
            set
            {
                Dimensions = new Size(Columns, value);
            }
        }

        public void UpdateDisplay(int[,] puzzle, bool[,] puzLasers)
        {
            Debug.Assert(puzzle.GetLength(0) == Rows);
            Debug.Assert(puzzle.GetLength(1) == Columns);
            Debug.Assert(puzLasers.GetLength(0) == Rows);
            Debug.Assert(puzLasers.GetLength(1) == Columns);

            _dirs.Clear();
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (puzLasers[row, col])
                    {
                        _labels[col, row].Text = "";
                        _errors[row, col] = false;
                        _dirs[_lookup[_labels[col, row]]] = (Dir)puzzle[row, col];
                    }
                    else
                    {
                        _labels[col, row].Image = null;
                        if (puzzle[row, col] == 0)
                            _labels[col, row].Text = "";
                        else
                        {
                            _labels[col, row].Text = Math.Abs(puzzle[row, col]).ToString();
                            _errors[row, col] = (puzzle[row, col] < 0);
                        }
                    }
                }
            }

            RenderPuzzle();
        }
        #endregion

        #region Event Handlers
        protected override void OnSizeChanged(EventArgs e)
        {
            EnableDrawing(false);

            base.OnSizeChanged(e);

            if (!_ready || Width == 1 || Height == 1)
            {
                EnableDrawing(true);
                return;
            }

            GetFittedFonts();
            ScaleIcons();
            RenderPuzzle();

            EnableDrawing(true);
        }
        #endregion

        #region Callbacks
        void LabelFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
            Cursor.Current = Cursors.Default;
        }

        void LaserPaint(object sender, PaintEventArgs e)
        {
            Label label = sender as Label;
            if (label == null)
                return;

            ControlPaint.DrawBorder(e.Graphics, label.DisplayRectangle, LaserBright, ButtonBorderStyle.Solid);
        }

        void LabelMouseDown(object sender, MouseEventArgs e)
        {
            Label label = sender as Label;
            if (label == null)
                return;

            int row = _lookup[label].Row;
            int col = _lookup[label].Col;
            if (IsLaser(row, col))
            {
                label.Paint += new PaintEventHandler(LaserPaint);
                label.DoDragDrop(label, DragDropEffects.Link);
                label.Refresh();
            }
        }

        void LabelDrop(object sender, DragEventArgs e)
        {
            Label label = sender as Label;
            if (label == null)
                return;

            if (e.Data.GetDataPresent(LabelType))
            {
                Label src = e.Data.GetData(LabelType) as Label;
                if (src == null)
                    return;
                src.Paint -= LaserPaint;

                Coord srcCoord = _lookup[src];
                EmitLaserDragged(srcCoord, DirBetween(srcCoord, _lookup[label]));
            }
        }

        void LabelDrag(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(LabelType))
            {
                e.Effect = DragDropEffects.Link;

                Label label = sender as Label;
                if (label == null)
                    return;

                Label src = e.Data.GetData(LabelType) as Label;
                if (src == null)
                    return;
                src.Refresh();

                RenderPuzzle();

                int srcRow = _lookup[src].Row;
                int srcCol = _lookup[src].Col;
                int dstRow = _lookup[label].Row;
                int dstCol = _lookup[label].Col;

                bool same = (dstCol == srcCol) && (dstRow == srcRow);

                if (!same)
                {
                    LightItUp(srcRow, srcCol, DirBetween(_lookup[src], _lookup[label]));
                }
            }
        }
        #endregion

        #region Private Helpers
        private void RenderPuzzle()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    Label l = _labels[col, row];

                    int index = (l.Text.Equals("") || _errors[row, col]) ? 0 : int.Parse(l.Text);

                    l.BackColor = _errors[row, col] ? ErrCell : DefaultBack;
                    l.ForeColor = DisplayColors[index % DisplayColors.Length];

                    if (!l.Text.Equals(""))
                        l.Font = (l.Text.Length > 1) ? _smallFont : _normalFont;
                    else if (_dirs.ContainsKey(_lookup[l]))
                        l.Image = _icons[-1 - (int)_dirs[_lookup[l]]];
                }
            }
        }

        private void EnableDrawing(bool enable)
        {
            SendMessage(this.Handle, WM_SETREDRAW, enable, 0);
            if (enable)
                Refresh();
        }

        private bool IsLaser(int row, int col)
        {
            return _labels[col, row].Image != null;
        }

        private bool IsNegative(int row, int col)
        {
            return (_labels[col, row].BackColor == ErrLightUp) ||
                (_labels[col, row].BackColor == ErrCell);
        }

        private Dir DirBetween(Coord srcCoord, Coord dstCoord)
        {
            int max = Math.Abs(dstCoord.Row - srcCoord.Row);
            Dir dir = (dstCoord.Row > srcCoord.Row) ? Dir.Dn : Dir.Up;
            if (max < Math.Abs(dstCoord.Col - srcCoord.Col))
            {
                max = Math.Abs(dstCoord.Col - srcCoord.Col);
                dir = (dstCoord.Col > srcCoord.Col) ? Dir.Rt : Dir.Lt;
            }
            if (max == 0)
                return Dir.NA;

            return dir;
        }

        private void LightItUp(int srcRow, int srcCol, Dir dir)
        {
            if (dir == Dir.NA)
                return;

            bool useCol = (dir == Dir.Up) || (dir == Dir.Dn);
            int rowStep = useCol ? ((dir == Dir.Up) ? -1 : 1) : 0;
            int colStep = useCol ? 0 : ((dir == Dir.Lt) ? -1 : 1);

            for (int col = srcCol + colStep, row = srcRow + rowStep;
                col >= 0 && col < Columns && row >= 0 && row < Rows;
                col += colStep, row += rowStep)
            {
                if (!IsLaser(row, col) && IsNegative(row, col))
                    _labels[col, row].BackColor = ErrLightUp;
                else
                    _labels[col, row].BackColor = LightUp;
            }
        }

        private void EmitLaserDragged(Coord srcCoord, Dir dir)
        {
            if (LaserDragged != null)
                LaserDragged(srcCoord, dir);
        }

        private void GetFittedFonts()
        {
            const float AbsMax = 72;
            const float AbsMin = 3;

            float minSize1 = AbsMin;
            float maxSize1 = AbsMax;
            float minSize2 = AbsMin;
            float maxSize2 = AbsMax;

            Font f1 = new Font(FontName, minSize1, FontProperties);
            Font f2 = new Font(FontName, minSize2, FontProperties);
            Graphics g = CreateGraphics();
            SizeF extent1 = g.MeasureString("8", f1);
            SizeF extent2 = g.MeasureString("88", f2);

            Size s = _labels[0, 0].Size;

            float newSize1 = minSize1;
            float newSize2 = minSize2;
            {
                float hRatio = s.Height / extent1.Height;
                float wRatio = s.Width / extent1.Width;
                float ratio = (hRatio < wRatio) ? hRatio : wRatio;

                newSize1 = f1.Size * ratio;

                if (newSize1 < minSize1)
                    newSize1 = minSize1;
                if (newSize1 > maxSize1)
                    newSize1 = maxSize1;
            }
            {
                float hRatio = s.Height / extent2.Height;
                float wRatio = s.Width / extent2.Width;
                float ratio = (hRatio < wRatio) ? hRatio : wRatio;

                newSize2 = f2.Size * ratio;

                if (newSize2 < minSize2)
                    newSize2 = minSize2;
                if (newSize2 > maxSize2)
                    newSize2 = maxSize2;
            }

            _normalFont = new Font(FontName, newSize1 - 2, FontProperties);
            _smallFont = new Font(FontName, newSize2 - 1, FontProperties);
            extent1 = g.MeasureString("8", _normalFont);
            extent2 = g.MeasureString("88", _smallFont);

            Debug.Assert((int)extent1.Width <= s.Width && (int)extent1.Height <= s.Height);
            Debug.Assert((int)extent2.Width <= s.Width && (int)extent2.Height <= s.Height);
        }

        private void ScaleIcons()
        {
            Size s = _labels[0, 0].Size;
            Size origSize = BigLaserIcons[0].Size;

            float hRatio = s.Height / (float)origSize.Height;
            float wRatio = s.Width / (float)origSize.Width;
            float ratio = 0.7f * ((hRatio < wRatio) ? hRatio : wRatio);

            Size newSize = new Size((int)(origSize.Width * ratio), (int)(origSize.Height * ratio));

            for (int i = 0; i < BigLaserIcons.Length; i++)
            {
                _icons[i] = new Bitmap(BigLaserIcons[i], newSize);
            }
        }
        #endregion

    }
}
