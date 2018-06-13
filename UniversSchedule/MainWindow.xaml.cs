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
        int editID;
        bool state, stateStatus=true;
        Visibility visibilityButton = Visibility.Visible;
        string selectDate;


        public MainWindow()
        {
            InitializeComponent();
            var fdf = DB.Teachers.Select(x => x.FIO_teacher).ToList().Concat(DB.Groups.Select(x => x.name_of_group).ToList());
            search.ItemsSource = fdf;
            cbNameSubject.ItemsSource = DB.Disciplines.Select(x => x.name_of_discipline).ToList();
            cbTeacher.ItemsSource = DB.Teachers.Select(x => x.FIO_teacher).ToList();
            // дописать изменение, добавление пар и нормально оссортировать пары 
        }



        private void FIllSchedule(int? id_teacher=null, int? id_group = null )
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
                




                foreach (var item in DB.Schedule.Where(x => x.data == day && (x.id_group == id_group || x.id_teacher == id_teacher)))
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

                    TextBlock tbNameDiscipline = new TextBlock()
                    {
                        Text = DB.Disciplines.Where(x => x.id_discipline == item.id_discipline).FirstOrDefault().name_of_discipline.ToString() +
                        $"({DB.Groups.Where(x => x.id_group == item.id_group).FirstOrDefault().name_of_group.ToString()})"
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

                    WrapPanel wpNameRoomAndEditButton = new WrapPanel()
                    {
                        Orientation = Orientation.Vertical
                    };

                    TextBlock classRoom = new TextBlock()
                    {
                        Text = item.room
                    };
                    
                    Button btEdit = new Button()
                    {
                        Content = "Edit",
                        Tag =item.id_record,
                        Visibility = visibilityButton
                    };
                    btEdit.Click += BtEdit_Click;

                    Button btDelete = new Button()
                    {
                        Content = "Delete",
                        Tag = item.id_record,
                        Visibility = visibilityButton
                    };
                    btDelete.Click += BtDelete_Click; 
                    wpNameRoomAndEditButton.Children.Add(classRoom);
                    wpNameRoomAndEditButton.Children.Add(btEdit);
                    wpNameRoomAndEditButton.Children.Add(btDelete);

                    wpLesson.Children.Add(wpNameRoomAndEditButton);
                }
                Schedule.Children.Add(wp);

                Button b = new Button()
                {
                    Content = "Добавить занятие",
                    Tag = day,
                    Visibility = id_group == null ? Visibility.Collapsed: visibilityButton
                };
                b.Click += B_Click;
                wp.Children.Add(b);
            }  
        }

        private void B_Click(object sender, RoutedEventArgs e)
        {
            EditAndAddElement.Visibility = Visibility.Visible;
            state = true;
            selectDate = (sender as Button).Tag.ToString();
            
        }

        private void BtDelete_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)(sender as Button).Tag;
            DB.Schedule.Remove(DB.Schedule.Where(x => x.id_record == id).First());
            DB.SaveChanges();
            updateTable();
            //  обновление таблицы
        }

        private void updateTable()
        {
            string name = search.SelectedItem.ToString();
            search.SelectedItem = null;
            search.SelectedItem = name;
        }

        private void BtEdit_Click(object sender, RoutedEventArgs e)
        {
            EditAndAddElement.Visibility = Visibility.Visible;
            state = false;
            int id = (int)(sender as Button).Tag;
            editID = id;
            var element = DB.Schedule.Where(x => x.id_record == id).First();
            cbNameSubject.Text = DB.Disciplines.Where(x => x.id_discipline == element.id_discipline).FirstOrDefault().name_of_discipline.ToString();
            cbTeacher.Text = DB.Teachers.Where(x => x.id_teacher == element.id_teacher).FirstOrDefault().FIO_teacher.ToString();
            tbRoom.Text = element.room;
            string[] startTime = element.start_time.Split(':');
            tbStartHours.Text = startTime[0];
            tbStartMinutes.Text = startTime[1];

            string[] endTime = element.end_time.Split(':');
            tbEndHours.Text = endTime[0];
            tbEndMinutes.Text = endTime[1];
        }

        private void search_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Schedule.Children.Clear();
            string id_teacher,id_group;
            string el = ((ComboBox)sender).SelectedItem?.ToString();
            if (el == null) { }
            else { 
            if (DB.Groups.Where(x => x.name_of_group == el).Count() == 0)
            {
                id_teacher = DB.Teachers.Where(x => x.FIO_teacher == el).FirstOrDefault().id_teacher.ToString();
                int id = int.Parse(id_teacher);
                FIllSchedule(id);
                
                tbName.Text = DB.Teachers.Where(x => x.id_teacher == id).FirstOrDefault().FIO_teacher.ToString();
            }
            else
            {
                id_group = DB.Groups.Where(x => x.name_of_group == el).FirstOrDefault().id_group.ToString();
                int id = int.Parse(id_group);
                FIllSchedule(null,id);
                tbName.Text = DB.Groups.Where(x=> x.id_group == id).FirstOrDefault().name_of_group.ToString();
            }
            }

        }

   

        private void btAccept_Click(object sender, RoutedEventArgs e)
        {
            if(state)
            {
                Schedule schedule = new Schedule()
                {
                    id_discipline = DB.Disciplines.Where(x => x.name_of_discipline == cbNameSubject.Text).First().id_discipline,
                    id_teacher = DB.Teachers.Where(x => x.FIO_teacher == cbTeacher.Text).First().id_teacher,
                    id_group = DB.Groups.Where(x=> x.name_of_group == tbName.Text.ToString()).First().id_group,
                    room = tbRoom.Text,
                    data = selectDate,
                    start_time = tbStartHours.Text + ":" + tbStartMinutes.Text,
                    end_time = tbStartHours.Text + ":" + tbStartMinutes.Text
                };
                DB.Schedule.Add(schedule);
            }
            else
            {
                var el = DB.Schedule.Where(x => x.id_record == editID).First();
                el.id_discipline = DB.Disciplines.Where(x => x.name_of_discipline == cbNameSubject.Text).First().id_discipline;
                el.id_teacher = DB.Teachers.Where(x => x.FIO_teacher == cbTeacher.Text).First().id_teacher;
                el.room = tbRoom.Text;
                el.start_time = tbStartHours.Text + ":" + tbStartMinutes.Text;
                el.end_time = tbEndHours.Text + ":" + tbEndMinutes.Text;
            }
            
            DB.SaveChanges();
            EditAndAddElement.Visibility = Visibility.Hidden;
            updateTable();
            //search.SelectionChanged += search_SelectionChanged;
            //обновление таблицы
        }

        private void singIn_Click(object sender, RoutedEventArgs e) 
        {
            if (stateStatus) {
                visibilityButton = Visibility.Collapsed;
                singIn.Header = "Войти как учитель";
                stateStatus = !stateStatus;
            }
            else
            {
                
                visibilityButton = Visibility.Visible;
                
                singIn.Header = "Войти как студент";
                stateStatus = !stateStatus;
            }
        }
    }
    

}
