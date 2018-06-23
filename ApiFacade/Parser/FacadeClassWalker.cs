﻿using System;
using System.Collections.Generic;
using System.Linq;
using ApiFacade.Writer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ApiFacade.Parser
{
    public class FacadeClassWalker : CSharpSyntaxWalker
    {
        public int ClassCount => ClassNames.Count;
        public List<string> Usings { get; }
        public List<string> ClassNames { get; }
        public List<string> DeclaredNamespaces { get; }
        public List<string[]> ClassModifers { get; }
        public List<FacadeMethod> Methods { get; }

        public FacadeClassWalker()
        {
            ClassNames = new List<string>();
            ClassModifers = new List<string[]>();
            DeclaredNamespaces = new List<string>();
            Usings = new List<string>();
            Methods = new List<FacadeMethod>();
        }

        public override void VisitUsingDirective(UsingDirectiveSyntax Node)
        {
            base.VisitUsingDirective(Node);
            Usings.Add(Node.Name.ToString());
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax Node)
        {
            base.VisitClassDeclaration(Node);
            ClassNames.Add(Node.Identifier.ToString());
            ClassModifers.Add(Node.Modifiers.Select(M => M.ToString()).ToArray());
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax Node)
        {
            base.VisitNamespaceDeclaration(Node);
            DeclaredNamespaces.Add(Node.Name.ToString());
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax Node)
        {
            base.VisitMethodDeclaration(Node);
            if(Node.Modifiers.Any(M => M.ToString() == "private")) return;

            var parameters = new List<FacadeParameter>();
            for (var i = 0; i < Node.ParameterList.Parameters.Count; i++)
            {
                parameters.Add(new FacadeParameter
                {
                    Type = Node.ParameterList.Parameters[i].Type.ToString(),
                    Name = Node.ParameterList.Parameters[i].Identifier.ToString()
                });
            }
            Methods.Add(new FacadeMethod
            {
                Parameters = parameters.ToArray(),
                Name = Node.Identifier.ToString(),
                ReturnType = Node.ReturnType.ToString(),
                Type = ParseType(Node.Modifiers),
                Modifier = ParseModifier(Node.Modifiers)
            });
        }

        private static MethodModifierType ParseModifier(SyntaxTokenList Modifiers)
        {
            return Modifiers.Any(M => M.ToString().ToLowerInvariant() == "public") ? MethodModifierType.Public
                : Modifiers.Any(M => M.ToString().ToLowerInvariant() == "protected") ? MethodModifierType.Protected : MethodModifierType.Public;
        }

        private static MethodWriterType ParseType(SyntaxTokenList Modifiers)
        {
            return Modifiers.Any(M => M.ToString().ToLowerInvariant() == "abstract") ? MethodWriterType.Abstract
            : Modifiers.Any(M => M.ToString().ToLowerInvariant() == "virtual") ? MethodWriterType.Virtual
            : Modifiers.Any(M => M.ToString().ToLowerInvariant() == "static") ? MethodWriterType.Static : MethodWriterType.Normal;
        }
    }
}
