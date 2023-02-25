using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NodeEditor
{

    [InitializeOnLoad]
    public class LoadUpdate
    {
        private static void DfsUpdate(GameObject gameObject)
        {
            var x = gameObject.GetComponents<ObjectManager>();
            for (int i = 0; i < x.Length; i++)
            {
                //Debug.Log(x[i].name);
                x[i].Init();
            }
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                DfsUpdate(gameObject.transform.GetChild(i).gameObject);
            }
        }
        public static void InitAlllObjects()
        {
            List<GameObject> roots = new List<GameObject>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded)
                    continue;
                //roots.AddRange(scene.GetRootGameObjects());
                var gams = scene.GetRootGameObjects();
                for (int j = 0; j < gams.Length; j++)
                {
                    DfsUpdate(gams[j]);
                }
            }
        }
        public static void InitAssetObject()
        {
            var faobj = AssetDatabase.FindAssets("t:" + typeof(ObjectStruct).Name).Select(guid => AssetDatabase.LoadAssetAtPath<ObjectStruct>(AssetDatabase.GUIDToAssetPath(guid))).ToList();
            foreach (var f in faobj)
            {
                f.Init();
            }
        }
        static LoadUpdate()
        {
            //对各个Objectstruct进行初始化
            Debug.Log("Init ObjectStruct");
            InitAssetObject();
            InitAlllObjects();
        }
    }
}