using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System.Text;

public class CGameCfg 
{
    /// <summary>
    /// Url基础路径
    /// </summary>
    public string UrlBasePath;

    /// <summary>
    /// 根配置路径
    /// </summary>
    public string RootCfgPath;

    /// <summary>
    /// 是否使用MD5算法加密资源路径，已去除正式发布的资源的表意性。内网开发环境可不加密。
    /// </summary>
    public bool DoPathHash;

    /// <summary>
    /// 游戏版本号
    /// </summary>
    public string VersionNumber;

    public static void Serialize(Stream stream, CGameCfg data)
    {
        XmlSerializer xs = new XmlSerializer(typeof(CGameCfg));
        TextWriter writer = new StreamWriter(stream, Encoding.UTF8);
        xs.Serialize(writer, data);
    }

    public static CGameCfg Deserialize(Stream stream)
    {
        XmlSerializer xs = new XmlSerializer(typeof(CGameCfg));
        return (CGameCfg)xs.Deserialize(stream);
    }
}
