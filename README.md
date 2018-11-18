# SpecializedIndustryFixRedux
The successor of Specialized Industry Fix.

Warning: This mod uses the Harmony patching library. Do NOT directly put the source code into the %APPDATA%/ directory of the game. The game will fail to comprehend what Harmony is, and derp out error messages. Use IDEs (e.g. Visual Studio) to properly build the .dll, so that Harmony will be functioning appropriately.

# Technical Specifications
This mod uses the awesome Harmony library to patch methods, which provides significant productivity boost by eliminating the need to investigate detours. Go get the library [here](https://github.com/pardeike/Harmony).

This mod modifies IndustrialExtractorAI.SimulationStepActive via transpilation. (Refer to the Harmony wiki for more info on "transpiling".) While it is generally safe for other mods to modify this method by using prefixes or postfixes, it is certainly unsafe for them to modify this method also by transpilation.

# Documenting the Problem

While it is always the best to fully understand the program code before doing any changes, I do not wish to delve too deep into the code. That would derail discussion of the problem from its solutions to its causes. Besides, this method is so deep in the Cities Skylines code that it would be practically impossible to just understand what that method does without also understanding how the game works, which wastes time, and attention.

I will simply write down what works, and what works not.

## Description of Problem

Upon detailed inspection, specialized industry buildings (vanilla ones, not the Industry DLC ones) will constantly oscillate between "Operating Normally" and "Not Operating". This phenomenon can be seen clearly when one goes into the Fire Safety info view. Normally, industrial buildings generally have a lower fire safety rating (thus appear brown), but the color of the specialized industry buildings constantly switch back and forth between brown ("Operating Normally") and blue ("Not Operating", so no fire risk).

## Code Analysis
Thanks to steevm for providing information on this.

Refer to IndustrialExtractorAI.SimulationStepActive:

```
// Begin code snippet
Citizen.BehaviourData behaviour = default(Citizen.BehaviourData);
int aliveWorkerCount = 0;
int totalWorkerCount = 0;
int workPlaceCount = 0;
int num = HandleWorkers(buildingID, ref buildingData, ref behaviour, ref aliveWorkerCount, ref totalWorkerCount, ref workPlaceCount);
if ((buildingData.m_flags & Building.Flags.Evacuating) != 0)
{
    num = 0;
}
if (Singleton<SimulationManager>.instance.m_isNightTime)
{
    num = num + 1 >> 1;
}
int width = buildingData.Width;
int length = buildingData.Length;
int num2 = MaxOutgoingLoadSize();
int num3 = CalculateProductionCapacity((ItemClass.Level)buildingData.m_level, new Randomizer(buildingID), width, length);
int num4 = num3 * 500;
int num5 = Mathf.Max(num4, num2 * 2);
TransferManager.TransferReason outgoingTransferReason = GetOutgoingTransferReason();
if (num != 0)
{
    int num6 = (num5 - buildingData.m_customBuffer1) / 100;
    num = Mathf.Max(0, Mathf.Min(num, (num6 * 20000 + num5 - 1) / num5));
    int num7 = (num * num3 + Singleton<SimulationManager>.instance.m_randomizer.Int32(1000u)) / 1000;
    // Note: nu7 can be 0.
    if (num6 > 0 && num7 != 0)
    {
        // Code for extracting resources
    }
    num = num7 * 10;
    // Some other code
}
// End of code snippet
```

steevm discovered that, during the initialization of `int num7`, `num7` can be initialized to `0`. Whenever `num7 == 0`, the building will produce nothing, and will later shut down. Interestingly, in this case, the building will be re-activated again some time later, thus creating an infinite loop of shutting down and starting back up.

In the original mod by steevm, he added an additional if-statement to make `num7` carry a value of `1` whenever `num7 == 0`. That solved the issue and the building can produce things again.

What I did in this mod is to make another function, pass `num7` and some relevant parameters (e.g., alive workers) to it by reference to correct the value of `num7`. The newly-made function is then inserted back into the game using the Harmony Transpiler.

## What This Mod Fixes
By making sure that `num7` has a minimum value of `1`, specialized industry buildings are able to continuously operate, and produce their resources continuously. This normalization resulted in increased overall production, and also increased tax income.

There is always some complaint from players on "processing" specialized industry ordering resources from import rather than from within the specialized industry zone itself. In these complaints, the specialized industrial zone has access to said resource locally, and a significant number of "extraction" specialized industry buildings are present. It appears strange that "processing" would want to import resources when "extraction" is just next door.

Interestingly, when I play-test this mod, I noticed a significant reduction of such problem. I theorized that as "extraction" buildings work continuously, it is more likely that "processing" industry can find some local "extraction" industry that has the said resources, and thus rely less on imports.

TODO: write up the real stuff
