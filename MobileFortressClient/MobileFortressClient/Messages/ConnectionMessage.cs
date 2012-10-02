using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace MobileFortressClient.Messages
{
    enum ConnectMsgType { WrongPassword = 0, NewUserCreated = 1, LoginSuccess = 2 }
    struct ConnectionMessage
    {
        static byte[] EncryptionKey = Encoding.UTF8.GetBytes("SUPAH ENCRYPTING POWAHS!");

        public ConnectMsgType SubDataType;

        public ConnectionMessage(NetOutgoingMessage msg, string username, string password)
        {
            SubDataType = ConnectMsgType.LoginSuccess;
            msg.Write((byte)NetMsgType.Login);
            byte[] xUsername = Xor(Encoding.UTF8.GetBytes(username), EncryptionKey);
            byte[] xPassword = Xor(Encoding.UTF8.GetBytes(password), EncryptionKey);
            string encryptedUsername = Encoding.UTF8.GetChars(xUsername).ToString();
            string encryptedPassword = Encoding.UTF8.GetChars(xPassword).ToString();
            msg.Write(encryptedUsername);
            msg.Write(encryptedPassword);
        }
        public ConnectionMessage(NetIncomingMessage msg)
        {
            SubDataType = (ConnectMsgType)msg.ReadByte();
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
