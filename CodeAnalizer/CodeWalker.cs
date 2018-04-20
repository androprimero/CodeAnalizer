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
        bool methodLogged;
        bool blockAlreadyLogged;
        bool ifStatementlogged;
        bool tryStatementlogged;
        bool catchClauselogged;
        bool HasTry;
        bool HasIf;
        Dictionary<String, int> Stats;
        Dictionary<String,int> tryValues;
        Dictionary<String, bool> ifValues;
        Dictionary<String,bool> catchValues;
        Dictionary<String, int> elseValues;
        public Project CurrentProject{get; set;}

        public CodeWalker(Configuration configuration)
        {
            this.configuration = configuration;
            Stats = new Dictionary<String, int>();
            tryValues = new Dictionary<String, int>();
            ifValues = new Dictionary<string, bool>();
            catchValues = new Dictionary<String, bool>();
			elseValues = new Dictionary<string, int>();
        }

        public void AnalizeProject()
        {
            compilation = CurrentProject.GetCompilationAsync().Result;
            foreach (var file in CurrentProject.Documents)
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
        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            base.VisitMethodDeclaration(node);
            methodLogged = false;
            HasIf = false;
            HasTry = false;
            Console.WriteLine("Method " + node.Identifier.ToString());
            BlockSyntax body = node.Body;
            FindLogStatements(body);
            if (methodLogged)
            {
                AddStatistic("MethodLogged");
            }
            AnalizeBlock(body);
        }

        private void FindLogStatements(BlockSyntax block)
        {
            var statements = block.ChildNodes();
            foreach(var statement in statements)
            {
                if (configuration.IsLogStatement(statement))
                {
                    methodLogged = true;
                }
            }
        }
        public void AnalizeBlock(BlockSyntax block)
        {
            var childs = block.ChildNodes();
            foreach(var child in childs)
            {
                switch (child.Kind())
                {
                    case SyntaxKind.IfStatement:
                        IfStatement((IfStatementSyntax)child);
                        break;
                    case SyntaxKind.TryStatement:
                        TryStatement((TryStatementSyntax)child);
                        break;
                }
            }
        }
        public void IfStatement(IfStatementSyntax node)
        {
            HasIf = true;
            ifStatementlogged = false;
            AddStatistic("IfStatements");
            var block = node.ChildNodes();
            var condition = node.Condition;
            if (condition.ToString().Contains("null"))
            {
                AddIfValues("NullValueCondition");
            }
            foreach(var statement in block)
            {
                if (configuration.IsLogStatement(statement))
                {
                    ifStatementlogged = true;
                    AddStatistic("LoggedIfStatements");
                }
                else
                {
                    if (statement.IsKind(SyntaxKind.TryStatement))
                    {
                        TryStatement((TryStatementSyntax)statement);
                    }
                    else
                    {
                        AnalyzeKindIf(statement.Kind());
                    }
                }
            }
            if (node.Else !=null)
            {
                Console.WriteLine("else clause");
                ElseClause(node.Else);
            }
        }

        public void ElseClause(ElseClauseSyntax node)
        {
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

        public void TryStatement(TryStatementSyntax node)
        {
            var block = node.Block.ChildNodes();
            HasTry = true;
            AddStatistic("TryStatements");
            foreach (var statement in block)
            {
                if (configuration.IsLogStatement(statement)) // log statement placed inside try statement
                {
                    AddStatistic("LoggedTryStatements");
                    tryStatementlogged = true;
                }
                else
                {
                    AnalyzeKindTry(statement.Kind(), statement);
                }
            }
            foreach(var catchClause in node.Catches)
            {
                CatchClause(catchClause);
            }
        }

        public void CatchClause(CatchClauseSyntax node)
        {
            var block = node.Block.ChildNodes();
            var declarationchild = node.Declaration.ChildNodes();
            if(block.Count() == 0) { // captures the empty catches
                AddStatistic("EmptyCatchClauses");
                AddCatchValues("EmptyCatch");
            }
            else
            {
                foreach (var statement in block)
                {
                    if (configuration.IsLogStatement(statement))
                    {
                        catchClauselogged =true;
                        AddStatistic("LoggedCatchClauses");
                    }
                    else
                    {
                        AnalyzeKindCatch(statement.Kind());
                    }
                }
            }
            foreach (var child in declarationchild) // captures the exception declarations
            {
                    AddCatchValues(child.ToString(),blockAlreadyLogged);
            }
            AddStatistic("CatchClauses");
        }
        
        private void AnalyzeKindCatch(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.ReturnStatement:
                    AddCatchValues("ReturnCatch",blockAlreadyLogged); 
                    if (catchClauselogged)// to keep general Statistics
                    {
                        AddStatistic("LoggedReturnCatchs");
                    }
                    else
                    {
                        AddStatistic("NotLoggedReturnCatchs");
                    }
                    break;
                case SyntaxKind.ThrowStatement:
                    AddCatchValues("ThrowCacth", blockAlreadyLogged);
                    if (catchClauselogged)
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
            int trynumber = GetStatistic("TryStatements");
            switch (Kind)
            {
                case SyntaxKind.VariableDeclaration:
                    AddTryValues("VariableDeclaration" + trynumber.ToString());
                    break;
                case SyntaxKind.ReturnStatement:
                    if (tryStatementlogged)
                    {
                        AddStatistic("LoggedReturnTry");
                    }else
                    {
                        AddStatistic("NotLoggedReturnTry");
                    }
                    break;
                case SyntaxKind.ThrowStatement:
                    if (tryStatementlogged)
                    {
                        AddStatistic("LoggedThrowTry");
                    }
                    else
                    {
                        AddStatistic("NotLoggedThrowTry");
                    }
                    break;
                case SyntaxKind.IfStatement:
                    IfStatement((IfStatementSyntax)statement);
                    AddTryValues("IfStatements" + trynumber.ToString());
                    if (ifStatementlogged)
                    {
                        AddTryValues("LoggedTryIfStatement" + trynumber.ToString());
                    }
                    else
                    {
                        AddTryValues("NotLoggedTryIfStatement" + trynumber.ToString());
                    }
                    break;
                case SyntaxKind.InvocationExpression:
                    AddTryValues("ExpresionCount" + trynumber.ToString());
                    if (statement.ToString().Contains("Sleep"))
                    {
                        if (tryStatementlogged)
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
        private void AnalyzeKindIf(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.ReturnStatement:
                    AddIfValues("ReturnStatement");
                    break;
                case SyntaxKind.ThrowExpression:
                    AddIfValues("ThrowsStatement");
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
        private List<SyntaxNode> BreakBlock(BlockSyntax block)
        {
            return block.ChildNodes().ToList<SyntaxNode>();
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

        private void AddIfValues(String key)
        {
            if (!ifValues.ContainsKey(key)) // catch exceptions that are logged
            {
                ifValues.Add(key, true);
            }
        }

        private void AddIfValues(String key, bool Value)
        {
            if (!ifValues.ContainsKey(key))
            {
                ifValues.Add(key, Value);
            }
        }

        private void AddCatchValues(String key)
        {
            if (!catchValues.ContainsKey(key)) // catch exceptions that are logged
            {
                catchValues.Add(key, true);
            }
        }

        private void AddCatchValues(String key,bool Value)
        {
            if (!catchValues.ContainsKey(key))
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

        public bool GetCactchValue(String Key)
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

        public List<String> GetStatisticName()
        {
            return Stats.Keys.ToList<String>();
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
    }     
}