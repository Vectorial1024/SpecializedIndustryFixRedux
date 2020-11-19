﻿using HarmonyLib;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SpecializedIndustryFixRedux
{
    public class SpecializedIndustryFixRedux: LoadingExtensionBase, IUserMod
    {
        public virtual string Name
        {
            get
            {
                return "Specialized Industry Fix Redux";
            }
        }

        public virtual string Description
        {
            get
            {
                return "Specialized industry buildings no longer randomly switch off.";
            }
        }

        /// <summary>
        /// Executed whenever a level completes its loading process.
        /// This mod the activates and patches the game using Hramony library.
        /// </summary>
        /// <param name="mode">The loading mode.</param>
        public override void OnLevelLoaded(LoadMode mode)
        {
            /*
             * This function can still be called when loading up the asset editor,
             * so we have to check where we are right now.
             */
            try
            {
                // a simple check to see if Harmony v2 is installed.
                Harmony h = new Harmony("bruh");
                h = null;
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Could not find Harmony v2; Specialized Industry Fix Redux now requires Harmony v2.");
            }
            
            switch (mode)
            {
                case LoadMode.LoadGame:
                case LoadMode.NewGame:
                case LoadMode.LoadScenario:
                case LoadMode.NewGameFromScenario:
                    break;

                default:
                    return;
            }

            PatchController.Activate();
        }

        /// <summary>
        /// Executed whenever a map is being unloaded.
        /// This mod then undoes the changes using the Harmony library.
        /// </summary>
        public override void OnLevelUnloading()
        {
            PatchController.Deactivate();
        }
    }
}
