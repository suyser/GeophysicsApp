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
    /// Логика взаимодействия для SectorWindow.xaml
    /// </summary>
    public partial class SectorWindow : Window
    {
        public DataBase dataBase = new DataBase();
        public Sector currentSector;
        public int minX, minY;
        public SectorWindow(Sector sector)
        {
            InitializeComponent();
            dataBase.GetConnection();
            currentSector = sector;
            CreateProfilesList();
            DrawSector();
            DrawProfiles();
            CreateButtons();
            FillSectorText();
            FillDataGrid();
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            Window window = this.Owner;
            window.Show();
            this.Hide();
        }

        public void DrawSector()
        {
            sectorView.Children.Clear();

            if (currentSector == null || currentSector.Coordinates.Count < 3)
                return;

            Random rnd = new Random();

            Polygon polygon = new Polygon
            {
                Stroke = Brushes.DimGray,
                StrokeThickness = 2,
                Points = new PointCollection(),
                Cursor = Cursors.Hand,
                Tag = currentSector.IdSector,
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Gray,
                    Direction = 320,
                    ShadowDepth = 5,
                    Opacity = 0.4,
                    BlurRadius = 8
                }
            };

            int minX = currentSector.Coordinates[0].Item1;
            int minY = currentSector.Coordinates[0].Item2;

            // Найдем минимальные координаты для смещения
            for (int i = 1; i < currentSector.Coordinates.Count; i++)
            {
                if (currentSector.Coordinates[i].Item1 < minX)
                    minX = currentSector.Coordinates[i].Item1;
                if (currentSector.Coordinates[i].Item2 < minY)
                    minY = currentSector.Coordinates[i].Item2;
            }

            // Добавляем точки в полигона с учётом смещения и масштабирования
            foreach (var coord in currentSector.Coordinates)
            {
                double x = (coord.Item1 - minX + 6) * 1.7;
                double y = (coord.Item2 - minY + 6) * 1.7;
                polygon.Points.Add(new Point(x, y));
            }

            // Выбираем базовый цвет для заливки
            var baseColor = Color.FromRgb(
                (byte)rnd.Next(100, 256),
                (byte)rnd.Next(100, 256),
                (byte)rnd.Next(100, 256));

            polygon.Fill = new RadialGradientBrush(
                Color.FromArgb(180, baseColor.R, baseColor.G, baseColor.B),
                Color.FromArgb(50, baseColor.R, baseColor.G, baseColor.B))
            {
                RadiusX = 0.8,
                RadiusY = 0.8,
                Center = new Point(0.5, 0.5),
                GradientOrigin = new Point(0.5, 0.5)
            };

            // Обработчики для подсветки при наведении мыши
            polygon.MouseEnter += (sender, e) =>
            {
                var poly = sender as Polygon;
                poly.StrokeThickness = 4;
                poly.Stroke = new SolidColorBrush(Color.FromRgb(
                    (byte)Math.Min(baseColor.R + 50, 255),
                    (byte)Math.Min(baseColor.G + 50, 255),
                    (byte)Math.Min(baseColor.B + 50, 255)));
                poly.Fill = new RadialGradientBrush(
                    Color.FromArgb(220, baseColor.R, baseColor.G, baseColor.B),
                    Color.FromArgb(80, baseColor.R, baseColor.G, baseColor.B))
                {
                    RadiusX = 0.85,
                    RadiusY = 0.85,
                    Center = new Point(0.5, 0.5),
                    GradientOrigin = new Point(0.5, 0.5)
                };
            };

            polygon.MouseLeave += (sender, e) =>
            {
                var poly = sender as Polygon;
                poly.StrokeThickness = 2;
                poly.Stroke = Brushes.DimGray;
                poly.Fill = new RadialGradientBrush(
                    Color.FromArgb(180, baseColor.R, baseColor.G, baseColor.B),
                    Color.FromArgb(50, baseColor.R, baseColor.G, baseColor.B))
                {
                    RadiusX = 0.8,
                    RadiusY = 0.8,
                    Center = new Point(0.5, 0.5),
                    GradientOrigin = new Point(0.5, 0.5)
                };
            };

            polygon.MouseLeftButtonUp += (sender, e) =>
            {
                var poly = sender as Polygon;
                int sectorId = (int)poly.Tag;
   
            };

            sectorView.Children.Add(polygon);
        }


        public void CreateProfilesList()
        {
            DataTable dataTable = dataBase.SqlSelect("select * from Profiles where IdSector = " + currentSector.IdSector);
            if (dataTable.Rows.Count == 0)
            {
                return;
            }
            Profile profile;
            for(int i = 0; i < dataTable.Rows.Count - 1; i++)
            {
                profile = new Profile(Convert.ToInt32(dataTable.Rows[i][0]), Convert.ToInt32(dataTable.Rows[i][1]), Convert.ToInt32(dataTable.Rows[i][2]));
                DataTable dt = dataBase.SqlSelect("select * from ProfileCoordinates where IdProfile = " + Convert.ToInt32(dataTable.Rows[i][0]));
                for(int j = 0; j < dt.Rows.Count; j++)
                {
                    Tuple<int, int> coord = new Tuple<int, int>(Convert.ToInt32(dt.Rows[j][2]), Convert.ToInt32(dt.Rows[j][3]));
                    profile.Coordinates.Add(coord);
                }
                currentSector.Profiles.Add(profile);
            }
        }

        public void DrawProfiles()
{
    sectorView.Children.Clear();
    DrawSector();

    foreach (var p in currentSector.Profiles)
    {
        if (p.Coordinates.Count < 2)
            continue;

        Point profileStart = new Point(
            (p.Coordinates[0].Item1 - minX + 6) * 1.7,
            (p.Coordinates[0].Item2 - minY + 6) * 1.7);

        Point profileNext = new Point(
            (p.Coordinates[1].Item1 - minX + 6) * 1.7,
            (p.Coordinates[1].Item2 - minY + 6) * 1.7);

        Vector direction = profileNext - profileStart;

        // Нормализуем направление
        direction.Normalize();

        // Длинна луча — достаточно большая, чтобы пересечь полигон
        double rayLength = 10000;

        Point rayEnd = profileStart + direction * rayLength;

        // Ищем пересечение луча с контуром полигона
        Point? intersection = FindPolygonRayIntersection(profileStart, rayEnd, currentSector.Coordinates);

        if (intersection != null)
        {
            var line = new Line
            {
                X1 = intersection.Value.X,
                Y1 = intersection.Value.Y,
                X2 = profileStart.X,
                Y2 = profileStart.Y,
                Stroke = Brushes.DarkBlue,
                StrokeThickness = 3,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                Tag = p.IdProfile
            };

            sectorView.Children.Add(line);
        }
        else
        {
            // Если пересечения нет, рисуем просто короткую линию
            var line = new Line
            {
                X1 = profileStart.X,
                Y1 = profileStart.Y,
                X2 = profileStart.X + direction.X * 20,
                Y2 = profileStart.Y + direction.Y * 20,
                Stroke = Brushes.DarkBlue,
                StrokeThickness = 3,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                Tag = p.IdProfile
            };
            sectorView.Children.Add(line);
        }
    }
}

        // Функция поиска пересечения луча с многоугольником
        // Функция поиска пересечения луча с многоугольником
        private Point? FindPolygonRayIntersection(Point rayStart, Point rayEnd, List<Tuple<int, int>> polygonCoords)
        {
            Point? closestIntersection = null;
            double minDist = double.MaxValue;

            for (int i = 0; i < polygonCoords.Count; i++)
            {
                int nextIndex = (i + 1) % polygonCoords.Count;

                //Point p1 = new Point(
                //    (polygonCoords[i].Item1 - minX + 6) * 1.7,
                //    (polygonCoords[i].Item2 - minY + 6) * 1.7);

                //Point p2 = new Point(
                //    (polygonCoords[nextIndex].Item1 - minX + 6) * 1.7,
                //    (polygonCoords[nextIndex].Item2 - minY + 6) * 1.7);

                //if (LineSegmentsIntersect(rayStart, rayEnd, p1, p2, out Point intersection))
                //{
                //    double dist = (intersection - rayStart).Length;
                //    if (dist < minDist)
                //    {
                //        minDist = dist;
                //        closestIntersection = intersection;
                //    }
                //}
            }

            return closestIntersection;
        }


        // Проверка пересечения двух отрезков и вычисление точки пересечения
        private bool LineSegmentsIntersect(Point p1, Point p2, Point p3, Point p4, out Point intersection)
{
    intersection = new Point();

    double denominator = (p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y);
    if (denominator == 0)
        return false; // Параллельны

    double ua = ((p4.X - p3.X) * (p1.Y - p3.Y) - (p4.Y - p3.Y) * (p1.X - p3.X)) / denominator;
    double ub = ((p2.X - p1.X) * (p1.Y - p3.Y) - (p2.Y - p1.Y) * (p1.X - p3.X)) / denominator;

    if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
    {
        intersection.X = p1.X + ua * (p2.X - p1.X);
        intersection.Y = p1.Y + ua * (p2.Y - p1.Y);
        return true;
    }

    return false;
}




        public void FillSectorText()
        {
            sectorId.Text = currentSector.IdSector.ToString();
            sectorSquare.Text = currentSector.SquareSector.ToString();
            profilesAmount.Text = (currentSector.Profiles.Count + 1).ToString();
            string text = "";
            int i = 0;
            foreach (var c in currentSector.Coordinates)
            {
                text = c.Item1.ToString() + " " + c.Item2.ToString();
                listCoordinates.Items.Insert(i, text);
                i++;
            }
        }

        public void CreateButtons()
        {
            buttonsPanel.Children.Clear();

            foreach (var p in currentSector.Profiles)
            {
                var btn = CreateStyledButton(p.IdProfile);
                buttonsPanel.Children.Add(btn);
            }

            // Дополнительная кнопка Профиль 7
            var extraBtn = CreateStyledButton(7);
            buttonsPanel.Children.Add(extraBtn);
        }

        private Button CreateStyledButton(int profileId)
        {
            var btn = new Button
            {
                Content = profileId == 1 ? "Профиль 1" : $"Профиль {profileId}",
                Tag = profileId,
                Width = 120,
                Height = 40,
                Margin = new Thickness(5),
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Background = new SolidColorBrush(Color.FromRgb(33, 150, 243)), // Синий
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Padding = new Thickness(10, 5, 10, 5),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };

            btn.Click += btn_Click;

            // Эффект наведения
            btn.MouseEnter += (s, e) =>
            {
                btn.Background = new SolidColorBrush(Color.FromRgb(30, 136, 229)); // Темнее синий
            };
            btn.MouseLeave += (s, e) =>
            {
                btn.Background = new SolidColorBrush(Color.FromRgb(33, 150, 243)); // Исходный синий
            };

            // Эффект нажатия
            btn.PreviewMouseDown += (s, e) =>
            {
                btn.Background = new SolidColorBrush(Color.FromRgb(25, 118, 210)); // Ещё темнее
            };
            btn.PreviewMouseUp += (s, e) =>
            {
                btn.Background = new SolidColorBrush(Color.FromRgb(30, 136, 229));
            };

            // Скруглённые углы
            btn.Template = GetRoundedButtonTemplate();

            return btn;
        }

        private ControlTemplate GetRoundedButtonTemplate()
        {
            var templateXaml = @"
    <ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' TargetType='Button'>
      <Border Background='{TemplateBinding Background}' 
              CornerRadius='8' 
              BorderThickness='{TemplateBinding BorderThickness}' 
              BorderBrush='{TemplateBinding BorderBrush}'>
        <ContentPresenter HorizontalAlignment='Center' VerticalAlignment='Center'/>
      </Border>
    </ControlTemplate>";

            return (ControlTemplate)System.Windows.Markup.XamlReader.Parse(templateXaml);
        }

        public void btn_Click(object sender, EventArgs e)
        {
            Profile selectedProfile = null;
            foreach (var p in currentSector.Profiles)
            {
                if (p.IdProfile == Convert.ToInt32(((Button)sender).Tag))
                {
                    selectedProfile = p;
                    break;
                }
            }
            ProfileWindow window = new ProfileWindow(selectedProfile);
            window.Owner = this;
            window.Show();
            this.Hide();
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        public void FillDataGrid()
        {
            DataTable dataTable = dataBase.SqlSelect("select * from Profiles, ProfileCoordinates where " +
                "Profiles.IdProfile = ProfileCoordinates.IdProfile and IdSector = " + currentSector.IdSector);
            gridProfiles.ItemsSource = dataTable.DefaultView;
        }
    }
}
