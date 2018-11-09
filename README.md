# monogame-hotload
A template/demo solution showing how to do hotswappable game logic and shaders with monogame and/or C#.

# [Demo Video](https://youtu.be/D1708LklhW0).

# Features

* During game runtime, recompile your game logic project, and it will swap in as the game runs.
* The drawing code can also be in the hotswappable dll if you want, you just have to pass in the GraphicsDevice object each draw call
* Change effect/shader files as the game runs, upon save, the game will detect the change and swap in the new shader/effect
* Debugging in visual studio still works fine on both ends

# Limitations

* Each time you swap in a new game logic dll, it *adds* rather than replaces the assembly, so it is a memory leak.  
* You will often have to build the game logic dll twice as it may fail to write the pdb file. This only applies if you run the platform layer from within Visual Studio. If you run it from the command line, no problems.

# Help Wanted

If anyone has ideas about how to get around the two limitations above, let me know.  I know about adding assemblies to a separate app domain, but this has a number of limitations that won't always be appropriate.

# Attribution

Sample sprite by:
Stephen Challener (Redshrike)
https://opengameart.org/content/bosses-and-monsters-spritesheets-ars-notoria
