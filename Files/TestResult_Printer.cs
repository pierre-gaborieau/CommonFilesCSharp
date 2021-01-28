using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class TestResult_Printer
    {
        public static void Print_singleResult(Object result)
        {
            System.Diagnostics.Trace.WriteLine(TestResult_Printer.getEnteteModel(result));
            System.Diagnostics.Trace.WriteLine("============================================================================");

            System.Diagnostics.Trace.WriteLine(TestResult_Printer.getLigneModel(result));
        }

        public static void Print_listResult<Type>(this IEnumerable<Type> result)
        {
            System.Diagnostics.Trace.WriteLine(TestResult_Printer.getEnteteModel(result.First()));
            System.Diagnostics.Trace.WriteLine("============================================================================");
            foreach (var element in result)
            {
                System.Diagnostics.Trace.WriteLine(TestResult_Printer.getLigneModel(element));
            }
        }

        #region private
        private static string getEnteteModel(Object monModel)
        {
            PropertyInfo[] properties = monModel.GetType().GetProperties();

            string retVal = "";

            foreach (PropertyInfo element in properties)
            {
                retVal = retVal + element.Name + "|";
            }

            return retVal;
        }

        private static string getLigneModel(Object monModel)
        {
            PropertyInfo[] properties = monModel.GetType().GetProperties();

            string retVal = "";

            foreach (PropertyInfo element in properties)
            {
                retVal = retVal + monModel.GetType().GetProperty(element.Name).GetValue(monModel, null) + "|";
            }

            return retVal;
        }
        #endregion
    }



}
