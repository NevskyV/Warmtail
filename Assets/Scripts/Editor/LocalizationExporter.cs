using System.Collections.Generic;
using System.Linq;
using Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace Editor
{
    public static class LocalizationExporter
    {
        private static string SPREADSHEET_ID = "1PEswgSetu71j068EhzqhuEPMUwGHdLbFa2sXZ9EeWAg";
        private static readonly Dictionary<string, string> _nameToGid = new ()
        {
            {"UI", "1087436388"},
            {"Player", "1556233291"},
            {"Tertilus", "2008482062"},
            {"Finix", "1520681221"},
            {"Octoboss", "324218113"},
            {"Skyper", "1314801844"},
            {"Jelica", "739200791"},
            {"Star", "317498044"},
            {"Fragments", "1106492096"},
            {"Quests", "2071454227"}
        };
        private static SheetsService CreateService()
        {
            var credential = GoogleCredential
                .GetApplicationDefault()
                .CreateScoped(SheetsService.Scope.Spreadsheets);
        
            return new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Unity Dialogue Exporter"
            });
        }

        private static void DeleteRows(int dialogueId, string sheetName)
        {
            var service = CreateService();

            var range = $"{sheetName}!A:A";
            var request = service.Spreadsheets.Values.Get(SPREADSHEET_ID, range);
            var response = request.Execute();

            if (response.Values == null)
                return;

            var deleteRequests = new List<Request>();

            for (int i = response.Values.Count - 1; i >= 1; i--)
            {
                var key = response.Values[i][0].ToString();
                if (key.Contains($"_{dialogueId}_"))
                {
                    deleteRequests.Add(new Request
                    {
                        DeleteDimension = new DeleteDimensionRequest
                        {
                            Range = new DimensionRange
                            {
                                SheetId = int.Parse(_nameToGid[sheetName]),
                                Dimension = "ROWS",
                                StartIndex = i,
                                EndIndex = i + 1
                            }
                        }
                    });
                }
            }

            if (deleteRequests.Count == 0)
                return;

            service.Spreadsheets.BatchUpdate(
                new BatchUpdateSpreadsheetRequest
                {
                    Requests = deleteRequests
                },
                SPREADSHEET_ID
            ).Execute();
        }


        private static void InsertRow(string sheetName, string key, string ru)
        {
            var service = CreateService();

            var values = new List<IList<object>>
            {
                new List<object>
                {
                    key,
                    ru,
                    "=GOOGLETRANSLATE(INDIRECT(ADDRESS(ROW(),COLUMN()-1)),\"ru\",\"en\")"
                }
            };

            var body = new ValueRange
            {
                Values = values
            };

            var request = service.Spreadsheets.Values.Append(
                body,
                SPREADSHEET_ID,
                $"{sheetName}!A:C"
            );

            request.ValueInputOption =
                SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

            request.Execute();
        }

        public static void UploadToSheets(RuntimeDialogueGraph graph)
        {
            foreach (var table in _nameToGid.Keys)
                DeleteRows(graph.DialogueId, table);

            foreach (var node in graph.AllNodes.OfType<Data.Nodes.TextNode>())
            {
                var sheet = node.Character.ToString();
                var key = $"{sheet}_{graph.DialogueId}_{node.NodeId}";
                InsertRow(sheet, key, node.Text);
            }

            foreach (var node in graph.AllNodes.OfType<Data.Nodes.ChoiceNode>())
            {
                int i = 0;
                foreach (var line in node.Choices)
                {
                    var key = $"Player_{graph.DialogueId}_{node.NodeId}_{i}";
                    InsertRow("Player", key, line);
                    i++;
                }
            }
        }
    }
}