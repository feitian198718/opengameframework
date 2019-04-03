using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

class XMLHelper
{
    public static object DeSerializerObject(string path, Type type)
    {
        object obj = null;
        if (!File.Exists(path))
        {
            return null;
        }

        using (Stream streamFile = new FileStream(path, FileMode.Open))
        {
            if (streamFile == null)
            {
                Debug.LogError("OpenFile Erro");
                return obj;
            }

            try
            {
                if (streamFile != null)
                {
                    XmlSerializer xs = new XmlSerializer(type);
                    obj = xs.Deserialize(streamFile);
                }
            }
            catch (System.Exception ex)
            {
                //Debug.LogError("SerializerObject Erro:" + ex.ToString());
            }
        }
        
        return obj;
    }

    public static object DeSerializerObjectFromAssert(string path, Type type)
    {
        object objRet = null;
        TextAsset textFile = (TextAsset)Resources.Load(path);
        if (textFile == null)
        {
            return null;
        }

        using (MemoryStream stream = new MemoryStream(textFile.bytes))
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(type);
                objRet = xs.Deserialize(stream);
            }
            catch (System.Exception ex)
            {
                //Debug.Log("Deserialize Error:" + ex.ToString());
            }
        }
        Resources.UnloadAsset(textFile);
        return objRet;
    }

    public static void SerializerObject(string path, object obj)
    {
        if (File.Exists(path))
        { // remove exist file to fix unexcept text
            File.Delete(path);
        }

        using (Stream streamFile = new FileStream(path, FileMode.OpenOrCreate))
        {
            if (streamFile == null)
            {
                Debug.LogError("OpenFile Erro");
                return;
            }

            try
            {
                string strDirectory = Path.GetDirectoryName(path);
                if (!Directory.Exists(strDirectory))
                {
                    Directory.CreateDirectory(strDirectory);
                }
                
                XmlSerializer xs = new XmlSerializer(obj.GetType());
                TextWriter writer = new StreamWriter(streamFile, Encoding.UTF8);
                xs.Serialize(writer, obj);
            }
            catch (System.Exception ex)
            {
                //Debug.LogError("DeSerializerObject Erro:" + ex.ToString());
            }
        }
    }

    public static bool XmlSerialize<T>(T obj, string fname, bool append = false)
    {
        if(string.IsNullOrEmpty(Path.GetExtension(fname)))
            fname = fname + ".xml";

        string dir = Path.GetDirectoryName(fname);
        if(!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        StreamWriter writer = new StreamWriter(fname, append);
        var serializer = new XmlSerializer(obj.GetType());
        serializer.Serialize(writer, obj);
        writer.Flush();
        writer.Close();
/*
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
*/
        return true;
    }

    public static T XmlDeserialize<T>(Stream reader)
    {
        var deserializer = new XmlSerializer(typeof(T));
        T dobj = (T)deserializer.Deserialize(reader);
        return dobj;
    }

    /// <summary>
    /// 反序列化 Xml，文件名不含后缀
    /// </summary>
    /// <typeparam name="T">解析返回对象类型</typeparam>
    /// <param name="fname">需解析的文件名，不含后缀</param>
    /// <returns></returns>
    public static T XmlDeserialize<T>(string fname)
    {
        if(string.IsNullOrEmpty(Path.GetExtension(fname)))
            fname = fname + ".xml";

        var reader = new FileStream(fname, FileMode.Open);
        T dobj = XmlDeserialize<T>(reader);
        reader.Close();
        return dobj;
    }
}
