using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace ASA.Security
{
    /// <summary>
    /// Summary description for Encryption.
    /// </summary>
    public class Encryption
    {
        const string password = @"dGT~sfhsdkjfhkjHJGHJG%^#$@()*JKHJ_+~!@#%^";

        public static String Encrypt(object Instance)
        {
            System.IO.MemoryStream Stream = new System.IO.MemoryStream();
            BinaryFormatter bfor = new BinaryFormatter();
            bfor.Serialize(Stream, Instance);
            bfor = null;

            byte[] TicketBytes = Stream.ToArray();
            return (Encrypt(TicketBytes, password));
        }

        /// <summary>
        /// Encryptes a string using the password as the key
        /// </summary>
        /// <param name="original">String to encrypt</param>
        /// <param name="password">Password to use for encryption.</param>
        /// <returns>Encrypted string</returns>
        public static String Encrypt(String original, String password)
        {
            byte[] buffer = ASCIIEncoding.ASCII.GetBytes(original);
            return Encrypt(buffer,password);
        }

        public static string Encrypt(object Instance, string password,string Salt)
        {
            System.IO.MemoryStream Stream = new System.IO.MemoryStream();
            BinaryFormatter bfor = new BinaryFormatter();
            bfor.Serialize(Stream, Instance);
            bfor = null;

            byte [] TicketBytes = Stream.ToArray();
            return(Encrypt(TicketBytes,password));
        }

        public static T Decrypt<T>(string encryptedString, string password, string salt)
        {
            BinaryFormatter bf;
            MemoryStream ms = null;
            
            T instance;
            byte[] buffer = DecryptBytes(encryptedString, password);
            try
            {
                ms = new MemoryStream(buffer);
                {
                    bf = new BinaryFormatter();
                    instance = (T)bf.Deserialize(ms);
                    return instance;
                }
            }
            finally
            {
                bf = null;
                if (ms != null) ms.Close();
                ms = null;
            }
        }

        public static string Encrypt(byte[] buffer, string password)
        {
            String encrypted;
            TripleDESCryptoServiceProvider cryptoProvider;
            MD5CryptoServiceProvider md5Hash;
            byte[] passwordHash;

            md5Hash = new MD5CryptoServiceProvider();
            passwordHash = md5Hash.ComputeHash(ASCIIEncoding.ASCII.GetBytes(password));
            md5Hash = null;
            cryptoProvider = new TripleDESCryptoServiceProvider();
            cryptoProvider.Key = passwordHash;
            // A given bit of text is always encrypted the same way
            // when the same password is used.
            cryptoProvider.Mode = CipherMode.ECB;
            //byte[] buffer = ASCIIEncoding.ASCII.GetBytes(original);
            encrypted = Convert.ToBase64String(
                cryptoProvider.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length)
                );
            cryptoProvider = null;
            return encrypted;

        }
       
        /// <summary>
        /// Decrypts an encrypted string that was encrypted with the
        /// password provided.
        /// </summary>
        /// <param name="encryptedString">Encrypted string to decrypt</param>
        /// <param name="password">Password used to decrypt string</param>
        /// <returns>Decrypted string</returns>
        public static String Decrypt(String encryptedString, String password)
        {
            
            byte[] buffer = Convert.FromBase64String(encryptedString);
            return Decrypt(buffer,password);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encryptedString"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static T Decrypt<T>(string encryptedString)
        {
            BinaryFormatter bf;
            MemoryStream ms = null;

            T instance;
            byte[] buffer = DecryptBytes(encryptedString, password);
            try
            {
                ms = new MemoryStream(buffer);
                {
                    bf = new BinaryFormatter();
                    instance = (T)bf.Deserialize(ms);
                    return instance;
                }
            }
            finally
            {
                bf = null;
                if (ms != null) ms.Close();
                ms = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encryptedString"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static byte [] DecryptBytes(String encryptedString, String password)
        {

            byte[] buffer = Convert.FromBase64String(encryptedString);
            return DecryptBytes(buffer, password);
        }

        /// <summary>
        /// Decrypts an encrypted string that was encrypted with the
        /// password provided.
        /// </summary>
        /// <param name="encryptedString">Encrypted string to decrypt</param>
        /// <param name="password">Password used to decrypt string</param>
        /// <returns>Decrypted string</returns>
        public static String Decrypt(byte [] buffer, String password)
        {
            String decrypted;
            decrypted = ASCIIEncoding.ASCII.GetString(DecryptBytes(buffer,password));
            return decrypted;
        }
        public static byte [] DecryptBytes(byte[] buffer, String password)
        {
            TripleDESCryptoServiceProvider cryptoProvider;
            MD5CryptoServiceProvider md5Hash;
            byte[] passwordHash;
            md5Hash = new MD5CryptoServiceProvider();
            passwordHash = md5Hash.ComputeHash(ASCIIEncoding.ASCII.GetBytes(password));
            md5Hash = null;
            cryptoProvider = new TripleDESCryptoServiceProvider();
            cryptoProvider.Key = passwordHash;
            // A given bit of text is always encrypted the same way
            // when the same password is used.
            cryptoProvider.Mode = CipherMode.ECB;
            
            byte [] decrypted = cryptoProvider.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length);            
            cryptoProvider = null;
            return decrypted;
        }
    }
}
