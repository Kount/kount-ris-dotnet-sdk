//-----------------------------------------------------------------------
// <copyright file="Base85.cs" company="Keynetics Inc">
//   2011 Kount Inc. All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Util
{
    using System;
    using System.Text;
    using System.IO;

    /// <summary>
    /// C# implementation of BASE85 encoding. 
    /// Based on C code from http://www.stillhq.com/cgi-bin/cvsweb/BASE85/
    /// </summary>
    public static class Base85
    {
        /// <summary>
        /// Maximum line length for encoded BASE85 string; 
        /// set to zero for one unbroken line.
        /// </summary>
        public static int LineLength = 75;

        private const int _asciiOffset = 33;
        private static byte[] _decodedBlock = new byte[4];
        private static byte[] _encodedBlock = new byte[5];
        private static int _linePos = 0;
        private static uint _tuple = 0;
        private static uint[] pow85 = { 85 * 85 * 85 * 85,
                                        85 * 85 * 85,
                                        85 * 85,
                                        85,
                                        1 };

        /// <summary>
        /// Decodes an BASE85 encoded string into the original binary data
        /// </summary>
        /// <param name="s">BASE85 encoded string</param>
        /// <returns>byte array of decoded binary data</returns>
        public static byte[] Decode(string s)
        {
            byte[] deRes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                int count = 0;
                bool processChar = false;

                foreach (char c in s)
                {
                    switch (c)
                    {
                        case 'z':
                            if (count != 0)
                            {
                                throw new Exception("The character 'z' is invalid inside an BASE85 block.");
                            }
                            _decodedBlock[0] = 0;
                            _decodedBlock[1] = 0;
                            _decodedBlock[2] = 0;
                            _decodedBlock[3] = 0;
                            ms.Write(_decodedBlock, 0, _decodedBlock.Length);
                            processChar = false;
                            break;
                        case '\n':
                        case '\r':
                        case '\t':
                        case '\0':
                        case '\f':
                        case '\b':
                            processChar = false;
                            break;
                        default:
                            if (c < '!' || c > 'u')
                            {
                                throw new Exception("Bad character '" + c + "' found. BASE85 only allows characters '!' to 'u'.");
                            }
                            processChar = true;
                            break;
                    }

                    if (processChar)
                    {
                        _tuple += ((uint)(c - _asciiOffset) * pow85[count]);
                        count++;
                        if (count == _encodedBlock.Length)
                        {
                            DecodeBlock();
                            ms.Write(_decodedBlock, 0, _decodedBlock.Length);
                            _tuple = 0;
                            count = 0;
                        }
                    }
                }

                // if we have some bytes left over at the end..
                if (count != 0)
                {
                    if (count == 1)
                    {
                        throw new Exception("The last block of BASE85 data cannot be a single byte.");
                    }
                    count--;
                    _tuple += pow85[count];
                    DecodeBlock(count);
                    for (int i = 0; i < count; i++)
                    {
                        ms.WriteByte(_decodedBlock[i]);
                    }
                }

                deRes = ms.ToArray();
            }

            return deRes;
        }

        /// <summary>
        /// Encodes binary data into a plaintext BASE85 format string
        /// </summary>
        /// <param name="ba">binary data to encode</param>
        /// <returns>BASE85 encoded string</returns>
        public static string Encode(byte[] ba)
        {
            StringBuilder sb = new StringBuilder((int)(ba.Length * (_encodedBlock.Length / _decodedBlock.Length)));
            _linePos = 0;

            int count = 0;
            _tuple = 0;
            foreach (byte b in ba)
            {
                if (count >= _decodedBlock.Length - 1)
                {
                    _tuple |= b;
                    if (_tuple == 0)
                    {
                        AppendChar(sb, 'z');
                    }
                    else
                    {
                        EncodeBlock(sb);
                    }
                    _tuple = 0;
                    count = 0;
                }
                else
                {
                    _tuple |= (uint)(b << (24 - (count * 8)));
                    count++;
                }
            }

            // if we have some bytes left over at the end..
            if (count > 0)
            {
                EncodeBlock(count + 1, sb);
            }

            return sb.ToString();
        }

        private static void AppendChar(StringBuilder sb, char c)
        {
            sb.Append(c);
            _linePos++;
            if (LineLength > 0 && (_linePos >= LineLength))
            {
                _linePos = 0;
                sb.Append('\n');
            }
        }

        private static void AppendString(StringBuilder sb, string s)
        {
            if (LineLength > 0 && (_linePos + s.Length > LineLength))
            {
                _linePos = 0;
                sb.Append('\n');
            }
            else
            {
                _linePos += s.Length;
            }
            sb.Append(s);
        }

        private static void DecodeBlock()
        {
            DecodeBlock(_decodedBlock.Length);
        }

        private static void DecodeBlock(int bytes)
        {
            for (int i = 0; i < bytes; i++)
            {
                _decodedBlock[i] = (byte)(_tuple >> 24 - (i * 8));
            }
        }

        private static void EncodeBlock(StringBuilder sb)
        {
            EncodeBlock(_encodedBlock.Length, sb);
        }

        private static void EncodeBlock(int count, StringBuilder sb)
        {
            for (int i = _encodedBlock.Length - 1; i >= 0; i--)
            {
                _encodedBlock[i] = (byte)((_tuple % 85) + _asciiOffset);
                _tuple /= 85;
            }

            for (int i = 0; i < count; i++)
            {
                char c = (char)_encodedBlock[i];
                AppendChar(sb, c);
            }

        }

    }

}