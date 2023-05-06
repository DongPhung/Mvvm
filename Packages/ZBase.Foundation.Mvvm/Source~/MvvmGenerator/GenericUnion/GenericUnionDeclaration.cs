﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using ZBase.Foundation.SourceGen;

namespace ZBase.Foundation.Mvvm.GenericUnionSourceGen
{
    public partial class GenericUnionDeclaration
    {
        public const string INTERFACE = "global::ZBase.Foundation.Mvvm.Unions.IUnion<";

        public ImmutableArray<StructRef> ValueTypeRefs { get; }

        public ImmutableArray<StructRef> RefTypeRefs { get; }

        public ImmutableArray<StructRef> Redundants { get; }

        public GenericUnionDeclaration(
              ImmutableArray<StructRef> candidates
            , Compilation compilation
            , CancellationToken token
        )
        {
            using var redundants = ImmutableArrayBuilder<StructRef>.Rent();
            var valueTypeFiltered = new Dictionary<string, StructRef>();
            var refTypeFiltered = new Dictionary<string, StructRef>();

            foreach (var candidate in candidates)
            {
                var syntaxTree = candidate.Syntax.SyntaxTree;
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                candidate.Symbol = semanticModel.GetDeclaredSymbol(candidate.Syntax, token);

                var typeName = candidate.TypeArgument.ToFullName();

                if (typeName.ToUnionType().IsNativeUnionType())
                {
                    redundants.Add(candidate);
                    continue;
                }

                if (candidate.TypeArgument.IsValueType)
                {
                    if (valueTypeFiltered.ContainsKey(typeName) == false)
                    {
                        valueTypeFiltered[typeName] = candidate;
                    }
                    else
                    {
                        redundants.Add(candidate);
                    }
                }
                else
                {
                    if (refTypeFiltered.ContainsKey(typeName) == false)
                    {
                        refTypeFiltered[typeName] = candidate;
                    }
                    else
                    {
                        redundants.Add(candidate);
                    }
                }
            }

            using var valueTypeRefs = ImmutableArrayBuilder<StructRef>.Rent();
            using var refTypeRefs = ImmutableArrayBuilder<StructRef>.Rent();
            
            valueTypeRefs.AddRange(valueTypeFiltered.Values);
            refTypeRefs.AddRange(refTypeFiltered.Values);

            ValueTypeRefs = valueTypeRefs.ToImmutable();
            RefTypeRefs = refTypeRefs.ToImmutable();
            Redundants = redundants.ToImmutable();
        }
    }

    public class StructRef
    {
        public StructDeclarationSyntax Syntax { get; set; }

        public ITypeSymbol Symbol { get; set; }

        public ITypeSymbol TypeArgument { get; set; }
    }
}
