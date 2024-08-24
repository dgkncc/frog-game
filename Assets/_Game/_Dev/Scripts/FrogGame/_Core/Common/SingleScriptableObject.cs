using UnityEngine;

namespace FrogGame._Core.Common
{
    public class SingleScriptableObject<T> : ScriptableObject where T : SingleScriptableObject<T>
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    T[] assets = Resources.LoadAll<T>("");

                    if (assets == null || assets.Length < 1)
                    {
                        throw new System.Exception("Could not find any singleton scriptable object instance in the resources.");
                    }
                    else if (assets.Length > 1)
                    {
                        Debug.LogWarning("Multiple instances of the singleton scriptable object found in the resources");
                    }
                    instance = assets[0];
                }

                return instance;
            }
        }
    }
}
