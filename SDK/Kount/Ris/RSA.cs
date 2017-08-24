//-----------------------------------------------------------------------
// <copyright file="RSA.cs" company="Keynetics Inc">
//     Copyright Keynetics. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Ris
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// RSA Utility class for encrypting PANs <br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 7.0.0. <br/>
    /// <b>Copyright:</b> 2010 Keynetics Inc <br/>
    /// </summary>
    public class RSA
    {
        /// <summary>
        /// Encrypt a token with an RSA public key
        /// </summary>
        /// <param name="plainText">Plaintext string</param>
        /// <returns>Encrypted string</returns>
        public string Encrypt(string plainText)
        {
            //// read pem public key
            Stream s = System.Reflection.Assembly.GetExecutingAssembly().
                GetManifestResourceStream("KountRisSdk.kount.rsa.public.key");
            System.IO.StreamReader reader = new System.IO.StreamReader(s, Encoding.UTF8);
            string pem = reader.ReadToEnd();

            string header = String.Format("-----BEGIN PUBLIC KEY-----");
            string footer = String.Format("-----END PUBLIC KEY-----");
            int start = pem.IndexOf(header, StringComparison.Ordinal) + header.Length;
            int end = pem.IndexOf(footer, start, StringComparison.Ordinal) - start;
            byte[] key = Convert.FromBase64String(pem.Substring(start, end));

            UnicodeEncoding converter = new UnicodeEncoding();
            byte[] plainBytes = converter.GetBytes(plainText);

            RSACryptoServiceProvider rsa = null;

            //// encoded OID sequence for PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            byte[] seqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };

            byte[] seq = new byte[15];

            //// Read the asn.1 encoded SubjectPublicKeyInfo blob
            System.IO.MemoryStream mem = new System.IO.MemoryStream(key);
            System.IO.BinaryReader binr = new System.IO.BinaryReader(mem);
            byte bt = 0;
            ushort twobytes = 0;

            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                {
                    //// data read as little endian order
                    //// (actual data order for Sequence is 30 81)
                    binr.ReadByte(); //// advance 1 byte
                }
                else if (twobytes == 0x8230)
                {
                    binr.ReadInt16(); //// advance 2 bytes
                }
                else
                {
                    return null;
                }

                seq = binr.ReadBytes(15); //// read the Sequence OID
                if (!this.CompareBytearrays(seq, seqOID))
                {
                    //// make sure Sequence for OID is correct
                    return null;
                }

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8103)
                {
                    //// data read as little endian order (actual data order for Bit String is 03 81)
                    binr.ReadByte(); //// advance 1 byte
                }
                else if (twobytes == 0x8203)
                {
                    binr.ReadInt16(); //// advance 2 bytes
                }
                else
                {
                    return null;
                }

                bt = binr.ReadByte();
                if (bt != 0x00)
                {
                    //// expect null byte next
                    return null;
                }

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                {
                    //// data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte(); //// advance 1 byte
                }
                else if (twobytes == 0x8230)
                {
                    binr.ReadInt16(); //// advance 2 bytes
                }
                else
                {
                    return null;
                }

                twobytes = binr.ReadUInt16();
                byte lowbyte = 0x00;
                byte highbyte = 0x00;

                if (twobytes == 0x8102)
                {
                    //// data read as little endian order
                    //// (actual data order for Integer is 02 81)
                    lowbyte = binr.ReadByte();  //// read next bytes which is bytes in modulus
                }
                else if (twobytes == 0x8202)
                {
                    highbyte = binr.ReadByte(); //// advance 2 bytes
                    lowbyte = binr.ReadByte();
                }
                else
                {
                    return null;
                }

                byte[] modint =
                {
                  lowbyte, highbyte, 0x00, 0x00
                };

                int modsize = BitConverter.ToInt32(modint, 0);

                int firstbyte = binr.PeekChar();
                if (firstbyte == 0x00)
                {
                    //// if first byte (highest order) of modulus is zero, don't include it
                    binr.ReadByte(); //// skip this null byte
                    modsize -= 1; //// reduce modulus buffer size by 1
                }

                byte[] modulus = binr.ReadBytes(modsize); //// read the modulus bytes
                if (binr.ReadByte() != 0x02)
                {
                    //// expect an Integer for the exponent data
                    return null;
                }

                int expbytes = (int)binr.ReadByte();
                byte[] exponent = binr.ReadBytes(expbytes);

                //// create RSACryptoServiceProvider instance and initialize public key
                rsa = new RSACryptoServiceProvider();
                RSAParameters rsaKeyInfo = new RSAParameters();
                rsaKeyInfo.Modulus = modulus;
                rsaKeyInfo.Exponent = exponent;
                rsa.ImportParameters(rsaKeyInfo);
            }
            finally
            {
                binr.Close();
            }

            byte[] encryptedBytes = rsa.Encrypt(plainBytes, false);
            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// Compare byte arrays.
        /// </summary>
        /// <param name="a">Byte array a for comparison.</param>
        /// <param name="b">Byte array b for comparison.</param>
        /// <returns>Bool true if byte arrays are equal.</returns>
        private bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                {
                    return false;
                }

                i++;
            }

            return true;
        }
    }
}