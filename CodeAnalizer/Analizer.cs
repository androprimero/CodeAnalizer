﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Accord.MachineLearning.DecisionTrees;

namespace CodeAnalizer
{
    public class Analizer
    {
        MSBuildWorkspace workspace;
        Solution solution;
        IEnumerable<Project> projects;
        CodeWalker walker;
        Configuration configuration;
        DecisionTreeLearning learningTree;
        DecisionTree tree;
        public Analizer(Configuration conf)
        {
            workspace = MSBuildWorkspace.Create();
            configuration = conf;
            walker = new CodeWalker(conf);
            learningTree = new DecisionTreeLearning();
            learningTree.AddColumn("SourceFile");
            learningTree.AddColumn("MethodName");
            learningTree.AddColumn("MethodLogged");
            learningTree.AddColumn("HasIf");
            learningTree.AddColumn("HasTry");
        }
        public void LoadSolution(String SolutionPath)
        {
            try
            {
                if(workspace == null)
                {
                    workspace = MSBuildWorkspace.Create();
                }
                solution = workspace.OpenSolutionAsync(SolutionPath).Result;
                projects = solution.Projects;
                Logger.Log("projects: " + projects.Count());
            }catch(System.AggregateException e)
            {
                Logger.Log(e);
                throw;
            }
        }
        public int NumberProjects()
        {
            int projets = projects.Count();
            return projets;
        }
        public void AnalizeSolution()
        {
            Logger.Log("Analyzing solution");
            foreach(Project project in projects)
            {
                Logger.Log("Project analyzed: " + project.FilePath);
                walker.CurrentProject = project;
                walker.AnalizeProject();
                foreach(var name in walker.GetStatisticName())
                {
                    Console.WriteLine(name + " : " + walker.GetStatistic(name));
                }
                foreach (var sourcename in walker.GetSourceNames())
                {
                    var statitic = walker.GetMethodStatisticsValue(sourcename);
                    foreach (var methodStats in statitic)
                    {
                        Logger.Log("Source " + sourcename + " Method Name" + methodStats.GetMethodName());
                        learningTree.AddDataRow(sourcename, methodStats);
                    }                    
                }
                learningTree.CreateInputs("MethodName");
                Console.Write(learningTree.GetExpression());
                walker.Clear();
            }
        }
    }
}
