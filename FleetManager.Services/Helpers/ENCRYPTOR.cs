using System;
using System.Security.Cryptography;  
using System.Text;
using System.IO;
using MutticoFleet.Service;

namespace MutticoFleet.Service
{
	internal class ENCRYPTOR
	{
		
		public ENCRYPTOR()  
		{  
		}
        public static string GetStringFromBytes(Byte[] byte_array)
        {
            byte[] dBytes = byte_array;
            string str;
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            str = enc.GetString(dBytes);
            return str;
        }
        public static byte[] DecryptCipheredText(byte[] cipher_bytes, string pass_, byte[] IV, byte[] salt, int key_size, byte pass_interactions)
        {
            try
            {
                byte[] initVectorBytes = IV;
                byte[] saltValueBytes = salt;
                byte[] cipherTextBytes = cipher_bytes;
                PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                           pass_,
                                                           saltValueBytes,
                                                           "SHA1",
                                                           pass_interactions);

                byte[] keyBytes = password.GetBytes(key_size / 8);
                // Create uninitialized Rijndael encryption object.
                RijndaelManaged symmetricKey = new RijndaelManaged();

                // It is reasonable to set encryption mode to Cipher Block Chaining
                // (CBC). Use default options for other symmetric key parameters.
                symmetricKey.Mode = CipherMode.CBC;

                // Generate decryptor from the existing key bytes and initialization 
                // vector. Key size will be defined based on the number of the key 
                // bytes.
                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(
                                                                 keyBytes,
                                                                 initVectorBytes);

                // Define memory stream which will be used to hold unencrypted data.
                MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

                // Define cryptographic stream (always use Read mode for encryption).
                CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                              decryptor,
                                                              CryptoStreamMode.Read);

                // Since at this point we don't know what the size of decrypted data
                // will be, allocate the buffer long enough to hold ciphertext;
                // plaintext is never longer than ciphertext.
                byte[] originalTextBytes = new byte[cipherTextBytes.Length];

                // Start decrypting.
                int decryptedByteCount = cryptoStream.Read(originalTextBytes,
                                                           0,
                                                           originalTextBytes.Length);

                // Close both streams.
                memoryStream.Close();
                cryptoStream.Close();
                return originalTextBytes;
                //// Convert decrypted data into a string. 
                //// Let us assume that the original plaintext string was UTF8-encoded.
                //string plainText = Encoding.UTF8.GetString(originalTextBytes,
                //                                           0,
                //                                           decryptedByteCount);

                //// Return decrypted string.   
                //return plainText;


            }
            catch (Exception ex)
            {
               
                return null;
            }
        }

        public static string GetSHA1Hash(byte[] byte_array)
        {

            string strResult = "";
            string strHashData = "";

            byte[] arrbytHashValue;


            System.Security.Cryptography.SHA1CryptoServiceProvider oSHA1Hasher =
                       new System.Security.Cryptography.SHA1CryptoServiceProvider();

            try
            {

                arrbytHashValue = oSHA1Hasher.ComputeHash(byte_array);
                strHashData = System.BitConverter.ToString(arrbytHashValue);
                strHashData = strHashData.Replace("-", "");
                strResult = strHashData;
            }
            catch (System.Exception ex)
            {
               
            }

            return (strResult);
        }
        public static byte[] GetBytes(string _string)
        {
            if (string.IsNullOrEmpty(_string)) { return null; }
            return System.Text.Encoding.Default.GetBytes(_string);
        }
        public static string Encrypt(string original)
		{
             
           
            return Encrypt(original, password_one());
		}
		
		public static string Decrypt(string original)
		{
             
            return Decrypt(original, password_one(), System.Text.Encoding.Default);
		}
        public static string Encrypt(string original, string key)  
		{
             
			byte[] buff = System.Text.Encoding.Default.GetBytes(original);  
			byte[] kb = System.Text.Encoding.Default.GetBytes(key);
			return Convert.ToBase64String(Encrypt(buff,kb));      
		}
		public static string Decrypt(string original, string key)
		{
             
			return Decrypt(original,key,System.Text.Encoding.Default);
		}
        public static string Decrypt(string encrypted, string key,Encoding encoding)  
		{
             
			byte[] buff = Convert.FromBase64String(encrypted);  
			byte[] kb = System.Text.Encoding.Default.GetBytes(key);
			return encoding.GetString(Decrypt(buff,kb));      
		}  
		public static byte[] Decrypt(byte[] encrypted)  
		{
             
            byte[] key = System.Text.Encoding.Default.GetBytes(password_one()); 
			return Decrypt(encrypted,key);     
		}
		public static byte[] Encrypt(byte[] original)  
		{
             
            byte[] key = System.Text.Encoding.Default.GetBytes(password_one()); 
			return Encrypt(original,key);     
		}  
		public static byte[] MakeMD5(byte[] original)
		{
             
			MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();   
			byte[] keyhash = hashmd5.ComputeHash(original);       
			hashmd5 = null;  
			return keyhash;
		}
        public static byte[] Encrypt(byte[] original, byte[] key)  
		{
             
			TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();       
			des.Key =  MakeMD5(key);
			des.Mode = CipherMode.ECB;  
            return des.CreateEncryptor().TransformFinalBlock(original, 0, original.Length);     
		}  
        public static byte[] Decrypt(byte[] encrypted, byte[] key)  
		{
             
			TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();  
			des.Key =  MakeMD5(key);    
			des.Mode = CipherMode.ECB;  
            return des.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length);
		}  
	    public static string password_one()
	    {
	        byte[] b_array = {37, 70, 83, 120, 104, 35, 115, 38, 65, 33, 67, 115, 100, 56, 49};
         return   fn.GetStringFromBytes(b_array);
	    }
        public static string password_two()
        {
             
            byte[] b_array = { 37, 37, 116, 119, 105, 108, 105, 103, 104, 116, 115, 97, 103, 97, 50, 48, 49, 48, 38, 42, 40 };
            return fn.GetStringFromBytes(b_array);
        }
        
		

		
	}
}
