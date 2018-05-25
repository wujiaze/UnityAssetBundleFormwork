/*
 * 
 *      Title:  "AssetBundle简单框架" 项目
 *              框架内部验证测试
 *      Description:
 *              功能:
 *                  框架整体验证测试 
 *      Author:  wujiaze
 *      Date:    2018.5.3
 *      Modify:
 *           
 */
using UnityEngine;
namespace AssetBundleFormWork
{
    public class TestClass_ABMgr:MonoBehaviour
    {  
        // 场景名称 一般是场景名称，也可以是任意的名称（只需要是二级目录）
        private string _SceneName = "Scene_1";
        // AB包名
        private string _AsserBundleName = "prefabs_1.ab"; // "scene_1/prefabs_1.ab"
        // 资源名称 只需要最后的名称
        private string _AssetName = "Sphere.prefab"; // “Sphere”
        // 测试
        private void Update()
        {
            
            if (Input.GetKeyDown(KeyCode.A))
            {
                // 加载 AB 包 
                StartCoroutine(AssetBundleManager.GetInstance().LoadAssetBundlePack(_SceneName, _AsserBundleName, LoadAllComplete));
            }
            if (Input.GetKeyDown(KeyCode.B))
            {

            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AssetBundleManager.GetInstance().DisposeAllAssets(_SceneName);
            }
        }
        /// <summary>
        /// 回调函数
        /// </summary>
        /// <param name="abName"></param>
        private void LoadAllComplete(string abName)
        {
            Object tmpObj = null;
            // 也可以采用泛型加载
            tmpObj = AssetBundleManager.GetInstance().LoadAsset(_SceneName, abName, _AssetName, false) as Object;
            if (tmpObj != null)
            {
                Instantiate(tmpObj);
            }
            AssetBundleManager.GetInstance().ShowAllABname();

            // 测试泛型加载
            // 测试结果表明：从AB包中加载同一个资源时，指向的是同一个内存地址 
            Sprite sprite1 = AssetBundleManager.GetInstance().LoadAsset<Sprite>(_SceneName, "Textures_1.ab", "Floor", false);
            Debug.Log(sprite1.name);
            sprite1.name = "hhhhh";
            Sprite sprite2 = AssetBundleManager.GetInstance().LoadAsset<Sprite>(_SceneName, "Textures_1.ab", "Floor", false);
            Debug.Log(sprite2.name);
            // 测试说明：采用 缓存机制，可以有效的提高效率
            Sprite sprite3 = AssetBundleManager.GetInstance().LoadAsset<Sprite>(_SceneName, "Textures_1.ab", "WhileFloor", true);
            Debug.Log(sprite3.name);
            sprite3.name = "wwww";
            Sprite sprite4 = AssetBundleManager.GetInstance().LoadAsset<Sprite>(_SceneName, "Textures_1.ab", "WhileFloor", true);
            Debug.Log(sprite4.name);
            

        }
    }
}
