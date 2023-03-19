using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Text;
using System.Globalization;
using Humanizer;


// Generate DBContext class automatically from Models folder

/// How to test?
/// > dotnet restore
/// > dotnet build
/// > cd EFCoreDbContextGenerator.CLITest
/// >  ..\EFCoreDbContextGenerator\bin\Debug\net7.0\EFCoreDbContextGen.exe -f=Models
/// 
/// To use you can also just copy EFCoreDbContextGen.exe to your models folder and just run

string modelsFolder = null;
string dbContextFileName = "AppDbContext";

foreach (string arg in args)
{
    if (arg.StartsWith("-folder="))
    {
        modelsFolder = arg.Substring(8);
    }

    if (arg.StartsWith("-file="))
    {
        dbContextFileName = arg.Substring(6);
    }
}

if (modelsFolder == null)
{
    modelsFolder = Environment.CurrentDirectory;
}
else if (!Directory.Exists(modelsFolder)){
    modelsFolder = Path.Combine(Environment.CurrentDirectory, modelsFolder);
}

if (!Directory.Exists(modelsFolder))
{
    Console.WriteLine($"ERROR: Models folder is not exists: {modelsFolder}");
    Environment.Exit(1);
}

Console.WriteLine($"Models folder located at: {modelsFolder}");

DbContextGen.GenerateDbContextFile(modelsFolder, dbContextFileName);

#region Core
public static class DbContextGen
{
    public static void GenerateDbContextFile(string modelsFolder, string dbContextFileName)
    {
        var models = new List<string>();
        foreach (string file in Directory.EnumerateFiles(modelsFolder, "*.cs"))
        {
            if (file.ToLower().EndsWith("dbcontext.cs"))
            {
                continue;
            }

            Console.WriteLine(file);
            var code = File.ReadAllText(file);
            var publicClasses = GetListOfPublicClasses(code);
            models.AddRange(publicClasses);
        }

        if (models.Count == 0)
        {
            Console.WriteLine("No nodels found");
            Environment.Exit(2);
        }

        var content = GetDbContextContent(dbContextFileName, models);
        var dbContextFilePath = Path.Combine(modelsFolder, $"{dbContextFileName}.cs");
        
        if (File.Exists(dbContextFilePath))
        {
            Console.WriteLine("DbContext file allread exists. Do you want replace? y/n");
            if (Console.ReadLine() != "y")
            {
                Environment.Exit(3);
            }

            File.Delete(dbContextFilePath);
        }

        File.WriteAllText(dbContextFilePath, content);
    }

    static string GetDbContextContent(string dbContextName, List<string> models)
    {
        var content = new StringBuilder();
        content.AppendLine($"public class {dbContextName} : DbContext");
        content.AppendLine("{");
        content.AppendLine($"\tpublic {dbContextName}()");
        content.Append(@"
    {
  
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
    
");

        foreach (var item in models)
        {
            content.AppendLine($"\tpublic DbSet<{item}> {item.Pluralize()} {{ get; set; }}");
        }

        content.AppendLine("}");
        return content.ToString();
    }

    static List<string> GetListOfPublicClasses(string code)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
        SyntaxNode root = tree.GetCompilationUnitRoot();

        IEnumerable<ClassDeclarationSyntax> classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

        var publicClasses = new List<string>();
        foreach (ClassDeclarationSyntax classDeclaration in classes)
        {
            if (classDeclaration.Modifiers.Any(x => x.ValueText == "public"))
            {
                publicClasses.Add(classDeclaration.Identifier.ValueText);
            }
        }
        return publicClasses;
    }

}
#endregion