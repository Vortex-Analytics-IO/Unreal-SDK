using UnrealBuildTool;

public class Hintway : ModuleRules
{
	public Hintway(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;

		PublicDependencyModuleNames.AddRange(new string[] {
			"Core", "CoreUObject", "Engine", "HTTP", "Json", "JsonUtilities", "DeveloperSettings"
		});
	}
}