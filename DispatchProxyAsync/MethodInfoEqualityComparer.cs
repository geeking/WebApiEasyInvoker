using System;
using System.Collections.Generic;
using System.Reflection;

namespace DispatchProxyAsync
{
    internal sealed class MethodInfoEqualityComparer : EqualityComparer<MethodInfo>
    {
        public static readonly MethodInfoEqualityComparer Instance = new MethodInfoEqualityComparer();

        private MethodInfoEqualityComparer()
        {
        }

        public sealed override bool Equals(MethodInfo left, MethodInfo right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left == null)
                return right == null;
            else if (right == null)
                return false;

            // This assembly should work in netstandard1.3,
            // so we cannot use MemberInfo.MetadataToken here.
            // Therefore, it compares honestly referring ECMA-335 I.8.6.1.6 Signature Matching.
            if (!Equals(left.DeclaringType, right.DeclaringType))
                return false;

            if (!Equals(left.ReturnType, right.ReturnType))
                return false;

            if (left.CallingConvention != right.CallingConvention)
                return false;

            if (left.IsStatic != right.IsStatic)
                return false;

            if (left.Name != right.Name)
                return false;

            Type[] leftGenericParameters = left.GetGenericArguments();
            Type[] rightGenericParameters = right.GetGenericArguments();
            if (leftGenericParameters.Length != rightGenericParameters.Length)
                return false;

            for (int i = 0; i < leftGenericParameters.Length; i++)
            {
                if (!Equals(leftGenericParameters[i], rightGenericParameters[i]))
                    return false;
            }

            ParameterInfo[] leftParameters = left.GetParameters();
            ParameterInfo[] rightParameters = right.GetParameters();
            if (leftParameters.Length != rightParameters.Length)
                return false;

            for (int i = 0; i < leftParameters.Length; i++)
            {
                if (!Equals(leftParameters[i].ParameterType, rightParameters[i].ParameterType))
                    return false;
            }

            return true;
        }

        public sealed override int GetHashCode(MethodInfo obj)
        {
            if (obj == null)
                return 0;

            int hashCode = obj.DeclaringType.GetHashCode();
            hashCode ^= obj.Name.GetHashCode();
            foreach (ParameterInfo parameter in obj.GetParameters())
            {
                hashCode ^= parameter.ParameterType.GetHashCode();
            }

            return hashCode;
        }
    }
}