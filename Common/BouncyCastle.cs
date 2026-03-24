using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Text;

namespace HaskyNavLink.Common
{
    /// <summary>
    /// 国密加解密
    /// </summary>
    internal static class BouncyCastle
    {
        private static readonly byte[] iv = Encoding.UTF8.GetBytes("guomipianyiliang");
        private static readonly string publicKey = "gonggong4jjmiyao";


        /// <summary>
        /// 国密SM3加密,对标MD5
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string SM3Encrypt(this string txt)
        {
            byte[] plaintext = Encoding.UTF8.GetBytes(txt);
            IDigest sm3 = DigestUtilities.GetDigest("SM3");
            sm3.BlockUpdate(plaintext, 0, plaintext.Length);
            byte[] result = new byte[sm3.GetDigestSize()];
            sm3.DoFinal(result, 0);
            return Convert.ToHexString(result);
        }

        /// <summary>
        /// 国密SM4加密
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string SM4Encrypt(this string txt)
        {
            return SM4Encrypt(txt, publicKey);
        }

        /// <summary>
        /// 国密SM4加密
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="keystr"></param>
        /// <returns></returns>
        public static string SM4Encrypt(string txt, string keystr)
        {
            byte[] plaintext = Encoding.UTF8.GetBytes(txt);
            byte[] keyBytes = Encoding.UTF8.GetBytes(keystr);
            var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new SM4Engine()), new Pkcs7Padding());
            cipher.Init(true, new ParametersWithIV(new KeyParameter(keyBytes), iv));
            byte[] output = new byte[cipher.GetOutputSize(plaintext.Length)];
            int bytesProcessed = cipher.ProcessBytes(plaintext, 0, plaintext.Length, output, 0);
            cipher.DoFinal(output, bytesProcessed);
            return Convert.ToBase64String(output);

        }

        /// <summary>
        /// 国密SM4解密
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string SM4Decrypt(this string txt)
        {
            return SM4Decrypt(txt, publicKey);
        }

        /// <summary>
        /// 国密SM4解密
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="keystr"></param>
        /// <returns></returns>
        public static string SM4Decrypt(string txt, string keystr)
        {
            byte[] ciphertext = Convert.FromBase64String(txt); // 修复：Base64解码输入
            byte[] keyBytes = Encoding.UTF8.GetBytes(keystr);

            var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new SM4Engine()), new Pkcs7Padding());
            cipher.Init(false, new ParametersWithIV(new KeyParameter(keyBytes), iv));
            byte[] output = new byte[cipher.GetOutputSize(ciphertext.Length)];
            int bytesProcessed = cipher.ProcessBytes(ciphertext, 0, ciphertext.Length, output, 0);
            bytesProcessed += cipher.DoFinal(output, bytesProcessed); // 累加最终处理的字节数
            return Encoding.UTF8.GetString(output, 0, bytesProcessed); // 截取有效数据
        }
    }
}