using Harmony;
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

        public override void OnLevelLoaded(LoadMode mode)
        {
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

        public override void OnLevelUnloading()
        {
            PatchController.Deactivate();
        }
    }
}
