using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeAnalizer;

namespace MainApp
{
    public class Controller
    {
        private Analizer analizer;
        public Controller()
        {
            analizer = new Analizer();
        }
        public int OpenSolution(String solutionPath)
        {
            try
            {
                analizer.LoadSolution(solutionPath);
                return analizer.NumberProjects();
            }catch(System.AggregateException e)
            {
                Logger.Log(e);
                return -1;
            }
        }
    }
}
