using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System.Text;

public class CRootCfg
{
    public string mResUrlCfgPath;
    public string mResCacheCfgPath;
    public string mTableUrlCfgPath;
    public string mTableCacheCfgPath;
    public string mSongUrlCfgPath;
    public string mSongCacheCfgPath;

    public string mQQServerIp;
    public int mQQServerPort;
    public string mWXServerIp;
    public int mWXServerPort;

    public string mQQTDirServerIp;
    public int mQQTDirServerPort;
    public string mWXTDirServerIp;
    public int mWXTDirServerPort;


    //public ELogLevel mLogLevel = ELogLevel.Debug;

    public static CRootCfg CombineLocalAndWeb(CRootCfg localCfg, CRootCfg webCfg)
    {
        if (webCfg == null && localCfg == null)
            return null;//throw new System.Exception("No RootCfg Found");//todo mainLoading notify 
        else if (webCfg == null)
            return localCfg;
        else if (localCfg == null)
            return webCfg;
        else
        {
            CRootCfg ret = webCfg;

            if (string.IsNullOrEmpty(ret.mResUrlCfgPath))
                ret.mResUrlCfgPath = localCfg.mResUrlCfgPath;
            if (string.IsNullOrEmpty(ret.mResCacheCfgPath))
                ret.mResCacheCfgPath = localCfg.mResCacheCfgPath;

            if (string.IsNullOrEmpty(ret.mTableUrlCfgPath))
                ret.mTableUrlCfgPath = localCfg.mTableUrlCfgPath;
            if (string.IsNullOrEmpty(ret.mTableCacheCfgPath))
                ret.mTableCacheCfgPath = localCfg.mTableCacheCfgPath;

            if (string.IsNullOrEmpty(ret.mSongUrlCfgPath))
                ret.mSongUrlCfgPath = localCfg.mSongUrlCfgPath;
            if (string.IsNullOrEmpty(ret.mSongCacheCfgPath))
                ret.mSongCacheCfgPath = localCfg.mSongCacheCfgPath;

            if (string.IsNullOrEmpty(ret.mQQServerIp))
            {
                ret.mQQServerIp = localCfg.mQQServerIp;
                ret.mQQServerPort = localCfg.mQQServerPort;
            }
            if (string.IsNullOrEmpty(ret.mWXServerIp))
            {
                ret.mWXServerIp = localCfg.mWXServerIp;
                ret.mWXServerPort = localCfg.mWXServerPort;
            }

            if (string.IsNullOrEmpty(ret.mQQTDirServerIp))
            {
                ret.mQQTDirServerIp = localCfg.mQQTDirServerIp;
                ret.mQQTDirServerPort = localCfg.mQQTDirServerPort;
            }

            if (string.IsNullOrEmpty(ret.mWXTDirServerIp))
            {
                ret.mWXTDirServerIp = localCfg.mQQTDirServerIp;
                ret.mWXTDirServerPort = localCfg.mWXTDirServerPort;
            }

            return ret;
        }
    }

    public static void Serialize(Stream stream, CRootCfg data)
    {
        XmlSerializer xs = new XmlSerializer(typeof(CRootCfg));
        TextWriter writer = new StreamWriter(stream, Encoding.UTF8);
        xs.Serialize(writer, data);
    }

    public static CRootCfg Deserialize(Stream stream)
    {
        XmlSerializer xs = new XmlSerializer(typeof(CRootCfg));
        return (CRootCfg)xs.Deserialize(stream);
    }

    public override string ToString()
    {
        using (MemoryStream stream = new MemoryStream())
        {
            Serialize(stream, this);
            return Encoding.UTF8.GetString(stream.GetBuffer());
        }
    }
}
