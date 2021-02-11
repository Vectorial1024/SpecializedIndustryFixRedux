using CitiesHarmony.API;
using HarmonyLib;
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
            // we no longer need to worry about harmony; we will let citiesharmony help out with harmony

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

            UnifyHarmonyVersions();
            PatchController.Activate();
        }

        /// <summary>
        /// Executed whenever a map is being unloaded.
        /// This mod then undoes the changes using the Harmony library.
        /// </summary>
        public override void OnLevelUnloading()
        {
            UnifyHarmonyVersions();
            PatchController.Deactivate();
        }

        private void UnifyHarmonyVersions()
        {
            if (HarmonyHelper.IsHarmonyInstalled)
            {
                // this code will redirect our Harmony 2.x version to the authoritative version stipulated by CitiesHarmony
                // I will make it such that the game will throw hard error if Harmony is not found,
                // as per my usual software deployment style
                // the user will have to subscribe to Harmony by themselves. I am not their parent anyways.
                // so this block will have to be empty.
            }
        }
    }
}
