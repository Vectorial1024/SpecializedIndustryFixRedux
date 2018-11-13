# SpecializedIndustryFixRedux
The successor of Specialized Industry Fix.

Warning: This mod uses the Harmony patching library. Do NOT directly put the source code into the %APPDATA%/ directory of the game. The game will fail to comprehend what Harmony is, and derp out error messages. Use IDEs (e.g. Visual Studio) to properly build the .dll, so that Harmony will be functioning appropriately.

# Technical Specifications
This mod uses the awesome Harmony library to patch methods, which provides significant productivity boost by eliminating the need to investigate detours. Go get the library [here](https://github.com/pardeike/Harmony).

This mod modifies IndustrialExtractorAI.SimulationStepActive via transpilation. (Refer to the Harmony wiki for more info on "transpiling".) While it is generally safe for other mods to modify this method by using prefixes or postfixes, it is certainly unsafe for them to modify this method also by transpilation.

# Documenting the Problem

While it is always the best to fully understand the program code before doing any changes, I do not wish to delve too deep into the code. That would derail discussion of the problem from its solutions to its causes. Besides, this method is so deep in the Cities Skylines code that it would be practically impossible to just understand what that method does without also understanding how the game works, which wastes time, and attention.

TODO: write up the real stuff
