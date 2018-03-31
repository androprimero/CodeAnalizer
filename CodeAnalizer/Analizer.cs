using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CodeAnalizer
{
    public class Analizer
    {
        MSBuildWorkspace workspace;
        Solution solution;
        IEnumerable<Project> projects;
        CodeWalker walker;
        Configuration configuration;
        public Analizer(Configuration conf)
        {
            workspace = MSBuildWorkspace.Create();
            configuration = conf;
            walker = new CodeWalker(conf);
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
                walker.currentProject = project;
                walker.AnalizeProject();
                Console.WriteLine("---Results project---\n");
                Console.WriteLine("If Statements  " + walker.GetIfStatementsCount()+"\n");
                Console.WriteLine("Try Statements " + walker.GetTryStatementsCount() + "\n");
                Console.WriteLine("Catch Clauses  " + walker.GetCatchClausesCount() + "\n");
                Console.WriteLine("Logged if Statements " + walker.GetLoggedIfStatements() + "\n");
                Console.WriteLine("Logged CatchClauses " + walker.GetLoggedCatchClauses() + "\n");
                Console.WriteLine("Logged try Statements " + walker.GetLoggedTryStatements() + "\n");

            }
        }
    }
}
