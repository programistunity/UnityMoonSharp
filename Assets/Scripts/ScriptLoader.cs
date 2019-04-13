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
    Script script = new Script();
    script.Options.ScriptLoader = new EmbeddedResourcesScriptLoader();
    script.DoFile("Lua.txt");
    return script;
  }
}
