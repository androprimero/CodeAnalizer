using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace CodeAnalizer
{
    public class Configuration
    {
        static String configurationPath;
        List<String> logStatements;
        public Configuration()
        {
            logStatements = new List<String>();
        }
        public void LoadConfigurations(String configurationpath)
        {
            configurationPath = configurationpath;
            try
            {
                String line;
                int i = 0;
                FileStream filestream = new FileStream(configurationpath, FileMode.Open);
                StreamReader reader = new StreamReader(filestream);
                while((line = reader.ReadLine()) != null)
                {
                    logStatements.Add(line);
                    i++;
                }
            }catch(Exception e)
            {
                Logger.Log(e);
            }
        }
        public bool IsLogStatement(SyntaxNode node)
        {
            foreach(var losgStatement in logStatements)
            {
                if (node.ToString().Contains(losgStatement))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
