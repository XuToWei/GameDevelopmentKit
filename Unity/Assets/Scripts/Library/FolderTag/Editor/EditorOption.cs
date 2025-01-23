using UnityEditor;
using UnityEngine;

namespace FolderTag
{
    public class EditorOption<T>
    {
        private readonly string _key;
        private T _val;
        private bool _loaded;

        public EditorOption(string key, T val)
        {
            _key = key;
            _val = val;
        }

        public T Value
        {
            get
            {
                if (!_loaded)
                {
                    Get();
                    _loaded = true;
                }

                return _val;
            }
            set => Set(value);
        }

        private void Get()
        {
            if (!EditorPrefs.HasKey(_key)) return;
            try
            {
                var type = typeof(T);

                if (type == typeof(bool))
                    _val = (T)(object)EditorPrefs.GetBool(_key);
                if (type == typeof(int))
                    _val = (T)(object)EditorPrefs.GetInt(_key);
                if (type == typeof(float))
                    _val = (T)(object)EditorPrefs.GetFloat(_key);
                if (type == typeof(string))
                    _val = (T)(object)EditorPrefs.GetString(_key);

                _val = JsonUtility.FromJson<T>(EditorPrefs.GetString(_key));
            }
            catch
            {
            }
        }

        private void Set(T val)
        {
            var type = typeof(T);
            _val = val;

            if (type == typeof(bool))
                EditorPrefs.SetBool(_key, _val.Equals(true));
            else if (type == typeof(int))
                EditorPrefs.SetInt(_key, (int)(object)_val);
            else if (type == typeof(string))
                EditorPrefs.SetString(_key, (string)(object)_val);
            else if (type == typeof(float))
                EditorPrefs.SetFloat(_key, (float)(object)_val);
            else
                EditorPrefs.SetString(_key, JsonUtility.ToJson(val));
        }
    }
}