using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace paydemo.con
{
    internal static class Extensions
    {
        static SnakeCaseNamingStrategy snakeCaseNaming = new SnakeCaseNamingStrategy();
        static CamelCaseNamingStrategy camelCaseNaming = new CamelCaseNamingStrategy();
        /// <summary>
        /// 将 SnakeCaseNaming 转换为 snake_case_naming 格式
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string SnakeCaseNaming(this string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }
            return snakeCaseNaming.GetPropertyName(name, false);
        }
        /// <summary>
        /// 将CaseNaming 转换为 caseNaming 格式
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string CamelCaseNaming(this string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }
            return camelCaseNaming.GetPropertyName(name, false);
        }

        /// <summary>
        /// 将字典组织为 query string
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string ToQueryString<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            if (dict == null || dict.Count == 0)
            {
                return string.Empty;
            }
            return string.Join("&", dict
                .Select(t => $"{t.Key}={(t.Value == null ? "" : System.Net.WebUtility.UrlEncode(t.Value.ToString()))}"));
        }
        /// <summary>
        /// 将dict转换成string 并且不加encode（编码）
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string ToQueryStringWithNoEncode<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            if (dict == null || dict.Count == 0)
            {
                return string.Empty;
            }
            return string.Join("&", dict
                .Select(t => $"{t.Key}={(t.Value == null ? "" : t.Value.ToString())}"));
        }
        public static StringBuilder QueryString(this IDictionary<string, object> datas, bool checkIsNull = true)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var kv in datas)
            {
                if (checkIsNull && kv.Value == null)
                {
                    throw new Exception($"ToQueryString 内部含有值为空的字段!ToQueryString:{kv.Key}");
                }
                if (kv.Key.Equals("sign", StringComparison.OrdinalIgnoreCase) || kv.Key.Equals("key", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                sb.Append($"&{kv.Key}={kv.Value}");
            }
            sb.Remove(0, 1);
            return sb;
        }
        public static bool IsEmpty(this IDictionary<string, object> datas, string key)
        {
            var has = datas.ContainsKey(key);
            if (has)
            {
                var value = datas[key];
                if (value == null)
                {
                    has = false;
                }
                else if (string.IsNullOrWhiteSpace(value.ToString()))
                {
                    has = false;
                }
            }
            return has;
        }
        public static string Md5Sign(this IDictionary<string, object> datas, string key)
        {
            var str = datas.QueryString();
            str.Append($"&key={key}");

            MD5 md5 = MD5.Create();
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(str.ToString()));

            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
        

        public static SortedDictionary<string, TValue> FromXml<TValue>(this string xml)
            where TValue : class
        {
            if (string.IsNullOrEmpty(xml))
            {
                throw new Exception("空的xml不合法!");
            }
            SortedDictionary<string, TValue> data = new SortedDictionary<string, TValue>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            foreach (XmlNode node in doc.FirstChild.ChildNodes)
            {
                if (node is XmlElement)
                {
                    data[node.Name] = node.InnerText as TValue;
                }
            }
            return data;
        }

       
        public static string GetJson(this object obj)
        {
            if (obj == null)
            {
                return "{}";
            }
            return JsonConvert.SerializeObject(obj);
        }
        public static string GetJsonString(this object obj,Action<JsonSerializer> options=null)
        {
            if (obj == null)
            {
                return "{}";
            }
            var jsonSerializer = JsonSerializer.CreateDefault();
            jsonSerializer.ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() };
            jsonSerializer.NullValueHandling = NullValueHandling.Ignore;
            if (options != null)
            {
                options(jsonSerializer);
            }
            StringWriter stringWriter = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
            using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
            {
                jsonTextWriter.Formatting = jsonSerializer.Formatting;
                jsonSerializer.Serialize(jsonTextWriter, obj, null);
            }
            return stringWriter.ToString();
        }

        internal static JsonSerializerSettings DefaultSerializerSettings
        {
            get
            {
                return new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatString = "MM/dd/yyyy HH:mm",
                    ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
                    Formatting = Newtonsoft.Json.Formatting.None,
                    MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
                    MaxDepth = 32,
                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
                    Culture = new System.Globalization.CultureInfo("en-us"),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    SerializationBinder= new DefaultSerializationBinder(),
                   
                };
            }
        }
        public static T GetModel<T>(this string input, Action<JsonSerializerSettings> setting = null)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return default(T);
            }
            input = input.FormatJson();
            var settings = DefaultSerializerSettings;
            settings.ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() };
            if (setting != null)
            {
                setting(settings);
            }
            if (!input.StartsWith("["))
            {
                return JsonConvert.DeserializeObject<T>(input, settings);
            }
            var obj = JsonConvert.DeserializeObject<List<T>>(input, settings);
            if (obj == null||obj.Count==0)
            {
                return default(T);
            }
            return obj.FirstOrDefault();
        }
        private static string FormatJson(this string input)
        {
           var _input = input.Trim('\n', '\r', '\t', ' ');
            if (_input.StartsWith("<html ", StringComparison.OrdinalIgnoreCase) || _input.StartsWith("<!DOCTYPE", StringComparison.Ordinal))
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("html", _input);
                _input = JsonConvert.SerializeObject(dict);
            }
            else if (_input.StartsWith("<"))
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("json", _input);
                _input = JsonConvert.SerializeObject(dict);
            }
            return _input;
        }
       
        public static JContainer ToModel(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new JObject();
            }
            input = input.FormatJson();
            var obj = JsonConvert.DeserializeObject<JContainer>(input, DefaultSerializerSettings);            
            return obj;
        }
        public static T Get<T>(this JContainer container, string path)
        {
            var token = container.SelectToken(path);
            if (token == null)
            {
                return default(T);
            }
            return token.ToObject<T>();
        }
        public static T Get<T>(this JToken container, string path)
        {
            var token = container.SelectToken(path);
            if (token == null)
            {
                return default(T);
            }
            return token.ToObject<T>();
        }
        /// <summary>
        /// Deserialize json string to List&lt;T&gt;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<T> GetModelList<T>(this string input)
        {
           
            if (string.IsNullOrWhiteSpace(input))
            {
                return new List<T>();
            }
            input = input.FormatJson();
            if (!input.StartsWith("["))
            {
                return new List<T> { JsonConvert.DeserializeObject<T>(input, DefaultSerializerSettings) };
            }
            var list = JsonConvert.DeserializeObject<List<T>>(input, DefaultSerializerSettings);
            return list;
        }
    }
}