using UnityEditor;

public class CreateNewScriptClassFromCustomTemplate
{
    private const string pathToYourScriptTemplate = "Assets/com.dotdothorse.roadtrip/Scripts/Templates/RoadtripTemplate.cs.txt";

    [MenuItem(itemName: "Assets/Create/Roadtrip Script", isValidateFunction: false, priority: 51)]
    public static void CreateScriptFromTemplate()
    {
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(pathToYourScriptTemplate, "YourDefaultNewScriptName.cs");
    }
}