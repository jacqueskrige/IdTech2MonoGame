/*
==========================================================================================
MIT License

Copyright (c) 2011-2024 Corax Software

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
==========================================================================================
Quake is a trademark of Id Software, Inc., (c) 1996 Id Software, Inc. All rights reserved.
All other trademarks are the property of their respective owners.
------------------------------------------------------------------------------------------
 - Jacques Krige
 - jkrige1978@gmail.com
 - www.jacqueskrige.site
 - www.corax.software
==========================================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IdTech2MonoGame.Library
{
    public class CCommand
    {
        public const string TextReturn = "\r\n";

        public static bool WriteFile(string FilePath, string FileData)
        {
            FileStream FS;
            BinaryWriter BW;

            if (string.IsNullOrEmpty(FilePath) == true)
                return false;

            if (string.IsNullOrEmpty(FileData) == true)
                return false;

            FS = System.IO.File.Open(FilePath, FileMode.Create, FileAccess.Write);

            if (FS == null)
                return false;

            BW = new BinaryWriter(FS);
            for (int i = 0; i < FileData.Length; i++)
            {
                BW.Write(FileData[i]);
            }

            BW.Close();
            BW = null;

            FS.Close();
            FS.Dispose();
            FS = null;

            return true;
        }

        public static string LoadFile(string FileName)
        {
            string MapData;
            char[] ch;
            FileStream FS;
            BinaryReader BR;
            StringBuilder SB;

            if (string.IsNullOrEmpty(FileName) == true)
                return null;

            FS = System.IO.File.OpenRead(FileName);

            if (FS == null)
                return null;

            BR = new BinaryReader(FS);
            ch = BR.ReadChars((int)FS.Length);

            BR.Close();
            BR = null;

            FS.Close();
            FS.Dispose();
            FS = null;

            SB = new StringBuilder(ch.Length);
            SB.Append(ch);

            MapData = SB.ToString();

            if (string.IsNullOrEmpty(MapData) == true)
                return null;

            // fix line ends
            MapData = MapData.Replace("\r", "").Replace("\n", "\r\n");

            return MapData;
        }

    }
}
