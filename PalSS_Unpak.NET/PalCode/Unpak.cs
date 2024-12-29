using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
using static PalssUnpak.System;

namespace PalssUnpak
{
   public unsafe class Unpak
   {

      public static PAL_POS
      PAL_XY(dynamic x, dynamic y) => (PAL_POS)((((WORD)y << 16) & 0xFFFF0000) | ((WORD)x & 0xFFFF));

      public static WORD
      PAL_X(PAL_POS xy) => (WORD)((xy) & 0xFFFF);

      public static WORD
      PAL_Y(PAL_POS xy) => (WORD)(((xy) >> 16) & 0xFFFF);

      public static BYTE
      CalcShadowColor(
         BYTE     bSourceColor
      )
      {
         return (BYTE)((bSourceColor & 0xF0) | ((bSourceColor & 0x0F) >> 1));
      }

      public static INT
      RLEBlitToSurface(
            BYTE*       lpBitmapRLE,
      ref   Surface     lpDstSurface,
            PAL_POS     pos
      )
      {
         return RLEBlitToSurfaceWithShadow(lpBitmapRLE, ref lpDstSurface, pos, FALSE);
      }

      public static INT
      RLEBlitToSurfaceWithShadow(
            BYTE*       lpBitmapRLE,
      ref   Surface     surfacelpDest,
            PAL_POS     pos,
            BOOL        bShadow
      )
      {
         UINT        i, j, k, sx;
         INT         x, y;
         UINT        uiLen = 0;
         UINT        uiWidth = 0;
         UINT        uiHeight = 0;
         UINT        uiSrcX = 0;
         BYTE        T;
         INT         dx = PAL_X(pos);
         INT         dy = PAL_Y(pos);
         BYTE*       p;

         fixed (Surface* lpDstSurface = &surfacelpDest)
         {
            //
            // Check for NULL pointer.
            //
            if (lpBitmapRLE == null || lpDstSurface == null)
               return -1;

            //
            // Skip the 0x00000002 in the file header.
            //
            if (lpBitmapRLE[0] == 0x02 && lpBitmapRLE[1] == 0x00 &&
               lpBitmapRLE[2] == 0x00 && lpBitmapRLE[3] == 0x00)
            {
               lpBitmapRLE += 4;
            }

            //
            // Get the width and height of the bitmap.
            //
            uiWidth  = (UINT)(lpBitmapRLE[0] << 8 | (lpBitmapRLE[1]));
            uiHeight = (UINT)(lpBitmapRLE[2] << 8 | (lpBitmapRLE[3]));

            //
            // Check whether bitmap intersects the surface.
            //
            if (uiWidth + dx <= 0 || dx >= lpDstSurface->w ||
                uiHeight + dy <= 0 || dy >= lpDstSurface->h)
            {
               goto end;
            }

            //
            // Calculate the total length of the bitmap.
            // The bitmap is 8-bpp, each pixel will use 1 BYTE.
            //
            uiLen = uiWidth * uiHeight;

            //
            // Start decoding and blitting the bitmap.
            //
            lpBitmapRLE += 4;
            for (i = 0; i < uiLen;)
            {
               T = *lpBitmapRLE++;
               if (((T & 0x80) != 0) && (T <= (0x80 + uiWidth)))
               {
                  i += (UINT)(T - 0x80);
                  uiSrcX += (UINT)(T - 0x80);

                  if (uiSrcX >= uiWidth)
                  {
                     uiSrcX -= uiWidth;
                     dy++;
                  }
               }
               else
               {
                  //
                  // Prepare coordinates.
                  //
                  j = 0;
                  sx = uiSrcX;
                  x = (INT)(dx + uiSrcX);
                  y = dy;

                  //
                  // Skip the points which are out of the surface.
                  //
                  if (y < 0)
                  {
                     j += (UINT)(-y * uiWidth);
                     y = 0;
                  }
                  else if (y >= lpDstSurface->h)
                  {
                     goto end; // No more pixels needed, break out
                  }

                  while (j < T)
                  {
                     //
                     // Skip the points which are out of the surface.
                     //
                     if (x < 0)
                     {
                        j -= (UINT)x;

                        if (j >= T) break;

                        sx -= (UINT)x;
                        x = 0;
                     }
                     else if (x >= lpDstSurface->w)
                     {
                        j += uiWidth - sx;
                        x = (INT)(x - sx);
                        sx = 0;
                        y++;

                        if (y >= lpDstSurface->h)
                        {
                           goto end; // No more pixels needed, break out
                        }

                        continue;
                     }

                     //
                     // Put the pixels in row onto the surface
                     //
                     k = T - j;

                     if (lpDstSurface->w - x < k) k = (UINT)(lpDstSurface->w - x);
                     if (uiWidth - sx < k) k = uiWidth - sx;

                     sx += k;
                     p = ((BYTE*)lpDstSurface->pixels) + y * lpDstSurface->w;

                     if (bShadow)
                     {
                        j += k;

                        for (; k != 0; k--)
                        {
                           p[x] = CalcShadowColor(p[x]);
                           x++;
                        }
                     }
                     else
                     {
                        for (; k != 0; k--)
                        {
                           p[x] = lpBitmapRLE[j];
                           j++;
                           x++;
                        }
                     }

                     if (sx >= uiWidth)
                     {
                        sx -= uiWidth;
                        x = (INT)(x - uiWidth);
                        y++;

                        if (y >= lpDstSurface->h)
                        {
                           goto end; // No more pixels needed, break out
                        }
                     }
                  }

                  lpBitmapRLE += T;
                  i += T;
                  uiSrcX += T;

                  while (uiSrcX >= uiWidth)
                  {
                     uiSrcX -= uiWidth;
                     dy++;
                  }
               }
            }
         }

      end:
         //
         // Success
         //
         return 0;
      }

      public static void
      Decode_RNG(
         BYTE*    src,
         BYTE*    dest
      )
      {
         int x = 0, y = 0;
         int code;

         for (; ; )
         {
            code = *src++;
            if (code == 0x00)
               break;
            else if (code == 0x10)
            {
               dest += 320;
               x = 0;
               y++;
            }
            else if (code >= 0x20 && code <= 0x2f)
               x += code & 0xf;
            else if (code >= 0x30 && code <= 0x3f)
               x += ((code & 0xf) << 8) | *src++;
            else if (code >= 0x41 && code <= 0x4f)
            {
               code &= 0xf;
               for (int i = 0; i < code; i++)
                  dest[x++] = 0xff;
            }
            else if (code >= 0x50 && code <= 0x5f)
            {
               code = ((code & 0xf) << 8) | *src++;
               for (int i = 0; i < code; i++)
                  dest[x++] = 0xff;
            }
            else if (code >= 0x61 && code <= 0x6f)
            {
               code &= 0xf;
               for (int i = 0; i < code; i++)
                  dest[x++] = *src++;
            }
            else if (code >= 0x70 && code <= 0x7f)
            {
               code = ((code & 0xf) << 8) | *src++;
               for (int i = 0; i < code; i++)
                  dest[x++] = *src++;
            }
            else if (code >= 0x81 && code <= 0x8f)
            {
               BYTE ch = *src++;
               code &= 0xf;
               for (int i = 0; i < code; i++)
                  dest[x++] = ch;
            }
            else if (code >= 0x90 && code <= 0x9f)
            {
               code = ((code & 0xf) << 8) | *src++;
               BYTE ch = *src++;
               for (int i = 0; i < code; i++)
                  dest[x++] = ch;
            }
            else
            {
               System.Faild(
                  $"Decode_RNG Faild:\n" +
                  $"\tunknown code: {code}"
               );
               int offset = y * 320 + x;
               continue;
            }
         }
      }

      public static DWORD
      NEW_GetCount(
         BYTE*    lpNEW
      )
      {
         DWORD*      lpNew;
         DWORD       dwCount;

         lpNew    = (DWORD*)lpNEW;
         dwCount  = Util.ReverseBit(lpNew[0]) / sizeof(DWORD) - 1;

         return dwCount;
      }

      public static DWORD
      NEW_GetChunkOffest(
         BYTE*    lpNEW,
         DWORD    dwChunkID
      )
      {
         DWORD*      lpNew;
         DWORD       dwCount, dwOffest;

         //
         // 获取总块数
         //
         dwCount = NEW_GetCount(lpNEW);

         //
         // 检查目标块是否存在
         //
         if (dwChunkID > dwCount)
         {
            System.Faild(
               $"NEW_GetChunkSize Faild:\n" +
               $"\tMaximum ChunkID: {dwCount}\n" +
               $"\tActual ChunkID: {dwChunkID}"
            );
         }

         lpNew    = (DWORD*)lpNEW;
         dwOffest = Util.ReverseBit(lpNew[dwChunkID]);

         return dwOffest;
      }

      public static DWORD
      NEW_GetChunkSize(
         BYTE*    lpNEW,
         DWORD    dwChunkID
      )
      {
         DWORD       dwOffest, dwOffestNext;

         dwOffest       = NEW_GetChunkOffest(lpNEW, dwChunkID);
         dwOffestNext   = NEW_GetChunkOffest(lpNEW, dwChunkID + 1);
         dwOffest       = Util.ReverseBit(dwOffest);
         dwOffestNext   = Util.ReverseBit(dwOffestNext);

         return dwOffestNext - dwOffest;
      }

      public static WORD
      SPRITE_GetCount(
         BYTE*    lpNEW
      )
      {
         WORD*    lpNew;
         WORD     wCount;

         lpNew    = (WORD*)lpNEW;
         wCount   = Util.ReverseBit(lpNew[0]);

         return wCount;
      }

      public static WORD
      SPRITE_GetChunkOffest(
         BYTE*    lpNEW,
         WORD     wChunkID
      )
      {
         WORD*    lpNew;
         WORD     wCount, dwOffest;

         //
         // 获取总块数
         //
         wCount = SPRITE_GetCount(lpNEW);

         //
         // 检查目标块是否存在
         //
         if (wChunkID > wCount)
         {
            System.Faild(
               $"NEW_GetChunkSize Faild:\n" +
               $"\tMaximum ChunkID: {wCount}\n" +
               $"\tActual ChunkID: {wChunkID}"
            );
         }

         lpNew    = (WORD*)lpNEW;
         dwOffest = Util.ReverseBit(lpNew[wChunkID]);

         return dwOffest;
      }

      public static WORD
      SPRITE_GetChunkSize(
         BYTE*    lpNEW,
         WORD     wChunkID
      )
      {
         WORD     wOffest, wOffestNext;

         wOffest       = SPRITE_GetChunkOffest(lpNEW, wChunkID);
         wOffestNext   = SPRITE_GetChunkOffest(lpNEW, wChunkID);
         wOffest       = Util.ReverseBit(wOffest);
         wOffestNext   = Util.ReverseBit(wOffestNext);

         return (WORD)(wOffestNext - wOffest);
      }
   }
}
