using Harmony;
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

        public static void testMethod()
        {
            int extractionRate = 2, aliveWorkers = 4;
            ExtractionRateCorrection(ref extractionRate, ref aliveWorkers);
            /*
             *  IL_0005: ldloca.s 0 extractionRate at (loc-17)
	            IL_0007: ldloca.s 1 aliveWorker at (loc-5)
	            IL_0009: call void SpecializedIndustryFixRedux.Transpiler_SimulationStepActive::CorrectExtractionRate(int32&, int32&)
             */
        }

        public static void ExtractionRateCorrection(ref int extractionRate, ref int aliveWorker)
        {
            if (extractionRate < 1 && aliveWorker > 0)
            {
                extractionRate = 1;
            }
        }
    }
}
