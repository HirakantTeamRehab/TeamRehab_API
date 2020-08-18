using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;
using System.IO;
namespace Team.Rehab.Repository.Common
{
   public static class Encryption
    {  //private static byte[] Decrypt(string inputFilePath)
        //{
        //    string EncryptionKey = "ABC";
        //    using (Aes encryptor = Aes.Create())
        //    {
        //        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
        //        encryptor.Key = pdb.GetBytes(32);
        //        encryptor.IV = pdb.GetBytes(16);
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            using (FileStream fs = new FileStream(inputFilePath, FileMode.Open))
        //            {
        //                using (CryptoStream cs = new CryptoStream(fs, encryptor.CreateDecryptor(), CryptoStreamMode.Read))
        //                {
        //                    int data;
        //                    while ((Assign(data, cs.ReadByte())) != -1)
        //                        ms.WriteByte(System.Convert.ToByte(data));
        //                }
        //            }
        //            return ms.ToArray();
        //        }
        //    }
        //}
        //private static void EncryptFile(string inputFilePath, string outputfilePath)
        //{
        //    string EncryptionKey = "ABC";
        //    using (Aes encryptor = Aes.Create())
        //    {
        //        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
        //        encryptor.Key = pdb.GetBytes(32);
        //        encryptor.IV = pdb.GetBytes(16);
        //        using (FileStream fs = new FileStream(outputfilePath, FileMode.Create))
        //        {
        //            using (CryptoStream cs = new CryptoStream(fs, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
        //            {
        //                using (FileStream fsInput = new FileStream(inputFilePath, FileMode.Open))
        //                {
        //                    int data;
        //                    while ((Assign(data, fsInput.ReadByte())) != -1)
        //                        cs.WriteByte(System.Convert.ToByte(data));
        //                    try
        //                    {
        //                        fsInput.Close();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}




        public static string AESEncryptString(string clearText, string passText, string saltText)
        {
            passText = "b1a2h3e4r5m6a7e8t";
            saltText = "b8a7h6e5r4m3a2e1t";
            byte[] clearBytes = Encoding.UTF8.GetBytes(clearText);
            byte[] passBytes = Encoding.UTF8.GetBytes(passText);
            byte[] saltBytes = Encoding.UTF8.GetBytes(saltText);
            return Convert.ToBase64String(AESEncryptBytes(clearBytes, passBytes, saltBytes));
        }

        public static string AesDecryptString(string cryptText, string passText, string saltText)
        {
            passText = "b1a2h3e4r5m6a7e8t";
            saltText = "b8a7h6e5r4m3a2e1t";
            byte[] cryptBytes = Convert.FromBase64String(cryptText);
            byte[] passBytes = Encoding.UTF8.GetBytes(passText);
            byte[] saltBytes = Encoding.UTF8.GetBytes(saltText);
            return Encoding.UTF8.GetString(AESDecryptBytes(cryptBytes, passBytes, saltBytes));
        }

        public static byte[] AESEncryptBytes(byte[] clearBytes, byte[] passBytes, byte[] saltBytes)
        {
            byte[] encryptedBytes = null;

            // create a key from the password and salt, use 32K iterations – see note
            var key = new Rfc2898DeriveBytes(passBytes, saltBytes, 32768);

            // create an AES object
            using (Aes aes = new AesManaged())
            {
                // set the key size to 256
                aes.KeySize = 256;
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(),
          CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }
            return encryptedBytes;
        }

        public static byte[] AESDecryptBytes(byte[] cryptBytes, byte[] passBytes, byte[] saltBytes)
        {
            byte[] clearBytes = null;

            // create a key from the password and salt, use 32K iterations
            var key = new Rfc2898DeriveBytes(passBytes, saltBytes, 32768);

            using (Aes aes = new AesManaged())
            {
                // set the key size to 256
                aes.KeySize = 256;
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cryptBytes, 0, cryptBytes.Length);
                        cs.Close();
                    }
                    clearBytes = ms.ToArray();
                }
            }
            return clearBytes;
        }
    }
}
