using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Object = UnityEngine.Object;
using JetBrains.Annotations;
using Bunker.Core.Attributes;
using Bunker.Core.Extensions;

namespace Bunker.Core.Utilities
{
    public static class Assert
    {
        public static void SerializedFields(MonoBehaviour current)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            var fields = current.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.GetCustomAttribute<SerializeField>() == null)
                    continue;

                if (fieldInfo.FieldType == typeof(GameObject))
                {
                    var value = fieldInfo.GetValue(current) as GameObject;

                    var canBeNull = fieldInfo.GetCustomAttribute<CanBeNullAttribute>() != null;
                    var skipSceneCheck = fieldInfo.GetCustomAttribute<SkipSceneCheckAttribute>() != null;
                    var isPrefab = fieldInfo.GetCustomAttribute<PrefabAttribute>() != null;

                    if (!canBeNull && value == null)
                        throw new UnassignedReferenceException($"{current.gameObject.GetPath()}: {fieldInfo.Name} not set");

                    if (!skipSceneCheck && value != null)
                    {
                        if (isPrefab)
                        {
                            if (value.scene != default)
                                throw new UnityException($"{current.gameObject.GetPath()}: {fieldInfo.Name} must be a prefab");
                        }
                        else
                        {
                            if (value.scene == default)
                                throw new UnityException($"{current.gameObject.GetPath()}: {fieldInfo.Name} must be a scene object");
                        }
                    }
                }
                else if (fieldInfo.FieldType == typeof(GameObject[]))
                {
                    var values = fieldInfo.GetValue(current) as GameObject[] ?? new GameObject[0];

                    var canBeNull = fieldInfo.GetCustomAttribute<ItemCanBeNullAttribute>() != null;
                    var skipSceneCheck = fieldInfo.GetCustomAttribute<ItemSkipSceneCheckAttribute>() != null;
                    var isPrefab = fieldInfo.GetCustomAttribute<ItemPrefabAttribute>() != null;

                    for (var i = 0; i < values.Length; i++)
                    {
                        if (!canBeNull && values[i] == null)
                            throw new UnassignedReferenceException($"{current.gameObject.GetPath()}: {fieldInfo.Name} [{i}] not set");

                        if (!skipSceneCheck && values[i] != null)
                        {
                            if (isPrefab)
                            {
                                if (values[i].scene != default)
                                    throw new UnityException($"{current.gameObject.GetPath()}: {fieldInfo.Name} [{i}] must be a prefab");
                            }
                            else
                            {
                                if (values[i].scene == default)
                                    throw new UnityException($"{current.gameObject.GetPath()}: {fieldInfo.Name} [{i}] must be a scene object");
                            }
                        }
                    }
                }
                else if (fieldInfo.FieldType == typeof(List<GameObject>))
                {
                    var values = fieldInfo.GetValue(current) as List<GameObject> ?? new List<GameObject>();

                    var canBeNull = fieldInfo.GetCustomAttribute<ItemCanBeNullAttribute>() != null;
                    var skipSceneCheck = fieldInfo.GetCustomAttribute<ItemSkipSceneCheckAttribute>() != null;
                    var isPrefab = fieldInfo.GetCustomAttribute<ItemPrefabAttribute>() != null;

                    for (var i = 0; i < values.Count; i++)
                    {
                        if (!canBeNull && values[i] == null)
                            throw new UnassignedReferenceException($"{current.gameObject.GetPath()}: {fieldInfo.Name} [{i}] not set");

                        if (!skipSceneCheck && values[i] != null)
                        {
                            if (isPrefab)
                            {
                                if (values[i].scene != default)
                                    throw new UnityException($"{current.gameObject.GetPath()}: {fieldInfo.Name} [{i}] must be a prefab");
                            }
                            else
                            {
                                if (values[i].scene == default)
                                    throw new UnityException($"{current.gameObject.GetPath()}: {fieldInfo.Name} [{i}] must be a scene object");
                            }
                        }
                    }
                }
                else if (fieldInfo.FieldType.IsSubclassOf(typeof(Component)))
                {
                    var value = fieldInfo.GetValue(current) as Component;

                    var canBeNull = fieldInfo.GetCustomAttribute<CanBeNullAttribute>() != null;
                    var skipSceneCheck = fieldInfo.GetCustomAttribute<SkipSceneCheckAttribute>() != null;
                    var isPrefab = fieldInfo.GetCustomAttribute<PrefabAttribute>() != null;

                    if (!canBeNull && value == null)
                        throw new UnassignedReferenceException($"{current.gameObject.GetPath()}: {fieldInfo.Name} not set");

                    if (!skipSceneCheck && value != null)
                    {
                        if (isPrefab)
                        {
                            if (value.gameObject.scene != default)
                                throw new UnityException($"{current.gameObject.GetPath()}: {fieldInfo.Name} must be a prefab");
                        }
                        else
                        {
                            if (value.gameObject.scene == default)
                                throw new UnityException($"{current.gameObject.GetPath()}: {fieldInfo.Name} must be a scene object");
                        }
                    }
                }
                else if (fieldInfo.FieldType.IsSubclassOf(typeof(Object)))
                {
                    var value = fieldInfo.GetValue(current) as Object;

                    var canBeNull = fieldInfo.GetCustomAttribute<CanBeNullAttribute>() != null;

                    if (!canBeNull && value == null)
                        throw new UnassignedReferenceException($"{current.gameObject.GetPath()}: {fieldInfo.Name} not set");
                }
                else if (fieldInfo.FieldType.IsSubclassOf(typeof(Array)))
                {
                    var values = fieldInfo.GetValue(current) as Object[] ?? new Object[0];

                    var canBeNull = fieldInfo.GetCustomAttribute<ItemCanBeNullAttribute>() != null;
                    var skipSceneCheck = fieldInfo.GetCustomAttribute<ItemSkipSceneCheckAttribute>() != null;
                    var isPrefab = fieldInfo.GetCustomAttribute<ItemPrefabAttribute>() != null;

                    for (var i = 0; i < values.Length; i++)
                    {
                        if (!canBeNull && values[i] == null)
                            throw new UnassignedReferenceException($"{current.gameObject.GetPath()}: {fieldInfo.Name} [{i}] not set");

                        if (!skipSceneCheck && values[i] as Component is var c && c != null)
                        {
                            if (isPrefab)
                            {
                                if (c.gameObject.scene != default)
                                    throw new UnityException($"{current.gameObject.GetPath()}: {fieldInfo.Name} [{i}] must be a prefab");
                            }
                            else
                            {
                                if (c.gameObject.scene == default)
                                    throw new UnityException($"{current.gameObject.GetPath()}: {fieldInfo.Name} [{i}] must be a scene object");
                            }
                        }
                    }
                }
                else if (fieldInfo.FieldType.IsInterfaceOf<IList>())
                {
                    var values = ((IList) fieldInfo.GetValue(current)).OfType<Object>();

                    var canBeNull = fieldInfo.GetCustomAttribute<ItemCanBeNullAttribute>() != null;
                    var skipSceneCheck = fieldInfo.GetCustomAttribute<ItemSkipSceneCheckAttribute>() != null;
                    var isPrefab = fieldInfo.GetCustomAttribute<ItemPrefabAttribute>() != null;

                    foreach (var value in values)
                    {
                        if (!canBeNull && value == null)
                            throw new UnassignedReferenceException($"{current.gameObject.GetPath()}: {fieldInfo.Name} not set");

                        if (!skipSceneCheck && value as Component is var c && c != null)
                        {
                            if (isPrefab)
                            {
                                if (c.gameObject.scene != default)
                                    throw new UnityException($"{current.gameObject.GetPath()}: {fieldInfo.Name} must be a prefab");
                            }
                            else
                            {
                                if (c.gameObject.scene == default)
                                    throw new UnityException($"{current.gameObject.GetPath()}: {fieldInfo.Name} must be a scene object");
                            }
                        }
                    }
                }
            }
#endif
        }
    }
}
