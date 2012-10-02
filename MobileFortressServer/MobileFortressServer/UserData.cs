using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Lidgren.Network;

namespace MobileFortressServer
{
    class UserData
    {
        static Dictionary<string, UserData> table = new Dictionary<string, UserData>();

        static List<long> IDBans = new List<long>();

        public static void Add(string username, string password)
        {
            if (username.Length < 3 || password.Length < 6)
                throw new ApplicationException("Username or password too short.");
            byte[] userbytes = Encoding.UTF8.GetBytes(username);
            int salt = userbytes[1] * userbytes[1] + userbytes[2] * userbytes[2] + userbytes[3] * userbytes[3];
            string salted_password = new StringBuilder(username).Append(password).Append(salt).ToString();
            byte[] hash = HashStr(salted_password);

            table.Add(username, new UserData(hash));
        }

        public static bool Check(string username, string password)
        {
            byte[] userbytes = Encoding.UTF8.GetBytes(username);
            int salt = userbytes[1] * userbytes[1] + userbytes[2] * userbytes[2] + userbytes[3] * userbytes[3];
            string salted_password = new StringBuilder(username).Append(password).Append(salt).ToString();
            byte[] hash = HashStr(salted_password);

            return ConfirmPassword(hash, table[username].Hash);
        }

        public static bool UserExists(string username)
        {
            return table.ContainsKey(username);
        }

        public static bool IsBanned(string username, long uID)
        {
            if (table[username].Banned) return true;
            if (IDBans.Contains(uID)) return true;
            return false;
        }

        public static void BanUser(string username, NetConnection connection)
        {
            IDBans.Add(connection.RemoteUniqueIdentifier);
            table[username].Banned = true;
        }

        static byte[] HashStr(string value)
        {
            byte[] valuebytes = Encoding.UTF8.GetBytes(value);
            return new SHA256Managed().ComputeHash(valuebytes);
        }
        static bool ConfirmPassword(byte[] newHash, byte[] oldHash)
        {
            return newHash.SequenceEqual(oldHash);
        }

        public byte[] Hash;
        public bool Banned = false;

        public UserData(byte[] hash)
        {
            Hash = hash;
        }


    }
}
