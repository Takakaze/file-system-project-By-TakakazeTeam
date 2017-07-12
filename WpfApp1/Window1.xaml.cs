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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int i;
            if (Array.IndexOf<string>(ID.Username, username.Text) == -1)//验证用户名
            {
                MessageBox.Show("查无此用户！");
            }
            else
            {
                for (i = 0; i < 8; i++)
                {
                    if (username.Text == ID.Username[i])
                    {
                        if (password.Password == ID.Password[i])
                        {
                            ID.login_userid = i;
                            MainWindow mw = new MainWindow();
                            mw.ShowDialog();//返回登陆页面框
                            Close();//关闭页面 
                        }
                        else
                        {
                            MessageBox.Show("密码错误！");
                        }
                    }
                }                                
            }
        }
    }
}
