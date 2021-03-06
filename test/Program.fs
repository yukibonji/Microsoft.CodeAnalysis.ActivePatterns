﻿/////////////////////////////////////////////////////////////////////////////
//
// Microsoft.CodeAnalysis.ActivePatterns - F# Active pattern matching library for Roslyn
// Copyright (c) 2016 Kouji Matsui (@kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
/////////////////////////////////////////////////////////////////////////////

open System
open System.IO
open System.Reflection
open System.Text
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis

[<EntryPoint>]
let main argv =
    let sampleCode =
        use fs = Assembly.GetEntryAssembly().GetManifestResourceStream "Sample.cs"
        let tr = new StreamReader(fs, Encoding.UTF8)
        tr.ReadToEnd()

    let tree = CSharpSyntaxTree.ParseText sampleCode
    let root = tree.GetRoot() :?> CSharpSyntaxNode
        
    match root with
    | CompilationUnitSyntax
        (_, [ UsingDirectiveSyntax(_, _, _, IdentifierNameSyntax(TextToken("System")), _)], _,
         [ NamespaceDeclarationSyntax(_,
            IdentifierNameSyntax(TextToken("SampleNamespace")), _, _, _,
            [ ClassDeclarationSyntax(decl,
                _, TextToken("SampleClass"), _, _, _, _,
                memberDecls,
                _, _)],
            _, _) ],
         _) ->
            memberDecls
            |> Seq.choose (function
              | PropertyDeclarationSyntax(_, typeSyntax, _, TextToken(id), _, _, _, _) ->
                 Some (typeSyntax, id)
              | _ -> None)
            |> Seq.iter (printf "%A")
            
    | _ -> ()
    0

