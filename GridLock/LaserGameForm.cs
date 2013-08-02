using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using LaserPuzzle;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;

namespace LaserGame
{
    public partial class LaserGameForm : Form
    {
        #region Constants
        private static readonly Dictionary<string, int> Translation = new Dictionary<string, int>();
        static LaserGameForm()
        {
            Translation["."] = (int)Dir.NA;
            Translation["^"] = (int)Dir.Up;
            Translation[">"] = (int)Dir.Rt;
            Translation["v"] = (int)Dir.Dn;
            Translation["<"] = (int)Dir.Lt;
        }
        #endregion

        #region Private Members
        private int[,] _puzzle;
        private bool[,] _puzLasers;
        private bool _disallowOutward = false;
        private bool _disallowParallel = false;
        private bool _disallowZeros = false;
        private bool _attemptUnique = false;
        private Puzzle _origPuz = null;
        private GameGrid _grid;
        private object _locker = new string('x', 13);
        #endregion

        public LaserGameForm()
        {
            InitializeComponent();

            _grid = new GameGrid();
            _grid.LaserDragged += new GameGrid.LaserDragHandler(HandleDrag);
            _grid.Dock = DockStyle.Fill;
            //_grid.AutoSize = true;
            //_grid.AutoSizeMode = AutoSizeMode.GrowOnly;
            tableLayoutPanel1.Controls.Add(_grid, 0, 0);

            Initialize(9, 9, 30, Puzzle.Constraints.None);
        }

        #region Properties
        int Rows
        {
            get { return _puzzle.GetLength(0); }
        }

        int Columns
        {
            get { return _puzzle.GetLength(1); }
        }

        int LaserCount
        {
            get
            {
                int count = 0;
                for (int row = 0; row < Rows; row++)
                    for (int col = 0; col < Columns; col++)
                        if (_puzLasers[row, col])
                            count++;
                return count;
            }
        }

        private int RemainingLaserCount
        {
            get
            {
                int count = LaserCount;
                for (int row = 0; row < Rows; row++)
                    for (int col = 0; col < Columns; col++)
                    {
                        if (!_puzLasers[row, col])
                            continue;
                        if (_puzzle[row, col] != (int)Dir.NA)
                            count--;
                    }
                return count;
            }
        }

        private bool AllZero
        {
            get
            {
                for (int row = 0; row < Rows; row++)
                    for (int col = 0; col < Columns; col++)
                    {
                        if (!_puzLasers[row, col] && _puzzle[row, col] != 0)
                            return false;
                    }
                return true;
            }
        }

        private Puzzle.Constraints Constraints
        {
            get
            {
                Puzzle.Constraints c = Puzzle.Constraints.None;
                if (_disallowOutward)
                    c |= Puzzle.Constraints.DisallowOutwardBoundaryLasers;
                if (_disallowParallel)
                    c |= Puzzle.Constraints.DisallowParallelLaserBlocks;
                if (_disallowZeros)
                    c |= Puzzle.Constraints.DisallowStartingZeros;
                if (_attemptUnique)
                    c |= Puzzle.Constraints.AttemptUniqueSolution;
                return c;
            }
        }
        #endregion

        #region Private Methods
        private class GenerateArgs
        {
            public int rows;
            public int cols;
            public int lasers;
            public Puzzle.Constraints constraints;
            public LaserGameForm form;
        }

        private static void GenerateThread(object o)
        {
            GenerateArgs args = o as GenerateArgs;
            if (args == null)
                return;

            args.form.CreatePuzzle(args);
        }

        private void Initialize(int rows, int cols, int lasers, Puzzle.Constraints constraints)
        {
            _disallowZeros = Puzzle.HasConstraint(constraints, Puzzle.Constraints.DisallowStartingZeros);
            _disallowParallel = Puzzle.HasConstraint(constraints, Puzzle.Constraints.DisallowParallelLaserBlocks);
            _disallowOutward = Puzzle.HasConstraint(constraints, Puzzle.Constraints.DisallowOutwardBoundaryLasers);
            _attemptUnique = Puzzle.HasConstraint(constraints, Puzzle.Constraints.AttemptUniqueSolution);

            GenerateArgs args = new GenerateArgs();
            args.rows = rows;
            args.cols = cols;
            args.lasers = lasers;
            args.constraints = constraints;
            args.form = this;

            Puzzle last = _origPuz;
            bool success = false;
            if (constraints != Puzzle.Constraints.None)
            {
                Thread t = new Thread(GenerateThread);
                t.Start(args);
                while (!t.IsAlive) ;
                WaitDlg dlg = new WaitDlg(t);
                if (dlg.ShowDialog() == DialogResult.Abort)
                {
                    lock (_locker)
                    {
                        t.Abort();
                    }
                }
                else
                {
                    t.Join();
                }
            }
            else
                CreatePuzzle(args);

            if (_origPuz != last)
                SetupPuzzle();
        }

        private void CreatePuzzle(GenerateArgs args)
        {
            Puzzle puz;
            try
            {
                int rows = args.rows;
                int cols = args.cols;
                int lasers = args.lasers;
                Puzzle.Constraints constraints = args.constraints;

                puz = new Puzzle(rows, cols, lasers, constraints);

                lock (_locker)
                {
                    _origPuz = puz;
                    PopulateFromOriginal();
                }
            }
            catch (ThreadAbortException ex)
            {
                return;
            }
        }

        private void PopulateFromOriginal()
        {
            Dictionary<Direction, int> values = new Dictionary<Direction, int>();
            values[Direction.None] = -1;
            _puzzle = _origPuz.RenderPuzzleGrid(values, true);
            _puzLasers = new bool[Rows, Columns];
            for (int row = 0; row < Rows; row++)
                for (int col = 0; col < Columns; col++)
                {
                    if (_puzzle[row, col] < 0)
                        _puzLasers[row, col] = true;
                    else
                        _puzLasers[row, col] = false;
                }
        }

        private void TranslatePuzzle(string puzzle)
        {
            string[] rows = puzzle.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            string[] cols = rows[0].Split(' ');
            _puzzle = new int[rows.Length, cols.Length];
            _puzLasers = new bool[rows.Length, cols.Length];
            for (int row = 0; row < rows.Length; row++)
            {
                cols = rows[row].Split(' ');
                for (int col = 0; col < cols.Length; col++)
                {
                    _puzLasers[row, col] = false;
                    if (!int.TryParse(cols[col], out _puzzle[row, col]))
                    {
                        _puzzle[row, col] = Translation[cols[col]];
                        _puzLasers[row, col] = true;
                    }
                }
            }
        }

        private void SetupPuzzle()
        {
            _grid.SetDimensions(Columns, Rows);
            _grid.UpdateDisplay(_puzzle, _puzLasers);
        }

        private void AdjustSegment(Coord srcCoord, Dir dir, int adjustment)
        {
            if (dir == Dir.NA)
                return;

            int srcRow = srcCoord.Row;
            int srcCol = srcCoord.Col;

            bool useCol = (dir == Dir.Up) || (dir == Dir.Dn);
            int rowStep = useCol ? ((dir == Dir.Up) ? -1 : 1) : 0;
            int colStep = useCol ? 0 : ((dir == Dir.Lt) ? -1 : 1);

            for (int col = srcCol + colStep, row = srcRow + rowStep;
                col >= 0 && col < Columns && row >= 0 && row < Rows;
                col += colStep, row += rowStep)
            {
                if (!_puzLasers[row, col])
                    _puzzle[row, col] += adjustment;
            }
        }

        private void HandleDrag(Coord srcCoord, Dir dir)
        {
            Dir oldDir = (Dir)_puzzle[srcCoord.Row, srcCoord.Col];
            AdjustSegment(srcCoord, oldDir, 1);
            AdjustSegment(srcCoord, dir, -1);

            _puzzle[srcCoord.Row, srcCoord.Col] = (int)dir;

            _grid.UpdateDisplay(_puzzle, _puzLasers);

            if (RemainingLaserCount == 0 && AllZero)
                Celebrate();
        }

        private void Celebrate()
        {
            MessageBox.Show("A winner is you!", "CONGRATULATIONS");
        }

        private Puzzle ReverseToPuzzle()
        {
            Direction[,] dirs = new Direction[Rows, Columns];
            var locs = new List<KeyValuePair<int, int>>();
            for (int row = 0; row < Rows; row++)
                for (int col = 0; col < Columns; col++)
                {
                    dirs[row, col] = Direction.None;
                    if (_puzLasers[row, col])
                    {
                        dirs[row, col] =
                            (_puzzle[row, col] == (int)Dir.Up) ? Direction.North :
                            (_puzzle[row, col] == (int)Dir.Rt) ? Direction.East :
                            (_puzzle[row, col] == (int)Dir.Dn) ? Direction.South :
                            (_puzzle[row, col] == (int)Dir.Lt) ? Direction.West : Direction.None;
                        locs.Add(new KeyValuePair<int, int>(row, col));
                    }
                }

            return new Puzzle(_puzzle, dirs, locs);
        }
        #endregion

        #region Menu Actions
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutDlg dlg = new AboutDlg();
            dlg.ShowDialog();
        }

        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SettingsDlg dlg = new SettingsDlg(Rows, Columns, LaserCount, _disallowParallel, _disallowOutward, _disallowZeros, _attemptUnique);
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                Puzzle.Constraints constraints = Puzzle.Constraints.None;
                if (dlg.DisallowOutwardLasers)
                    constraints |= Puzzle.Constraints.DisallowOutwardBoundaryLasers;
                if (dlg.DisallowParallelBlocks)
                    constraints |= Puzzle.Constraints.DisallowParallelLaserBlocks;
                if (dlg.DisallowStartingZeros)
                    constraints |= Puzzle.Constraints.DisallowStartingZeros;
                if (dlg.AttemptUniqueSolution)
                    constraints |= Puzzle.Constraints.AttemptUniqueSolution;

                Initialize(dlg.Rows, dlg.Columns, dlg.Lasers, constraints);
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Initialize(Rows, Columns, LaserCount, Constraints);
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PopulateFromOriginal();
            _grid.UpdateDisplay(_puzzle, _puzLasers);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void saveCurrentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = "txt";
            dlg.FileName = "myGridLock";
            dlg.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            dlg.InitialDirectory = Directory.GetCurrentDirectory();
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                Puzzle curPuz = ReverseToPuzzle();

                FileStream stream = new FileStream(dlg.FileName, FileMode.Create);
                StreamWriter writer = new StreamWriter(stream);
                _origPuz.WritePuzzle(writer, false);
                writer.WriteLine("");
                curPuz.WritePuzzle(writer, true);
                stream.Close();
            }
        }

        private void loadPuzzleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = "txt";
            dlg.FileName = "myGridLock";
            dlg.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            dlg.InitialDirectory = Directory.GetCurrentDirectory();
            DialogResult result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                StreamReader reader = new StreamReader(dlg.FileName);
                string content = reader.ReadToEnd();
                reader.Close();
                string[] sections = content.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
                TranslatePuzzle(sections[0]);
                _origPuz = ReverseToPuzzle();
                TranslatePuzzle(sections[1]);
                SetupPuzzle();
            }
        }

        private void LaserGameForm_Load(object sender, EventArgs e)
        {

        }
        #endregion

    }
}
