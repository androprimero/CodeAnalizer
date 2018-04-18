using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using System.Data;
namespace CodeAnalizer
{
    public class DecisionTreeLearning
    {
        private DecisionTree decisions;
        private DataTable dataTable;
        private DecisionVariableCollection variables;
        //private C45Learning C45learningTree; // learning using the algorithm C45
        //private ID3Learning ID3learningTree;
        public DecisionTreeLearning(List<DecisionVariable> decisionVariables)
        {
            dataTable = new DataTable();
            variables = new DecisionVariableCollection(decisionVariables);
          //  C45learningTree = new C45Learning(variables.ToArray());
          //  ID3learningTree = new ID3Learning(variables.ToArray());
        }
        public List<DecisionVariable> ToDecisionVariables(List<String> names, DecisionVariableKind kind)
        {
            List<DecisionVariable> decisionVariables = new List<DecisionVariable>();
            foreach(var name in names)
            {
                DecisionVariable variable = new DecisionVariable(name, kind);
                decisionVariables.Add(variable);
            }
            return decisionVariables;
        }
      public void LoadDataColumns(List<String> columnNames)
        {
            if(dataTable == null) // creates a new instance of data table if there is no other
            {
                dataTable = new DataTable("Data"); 

            }
            dataTable.Columns.Add(columnNames.ToArray()); // Adds the columns titles
        }
        public void AddDataRow(Dictionary<String,int> keyValuePairs)
        {
            List<int> row = new List<int>();
            if (dataTable != null) // if there isn't a table is not possible to add rows
            {
                foreach(var column in dataTable.Columns)
                {
                    if (keyValuePairs.ContainsKey(column.ToString()))
                    {
                        row.Add(keyValuePairs[column.ToString()]); // adding the value to the row
                    }
                    else
                    {
                        row.Add(0); // the column should be at 0
                    }
                }
                dataTable.Rows.Add(row.ToArray());
            }
        }
        public DataTable GetDataTable()
        {
            return dataTable;
        }
    }
}
