using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GameInterface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace HotloadPong
{
    public class Hotloader : IGameInterface
    {
        // For gamelogic hotloading
        Assembly asm; // The current loaded gamelogic assembly
        DateTime lastUpdateDLL; // Last time the gamelogic dll file was updated
        IGameInterface proxy; // Proxy through we we talk to the gamelogic class
        string solutionPath;
        string executionPath;

        //For Shader Hotloading
#if DEBUG
        ContentManager tempContent;
        DateTime lastUpdateShaders;
        //Location of mgcb executable, may be different on your system.
        string mgcbPathExe = @"C:\Program Files (x86)\MSBuild\MonoGame\v3.0\Tools\MGCB.exe";
#endif
        public Dictionary<string, Effect> shaders;
        ContentManager content;
        GraphicsDevice device;


        public Hotloader(ContentManager content, GraphicsDevice device)
        {
            //This gets the execution directory, 5 folders deep from solution
            //Adjust as necessary for your project structure
            executionPath = AppDomain.CurrentDomain.BaseDirectory;
            solutionPath = executionPath + @"..\..\..\..\..";

            //Load gamelogic dll
            LoadDLL();

            //Setup shader hotloading
            this.content = content;
            this.device = device;
            shaders = new Dictionary<string, Effect>();
#if DEBUG
            tempContent = new ContentManager(content.ServiceProvider, content.RootDirectory);
            lastUpdateShaders = DateTime.Now;
#endif
        }

        public void LoadDLL()
        {            
            var path = solutionPath + @"\GameLogic\bin\Debug\GameLogic.dll";            
            lastUpdateDLL = File.GetLastWriteTime(path);            
            asm = Assembly.Load(File.ReadAllBytes(path));
            proxy = (IGameInterface)asm.CreateInstance("GameLogic.GameLogic");
        }

        public GameState Update(KeyboardState keyboard, GameTime gameTime)
        {
            return proxy.Update(keyboard,gameTime);
        }

        public void SetState(GameState state)
        {
            proxy.SetState(state);
        }

        public void CheckDLL(GameState state)
        {            
            var path = solutionPath + @"\GameLogic\bin\Debug\GameLogic.dll";
            var update = File.GetLastWriteTime(path);            
            if (update > lastUpdateDLL)
            {                
                asm = null;                
                LoadDLL();
                proxy.SetState(state);
            }
        }

        

#if DEBUG
        public void CheckShaders()
        {
            
            var files = Directory.GetFiles(solutionPath+@"/PlatformLayer/Content", "*.fx");
            foreach (var file in files)
            {
                var t = File.GetLastWriteTime(file);
                if (t > lastUpdateShaders)
                {
                    ShaderChanged(file);
                    lastUpdateShaders = t;
                }
            }

        }

        public void ShaderChanged(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            Process pProcess = new Process
            {
                StartInfo =
                       {
                            FileName = mgcbPathExe,
                            Arguments = "/platform:Windows /config: /profile:Reach /compress:False /importer:EffectImporter /processor:EffectProcessor /processorParam:DebugMode=Auto /build:"+name+".fx",
                            CreateNoWindow = true,
                            WorkingDirectory = solutionPath+@"\PlatformLayer\Content",
                            UseShellExecute = false,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true
                        }
            };

            //Get program output
            string stdError = null;
            StringBuilder stdOutput = new StringBuilder();
            pProcess.OutputDataReceived += (sender, args) => stdOutput.Append(args.Data);

            
            pProcess.Start();
            pProcess.BeginOutputReadLine();
            stdError = pProcess.StandardError.ReadToEnd();
            pProcess.WaitForExit();

            string builtPath = solutionPath+@"\PlatformLayer\Content\" + name + ".xnb";
            string movePath = executionPath+ "/Content/" + name + ".xnb";
            File.Copy(builtPath, movePath, true);

            ContentManager newTemp = new ContentManager(tempContent.ServiceProvider, tempContent.RootDirectory);
            var newShaders = new Dictionary<string, Effect>();
            foreach (var shaderName in shaders.Keys)
            {
                var effect = newTemp.Load<Effect>(shaderName);
                newShaders.Add(shaderName.ToLower(), effect);
            }

            tempContent.Unload();
            tempContent.Dispose();
            tempContent = newTemp;
            shaders = newShaders;                        

        }
#endif

        public Effect GetShader(string name)
        {
            return shaders[name.ToLower()];
        }

        public void AddShader(string name)
        {
            if (!shaders.ContainsKey(name.ToLower()))
            {
                var shader = content.Load<Effect>(name.ToLower());
                shaders.Add(name.ToLower(), shader);
            }
        }




    }



}
