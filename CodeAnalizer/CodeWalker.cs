using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;

namespace CodeAnalizer
{
    public class CodeWalker : CSharpSyntaxWalker
    {
        SyntaxTree tree;
        SemanticModel model;
        Compilation compilation;
        Configuration configuration;
        int ifstatements;
        int catchclauses;
        int trystatements;
        int loggedifStatements;
        int loggedCatchClauses;
        public Project currentProject{get; set;}
        public CodeWalker(Configuration configuration)
        {
            this.configuration = configuration;
        }
        public void AnalizeProject()
        {
            compilation = currentProject.GetCompilationAsync().Result;
            ifstatements = 0;
            catchclauses = 0;
            trystatements = 0;
            loggedifStatements = 0;
            loggedCatchClauses = 0;
            foreach (var file in currentProject.Documents)
            {
                Console.WriteLine("Source analyzed: " + file.FilePath + "\n");
                tree = file.GetSyntaxTreeAsync().Result;
                model = compilation.GetSemanticModel(tree);
                Visit(tree.GetRoot());
            }
            
        }
        public override void VisitIfStatement(IfStatementSyntax node)
        {
            base.VisitIfStatement(node);
            ifstatements++;
            var block = node.ChildNodes();
            foreach(var statement in block)
            {
                if (configuration.IsLogStatement(statement))
                {
                    loggedifStatements++;
                }
            }
            Console.WriteLine("has an if Statement");
        }
        public override void VisitCatchClause(CatchClauseSyntax node)
        {
            base.VisitCatchClause(node);
            Console.WriteLine("has a catch Clause");
            var block = node.ChildNodes();
            foreach (var statement in block)
            {
                if (configuration.IsLogStatement(statement))
                {
                    loggedCatchClauses++;
                }
            }
            catchclauses++;
        }
        public override void VisitTryStatement(TryStatementSyntax node)
        {
            base.VisitTryStatement(node);
            Console.WriteLine("has a try statement");
            trystatements++;
        }
        public int GetIfStatementsCount()
        {
            return ifstatements;
        }
        public int GetCatchClausesCount()
        {
            return catchclauses;
        }
        public int GetTryStatementsCount()
        {
            return trystatements;
        }
        public int GetLoggedIfStatements()
        {
            return loggedifStatements;
        }
        public int GetLoggedCatchClauses()
        {
            return loggedCatchClauses;
        }
    }
        
}
