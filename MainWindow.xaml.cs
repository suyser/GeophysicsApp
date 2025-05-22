using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GeophysicsApp
{
    public partial class MainWindow : Window
    {
        public List<Sector> sectors = new List<Sector>();
        DataBase dataBase = new DataBase();

        public MainWindow()
        {
            InitializeComponent();
            dataBase.GetConnection();
            CreateSectorsList();
            DrawSectors();
            FillDataGrid();
        }

        public void CreateSectorsList()
        {
            sectors.Clear();
            DataTable dataTable = dataBase.SqlSelect("select * from Sectors");
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                var sector = new Sector(Convert.ToInt32(dataTable.Rows[i][0]), Convert.ToInt32(dataTable.Rows[i][2]));
                DataTable dt = dataBase.SqlSelect("select * from SectorCoordinates where IdSector = " + sector.IdSector);
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    var coord = new Tuple<int, int>(Convert.ToInt32(dt.Rows[j][2]), Convert.ToInt32(dt.Rows[j][3]));
                    sector.Coordinates.Add(coord);
                }
                sectors.Add(sector);
            }
        }

        // Метод для создания скругленного многоугольника (Path с дугами)
        private Path CreateRoundedPolygon(List<Point> points, double radius, Brush fill, Brush stroke, double thickness)
        {
            if (points.Count < 3) return null;

            PathFigure figure = new PathFigure();
            figure.IsClosed = true;
            figure.IsFilled = true;

            for (int i = 0; i < points.Count; i++)
            {
                Point p0 = points[(i - 1 + points.Count) % points.Count];
                Point p1 = points[i];
                Point p2 = points[(i + 1) % points.Count];

                Vector v1 = (p0 - p1);
                v1.Normalize();
                Vector v2 = (p2 - p1);
                v2.Normalize();

                // Точки начала и конца "скругления"
                Point start = p1 + v1 * radius;
                Point end = p1 + v2 * radius;

                if (i == 0)
                {
                    figure.StartPoint = start;
                }
                else
                {
                    figure.Segments.Add(new LineSegment(start, true));
                }

                // Добавляем дугу между start и end с заданным радиусом
                bool isLargeArc = false;
                SweepDirection sweepDirection = SweepDirection.Clockwise;

                ArcSegment arc = new ArcSegment(end, new Size(radius, radius), 0, isLargeArc, sweepDirection, true);
                figure.Segments.Add(arc);
            }

            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            Path path = new Path
            {
                Data = geometry,
                Fill = fill,
                Stroke = stroke,
                StrokeThickness = thickness,
                Cursor = Cursors.Hand
            };

            return path;
        }

        public void DrawSectors()
        {
            sectorsDiagram.Children.Clear();

            Random rnd = new Random();

            foreach (var s in sectors)
            {
                List<Point> pts = new List<Point>();
                foreach (var coord in s.Coordinates)
                {
                    pts.Add(new Point(coord.Item1 * 1.2, coord.Item2 * 1.2));
                }

                var baseColor = Color.FromRgb(
                    (byte)rnd.Next(100, 256),
                    (byte)rnd.Next(100, 256),
                    (byte)rnd.Next(100, 256));

                var fillBrush = new RadialGradientBrush(
                    Color.FromArgb(180, baseColor.R, baseColor.G, baseColor.B),
                    Color.FromArgb(50, baseColor.R, baseColor.G, baseColor.B))
                {
                    RadiusX = 0.8,
                    RadiusY = 0.8,
                    Center = new Point(0.5, 0.5),
                    GradientOrigin = new Point(0.5, 0.5)
                };

                var strokeBrush = Brushes.DimGray;
                double strokeThickness = 2;

                Path path = CreateRoundedPolygon(pts, 10, fillBrush, strokeBrush, strokeThickness);
                if (path == null) continue;

                path.Tag = s.IdSector;

                path.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Gray,
                    Direction = 320,
                    ShadowDepth = 5,
                    Opacity = 0.4,
                    BlurRadius = 8
                };

                path.MouseEnter += (sender, e) =>
                {
                    var p = sender as Path;
                    p.StrokeThickness = 4;
                    p.Stroke = new SolidColorBrush(Color.FromRgb(
                        (byte)Math.Min(baseColor.R + 50, 255),
                        (byte)Math.Min(baseColor.G + 50, 255),
                        (byte)Math.Min(baseColor.B + 50, 255)));

                    var brush = new RadialGradientBrush(
                        Color.FromArgb(220, baseColor.R, baseColor.G, baseColor.B),
                        Color.FromArgb(80, baseColor.R, baseColor.G, baseColor.B))
                    {
                        RadiusX = 0.85,
                        RadiusY = 0.85,
                        Center = new Point(0.5, 0.5),
                        GradientOrigin = new Point(0.5, 0.5)
                    };
                    p.Fill = brush;
                };

                path.MouseLeave += (sender, e) =>
                {
                    var p = sender as Path;
                    p.StrokeThickness = 2;
                    p.Stroke = Brushes.DimGray;
                    p.Fill = fillBrush;
                };

                path.MouseLeftButtonUp += (sender, e) =>
                {
                    var p = sender as Path;
                    int sectorId = (int)p.Tag;
                    OpenSectorWindow(sectorId);
                };

                sectorsDiagram.Children.Add(path);
            }
        }

        private void OpenSectorWindow(int sectorId)
        {
            Sector selectedSector = sectors.Find(s => s.IdSector == sectorId);
            if (selectedSector != null)
            {
                SectorWindow window = new SectorWindow(selectedSector);
                window.Owner = this;
                window.Show();
                this.Hide();
            }
        }

        public void FillDataGrid()
        {
            DataTable dataTable = dataBase.SqlSelect("select * from Sectors, SectorCoordinates where Sectors.IdSector = SectorCoordinates.IdSector");
            gridSectors.ItemsSource = dataTable.DefaultView;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (control.SelectedIndex == 0)
            {
                sectors = new List<Sector>();
                sectorsDiagram.Children.Clear();
                CreateSectorsList();
                DrawSectors();
            }
        }
    }
}
