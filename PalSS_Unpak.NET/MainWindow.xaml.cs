using PalssUnpak;
using System;
using System.Collections.Generic;
using System.IO;
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

using BOOL        = System.Boolean;
using CHAR        = System.Char;
using BYTE        = System.Byte;
using SHORT       = System.Int16;
using WORD        = System.UInt16;
using INT         = System.Int32;
using UINT        = System.UInt32;
using DWORD       = System.UInt32;
using LONG        = System.Int64;
using QWORD       = System.UInt64;
using FLOAT       = System.Single;
using LPSTR       = System.String;
using FILE        = System.IO.File;

using static PalssUnpak.System;
using System.Threading;

namespace PalSS_Unpak.NET
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

      private void Button_Click(object sender, RoutedEventArgs e)
      {
         BOOL     fIsExists = TRUE;

         if (!File.Exists($@"{GAME_PATH}\{FILE_NAME_MAIN}"))
         {
            fIsExists = FALSE;

            PalssUnpak.System.Faild(
               $"PalssUnpak Faild:\n" +
               $"\t找不到程序入口点：\n\n{GAME_PATH}\\{FILE_NAME_MAIN}",
               FALSE
            );
         }

         if (!File.Exists($@"{GAME_PATH}\{FILE_NAME_LIB}"))
         {
            fIsExists = FALSE;

            PalssUnpak.System.Faild(
               $"PalssUnpak Faild:\n" +
               $"\t找不到动态库入口点：\n\n{GAME_PATH}\\{FILE_NAME_LIB}",
               FALSE
            );
         }

         if (fIsExists)
         {
            //
            // 开始解包
            //
            GoUnpak.Unpak_Main();
         }

         Button button = sender as Button;
         button.Content = "完毕．．．";
         button.IsEnabled = FALSE;
      }
   }
}
