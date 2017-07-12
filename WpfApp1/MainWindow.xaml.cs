using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Automation.Peers;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 命令发布
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_click(object sender, RoutedEventArgs e)
        {
            textbox1.Text += textbox2.Text;
            textbox1.Text += " ";
            textbox1.Text += "\n";
            mainloop();

        }

        /// <summary>
        /// 保证光标在底部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textbox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.textbox1.Focus();
            this.textbox1.Select(this.textbox1.Text.Length, 0);
        }

        /// <summary>
        /// logoff操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_click(object sender, RoutedEventArgs e)
        {
            write_file();    //退出登录
            Close();//关闭页面 
            Window1 mw = new Window1();
            mw.ShowDialog();//返回登陆页面框
        }

        /// <summary>
        /// quit操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_click(object sender, RoutedEventArgs e)
        {
            write_file();    //退出登录
            Close();
        }

        /// <summary>
        /// 主函数
        /// </summary>
        private void main()
        {
            InitializeComponent();
            int i;
            //define all struct arrays
            for (i = 0; i < 22; i++)
            {
                DATA.cmd.Add(new command());
            }

            for (i = 0; i < 20449; i++)
            {
                DATA.memory.Add(new block());
            }

            DATA.super_block.Add(new block_super());

            for (i = 0; i < 640; i++)
            {
                DATA.i_node.Add(new node());
                DATA.root.Add(new dir());
            }
            //define end
            help();
            DATA.cmd[0].com = "format";    //将各个命令存进命令表
            DATA.cmd[1].com = "dir";
            DATA.cmd[2].com = "cat";
            DATA.cmd[3].com =  "ls";
            DATA.cmd[4].com = "md";
            DATA.cmd[5].com = "vi";
            DATA.cmd[6].com = "del";
            DATA.cmd[7].com = "deldir";
            DATA.cmd[8].com = "cd";
            DATA.cmd[9].com = "cd..";
            DATA.cmd[10].com = "help";
            DATA.cmd[11].com = "quit";
            DATA.cmd[12].com = "open";
            DATA.cmd[13].com = "read";
            DATA.cmd[14].com = "write";
            DATA.cmd[15].com = "logout";
            DATA.cmd[16].com = "close";
            DATA.cmd[17].com = "chdir";
            DATA.cmd[18].com = "chlim";
            DATA.cmd[19].com = "chnam";
            DATA.cmd[20].com = "cp";
            DATA.cmd[21].com = "clin";
            try    //判断系统文件是否存在
            {
                read_file();
            }
            catch (Exception ex)
            {
                textbox1.Text += ex;
                textbox1.Text += "disk format!";
                format();
            }

            DATA.j = 0;         //必须重新给恢复0否则出错
            DATA.tmp = DATA.cur_dir;

            while (DATA.tmp != "filsystem")
            {
                for (i = 0; i < 640; i++)
                {
                    DATA.p = DATA.root[i].i_num;
                    if ((DATA.tmp == DATA.root[i].file_name) && (DATA.i_node[DATA.p].file_style == 0))
                    {
                        DATA.tmp2[DATA.j].com = DATA.tmp;
                        DATA.j++;
                        DATA.tmp = DATA.root[i].dir_name;
                    }
                }
            }
            DATA.tmp2.Add(new command());
            DATA.tmp2[DATA.j].com = DATA.tmp;
            for (i = DATA.j; i >= 0; i--)
            {
                textbox1.Text += DATA.tmp2[i].com.ToString();
                textbox1.Text += "/";
            }
        }

        /// <summary>
        /// 格式化
        /// </summary>
        void format()
        {
            int i, j, k;
            DATA.super_block[0].n = 50;
            for (i = 0; i < 50; i++)     //超级块初始化
            {
                DATA.super_block[0].free[i] = i;   //存放进入栈中的空闲块
                                           //super_block.stack[i]=50+i;  //存放下一组的盘块
            }
            for (i = 0; i < 640; i++)     //i结点信息初始化
            {
                for (j = 0; j < 100; j++)
                {
                    DATA.i_node[i].file_address[j] = -1;//文件地址
                }
                DATA.i_node[i].limit = -1;
                DATA.i_node[i].file_length = -1;  //文件长度
                DATA.i_node[i].file_style = -1; //文件类型
                DATA.i_node[i].file_UserId = -1;//用户ID
            }
            for (i = 0; i < 640; i++)     //目录项信息初始化
            {
                DATA.root[i].file_name = "";
                DATA.root[i].i_num = -1;
                DATA.root[i].dir_name = "";
            }
            for (i = 0; i < 20449; i++)     //存储空间初始化
            {
                DATA.memory[i].n = 0;      //必须有这个
                DATA.memory[i].a = 0;
                for (j = 0; j < 50; j++)
                {
                    DATA.memory[i].free[j] = -1;
                }
                DATA.memory[i].content = "\0";
            }
            for (i = 0; i < 20449; i++)    //将空闲块的信息用成组链接的方法写进每组的最后一个块中
            {         //存储空间初始化
                if ((i + 1) % 50 == 0)
                {
                    k = i + 1;
                    for (j = 0; j < 50; j++)
                    {
                        if (k < 20450)
                        {
                            DATA.memory[i].free[j] = k;//下一组空闲地址
                            DATA.memory[i].n++;  //下一组空闲个数   注意在memory[i].n++之前要给其赋初值
                            k++;
                        }
                        else
                        {
                            DATA.memory[i].free[j] = -1;
                        }
                    }
                    DATA.memory[i].a = 0;    //标记为没有使用
                    continue;     //处理完用于存储下一组盘块信息的特殊盘块后，跳过本次循环
                }
                for (j = 0; j < 50; j++)
                {
                    DATA.memory[i].free[j] = -1;
                }
                DATA.memory[i].n = 0;
            }
            int l;
            for (l = 0; l < 8; l++)
            {
                DATA.file_array[l] = -1;
            }
            DATA.file_array_head = 0;
            int u;
            for (u = 0; u < 100; u++)
            {
                DATA.physic[u] = -1;
            }
            textbox1.Text += "已经初始化完毕\n";
            textbox1.Text += "进入UNIX文件模拟............\n\n";
        }

        /// <summary>
        /// 回收磁盘空间
        /// </summary>
        /// <param name="length"></param>
        void callback(int length)
        {
            int i, j, k, m, q = 0;
            for (i = length - 1; i >= 0; i--)
            {
                k = DATA.physic[i];     //需要提供要回收的文件的地址
                m = 49 - DATA.super_block[0].n;    //回收到栈中的哪个位置
                if (DATA.super_block[0].n == 50)   //注意 当super_block.n==50时 m=-1;的值
                {        //super_block.n==50的时候栈满了，要将这个栈中的所有地址信息写进下一个地址中
                    for (j = 0; j < 50; j++)
                    {
                        DATA.memory[k].free[j] = DATA.super_block[0].free[j];
                    }
                    int u;
                    for (u = 0; u < 50; u++)
                    {
                        DATA.super_block[0].free[u] = -1;
                        //super_block.stack[u]=memory[k].free[u];
                    }
                    DATA.super_block[0].n = 0;
                    DATA.memory[k].n = 50;
                }
                DATA.memory[k].a = 0;
                if (m == -1)
                {
                    m = 49;      //将下一个文件地址中的盘块号回收到栈底中，这个地址中存放着刚才满栈的地址的信息
                }
                DATA.super_block[0].free[m] = DATA.physic[i]; //将下一个文件地址中的盘块号回收到栈中
                DATA.super_block[0].n++;
            }
        }

        /// <summary>
        /// 分配空间
        /// </summary>
        /// <param name="length"></param>
        void allot(int length) 
        {
            int i, j, k, m, p;
            for (i = 0; i < length; i++)
            {
                k = 50 - DATA.super_block[0].n;    //超级块中表示空闲块的指针
                m = DATA.super_block[0].free[k];   //栈中的相应盘块的地址
                p = DATA.super_block[0].free[49];   //栈中的最后一个盘块指向的地址
                if (p == -1/*||memory[p].a==1*/)  //没有剩余盘块
                {
                    textbox1.Text += "内存不足,不能够分配空间\n";
                    callback(i);//之前已分配的i个盘块回收；
                    break;
                }
                if (DATA.super_block[0].n == 1)
                {
                    DATA.memory[m].a = 1;    //将最后一个盘块分配掉
                    DATA.physic[i] = m;
                    DATA.super_block[0].free[49] = -1;
                    DATA.super_block[0].n = 0;
                    for (j = 50 - DATA.memory[m].n; j < 50; j++) //从最后一个盘块中取出下一组盘块号写入栈中
                    {
                        DATA.super_block[0].free[j] = DATA.memory[m].free[j];
                        DATA.super_block[0].n++;
                    }
                    continue;     //要跳过这次循环，下面的语句在IF中已经执行过
                }
                DATA.physic[i] = m;     //如果栈中超过一个盘，栈中的相应盘块的地址写进 文件地址缓冲区
                DATA.memory[m].a = 1;
                m = -1;
                DATA.super_block[0].n--;
            }
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="length"></param>
        /// <param name="userid"></param>
        /// <param name="limit"></param>
        void create_file(string filename, int length, int userid, int limit) 
        {
            int i, j;
            for (i = 0; i < 640; i++)
            {
                if (filename == DATA.root[i].file_name)
                {
                   textbox1.Text += "已经存在同名文件，不允许建立重名的文件\n";
                    return;
                }
            }
            for (i = 0; i < 640; i++)
            {
                if (DATA.root[i].i_num == -1)
                {
                    DATA.root[i].i_num = i;
                    DATA.root[i].file_name = filename;
                    DATA.root[i].dir_name = DATA.cur_dir;  //把当前目录名 给新建立的文件
                    DATA.i_node[i].file_style = DATA.style;//style==0 说明文件是目录文件
                    DATA.i_node[i].file_length = length;
                    DATA.i_node[i].limit = limit;
                    DATA.i_node[i].file_UserId = userid; //printf("%s.%d\n",root[i].file_name,i_node[i].file_UserId);
                    allot(length);
                    for (j = 0; j < length; j++)
                    {
                        DATA.i_node[i].file_address[j] = DATA.physic[j];
                    }
                    int u;
                    for (u = 0; u < 100; u++)//分配完清空缓冲区
                    {
                        DATA.physic[u] = -1;
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filename"></param>
        void del_file(string filename)
        {
            int i, j, k;
            for (i = 0; i < 640; i++)
            {
                if ((filename == DATA.root[i].file_name) && (DATA.cur_dir == DATA.root[i].dir_name) && (DATA.i_node[DATA.root[i].i_num].file_UserId == ID.login_userid))
                {//printf("1get here\n");
                    int add, c;
                    if (DATA.root[i].file_style == 0)
                    {
                        for (add = 0; add < DATA.i_node[DATA.root[i].i_num].file_length; add++)//文件内容清空
                        {
                            for (c = 0; DATA.memory[DATA.i_node[DATA.root[i].i_num].file_address[add]].content[c] != '\0'; c++)
                            {
                                DATA.memory[DATA.i_node[DATA.root[i].i_num].file_address[add]].content = "\0";
                            }
                        }
                        k = DATA.root[i].i_num; //printf("2get here\n");
                        DATA.i_node[k].file_UserId = -1;
                        DATA.i_node[k].limit = -1;
                        for (j = 0; j < DATA.i_node[k].file_length; j++)
                        {
                            DATA.physic[j] = DATA.i_node[k].file_address[j];
                        }// printf("get here\n");
                        callback(DATA.i_node[k].file_length); //调用 回收函数
                        int u;//回收完情空缓存区
                        for (u = 0; u < 100; u++)
                        {
                            DATA.physic[u] = -1;
                        }
                        for (j = 0; j < 100; j++)     //删除文件后要将文件属性和目录项的各个值恢复初值
                        {
                            DATA.i_node[k].file_address[j] = -1; //文件占用的块号地址恢复初值
                        }
                        DATA.i_node[k].file_length = -1;   //文件长度恢复
                        DATA.i_node[k].file_style = -1;   //文件类型恢复初值
                    }
                    DATA.root[i].file_name = "";  //文件名恢复初值
                    DATA.root[i].i_num = -1;     //目录项的I结点信息恢复初值
                    DATA.root[i].dir_name = "";  //目录项的文件目录信息恢复初值
                    DATA.root[i].file_style = 0; //快捷方式处理
                    break;
                }
            }
            if (i == 640)
            {
                textbox1.Text += "登录用户的该目录下不存在这个文件\n";
            }
        }

        /// <summary>
        /// 显示系统信息（磁盘使用情况）
        /// </summary>
        void display_sys()
        {
            int i, m, k = 0;
            for (i = 0; i < 20449; i++)
            {
                if (DATA.memory[i].a == 0)
                    k++;
            }
            m = 20449 - k;
            textbox1.Text += "空闲的盘块数是：\t";
            textbox1.Text += k.ToString(); 
            textbox1.Text += "\n";
            textbox1.Text += "使用的盘块数是：\t";
            textbox1.Text += m.ToString();
            textbox1.Text += "\n";
        }

        /// <summary>
        /// 显示文件信息
        /// </summary>
        /// <param name="filename"></param>
        void show_file(string filename)
        {
            int i, j, k;
            textbox1.Text += "\t\t文件名字  文件类型  文件长度  读取权限  所属目录\t所属用户\n";
            for (i = 0; i < 640; i++)
            {
                k = DATA.root[i].i_num;
                if ((filename == DATA.root[i].file_name) && (DATA.i_node[k].file_style == 1))
                {
                    textbox1.Text += "\t\t";
                    textbox1.Text += DATA.root[i].file_name.ToString();  //文件名
                    textbox1.Text += "\t";
                    textbox1.Text += "\t";
                    textbox1.Text += DATA.i_node[k].file_style.ToString();  //文件的类型
                    textbox1.Text += "\t";
                    textbox1.Text += DATA.i_node[k].file_length.ToString(); //文件的长度
                    textbox1.Text += "\t";
                    textbox1.Text += DATA.i_node[k].limit.ToString();
                    textbox1.Text += "\t";
                    textbox1.Text += DATA.root[i].dir_name;  //文件所在的目录
                    textbox1.Text += "\t";
                    textbox1.Text += "user";
                    textbox1.Text += DATA.i_node[DATA.root[i].i_num].file_UserId.ToString();
                    textbox1.Text += "\n";
                    textbox1.Text += "\t\t文件占用的物理地址\n";
                    for (j = 0; j < DATA.i_node[k].file_length; j++)   //显示物理地址
                    {
                        textbox1.Text += DATA.i_node[k].file_address[j].ToString(); //文件具体占用的盘块号
                        textbox1.Text += " ";
                    }
                    textbox1.Text += "\n";
                    break;
                }
            }
            if (i == 640)
            {
                textbox1.Text += "没有这个文件 或者这个文件不是正规文件\n";
            }
        }

        /// <summary>
        /// 将信息写入系统文件
        /// </summary>
        unsafe void write_file()
        {
            int i;
            FileStream fs = new FileStream("system.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryWriter bw = new BinaryWriter(fs, Encoding.UTF8);
            for (i = 0; i < 20449; i++)
            {
                bw.Write(DATA.memory[i].n);
                for (int j = 0; j < 50; j++)
                {
                    bw.Write(DATA.memory[i].free[j]);
                }
                bw.Write(DATA.memory[i].a);
                bw.Write(DATA.memory[i].content);
            }

            bw.Write(DATA.super_block[0].n);
            for (int j = 0; j < 50; j++)
            {
                bw.Write(DATA.super_block[0].free[j]);
            }

            for (i = 0; i < 640; i++)
            {
                bw.Write(DATA.i_node[i].file_style);
                bw.Write(DATA.i_node[i].file_length);
                for (int j = 0; j < 100; j++)
                {
                    bw.Write(DATA.i_node[i].file_address[j]);
                }
                bw.Write(DATA.i_node[i].limit);
                bw.Write(DATA.i_node[i].file_UserId);
            }

            for (i = 0; i < 640; i++)
            {
                bw.Write(DATA.root[i].file_name);
                bw.Write(DATA.root[i].i_num);
                bw.Write(DATA.root[i].dir_name);
                bw.Write(DATA.root[i].file_style);
            }
            fs.Close();
        }

        /// <summary>
        /// 从系统文件中读取信息
        /// </summary>
        unsafe void read_file() 
        {
            int i;
            FileStream fs = new FileStream("system.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryReader br = new BinaryReader(fs, Encoding.UTF8);
            for (i = 0; i < 20449; i++)
            {
                DATA.memory[i].n = br.ReadInt32();
                for (int j = 0; j < 50; j++)
                {
                    DATA.memory[i].free[j] = br.ReadInt32();
                }
                DATA.memory[i].a = br.ReadInt32();
                DATA.memory[i].content = br.ReadString();
            }

            DATA.super_block[0].n = br.ReadInt32();
            for (int j = 0; j < 50; j++)
            {
                DATA.super_block[0].free[j] = br.ReadInt32();
            }

            for (i = 0; i < 640; i++)
            {
                DATA.i_node[i].file_style = br.ReadInt32();
                DATA.i_node[i].file_length = br.ReadInt32();
                for (int j = 0; j < 100; j++)
                {
                    DATA.i_node[i].file_address[j] = br.ReadInt32();
                }
                DATA.i_node[i].limit = br.ReadInt32();
                DATA.i_node[i].file_UserId = br.ReadInt32();
            }

            for (i = 0; i < 640; i++)
            {
                DATA.root[i].file_name = br.ReadString();
                DATA.root[i].i_num = br.ReadInt32();
                DATA.root[i].dir_name = br.ReadString();
                DATA.root[i].file_style = br.ReadInt32();
            }
            fs.Close();
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        int open(string filename)
        {
            int i;
            for (i = 0; i < 640; i++)
            {
                if ((DATA.root[i].file_name == filename) && (DATA.i_node[DATA.root[i].i_num].file_style == 1) && (DATA.i_node[DATA.root[i].i_num].file_UserId == ID.login_userid))
                {
                    if (DATA.file_array_head < 8)
                    {
                        DATA.file_array[DATA.file_array_head] = DATA.root[i].i_num;
                        DATA.file_array_head++;
                    }
                    else
                    {
                        textbox1.Text += "打开的文件已达上限，无法打开本文件\n";
                    }
                    return DATA.root[i].i_num;
                }
            }
            if (i == 640)
            {
                textbox1.Text += "您要打开的文件不存在或不属于该用户\n";
            }
            return 0;
        }

        /// <summary>
        /// 关闭文件
        /// </summary>
        /// <param name="filename"></param>
        void close(string filename)
        {
            int i;
            for (i = 0; i < 640; i++)
            {
                if ((DATA.root[i].file_name == filename) && (DATA.i_node[DATA.root[i].i_num].file_style == 1) && (DATA.i_node[DATA.root[i].i_num].file_UserId == ID.login_userid))
                {
                    int j;
                    for ( j = 0; j < DATA.file_array_head; j++)
                    {
                        if (DATA.root[i].i_num == DATA.file_array[j])
                        {
                            int m;
                            for (m = j; m < DATA.file_array_head; m++)
                            {
                                DATA.file_array[m] = DATA.file_array[m + 1];
                            }
                            DATA.file_array_head--;
                            return;
                        }
                    }
                    if (j == DATA.file_array_head)
                    {
                        textbox1.Text += "您要关闭的文件未打开过！\n";
                    }
                    return;
                }
            }
            if (i == 640)
            {
                textbox1.Text += "您要关闭的文件不存在或不属于该用户\n";
            }
            return;
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="filename"></param>
        void create_dir(string filename)
        {
            DATA.style = 0;         //0代表文件类型是目录文件
            create_file(filename, 4, ID.login_userid, -1);
            DATA.style = 1;         //用完恢复初值，因为全局变量，否则error
        }

        /// <summary>
        /// 删除目录   需要判断目录下时候为空,不为空就不删除
        /// </summary>
        /// <param name="filename"></param>
        void del_dir(string filename)
        {
            int i, j, k;
            for (i = 0; i < 640; i++)       //还要加条件判断要删除的目录是不是当前目录
            {
                k = DATA.root[i].i_num;      //找到目录名字
                if ((DATA.root[i].file_name == filename) && (DATA.cur_dir != filename) && (DATA.i_node[k].file_style == 0)) ;
                {
                    for (j = 0; j < 640; j++)
                    {
                        if (filename == DATA.root[j].dir_name)
                        {
                            textbox1.Text += "目录不为空不能直接删除\n";
                            break;
                        }
                    }
                    if (j == 640 || DATA.i_node[DATA.root[i].i_num].file_UserId == ID.login_userid)
                    {
                        del_file(filename);
                        break;
                    }
                    break;
                }
            }
            if (i == 640)
            {
                textbox1.Text += "这个不是目录文件 或者已登录用户不存在这个目录,或者你要删除的是当前目录\n";
            }
        }

        /// <summary>
        /// 显示当前目录下的文件列表
        /// </summary>
        void display_curdir()
        {
            int i, k;
            textbox1.Text += "用户名：";
            textbox1.Text += ID.Username[ID.login_userid].ToString();
            textbox1.Text += "\n";
            textbox1.Text += "\t\t文件名字  文件类型  文件长度  所属目录\n";
            for (i = 0; i < 640; i++)
            {
                if ((DATA.cur_dir == DATA.root[i].dir_name) && (DATA.i_node[DATA.root[i].i_num].file_UserId == ID.login_userid))   //查询文件中 所在目录信息和当前目录信息相同的数据
                {
                    k = DATA.root[i].i_num;
                    textbox1.Text += "\t\t  ";
                    textbox1.Text += DATA.root[i].file_name.ToString();  //文件名
                    textbox1.Text += "\t";
                    if (DATA.root[i].file_style == 2)
                    {
                        textbox1.Text += "\t";
                        textbox1.Text += DATA.root[i].file_style.ToString();  //文件的类型(快捷方式)
                        textbox1.Text += "\t";
                    }
                    else
                    {
                        textbox1.Text += "\t";
                        textbox1.Text += DATA.i_node[k].file_style.ToString();  //文件的类型
                        textbox1.Text += "\t";
                    }
                    textbox1.Text += DATA.i_node[k].file_length.ToString();  //文件的长度
                    textbox1.Text += "\t";
                    textbox1.Text += DATA.root[i].dir_name;  //文件所在的目录
                    textbox1.Text += "\n";   
                }
            }
        }

        /// <summary>
        /// 进入指定的目录
        /// </summary>
        /// <param name="filename"></param>
        void display_dir(string filename)
        {
            int i, k;
            for (i = 0; i < 640; i++)
            {
                k = DATA.root[i].i_num;    //printf("i_node[%d].file_UserId %d,login_userid %d",k,i_node[k].file_UserId,login_userid)  ; //判断文件类型是不是目录类型
                if ((filename == DATA.root[i].file_name) && (DATA.i_node[k].file_style == 0) && (DATA.i_node[k].file_UserId == ID.login_userid))
                {//printf("yes\n");
                    DATA.cur_dir = filename; //printf("cur_dir= %s\n",cur_dir);   //将要进入的指定目录设置为当前目录  赋值不要反了strcpy(目的，源)
                    return;
                }
            }
            if (i == 640)
            {
                textbox1.Text += "登录用户没有这个目录\n";
            }
        }

        /// <summary>
        /// 返回上一级目录
        /// </summary>
        void back_dir()
        {
            int i, k;
            for (i = 0; i < 640; i++)       //查询和当前目录名相同的目录文件名
            {
                k = DATA.root[i].i_num;
                if ((DATA.cur_dir == DATA.root[i].file_name) && (DATA.i_node[k].file_style == 0))
                {
                    DATA.cur_dir = DATA.root[i].dir_name; //将查询到的目录文件名  所在的目录赋值给当前目录
                }
            }
        }

        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        int read(string filename)
        {
            int i;
            for (i = 0; i < 640; i++)
            {
                if ((DATA.root[i].file_name == filename) && (DATA.i_node[DATA.root[i].i_num].file_style == 1))
                {
                    int j;
                    for (j = 0; j < 8; j++)
                    {          //int n; for (n=0;n<8;n++) printf("%d\n",file_array[n]);
                        if (DATA.root[i].i_num == DATA.file_array[j])
                        {
                            if (DATA.i_node[DATA.root[i].i_num].limit == 0 || DATA.i_node[DATA.root[i].i_num].limit == 1)
                            {
                                textbox1.Text += "\n  文件内容：";
                                for (int add = 0; add < DATA.i_node[DATA.root[i].i_num].file_length; add++)
                                {
                                    textbox1.Text += DATA.memory[DATA.i_node[DATA.root[i].i_num].file_address[add]].content.ToString();
                                }
                                textbox1.Text += "\n ";
                            }
                            else
                            {
                                textbox1.Text += "你没有权限读取文件内容！！\n";
                            }
                            return 0;
                        }
                    }
                    if (j == 8)
                    {
                        textbox1.Text += "\n  该文件未打开，请先打开文件再进行读写操作!!\n";
                    }
                    return 0;
                }
            }
            if (i == 640)
            {
               textbox1.Text += "您要读取的文件不存在\n";
            }
            return 0;
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="writec"></param>
        void write(string filename, string writec)
        {
            int i;
            for (i = 0; i < 640; i++)
            {
                if ((DATA.root[i].file_name == filename) && (DATA.i_node[DATA.root[i].i_num].file_style == 1))
                {
                    int j;  //for(j=0;j<8;j++) printf("%d",file_array[j]);
                    for (j = 0; j < 8; j++)
                    {
                        if (DATA.root[i].i_num == DATA.file_array[j])
                        {
                            if (DATA.i_node[DATA.root[i].i_num].limit == 0 || DATA.i_node[DATA.root[i].i_num].limit == 2)
                            {
                                int add;
                                for (add = 0; add < DATA.i_node[DATA.root[i].i_num].file_length; add++)
                                {
                                    if (DATA.memory[DATA.i_node[DATA.root[i].i_num].file_address[add]].content == "\0")
                                    {
                                        //printf("\n%d  %d\n",add,c);
                                        DATA.memory[DATA.i_node[DATA.root[i].i_num].file_address[add]].content = writec;
                                        // memory[i_node[root[i].i_num].file_address[add]].content[c]='b';
                                        textbox1.Text += "\n";
                                        textbox1.Text += DATA.memory[DATA.i_node[DATA.root[i].i_num].file_address[add]].content.ToString();
                                        textbox1.Text += "已写入文件末尾！\n";
                                        return;
                                    }
                                }
                                if (add == DATA.i_node[DATA.root[i].i_num].file_length)
                                {
                                    textbox1.Text += "\n文件空间已满，写入失败！！\n";
                                }
                                return;
                            }
                            else
                            {
                                textbox1.Text += "你没有权限将内容写入文件！！\n";
                                return;
                            }
                        }
                    }
                    if (j == 8)
                    {
                        textbox1.Text += "\n  该文件未打开，请先打开文件再进行读写操作!!\n";
                    }
                    return;
                }
            }
            if (i == 640)
            {
                textbox1.Text += "您要写入的文件不存在\n";
            }
            return;
        }

        /// <summary>
        /// 改变目录
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="dirf"></param>
        void chdir(string filename, string dirf)  
        {
            int i;
            for (i = 0; i < 640; i++)
            {
                if ((DATA.root[i].file_name == filename) && (DATA.i_node[DATA.root[i].i_num].file_style == 1) && (DATA.i_node[DATA.root[i].i_num].file_UserId == ID.login_userid))
                {
                    int j;
                    for (j = 0; j < 640; j++)
                    {
                        if ((DATA.root[j].file_name == dirf) && (DATA.i_node[DATA.root[j].i_num].file_style == 0) && (DATA.i_node[DATA.root[j].i_num].file_UserId == ID.login_userid))
                        {
                            DATA.root[i].dir_name = dirf;
                            return;
                        }
                    }
                    textbox1.Text += "目录不存在或不属于该用户\n";
                    return;
                }
            }
            if (i == 640)
            {
                textbox1.Text += "文件不存在或不属于该用户\n";
            }
            return;
        }

        /// <summary>
        /// 改变权限
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="limf"></param>
        void chlim(string filename, int limf)  
        {
            if (limf < 0 || limf > 3)
            {
                textbox1.Text += "输入权限错误！\n";
                return;
            }
            int i;
            for (i = 0; i < 640; i++)
            {
                if ((DATA.root[i].file_name == filename) && (DATA.i_node[DATA.root[i].i_num].file_style == 1) && (DATA.i_node[DATA.root[i].i_num].file_UserId == ID.login_userid))
                {
                    DATA.i_node[DATA.root[i].i_num].limit = limf;
                    //i_node[i].limit = limf;
                    return;
                }
            }
            if (i == 640)
            {
                textbox1.Text += "文件不存在或不属于该用户\n";
            }
            return;
        }

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="namf"></param>
        void chnam(string filename, string namf)
        {
            int i;
            for (i = 0; i < 640; i++)
            {
                if ((DATA.root[i].file_name == filename) && (DATA.i_node[DATA.root[i].i_num].file_style == 1) && (DATA.i_node[DATA.root[i].i_num].file_UserId == ID.login_userid))
                {
                    int j;
                    for (j = 0; j < 640; j++)
                    {
                        if ((DATA.root[j].file_name == namf) && (DATA.i_node[DATA.root[j].i_num].file_style == 1) && (DATA.i_node[DATA.root[j].i_num].file_UserId == ID.login_userid))
                        {
                            textbox1.Text += "文件名已存在\n";
                            return;
                        }
                    }
                    DATA.root[i].file_name = namf;
                    return;
                }
            }
            if (i == 640)
            {
                textbox1.Text += "文件不存在或不属于该用户\n";
            }
            return;
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="namf"></param>
        void cp(string filename, string namf)
        {
            int i;
            for (i = 0; i < 640; i++)
            {
                if ((DATA.root[i].file_name == filename) && (DATA.i_node[DATA.root[i].i_num].file_style == 1) && (DATA.i_node[DATA.root[i].i_num].file_UserId == ID.login_userid))
                {
                    int j;
                    for (j = 0; j < 640; j++)
                    {
                        if ((DATA.root[j].file_name == namf) && (DATA.i_node[DATA.root[j].i_num].file_style == 1) && (DATA.i_node[DATA.root[j].i_num].file_UserId == ID.login_userid))
                        {
                            textbox1.Text += "文件名已存在\n";
                            return;
                        }
                    }

                    if (DATA.root[i].file_style == 2)
                    {
                        clin(filename, namf);
                    }
                    else
                    {
                        create_file(namf, DATA.i_node[DATA.root[i].i_num].file_length, ID.login_userid, DATA.i_node[DATA.root[i].i_num].limit);
                    }

                    for (j = 0; j < 640; j++)
                    {
                        if (DATA.root[j].file_name == namf)
                        {
                            for (int add = 0; add < DATA.i_node[DATA.root[i].i_num].file_length; add++)
                            {
                                for (int c = 0; c < 1000; c++)
                                {
                                    DATA.memory[DATA.i_node[DATA.root[j].i_num].file_address[add]].content = DATA.memory[DATA.i_node[DATA.root[i].i_num].file_address[add]].content;
                                }
                            }
                        }
                    }
                    return;
                }
            }
            if (i == 640)
            {
                textbox1.Text += "文件不存在或不属于该用户\n";
            }
            return;
        }

        /// <summary>
        /// 显示帮助信息
        /// </summary>
        void help()
        {
            textbox1.Text += "用户名：user";
            textbox1.Text += ID.login_userid.ToString();
            textbox1.Text += "\n";
            textbox1.Text += "注意：创建的文件长度 < 100\n\n"; //说明文件
            textbox1.Text += "0.初始化-------------------------format\n";
            textbox1.Text += "1.查看当前目录文件列表-----------dir\n";
            textbox1.Text += "2.查看文件信息-----------------------cat-----(cat + 空格 + 文件名)  \n";
            textbox1.Text += "3.查看系统信息-------------------ls    \n";
            textbox1.Text += "4.创建目录-----------------------md------(md  + 空格 + 目录名)  \n";
            textbox1.Text += "5.创建文件-----------------------vi-----(vi  + 文件名 + 文件长度 + 权限)\n";
            textbox1.Text += "6.删除文件----------------------del-----(del + 空格 + 文件名) \n";
            textbox1.Text += "7.打开文件----------------------open----(open + 空格 + 文件名) \n";
            textbox1.Text += "8.关闭文件----------------------close---(close + 空格 + 文件名) \n";
            textbox1.Text += "9.读取文件----------------------read----(read + 空格 + 文件名) \n";
            textbox1.Text += "10.写入文件---------------------write--(write + 空格 + 文件名+ 空格 + 写入字符)\n";
            textbox1.Text += "11.改变目录---------------------chdir--(chdir + 空格 + 文件名+ 空格 + 目录)\n";
            textbox1.Text += "12.改变权限---------------------chlim--(chlim + 空格 + 文件名+ 空格 + 权限)\n";
            textbox1.Text += "13.删除目录----------------------deldir--(deldir + 空格 + 目录名)\n";
            textbox1.Text += "14.进入当前目录下的指定目录-------cd--------(cd + 空格 + 目录名)\n";
            textbox1.Text += "15.返回上一级目录-----------------cd..  \n";
            textbox1.Text += "16.显示帮助命令-----------------help  \n";
            textbox1.Text += "17.退出文件系统------------------quit  \n";
            textbox1.Text += "18.退出登录-------------------logout  \n";
            textbox1.Text += "19.重命名---------------------chnam--(chnam + 空格 + 文件名+ 空格 + 新文件名)\n";
            textbox1.Text += "20.复制文件---------------------cp--(cp + 空格 + 文件名+ 空格 + 新文件名)\n";
            textbox1.Text += "21.创建快捷方式---------------------clin--(clin + 空格 + 文件名+ 空格 + 新文件名)\n";
        }

        /// <summary>
        /// 创建快捷方式
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="namf"></param>
        void clin(string filename, string namf) 
        {
            int i, j;
            for (i = 0; i < 640; i++)
            {
                if ((DATA.root[i].file_name == filename) && (DATA.i_node[DATA.root[i].i_num].file_style == 1) && (DATA.i_node[DATA.root[i].i_num].file_UserId == ID.login_userid))
                {
                    for (j = 0; j < 640; j++)
                    {
                        if ((DATA.root[j].file_name == namf) && (DATA.i_node[DATA.root[j].i_num].file_style == 1) && (DATA.i_node[DATA.root[j].i_num].file_UserId == ID.login_userid))
                        {
                            textbox1.Text += "文件名已存在\n";
                            return;
                        }
                    }
                    for (j = 0; j < 640; j++)
                    {
                        if (DATA.root[j].i_num == -1)
                        {
                            DATA.root[j].i_num = DATA.root[i].i_num;
                            DATA.root[j].file_name = namf;
                            DATA.root[j].dir_name = DATA.cur_dir;  //把当前目录名 给新建立的文件
                            DATA.root[j].file_style = 2;
                            return;
                        }
                    }
                }
            }
            if (i == 640)
            {
                textbox1.Text += "文件不存在或不属于该用户\n";
            }
            return;
        }
        
        /// <summary>
        /// 保证打开界面时main函数运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            main();
        }

        /// <summary>
        /// mainloop循环，是保证点击发送按钮后能够直接调用
        /// </summary>
        void mainloop()
        {
            int i, k,len = 0;
            string filename = " ";
            string tempchange = " ";
            string lim = " ";
            char[] chr = textbox2.Text.ToCharArray();
            char[] chname = new char[chr.Length];
            char[] chos = new char[chname.Length];
            char[] chok = new char[chos.Length];
            for (i = 0; i < chr.Length; i++)
            {
                if (chr[i] == ' ')
                {
                    i++;
                    for (k = i; k < chr.Length; k++)
                    {
                        chname[k - i] = chr[k];
                        chr[k] = ' ';
                    }
                    break;
                }
            }

            if (chname[0] != ' ')
            {
                for (i = 0; i < chname.Length; i++)
                {
                    if (chname[i] == ' ')
                    {
                        i++;
                        for (k = i; k < chname.Length; k++)
                        {
                            chos[k - i] = chname[k];
                            chname[k] = ' ';
                        }
                        break;
                    }
                }
            }

            if (chos[0] != ' ')
            {
                for (i = 0; i < chos.Length; i++)
                {
                    if (chos[i] == ' ')
                    {
                        i++;
                        for (k = i; k < chos.Length; k++)
                        {
                            chok[k - i] = chos[k];
                            chos[k] = ' ';
                        }
                        break;
                    }
                }
            }

            DATA.com = new string(chr);
            filename = new string(chname);
            tempchange = new string(chos);
            lim = new string(chok);
            DATA.com = DATA.com.Trim(' ');
            filename = filename.Trim(' ');
            tempchange = tempchange.Trim(' ');
            DATA.com = DATA.com.Trim('\0');
            filename = filename.Trim('\0');
            tempchange = tempchange.Trim('\0');
            lim = lim.Trim('\0');

            for (i = 0; i < 22; i++)
            {
                if (DATA.com == DATA.cmd[i].com)
                {
                    DATA.p = i;
                    break;
                }
            }
            if (i >= 22)         //如果没有这个语句以后输入的命令都和第一次输入的效果一样
            {
                DATA.p = -1; //随便的一个值
            }

            switch (DATA.p)
            {
                case 0:
                    format();       //初始化
                    break;
                case 1:
                    display_curdir();     //查看当前目录下的文件列表
                    break;
                case 2:
                    DATA.tmp = filename;     //查看文件
                    show_file(DATA.tmp);
                    break;
                case 3:
                    display_sys();      //查看系统信息
                    break;
                case 4:
                    DATA.tmp = filename;      //创建目录
                    create_dir(DATA.tmp);
                    break;
                case 5:
                    DATA.tmp = filename;     //创建文件
                    len = Convert.ToInt32(tempchange);
                    int limit;
                    limit = Convert.ToInt32(lim);
                    create_file(DATA.tmp, len, ID.login_userid, limit);
                    break;
                case 6:
                    DATA.tmp = filename;     //删除文件
                    for (i = 0; i < 640; i++)     //判断文件是不是正规文件
                    {
                        DATA.j = DATA.root[i].i_num;
                        if ((DATA.tmp == DATA.root[i].file_name) && (DATA.i_node[DATA.j].file_style == 1))
                        {
                            del_file(DATA.tmp);
                            break;
                        }
                    }
                    if (i == 640)
                    {
                        textbox1.Text += "这个不是正规文件文件\n";
                    }
                    break;
                case 7:
                    DATA.tmp = filename;     //删除目录
                    del_dir(DATA.tmp);
                    break;
                case 8:
                    DATA.tmp1 = filename;     //进入当前目录下的指定目录   相当于进入目录  cd  +  目录名
                    display_dir(DATA.tmp1);
                    break;
                case 9:
                    back_dir();       //返回上一级目录
                    break;
                case 10:
                    help();
                    break;
                case 11:
                    write_file();      //将磁盘利用信息写进系统文件，退出
                    Close();
                    break;
                case 12:
                    DATA.tmp = filename;    //打开文件
                    open(DATA.tmp);
                    break;
                case 13:
                    DATA.tmp = filename;    //读文件
                    read(DATA.tmp);
                    break;
                case 14:
                    DATA.tmp = filename;    //写文件
                    string writec;
                    writec = tempchange;
                    // printf("tmp=%s writec=%c\n",tmp,writec);
                    write(DATA.tmp, writec);
                    // printf("return here?");
                    break;
                case 15:
                    write_file();    //退出登录
                    Close();//关闭页面 
                    Window1 mw = new Window1();
                    mw.ShowDialog();//返回登陆页面框
                    break;
                case 16:
                    DATA.tmp = filename;    //关闭文件
                    close(DATA.tmp);
                    break;
                case 17:
                    DATA.tmp = filename;    //改变目录
                    string dirf;
                    dirf = tempchange;
                    chdir(DATA.tmp, dirf);
                    break;
                case 18:
                    DATA.tmp = filename;    //改变权限
                    int limf;
                    limf = Convert.ToInt32(tempchange);
                    chlim(DATA.tmp, limf);
                    break;
                case 19:
                    DATA.tmp = filename;    //重命名
                    string namf;
                    namf = tempchange;
                    chnam(DATA.tmp, namf);
                    break;
                case 20:
                    DATA.tmp = filename;    //复制文件
                    string fnam;
                    fnam = tempchange;
                    cp(DATA.tmp, fnam);
                    break;
                case 21:
                    DATA.tmp = filename;    //创建快捷方式
                    string cnam;
                    cnam = tempchange;
                    clin(DATA.tmp, cnam);
                    break;

                default:
                    textbox1.Text += "SORRY,没有这个命令\n";
                    break;
            }
            DATA.j = 0;         //必须重新给恢复0否则出错
            DATA.tmp = DATA.cur_dir;

            while (DATA.tmp != "filsystem")
            {
                for (i = 0; i < 640; i++)
                {
                    DATA.p = DATA.root[i].i_num;
                    if ((DATA.tmp == DATA.root[i].file_name) && (DATA.i_node[DATA.p].file_style == 0))
                    {
                        DATA.tmp2[DATA.j].com = DATA.tmp;
                        DATA.j++;
                        DATA.tmp = DATA.root[i].dir_name;
                    }
                }
            }
            DATA.tmp2.Add(new command());
            DATA.tmp2[DATA.j].com = DATA.tmp;
            for (i = DATA.j; i >= 0; i--)
            {
                textbox1.Text += DATA.tmp2[i].com.ToString();
                textbox1.Text += "/";
            }
        }
    }
}

