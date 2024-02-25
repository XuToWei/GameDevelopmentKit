using System;
using System.Collections;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography;

namespace ThunderFireUITool
{
    //[InitializeOnLoad]
    public static class UXToolAnalysis
    {

        private static string token = "hhrL8wNBCrMz4eJv3UUQKLh7BFxMSqyG";
        public struct CollectData
        {
            public string content;
            public string token;
        }

        public struct UXToolLogData
        {
            public string userId;
            public string ip;
            public string mac;
            public string name;
            public string timeStamp;
            public string funcName;
        }

        private static UXToolLogData GenLogData()
        {
            var data = new UXToolLogData();

            data.ip = GetLocalIPAddress();
            data.mac = GetLocalMacAddress();
            data.userId = CalcUserID(data.ip + data.mac);
            data.timeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            data.name = GetLocalPath();
            return data;
        }

        private static NetworkInterface[] GetNetworkInterfaces()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback && n.OperationalStatus == OperationalStatus.Up)
                .ToArray();
            return networkInterfaces;
        }

        private static string GetLocalIPAddress()
        {
            string ipAddress = "";

            NetworkInterface[] networkInterfaces = GetNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                UnicastIPAddressInformationCollection unicastAddresses = ipProperties.UnicastAddresses;

                foreach (UnicastIPAddressInformation unicastAddress in unicastAddresses)
                {
                    if (unicastAddress.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        ipAddress = unicastAddress.Address.ToString();
                        return ipAddress;
                    }
                }
            }

            return ipAddress;
        }
        private static string GetLocalMacAddress()
        {
            string macAddress = "";

            NetworkInterface[] networkInterfaces = GetNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (networkInterface.Description.ToLower().Contains("virtual") ||
                    networkInterface.Description.ToLower().Contains("pseudo"))
                {
                    continue;
                }

                try
                {
                    macAddress = networkInterface.GetPhysicalAddress().ToString();
                    break;
                }
                catch (System.Exception)
                {
                    continue;
                }
            }
            return macAddress;
        }

        private static long GetLocalTimeStamp()
        {
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - unixEpoch;

            return (long)timeSpan.TotalSeconds;
        }

        private static string GetLocalPath()
        {
            string path = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
            string shortPath = path.Substring(path.LastIndexOf('/') + 1);
            return shortPath;
        }

        public static string GetUserID()
        {
            string ip = GetLocalIPAddress();
            string mac = GetLocalMacAddress();

            string userID = ip + mac;

            userID = CalcUserID(userID);
            return userID;
        }

        private static string CalcUserID(string userID)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(userID);
                byte[] hashBytes = md5.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private static bool CheckLogTimeStamp(string funcName)
        {
            long curTimeStamp = GetLocalTimeStamp();

            string lastTimeStampStr = EditorPrefs.GetString("UX_" + funcName, "0");

            long lastTimeStamp = 0;
            long.TryParse(lastTimeStampStr, out lastTimeStamp);

            return IsAnotherDay(curTimeStamp, lastTimeStamp);
        }

        private static bool IsAnotherDay(long timestamp1, long timestamp2)
        {
            DateTime dateTime1 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(timestamp1)
                .ToLocalTime();

            DateTime dateTime2 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(timestamp2)
                .ToLocalTime();

            if (dateTime1.Date.Equals(dateTime2.Date))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static void SaveLogTimeStamp(string funcName)
        {
            long timeStamp = GetLocalTimeStamp();
            EditorPrefs.SetString("UX_" + funcName, timeStamp.ToString());
        }

        public static void SendUXToolLog(string funcName = "Test1")
        {
            if (!CheckLogTimeStamp(funcName))
            {
                return;
            }
            SaveLogTimeStamp(funcName);


            UXToolLogData logData = GenLogData();
            logData.funcName = funcName;
            string logDataString = JsonUtility.ToJson(logData);
            //Debug.Log(logDataString);

            CollectData collectData = new CollectData();
            collectData.content = logDataString;
            collectData.token = token;
            string dataString = JsonUtility.ToJson(collectData);

            string lang = Application.systemLanguage.ToString();
            string url;

            if (lang.Equals("ChineseSimplified"))
            {
                url = "https://uxtool-collector.ux.leihuo.netease.com/uxtool-collector/api/collect";
            }
            else
            {
                url = "https://uxtool-collector.ux.leihuo.easebar.com/uxtool-collector/api/collect";
            }

            UnityWebRequest www = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(dataString);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            //www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SendWebRequest();
            www.uploadHandler.Dispose();
        }
    }
}