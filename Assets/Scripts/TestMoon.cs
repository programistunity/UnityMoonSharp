using System;
using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using UnityEngine;

[MoonSharpUserData]
public class MoonTransform
{
  public MoonVector3 Position=new MoonVector3(0,0,0);
}
[MoonSharpUserData]
public class LuaTransform
{

  private Transform _transform;

  public LuaTransform(Transform transform)
  {
    _transform = transform;
  }

  public Vector3 position
  {
    get
    {
      Debug.Log("Getting");
      return _transform.position;
    }
    set
    {
      Debug.Log("Setting");
      _transform.position = value;
    }
  }

}

[MoonSharpUserData]
public class MoonVector3
{
  public float X,Y,Z;

  public MoonVector3()
  {
    X = 0;
    Y = 0;
    Z = 0;
  }
  [MoonSharpVisible(true)]
  public MoonVector3(float x, float y, float z)
  {
    X = x;
    Y = y;
    Z = z;
  }

  public MoonVector3 V3()
  {
    return  new MoonVector3(10,5,10);
  }
}

class ProxyTransform
{
  Transform target;

  [MoonSharpHidden]
  public ProxyTransform(Transform p)
  {
    this.target = p;
  }

  public void Rotate(Vector3 axis,float angle)
  {
    target.Rotate(axis,angle);
  }
}


public class TestMoon : MonoBehaviour
{
  public List<Transform> RotateObjects = new List<Transform>(4);

  public bool GlobalScripts;
  public bool MoonRotate;

  List<Script> _globalScripts = new List<Script>();
  private string _code;

  public string LuaCode = @"
local function rotate = function(self, delta)
  self._game_object.transform.Rotate(0, 0, self._speed * delta);
end -- rotate
local make = function(game_object, speed)
  return 
  {
    _game_object = game_object;
    _speed = speed;

    --

    rotate = rotate;
  }
end

--

return
{
  make = make;
}";

  private Script LuaScript;

  // Start is called before the first frame update
  void Start()
  {

    UserData.RegisterType<Vector3>();
    UserData.RegisterType<Transform>();
    foreach (var rotateObject in RotateObjects)
    {
      var script = new Script();
      script.Globals["mytarget"] = rotateObject;
      script.Globals["axis"] = Vector3.up;
      script.Globals["angle"] = 7;

      _globalScripts.Add(script);
    }

    _code = @"
		x = mytarget.Rotate(axis,angle);";

    Debug.LogError(MoonFact(7));
    Debug.LogError(UnityFact(7));
    Debug.LogError(MoonCallUnity(7));
    MoonRotateTransform(Vector3.up, 7, transform);

    LuaScript = new Script();
    
    
  }

  private void Update()
  {
    if (GlobalScripts)
    {
      foreach (var script in _globalScripts)
      {
        script.DoString(_code);
      }

      return;
    }

    if (MoonRotate)
    {
      foreach (var rotateObject in RotateObjects)
      {
        MoonRotateTransform2(Vector3.up, 7, rotateObject);
      }
    }
    else
    {
      foreach (var rotateObject in RotateObjects)
      {
        RotateTransform(Vector3.up, 7, rotateObject);
      }
    }

  }

  double MoonFact(int i)
  {
    string scriptCode = @"    
		-- defines a factorial function
		function fact (n)
			if (n == 0) then
				return 1
			else
				return n*fact(n - 1)
			end
		end

		return fact(mynumber)";

    var script = new Script();
    script.Globals["mynumber"] = i;

    var res = script.DoString(scriptCode);
    return res.Number;
  }

  private double UnityFact(int i)
  {
    if (i == 0)
    {
      return 1;
    }

    return i * UnityFact(i - 1);
  }

  private double MoonCallUnity(int i)
  {
    string scriptCode = @"    
		-- defines a factorial function
		function fact (n)			
				return call(n);
        end";

    Script script = new Script();

    var unityFact = (Func<int, double>) UnityFact;
    script.Globals["call"] = unityFact;

    script.DoString(scriptCode);

    DynValue res = script.Call(script.Globals["fact"], i);
    return res.Number;
  }

  void MoonRotateTransform(Vector3 axis, float angle, Transform target)
  {
    UserData.RegisterType<Vector3>();
    UserData.RegisterProxyType<ProxyTransform, Transform>(r => new ProxyTransform(r));

    var s = new Script();

    s.Globals["mytarget"] = new ProxyTransform(target);
    s.Globals["axis"] = axis;
    s.Globals["angle"] = angle;

    s.DoString(@"
		x = mytarget.Rotate(axis,angle);");

  }

  void MoonRotateTransform2(Vector3 axis, float angle, Transform target)
  {
    var s = new Script();

    s.Globals["mytarget"] = target;
    s.Globals["axis"] = axis;
    s.Globals["angle"] = angle;


    s.DoString(@"
		x = mytarget.Rotate(axis,angle);");

  }

  void RotateTransform(Vector3 axis, float angle, Transform target)
  {
    target.Rotate(axis, angle);
  }

  private void LuaRotate(Vector3 axis, float angle, Transform target)
  {
    Script script = new Script();

    script.DoString(LuaCode);

  

    var s = new Script();

    s.Globals["mytarget"] = target;
    s.Globals["axis"] = axis;
    s.Globals["angle"] = angle;
    
  }
}


