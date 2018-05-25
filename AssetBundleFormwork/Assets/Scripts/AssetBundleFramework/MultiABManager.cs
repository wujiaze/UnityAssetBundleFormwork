/*
 * 
 *      Title:  "AssetBundle简单框架" 项目
 *              主流程（第三层）：（一个场景中）多个 AB 包管理
 *      Description:
 *              功能:
 *                  1、获取AB包之间的依赖和引用关系
 *                  2、管理AB包之间的自动连锁（递归）加载机制
 *      Author:  wujiaze
 *      Date:    2018.5.3
 *      Modify:
 *           
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AssetBundleFormWork
{
#pragma warning disable 414
    public class MultiABManager
    {
        // 引用类 “单个AB包加载实现类”
        private SingleABLoader _CurrentSingleABLoader;

        // "AB包实现类"缓存集合（作用：缓存AB包，防止重复加载）
        private Dictionary<string, SingleABLoader> _DicSingleABLoaderCache;
        public Dictionary<string, SingleABLoader> DicSingleABLoaderCache { get { return _DicSingleABLoaderCache; } }
        // 当前场景（调试使用）
        private string _CurrentSceneName;

        // 当前 AB 名称
        private string _CurrentABName;

        // AB 包与其对应依赖引用关系集合
        private Dictionary<string, ABRelation> _DicABRelation;

        /// <summary>
        /// 委托：所有AB包加载完成
        /// </summary>
        private DelLoadComplete _loadAllABPackageCompleteHandle;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="abName">AB包名臣</param>
        /// <param name="LoadAllABPackageCompleteHandle">委托：是否调用完成</param>
        public MultiABManager(string sceneName)
        {
            _CurrentSceneName = sceneName;
            _DicSingleABLoaderCache=new Dictionary<string, SingleABLoader>();
            _DicABRelation =new Dictionary<string, ABRelation>();
        }

        /// <summary>
        /// 设置当前的AB包名和处理委托
        /// </summary>
        /// <param name="abName"></param>
        public void SetCurrentABNameAndHandler(string abName, DelLoadComplete LoadAllABPackageCompleteHandle)
        {
            _loadAllABPackageCompleteHandle = null;
            _loadAllABPackageCompleteHandle = LoadAllABPackageCompleteHandle;
            _CurrentABName = abName;
        }

        /// <summary>
        /// 加载 AB 包
        /// </summary>
        /// <param name="abName">AB包名称</param>
        /// <returns></returns>
        public IEnumerator LoadAssetBundle(string abName)
        {

            // AB包关系的建立
            if (!_DicABRelation.ContainsKey(abName))
            {
                ABRelation tmpABrelation = new ABRelation(abName);
                _DicABRelation.Add(abName, tmpABrelation);
            }
            ABRelation abRelation = _DicABRelation[abName];
            // 得到指定AB包的依赖引用关系
            string[] strDenendence = ABManifestLoader.GetInstance().RetrivalDependce(abName);
            // 相同的包名在添加 依赖和引用项时 内部会做一个判断：查看是否添加过，添加过就不再添加了
            foreach (string item in strDenendence)
            {
                // 添加 “依赖项”
                abRelation.AddDenpendce(item);
                // 添加 “引用项”
                yield return LoadReference(item, abName);
            }
            //加载AB包
            if (!_DicSingleABLoaderCache.ContainsKey(abName))
            {
                _CurrentSingleABLoader = new SingleABLoader(abName, CompleteLoadAB);
                _DicSingleABLoaderCache.Add(abName, _CurrentSingleABLoader);
                yield return _DicSingleABLoaderCache[abName].LoadAssetBundle();
            }
        }

        /// <summary>
        /// 添加引用项 
        /// 作用：当两个包相互依赖时，光靠依赖关系，无法终止遍历，
        /// 这里通过增加引用关系，来终止死循环。
        /// 例如：A、B两包， A包先加载，添加B包为依赖项，B包将A包添加为引用项，然后加载B包
        /// 此时，B包添加A包为依赖项,并且A包添加B包为引用项，由于A包已经添加过引用关系，所以不再加载B包，从而避免了循环
        /// 总结：实际上，只需要一个有包名的列表就可以了，加载过的包名不在加载，就可以了，这里的引用的依赖包关系，没有用到
        /// </summary>
        private IEnumerator LoadReference(string abName,string refAbName)
        {
            // 表示： 该 AB 包的引用包，已经加载过了，就不在加载，只需要添加到引用关系中
            if (_DicABRelation.ContainsKey(abName))
            {
                ABRelation tmpABRelation = _DicABRelation[abName];
                // 添加 AB 包的引用关系
                tmpABRelation.AddReference(refAbName);
            }
            else
            {
                ABRelation tmpABRelation = new ABRelation(abName);
                tmpABRelation.AddReference(refAbName);
                _DicABRelation.Add(abName, tmpABRelation);
                yield return LoadAssetBundle(abName);
            }
        }


        /// <summary>
        /// 加载指定AB包完成的委托
        /// </summary>
        /// <param name="abName">AB包名</param>
        private void CompleteLoadAB(string abName)
        {
            if (abName.Equals(_CurrentABName)) // 每一个AB包加载完成，就会调用这个委托，只有当指定的AB包加载完成(即依赖包都加载完成)，才会执行最后的委托
            {
                if (_loadAllABPackageCompleteHandle != null)
                {
                    _loadAllABPackageCompleteHandle(abName); 
                }
            }
        }


        /// <summary>
        /// 加载AB包中的资源,通过遍历
        /// Resources加载实质也是通过遍历
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="isCache"></param>
        /// <returns></returns>
        public UnityEngine.Object LoadAsset(string abName, string assetName, bool isCache)
        {
            foreach (var item in _DicSingleABLoaderCache)
            {
                if (item.Key == abName)
                {
                    return item.Value.LoadAsset(assetName, isCache);
                }
            }
            Debug.LogError(GetType()+ "/MyLoadAsset（） 找不到 AB 包 abName");
            return null;
        }

        /// <summary>
        /// 加载AB包中的资源(泛型)
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <param name="isCache"></param>
        /// <returns></returns>
        public T LoadAsset<T>(string abName, string assetName, bool isCache) where T : UnityEngine.Object
        {
            foreach (var item in _DicSingleABLoaderCache)
            {
                if (item.Key == abName)
                {
                    return item.Value.LoadAsset<T>(assetName, isCache);
                }
            }
            Debug.LogError(GetType() + "/MyLoadAsset（） 找不到 AB 包 abName");
            return default(T);
        }

        /// <summary>
        /// 释放本场景中所有的资源
        /// 注意点：用于场景之间的切换
        /// </summary>
        public void DisposeAllAsset()
        {
            try
            {
                if(_DicSingleABLoaderCache!=null&& _DicSingleABLoaderCache.Count > 0)
                {
                    foreach (SingleABLoader singleAbLoader in _DicSingleABLoaderCache.Values)
                    {
                        singleAbLoader.DisposeAll();
                    }
                }
            }
            catch {
                Debug.Log(GetType() + "DisposeAllAsset/ 出错");
            }
            finally
            {
                _DicSingleABLoaderCache.Clear();
                _DicSingleABLoaderCache = null;
                // 释放其他的资源
                _DicABRelation.Clear();
                _DicABRelation = null;
                _CurrentSceneName = null;
                _CurrentABName = null;
                _loadAllABPackageCompleteHandle = null;
                _CurrentSingleABLoader = null;
                // 卸载没有用到的资源
                Resources.UnloadUnusedAssets();
                // 强制垃圾收集
                System.GC.Collect();
            }
        }
    }
}
