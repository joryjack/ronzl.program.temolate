using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace Ronzl.Framework.Utility
{
    public enum SerializationType
    {
        Xml,
        Json,
        DataContract,
        Binary
    }

    /// <summary>
    /// 序列化助手类
    /// </summary>
    [System.Serializable]
    public class SerializationHelper
    {

        private SerializationHelper()
        {
        }

        #region ========== XmlSerializer ==========
        /// <summary>
        /// 序列化，使用标准的XmlSerializer，优先考虑使用。
        /// 不能序列化IDictionary接口.
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="filename">文件路径</param>
        public static void XmlSerialize(object obj, string filename)
        {
            FileStream fs = null;
            // serialize it...
            try
            {
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(fs, obj);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        /// <summary>
        /// 反序列化，使用标准的XmlSerializer，优先考虑使用。
        /// 不能序列化IDictionary接口.
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="filename">文件路径</param>
        /// <returns>type类型的对象实例</returns>
        public static object XmlDeserializeFromFile(Type type, string filename)
        {
            FileStream fs = null;
            try
            {
                // open the stream...
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(type);
                return serializer.Deserialize(fs);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        public static string XmlSerialize(object obj, System.Text.Encoding ecoding)
        {
            if (obj == null)
            {
                return null;
            }
            XmlSerializer ser = new XmlSerializer(obj.GetType());
            MemoryStream stream = new MemoryStream();//制定编码和磁盘文件 
            StreamWriter sWriter = new StreamWriter(stream, ecoding);
            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            //empty namespaces
            xsn.Add(String.Empty, String.Empty);

            ser.Serialize(sWriter, obj, xsn);//序列化 

            string str = ecoding.GetString(stream.ToArray());

            stream.Close();

            return str;

        }

        #region Xml序列化
        /// <summary>
        /// Xml序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string XmlSerialize(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            XmlSerializer ser = new XmlSerializer(obj.GetType());
            StringWriter sWriter = new StringWriter();
            ser.Serialize(sWriter, obj);
            return sWriter.ToString();
        }
        #endregion

        /// <summary>
        /// Xml反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="xmlStr"></param>
        /// <returns></returns>
        public static object XmlDeserialize(Type type, string xmlStr)
        {
            if (xmlStr == null || xmlStr.Trim() == "")
            {
                return null;
            }
            XmlSerializer ser = new XmlSerializer(type);
            StringReader sWriter = new StringReader(xmlStr);
            return ser.Deserialize(sWriter);
        }
        #endregion

        #region ========== DataContractSerializer ==========
        public static string DataContractSerialize(object o)
        {
            if (o == null)
            {
                return null;
            }
            MemoryStream stream = new MemoryStream();
            DataContractSerializer ser = new DataContractSerializer(o.GetType());
            ser.WriteObject(stream, o);

            string ret = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            return ret;
        }

        public static object DataContractDeserialize(Type type, string xmlStr)
        {
            if (xmlStr == null || xmlStr.Trim() == "")
            {
                return null;
            }
            DataContractSerializer ser = new DataContractSerializer(type);
            MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xmlStr));//new StringReader(xmlStr);
            return ser.ReadObject(stream);
        }

        #endregion

        #region ========== BinaryBytes ==========
        /// <summary>
        /// 将对象使用二进制格式序列化成byte数组
        /// </summary>
        /// <param name="obj">待保存的对象</param>
        /// <returns>byte数组</returns>
        public static byte[] SaveToBinaryBytes(object obj)
        {
            //将对象序列化到MemoryStream中
            MemoryStream ms = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            //从MemoryStream中获取获取byte数组
            return ms.ToArray();
        }

        /// <summary>
        /// 将使用二进制格式保存的byte数组反序列化成对象
        /// </summary>
        /// <param name="bytes">byte数组</param>
        /// <returns>对象</returns>
        public static object LoadFromBinaryBytes(byte[] bytes)
        {
            object result = null;
            BinaryFormatter formatter = new BinaryFormatter();
            if (bytes != null)
            {
                MemoryStream ms = new MemoryStream(bytes);
                result = formatter.Deserialize(ms);
            }
            return result;
        }
        #endregion

        #region ========= other ==========
        /// <summary>
        /// 使用BinaryFormatter将对象系列化到MemoryStream中。
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>保存对象的MemoryStream</returns>
        public static MemoryStream SaveToMemoryStream(object obj)
        {
            MemoryStream ms = new MemoryStream();
            BufferedStream stream = new BufferedStream(ms);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            return ms;
        }

        #endregion

        /// <summary>
        /// 将C#数据实体转化为JSON数据
        /// </summary>
        /// <param name="obj">要转化的数据实体</param>
        /// <returns>JSON格式字符串</returns>
        public static string JsonSerialize<T>(T obj)
        {
            return JsonSerialize(obj, Encoding.UTF8);
        }

        /// <summary>
        /// 将C#数据实体转化为JSON数据
        /// </summary>
        /// <param name="obj">要转化的数据实体</param>
        /// <returns>JSON格式字符串</returns>
        public static string JsonSerialize(object obj, Encoding encoding)
        {
            if (obj == null)
            {
                return null;
            }
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, obj);
            stream.Position = 0;

            StreamReader sr = new StreamReader(stream, encoding);
            string resultStr = sr.ReadToEnd();
            sr.Close();
            stream.Close();

            return resultStr;
        }


        /// <summary>
        /// 将JSON数据转化为C#数据实体
        /// </summary>
        /// <param name="json">符合JSON格式的字符串</param>
        /// <returns>T类型的对象</returns>
        public static T JsonDeserialize<T>(string json)
        {
            return (T)JsonDeserialize(typeof(T), json);
        }

        /// <summary>
        /// 将JSON数据转化为C#数据实体
        /// </summary>
        /// <param name="json">符合JSON格式的字符串</param>
        /// <returns>T类型的对象</returns>
        public static object JsonDeserialize(Type type, string json)
        {
            if (json == null)
            {
                return null;
            }
            //将"yyyy-MM-dd HH:mm:ss"格式的字符串转为"//Date(1294499956278+0800)//"格式    
            string dateReg = @"((((1[6-9]|[2-9]\d)\d{2})-(1[02]|0?[13578])-([12]\d|3[01]|0?[1-9]))|(((1[6-9]|[2-9]\d)\d{2})-(1[012]|0?[13456789])-([12]\d|30|0?[1-9]))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(1\d|2[0-8]|0?[1-9]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))\s([01]?\d|2[0-3]):[0-5]?\d:[0-5]?\d";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(dateReg);
            json = reg.Replace(json, matchEvaluator);

            //json 必须为 {name:"value",name:"value"} 的格式(要符合JSON格式要求)
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);

            MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json.ToCharArray()));
            object obj = serializer.ReadObject(ms);
            ms.Close();

            return obj;
        }
        /// <summary>    
        /// 将Json序列化的时间由/Date(1294499956278+0800)转为字符串    
        /// </summary>    
        private static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }

        /// <summary>    
        /// 将时间字符串转为Json时间    
        /// </summary>    
        private static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format("/Date({0}+{1})/", ts.TotalMilliseconds, TimeZoneInfo.Local.Id);
            return result;
        }
    }
}