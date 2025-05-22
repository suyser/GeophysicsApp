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
    /// Логика взаимодействия для AddEditSectorWindow.xaml
    /// </summary>
    public partial class AddEditSectorWindow : Window
    {
        public DataBase dataBase = new DataBase();
        public Sector currentSector;
        public AddEditSectorWindow(Sector sector)
        {
            InitializeComponent();
            dataBase.GetConnection();
            currentSector = sector;
            FillTextBox();
        }

        public void FillTextBox()
        {
            if (currentSector == null) return;
            textSquare.Text = currentSector.SquareSector.ToString();
            string text;
            foreach (var c in currentSector.Coordinates)
            {
                text = c.Item1.ToString() + " " + c.Item2.ToString() + "\n";
                listCoordinates.Text += text;
            }
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            if(textSquare.Text == "" || listCoordinates.Text == "")
            {
                MessageBox.Show("Сначала заполните все поля");
                return;
            }
            if(currentSector == null)
            {
                dataBase.SqlQuery("insert into Sectors values (1, " + textSquare.Text + ")");
                DataTable dataTable = dataBase.SqlSelect("select IdSector from Sectors where IdProject = 1 and SquareSector = " +
                    textSquare.Text);
                string idSector = dataTable.Rows[0][0].ToString();
                string[] arr = listCoordinates.Text.Split('\n');
                string[] coord;
                int i = 1;
                foreach (var a in arr)
                {
                    coord = a.Split(' ');
                    dataBase.SqlQuery("insert into SectorCoordinates values (" + idSector + ", " + i.ToString() +
                        ", " + coord[0] + ", " + coord[1] + ")");
                    i++;
                }
                MessageBox.Show("Участок добавлен");
                this.Owner.Show();
                this.Close();
            }
            else
            {
                dataBase.SqlQuery("update Sectors set SquareSector = " + textSquare.Text + " where IdSector = " + currentSector.IdSector.ToString());
                dataBase.SqlQuery("delete from SectorCoordinates where IdSector = " + currentSector.IdSector.ToString());
                string[] arr = listCoordinates.Text.Split('\n');
                string[] coord;
                int i = 1;
                foreach (var a in arr)
                {
                    coord = a.Split(' ');
                    if (coord[0] == "" || coord[1] == "") break;
                    dataBase.SqlQuery("insert into SectorCoordinates values (" + currentSector.IdSector.ToString() + ", " + i.ToString() +
                        ", " + coord[0] + ", " + coord[1] + ")");
                    i++;
                }
                MessageBox.Show("Участок изменён");
                this.Owner.Show();
                this.Close();
            }
        }
    }
}
