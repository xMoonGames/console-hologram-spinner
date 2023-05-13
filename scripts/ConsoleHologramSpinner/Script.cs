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

namespace ConsoleHologramSpinner {

  public sealed class Program: MyGridProgram {

    #region ConsoleHologramSpinner

    Vector3I currentRotation = new Vector3I(0, 0, 0);
    string consoleName = "Console Block";
    Vector3I rotationalVector = new Vector3I(1, 0, 0);
    IMyProjector consoleBlock = null;

    public Program() {
      Runtime.UpdateFrequency = UpdateFrequency.Update1;

      // load config from Custom Data
      consoleBlock = GridTerminalSystem.GetBlockWithName(consoleName) as IMyProjector;
      if (consoleBlock != null) {
        consoleBlock.ProjectionRotation = new Vector3I(0, 0, 0);
        currentRotation = consoleBlock.ProjectionRotation;
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
          rotationalVector = new Vector3I(xyz[0], xyz[1], xyz[2]);
          break;
        case "ConsoleName":
          name = line.Remove(0, 11).Trim().Split('=')[1].Trim();
          consoleName = name;
          consoleBlock = GridTerminalSystem.GetBlockWithName(consoleName) as IMyProjector;
          if (consoleBlock == null) {
            break;
          }
          consoleBlock.ProjectionRotation = new Vector3I(0, 0, 0);
          currentRotation = consoleBlock.ProjectionRotation;
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
    }

    public void Save() {}

    public void Main(string argument, UpdateType updateSource) {
      

      if (consoleBlock == null) {
        Echo("Could not find block named: " + consoleName);
        return;
      }

      // rotate projection
      currentRotation = consoleBlock.ProjectionRotation + rotationalVector;
      currentRotation.X = currentRotation.X > 180 ? -179 : currentRotation.X < -180 ? 179 : (int) VRageMath.MyMath.Clamp(currentRotation.X, -179, 180);
      currentRotation.Y = currentRotation.Y > 180 ? -179 : currentRotation.Y < -180 ? 179 : (int) VRageMath.MyMath.Clamp(currentRotation.Y, -179, 180);
      currentRotation.Z = currentRotation.Z > 180 ? -179 : currentRotation.Z < -180 ? 179 : (int) VRageMath.MyMath.Clamp(currentRotation.Z, -179, 180);

      consoleBlock.ProjectionRotation = new Vector3I(currentRotation.X, currentRotation.Y, currentRotation.Z);
      consoleBlock.UpdateOffsetAndRotation();

      // debug
      Echo("Current rotation: " + consoleBlock.ProjectionRotation.ToString());
      Echo("\nConfig:\n" + Me.CustomData);
      Echo("\nRotating by " + rotationalVector.ToString());
      Echo("Update source type:" + updateSource.ToString());
      Echo("\nRecompile for changes in config to take effect");
    }

    #endregion // ConsoleHologramSpinner
  }
}
