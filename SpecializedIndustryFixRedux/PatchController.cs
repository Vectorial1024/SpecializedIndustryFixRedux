﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SpecializedIndustryFixRedux
{
    public class PatchController
    {
        public static string HarmonyModID
        {
            get
            {
                return "com.vectorial1024.cities.sifr";
            }
        }

        /*
         * The "singleton" design is pretty straight-forward.
         */

        private static Harmony harmony;

        public static Harmony GetHarmonyInstance()
        {
            if (harmony == null)
            {
                harmony = new Harmony(HarmonyModID);
            }

            return harmony;
        }

        public static void Activate()
        {
            GetHarmonyInstance().PatchAll(Assembly.GetExecutingAssembly());
        }
        
        public static void Deactivate()
        {
            GetHarmonyInstance().UnpatchAll(HarmonyModID);
        }
    }
}
