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

namespace PalssUnpak
{
   public class Util
   {
      public static WORD
      ReverseBit(
         WORD     value
      )
      {
         if (BitConverter.IsLittleEndian)
         {
            value = BitConverter.ToUInt16(BitConverter.GetBytes(value).Reverse().ToArray(), 0);
         }

         return value;
      }

      public static DWORD
      ReverseBit(
         DWORD    value
      )
      {
         if (BitConverter.IsLittleEndian)
         {
            value = BitConverter.ToUInt32(BitConverter.GetBytes(value).Reverse().ToArray(), 0);
         }

         return value;
      }

      public static QWORD
      ReverseBit(
         QWORD    value
      )
      {
         if (BitConverter.IsLittleEndian)
         {
            value = BitConverter.ToUInt64(BitConverter.GetBytes(value).Reverse().ToArray(), 0);
         }

         return value;
      }

      public static SHORT
      ReverseBit_Signed(
         SHORT    value
      )
      {
         if (BitConverter.IsLittleEndian)
         {
            value = BitConverter.ToInt16(BitConverter.GetBytes(value).Reverse().ToArray(), 0);
         }

         return value;
      }

      public static INT
      ReverseBit_Signed(
         INT      value
      )
      {
         if (BitConverter.IsLittleEndian)
         {
            value = BitConverter.ToInt32(BitConverter.GetBytes(value).Reverse().ToArray(), 0);
         }

         return value;
      }

      public static LONG
      ReverseBit_Signed(
         LONG     value
      )
      {
         if (BitConverter.IsLittleEndian)
         {
            value = BitConverter.ToInt64(BitConverter.GetBytes(value).Reverse().ToArray(), 0);
         }

         return value;
      }
   }
}
