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
        bool blockAlreadyLogged;
        Dictionary<String, int> Stats;
        Dictionary<String,int> tryValues;
        Dictionary<String,int> catchValues;
        Dictionary<String, int> elseValues;
        public Project currentProject{get; set;}
        public CodeWalker(Configuration configuration)
        {
            this.configuration = configuration;
            Stats = new Dictionary<String, int>();
            tryValues = new Dictionary<String, int>();
            catchValues = new Dictionary<String, int>();
        }
        public void AnalizeProject()
        {
            compilation = currentProject.GetCompilationAsync().Result;
            foreach (var file in currentProject.Documents)
            {
                Console.WriteLine("Source analyzed: " + file.FilePath + "\n");
                tree = file.GetSyntaxTreeAsync().Result;
                model = compilation.GetSemanticModel(tree);
                AddStatistic("SyntaxNodes", tree.GetRoot().ChildNodes().Count());
                AddStatistic("LinesOfCode", tree.GetText().Lines.Count);
                Visit(tree.GetRoot());
                blockAlreadyLogged = false;
            }
            
        }
        public override void VisitIfStatement(IfStatementSyntax node)
        {
            base.VisitIfStatement(node);
            AddStatistic("IfStatements");
            var block = node.ChildNodes();
            foreach(var statement in block)
            {
                if (configuration.IsLogStatement(statement))
                {
                    AddStatistic("LoggedIfStatements");
                }
            }
        }
        public override void VisitElseClause(ElseClauseSyntax node)
        {
            base.VisitElseClause(node);
            AddStatistic("ElseStatements");
            var block = node.ChildNodes();
            foreach (var statement in block)
            {
                if (configuration.IsLogStatement(statement))
                {
                    AddStatistic("LoggedElseStatements");
                }
                else
                {
                    AnalyzeKindElse(statement.Kind());
                }
            }
        }
        public override void VisitCatchClause(CatchClauseSyntax node)
        {
            base.VisitCatchClause(node);
            var block = node.ChildNodes();
            var declarationchild = node.Declaration.ChildNodes();
            if(block.Count() == 0) {
                AddStatistic("EmptyCatchClauses");
            }
            else
            {
                foreach (var statement in block)
                {
                    if (configuration.IsLogStatement(statement))
                    {
                        blockAlreadyLogged = true;
                        AddStatistic("LoggedCatchClauses");
                    }
                    else
                    {
                        AnalyzeKindCatch(statement.Kind());
                    }
                }
            }
            if (blockAlreadyLogged)
            {
                foreach (var child in declarationchild)
                {
                    AddCatchValues(child.ToString());
                }
            }
            AddStatistic("CatchClauses");
        }

        public override void VisitTryStatement(TryStatementSyntax node)
        {
            base.VisitTryStatement(node);
            var block = node.ChildNodes();
            foreach (var statement in block)
            {
                if (configuration.IsLogStatement(statement))
                {
                    AddStatistic("LoggedTryStatements");
                    blockAlreadyLogged = true;
                }
                else
                {
                    AnalyzeKindTry(statement.Kind(),statement);
                }
            }
            AddStatistic("TryStatements");
        }
        public override void VisitBlock(BlockSyntax node)
        {
            base.VisitBlock(node);
            AddStatistic("BlockStatements");
            var block = node.ChildNodes();
            foreach (var statement in block)
            {
                if (configuration.IsLogStatement(statement))
                {
                    AddStatistic("Logged Block Statements");
                }
            }
        }
        public int GetStatistic(String Statistic)
        {
            if (Stats.ContainsKey(Statistic))
            {
                return Stats[Statistic];
            }
            else
            {
                return 0;
            }
        }
        private void AnalyzeKindCatch(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.ReturnStatement:
                    if (blockAlreadyLogged)
                    {
                        AddStatistic("LoggedReturnCatchs");
                    }
                    else
                    {
                        AddStatistic("NotLoggedReturnCatchs");
                    }
                    break;
                case SyntaxKind.ThrowStatement:
                    if (blockAlreadyLogged)
                    {
                        AddStatistic("LoggedThrowCatchs");
                    }
                    else
                    {
                        AddStatistic("NotLoggedThrowCatchs");
                    }
                    break;
            }
        }
        private void AnalyzeKindTry(SyntaxKind Kind,SyntaxNode statement)
        {
            switch (Kind)
            {
                case SyntaxKind.ReturnStatement:
                    if (blockAlreadyLogged)
                    {
                        AddStatistic("LoggedReturnTry");
                    }else
                    {
                        AddStatistic("NotLoggedReturnTry");
                    }
                    break;
                case SyntaxKind.ThrowStatement:
                    if (blockAlreadyLogged)
                    {
                        AddStatistic("LoggedThrowTry");
                    }
                    else
                    {
                        AddStatistic("NotLoggedThrowTry");
                    }
                    break;
                case SyntaxKind.IfStatement:
                    int trynumber = GetStatistic("TryStatements");
                    AddTryValues("IfStatements" + trynumber.ToString());
                    if (blockAlreadyLogged)
                    {
                        AddTryValues("LoggedTryIfStatement"+trynumber.ToString());
                    }
                    else
                    {
                        AddTryValues("NotLoggedTryIfStatement"+trynumber.ToString());
                    }
                    break;
                case SyntaxKind.InvocationExpression:
                    Console.WriteLine("Invocation Expresion " + statement.ToString());
                    if (statement.ToString().Contains("Sleep"))
                    {
                        if (blockAlreadyLogged)
                        {
                            AddStatistic("LoggedSleepTry");
                        }
                        else
                        {
                            AddStatistic("NotLoggedSleepTry");
                        }
                    }
                    break;
            }
        }
        private void AnalyzeKindElse(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.ReturnStatement:
                    if (blockAlreadyLogged)
                    {
                        AddStatistic("LoggedReturnElse");
                    }
                    else
                    {
                        AddStatistic("NotLoggedReturnElse");
                    }
                    break;
                case SyntaxKind.ThrowStatement:
                    if (blockAlreadyLogged)
                    {
                        AddStatistic("LoggedThrowElse");
                    }
                    else
                    {
                        AddStatistic("NotLoggedThrowElse");
                    }
                    break;
            }
        }
        private void AddStatistic(String Key)
        {
            if (Stats.ContainsKey(Key))
            {
                Stats[Key]++;
            }
            else
            {
                Stats.Add(Key, 1);
            }
        }
        private void AddStatistic(String Key,int Value)
        {
            if (Stats.ContainsKey(Key))
            {
                Stats[Key] += Value;
            }
            else
            {
                Stats.Add(Key, Value);
            }
        }
        private void AddTryValues(String key)
        {
            if (tryValues.ContainsKey(key))
            {
                tryValues[key]++;
            }
            else
            {
                tryValues.Add(key, 1);
            }
        }
        private void AddTryValues(String key, int Value)
        {
            if (tryValues.ContainsKey(key))
            {
                tryValues[key] += Value;
            }
            else
            {
                tryValues.Add(key, Value);
            }
        }
        private void AddCatchValues(String key)
        {
            if (catchValues.ContainsKey(key))
            {
                catchValues[key]++;
            }
            else
            {
                catchValues.Add(key, 1);
            }
        }
        private void AddCatchValues(String key,int Value)
        {
            if (catchValues.ContainsKey(key))
            {
                catchValues[key] += Value;
            }
            else
            {
                catchValues.Add(key, Value);
            }
        }
        private void AddElseValues(String key, int Value)
        {
            if (elseValues.ContainsKey(key))
            {
                elseValues[key] += Value;
            }
            else
            {
                elseValues.Add(key, Value);
            }
        }
        public List<String> GetCatchKeys()
        {
            return catchValues.Keys.ToList<String>();
        }
        public List<String> GetTryKeys()
        {
            return tryValues.Keys.ToList<String>();
        }
        public List<String> GetElseKeys()
        {
            return elseValues.Keys.ToList<String>();
        }
        public int GetCactchValue(String Key)
        {
            return catchValues[Key];
        }
        public int GetTryValue(String key)
        {
            return tryValues[key];
        }
        public int GetElseValue(String key)
        {
            return elseValues[key];
        }
    }     
}
