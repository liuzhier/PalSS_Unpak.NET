using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

using static PalssUnpak.System;

namespace PalssUnpak
{
   public unsafe class Palette
   {
      public const   LPSTR          PALETTE_PATH            = @"ALLDAT.NEW";
      public const   LPSTR          PALETTE_RNG_PATH        = @"RNG.PAL";
      public const   INT            PALETTE_MAX_COUNT       = 0x100;
      public const   INT            PALETTE_SIZE            = PALETTE_MAX_COUNT * sizeof(WORD);
      public const   INT            PALETTE_SIZE_TOTAL      = 0x1000;
      public const   INT            PALETTE_COUNT           = PALETTE_SIZE_TOTAL / PALETTE_SIZE;

      public static  List<Color>[]  palettes;

      // ALLDAT.NEW: 前 0x1000 字节为 8 个调色板，其中 3、4为人物头像（状态背景）调色板
      public static void
      Init()
      {
         BYTE[]      dataAllDatNew;
         INT         pat_color, i, j;
         WORD*       lpPat;
         Color       color;

         //
         // 读取调色板文件
         //
         dataAllDatNew  = FILE.ReadAllBytes($@"{GAME_PATH}\{PALETTE_PATH}");

         fixed (BYTE* lpPAT = dataAllDatNew)
         {
            //
            // 初始化调色板
            //
            palettes = new List<Color>[PALETTE_COUNT];

            for (i = 0; i < palettes.Length; i++)
            {
               lpPat = (WORD*)lpPAT + PALETTE_MAX_COUNT * i;

               palettes[i]    = new List<Color>();

               for (j = 0; j < PALETTE_MAX_COUNT; j++)
               {
                  pat_color = Util.ReverseBit(lpPat[j]);

                  color    = new Color();
                  color.B  = (BYTE)((pat_color >> 10 & 0x001F) << 3);
                  color.G  = (BYTE)((pat_color >> 5 & 0x001F) << 3);
                  color.R  = (BYTE)((pat_color & 0x001F) << 3);
                  color.A  = 0xFF;

                  palettes[i].Add(color);
               }
            }
         }
      }
   }
}
