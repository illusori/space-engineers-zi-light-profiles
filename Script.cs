string _script_name = "Zephyr Industries Light Profiles";
string _script_version = "1.0.1";

/* No in-script configuration required.
 * Just switch your lights to a profile by running the programmable block with the new profile name.
 * Current light settings are saved under the old profile name when the light changes profile.
 * Pro Tips:
 * - If you're paranoid and really want to save RIGHT NOW, just switch to the current profile.
 * - If a light has no profile, it'll save to the "original" profile.
 * - Try profile "Capac" for some special fun.
 */

string _script_title = null;

MyIni _ini = new MyIni();

List<IMyLightingBlock> _lights = new List<IMyLightingBlock>();
long _cycles = 0;
string _lights_profile = "original";

public Program() {
    _script_title = $"{_script_name} v{_script_version}";

    string[] stored_data = Storage.Split(':');
    if (stored_data.Length > 1) {
        _lights_profile = stored_data[1];
    }

    FindLights();

    if (!Me.CustomName.Contains(_script_name)) {
        // Update our block to include our script name
        Me.CustomName = $"{Me.CustomName} - {_script_name}";
    }
    Log(_script_title);

    Runtime.UpdateFrequency |= UpdateFrequency.Update100;
    if (_lights_profile == "capac") {
        //Runtime.UpdateFrequency |= UpdateFrequency.Update10;
    }
}

public void Save() {
    Storage = $"{_script_version}:{_lights_profile}";
}

public void Main(string argument, UpdateType updateSource) {
    try {
        if ((updateSource & UpdateType.Update100) != 0) {
	    _cycles++;

            if ((_cycles % 30) == 0) {
                FindLights();
            }

            if (_lights_profile == "capac") {
                UpdateCapacLights();
            }
        }
        if ((updateSource & (UpdateType.Trigger | UpdateType.Terminal)) != 0) {
            Log($"Switching lights profile from '{_lights_profile}' to '{argument}'.");
            SetLightProfiles(argument.ToLower());
        }
    } catch (Exception e) {
        string mess = $"An exception occurred during script execution.\nException: {e}\n---";
        Log(mess);
        throw;
    }
}

public void FindLights() {
    _lights.Clear();
    GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(_lights);
    if (_lights_profile == "capac") {
        // Ensure any newly created or merged blocks get their profiles saved before we trash them.
        SetLightProfiles("capac");
    }
    Log($"Found {_lights.Count} light blocks.");
}

public void SetLightProfile(IMyLightingBlock light, string name) {
    MyIniParseResult parse_result;
    string current_profile, section;
    Color color;

    _ini.Clear();
    _ini.TryParse(light.CustomData, out parse_result);
    current_profile = _ini.Get("light_profile", "current").ToString("original");

    // Update the current profile with the current settings.
    if (current_profile != "capac") {
        section = $"light_profile_{current_profile}";
	_ini.Set(section, "Radius", light.Radius);
	_ini.Set(section, "Intensity", light.Intensity);
	_ini.Set(section, "Falloff", light.Falloff);
	_ini.Set(section, "BlinkIntervalSeconds", light.BlinkIntervalSeconds);
	_ini.Set(section, "BlinkLength", light.BlinkLength);
	_ini.Set(section, "BlinkOffset", light.BlinkOffset);
	_ini.Set(section, "Offset", light.GetValueFloat("Offset")); // No property? Weird.
	_ini.Set(section, "Color.R", light.Color.R);
	_ini.Set(section, "Color.G", light.Color.G);
	_ini.Set(section, "Color.B", light.Color.B);
	_ini.Set(section, "Color.A", light.Color.A);
	_ini.Set(section, "Enabled", light.Enabled);
    }

    // Try to load the new profile.
    if (name != "capac") {
        section = $"light_profile_{name}";
        if (_ini.ContainsSection(section)) {
	    light.Radius = _ini.Get(section, "Radius").ToSingle(2.0F);
	    light.Intensity = _ini.Get(section, "Intensity").ToSingle(4.0F);
	    light.Falloff = _ini.Get(section, "Falloff").ToSingle(1.0F);
	    light.BlinkIntervalSeconds = _ini.Get(section, "BlinkIntervalSeconds").ToSingle(0.0F);
	    light.BlinkLength = _ini.Get(section, "BlinkLength").ToSingle(10.0F);
	    light.BlinkOffset = _ini.Get(section, "BlinkOffset").ToSingle(0.0F);
	    light.SetValueFloat("Offset", _ini.Get(section, "Offset").ToSingle(0.5F));
	    color = light.Color;
	    color.R = _ini.Get(section, "Color.R").ToByte((byte)255);
	    color.G = _ini.Get(section, "Color.G").ToByte((byte)255);
	    color.B = _ini.Get(section, "Color.B").ToByte((byte)255);
	    color.A = _ini.Get(section, "Color.A").ToByte((byte)255);
	    light.Color = color;
	    light.Enabled = _ini.Get(section, "Enabled").ToBoolean(true);
        }
    }
    _ini.Set("light_profile", "current", name);
    light.CustomData = _ini.ToString();
}

public void SetLightProfiles(string name) {
    if (_lights_profile == "capac" && name != "capac") {
        DisableCapacLights();
    }
    foreach (IMyLightingBlock light in _lights) {
        if (light != null) {
            SetLightProfile(light, name);
        }
    }
    if (name == "capac" && _lights_profile != "capac") {
        EnableCapacLights();
    }
    _lights_profile = name;
}

public void UpdateCapacLights() {
    Color color;
    Random r = new Random();
    foreach (IMyLightingBlock light in _lights) {
        if (light != null) {
	    light.Radius = (float)r.NextDouble() * 20F;
	    light.Falloff = (float)r.NextDouble() * 3F;
	    light.Intensity = (float)r.NextDouble() * 10F;
	    light.BlinkIntervalSeconds = (float)r.NextDouble() * 10F;
	    light.BlinkLength = (float)r.NextDouble() * 100F;
	    light.BlinkOffset = (float)r.NextDouble() * 100F;
	    light.SetValueFloat("Offset", (float)r.NextDouble() * 5F);
	    color = light.Color;
	    color.R = (byte)r.Next(256);
	    color.G = (byte)r.Next(256);
	    color.B = (byte)r.Next(256);
	    color.A = (byte)255;
	    light.Color = color;
        }
    }
}

public void EnableCapacLights() {
    // TODO: change update freq?
    UpdateCapacLights();
}

public void DisableCapacLights() {
    // TODO: change update freq?
}

public void Log(string s) {
    Echo(s);
}
