using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LocalCommons.Cryptography
{
    public class Encrypt
    {
        /// <summary>
        /// Calculation of the packet checksum, used in packet encryption DD05 and 0005
        /// </summary>
        public static byte Crc8(byte[] data, int size)
        {
            var len = size;
            uint checksum = 0;
            for (var i = 0; i <= len - 1; i++)
            {
                checksum *= 0x13;
                checksum += data[i];
            }
            return (byte)(checksum);
        }
        public static byte Crc8(byte[] data)
        {
            var len = data.Length;
            uint checksum = 0;
            for (var i = 0; i <= len - 1; i++)
            {
                checksum = checksum * 0x13;
                checksum += data[i];
            }
            return (byte)(checksum);
        }
        //--------------------------------------------------------------------------------------
        /// <summary>
        /// sub routine for encode / decode server / client packages
        /// </summary>
        /// <param name="cry"></param>
        /// <returns></returns>
        private static byte Inline(ref uint cry)
        {
            cry += 0x2FCBD5U;
            var n = (byte)(cry >> 0x10);
            n = (byte)(n & 0x0F7);
            return (byte)(n == 0 ? 0x0FE : n);
        }
        //--------------------------------------------------------------------------------------
        /// <summary>

        /// subroutine for encode / decode server packages, correctly encrypts and decrypts server packages DD05 for version 3.0.3.0
        /// </summary>
        /// <param name="bodyPacket">address starting with a byte of DD05</param>
        /// <returns>returns the address to the prepared data</returns>
        public static byte[] StoCEncrypt(byte[] bodyPacket)
        {
            var array = new byte[bodyPacket.Length];
            var cry = (uint)(bodyPacket.Length ^ 0x1F2175A0);
            var n = 4 * (bodyPacket.Length / 4);
            for (var i = n - 1; i >= 0; i--)
            {
                array[i] = (byte)(bodyPacket[i] ^ (uint)Inline(ref cry));
            }

            for (var i = n; i < bodyPacket.Length; i++)
            {
                array[i] = (byte)(bodyPacket[i] ^ (uint)Inline(ref cry));
            }

            return array;
        }
        //--------------------------------------------------------------------------------------
        //internal static uint XorKey; // XorKey = XorKey * XorKey & 0xffffffff; TODO: find where it comes from!!!;
        internal static int Num = -1;
        //--------------------------------------------------------------------------------------
        /// <summary>
        /// DeXORing packet
        /// </summary>
        /// <param name="bodyPacket">packet data, starting after message-key byte</param>
        /// <param name="msgKey">unique key for each message</param>
        /// <param name="xorKey">xor key </param>
        /// <param name="offset">xor decryption can start from some offset (don't know the rule yet)</param>
        /// <returns>xor decrypted packet</returns>
        private static byte[] DecryptXor(byte[] bodyPacket, uint msgKey, uint xorKey, int offset = 0)
        {
            var length = bodyPacket.Length;
            var array = new byte[length];

            var mul = xorKey * msgKey;
            var key = (0x75a024a4 ^ mul) ^ 0xC3903b6a;
            var n = 4 * (length / 4);
            for (var i = n - offset - 1; i >= 0; i--)
            {
                array[i] = (byte)(bodyPacket[i] ^ (uint)Inline(ref key));
            }
            for (var i = n - offset; i < length; i++)
            {
                array[i] = (byte)(bodyPacket[i] ^ (uint)Inline(ref key));
            }
            return array;
        }
        //--------------------------------------------------------------------------------------
        public static byte[] CtoSEncrypt(byte[] bodyPacket, uint xorKey)
        {
            uint msgKey = 0;
            var length = bodyPacket.Length;
            var mBodyPacket = new byte[length - 5];
            Buffer.BlockCopy(bodyPacket, 5, mBodyPacket, 0, length - 5);
            byte[] packet = new byte[mBodyPacket.Length];

            int caseSwitch = bodyPacket[4];
            switch (caseSwitch)
            {
                case 0x30: //like, we are only interested in the first figure
                                        msgKey = 0x01; //0X11; //0x2F
                    break;
                case 0x31:
                    msgKey = 0x02; //0x02; //? No
                    break;
                case 0x32:
                    msgKey = 0x03; //0x03; //? No
                    break;
                case 0x33:
                    msgKey = 0x04; //0x04; //Yes, Checked
                    break;
                case 0x34:
                    msgKey = 0x05; //0x15; //? There is
                    break;
                case 0x35:
                    msgKey = 0x06; //0x16; //Yes, Checked
                    break;
                case 0x36:
                    msgKey = 0x07; //0x27; //Yes, Checked 0x17 - when exiting the game, 0x07
                    break;
                case 0x37:
                    msgKey = 0x08; //0x08; //Yes, Checked 0x0D88 - When exiting the game
                    break;
                case 0x38:
                    msgKey = 0x09; //
                    break;
                case 0x39:
                    msgKey = 0x0A; //Yes, Checked
                    break;
                case 0x3A:
                    msgKey = 0x0b; //0x1B; //? No
                    break;
                case 0x3B:
                    msgKey = 0x0c; //0x1C; //
                    break;
                case 0x3C:
                    msgKey = 0x0d; //0x0D; //Yes, Checked
                    break;
                case 0x3D:
                    msgKey = 0x0e; //0x1E; //? No
                    break;
                case 0x3E:
                    msgKey = 0x0f; //0x1F; //
                    break;
                case 0x3F:
                    msgKey = 0x10; //0x10; //Yes, Checked
                    break;
            }
            Num += 1; // global calculation of client packages
            // Hardcoded offset rules
            switch (Num)
            {
                case 0:
                    packet = DecryptXor(mBodyPacket, xorKey, msgKey, 7);
                    break;
                case 1:
                case 5:
                case 6:
                case 7:
                case 9:
                case 19:
                case 21:
                case 28:
                case 29:
                case 30:
                case 34:
                case 35:
                case 46:
                case 48:
                case 50:
                case 52:
                    packet = DecryptXor(mBodyPacket, xorKey, msgKey, 1);
                    break;
                case 15:
                case 41:
                    packet = DecryptXor(mBodyPacket, xorKey, msgKey, 5);
                    break;
                case 16:
                case 25:
                case 31:
                case 37:
                case 44:
                case 51:
                    packet = DecryptXor(mBodyPacket, xorKey, msgKey, 2);
                    break;
                //case ??:
                //    packet = Decryptxor(mBodyPacket, xorKey, msgKey, 3);
                //    break;
                default:
                    packet = DecryptXor(mBodyPacket, xorKey, msgKey);
                    break;
            }
            return packet;
        }

        /*
         
        * You have two options (and here I describe only the encryption process, but the decryption is similar):
        Use a stream cipher (for example, AES-CTR)
        You initialize the cipher with a 16-byte key and a truly random 16-byte nonce, write a nonce,
        load the first part, encrypt it, write down the result, load the second part and so on.
        Note that you must initialize the cipher only once. The size of a piece can be arbitrary;
        it does not even have to be the same every time.
        
        
        Use a block cipher with single-pass chain mode, for example AES128-CBC
        You initialize the cipher with a 16-byte key, generate a random 16-byte IV, write IV, write the total length of the file,
        load the first part, encrypt it together with IV, record the result, load the second fragment, encrypt using
        The last 16 bytes of the previous encrypted block as IV, write down the result, etc.
        The fragment size must be a multiple of 16 bytes; again, it does not have to be the same every time.
        You may need to fill the last block with zeros.
        
        
        In both cases
        You must compute the cryptographic hash of the original unencrypted file (for example, using SHA-256) and write it down,
        when the encryption is complete. It's pretty simple: you initialize the hash at the very beginning and load each block into it,
        as soon as it is loaded (including nonce / IV and, possibly, a length field). On the decryption side, you do the same.
        After all, you need to make sure that the computed digest corresponds to the one that came with the encrypted file.
        EDIT: nonce / IV and length are also hashed.
         */

        private const int Size = 16;

        private static RijndaelManaged GetRijndaelManaged(byte[] key, byte[] iv)
        {
            var rm = new RijndaelManaged
            {
                KeySize = 128,
                BlockSize = 128,
                Padding = PaddingMode.None,
                Mode = CipherMode.CBC
            };
            rm.Key = key;
            rm.IV = iv;

            return rm;
        }
        public static byte[] DecryptAes(byte[] cipherData, byte[] key, byte[] iv)
        {
            byte[] mIv = new byte[16];
            Buffer.BlockCopy(iv, 0, mIv, 0, Size);
            var len = cipherData.Length / Size;
            Buffer.BlockCopy(cipherData, (len - 1) * Size, iv, 0, Size);
            //Buffer.BlockCopy(cipherData, 0, iv, 0, Size);
            // Create a MemoryStream that is going to accept the decrypted bytes
            using (var memoryStream = new MemoryStream())
            {
                // Create a symmetric algorithm.
                // We are going to use RijndaelRijndael because it is strong and available on all platforms.
                // You can use other algorithms, to do so substitute the next line with something like
                // TripleDES alg = TripleDES.Create();
                using (var alg = GetRijndaelManaged(key, mIv))
                {
                    // Create a CryptoStream through which we are going to be pumping our data.
                    // CryptoStreamMode.Write means that we are going to be writing data to the stream
                    // and the output will be written in the MemoryStream we have provided.
                    using (var cs = new CryptoStream(memoryStream, alg.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        // Write the data and make it do the decryption
                        cs.Write(cipherData, 0, cipherData.Length);

                        // Close the crypto stream (or do FlushFinalBlock).
                        // This will tell it that we have done our decryption and there is no more data coming in,
                        // and it is now a good time to remove the padding and finalize the decryption process.
                        cs.FlushFinalBlock();
                        cs.Close();
                    }
                }
                // Now get the decrypted data from the MemoryStream.
                // Some people make a mistake of using GetBuffer() here, which is not the right way.
                var decryptedData = memoryStream.ToArray();
                return decryptedData;
            }
        }
    }

    /*
    In general, the classical Diffie-Hellman algorithm looks like this:

    Both sides choose the same more integer.
    Both sides choose, so-called, the generator (usually AES), which will be used for calculations.
    Regardless of each other, each side chooses another number that is kept secret.
    It is used as the private key for this operation.
    The generated private key, the AES generator and the total number (item 1) are used to create a public key,
    which, in the end, is associated with a private one, but can be transferred to the other party.
    Both sides are being changed by public keys that have just been created.
    Each party uses its private key, the public key of the other party and the number of clause 1,
    to calculate the shared secret key. Although, the calculation process is independent for each of the parties,
    the result is the same keys. This is the essence of the Diffie-Hellman algorithm.
    Then, the secret key obtained is used to symmetrically encrypt the connection.

    
    Symmetric encryption, which is used from this point throughout the entire connection,
    is called a binary packet protocol. The above process,
    allows each party to participate in the generation of a shared secret key.
    Does not allow one of the parties to control it. But most importantly, it becomes possible to create the same key on each of the machines,
    without its transfer through an unprotected connection.

    
    The resulting key is symmetric. That is, it is used for encryption and decryption.
    Its purpose is to protect all transmitted data between the client and the server, create a kind of tunnel,
    the contents of which can not be read by third parties
    */

    class Alice
    {
        public static byte[] alicePublicKey;

        public static void Main(string[] args)
        {
            using (ECDiffieHellmanCng alice = new ECDiffieHellmanCng())
            {

                alice.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                alice.HashAlgorithm = CngAlgorithm.Sha256;
                alicePublicKey = alice.PublicKey.ToByteArray();
                Bob bob = new Bob();
                CngKey k = CngKey.Import(bob.bobPublicKey, CngKeyBlobFormat.EccPublicBlob);
                byte[] aliceKey = alice.DeriveKeyMaterial(CngKey.Import(bob.bobPublicKey, CngKeyBlobFormat.EccPublicBlob));
                byte[] encryptedMessage = null;
                byte[] iv = null;
                Send(aliceKey, "Secret message", out encryptedMessage, out iv);
                bob.Receive(encryptedMessage, iv);
            }

        }

        private static void Send(byte[] key, string secretMessage, out byte[] encryptedMessage, out byte[] iv)
        {
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                iv = aes.IV;

                // Encrypt the message
                using (MemoryStream ciphertext = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] plaintextMessage = Encoding.UTF8.GetBytes(secretMessage);
                    cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                    cs.Close();
                    encryptedMessage = ciphertext.ToArray();
                }
            }
        }

    }

    public class Bob
    {
        public byte[] bobPublicKey;
        private byte[] bobKey;
        public Bob()
        {
            using (ECDiffieHellmanCng bob = new ECDiffieHellmanCng())
            {

                bob.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                bob.HashAlgorithm = CngAlgorithm.Sha256;
                bobPublicKey = bob.PublicKey.ToByteArray();
                bobKey = bob.DeriveKeyMaterial(CngKey.Import(Alice.alicePublicKey, CngKeyBlobFormat.EccPublicBlob));

            }
        }

        public void Receive(byte[] encryptedMessage, byte[] iv)
        {

            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = bobKey;
                aes.IV = iv;
                // Decrypt the message
                using (MemoryStream plaintext = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(encryptedMessage, 0, encryptedMessage.Length);
                        cs.Close();
                        string message = Encoding.UTF8.GetString(plaintext.ToArray());
                        Console.WriteLine(message);
                    }
                }
            }
        }

    }
}