using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using System.IO;
namespace CodeAnalizer
{
    class Program
    {
        static void Main(string[] args)
        {
            String solutionPath = args[0];
            String configurationPath = args[1];
            String resultPath;
            String rulesPath;
            string directory = Path.GetDirectoryName(configurationPath);
            Console.WriteLine("Root Path" + directory);
            if (args.Length > 2)
            {
                if (!String.IsNullOrEmpty(args[2]))
                {
                    resultPath = args[2];
                }
                else
                {
                    resultPath = directory + Path.DirectorySeparatorChar + "Report.txt";
                }
            }
            else
            {
                
                resultPath = directory + Path.DirectorySeparatorChar + "Report.txt";
            }
            rulesPath = directory + Path.DirectorySeparatorChar + "Rules.xml";// File that keeps the decision tree
            Configuration conf = new Configuration();
            conf.LoadConfigurations(configurationPath);
            Analizer analizer = new Analizer(conf,rulesPath,resultPath);
            analizer.LoadSolution(solutionPath);
            Console.WriteLine("Projects number: " + analizer.NumberProjects());
            analizer.AnalizeSolution();
            analizer.Close();
            Console.Read();
        }
    }
}
