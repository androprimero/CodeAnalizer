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
        bool elseClauselogged;
        bool ifStatementlogged;
        bool tryStatementlogged;
        bool catchClauselogged;
        string documentPath;
        MethodStatistics methodStatistics;
        Dictionary<String, int> Stats;
        Dictionary<String, MethodStatistics> documentStats;
        public Project CurrentProject{get; set;}

        public CodeWalker(Configuration configuration)
        {
            this.configuration = configuration;
            Stats = new Dictionary<String, int>(); // general Statistics
            documentStats = new Dictionary<String, MethodStatistics>(); // Statistics classified by document 
        }

        public void AnalizeProject()
        {
            compilation = CurrentProject.GetCompilationAsync().Result;
            foreach (var file in CurrentProject.Documents)
            {
                documentPath = file.FilePath;
                Console.WriteLine("Source analyzed: " + documentPath + "\n");
                tree = file.GetSyntaxTreeAsync().Result;
                model = compilation.GetSemanticModel(tree);
                AddStatistic("SyntaxNodes", tree.GetRoot().ChildNodes().Count());
                AddStatistic("LinesOfCode", tree.GetText().Lines.Count);
                Visit(tree.GetRoot());
            }
            
        }
        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            base.VisitMethodDeclaration(node);
            Console.WriteLine("Method " + node.Identifier.ToString());
            methodStatistics = new MethodStatistics(node.Identifier.ToString());
            BlockSyntax body = node.Body;
            FindLogStatements(body);
            if (methodStatistics.MethodIsLogged())
            {
                AddStatistic("MethodLogged");
            }
            AnalizeBlock(body);
            documentStats.Add(documentPath, methodStatistics);
        }

        private void FindLogStatements(BlockSyntax block)
        {
            var statements = block.ChildNodes();
            foreach(var statement in statements)
            {
                if (configuration.IsLogStatement(statement))
                {
                    methodStatistics.SetMethodLogged(true);
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
            methodStatistics.SetHasIf(true);
            ifStatementlogged = false;
            AddStatistic("IfStatements");
            var block = node.ChildNodes();
            var condition = node.Condition;
            if (condition.ToString().Contains("null"))
            {
                methodStatistics.AddIfValues("NullValueCondition");
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
                methodStatistics.AddIfValues("HasElse");
                ElseClause(node.Else);
            }
        }

        public void ElseClause(ElseClauseSyntax node)
        {
            AddStatistic("ElseStatements");
            elseClauselogged = false;
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
            methodStatistics.SetHasTry(true);
            tryStatementlogged = false;
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
                methodStatistics.AddTryValues("NumberCatchClauses");
                CatchClause(catchClause);
            }
        }

        public void CatchClause(CatchClauseSyntax node)
        {
            var block = node.Block.ChildNodes();
            var declarationchild = node.Declaration.ChildNodes();
            catchClauselogged = false;
            if(block.Count() == 0) { // captures the empty catches
                AddStatistic("EmptyCatchClauses");
                methodStatistics.AddCatchValues("EmptyCatch");
            }
            else
            {
                foreach (var statement in block)
                {
                    if (configuration.IsLogStatement(statement))
                    {
                        catchClauselogged =true;
                        AddStatistic("LoggedCatchClauses");// general statistic
                    }
                    else
                    {
                        AnalyzeKindCatch(statement.Kind());
                    }
                }
            }
            foreach (var child in declarationchild) // captures the exception declarations
            {
                methodStatistics.AddCatchValues(child.ToString(), catchClauselogged);
            }
            AddStatistic("CatchClauses");// general statistic
        }
        
        private void AnalyzeKindCatch(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.ReturnStatement: // return statement must be the last statement 
                    methodStatistics.AddCatchValues("ReturnCatch", catchClauselogged); 
                    if (catchClauselogged)// to keep general Statistics
                    {
                        AddStatistic("LoggedReturnCatchs");
                    }
                    else
                    {
                        AddStatistic("NotLoggedReturnCatchs");
                    }
                    break;
                case SyntaxKind.ThrowStatement: // throw statement must be the last statement
                    methodStatistics.AddCatchValues("ThrowCacth", catchClauselogged);
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
                    methodStatistics.AddTryValues("VariableDeclaration" + trynumber.ToString());
                    break;
                case SyntaxKind.ReturnStatement:
                    methodStatistics.AddTryValues("ReturnStatement" + trynumber.ToString(), 1);
                    if (tryStatementlogged)
                    {
                        AddStatistic("LoggedReturnTry");
                    }else
                    {
                        AddStatistic("NotLoggedReturnTry");
                    }
                    break;
                case SyntaxKind.ThrowStatement:
                    methodStatistics.AddTryValues("ThrowStatement" + trynumber.ToString(), 1);
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
                    methodStatistics.AddTryValues("IfStatements" + trynumber.ToString());
                    if (ifStatementlogged) // general Statistic
                    {
                        methodStatistics.AddTryValues("LoggedTryIfStatement" + trynumber.ToString());
                    }
                    else
                    {
                        methodStatistics.AddTryValues("NotLoggedTryIfStatement" + trynumber.ToString());
                    }
                    break;
                case SyntaxKind.InvocationExpression:
                    methodStatistics.AddTryValues("ExpresionCount" + trynumber.ToString());
                    if (statement.ToString().Contains("Sleep"))
                    {
                        methodStatistics.AddTryValues("SleepInTry" + trynumber.ToString(), 1);
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
                    methodStatistics.AddIfValues("ReturnStatement");
                    break;
                case SyntaxKind.ThrowExpression:
                    methodStatistics.AddIfValues("ThrowsStatement");
                    break;
            }
        }
        private void AnalyzeKindElse(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.ReturnStatement:
                    methodStatistics.AddElseValues("ReturnElse");
                    if (elseClauselogged)
                    {
                        AddStatistic("LoggedReturnElse");
                    }
                    else
                    {
                        AddStatistic("NotLoggedReturnElse");
                    }
                    break;
                case SyntaxKind.ThrowStatement:
                    methodStatistics.AddElseValues("ThrowElse");
                    methodStatistics.AddElseValues("ThrowElse");
                    if (elseClauselogged)
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
        public List<String> GetMethodsNames()
        {
            return documentStats.Keys.ToList<String>();
        }
        public Tuple<bool, bool, Dictionary<String, bool>, Dictionary<String, bool>, Dictionary<String, int>, Dictionary<String, bool>>
            GetMethodStatisticsValue(String key)
        {
            return documentStats[key];
        }
    }     
}