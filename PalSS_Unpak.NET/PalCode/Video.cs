using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

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
   public unsafe class Video
   {
      public struct Surface
      {
         public WORD             w, h;
         public BYTE*            pixels;
         public BYTE[]           pixels_arr;
         public List<Color>      palette;

         public void
         SaveAsPng(
            LPSTR    lpszPathName,
            LPSTR    lpszFileName,
            BYTE     bAlphaID = 0xFF
         )
         {
#if   FALSE
            INT                  x, y, iPixelIndex;
#endif   // FALSE
            PAL_Rect destRect;
            WriteableBitmap      wbRenderer;
            PngBitmapEncoder     encoder;

            destRect = new PAL_Rect(0, 0, this.w, this.h);

            //
            // 如果目录不存在，则先创建目录
            //
            if (!Directory.Exists(lpszPathName)) Directory.CreateDirectory(lpszPathName);

            //
            // 创建一个文件流
            //
            using (var stream = new FileStream($@"{lpszPathName}\{lpszFileName}", FileMode.Create))
            {
               Color alphaColor = this.palette[bAlphaID];
               alphaColor.A = 0;
               this.palette.RemoveAt(bAlphaID);
               this.palette.Insert(bAlphaID, alphaColor);

               //
               // writeableBitmap 为 <NULL> 时将完全覆盖
               //
               wbRenderer = new WriteableBitmap(
                  this.w, this.h, 0, 0,
                  PixelFormats.Indexed8,
                  new BitmapPalette(this.palette)
               );

               //
               // 填充像素颜色
               //
#if   FALSE
               if (FALSE)
               {
                  //
                  // 锁定 WriteableBitmap 的 BackBuffer
                  //
                  wbRenderer.Lock();

                  for (y = 0; y < destRect.Height; y++)
                  {
                     for (x = 0; x < destRect.Width; x++)
                     {
                        iPixelIndex = (destRect.Y + y) * this.w + x + destRect.X;

                        ((BYTE*)wbRenderer.BackBuffer)[iPixelIndex] = this.pixels[iPixelIndex];
                     }
                  }

                  //
                  // 标记被修改的区域
                  //
                  wbRenderer.AddDirtyRect(destRect);

                  //
                  // 解锁 WriteableBitmap 的 BackBuffer
                  //
                  wbRenderer.Unlock();
               }
               else
#endif   // FALSE
               {
                  wbRenderer.WritePixels(destRect, this.pixels_arr, this.w, 0);
               }

               //
               // 创建PNG编码器
               //
               encoder = new PngBitmapEncoder();
               encoder.Frames.Add(BitmapFrame.Create(wbRenderer));

               //
               // 将编码器中的数据保存到文件流
               //
               encoder.Save(stream);
            }
         }
      }
   }
}
