// Generate DBContext class automatically from Models folder
// 

string modelsFolder = null;
foreach (string arg in args)
{
    if (arg.StartsWith("-f="))
    {
        modelsFolder = arg.Substring(3);
    }
}

if (modelsFolder != null)
{
    Console.WriteLine($"{modelsFolder}!");
}

Console.WriteLine("");