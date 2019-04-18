using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using UnityEngine;

public class ScriptLoader : MonoBehaviour
{

  public static Script LoadUnity()
  {
    Dictionary<string, string> scripts = new Dictionary<string, string>();
    object[] result = Resources.LoadAll("MoonSharp/Scripts/Lua", typeof(TextAsset));

    foreach (TextAsset ta in result.OfType<TextAsset>())
    {
      scripts.Add(ta.name, ta.text);
    }
    
    Script.DefaultOptions.ScriptLoader = new UnityAssetsScriptLoader(scripts);
    Script script = new Script();
    script.Options.ScriptLoader = new UnityAssetsScriptLoader(scripts);
        
    script.DoFile("Lua");
    return script;
  }

  public static Script LoadScript()
  {

    Script.DefaultOptions.ScriptLoader = new UnityAssetsScriptLoader("MoonSharp/Scripts/");
    var paths = new UnityAssetsScriptLoader("MoonSharp/Scripts/");
    ((ScriptLoaderBase) Script.DefaultOptions.ScriptLoader).ModulePaths = paths.ModulePaths;
       
   // Debug.LogError(paths.ModulePaths.Length);

     //   new string[] { "MoonSharp/Scripts/mover/?", "MoonSharp/Scripts/mover/?.txt" };



    var _main = new Script();

    ((ScriptLoaderBase)_main.Options.ScriptLoader).ModulePaths =
        new string[] { "MoonSharp/Scripts/?", "MoonSharp/Scripts/?.txt" };
    //paths.ModulePaths;
    //((ScriptLoaderBase)_main.Options.ScriptLoader).ModulePaths =
    //   ScriptLoaderBase.UnpackStringPaths("MoonSharp/Scripts/mover/?;MoonSharp/Scripts/mover?.lua");

    // ((ScriptLoaderBase)_main.Options.ScriptLoader).ModulePaths = 
    //new string[] { "MoonSharp/Scripts/mover/?", "MoonSharp/Scripts/mover/?.lua" };
    _main.DoFile("Lua");
  
    return _main;
  }

  public static Script LoadScript2()
  {
    
    Script.DefaultOptions.ScriptLoader = new UnityAssetsScriptLoader("MoonSharp/Scripts/Test");
    ((ScriptLoaderBase) Script.DefaultOptions.ScriptLoader).ModulePaths =
        ScriptLoaderBase.UnpackStringPaths("MoonSharp/Scripts/Test/?;MoonSharp/Scripts/Test/?.txt");


    var _main = new Script();
    _main.DoFile("Test");
    return _main;
  }


}
