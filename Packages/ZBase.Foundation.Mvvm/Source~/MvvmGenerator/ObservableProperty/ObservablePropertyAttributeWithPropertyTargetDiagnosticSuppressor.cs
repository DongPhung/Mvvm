﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using ZBase.Foundation.SourceGen;

using static ZBase.Foundation.Mvvm.SuppressionDescriptors;

namespace ZBase.Foundation.Mvvm
{
    /// <summary>
    /// <para>
    /// A diagnostic suppressor to suppress CS0657 warnings for fields with [ObservableProperty] using a [property:] attribute list.
    /// </para>
    /// <para>
    /// That is, this diagnostic suppressor will suppress the following diagnostic:
    /// <code>
    /// public partial class MyViewModel : IObservableObject
    /// {
    ///     [ObservableProperty]
    ///     [property: JsonPropertyName("Name")]
    ///     private string? name;
    /// }
    /// </code>
    /// </para>
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ObservablePropertyAttributeWithPropertyTargetDiagnosticSuppressor : DiagnosticSuppressor
    {
        /// <inheritdoc/>
        public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => ImmutableArray.Create(PropertyAttributeListForObservablePropertyField);

        /// <inheritdoc/>
        public override void ReportSuppressions(SuppressionAnalysisContext context)
        {
            foreach (Diagnostic diagnostic in context.ReportedDiagnostics)
            {
                var syntaxNode = diagnostic.Location.SourceTree?.GetRoot(context.CancellationToken).FindNode(diagnostic.Location.SourceSpan);

                // Check that the target is effectively [property:] over a field declaration with at least one variable, which is the only case we are interested in
                if (syntaxNode is AttributeTargetSpecifierSyntax attributeTarget
                    && attributeTarget.Parent.Parent is FieldDeclarationSyntax fieldDeclaration
                    && fieldDeclaration.Declaration.Variables.Count > 0
                    && attributeTarget.Identifier.IsKind(SyntaxKind.PropertyKeyword)
                )
                {
                    var semanticModel = context.GetSemanticModel(syntaxNode.SyntaxTree);

                    // Get the field symbol from the first variable declaration
                    var declaredSymbol = semanticModel.GetDeclaredSymbol(fieldDeclaration.Declaration.Variables[0], context.CancellationToken);

                    // Check if the field is using [ObservableProperty], in which case we should suppress the warning
                    if (declaredSymbol is IFieldSymbol fieldSymbol
                        && semanticModel.Compilation.GetTypeByMetadataName("ZBase.Foundation.Mvvm.ComponentModel.ObservablePropertyAttribute") is INamedTypeSymbol observablePropertySymbol
                        && fieldSymbol.HasAttributeWithType(observablePropertySymbol)
                    )
                    {
                        context.ReportSuppression(Suppression.Create(PropertyAttributeListForObservablePropertyField, diagnostic));
                    }
                }
            }
        }
    }
}
