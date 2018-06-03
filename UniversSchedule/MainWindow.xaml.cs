using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UniversSchedule
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BD_StudyEntities DB = new BD_StudyEntities();

        public MainWindow()
        {
            InitializeComponent();

            search.ItemsSource = DB.Groups.Select(x => x.name_of_group).ToList();

        }



        private void FIllSchedule(int id)
        {
            
            for (int i = 1; i < 7; i++)
            {
                var day = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + i)
                    .ToString("dd.MM.yyyy");
               
                WrapPanel wp = new WrapPanel()
                {
                    Background = new SolidColorBrush(Color.FromRgb(248, 248, 248)),
                    Margin = new Thickness(10,0,0,0),
                    Orientation = Orientation.Vertical
                };


                TextBlock tb = new TextBlock()
                {
                    Text = day,
                    Background = new SolidColorBrush(Color.FromRgb(102, 122, 154)),
                    Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                    Padding = new Thickness(10,0,0,0)
                };
                wp.Children.Add(tb);
                




                foreach (var item in DB.Schedule.Where(x => x.data == day && x.id_group == id))
                { 
                    WrapPanel wpLesson = new WrapPanel()
                    {
                        Orientation = Orientation.Horizontal
                    };
                    wp.Children.Add(wpLesson);

                    #region Время занятий

                    
                    WrapPanel wpTime = new WrapPanel()
                    {
                        Orientation = Orientation.Vertical,
                        Margin = new Thickness(0,0,10,0)
                    };

                    TextBlock tbStart = new TextBlock()
                    {
                        Text = item.start_time,
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        Foreground = new SolidColorBrush(Color.FromRgb(94, 94, 115))
                    };

                    TextBlock tbEnd = new TextBlock() {
                        Text = item.end_time,
                        Foreground = new SolidColorBrush(Color.FromRgb(169, 169, 199))
                        
                    };
                    
                    wpTime.Children.Add(tbStart);
                    wpTime.Children.Add(tbEnd);
                    wpLesson.Children.Add(wpTime);

                    #endregion

                    #region Занятие и преподователь

                    
                    WrapPanel wpNameAndTeacher = new WrapPanel() {
                        Orientation = Orientation.Vertical,
                        Width = 140
                    };

                    TextBlock tbNameDiscipline= new TextBlock()
                    {
                        Text = DB.Disciplines.Where(x => x.id_discipline == item.id_discipline).FirstOrDefault().name_of_discipline.ToString()
                    };

                    TextBlock tbNameTeacher = new TextBlock()
                    {
                        Text = DB.Teachers.Where(x => x.id_teacher == item.id_teacher).FirstOrDefault().FIO_teacher.ToString(),
                        Foreground = new SolidColorBrush(Color.FromRgb(35, 82, 124)),
                        
                    };

                    wpNameAndTeacher.Children.Add(tbNameDiscipline);
                    wpNameAndTeacher.Children.Add(tbNameTeacher);

                    wpLesson.Children.Add(wpNameAndTeacher);
                    #endregion

                    TextBlock classRoom = new TextBlock()
                    {
                        Text = item.room
                       
                    };
                    wpLesson.Children.Add(classRoom);


                }




                Schedule.Children.Add(wp);

            }

           

            
            
        }

        private void search_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Schedule.Children.Clear();
            var id = DB.Groups.Where(x => x.name_of_group == ((ComboBox)sender).SelectedItem.ToString()).FirstOrDefault().id_group.ToString();

            FIllSchedule(int.Parse(id));
        }
    }
}
