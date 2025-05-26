 
using System;
using System.IO;
using System.Text;

namespace FS.Utils
{


    public static class HexFileHelper
    {
        /// <summary>
        /// 将 byte[] 保存为连续的16进制字符串文件
        /// </summary>
        /// <param name="bytes">要保存的字节数组</param>
        /// <param name="filePath">目标文件路径</param>
        public static void SaveBytesAsHexString(byte[] bytes, string filePath)
        {
            // 转换为连续的16进制字符串（无分隔符）
            string hexString = BitConverter.ToString(bytes);
        
            // 写入文件（UTF-8编码）
            File.WriteAllText(filePath, hexString, Encoding.UTF8);
        }

        /// <summary>
        /// 从16进制字符串文件读取字节数组
        /// </summary>
        public static byte[] ReadBytesFromHexFile(string filePath)
        {
            string hexString = File.ReadAllText(filePath, Encoding.UTF8);
            byte[] bytes = new byte[hexString.Length / 2];
        
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return bytes;
        }
    }
}