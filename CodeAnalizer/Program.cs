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

namespace CodeAnalizer
{
    class Program
    {
        static void Main(string[] args)
        {
            String solutionPath = args[0];
            String configurationPath = args[1];
            Configuration conf = new Configuration();
            conf.LoadConfigurations(configurationPath);
            Analizer analizer = new Analizer(conf);
            analizer.LoadSolution(solutionPath);
            Console.WriteLine("Projects number: " + analizer.NumberProjects());
            analizer.AnalizeSolution();
            Console.Read();
        }
    }
}
