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
using System.Configuration;

namespace IdTech2MonoGame
{
    public class CConfig
    {
        /// <summary>
        /// Gets the value of a configuration setting key and attempts to return its value as a boolean
        /// </summary>
        /// <param name="KeyName">The configuration setting key</param>
        /// <returns>Returns configuration setting key value as a boolean</returns>
        public static bool GetConfigBOOL(string KeyName)
        {
            bool KeyValue;

            try
            {
                KeyValue = Convert.ToBoolean(ConfigurationManager.AppSettings[KeyName].ToString());
            }
            catch
            {
                KeyValue = false;
            }

            return KeyValue;
        }

        /// <summary>
        /// Gets the value of a configuration setting key and attempts to return its value as a string
        /// </summary>
        /// <param name="KeyName">The configuration setting key</param>
        /// <returns>Returns configuration setting key value as a string</returns>
        public static string GetConfigSTRING(string KeyName)
        {
            string KeyValue;

            try
            {
                KeyValue = ConfigurationManager.AppSettings[KeyName].ToString();
            }
            catch
            {
                KeyValue = null;
            }

            return KeyValue;
        }

    }
}
