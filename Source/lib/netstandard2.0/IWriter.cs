/*
* Copyright 2008 ZXing authors
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System.Collections.Generic;
using ZXing.Common;
using ZXing.QrCode.Internal;

namespace ZXing
{
    /// <summary> The base class for all objects which encode/generate a barcode image.
    /// 
    /// </summary>
    /// <author>  dswitkin@google.com (Daniel Switkin)
    /// </author>
    /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
    /// </author>
    public interface IWriter
    {
        /// <summary> </summary>
        /// <param name="contents">The contents to encode in the barcode</param>
        /// <param name="format">The barcode format to generate</param>
        /// <param name="width">The preferred width in pixels</param>
        /// <param name="height">The preferred height in pixels</param>
        /// <param name="mode">The mode to encode</param>
        /// <param name="quietZone">The qrcode quiet zone</param>
        /// <param name="hints">Additional parameters to supply to the encoder</param>
        /// <returns> The generated barcode as a Matrix of unsigned bytes (0 == black, 255 == white)</returns>
        BitMatrix Encode(string contents, BarcodeFormat format, int width, int height, Mode mode = null, int quietZone = 4, IDictionary<EncodeHintType, object> hints = null);
    }
}