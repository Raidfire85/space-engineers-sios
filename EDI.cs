using Sandbox;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage;
using VRageMath;
using CoreOs;

namespace core.edi
{
    public class EDI
    {
        ///<SUMMARY>
        /// 
        IMyGridTerminalSystem GridTerminalSystem;
        
        const string creator = "Mahtrok";
        const string title = "$IOS <EDI> ";
        const string version = "v1.0";
        const string created = "10-15-15";
        const string updated = "10-17-15";
        const string contact = "Mahtrok@mahtrok.de";

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Exodus Systems - $IOS - Self installing operating system <EDI>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ATTENTION: The <EDI> is NOT A STAND-A-LONE Script! You have to install the $IOS <CORE> on your 
        /// GRID first, or there will not be any blocks that could be managed by this Script.
        /// 
        /// After installing the $IOS Core all blocks on your grid should be renamed correct. If the Core doesn't give any
        /// Errors or Crashes you can set up the <EDI> Script.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// >>> What is the $IOS <EDI>? <<<
        /// 
        /// This Script checks your Platform/Ship Grid (and only yours, even if there are docked ships), identified by a
        /// passed in platformID, for any incoming damage through hacking or attacks.
        /// Besides that it detects unpressurized AirVents and is able to close doors around this AirVent to prevent further
        /// loss of atmospheric pressure.
        /// The Script can change the $IOS <CORE>s Condition Managers state and therefor trigger alarms, change light
        /// -ing and LCD Colors, start timers, automatically close security doors, deactivate gravity generators and activate
        /// turrets to defend all vital systems in case of an attack.
        /// The hole $IOS system depends on an external configuration screen that is also used as external data storage.
        /// An exact explanation of how this works can be find inside the $IOS <CORE> Scripts Manual.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////


        /* >> 1.0 ATTACK & HACKING DEFENCE SETTINGS << */

        /// THERE ARE A FEW THINGS TO BE SET UP CORRECT SO READ CAREFULLY!
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// If you want to have a block be controlled automatically when condition orange or red is called you will have 
        /// to go to this block and adding a divider "|" and the following ID inside the [ ] Brackets at the end of the $IOS 
        /// info. "|$Sec"
        /// For Example an Alarm Block should be called like: "[paltformID|$Spea|$Sec] Sound Block 2" 
        /// If so, the block will start playing as soon as "condition: red" is called by either this or any other $IOS script. 

        const char divider = '|';
        const string securityID = "$Sec";

        /// You can add the "|$Sec" statement to the following blocktypes:

        /// >> Doors - will be closed in case of an alert automatically
        /// >> Gravity Generators - will be turned off in case of an alert so be sure you want that!
        /// >> Lights - will be turned ON in case of an alert (normally for blinking warning lights)
        /// >> Sound Blocks - will be playing in case of an alert, stopping when condition: GREEN or ORANGE is set
        /// >> Turrets - will be activated in case of a RED alert ( not ORANGE ) and deactivated when GREEN or ORANGE
        /// >> Timers - will be STARTED in case of a RED alert ( not ORANGE ) and stopped when GREEN or ORANGE

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// NOTE: If the alarm has been triggered at least One time, you will have to manually reset the program!
        /// This has to be done by running the programmable block, providing the argument "secured". Then and
        /// ONLY then alarm stops and condition is set to green.
        /// If you want to you can set the programmable blocks argument to secured by default then the alarm stops
        /// automatically if no damage or hacking is detected any longer. 
        /// For example if all repairs have been done, after the Intruder has been catched or killed.
        /// If you don't want to hear the alarm continuesly you will have to run the program "secured" manually.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /* >> 2.0 ANTI DEPRESSURIZATION SETTINGS << */

        /// THE $IOS <CORE> renames all AirVents to something like [platformID|$Vent|$0000], [platformID|$Vent|$0001], 
        /// [platformID|$Vent|$0002] aso. so every AirVent has its unique identifier ("|$0002").
        /// You will have to copy this ID and paste it into any surrounding Doors name at the end (before the "]" bracket)
        /// You can add as much IDs as you would like to a single door but you do only need 1 AirVent per room 
        /// (for they all will be not pressurized so you could safe some character space)

        /// Example: A door is between AirVent 0000 and 0001 so you could make it [platformID|$Vent|$0000|$0001]
        /// Now if any of those rooms will be depressurized the door will close automatically.




        /*
        AVAILABLE PASS-IN ARGUMENTS:
        secured                                   // Resets the <EDI> after a hack or an attack has been detected and secured
        */
        ///</SUMMARY>





        // << 1.0 BLOCK ID SETTINGS >> //

        // To use a Debugger Screen you have to set the desired Screens PUBLIC TITLE to the following ID:
        const string debugID = "$IOS <EDI> Debug";

        // These Shortcuts will should've been added to your Grids blocks names by the $IOS <CORE> Script.
        // If you want to change these IDs you will first have to uninstall the Script from the CORE, then change the IDs in
        // EVERY SINGLE Exodus Systems $IOS Script you are using, starting with the CORE and reinstalling it.

        const string infoID = "$Info <EDI>";

        const string gravID = "$Grav";
        const string airVentID = "$Vent";
        const string doorID = "$Door";
        const string speakerID = "$Spea";
        const string lightID = "$Light";
        const string panelID = "$LCD";
        const string programID = "$Prog";
        const string timerID = "$Time";
        const string missileID = "$MTur";
        const string gatlingID = "$GTur";
        const string turretID = "$ITur";
        const string coreID = "$IOS <CORE>";
        const string configID = "$IOS EDI Config";




        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // THERE IS NO NEED TO CHANGE ANYTHING BELOW THIS LINE, AS LONG AS YOU DO NOT EXACTLY KNOW      //
        // WHAT YOU ARE DOING!                                                                                                                                            //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Condition Manager: To define the background and font Color for your LCDs
        Color conditionRedBG = new Color(40, 0, 0);
        Color conditionOrangeBG = new Color(40, 10, 0);
        Color panelDefaultBG = new Color(40, 40, 40);
        Color conditionRedFC = new Color(255, 255, 255);
        Color conditionOrangeFC = new Color(255, 255, 255);
        Color panelDefaultFC = new Color(0, 0, 0);

        // Condition Manager: To define the Colors for the Interior Lights
        Color conditionRedLight = new Color(255, 0, 0);
        Color conditionOrangeLight = new Color(255, 110, 0);

        string platformID = "";
        const char listDivider = ',';
        const char configDivider = '$';
        const char suffix = ']';
        const string prefix = "[";
        bool debugEnabled = false;
        bool securityActive = false;
        bool hackDetected = false;
        bool attackDetected = false;
        bool installEnabled = false;
        bool hideVersionInfoOnScreen = false;
        bool allowBeaconRename = false;
        bool booted = false;

        List<string> debugMessages;

        List<IMyGravityGenerator> gravs;
        List<IMyAirVent> airvents;
        List<IMySoundBlock> alerts;
        List<IMyInteriorLight> lights;
        List<IMyDoor> doors;
        List<IMyDoor> securityDoors;

        List<IMyTextPanel> panels;
        List<IMyTextPanel> debugger;
        IMyTextPanel config;

        List<IMyProgrammableBlock> programs;
        List<IMyTimerBlock> timers;
        IMyTimerBlock securityTimer;
        IMyProgrammableBlock core;

        List<IMyLargeInteriorTurret> turrets;
        List<IMyLargeGatlingTurret> gatlings;
        List<IMyLargeMissileTurret> missiles;

        int platformRoleID = 0;
        int airVentCnt = 0;
        int initLoop = 0;
        const string blank = "\n";
        const string tab = "    ";
        const string empty = " ";
        string condition = "green";

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        /*                                                                            GETTER                                                                                  */
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        string GetTimeStamp()
        {
            return prefix + System.DateTime.Now.Hour.ToString() + ":"
                                 + System.DateTime.Now.Minute.ToString() + ":"
                                 + System.DateTime.Now.Second.ToString() + suffix;
        }

        string GetVersionInfo()
        {
            return title + empty + version + " by " + creator + ", last Update: " + updated + blank;
        }

        string GetDebugInfo()
        {
            string _info = GetVersionInfo();
            for (int i = 0; i < debugMessages.Count; i++)
            {
                _info += debugMessages[i];
            }
            return _info;
        }

        bool IsBeingHacked(IMyTerminalBlock _block)
        {
            return _block.IsBeingHacked;
        }

        bool IsBeingAttacked(IMyTerminalBlock _block)
        {
            IMySlimBlock _slimBlock = _block.CubeGrid.GetCubeBlock(_block.Position);
            if (_slimBlock.DamageRatio > 1 || _slimBlock.CurrentDamage > _slimBlock.MaxIntegrity * 0.1f)
            {
                if (_slimBlock.DamageRatio > 1.66 || _slimBlock.CurrentDamage > _slimBlock.MaxIntegrity * 0.3f)
                {
                    Debug("Heavy Damage detected!");
                }
                else if (_slimBlock.DamageRatio > 1.33 || _slimBlock.CurrentDamage > _slimBlock.MaxIntegrity * 0.6f)
                {
                    Debug("Medium Damage detected!");
                }
                else
                    Debug("Light Damage detected!");
                return true;
            }
            else
                return false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        /*                                                                       MAIN METHOD                                                                           */
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void Init()
        {
            debugMessages = new List<string>();
            gravs = new List<IMyGravityGenerator>();
            airvents = new List<IMyAirVent>();
            alerts = new List<IMySoundBlock>();
            lights = new List<IMyInteriorLight>();
            doors = new List<IMyDoor>();
            securityDoors = new List<IMyDoor>();
            panels = new List<IMyTextPanel>();
            debugger = new List<IMyTextPanel>();
            programs = new List<IMyProgrammableBlock>();
            timers = new List<IMyTimerBlock>();
            turrets = new List<IMyLargeInteriorTurret>();
            gatlings = new List<IMyLargeGatlingTurret>();
            missiles = new List<IMyLargeMissileTurret>();
            List<IMyTerminalBlock> _configs = new List<IMyTerminalBlock>();
            GridTerminalSystem.SearchBlocksOfName(configID, _configs);
            List<IMyTerminalBlock> _debugs = new List<IMyTerminalBlock>();
            GridTerminalSystem.SearchBlocksOfName(debugID, _debugs);
            if (_debugs.Count > 0) {
                debugger.Add((IMyTextPanel)_debugs[0]);
                debugEnabled = true;
            }

            if (_configs.Count > 1)
            {
                Debug("==============================================");
                Debug("CRITICAL ERROR: Detected more then 1 $IOS config!");
                Debug("Please rename or undock every $IOS config, but the ");
                Debug("One belonging to this Station!");
                Debug("==============================================");
            }
            else
            {
                config = (IMyTextPanel)_configs[0];
                config.ShowPublicTextOnScreen();
                //config.SetValue("BackgroundColor", panelDefaultBG);
                //config.SetValue("FontColor", panelDefaultFC);
                //config.SetValue("FontSize", 0.8f);
            }
            if (config == null)
            {
                Debug("No external config file found.");
            }
        }

        void Main(string argument)
        {
            
            ////////////////////// INITIALIZATION
            Init();
            if (argument != "")
                ProcessArgument(argument);
            ////////////////////// GETTING ALL BLOCKs
            if (!booted)
                GetBlocks();
            ////////////////////// SECURITY
            if (securityActive)
            {
                Debug("<EDI>: Active");
                if (hackDetected || attackDetected)
                {
                    List<IMyTerminalBlock> _blocks = new List<IMyTerminalBlock>();
                    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(_blocks);

                    for (int i = 0; i < _blocks.Count; i++)
                    {
                        if (_blocks[i].CustomName.Contains(programID))
                        {
                            IMyProgrammableBlock _program = (IMyProgrammableBlock)_blocks[i];
                            if (_program.CustomName.Contains(coreID)) { 
                                core = _program;
                                core.TryRun("API: attack detected");
                            }
                        }
                    }
                    //IMyTerminalBlock CORE = GridTerminalSystem.GetBlockWithName("$IOS CORE");
                    //core = (IMyProgrammableBlock)CORE;
                    //core.TryRun("API: attack detected");
                }
                else
                {
                    Debug("-> Systems: Secured");
                }
                if (CheckPressureState())
                {
                    Debug("ALERT: Hullbreach detected!");
                }
                else
                {
                    Debug("-> Pressure: Stable");
                }
            }
            ///////////////////// DISPLAY
            Display();
            ///////////////////// DEBUGGER
            if (debugEnabled)
                DisplayDebug();
            ///////////////////// RESET SCRIPT
            Reset();
            initLoop++;
        }

        void ProcessArgument(string _argument)
        {
            Debug("Argument: " + _argument);
            _argument = _argument.ToLower();
   
            if (_argument.Contains("debug")) {
                List<IMyTerminalBlock> _blocks = new List<IMyTerminalBlock>();
                GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(_blocks);
                Debug("Argument contains debug");
                for (int i = 0; i < _blocks.Count; i++)
                {
                    if (_blocks[i].CustomName.Contains(programID))
                    {
                        IMyProgrammableBlock _program = (IMyProgrammableBlock)_blocks[i];
                        core = _program;
                        core.TryRun("API: attack detected");
                    }
                }
            }  
            else if (_argument.Contains("secured"))
            {
                hackDetected = false;
                attackDetected = false;
                if (!booted)
                    GetBlocks();
                SetCondition("green");
            }
            else
            {
                Debug("Unknown Argument: " + _argument);
            }
        }

        void Reset()
        {
            installEnabled = false;
            airVentCnt = 0;
            booted = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////// 
        /*                                                                       INITIALIZATION                                                                           */
        ////////////////////////////////////////////////////////////////////////////////////////////////////////// 

       

        void GetBlocks()
        {
            List<IMyTerminalBlock> _blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(_blocks);
            for (int i = 0; i < _blocks.Count; i++)
            {
                if (_blocks[i].CustomName.Contains(platformID))
                {
                    ///////////////////////// CHECK FOR HACKED BLOCKs
                    if (IsBeingHacked(_blocks[i]))
                    {
                        Debug("->" + _blocks[i].CustomName + " is being hacked!");

                        hackDetected = true;
                    }
                    ///////////////////////// CHECK FOR DAMAGED BLOCKs
                    if (IsBeingAttacked(_blocks[i]))
                    {
                        Debug("->" + _blocks[i].CustomName + " is damaged!");
                        attackDetected = true;
                    }
                    /////////////////////// GETTING GRAV GENs //////////////////////
                    if (_blocks[i].CustomName.Contains(gravID) && _blocks[i].CustomName.Contains(securityID))
                    {
                        gravs.Add((IMyGravityGenerator)_blocks[i]);
                    }
                    /////////////////////// GETTING AIRVENTS //////////////////////
                    if (_blocks[i].CustomName.Contains(airVentID))
                    {
                        IMyAirVent _vent = (IMyAirVent)_blocks[i];
                        airvents.Add(_vent);
                    }
                    ////////////////// GETTING ALERT SPEAKERS ////////////////////
                    if (_blocks[i].CustomName.Contains(speakerID) && _blocks[i].CustomName.Contains(securityID))
                    {
                        alerts.Add((IMySoundBlock)_blocks[i]);
                    }
                    /////////////////// GETTING INTERIOR LIGHTS ////////////////////
                    if (_blocks[i].CustomName.Contains(lightID) && _blocks[i].CustomName.Contains(securityID))
                    {
                        lights.Add((IMyInteriorLight)_blocks[i]);
                    }
                    /////////////////////// GETTING DOORS //////////////////////////
                    if (_blocks[i].CustomName.Contains(doorID))
                    {
                        doors.Add((IMyDoor)_blocks[i]);
                        if (_blocks[i].CustomName.Contains(securityID))
                        {
                            securityDoors.Add((IMyDoor)_blocks[i]);
                        }
                    }
                    ///////////////////// GETTING TEXT PANELS /////////////////////
                    if (_blocks[i].CustomName.Contains(panelID))
                    {
                        IMyTextPanel _panel = (IMyTextPanel)_blocks[i];
                        if (_panel.GetPublicTitle().Contains(debugID))
                        {
                            //debugger.Add(_panel);
                            //debugEnabled = true;
                        }
                        else
                        {
                            if (!_panel.CustomName.Contains(configID) && _panel.GetPublicTitle() == infoID)
                            {
                                _panel.ShowPublicTextOnScreen();
                                _panel.SetValue("FontSize", 0.8f);
                                panels.Add(_panel);
                            }
                        }
                    }
                
                    ////////////////////// GETTING PROGRAMS //////////////////////
                    if (_blocks[i].CustomName.Contains(programID))
                    {
                        IMyProgrammableBlock _program = (IMyProgrammableBlock)_blocks[i];
                        if (_program.CustomName.Contains(coreID))
                            core = _program;
                        else
                            programs.Add(_program);
                    }
                    ///////////////////// GETTING TIMERBLOCKS ////////////////////
                    if (_blocks[i].CustomName.Contains(timerID) && _blocks[i].CustomName.Contains(securityID))
                    {
                        timers.Add((IMyTimerBlock)_blocks[i]);
                    }
                    /////////////////// GETTING GATLING TURRETS /////////////////
                    if (_blocks[i].CustomName.Contains(gatlingID))
                    {
                        gatlings.Add((IMyLargeGatlingTurret)_blocks[i]);
                    }
                    if (_blocks[i].CustomName.Contains(missileID))
                    {
                        missiles.Add((IMyLargeMissileTurret)_blocks[i]);
                    }
                    if (_blocks[i].CustomName.Contains(turretID))
                    {
                        turrets.Add((IMyLargeInteriorTurret)_blocks[i]);
                    }
                }
            }
            booted = true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        /*                                                           EXTERNAL DATA PROCESSING                                                            */
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        void LoadExternalConfigData()
        {
            string _file = config.GetPublicText();
            if (_file.Contains(configDivider.ToString()))
            {
                string[] _data = _file.Split(configDivider);
                //////////////////////// GETTING PLATFORM ID ////////////////////////////
                if (_data.Length > 1)
                    platformID = _data[1].Replace("platformID: ", "").Replace(blank, "");
                if (platformID == "")
                {
                    Debug("<SEC>: No valid platformID found!");
                }
                Debug("PlatformID: " + platformID);
                ///////////////////// GETTING PLATFORM ROLE ID /////////////////////////
                if (_data.Length > 2)
                    platformRoleID = int.Parse(_data[2].Replace("platformRoleID: ", "").Replace(blank, ""));
                Debug("Platform Role ID: " + platformRoleID.ToString());
                //////////////////// GETTING CURRENT CONDITION /////////////////////////
                if (_data.Length > 3)
                    condition = _data[3].Replace("condition: ", "").Replace(blank, "");
                Debug("Condition: " + condition);
                if (_data.Length > 4)
                {
                    ////////////////////// GETTING INSTALL ENABLED /////////////////////////
                    if (_data[4].ToLower().Contains("true") || _data[4].ToLower().Contains("yes"))
                        installEnabled = true;
                    else
                        installEnabled = false;
                    Debug("Install Enabled: " + installEnabled.ToString());
                }
                else    ///////////////////////// CORRECTING FORMAT
                    StoreExternalConfigData();
                if (_data.Length > 5)
                {
                    ///////////////////// GETTING HIDE VERSION INFO ////////////////////////
                    if (_data[5].ToLower().Contains("true") || _data[5].ToLower().Contains("yes"))
                        hideVersionInfoOnScreen = true;
                    else
                        hideVersionInfoOnScreen = false;
                    Debug("Hide Version Info: " + hideVersionInfoOnScreen.ToString());
                }
                else    ///////////////////////// CORRECTING FORMAT
                    StoreExternalConfigData();
                if (_data.Length > 6)
                {
                    /////////////////// GETTING ALLOW BEACON RENAME //////////////////////
                    if (_data[6].ToLower().Contains("true") || _data[6].ToLower().Contains("yes"))
                        allowBeaconRename = true;
                    else
                        allowBeaconRename = false;
                    Debug("Allow beacon rename: " + allowBeaconRename.ToString());
                }
                else    ///////////////////////// CORRECTING FORMAT
                    StoreExternalConfigData();
                if (_data.Length > 7)
                {
                    /////////////////// GETTING SECURITY ACTIVE //////////////////////
                    if (_data[7].ToLower().Contains("true") || _data[7].ToLower().Contains("yes"))
                        securityActive = true;
                    else
                        securityActive = false;
                    Debug("activate security: " + securityActive.ToString());
                }
                else    ///////////////////////// CORRECTING FORMAT
                    StoreExternalConfigData();
                if (_data.Length > 9)
                {
                    ///////////////////////// GET DEFAULT PANEL BG COLOR
                    string[] _temp = _data[8].Split('}');
                    _temp[0] = _temp[0].Replace("LCD BG Color: {", "");
                    string[] _colors = _temp[0].Split(' ');
                    _colors[0] = _colors[0].Replace("R:", "");
                    _colors[1] = _colors[1].Replace("G:", "");
                    _colors[2] = _colors[2].Replace("B:", "");
                    panelDefaultBG = new Color(int.Parse(_colors[0]), int.Parse(_colors[1]), int.Parse(_colors[2]));
                    ///////////////////////// GET DEFAULT PANEL FONT COLOR
                    _temp = _data[9].Split('}');
                    _temp[0] = _temp[0].Replace("LCD Font Color: {", "");
                    _colors = _temp[0].Split(' ');
                    _colors[0] = _colors[0].Replace("R:", "");
                    _colors[1] = _colors[1].Replace("G:", "");
                    _colors[2] = _colors[2].Replace("B:", "");
                    panelDefaultFC = new Color(int.Parse(_colors[0]), int.Parse(_colors[1]), int.Parse(_colors[2]));
                }
                else    ///////////////////////// CORRECTING FORMAT
                    StoreExternalConfigData();
                // StoreExternalConfigData ();
            }
            else
            {
                ///////////////////////// CORRECTING FORMAT
                StoreExternalConfigData();
            }
            SetCondition(condition);
        }

        void StoreExternalConfigData()
        {
            string _file = "$platformID: " + platformID + blank;
            _file += "$programID: " + programID + blank;
            _file += "$platformRoleID: " + platformRoleID.ToString() + blank;
            _file += "$condition: " + condition + blank;
            _file += "$install enabled: " + installEnabled.ToString() + blank;
            _file += "$activate security: " + securityActive.ToString() + blank;
            _file += "$edi Debug: " + debugEnabled.ToString() + blank;
            config.WritePublicText(_file);
        }

        void Display()
        {
            string _info = GetVersionInfo();
            if (hideVersionInfoOnScreen)
                _info = "";
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].WritePublicText(_info);
            }
        }

        string GetAirVentID(string _airVentName)
        {
            string[] _temp = _airVentName.Split(divider);
            _temp[2] = _temp[2].Substring(0, 5).Replace("$", "");
            return _temp[2];
        }

        bool CheckPressureState()
        {
            bool _found = false;
            for (int i = 0; i < airvents.Count; i++)
            {
                if (airvents[i].CustomName.Contains(platformID))
                {
                    if (!airvents[i].IsPressurized())
                    {
                        CloseSecurityDoors(GetAirVentID(airvents[i].CustomName));
                        _found = true;
                    }
                }
            }
            return _found;
        }

        void CloseSecurityDoors()
        {
            for (int i = 0; i < doors.Count; i++)
            {
                doors[i].GetActionWithName("Open_Off").Apply(doors[i]);
            }
        }

        void CloseSecurityDoors(string _id)
        {
            for (int i = 0; i < doors.Count; i++)
            {
                if (doors[i].CustomName.Contains(_id))
                {
                    doors[i].GetActionWithName("Open_Off").Apply(doors[i]);
                }
            }
        }

        void StartTimers()
        {
            for (int i = 0; i < timers.Count; i++)
            {
                timers[i].GetActionWithName("Start").Apply(timers[i]);
            }
        }

        void StopTimers()
        {
            for (int i = 0; i < timers.Count; i++)
            {
                timers[i].GetActionWithName("Stop").Apply(timers[i]);
            }
        }

        void ShutDownGravity()
        {
            for (int i = 0; i < gravs.Count; i++)
            {
                gravs[i].GetActionWithName("OnOff_Off").Apply(gravs[i]);
            }
        }

        void ActivateGravity()
        {
            for (int i = 0; i < gravs.Count; i++)
            {
                gravs[i].GetActionWithName("OnOff_On").Apply(gravs[i]);
            }
        }

        void ActivateTurrets()
        {
            for (int i = 0; i < turrets.Count; i++)
            {
                turrets[i].GetActionWithName("OnOff_On").Apply(turrets[i]);
            }
            for (int i = 0; i < gatlings.Count; i++)
            {
                gatlings[i].GetActionWithName("OnOff_On").Apply(gatlings[i]);
            }
            for (int i = 0; i < missiles.Count; i++)
            {
                missiles[i].GetActionWithName("OnOff_On").Apply(missiles[i]);
            }
        }

        void DeactivateTurrets()
        {
            Debug("Deactivating turrets.");
            for (int i = 0; i < turrets.Count; i++)
            {
                turrets[i].GetActionWithName("OnOff_Off").Apply(turrets[i]);
            }
            for (int i = 0; i < gatlings.Count; i++)
            {
                gatlings[i].GetActionWithName("OnOff_Off").Apply(gatlings[i]);
            }
            for (int i = 0; i < missiles.Count; i++)
            {
                missiles[i].GetActionWithName("OnOff_Off").Apply(missiles[i]);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        /*                                                               CONDITION MANAGEMENT                                                                */
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        void SetCondition(string _condition)
        {
            Debug("New Condition: [" + _condition + "]");
            switch (_condition)
            {
                case "green":
                    condition = _condition;
                    StopAlerts();
                    ActivateGravity();
                    StopBlinking();
                    StopTimers();
                    DeactivateTurrets();
                    StoreExternalConfigData();
                    break;
                case "orange":
                    condition = _condition;
                    CloseSecurityDoors();
                    StopAlerts();
                    StopTimers();
                    StartBlinking();
                    ActivateGravity();
                    StoreExternalConfigData();
                    break;
                case "red":
                    condition = _condition;
                    PlayAlerts();
                    CloseSecurityDoors();
                    StartTimers();
                    StartBlinking();
                    ShutDownGravity();
                    ActivateTurrets();
                    StoreExternalConfigData();
                    break;
                default:
                    break;
            }
        }

        void StartBlinking()
        {
            for (int i = 0; i < lights.Count; i++)
            {
                lights[i].GetActionWithName("OnOff_On").Apply(lights[i]);
            }
        }

        void StopBlinking()
        {
            for (int i = 0; i < lights.Count; i++)
            {
                lights[i].GetActionWithName("OnOff_Off").Apply(lights[i]);
            }
        }

        void PlayAlerts()
        {
            for (int i = 0; i < alerts.Count; i++)
            {
                alerts[i].GetActionWithName("OnOff_On").Apply(alerts[i]);
                alerts[i].GetActionWithName("PlaySound").Apply(alerts[i]);
            }
        }

        void StopAlerts()
        {
            for (int i = 0; i < alerts.Count; i++)
            {
                alerts[i].GetActionWithName("StopSound").Apply(alerts[i]);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        /*                                                                          DEBUGGER                                                                              */
        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        void Debug(string _msg)
        {
            debugMessages.Add(GetTimeStamp() + ": " + _msg + blank);
        }

        void DisplayDebug()
        {
            try
            {
                for (int i = 0; i < debugger.Count; i++)
                {
                    if (debugger[i] != null)
                    {
                        debugger[i].ShowPublicTextOnScreen();
                        debugger[i].SetValue("BackgroundColor", new Color(0, 0, 255));
                        debugger[i].SetValue("FontColor", new Color(255, 255, 0));
                        debugger[i].SetValue("FontSize", 0.6f);
                        debugger[i].WritePublicText(GetDebugInfo());
                    }
                    else
                        throw new ArgumentNullException("Missing Block!");
                }
            }
            catch (NullReferenceException ex)
            {
                Debug(ex.ToString());
            }
        }

    }
}
