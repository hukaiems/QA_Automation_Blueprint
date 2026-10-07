namespace Team2QA.Automation.Tests.Utils;

public static class StringUtility
{
    public static string ResolveFilePath(this string fileName)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, fileName);
            if (Path.Exists(candidate))
            {
                return candidate;
            }
            dir = dir.Parent;
        }
        return Path.Combine(AppContext.BaseDirectory, fileName);
    }
    public static string GetRandomName()
    {
        var random = new Random();
        string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string allChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._";
        int length = random.Next(10, 16);
        char[] nameArray = new char[length];
        nameArray[0] = chars[random.Next(chars.Length)];
        nameArray[length - 1] = chars[random.Next(chars.Length)];

        for (int i = 1; i < length - 1; i++)
        {
            char c;
            do
            {
                c = allChars[random.Next(allChars.Length)];
            } while ((c == '.' || c == '_') && (nameArray[i - 1] == '.' || nameArray[i - 1] == '_'));
            nameArray[i] = c;
        }

        return new string(nameArray);
    }
}
