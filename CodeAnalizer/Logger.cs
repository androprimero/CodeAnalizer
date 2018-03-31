using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace CodeAnalizer
{
    public static class Logger
    {
        public static void Log(String Statement)
        {
            Debug.WriteLine(Statement);
        }
        public static void Log(int number)
        {
            Debug.WriteLine(number.ToString());
        }
        public static void Log(Object obj)
        {
            Debug.WriteLine(obj.ToString());
        }
        public static void Log(Exception r)
        {
            Debug.WriteLine(r.ToString() + r.StackTrace);
        }
    }
}
