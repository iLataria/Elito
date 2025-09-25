using JetBrains.Annotations;
using ONiGames.Utilities;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Bunker.Core.Utilities
{
    public class Utils
    {
        public static ValueDropdownList<AnimationStateProperty> GetAnimatorStateItems(Animator animator)
        {
            var values = new ValueDropdownList<AnimationStateProperty>();

#if UNITY_EDITOR
            if (animator == null)
                return values;

            var ac = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            if (ac == null)
            {
                Debug.LogWarning($"{animator.gameObject.GetPath()}: Animation Controller not found");
                return values;
            }

            foreach (var name in ac.GetStateNames())
                values.Add(new AnimationStateProperty { name = name });
#endif

            return values;
        }

        public static T DeserializeFromJson<T>(string json, T def = default)
        {
            return TryDeserializeFromJson<T>(json, out var result) ? result : def;
        }

        public static bool TryDeserializeFromJson<T>(string json, out T result)
        {
            try
            {
                result = JsonUtility.FromJson<T>(json);
                return result != null;
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);

                result = default;
                return false;
            }
        }


        [Serializable]
        public class AnimationStateProperty
        {
            class Cached
            {
                public int hash;
            }

            [NotNull]
            public string name = string.Empty;

            [CanBeNull]
            Cached id;

            public int NameHash(bool forceRehash = false)
            {
                if (id == null || forceRehash)
                    id = new Cached { hash = Animator.StringToHash(name) };
                return id.hash;
            }

            #region ToString

            public override string ToString()
            {
                return $"{name}";
            }

            #endregion
        }
    }
}
