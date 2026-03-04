using System.Windows;

namespace DiceGame1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var gm = new GameManager();
            LblBestMenu.Text = $"Лучший результат: {gm.BestScore}";
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            var gw = new GameWindow();
            gw.Show();
            this.Close();
        }

        private void BtnRules_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Правила:\n" +
                "1) Создайте исходный кубик — укажите ровно 6 значений (1..6).\n" +
                "2) Раунд 1: только оригинал. Смотри цель и бросай.\n" +
                "3) После успешного попадания: появляется цель следующего раунда — выбери модификатор и создай новые клоны (из оригинала).\n" +
                "4) Повторяй: количество новых клонов растёт (1,2,4,8...).",
                "Правила");
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}