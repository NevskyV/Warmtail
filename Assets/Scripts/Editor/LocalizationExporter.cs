using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Editor
{
    public static class LocalizationExporter
    {
        private static string ToolPath = "\"D:\\Programms\\DialogueExporter\\GoogleSheetsExporter.exe\"";
 
        public static void Export(
            string sheetName,
            int dialogueIndex,
            IEnumerable<(string key, string ru)> lines)
        {
            var args = new List<string>
            {
                sheetName,
                $"{sheetName}_{dialogueIndex}_"
            };
            Debug.Log(lines);

            args.AddRange(lines.Select(l =>
                l.key+"\t"+l.ru+"\t=GOOGLETRANSLATE(INDIRECT(ADDRESS(ROW(),COLUMN()-1)),\"ru\",\"en\")"
            ));

            RunTool(args);
        }

        static void RunTool(List<string> args)
        {
            var p = new Process();
            p.StartInfo.FileName = ToolPath;
            p.StartInfo.UseShellExecute = false;

            foreach (var arg in args)
                p.StartInfo.ArgumentList.Add(arg);

            p.Start();
            p.WaitForExit();
        }

    }
}