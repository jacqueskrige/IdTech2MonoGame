# Quake 2 BSP Rendering using MonoGame (Microsoft XNA)

Very special thanks to Shaun Nirenstein (Crowley9) at NVIDIA for helping out with various aspects and considerations regarding rendering performance and HLSL programming.


# Quake 2 BSP Renderer

It's been quite some time since I last spent time on my idTech2 XNA Renderer, but I finally managed to upgrade the project to Microsoft XNA 4.0 as well as adding new features and bug fixes. For the longest time I wanted to add MD2 model rendering support, but never got around actually doing the code for it. While on vacation I thought it will be cool to quickly convert the technology from Microsoft .NET 3.5 and Microsoft XNA 3.1 to .NET 4.0 and XNA 4.0, but great was my disappointment when I realized that it won't be a quicky and that there were significant breaking changes introduced in XNA. At least these were all good breaking changes, but none the less it still meant I need to go through the technology with a fine tooth comb and fix the now broken bits of code.

After spending days reading loads of Shawn Hargreaves blog posts regarding these breaking changes and fixing code I finally managed to restore the idTech2 XNA Renderer to a working state. Some of the most challenging issues were changing the actual rendering of some of the existing geometry as it seems they dropped support for using triangle fans which forced me to rewrite all the logic where triangle fans were used in favour of triangle lists. The other tricky change was the way rendering effects got changed which also caused the bloom post processing to break. So after around a week or two's work the renderer was back to its original rendering state. I also found a few minor bugs that also got fixed and quite a number of performance improvements were done.

Another problem that's currently not fixed involves Shader Model 3.0 and the bloom post-processing effect. Since the bloom post-processing effect only uses a pixel shader and no vertex shader, running it via SM3 generates the error message "Cannot mix shader model 3.0 with earlier shader models". If either the vertex shader or pixel shader is compiled as 3.0, they must both be. I've checked the internet, but at the time of finishing this version of the idTech2 XNA Renderer I couldn't find a solution to this other than using the Shader Model 2 path.

There were quite a number of new features added to the technology, but let's start with what got dropped-out. That stupid XML configuration file was removed and made way for a proper application configuration file. Thinking back I really don't know why I didn't do this from the beginning. One of the real big internal changes in the renderer is the way it loads the BSP files. This was needed because an entity system was necessary to manage the loading and rendering of MD2 models. After doing the groundwork and researching idTech2's entity system the renderer sports a 99% complete idTech2-based entity system which manages the loading and rendering of two out of three types of models namely brush models and alias models. Sprite models isn't implemented yet. Brush models are any world geometry be it static worldmodel geometry or geometry the user can interact with and that isn't static. Alias models are any MD2 models representing objects, items, monsters etc.

The MD2 model (Alias model) loading and rendering code was done in a record 24 hours, but getting it rock-solid took a further few days with the lighting being properly applied to each model only a few weeks later. The brush model loading and rendering code was significantly changed because in the previous version of the renderer it only loaded the static worldmodel and rendered that via BSP, but within the worldmodel data there are sections of geometry in the data stream that is meant to form part of an entity similarly to a MD2 model. This geometry data is called submodels. These submodels makes up all the interactive parts of the static BSP world like breakable parts, rotating parts, moveable parts, doors, lifts etc. With some game code (logic) implemented eventually these seemingly static submodels will add some mechanical life to the BSP world.

Not only is the idTech2 XNA Renderer capable of rendering Quake2 data, its also capable of rendering Heretic II data. Since the Heretic II game is just a heavily modified Quake 2 engine it didn't take much extra work to get it to render. Infact, Raven Software didn't do any changes to the BSP file structure and it was possible to directly use the Quake 2 BSP tools to build Heretic II BSPs. Most of the work involved implementing Heretic II's M8 texture format which is only a modified Quake 2 WAL texture format. Adding support for Heretic II's M8 format enabled the renderer to be able to load and render Heretic II BSP files. Heretic II's custom model format isn't currently implemented, but maybe the next version of the idTech2 XNA Renderer will see that done.

There are quite a number of entity effects which isn't currently implemented in the renderer, because most entity effects relies on game code (logic) which is typically found in Quake2's gamex86.dll file. Some examples of these effects are model animation, rotation as well as the application of gravity. All model positioning and rotations in the renderer are expressed as design-time information which means what you see on-screen is actually where they are placed in the world before the game logic takes over. At times these positions and rotations might look strange. Another example is the static nature of submodels like doors, lifts, fans and other non-static world geometry. Since these submodels are attached to entities they are also rendering without any game code driving them.

Work was started on the basic framework/skeleton of the idTech2 engine's Client/Server technology, but its far from finished. Anyone that will can take this development further. The framework needs to be implemented for example to get static lightmap styles working. At the moment static lightmap styles are not implemented.


# Porting to MonoGame

The process porting the project from Microsoft XNA version 4.0 over to MonoGame version 3.8.2 went fairly smooth.
Most of the code compiled without issue. Only a few lines of code required simple adjustments to compile. This were marked with comments in the code showing the original version of the line of code as well as what it got replaced with.

Most of the work went into re-writing the HLS code to port it from Shader Model version 3.0 to Shader Model version 4.0 with the main difference being originally the HLSL code was compiled with an MSBuild which got replaced with MonoGame's native process.


I hope this code help anyone wanting to understand how to render Quake 2 assets using C# and MonoGame.

Jacques Krige
-------------
https://github.com/jacqueskrige/IdTech2MonoGame