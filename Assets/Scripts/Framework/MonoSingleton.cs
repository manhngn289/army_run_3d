using UnityEngine;

namespace Framework
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;

        public static T Insance => _instance;

        protected virtual void Awake()
        {
            if(!_instance)
            {
                _instance = this as T;
                DontDestroyOnLoad(_instance);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}