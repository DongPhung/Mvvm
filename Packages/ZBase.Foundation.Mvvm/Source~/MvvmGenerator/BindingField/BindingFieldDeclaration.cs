﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Threading;
using ZBase.Foundation.SourceGen;

namespace ZBase.Foundation.Mvvm
{
    public partial class BindingFieldDeclaration
    {
        public const string IBINDER_INTERFACE = "global::ZBase.Foundation.Mvvm.IBinder";
        public const string BINDING_ATTRIBUTE = "global::ZBase.Foundation.Mvvm.BindingAttribute";
        public const string UNION_TYPE = "global::ZBase.Foundation.Mvvm.Unions.Union";
        public const string MONO_BEHAVIOUR_TYPE = "global::UnityEngine.MonoBehaviour";

        public ClassDeclarationSyntax Syntax { get; }

        public INamedTypeSymbol Symbol { get; }

        public bool IsBaseBinder { get; }

        public bool ReferenceUnityEngine { get; }

        public List<MemberRef> Members { get; }

        public BindingFieldDeclaration(ClassDeclarationSyntax candidate, SemanticModel semanticModel, CancellationToken token)
        {
            Syntax = candidate;
            Symbol = semanticModel.GetDeclaredSymbol(candidate, token);
            Members = new List<MemberRef>();

            if (Symbol.BaseType != null && Symbol.BaseType.TypeKind == TypeKind.Class)
            {
                if (Symbol.BaseType.ImplementsInterface(IBINDER_INTERFACE))
                {
                    IsBaseBinder = true;
                }
            }

            foreach (var assembly in Symbol.ContainingModule.ReferencedAssemblySymbols)
            {
                if (assembly.ToDisplayString().StartsWith("UnityEngine,"))
                {
                    ReferenceUnityEngine = true;
                    break;
                }
            }

            var members = Symbol.GetMembers();

            foreach (var member in members)
            {
                if (member is not IMethodSymbol method
                    || method.Parameters.Length != 1
                    || method.Parameters[0].Type.ToFullName() != UNION_TYPE
                )
                {
                    continue;
                }

                var attribute = method.GetAttribute(BINDING_ATTRIBUTE);
                
                if (attribute == null)
                {
                    continue;
                }

                var label = string.Empty;

                foreach (var kv in attribute.NamedArguments)
                {
                    if (kv.Key == "Label" && kv.Value.Value is string lbl)
                    {
                        label = lbl;
                    }
                }

                Members.Add(new MemberRef {
                    Member = method,
                    Label = label,
                });
            }
        }

        public class MemberRef
        {
            public IMethodSymbol Member { get; set; }

            public string Label { get; set; }
        }
    }
}