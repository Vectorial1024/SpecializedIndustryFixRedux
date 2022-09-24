using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace SpecializedIndustryFixRedux
{
    [HarmonyPatch(typeof(IndustrialExtractorAI))]
    [HarmonyPatch("SimulationStepActive")]
    public static class Patch_ExtractionRateCorrector
    {
        /// <summary>
        /// This function is rather complex. 
        /// It is recommended that you first read the Harmony library transpilation guides, then read this code.
        /// </summary>
        /// <param name="instructions"></param>
        /// <returns></returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool patchComplete = false;
            short occurenceLdloc = 0;
            // StreamWriter outputFile = new StreamWriter("C:\\Users\\Vincent Wong\\Desktop\\output.txt", true);

            CodeInstruction sampleIns = new CodeInstruction(OpCodes.Ldloc_S, 16);

            /*
             * The second appeareance of ldloc.s 16 indicates that we should begin patching.
             * Or, alternatively, the 15-th occurence of ldloc.s; this is more efficient due to
             * limitations to the Harmony library.
             * (Even more technical info: Harmony library cannot read the actual value of the operand, duh...)
             */
            foreach (CodeInstruction instruction in instructions)
            {
                if (!patchComplete)
                {
                    // outputFile.WriteLine(instruction + ": occurence of LdLoc-16 so far: " + occurenceLdloc);
                    if (instruction.opcode == OpCodes.Ldloc_S)
                    {
                        occurenceLdloc++;

                        if (occurenceLdloc == 15)
                        {
                            // Begin yielding our instructions here.
                            yield return new CodeInstruction(OpCodes.Ldloca_S, 17);
                            yield return new CodeInstruction(OpCodes.Ldloca_S, 5);
                            yield return new CodeInstruction(OpCodes.Call, typeof(Patch_ExtractionRateCorrector).GetMethod("ExtractionRateCorrection"));

                            patchComplete = true;
                        }
                    }
                }

                yield return instruction;
            }

            // outputFile.Close();
        }

        /*
         * For those that are interested in the vanilla code, here's the relevant section decompiled for you.
         * 
		int num = HandleWorkers(...);
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
		if (num != 0)
		{
			int num6 = (num5 - buildingData.m_customBuffer1) / 100;
			num = Mathf.Max(0, Mathf.Min(num, (num6 * 20000 + num5 - 1) / num5));
			int num7 = (num * num3 + Singleton<SimulationManager>.instance.m_randomizer.Int32(1000u)) / 1000;
			if (num6 > 0 && num7 != 0)
			{
                // Extract resources
			}
			num = num7 * 10;
		}

        According to information from the original mod page by Steam user steevm, it is said that the variable "num" can be zero, which will lead to the building being shut down.
        Thus, by checking eg alive workers count and forcing "num" to become 1, those industrial buildings can continue functioning.
        Original documentation here:
        https://steamcommunity.com/sharedfiles/filedetails/?id=662386761
         */

        /// <summary>
        /// I use this method in conjunction with IL-Spy to help me determine what IL-code to inject to the vanilla method.
        /// </summary>
        public static void testMethod()
        {
            int extractionRate = 2, aliveWorkers = 4;
            ExtractionRateCorrection(ref extractionRate, ref aliveWorkers);
            /*
             *  IL_0005: ldloca.s 0 corresponds to extractionRate at (loc-17)
	            IL_0007: ldloca.s 1 corresponds to aliveWorker at (loc-5)
	            IL_0009: call void SpecializedIndustryFixRedux.Transpiler_SimulationStepActive::CorrectExtractionRate(int32&, int32&)
             */
        }

        /// <summary>
        /// A lazy hack. Saves the work of actually planning the IL-code by redirecting the code to a written method.
        /// </summary>
        /// <param name="extractionRate"></param>
        /// <param name="aliveWorker"></param>
        public static void ExtractionRateCorrection(ref int extractionRate, ref int aliveWorker)
        {
            if (extractionRate < 1 && aliveWorker > 0)
            {
                extractionRate = 1;
            }
        }
    }
}
