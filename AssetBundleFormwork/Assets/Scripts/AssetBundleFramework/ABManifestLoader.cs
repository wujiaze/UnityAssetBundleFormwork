/*
 * 
 *      Title:  "AssetBundle简单框架" 项目
 *              辅助类 ：读取 AB 依赖关系清单文件。 （主 清单文件）
 *      Description:
 *              功能:
 *              
 *      Author:  wujiaze
 *      Date:    2018.5.3
 *      Modify:
 *           
 */

using System.Collections;
using UnityEngine;
namespace AssetBundleFormWork
{
    public class ABManifestLoader : System.IDisposable
    {
        // 本类实例
        private static ABManifestLoader _Instance = null;
        // AB 清单文件(系统类)
        private AssetBundleManifest _ManifestObj = null;
        // AB 清单文件的路径
        private string _StrManifestPath = null;
        // AB 清单文件的 AB
        private AssetBundle _ABReadManifest = null;
        // 是否加载 Manifest 完成
        private bool _isLoadFinish;

        public bool IsLoadFinish
        {
            get { return _isLoadFinish; }
        }


        private ABManifestLoader()
        {
            // 确定清单文件 www 下载路径
            // 注意点：当生成 AB 包的时候，会生成两个整体的清单文件，命名是跟输出路径的最后“/”后面的保持一致，这里就是 PathTools.GetPlatformName() 的值
            // 这两个文件其实都是清单文件 ，.manifest是给我们看的，另一个 AB 包是给计算机看的，而其他的 AB 文件，则是 清单文件+资源文件
            // 所以这里需要加载的是给电脑看的清单文件 ，路径就是这样的
            _StrManifestPath = PathTools.GetABwwwPath() + "/" + PathTools.GetPlatformName();


        }

        /// <summary>
        /// 获取本类实例
        /// </summary>
        /// <returns></returns>
        public static ABManifestLoader GetInstance()
        {
            if(_Instance==null) {
            _Instance =new ABManifestLoader();
            }
            return _Instance;
        }

        /// <summary>
        /// 加载 Mainfest 清单文件
        /// </summary>
        /// <returns></returns>
        public IEnumerator LoadManifestFile()
        {
            using (WWW www = new WWW(_StrManifestPath))
            {
                yield return www;
                if (www.progress >= 1)
                {
                    // 加载完成 湖区 AB 实例
                    AssetBundle abObj = www.assetBundle;
                    if (abObj != null)
                    {
                        _ABReadManifest = abObj;
                        // 从 主AB 文件中 ，加载 主清单文件，其实也就这一个资源，AssetBundleManifest 是固定常量
                        _ManifestObj = _ABReadManifest.LoadAsset(AbDefine.SYS_ASSETBUNDLE_MANIFEST) as AssetBundleManifest;
                        _isLoadFinish = true;
                    }
                    else
                    {
                        Debug.Log(GetType()+ "/ LoadManifestFile() / www 下载出错 _StrManifestPath"+ _StrManifestPath+"错误信息： "+www.error);
                    }
                }
            }
        }

        /// <summary>
        /// 获取 AssetBundleManifest 系统实例类
        /// </summary>
        /// <returns></returns>
        public AssetBundleManifest GetABManifest()
        {
            if (_isLoadFinish)
            {
                if (_ManifestObj != null)
                {
                    return _ManifestObj;
                }
                else
                {
                    Debug.Log(GetType() + "/GetABManifest()/ _ManifestObj=null");
                }
            }
            else
            {
                Debug.Log(GetType() + "/GetABManifest()/ _isLoadFinish=fasle");
            }
            return null;
        }

        /// <summary>
        /// 获取 AssetBundleManifest 中指定 AB 包的依赖项
        /// </summary>
        /// <param name="abName">指定 AB 包</param>
        /// <returns></returns>
        public string[] RetrivalDependce(string abName)
        {
            if (_ManifestObj!=null && !string.IsNullOrEmpty(abName))
            {
                return _ManifestObj.GetDirectDependencies(abName);
            }
            return null;
        }

        /// <summary>
        /// 释放本类的资源，仅仅是内存镜像
        /// </summary>
        public void Dispose()
        {
            if (_ABReadManifest != null)
            {
                _ABReadManifest.Unload(false);
            }
        }
        /// <summary>
        /// 释放本类的资源,以及内存资源
        /// </summary>
        public void DisposeAll()
        {
            if (_ABReadManifest != null)
            {
                _ABReadManifest.Unload(true);
            }
        }
    }
}
