using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
namespace Team2QA.Automation.Tests.Utils;

public static class ExcelUtil
{
    private static string GetProjectRoot()
    {
        string? currentDir = AppDomain.CurrentDomain.BaseDirectory;
        while (currentDir != null)
        {
            if (Directory.GetFiles(currentDir, "*.csproj").Length > 0)
            {
                return currentDir;
            }
            currentDir = Directory.GetParent(currentDir)?.FullName;
        }
        return AppDomain.CurrentDomain.BaseDirectory;
    }

    public static List<Dictionary<string, string>> GetTestData(string filePath, string sheetName = "TestCase")
    {
        var testData = new List<Dictionary<string, string>>();

        string projectRoot = GetProjectRoot();
        string fileName = Path.GetFileName(filePath);
        string fullPath = Path.Combine(projectRoot, "Data", fileName);
        if (!File.Exists(fullPath))
        {
            fullPath = filePath;
        }

        if (!File.Exists(fullPath))
        {
            Console.WriteLine($"[Warning] Excel file not found at: {fullPath}");
            return testData;
        }

        using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            IWorkbook workbook = new XSSFWorkbook(stream);
            ISheet sheet = workbook.GetSheet(sheetName) ?? workbook.GetSheetAt(0);
            IRow headerRow = sheet.GetRow(0);
            if (headerRow == null)
            {
                return testData;
            }

            int colCount = headerRow.LastCellNum;
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow currentRow = sheet.GetRow(i);
                if (currentRow == null)
                {
                    continue;
                }

                var rowData = new Dictionary<string, string>();

                for (int j = 0; j < colCount; j++)
                {
                    string? columnName = headerRow.GetCell(j)?.ToString()?.Trim();
                    if (string.IsNullOrEmpty(columnName))
                    {
                        continue;
                    }

                    string cellValue = currentRow.GetCell(j)?.ToString()?.Trim() ?? "";
                    rowData.Add(columnName, cellValue);
                }
                testData.Add(rowData);
            }
        }

        return testData;
    }
}
