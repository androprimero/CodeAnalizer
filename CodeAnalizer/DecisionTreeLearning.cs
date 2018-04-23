using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using System.Data;
using Accord.Statistics.Filters;
using Accord.Math;
using System.Linq.Expressions;

namespace CodeAnalizer
{
    public class DecisionTreeLearning
    {
        private DecisionTree decisions;
        private DataTable dataTable;
        private DecisionVariableCollection variables;
        private List<string> columnNames;
        private int[][] inputs;
        private int[] output;
        private int columns;
        private Codification codeBook;
        // private C45Learning C45learningTree; // learning using the algorithm C45
        private ID3Learning ID3learningTree;
        public DecisionTreeLearning()
        {
            columns = 0;
            columnNames = new List<string>();
            dataTable = new DataTable("Data");
        }

        public void InitTree(List<DecisionVariable> decisionVariables)
        {
            variables = new DecisionVariableCollection(decisionVariables);
            // C45learningTree = new C45Learning(variables.ToArray());
            ID3learningTree = new ID3Learning(variables.ToArray());
            // decisions = C45learningTree.Learn(inputs, output);// train the tree
            decisions = ID3learningTree.Learn(inputs, output);
        }

        public List<DecisionVariable> ToDecisionVariables(string[] names, DecisionVariableKind kind)
        {
            List<DecisionVariable> decisionVariables = new List<DecisionVariable>();
            foreach(var name in names)
            {
                DecisionVariable variable = new DecisionVariable(name, kind);
                decisionVariables.Add(variable);
            }
            return decisionVariables;
        }

        public void AddDataRow(string sourcename,MethodStatistics methodStatistics)
        {
             if (dataTable != null) // if there isn't a table is not possible to add rows
             {
                DataRow row = dataTable.NewRow(); // create a new Instance of the row
                //===== Common values
                row[0] = sourcename; // the first columns is sourceFile
                row[1] = methodStatistics.GetMethodName();
                row[2] = methodStatistics.MethodIsLogged();
                row[3] = methodStatistics.MethodHasIf();
                row[4] = methodStatistics.MethodHasTry();
                //=== End Common Values
                foreach (var key in methodStatistics.GetIfKeys())
                {
                    if (!dataTable.Columns.Contains(key))
                    {
                        AddColumn(key);
                    }
                    row[key] = methodStatistics.GetIfValue(key);
                }
                foreach (var key in methodStatistics.GetElseKeys())
                {
                    if (!dataTable.Columns.Contains(key))
                    {
                        AddColumn(key);
                    }
                    row[key] = methodStatistics.GetElseValue(key);
                }
                foreach (var key in methodStatistics.GetTryKeys())
                {
                    if (!dataTable.Columns.Contains(key))
                    {
                        AddColumn(key);
                    }
                    row[key] = methodStatistics.GetTryValue(key);
                }
                foreach (var key in methodStatistics.GetCatchKeys())
                {
                    if (!dataTable.Columns.Contains(key))
                    {
                        AddColumn(key);
                    }
                    row[key] = methodStatistics.GetCatchValue(key);
                }
                dataTable.Rows.Add(row);
             }
        }

        public void AddColumn(String column)
        {
            if (!dataTable.Columns.Contains(column))
            {
                dataTable.Columns.Add(column);
                columnNames.Add(column);
            }
        }

        public DataTable GetDataTable()
        {
            return dataTable;
        }

        public void CreateInputs(string outputName)
        {
            codeBook = new Codification(dataTable);
            DataTable symbols = codeBook.Apply(dataTable);
            string[] variableNames = SubArray(columnNames.ToArray<string>(), 3, columnNames.Count);
#pragma warning disable CS0618 // Type or member is obsolete
            inputs = symbols.ToArray<int>(variableNames);
            output = symbols.ToArray<int>(outputName);
#pragma warning restore CS0618 // Type or member is obsolete
            // check how to create the inputs and outputs to ID3tree (int)
            variables = new DecisionVariableCollection(ToDecisionVariables(variableNames, DecisionVariableKind.Continuous));
            InitTree(variables.ToList<DecisionVariable>());
        }

        private string[] SubArray(string[] data,int index,int length)
        {
            int realLength = length - index;
            string[] result = new string[realLength];
            Array.Copy(data, index, result, 0, realLength);
            return result;
        }

        public String GetExpression()
        {
            return decisions.ToExpression().ToString();
        }
    }
}
