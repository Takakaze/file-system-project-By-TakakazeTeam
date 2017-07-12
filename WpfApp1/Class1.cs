using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WpfApp1//定义类内部变量
{
    class Class1
    {

    }
    public class command //指令
    {
        public string com { get; set; } = " ";
    }

    public class block //一个块占1KB
    {
        public int n; //存放空闲盘块的个数
        public int[] free = new int[50]; //存放空闲盘块的地址
        public int a;  //盘块是否被占用标志
        public string content = " ";//块上每个字节存放的符号
    }

    public class block_super
    {
        public int n;  //空闲盘块的个数
        public int[] free = new int[50]; //存放进入栈中的空闲块 
                                         //int stack[50];    //存放下一组空闲盘快的地址
    }

    public class node //i结点信息
    {
        public int file_style; //i结点 文件类型
        public int file_length; //i结点 文件长度
        public int[] file_address = new int[100]; //文件占用的物理块号。
        public int limit; //打开读写权限，0表示能打开读写，1表示能打开读，2表示能打开写，3表示只能打开
        public int file_UserId;
    }

    public class dir //目录项信息
    {
        public string file_name = "";  //文件名
        public int i_num; //文件的结点号
        public string dir_name = "";  //目录名或者说文件所在目录
        public int file_style = 0; //文件格式
    }

    public class ID
    {
        public static int login_userid = -1; //登陆userid 如果该值=0，则Username = “user0”
        public static String[] Username = new string[8] { "user0", "user1", "user2", "user3", "user4", "user5", "user6", "user7" };
        public static String[] Password = new string[8] { "2014", "2015", "2016", "2017", "2018", "2019", "2020", "2021" };
    }
}