using System;

// Space Engineers DLLs
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Game.EntityComponents;
using VRageMath;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using SpaceEngineers.Game.ModAPI.Ingame;

/*
 * Must be unique per each script project.
 * Prevents collisions of multiple `class Program` declarations.
 * Will be used to detect the ingame script region, whose name is the same.
 */
namespace ConsoleHologramSpinner {

  /*
   * Do not change this declaration because this is the game requirement.
   */
  public sealed class Program: MyGridProgram {

    /*
     * Must be same as the namespace. Will be used for automatic script export.
     * The code inside this region is the ingame script.
     */
    #region ConsoleHologramSpinner

    Vector3I rot = new Vector3I(0, 0, 0);
    bool ranFirst = false;
    string consoleName = "Console Block";
    Vector3I rotation_vector = new Vector3I(1, 0, 0);
    IMyProjector console = null;

    /*
     * The constructor, called only once every session and always before any 
     * other method is called. Use it to initialize your script. 
     *    
     * The constructor is optional and can be removed if not needed.
     *
     * It's recommended to set RuntimeInfo.UpdateFrequency here, which will 
     * allow your script to run itself without a timer block.
     */
    public Program() {
      Runtime.UpdateFrequency = UpdateFrequency.Update1;
    }

    /*
     * Called when the program needs to save its state. Use this method to save
     * your state to the Storage field or some other means. 
     * 
     * This method is optional and can be removed if not needed.
     */
    public void Save() {}

    /*
     * The main entry point of the script, invoked every time one of the 
     * programmable block's Run actions are invoked, or the script updates 
     * itself. The updateSource argument describes where the update came from.
     * 
     * The method itself is required, but the arguments above can be removed 
     * if not needed.
     */
    public void Main(string argument, UpdateType updateSource) {
      // load config from Custom Data
      if (ranFirst == false) {
        console = GridTerminalSystem.GetBlockWithName(consoleName) as IMyProjector;
        if (console != null) {
          console.ProjectionRotation = new Vector3I(0, 0, 0);
          rot = console.ProjectionRotation;
        }
        foreach(string line in Me.CustomData.Split('\n')) {
          string[] splited = line.Split('=');
          string header = splited[0].Trim();
          string name;
          switch (header) {
          case "RotationVector":
            string[] argsplited = splited[1].Split(',');
            int[] xyz = {
              1,
              0,
              0
            };
            int index = 0;
            foreach(string arg in argsplited) {
              int result = 0;
              bool success = Int32.TryParse(arg.Trim(), out result);
              if (success) {
                xyz[index] = result;
                index += 1;
              } else {
                break;
              }
            }
            rotation_vector = new Vector3I(xyz[0], xyz[1], xyz[2]);
            break;
          case "ConsoleName":
            name = line.Remove(0, 11).Trim().Split('=')[1].Trim();
            consoleName = name;
            console = GridTerminalSystem.GetBlockWithName(consoleName) as IMyProjector;
            if (console == null) {
              break;
            }
            console.ProjectionRotation = new Vector3I(0, 0, 0);
            rot = console.ProjectionRotation;
            break;
          case "RefreshMode":
            name = line.Remove(0, 11).Trim().Split('=')[1].Trim();
            switch (name) {
            case "Realtime":
              Runtime.UpdateFrequency = UpdateFrequency.Update1;
              break;
            case "Normal":
              Runtime.UpdateFrequency = UpdateFrequency.Update10;
              break;
            case "Slow":
              Runtime.UpdateFrequency = UpdateFrequency.Update100;
              break;
            default:
              break;
            }
            break;
          default:
            break;
          }

        }
        ranFirst = true;
      }

      if (console == null) {
        Echo("Could not find block named: " + consoleName);
        return;
      }

      // rotate projection
      rot = console.ProjectionRotation + rotation_vector;
      if (rot.X > 180) {
        rot.X = -179;
      }
      if (rot.X < -180) {
        rot.X = 179;
      }
      if (rot.Y > 180) {
        rot.Y = -179;
      }
      if (rot.Y < -180) {
        rot.Y = 179;
      }
      if (rot.Z > 180) {
        rot.Z = -179;
      }
      if (rot.Z < -180) {
        rot.Z = 179;
      }

      console.ProjectionRotation = new Vector3I(rot.X, rot.Y, rot.Z);
      console.UpdateOffsetAndRotation();

      // debug
      Echo("Current rotation: " + console.ProjectionRotation.ToString());
      Echo("\nConfig:\n" + Me.CustomData);
      Echo("\nRotating by " + rotation_vector.ToString());
      Echo("Update source type:" + updateSource.ToString());
      Echo("\nRecompile for changes in config to take effect");
    }

    #endregion // ConsoleHologramSpinner
  }
}