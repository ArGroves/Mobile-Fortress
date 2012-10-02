using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace MobileFortressServer.Messages
{
    enum ConnectMsgType { WrongPassword = 0, NewUserCreated = 1, LoginSuccess = 2 }
    struct ConnectionMessage
    {
        static byte[] EncryptionKey = Encoding.UTF8.GetBytes("SUPAH ENCRYPTING POWAHS!");

        public string Username;
        public string Password;
        public ConnectionMessage(NetIncomingMessage msg)
        {
            string encryptedUsername = msg.ReadString();
            string encryptedPassword = msg.ReadString();
            byte[] xUsername = Xor(Encoding.UTF8.GetBytes(encryptedUsername), EncryptionKey);
            byte[] xPassword = Xor(Encoding.UTF8.GetBytes(encryptedPassword), EncryptionKey);
            Username = Encoding.UTF8.GetChars(xUsername).ToString();
            Password = Encoding.UTF8.GetChars(xPassword).ToString();
        }
        public ConnectionMessage(NetOutgoingMessage msg, ConnectMsgType type)
        {
            msg.Write((byte)type);
            Username = null;
            Password = null;
        }

        static byte[] Xor(byte[] plainText, byte[] key)
        {
            byte[] output = new byte[plainText.Length];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = (byte)(plainText[i] ^ key[i % key.Length]);
            }
            return output;
        }
    }
}
