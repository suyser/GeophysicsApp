using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GeophysicsApp
{
    /// <summary>
    /// Логика взаимодействия для ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        public DataBase dataBase = new DataBase();
        public Profile currentProfile;
        public List<Measurement> measurements = new List<Measurement>();
        public ProfileWindow(Profile profile)
        {
            InitializeComponent();
            dataBase.GetConnection();
            currentProfile = profile;
            CreatePicketList();
            CreateMeasurementList();
            DrawDiagram();
            FillDataGrids();
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }

        public void CreatePicketList()
        {
            DataTable dataTable = dataBase.SqlSelect("select * from Pickets where IdProfile = " + currentProfile.IdProfile);
            if(dataTable.Rows.Count == 0)
            {
                currentProfile.Pickets = null;
                return;
            }
            for(int i = 0; i < dataTable.Rows.Count; i++)
            {
                currentProfile.Pickets.Add(new Picket(Convert.ToInt32(dataTable.Rows[i][0]), Convert.ToInt32(dataTable.Rows[i][1]), 
                    new Tuple<int, int>(Convert.ToInt32(dataTable.Rows[i][2]), Convert.ToInt32(dataTable.Rows[i][3]))));
            }
        }

        public void CreateMeasurementList()
        {
            DataTable dataTable = dataBase.SqlSelect("select Measurements.* from Measurements, Pickets " +
                    "where IdPicket1 = IdPicket and IdProfile = " + currentProfile.IdProfile);
            if (dataTable.Rows.Count == 0) return;
            for(int i = 0; i < dataTable.Rows.Count; i++)
            {
                measurements.Add(new Measurement(Convert.ToInt32(dataTable.Rows[i][0]), Convert.ToInt32(dataTable.Rows[i][1]),
                    Convert.ToInt32(dataTable.Rows[i][2]), Convert.ToInt32(dataTable.Rows[i][3])));
            }
        }

        public void DrawDiagram()
        {
            picketsDiagram.Children.Clear();

            // Настройки
            int marginLeft = 60;
            int marginTop = 40;
            int marginBottom = 60;
            int marginRight = 150;

            int canvasWidth = (int)picketsDiagram.ActualWidth;
            int canvasHeight = (int)picketsDiagram.ActualHeight;

            if (canvasWidth == 0 || canvasHeight == 0)
            {
                // Если Canvas ещё не отрисован, установим дефолтные размеры
                canvasWidth = 800;
                canvasHeight = 400;
            }

            // Диапазон пикетов
            int piketCount = currentProfile.Pickets.Count;

            // Максимальная глубина измерения
            int maxDepth = measurements.Max(m => m.Depth);

            // Размеры графика (без отступов)
            int graphWidth = canvasWidth - marginLeft - marginRight;
            int graphHeight = canvasHeight - marginTop - marginBottom;

            // Расчёт шага по X и Y
            double stepX = (double)graphWidth / (piketCount - 1);
            double stepY = (double)graphHeight / maxDepth;

            // Рисуем оси
            DrawLine(marginLeft, marginTop, marginLeft, marginTop + graphHeight, Brushes.Black, 2); // Y
            DrawLine(marginLeft, marginTop + graphHeight, marginLeft + graphWidth, marginTop + graphHeight, Brushes.Black, 2); // X

            // Подписи осей
            AddText("Пикеты", marginLeft + graphWidth / 2, marginTop + graphHeight + 40, 16, FontWeights.Bold, HorizontalAlignment.Center);
            AddText("Глубина", marginLeft - 40, marginTop + graphHeight / 2, 16, FontWeights.Bold, HorizontalAlignment.Center, true);

            // Рисуем сетку и подписи по X (Пикеты)
            for (int i = 0; i < piketCount; i++)
            {
                double x = marginLeft + i * stepX;

                DrawLine(x, marginTop + graphHeight, x, marginTop + graphHeight + 5, Brushes.Black, 1); // маленькая отметка
                AddText($"П{i + 1}", x, marginTop + graphHeight + 10, 12, FontWeights.Normal, HorizontalAlignment.Center);
                DrawLine(x, marginTop, x, marginTop + graphHeight, Brushes.LightGray, 0.5); // сетка
            }

            // Рисуем сетку и подписи по Y (Глубина)
            for (int d = 0; d <= maxDepth; d++)
            {
                double y = marginTop + graphHeight - d * stepY;
                DrawLine(marginLeft - 5, y, marginLeft, y, Brushes.Black, 1); // маленькая отметка
                AddText(d.ToString(), marginLeft - 10, y, 12, FontWeights.Normal, HorizontalAlignment.Right);
                DrawLine(marginLeft, y, marginLeft + graphWidth, y, Brushes.LightGray, 0.5); // сетка
            }

            // Цвета по Difference и легенда
            var legendItems = new (string Label, Brush Color)[]
            {
        ("<= 210", Brushes.DarkBlue),
        ("211 - 600", Brushes.LightBlue),
        ("601 - 1200", Brushes.LightGreen),
        ("1201 - 2400", Brushes.Green),
        ("2401 - 9600", Brushes.Orange),
        ("> 9600", Brushes.Red)
            };

            DrawLegend(legendItems, canvasWidth - marginRight + 10, marginTop);

            // Отрисовка измерений как прямоугольников
            double rectWidth = stepX * 0.6;

            foreach (var m in measurements)
            {
                Brush fillColor = GetDifferenceColor(m.Difference);

                double x = marginLeft + (m.IdPicket1 - 1) * stepX - rectWidth / 2;
                double y = marginTop + graphHeight - m.Depth * stepY;

                Rectangle rect = new Rectangle()
                {
                    Width = rectWidth,
                    Height = stepY * 0.8,
                    Fill = fillColor,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.8,
                    ToolTip = $"Пикет: {m.IdPicket1}\nГлубина: {m.Depth}\nРазница: {m.Difference}"
                };

                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y - stepY * 0.8 / 2);

                picketsDiagram.Children.Add(rect);
            }
        }

        private void DrawLine(double x1, double y1, double x2, double y2, Brush color, double thickness)
        {
            var line = new Line()
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = color,
                StrokeThickness = thickness
            };
            picketsDiagram.Children.Add(line);
        }

        private void AddText(string text, double x, double y, double fontSize, FontWeight fontWeight, HorizontalAlignment alignment, bool rotate = false)
        {
            var tb = new TextBlock()
            {
                Text = text,
                FontSize = fontSize,
                FontWeight = fontWeight,
                Foreground = Brushes.Black
            };

            if (rotate)
            {
                tb.LayoutTransform = new RotateTransform(-90);
                // Сдвиг при повороте, чтобы текст центрировался
                tb.RenderTransformOrigin = new Point(0.5, 0.5);
            }

            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            double textWidth = tb.DesiredSize.Width;

            switch (alignment)
            {
                case HorizontalAlignment.Center:
                    Canvas.SetLeft(tb, x - textWidth / 2);
                    break;
                case HorizontalAlignment.Right:
                    Canvas.SetLeft(tb, x - textWidth);
                    break;
                default:
                    Canvas.SetLeft(tb, x);
                    break;
            }

            Canvas.SetTop(tb, y);
            picketsDiagram.Children.Add(tb);
        }

        private void DrawLegend((string Label, Brush Color)[] items, double startX, double startY)
        {
            double boxSize = 15;
            double spacing = 5;
            double textOffset = boxSize + spacing;

            for (int i = 0; i < items.Length; i++)
            {
                Rectangle rect = new Rectangle()
                {
                    Width = boxSize,
                    Height = boxSize,
                    Fill = items[i].Color,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.8
                };
                Canvas.SetLeft(rect, startX);
                Canvas.SetTop(rect, startY + i * (boxSize + spacing));
                picketsDiagram.Children.Add(rect);

                AddText(items[i].Label, startX + textOffset, startY + i * (boxSize + spacing), 12, FontWeights.Normal, HorizontalAlignment.Left);
            }
        }

        private Brush GetDifferenceColor(double difference)
        {
            if (difference <= 210) return Brushes.DarkBlue;
            else if (difference <= 600) return Brushes.LightBlue;
            else if (difference <= 1200) return Brushes.LightGreen;
            else if (difference <= 2400) return Brushes.Green;
            else if (difference <= 9600) return Brushes.Orange;
            else return Brushes.Red;
        }


        public void FillDataGrids()
        {
            DataTable dataTable = dataBase.SqlSelect("select * from Measurements");
            gridMeasurements.ItemsSource = dataTable.DefaultView;

            dataTable = dataBase.SqlSelect("select * from Pickets where IdProfile = " + currentProfile.IdProfile);
            gridPickets.ItemsSource = dataTable.DefaultView;
        }
    }
}
