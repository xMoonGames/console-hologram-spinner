// Welcome to the Console Block Auto Rotator script by RenanMsV.
//
//
//

Vector3I rot = new Vector3I(0, 0, 0);
bool ranFirst = false;
string consoleName = "Console Block";

Vector3I rotation_vector = new Vector3I(1, 0, 0);
IMyProjector console = null;

public Program() {
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
}

public void Save() {}

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