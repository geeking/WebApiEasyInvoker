using System;
using System.Diagnostics;
using System.Reflection.Emit;

namespace DispatchProxyAsync
{
    internal class ParametersArray
    {
        private ILGenerator _il;
        private Type[] _paramTypes;

        internal ParametersArray(ILGenerator il, Type[] paramTypes)
        {
            _il = il;
            _paramTypes = paramTypes;
        }

        internal void Get(int i)
        {
            _il.Emit(OpCodes.Ldarg, i + 1);
        }

        internal void BeginSet(int i)
        {
            _il.Emit(OpCodes.Ldarg, i + 1);
        }

        internal void EndSet(int i, Type stackType)
        {
            Debug.Assert(_paramTypes[i].IsByRef);
            Type argType = _paramTypes[i].GetElementType();
            BuildUtil.Convert(_il, stackType, argType, false);
            BuildUtil.Stind(_il, argType);
        }
    }
}