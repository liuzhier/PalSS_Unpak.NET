using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.IO;
using System.Security.Policy;
using System.Windows.Media.Imaging;
using System.Windows.Media;

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

using PAL_POS     = System.UInt32;
using PAL_Rect    = System.Windows.Int32Rect;

using static PalssUnpak.Video;
using PalSS_Unpak.NET;

namespace PalssUnpak
{
   public unsafe class System
   {
#if DEBUG
      public static  LPSTR   GAME_PATH          = @"D:\SWORD\PalResearch\palss\game\PalSS";
#else    // DEBUG
      //public const LPSTR   GAME_PATH            = @".\";
      public static  LPSTR   GAME_PATH          = Environment.CurrentDirectory;
#endif   // DEBUG
      public const LPSTR   OUTPUT_PATH          = @"PalssImages";
      public const LPSTR   OUTPUT_PATH_RGM      = @"Face";
      public const LPSTR   OUTPUT_PATH_BALL     = @"Item";
      public const LPSTR   OUTPUT_PATH_ABC      = @"Enemy";
      public const LPSTR   OUTPUT_PATH_FIGHT    = @"Fight";
      public const LPSTR   OUTPUT_PATH_MGO      = @"Char";
      public const LPSTR   OUTPUT_PATH_FBP      = @"FightBackPic";
      public const LPSTR   OUTPUT_PATH_FIRE     = @"Magic";
      public const LPSTR   OUTPUT_PATH_RNG      = @"Rng";
      
      public const LPSTR   FILE_NAME_MAIN       = @"TRUE.BIN";
      public const LPSTR   FILE_NAME_LIB        = @"PROG.BIN";
      public const LPSTR   FILE_NAME_RGM        = @"RGM.JOE";
      public const LPSTR   FILE_NAME_BALL       = @"BALL.NEW";
      public const LPSTR   FILE_NAME_ABC        = @"ABC.J";
      public const LPSTR   FILE_NAME_FIGHT      = @"FFF.J";
      public const LPSTR   FILE_NAME_MGO        = @"MGO.J";
      public const LPSTR   FILE_NAME_FBP        = @"FBP.PX";
      public const LPSTR   FILE_NAME_FIRE       = @"FIREN.AV";
      public const LPSTR   FILE_NAME_RNG        = @"RNG.AV";
      
      public const INT     VIDEO_WIDTH          = 320;
      public const INT     VIDEO_HEIGHT         = 200;
      
      public const INT     BUFFER_SIZE_CHUNK    = 0x800;
      public const INT     BUFFER_SIZE_JOE      = 0x2800;
      public const INT     BUFFER_SIZE_J        = VIDEO_WIDTH * VIDEO_HEIGHT;
      public const INT     BUFFER_SIZE_FBP      = 0x10000;

      public const BOOL    TRUE                 = true;
      public const BOOL    FALSE                = false;
      public const INT     PAL_ERROR            = 0x7FFFFFFF;

      public static void
      Faild(
         LPSTR    lpsztText,
         BOOL     bExit = TRUE
      )
      {
         MessageBox.Show(lpsztText, $"FAILD");

         if (bExit)
         {
            //
            // 遇到错误，结束程序
            //
            Environment.Exit(0);
         }
      }
   }
}
