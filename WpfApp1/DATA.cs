using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WpfApp1
{
    /// <summary>
    /// 全局变量定义
    /// </summary>
    public class DATA
    {
        public static int[] file_array = { -1, -1, -1, -1, -1, -1, -1, -1 }; //打开文件数组
        public static int file_array_head;

        public static int[] physic = new int[100]; //文件地址缓冲区
        public static int style = 1; //文件类型
        public static string cur_dir = "filsystem"; //当前目录

        public static List<command> cmd { get; set; } = new List<command>(); //指令
        public static List<block> memory { get; set; } = new List<block>(); //内存
        public static List<block_super> super_block { get; set; } = new List<block_super>(); //超级块
        public static List<node> i_node { get; set; } = new List<node>();//i结点//每个I节点占32字节
        public static List<dir> root { get; set; } = new List<dir>();//根目录

        public static string tmp, com, tmp1;
        public static int p;
        public static int j = 0;
        public static List<command> tmp2 = new List<command>();

    }
}