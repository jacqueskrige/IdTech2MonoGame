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

namespace IdTech2MonoGame
{
    public class CFiles
    {
        // ========================================================================
        // PAK FILE LOADING
        // 
        // The .pak files are just a linear collapse of a directory tree
        // ========================================================================
        public const int IDPAKHEADER = (('K' << 24) + ('C' << 16) + ('A' << 8) + 'P');
        public const int PACK_MAX_FILES = 8192; // bumped from 4096 to support loading Heretic2
        public const int PACK_MAX_FILENAME_LENGTH = 56;

        // total number of files inside pak
        private int fs_packFiles;

        // the loaded pack information
        private SPack? qPack;

        public void FS_LoadPak(string PakFile, string BaseName)
        {
            if (CProgram.gQ2Game.gCMain.r_usepak == false)
                return;

            qPack = FS_LoadPakFile(PakFile, BaseName);
        }

        public void FS_ClosePak()
        {
            if (CProgram.gQ2Game.gCMain.r_usepak == false)
                return;

            if (qPack.HasValue == false)
                return;

            qPack.Value.handle.Close();
            qPack = null;
        }

        /// <summary>
        /// FS_ReadFile
        /// -----------
        /// filename are relative to the quake search path
        /// </summary>
        public void FS_ReadFile(string qpath, out byte[] qdata)
        {
            qdata = FS_ReadFile2(qpath);
        }

        public void FS_ReadFile(string qpath, out MemoryStream qdata)
        {
            byte[] data = FS_ReadFile2(qpath);

            if (data != null)
                qdata = new MemoryStream(data);
            else
                qdata = null;
        }

        private byte[] FS_ReadFile2(string qpath)
        {
            int i;
            bool found = false;

            if (qPack.HasValue == false)
                return null;

            for (i = 0; i < qPack.Value.buildBuffer.Count; i++)
            {
                if (qpath == qPack.Value.buildBuffer[i].Name)
                {
                    found = true;
                    break;
                }

            }

            if (found == true)
            {
                BinaryReader r = new BinaryReader(qPack.Value.handle);

                r.BaseStream.Seek(qPack.Value.buildBuffer[i].Position, System.IO.SeekOrigin.Begin);
                return r.ReadBytes(qPack.Value.buildBuffer[i].Size);
            }

            return null;
        }

        private SPack? FS_LoadPakFile(string PakFile, string BaseName)
        {
            SPack Pack;
            BinaryReader r;

            PakFile = CProgram.gQ2Game.Content.RootDirectory + "\\" + PakFile;

            if (File.Exists(PakFile) == false)
            {
                CMain.Error(CMain.EErrorParm.ERR_FATAL, "PAK file not found.");
                return null;
            }

            Pack.handle = File.OpenRead(PakFile);

            if (Pack.handle == null)
                return null;

            r = new BinaryReader(Pack.handle);
            PakFile = PakFile.ToLower();
            BaseName = BaseName.ToLower();

            if (r.ReadInt32() != IDPAKHEADER)
            {
                Pack.handle.Close();
                Pack.handle = null;

                CMain.Error(CMain.EErrorParm.ERR_FATAL, PakFile + " is not a packfile");
                return null;
            }

            Pack.packDirOffset = r.ReadInt32();
            Pack.packDirLength = r.ReadInt32();

            // if the directory offset is beyond the EOF then we assume its htic2-0.pak
            // Raven Software probably did this so unaware PAK readers fails to read the Heretic2 content
            if (CProgram.gQ2Game.gCMain.r_htic2 == true)
            {
                if (Pack.packDirOffset > r.BaseStream.Length)
                {
                    Pack.packDirOffset = 215695973; // 0x0cdb4265 (215 695 973 bytes)
                    Pack.packDirLength = 264256;    // EOF - Pack.packDirOffset (EOF: 0x0cdf4aa5 | 215 960 229 bytes)
                }
            }

            // PACK_MAX_FILENAME_LENGTH + FilePosition + FileLength
            Pack.numfiles = Pack.packDirLength / (PACK_MAX_FILENAME_LENGTH + sizeof(int) + sizeof(int));

            if (Pack.numfiles > PACK_MAX_FILES)
                CMain.Error(CMain.EErrorParm.ERR_FATAL, PakFile + " has " + Pack.numfiles + " files");

            Pack.buildBuffer = new List<SFileInPack>();

            fs_packFiles += Pack.numfiles;
            Pack.pakFilename = PakFile;
            Pack.pakBasename = BaseName.Replace(".pak", "");

            r.BaseStream.Seek(Pack.packDirOffset, SeekOrigin.Begin);

            for (int i = 0; i < Pack.numfiles; i++)
            {
                SFileInPack _FileInPack;

                _FileInPack.Name = CShared.Com_ToString(r.ReadChars(PACK_MAX_FILENAME_LENGTH)).ToLower();
                _FileInPack.Position = r.ReadInt32();
                _FileInPack.Size = r.ReadInt32();

                Pack.buildBuffer.Add(_FileInPack);
            }

            //for (int i = 0; i < Pack.buildBuffer.Count; i++)
            //{
            //    r.BaseStream.Seek(Pack.buildBuffer[i].Position, System.IO.SeekOrigin.Begin);
            //    fs_headerLongs[fs_numHeaderLongs++] = CCrc32.GetMemoryCRC32(r.ReadBytes(Pack.buildBuffer[i].Size));
            //}

            //Pack.checksum = CProgram.vCCommon.Com_BlockChecksum(fs_headerLongs);
            //Pack.pure_checksum = CProgram.vCCommon.Com_BlockChecksumKey(fs_headerLongs, fs_checksumFeed);

            // As of yet unassigned
            //Pack.hashSize = 0;
            Pack.pakGamename = null;

            return Pack;
        }


        public struct SPack
        {
            public string pakFilename;              // c:\quake2\baseq2\pak0.pak
            public string pakBasename;              // pak0
            public string pakGamename;              // baseq2
            public FileStream handle;               // handle to pack file
            //public long checksum;                 // regular checksum
            //public long pure_checksum;            // checksum for pure
            public int numfiles;                    // number of files in pak
            public List<SFileInPack> buildBuffer;   // file entry

            public int packDirOffset;
            public int packDirLength;
        }

        public struct SFileInPack
        {
            public string Name;     // name of the file
            public long Position;   // file info position in pak
            public int Size;        // file info size in pak
        }


        // ========================================================================
        // PCX FILE LOADING
        // 
        // Used for as many images as possible
        // ========================================================================
        public struct SPCX
        {
            public byte manufacturer;
            public byte version;
            public byte encoding;
            public byte bits_per_pixel;
            public ushort xmin;
            public ushort ymin;
            public ushort xmax;
            public ushort ymax;
            public ushort hres;
            public ushort vres;
            public byte[] palette; // size: 48 (unsigned char ??)
            public byte reserved;
            public byte color_planes;
            public ushort bytes_per_line;
            public ushort palette_type;
            public byte[] filler; // size: 58
            public byte data; // unsigned char ??
        }

        /*typedef struct
        {
            char	manufacturer;
            char	version;
            char	encoding;
            char	bits_per_pixel;
            unsigned short	xmin,ymin,xmax,ymax;
            unsigned short	hres,vres;
            unsigned char	palette[48];
            char	reserved;
            char	color_planes;
            unsigned short	bytes_per_line;
            unsigned short	palette_type;
            char	filler[58];
            unsigned char	data;			// unbounded
        } pcx_t;*/

    }
}
