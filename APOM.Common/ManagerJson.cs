using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace APOM.Common
{
    public static class ManagerJson
    {

        public static void SaveJson(string path, string jsonIN)
        {
            System.IO.File.WriteAllText(path, jsonIN);
        }

        public static string GetJson(Object objectIN)
        {

            MemoryStream strmemo = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Object));
            serializer.WriteObject(strmemo, objectIN);
            string aux = Encoding.Default.GetString(strmemo.ToArray());
            return aux;
        }

        private static T Deserialize<T>(string json)
        {
            var instance = Activator.CreateInstance<T>();
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(instance.GetType());
                return (T)serializer.ReadObject(ms);
            }
        }

        public static string SerializeObject<T>(this T objectIN)
        {
            using (var ms = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(objectIN.GetType());
                serializer.WriteObject(ms, objectIN);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public static Stream SerializeObjectStream<T>(this T objectRequest)
        {
            using (var ms = new MemoryStream())
            {
                DataContractJsonSerializer serializerGen = new DataContractJsonSerializer(objectRequest.GetType());
                serializerGen.WriteObject(ms, objectRequest);
                return ms;
            }
        }


        public static Stream sendGenericPOST<T>(string strEndPointIN, T objectRequest)
        {
            using (var client = new WebClient())
            {
                using (var ms = new MemoryStream())
                {
                    DataContractJsonSerializer serializerGen = new DataContractJsonSerializer(objectRequest.GetType());
                    serializerGen.WriteObject(ms, objectRequest);
                    client.Headers["Content-type"] = "application/json";
                    client.Credentials = CredentialCache.DefaultCredentials;
                    client.UseDefaultCredentials = true;
                    byte[] byteResult = client.UploadData(strEndPointIN, "POST", ms.ToArray());
                    Stream objectRsponse = new MemoryStream(byteResult);
                    return objectRsponse;
                }
            }
        }

        public static Stream sendPOST(string strEndPointIN, MemoryStream objectRequest)
        {
            using (var client = new WebClient())
            {
                client.Headers["Content-type"] = "application/json";
                client.Credentials = CredentialCache.DefaultCredentials;
                client.UseDefaultCredentials = true;
                byte[] byteResult = client.UploadData(strEndPointIN, "POST", objectRequest.ToArray());
                Stream objectRsponse = new MemoryStream(byteResult);
                return objectRsponse;
            }
        }

        public static Stream sendGET(string strEndPointIN)
        {
            using (var client = new WebClient())
            {
                client.Headers["Content-type"] = "application/json";
                client.Credentials = CredentialCache.DefaultCredentials;
                client.UseDefaultCredentials = true;
                byte[] byteResult = client.DownloadData(strEndPointIN);
                Stream objectRsponse = new MemoryStream(byteResult);
                return objectRsponse;
            }
        }

        public static T DeserializeStream<T>(Stream json)
        {
            var instance = Activator.CreateInstance<T>();
            var serializer = new DataContractJsonSerializer(instance.GetType());
            return (T)serializer.ReadObject(json);
        }
    }
}
