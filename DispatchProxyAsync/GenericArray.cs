﻿using System;
using System.Reflection.Emit;

namespace DispatchProxyAsync
{
    internal class GenericArray<T>
    {
        private ILGenerator _il;
        private LocalBuilder _lb;

        internal GenericArray(ILGenerator il, int len)
        {
            _il = il;
            _lb = il.DeclareLocal(typeof(T[]));

            il.Emit(OpCodes.Ldc_I4, len);
            il.Emit(OpCodes.Newarr, typeof(T));
            il.Emit(OpCodes.Stloc, _lb);
        }

        internal void Load()
        {
            _il.Emit(OpCodes.Ldloc, _lb);
        }

        internal void Get(int i)
        {
            _il.Emit(OpCodes.Ldloc, _lb);
            _il.Emit(OpCodes.Ldc_I4, i);
            _il.Emit(OpCodes.Ldelem_Ref);
        }

        internal void BeginSet(int i)
        {
            _il.Emit(OpCodes.Ldloc, _lb);
            _il.Emit(OpCodes.Ldc_I4, i);
        }

        internal void EndSet(Type stackType)
        {
            BuildUtil.Convert(_il, stackType, typeof(T), false);
            _il.Emit(OpCodes.Stelem_Ref);
        }
    }
}