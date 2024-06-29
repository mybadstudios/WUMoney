using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public class WUMONEYDEFINE
{
	static WUMONEYDEFINE()
	{
		BuildTargetGroup btg = EditorUserBuildSettings.selectedBuildTargetGroup;
		string defines_field = PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);
		List<string> defines = new List<string>(defines_field.Split(';'));
		if (!defines.Contains("WUM"))
		{
			defines.Add("WUM");
            //uncomment this after you have imported the Tapjoy SDK into your project
			//defines.Add("WUTJ");
			PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, string.Join(";", defines.ToArray()));
		}
	}
}


