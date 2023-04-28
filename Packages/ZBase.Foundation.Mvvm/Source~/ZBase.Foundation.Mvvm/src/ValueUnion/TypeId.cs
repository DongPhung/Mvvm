﻿using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ZBase.Foundation.Mvvm
{
    public readonly struct TypeId : IEquatable<TypeId>
    {
        public static readonly TypeId Undefined = default;

        private readonly uint _id;

        private TypeId(uint id)
        {
            _id = id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(TypeId other)
            => _id == other._id;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
            => obj is TypeId other && _id == other._id;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
            => _id.GetHashCode();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
            => _id.ToString();

        public Type AsType()
        {
            if (TypeVault.TryGetType(this, out var type))
                return type;

            return TypeVault.UndefinedType;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in TypeId lhs, in TypeId rhs)
            => lhs._id == rhs._id;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in TypeId lhs, in TypeId rhs)
            => lhs._id != rhs._id;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TypeId Of<T>()
        {
            var id = new TypeId(Id<T>.Value);
            TypeVault.Register<T>(id);
            return id;
        }

        private readonly struct UndefinedType { }

        private static class TypeVault
        {
            public static readonly Type UndefinedType = typeof(UndefinedType);

            private static ConcurrentDictionary<TypeId, Type> s_vault = default;

            static TypeVault()
            {
                Init();
            }

#if UNITY_5_3_OR_NEWER
            [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
            private static void Init()
            {
                s_vault = new ConcurrentDictionary<TypeId, Type>();
                s_vault.TryAdd(Undefined, UndefinedType);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Register<T>(TypeId id)
                => s_vault.TryAdd(id, typeof(T));

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool TryGetType(TypeId id, out Type type)
                => s_vault.TryGetValue(id, out type);
        }

        private static class Incrementer
        {
            private static readonly object s_lock = new();
            private static uint s_current = default;

            public static uint Next
            {
                get
                {
                    lock (s_lock)
                    {
                        Increment(ref s_current);
                        return s_current;
                    }
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void Increment(ref uint location)
                => Interlocked.Add(ref Unsafe.As<uint, int>(ref location), 1);
        }

        private static class Id<T>
        {
            private static readonly uint s_value;

            public static uint Value => s_value;

            static Id()
            {
                s_value = Incrementer.Next;

#if UNITY_EDITOR
                UnityEngine.Debug.Log(
                    $"{nameof(TypeId)} {s_value} is assigned to {typeof(T)}.\n" +
                    $"If the value is overflowed, enabling Domain Reloading will reset it."
                );
#endif
            }
        }
    }
}