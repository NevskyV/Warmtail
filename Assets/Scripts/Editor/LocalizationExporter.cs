using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Editor
{
    public static class LocalizationExporter
    {
        private static string ToolPath =>
            Path.Combine(
                Application.dataPath,
                "Tools/SheetsTool/SheetsTool.exe"
            );
        public static void Export(
            string sheetName,
            int dialogueIndex,
            IEnumerable<(string key, string ru)> lines)
        {
            var args = new List<string>
            {
                sheetName,
                $"{sheetName}_{dialogueIndex}_",
                "key\tru\ten"
            };

            args.AddRange(lines.Select(l =>
                $"{l.key}\t{l.ru}\t=GOOGLETRANSLATE(INDIRECT(ADDRESS(ROW(),COLUMN()-1)),\"ru\",\"en\")"
            ));

            RunTool(args);
        }

        static void RunTool(List<string> args)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = ToolPath,
                Arguments = string.Join(" ", args.Select(a => $"\"{a}\"")),
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(ToolPath)
            };

            Process.Start(startInfo)?.WaitForExit();
        }

    }
}