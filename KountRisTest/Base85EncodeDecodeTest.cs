//-----------------------------------------------------------------------
// <copyright file="Base85EncodeDecodeTest.cs" company="Keynetics Inc">
//     Copyright  Kount Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace KountRisTest
{
    using Kount.Ris;
    using Kount.Enums;
    using Kount.Util;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Text;

    /// <summary>
    /// Base85 Encode Decode Test samples
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 0700 <br/>
    /// <b>Copyright:</b> 2017 Kount Inc. All Rights Reserved<br/>
    /// </summary>
    [TestClass]
    public class Base85EncodeDecodeTest
    {
        static string PLAIN_TEXT = "This is sample text for testing purposes.";
	    static string ENCODED_TEXT = "<+oue+DGm>F(&p)Ch4`2AU&;>AoD]4FCfN8Bl7Q+E-62?Df]K2/c";

        [TestMethod]
        public void TestEncode()
        {
            string encoded = Base85.Encode(Encoding.UTF8.GetBytes(PLAIN_TEXT));
            Assert.IsTrue(encoded.Equals(ENCODED_TEXT), "Encoded value is not the same as the expected.");
        }

        [TestMethod]
        public void TestDecode() 
        {
            string decoded = Encoding.UTF8.GetString(Base85.Decode(ENCODED_TEXT));
            Assert.IsTrue(decoded.Equals(PLAIN_TEXT), "Decoded value is not the same as the expected.");
        }
    }
}
