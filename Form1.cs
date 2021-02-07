using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using SwiPlcs.
using SbsSW.SwiPlCs;
using SbsSW.SwiPlCs.Callback;

namespace fdqueens_cs
{
    public partial class Form1 : Form
    {
        static string __FILE__([System.Runtime.CompilerServices.CallerFilePath] string fileName = "")
        {
            return fileName;
        }

        public Form1()
        {
Console.WriteLine("Form1");
            InitializeComponent();

            Delegate queen_paint_Delegate = new DelegateParameter3(queen_paint);
            PlEngine.RegisterForeign(queen_paint_Delegate);

            //String[] argv = { @"C:\Users\Carlo\source\repos\fdqueens_cs\fdqueens_cs\fdqueens.pl" };
            string project_dir = System.IO.Path.GetDirectoryName(__FILE__());
            string pl_source = project_dir + @"\fdqueens.pl";
            String[] argv = { pl_source };

            PlEngine.Initialize(argv);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
Console.WriteLine("Form1_Load");
            draw_chessboard();
        }

        // foreign predicate, with some configuration
        private int n_queens = 6;
        private static int count_paints_calls = 0;
        private static int msec_sleep = 50;
        private static bool echo_paints_calls = false;

        public bool queen_paint(PlTerm Q, PlTerm N, PlTerm V)
        {
            System.Diagnostics.Debug.Assert(Q.IsInteger);
            System.Diagnostics.Debug.Assert(N.IsInteger);
            System.Diagnostics.Debug.Assert(V.IsAtom);
            int q = (int)Q; 
            int n = (int)N;
            string v = (string)V;
            if (echo_paints_calls)
                Console.WriteLine("queen_paint c:{0} Q:{1} N:{2} V:{3}", count_paints_calls++, q, n, v);
            char c = '?';
            switch (v)
            {
                case "place":
                    c = 'Q';
                    break;
                case "clear":
                    c = ' ';
                    break;
                case "gray":
                    c = '-';
                    break;
            }
            _chessBoard[q - 1, n - 1].Text = c.ToString();
            if (msec_sleep > 0)
            {
                System.Threading.Thread.Sleep(msec_sleep);
                Application.DoEvents();
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PlQuery.PlCall(String.Format("fdqueens(_,{0})", n_queens));
        }

        // from https://stackoverflow.com/a/6733718/874024
        // I changed Panel to Label...
        // class member array of cells to track chessboard content
        private Label[ , ] _chessBoard;

        private void draw_chessboard()
        {
            const int tileSize = 40;
            var clr1 = Color.DarkGray;
            var clr2 = Color.White;

            // initialize the "chess board"
            _chessBoard = new Label[n_queens, n_queens];

            // double for loop to handle all rows and columns
            for (var n = 0; n < n_queens; n++)
            {
                for (var m = 0; m < n_queens; m++)
                {
                    // create new Label control which will be one 
                    // chess board tile
                    var newLabel = new Label
                    {
                        Size = new Size(tileSize, tileSize),
                        Location = new Point(tileSize * n, tileSize * m)
                    };

                    // add to Form's Controls so that they show up
                    Controls.Add(newLabel);

                    // add to our 2d array of panels for future use
                    _chessBoard[n, m] = newLabel;

                    // color the backgrounds
                    if (n % 2 == 0)
                        newLabel.BackColor = m % 2 != 0 ? clr1 : clr2;
                    else
                        newLabel.BackColor = m % 2 != 0 ? clr2 : clr1;
                }
            }
        }
    }
}
