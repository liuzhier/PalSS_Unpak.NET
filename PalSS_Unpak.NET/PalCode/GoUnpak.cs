using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Controls;

using BOOL        = System.Boolean;
using CHAR        = System.Char;
using BYTE        = System.Byte;
using SHORT       = System.Int16;
using WORD        = System.UInt16;
using INT         = System.Int32;
using DWORD       = System.UInt32;
using UINT        = System.UInt32;
using LONG        = System.Int64;
using QWORD       = System.UInt64;
using FLOAT       = System.Single;
using LPSTR       = System.String;
using FILE        = System.IO.File;

using static PalssUnpak.System;
using static PalssUnpak.Video;
using static PalssUnpak.Unpak;
using static PalssUnpak.Palette;


namespace PalssUnpak
{
   public unsafe class GoUnpak
   {
      public static void
      Unpak_Main()
      {
         Palette.Init();

         //
         // 这边不知道为啥，先解包 FIRE 再解包 RNG 会报错
         // 先解包 RNG 就没事。。。。。。
         //
         Unpak_RGM();
         Unpak_BALL();
         Unpak_J(FILE_NAME_ABC, OUTPUT_PATH_ABC, 2);
         Unpak_J(FILE_NAME_FIGHT, OUTPUT_PATH_FIGHT, 2);
         Unpak_J(FILE_NAME_MGO, OUTPUT_PATH_MGO, 1);
         Unpak_FBP();
         Unpak_FIRE();
         //Unpak_RNG();
      }

      private static void
      Unpak_RGM()
      {
         BYTE[]         dataRGM;
         BYTE*          buffer;
         Surface        dest;
         INT            i, j, iRleCount, iRleSize;

         //
         // 初始化 Surface，使用第 3 个 Palette
         //
         dest           = new Surface();
         dest.palette   = Palette.palettes[3];

         //
         // 读取角色肖像文件数据
         //
         dataRGM        = FILE.ReadAllBytes($@"{GAME_PATH}\{FILE_NAME_RGM}");

         iRleCount      = dataRGM.Length / BUFFER_SIZE_JOE;

         fixed (BYTE* lpRGM = dataRGM)
         {
            for (i = 0, buffer = lpRGM; i < iRleCount; i++)
            {
               //
               // 获取图像宽高，并转小端序
               //
               dest.w   = ((WORD*)buffer)[0];
               dest.h   = ((WORD*)buffer)[1];
               dest.w   = Util.ReverseBit(dest.w);
               dest.h   = Util.ReverseBit(dest.h);

               //
               // 宽高为 0，说明为空图像，直接跳过
               //
               if (dest.w == 0 || dest.h == 0) goto end_unpak;

               //
               // 计算 Rle 块解码后的长度
               //
               iRleSize = dest.w * dest.h;

               //
               // 初始化像素矩阵
               //
               dest.pixels = (BYTE*)Marshal.AllocHGlobal(iRleSize);
               for (j = 0; j < iRleSize; j++) dest.pixels[j] = 0xFF;

               //
               // 解码 Rle 块
               //
               Unpak.RLEBlitToSurface(buffer, ref dest, PAL_XY(0, 0));

               //
               // 复制解码后的内存缓冲区到数组
               //
               dest.pixels_arr = new BYTE[iRleSize];
               for (j = 0; j < dest.pixels_arr.Length; j++) dest.pixels_arr[j] = dest.pixels[j];

               //
               // 将像素矩阵导出为图像
               //
               dest.SaveAsPng($@"{GAME_PATH}\{OUTPUT_PATH}\{OUTPUT_PATH_RGM}", $@"{OUTPUT_PATH_RGM}-{i}.png");

end_unpak:
               buffer += BUFFER_SIZE_JOE;

               //
               // 释放像素矩阵占用的非托管内存
               //
               if (dest.pixels != null)
               {
                  Marshal.FreeHGlobal((IntPtr)dest.pixels);
                  dest.pixels = null;
               }
            }
         }
      }

      private static void
      Unpak_BALL()
      {
         BYTE[]         dataBall;
         BYTE*          buffer;
         Surface        dest;
         INT            iRleSize;
         DWORD          i, j, dwCount_NEW;

         //
         // 初始化 Surface，使用第 3 个 Palette
         //
         dest           = new Surface();
         dest.palette   = Palette.palettes[3];

         //
         // 读取道具图像文件数据
         //
         dataBall       = FILE.ReadAllBytes($@"{GAME_PATH}\{FILE_NAME_BALL}");

         fixed (BYTE* lpBALL = dataBall)
         {
            //
            // 获取 NEW 块数
            //
            dwCount_NEW = NEW_GetCount(lpBALL);

            for (i = 0; i < dwCount_NEW; i++)
            {
               buffer = lpBALL + NEW_GetChunkOffest(lpBALL, i);

               //
               // 获取图像宽高，并转小端序
               //
               dest.w   = ((WORD*)buffer)[0];
               dest.h   = ((WORD*)buffer)[1];
               dest.w   = Util.ReverseBit(dest.w);
               dest.h   = Util.ReverseBit(dest.h);

               //
               // 宽高为 0，说明为空图像，直接跳过
               //
               if (dest.w == 0 || dest.h == 0) goto end_unpak;

               //
               // 计算 Rle 块解码后的长度
               //
               iRleSize = dest.w * dest.h;

               //
               // 初始化像素矩阵
               //
               dest.pixels = (BYTE*)Marshal.AllocHGlobal(iRleSize);
               for (j = 0; j < iRleSize; j++) dest.pixels[j] = 0xFF;

               //
               // 解码 Rle 块
               //
               Unpak.RLEBlitToSurface(buffer, ref dest, PAL_XY(0, 0));

               //
               // 复制解码后的内存缓冲区到数组
               //
               dest.pixels_arr = new BYTE[iRleSize];
               for (j = 0; j < dest.pixels_arr.Length; j++) dest.pixels_arr[j] = dest.pixels[j];

               //
               // 将像素矩阵导出为图像
               //
               dest.SaveAsPng($@"{GAME_PATH}\{OUTPUT_PATH}\{OUTPUT_PATH_BALL}", $@"{OUTPUT_PATH_BALL}-{i}.png");

end_unpak:
               //
               // 释放像素矩阵占用的非托管内存
               //
               if (dest.pixels != null)
               {
                  Marshal.FreeHGlobal((IntPtr)dest.pixels);
                  dest.pixels = null;
               }
            }
         }
      }

      private static void
      Unpak_J(
         LPSTR    lpszFileName,
         LPSTR    lpszOutputName,
         BYTE     bPaletteID
      )
      {
         BYTE[]         dataJ;
         BYTE*          buffer, buffer_sprite;
         Surface        dest;
         INT            iSize;
         DWORD          i, j;
         WORD           wCount_J, wCount_SPRITE, k;
         WORD*          lpj, lpBuffer;
         DWORD[]        dwOffsets;

         //
         // 初始化 Surface
         //
         dest           = new Surface();
         dest.palette   = Palette.palettes[bPaletteID];

         //
         // 读取 J 文件数据
         //
         dataJ          = FILE.ReadAllBytes($@"{GAME_PATH}\{lpszFileName}");

         fixed (BYTE* lpJ = dataJ)
         {
            lpj = (WORD*)lpJ;

            //
            // 获取图像偏移索引
            //
            dwOffsets = new DWORD[BUFFER_SIZE_CHUNK / sizeof(WORD)];
            for (i = 0; i < dwOffsets.Length; i++)
            {
               dwOffsets[i] = (DWORD)(Util.ReverseBit(lpj[i]) * BUFFER_SIZE_CHUNK);

               if (dwOffsets[i] == 0) break;
            }

            //
            // 获取 J 总块数
            //
            wCount_J = (WORD)i;

            for (i = 0; i < wCount_J; i++)
            {
               buffer         = lpJ + dwOffsets[i];
               lpBuffer       = (WORD*)buffer;

               //
               // 获取 Sprite 总块数
               //
               wCount_SPRITE  = Util.ReverseBit(lpBuffer[0]);

               buffer        += sizeof(WORD);
               lpBuffer       = (WORD*)buffer;
               buffer_sprite  = buffer + wCount_SPRITE * sizeof(WORD) * 2;

               for (k = 0; k < wCount_SPRITE; k++)
               {
                  //
                  // 获取图像宽高
                  //
                  dest.w   = Util.ReverseBit(lpBuffer[0]);
                  dest.h   = Util.ReverseBit(lpBuffer[1]);
                  iSize    = dest.w * dest.h;

                  if (iSize <= 0) break;

                  //
                  // 复制像素矩阵到数组
                  //
                  dest.pixels_arr = new BYTE[iSize];
                  for (j = 0; j < dest.pixels_arr.Length; j++) dest.pixels_arr[j] = 0xFF;
                  for (j = 0; j < dest.pixels_arr.Length; j++) dest.pixels_arr[j] = buffer_sprite[j];

                  //
                  // 将像素矩阵导出为图像
                  //
                  dest.SaveAsPng($@"{GAME_PATH}\{OUTPUT_PATH}\{lpszOutputName}", $@"{lpszOutputName}-{i}-{k}.png", 0);

                  buffer_sprite  += iSize;
                  lpBuffer       += 2;
               }
            }
         }
      }

      private static void
      Unpak_FBP()
      {
         BYTE[]         dataFbp;
         Surface        dest;
         DWORD          i, j;
         WORD           wCount_FBP;
         BYTE*          lpFbp;

         //
         // 初始化 Surface
         //
         dest              = new Surface();
         dest.w            = VIDEO_WIDTH;
         dest.h            = VIDEO_HEIGHT;
         dest.pixels_arr   = new BYTE[BUFFER_SIZE_FBP];

         //
         // 读取战斗背景文件数据
         //
         dataFbp        = FILE.ReadAllBytes($@"{GAME_PATH}\{FILE_NAME_FBP}");

         fixed (BYTE* lpFBP = dataFbp)
         {
            lpFbp = lpFBP;

            //
            // 获取图像总数
            //
            wCount_FBP = (WORD)(dataFbp.Length / BUFFER_SIZE_FBP);

            for (i = 0; i < wCount_FBP; i++)
            {
               //
               // 获取像素矩阵
               //
               for (j = 0; j < BUFFER_SIZE_FBP; j++)
               {
                  dest.pixels_arr[j] = lpFbp[j];
               }

               //
               // 设置调色板
               //
               switch(i)
               {
                  case 0:
                  case 1:
                  case 3:
                  case 4:
                  case 5:
                     dest.palette = Palette.palettes[3];
                     break;

                  case 2:
                     dest.palette = Palette.palettes[1];
                     break;

                  default:
                     dest.palette = Palette.palettes[0];
                     break;
               }

               //
               // 将像素矩阵导出为图像
               //
               dest.SaveAsPng($@"{GAME_PATH}\{OUTPUT_PATH}\{OUTPUT_PATH_FBP}", $@"{OUTPUT_PATH_FBP}-{i}.png", 0);

               lpFbp += BUFFER_SIZE_FBP;
            }
         }
      }

      private static void
      Unpak_FIRE()
      {
         BYTE[]         dataFire;
         BYTE*          buffer, buffer_sprite;
         Surface        dest;
         INT            pat_color;
         DWORD          i, j, k;
         WORD           wCount_Fire, wCount_SPRITE;
         WORD*          lpFire, lpwBuffer;
         DWORD*         lpdwBuffer;
         DWORD[]        dwOffsets, dwOffsets_SPRITE;
         Color          color;

         //
         // 初始化 Surface
         //
         dest              = new Surface();
         dest.w            = VIDEO_WIDTH;
         dest.h            = VIDEO_HEIGHT;
         dest.pixels_arr   = new BYTE[BUFFER_SIZE_J];

         //
         // 读取 AV 文件数据
         //
         dataFire    = FILE.ReadAllBytes($@"{GAME_PATH}\{FILE_NAME_FIRE}");

         fixed (BYTE* lpFIRE = dataFire)
         {
            lpFire = (WORD*)lpFIRE;

            //
            // 获取图像偏移索引
            //
            dwOffsets = new DWORD[BUFFER_SIZE_CHUNK / sizeof(WORD)];
            for (i = 0; i < dwOffsets.Length; i++)
            {
               dwOffsets[i] = (DWORD)(Util.ReverseBit(lpFire[i]) * BUFFER_SIZE_CHUNK);

               if (dwOffsets[i] == 0) break;
            }

            //
            // 获取 AV 总块数
            //
            wCount_Fire = (WORD)i;

            for (i = 0; i < wCount_Fire - 1; i++)
            {
               buffer      = lpFIRE + dwOffsets[i];
               lpwBuffer   = (WORD*)buffer;

               //
               // 获取调色板
               //
               dest.palette   = new List<Color>();

               for (j = 0; j < PALETTE_MAX_COUNT; j++)
               {
                  pat_color = Util.ReverseBit(lpwBuffer[j]);

                  color    = new Color();
                  color.B  = (BYTE)((pat_color >> 10 & 0x001F) << 3);
                  color.G  = (BYTE)((pat_color >> 5 & 0x001F) << 3);
                  color.R  = (BYTE)((pat_color & 0x001F) << 3);
                  color.A  = (BYTE)((j == 0xFF) ? 0 : 0xFF);

                  dest.palette.Add(color);
               }
               
               buffer      = buffer + PALETTE_SIZE;
               lpwBuffer   = (WORD*)buffer;

               //
               // 获取帧总数
               //
               lpdwBuffer     = (DWORD*)buffer;
               wCount_SPRITE  = (WORD)(Util.ReverseBit(lpdwBuffer[0]) / sizeof(DWORD));

               //
               // 获取帧偏移
               //
               dwOffsets_SPRITE  = new DWORD[wCount_SPRITE];

               for (j = 0; j < wCount_SPRITE - 1; j++)
               {
                  dwOffsets_SPRITE[j] = Util.ReverseBit(lpdwBuffer[j]);
               }

               //
               // 对每一帧进行解码
               //
               for (j = 0; j < wCount_SPRITE - 1; j++)
               {
                  if (dwOffsets[j] >= 0x20000000)
                  {
                     //
                     // 未编码的块，因为对比上一帧，重复内容并不多
                     // 仅做特殊处理，实际不可能执行到这里
                     //
                     dwOffsets_SPRITE[j]  &= 0x7fffffff;
                     buffer_sprite  = buffer + dwOffsets_SPRITE[j];

                     //
                     // 直接读取 320 * 200 像素矩阵
                     //
                     for (k = 0; k < BUFFER_SIZE_J; k++)
                     {
                        dest.pixels_arr[k] = buffer_sprite[k];
                     }
                  }
                  else
                  {
                     //
                     // 编码的块，对比上一帧，重复内容足够多
                     //
                     dwOffsets_SPRITE[j] &= 0x0fffffff;
                     buffer_sprite = buffer + dwOffsets_SPRITE[j];

                     fixed (BYTE* lpPixelsArr = dest.pixels_arr)
                     {
                        Unpak.Decode_RNG(buffer_sprite, lpPixelsArr);
                     }
                  }

                  //
                  // 将像素矩阵导出为图像
                  //
                  dest.SaveAsPng($@"{GAME_PATH}\{OUTPUT_PATH}\{OUTPUT_PATH_FIRE}", $@"{OUTPUT_PATH_FIRE}-{i}-{j}.png");
               }
            }
         }
      }

      private static void
      Unpak_RNG()
      {
         BYTE[]         dataRng, dataPat;
         BYTE*          buffer, buffer_sprite;
         Surface        dest;
         INT            pat_color;
         DWORD          i, j, k;
         WORD           wCount_Rng, wCount_SPRITE;
         WORD*          lpRng, lpPat, lpwBuffer;
         DWORD*         lpdwBuffer;
         DWORD[]        dwOffsets, dwOffsets_SPRITE;
         Color          color;

         //
         // 初始化 Surface
         //
         dest              = new Surface();
         dest.w            = VIDEO_WIDTH;
         dest.h            = VIDEO_HEIGHT;
         dest.pixels_arr   = new BYTE[BUFFER_SIZE_J];

         //
         // 读取 AV 文件数据
         //
         dataRng    = FILE.ReadAllBytes($@"{GAME_PATH}\{FILE_NAME_RNG}");

         //
         // 读取 AV 调色板文件数据
         //
         dataPat    = FILE.ReadAllBytes($@"{GAME_PATH}\{PALETTE_RNG_PATH}");

         fixed (BYTE* lpRNG = dataRng, lpPAT = dataPat)
         {
            lpRng = (WORD*)lpRNG;
            lpPat = (WORD*)lpPAT;

            //
            // 获取图像偏移索引
            //
            dwOffsets = new DWORD[BUFFER_SIZE_CHUNK / sizeof(WORD)];
            for (i = 0; i < dwOffsets.Length; i++)
            {
               dwOffsets[i] = (DWORD)(Util.ReverseBit(lpRng[i]) * BUFFER_SIZE_CHUNK);

               if (dwOffsets[i] == 0) break;
            }

            //
            // 获取 AV 总块数
            //
            wCount_Rng = (WORD)i;

            for (i = 0; i < wCount_Rng; i++)
            {
               buffer      = lpRNG + dwOffsets[i];
               lpwBuffer   = (WORD*)buffer;

               //
               // 获取调色板
               //
               dest.palette = new List<Color>();
               lpPat = (WORD*)(lpPAT + i * PALETTE_SIZE);

               for (k = 0; k < PALETTE_MAX_COUNT; k++)
               {
                  pat_color = Util.ReverseBit(lpPat[k]);

                  color = new Color();
                  color.B = (BYTE)((pat_color >> 10 & 0x001F) << 3);
                  color.G = (BYTE)((pat_color >> 5 & 0x001F) << 3);
                  color.R = (BYTE)((pat_color & 0x001F) << 3);
                  color.A = 0xFF;

                  dest.palette.Add(color);
               }

               //
               // 获取帧总数
               //
               lpdwBuffer     = (DWORD*)buffer;
               wCount_SPRITE  = (WORD)(Util.ReverseBit(lpdwBuffer[0]));

               //
               // 获取帧偏移
               //
               dwOffsets_SPRITE  = new DWORD[wCount_SPRITE];

               for (j = 0; j < dwOffsets_SPRITE.Length - 1; j++)
               {
                  dwOffsets_SPRITE[j] = Util.ReverseBit(lpdwBuffer[j + 1]);
               }

               //
               // 对每一帧进行解码
               //
               for (j = 0; j < wCount_SPRITE - 1; j++)
               {
                  if (dwOffsets_SPRITE[j] >= 0x80000000)
                  {
                     //
                     // 未编码的块，因为对比上一帧，重复内容并不多
                     //
                     dwOffsets_SPRITE[j] &= 0x7fffffff;
                     buffer_sprite = buffer + dwOffsets_SPRITE[j];

                     //
                     // 直接读取 320 * 200 像素矩阵
                     //
                     for (k = 0; k < BUFFER_SIZE_J; k++)
                     {
                        dest.pixels_arr[k] = buffer_sprite[k];
                     }
                  }
                  else
                  {
                     //
                     // 编码的块，对比上一帧，重复内容足够多
                     //
                     buffer_sprite = buffer + dwOffsets_SPRITE[j];

                     fixed (BYTE* lpPixelsArr = dest.pixels_arr)
                     {
                        Unpak.Decode_RNG(buffer_sprite, lpPixelsArr);
                     }
                  }

                  //
                  // 将像素矩阵导出为图像
                  //
                  dest.SaveAsPng(
                     $@"{GAME_PATH}\{OUTPUT_PATH}\{OUTPUT_PATH_RNG}\{OUTPUT_PATH_RNG}-{i}",
                     $@"{OUTPUT_PATH_RNG}-{i}-{j}.png"
                  );
               }
            }
         }
      }
   }
}
