using UnityEditor.Compilation;
using UnityEditor.Networking;

public class TestUtility
{
    static void CompilationTestTrigger()
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; ++i)
        {
            if (args[i].ToLower().Contains("-unetcompile"))
            {
                var targetAssembly = args[i + 1];
                WeaverRunner.OnCompilationFinished(targetAssembly, new CompilerMessage[] { });
                break;
            }
        }
    }
}