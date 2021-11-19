using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameOfLife
{
    class Grid
    {
        int SizeX;
        int SizeY;
        Cell[,] cells;
        Canvas drawCanvas;
        Ellipse[,] cellsVisuals;

        public Grid(Canvas c)
        {
            drawCanvas = c;
            SizeX = (int)(c.Width / 5);
            SizeY = (int)(c.Height / 5);
            cells = new Cell[SizeX, SizeY];
            cellsVisuals = new Ellipse[SizeX, SizeY];

            Initialize();
        }

        void Initialize()
		{
            var rnd = new Random();

            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++)
                {
                    cells[i, j] = new Cell(i, j, 0, rnd.NextDouble() > 0.8);
                    InitCellsVisuals(i, j);
                    UpdateGraphics(i, j);
                }
        }

        void InitCellsVisuals(int i, int j)
        {
            var ellipse = new Ellipse();
            ellipse.Width = ellipse.Height = 5;
            ellipse.Margin = new Thickness(cells[i, j].PositionX, cells[i, j].PositionY, 0, 0);
            ellipse.Fill = Brushes.Gray;
            drawCanvas.Children.Add(ellipse);

            ellipse.MouseMove += MouseMove;
            ellipse.MouseLeftButtonDown += MouseMove;

            cellsVisuals[i, j] = ellipse;
        }

        public void Clear()
        {
			for (int i = 0; i < SizeX; i++)
				for (int j = 0; j < SizeY; j++)
				{
					cells[i, j] = new Cell(i, j, 0, false);
					cellsVisuals[i, j].Fill = Brushes.Gray;
				}
		}

        void MouseMove(object sender, MouseEventArgs e)
        {
            var cellVisual = sender as Ellipse;

            int i = (int)cellVisual.Margin.Left / 5;
            int j = (int)cellVisual.Margin.Top / 5;
            var cell = cells[i, j];

            if (e.LeftButton == MouseButtonState.Pressed && !cell.IsAlive)
			{
                cell.IsAlive = true;
                cell.Age = 0;
				cellVisual.Fill = Brushes.White;
			}
        }

        void UpdateGraphics(int i, int j)
        {
			cellsVisuals[i, j].Fill = cells[i, j].IsAlive
										  ? (cells[i, j].Age < 2 ? Brushes.White : Brushes.DarkGray)
										  : Brushes.Gray;
		}

        public void Update()
        {
            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++)
                {
                    CalculateNextGeneration(i, j);
                    UpdateGraphics(i, j);
                }
        }

        void CalculateNextGeneration(int row, int column)
        {
            var cell = cells[row, column];
            var isAlive = cell.IsAlive;
            var age = cell.Age;
            var count = CountNeighbors(row, column);

            if (cell.IsAlive && count < 2)
            {
                isAlive = false;
                age = 0;
            }

            if (isAlive && (count == 2 || count == 3))
            {
                cell.Age++;
                isAlive = true;
                age = cell.Age;
            }

            if (isAlive && count > 3)
            {
                isAlive = false;
                age = 0;
            }

            if (!isAlive && count == 3)
            {
                isAlive = true;
                age = 0;
            }

            cell.IsAlive = isAlive;
            cell.Age = age;
        }

        int CountNeighbors(int i, int j)
        {
            int count = 0;

            if (i != SizeX - 1 && cells[i + 1, j].IsAlive) count++;
            if (i != SizeX - 1 && j != SizeY - 1 && cells[i + 1, j + 1].IsAlive) count++;
            if (j != SizeY - 1 && cells[i, j + 1].IsAlive) count++;
            if (i != 0 && j != SizeY - 1 && cells[i - 1, j + 1].IsAlive) count++;
            if (i != 0 && cells[i - 1, j].IsAlive) count++;
            if (i != 0 && j != 0 && cells[i - 1, j - 1].IsAlive) count++;
            if (j != 0 && cells[i, j - 1].IsAlive) count++;
            if (i != SizeX - 1 && j != 0 && cells[i + 1, j - 1].IsAlive) count++;

            return count;
        }
    }
}